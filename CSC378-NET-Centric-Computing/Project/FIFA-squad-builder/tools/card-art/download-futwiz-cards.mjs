import fs from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";
import mysql from "mysql2/promise";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const repoRoot = path.resolve(__dirname, "..", "..");
const appRoot = path.join(repoRoot, "src", "FifaSquadBuilder");
const outputDir = path.join(appRoot, "wwwroot", "player-cards");
const manifestPath = path.join(__dirname, "cards.json");
const appsettingsPath = path.join(appRoot, "appsettings.Development.json");

const allowedHosts = new Set(["cdn.futwiz.com", "www.futwiz.com"]);
const allowedExtensions = new Set([".png", ".webp", ".jpg", ".jpeg"]);

function parseAspNetConnectionString(value) {
  const result = {};
  for (const part of value.split(";")) {
    const [rawKey, ...rawValue] = part.split("=");
    if (!rawKey || rawValue.length === 0) continue;
    result[rawKey.trim().toLowerCase()] = rawValue.join("=").trim();
  }

  return {
    host: result.server || result.host || "localhost",
    port: Number(result.port || 3306),
    user: result.user || result.uid || "root",
    password: result.password || result.pwd || "",
    database: result.database,
  };
}

function slugify(value) {
  return String(value)
    .normalize("NFKD")
    .replace(/[\u0300-\u036f]/g, "")
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "")
    .slice(0, 80) || "player";
}

function validateImageUrl(rawUrl) {
  const parsed = new URL(rawUrl);
  if (parsed.protocol !== "https:") {
    throw new Error(`Only https URLs are allowed: ${rawUrl}`);
  }
  if (!allowedHosts.has(parsed.hostname)) {
    throw new Error(`Only FUTWIZ image hosts are allowed: ${rawUrl}`);
  }

  const extension = path.extname(parsed.pathname).toLowerCase();
  if (!allowedExtensions.has(extension)) {
    throw new Error(`URL must end in an image extension (${[...allowedExtensions].join(", ")}): ${rawUrl}`);
  }

  return { parsed, extension };
}

async function readJson(filePath) {
  return JSON.parse(await fs.readFile(filePath, "utf8"));
}

async function downloadImage(imageUrl, destinationPath) {
  const response = await fetch(imageUrl, {
    headers: {
      "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
      "Accept": "image/avif,image/webp,image/apng,image/svg+xml,image/*,*/*;q=0.8",
    },
  });

  if (!response.ok) {
    throw new Error(`HTTP ${response.status} while downloading ${imageUrl}`);
  }

  const contentType = response.headers.get("content-type") || "";
  if (!contentType.startsWith("image/")) {
    throw new Error(`Expected image content, got '${contentType}' from ${imageUrl}`);
  }

  const bytes = Buffer.from(await response.arrayBuffer());
  await fs.writeFile(destinationPath, bytes);
}

async function updatePlayerCardUrl(connection, item, publicPath) {
  if (item.sourceId != null) {
    const [result] = await connection.execute(
      "UPDATE Players SET CardImageUrl = ? WHERE SourceId = ?",
      [publicPath, item.sourceId],
    );
    return result.affectedRows;
  }

  if (item.name) {
    const [result] = await connection.execute(
      "UPDATE Players SET CardImageUrl = ? WHERE Name = ?",
      [publicPath, item.name],
    );
    return result.affectedRows;
  }

  throw new Error("Each manifest item needs sourceId or name.");
}

async function main() {
  const manifest = await readJson(manifestPath);
  const appsettings = await readJson(appsettingsPath);
  const connectionString = appsettings.ConnectionStrings?.DefaultConnection;
  if (!connectionString) {
    throw new Error(`Missing ConnectionStrings:DefaultConnection in ${appsettingsPath}`);
  }

  await fs.mkdir(outputDir, { recursive: true });
  const connection = await mysql.createConnection(parseAspNetConnectionString(connectionString));

  try {
    for (const item of manifest) {
      const { extension } = validateImageUrl(item.imageUrl);
      const idPart = item.sourceId != null ? String(item.sourceId) : "name";
      const namePart = slugify(item.name || item.sourceId);
      const fileName = `${idPart}-${namePart}${extension}`;
      const outputPath = path.join(outputDir, fileName);
      const publicPath = `/player-cards/${fileName}`;

      console.log(`Downloading ${item.name || item.sourceId} -> ${publicPath}`);
      await downloadImage(item.imageUrl, outputPath);

      const affectedRows = await updatePlayerCardUrl(connection, item, publicPath);
      if (affectedRows === 0) {
        console.warn(`No matching player found for ${item.name || item.sourceId}. Image saved, DB not updated.`);
      } else {
        console.log(`Updated ${affectedRows} player row(s).`);
      }
    }
  } finally {
    await connection.end();
  }
}

main().catch((error) => {
  console.error(error.message);
  process.exitCode = 1;
});

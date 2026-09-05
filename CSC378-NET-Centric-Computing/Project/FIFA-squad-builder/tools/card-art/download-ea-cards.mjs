import fs from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";
import mysql from "mysql2/promise";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const repoRoot = path.resolve(__dirname, "..", "..");
const appRoot = path.join(repoRoot, "src", "FifaSquadBuilder");
const outputRoot = path.join(appRoot, "wwwroot", "player-cards", "ea");
const appsettingsPath = path.join(appRoot, "appsettings.Development.json");

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

function getArg(name, fallback = undefined) {
  const index = process.argv.indexOf(name);
  if (index === -1) return fallback;
  return process.argv[index + 1] ?? fallback;
}

function parseGames() {
  return String(getArg("--games", getArg("--game", "FC25,FC24,FC26")))
    .split(",")
    .map((value) => value.trim().toUpperCase())
    .filter(Boolean);
}

function hasArg(name) {
  return process.argv.includes(name);
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

async function readJson(filePath) {
  return JSON.parse(await fs.readFile(filePath, "utf8"));
}

async function fileExists(filePath) {
  try {
    await fs.access(filePath);
    return true;
  } catch {
    return false;
  }
}

async function downloadImage(imageUrl, destinationPath) {
  const response = await fetch(imageUrl, {
    headers: {
      "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
      "Accept": "image/avif,image/webp,image/apng,image/*,*/*;q=0.8",
      "Referer": "https://www.ea.com/",
    },
  });

  if (!response.ok) {
    throw new Error(`HTTP ${response.status}`);
  }

  const contentType = response.headers.get("content-type") || "";
  if (!contentType.startsWith("image/")) {
    throw new Error(`Expected image content, got '${contentType || "unknown"}'`);
  }

  const bytes = Buffer.from(await response.arrayBuffer());
  if (bytes.length < 1000) {
    throw new Error(`Image response was too small (${bytes.length} bytes)`);
  }

  await fs.writeFile(destinationPath, bytes);
}

async function selectPlayers(connection, limit, includeSquadOnly, sourceIds) {
  if (sourceIds.length > 0) {
    const placeholders = sourceIds.map(() => "?").join(", ");
    const [rows] = await connection.execute(
      `SELECT SourceId, Name
       FROM Players
       WHERE SourceId IN (${placeholders})
       ORDER BY Overall DESC, Name`,
      sourceIds,
    );
    return rows;
  }

  if (includeSquadOnly) {
    const [rows] = await connection.execute(
      `SELECT DISTINCT p.SourceId, p.Name
       FROM Players p
       INNER JOIN SquadPlayers sp ON sp.PlayerId = p.Id
       WHERE p.SourceId IS NOT NULL
       ORDER BY p.Overall DESC, p.Name`,
    );
    return rows;
  }

  const [rows] = await connection.execute(
    `SELECT SourceId, Name
     FROM Players
     WHERE SourceId IS NOT NULL
     ORDER BY Overall DESC, Name
     LIMIT ?`,
    [limit],
  );
  return rows;
}

async function updatePlayerCardUrl(connection, sourceId, publicPath) {
  const [result] = await connection.execute(
    "UPDATE Players SET CardImageUrl = ? WHERE SourceId = ?",
    [publicPath, sourceId],
  );
  return result.affectedRows;
}

async function processPlayer(connection, player, games, width, force) {
  const sourceId = Number(player.SourceId);
  const name = player.Name || String(sourceId);

  for (const game of games) {
    const gameFolder = game.toLowerCase();
    const outputDir = path.join(outputRoot, gameFolder);
    await fs.mkdir(outputDir, { recursive: true });

    const fileName = `${sourceId}-${slugify(name)}.png`;
    const outputPath = path.join(outputDir, fileName);
    const publicPath = `/player-cards/ea/${gameFolder}/${fileName}`;
    const imageUrl = `https://ratings-images-prod.pulse.ea.com/${game}/full/player-shields/en/${sourceId}.png?width=${width}`;

    try {
      if (!force && await fileExists(outputPath)) {
        const affectedRows = await updatePlayerCardUrl(connection, sourceId, publicPath);
        return { status: "cached", name, sourceId, publicPath, affectedRows, game };
      }

      await downloadImage(imageUrl, outputPath);
      const affectedRows = await updatePlayerCardUrl(connection, sourceId, publicPath);
      return { status: "saved", name, sourceId, publicPath, affectedRows, game };
    } catch {
      // Try the next EA FC version. Some older players do not have shields in
      // every ratings archive even when their SourceId exists in our dataset.
    }
  }

  return { status: "failed", name, sourceId };
}

async function runPool(items, concurrency, worker) {
  let nextIndex = 0;
  const workers = Array.from({ length: concurrency }, async () => {
    while (nextIndex < items.length) {
      const item = items[nextIndex];
      nextIndex += 1;
      await worker(item);
    }
  });
  await Promise.all(workers);
}

function parseSourceIds() {
  const raw = getArg("--source-ids", "");
  return raw
    .split(",")
    .map((value) => Number(value.trim()))
    .filter((value) => Number.isInteger(value) && value > 0);
}

async function main() {
  const games = parseGames();
  const width = Number(getArg("--width", "265"));
  const limit = Number(getArg("--top", "250"));
  const concurrency = Number(getArg("--concurrency", "6"));
  const force = hasArg("--force");
  const includeSquadOnly = hasArg("--squad");
  const all = hasArg("--all");
  const sourceIds = parseSourceIds();

  if (games.length === 0 || games.some((game) => !/^FC\d{2}$/.test(game))) {
    throw new Error("Use game values like FC24, FC25, or FC26.");
  }
  if (!Number.isInteger(width) || width < 120 || width > 1024) {
    throw new Error("--width must be an integer from 120 to 1024.");
  }
  if (!Number.isInteger(concurrency) || concurrency < 1 || concurrency > 12) {
    throw new Error("--concurrency must be an integer from 1 to 12.");
  }
  if (!all && sourceIds.length === 0 && !includeSquadOnly && (!Number.isInteger(limit) || limit < 1)) {
    throw new Error("--top must be a positive integer.");
  }

  const appsettings = await readJson(appsettingsPath);
  const connectionString = appsettings.ConnectionStrings?.DefaultConnection;
  if (!connectionString) {
    throw new Error(`Missing ConnectionStrings:DefaultConnection in ${appsettingsPath}`);
  }

  const connection = await mysql.createConnection(parseAspNetConnectionString(connectionString));
  const summary = { downloaded: 0, cached: 0, updated: 0, failed: 0 };

  try {
    const players = await selectPlayers(connection, all ? 1000000 : limit, includeSquadOnly, sourceIds);
    if (players.length === 0) {
      throw new Error("No matching players found in the database.");
    }

    console.log(`Trying ${games.join(", ")}. Processing ${players.length} player(s), concurrency=${concurrency}.`);

    await runPool(players, concurrency, async (player) => {
      const result = await processPlayer(connection, player, games, width, force);
      if (result.status === "saved") {
        summary.downloaded += 1;
        summary.updated += result.affectedRows;
        console.log(`[saved:${result.game}] ${result.name} -> ${result.publicPath}`);
      } else if (result.status === "cached") {
        summary.cached += 1;
        summary.updated += result.affectedRows;
        console.log(`[cached:${result.game}] ${result.name} -> ${result.publicPath}`);
      } else {
        summary.failed += 1;
        console.warn(`[skip] ${result.name} (${result.sourceId}): no shield found in ${games.join(", ")}`);
      }
    });
  } finally {
    await connection.end();
  }

  console.log("");
  console.log(`Done. downloaded=${summary.downloaded}, cached=${summary.cached}, updated=${summary.updated}, failed=${summary.failed}`);
  if (summary.failed > 0) {
    console.log("Some players have no public EA shield image in the selected game versions.");
  }
}

main().catch((error) => {
  console.error(error.message);
  process.exitCode = 1;
});

(function () {
    "use strict";

    const squadId = document.getElementById("sb-data").dataset.squadId;
    const antiforgeryToken = document.querySelector('#sb-antiforgery input[name="__RequestVerificationToken"]').value;

    const modalBackdrop = document.getElementById("sb-modal-backdrop");
    const modalTitle = document.getElementById("sb-modal-title");
    const resultsEl = document.getElementById("sb-modal-results");
    const errorBanner = document.getElementById("sb-error-banner");
    const filterForm = document.getElementById("sb-modal-filters-form");

    let currentTargetSlotId = null; // null = bench

    function showError(message) {
        errorBanner.textContent = message;
        errorBanner.classList.add("show");
    }

    function hideError() {
        errorBanner.classList.remove("show");
    }

    function openPicker(formationPositionId, defaultPositionId) {
        currentTargetSlotId = formationPositionId; // null for bench
        hideError();
        modalTitle.textContent = formationPositionId ? "Select player for this position" : "Add player to bench";
        filterForm.reset();
        if (defaultPositionId) {
            filterForm.querySelector('[name="PositionId"]').value = defaultPositionId;
        }
        modalBackdrop.classList.add("open");
        runSearch();
    }

    function closePicker() {
        modalBackdrop.classList.remove("open");
    }

    async function runSearch() {
        const params = new URLSearchParams(new FormData(filterForm));
        params.set("PageSize", "30");
        resultsEl.innerHTML = '<p class="text-muted">Searching...</p>';

        try {
            const resp = await fetch(`/Squads/PlayerSearch?${params.toString()}`);
            if (!resp.ok) throw new Error("Search failed");
            const data = await resp.json();
            renderResults(data.items || []);
        } catch (err) {
            resultsEl.innerHTML = '<p class="text-danger">Search failed. Try again.</p>';
        }
    }

    function renderResults(items) {
        if (items.length === 0) {
            resultsEl.innerHTML = '<p class="text-muted">No players match these filters.</p>';
            return;
        }
        resultsEl.innerHTML = "";
        for (const p of items) {
            const row = document.createElement("div");
            row.className = "sb-result-row";
            row.innerHTML = `
                <div>
                    <div class="sb-result-name">${escapeHtml(p.name)} <span class="sb-result-meta">(${p.positionCode}, age ${p.age})</span></div>
                    <div class="sb-result-meta">${escapeHtml(p.clubName || "Free agent")} - ${escapeHtml(p.nationName)} - &euro;${Number(p.valueEUR).toLocaleString()}</div>
                </div>
                <div class="sb-result-ovr">${p.overall}</div>
            `;
            row.addEventListener("click", () => assignPlayer(p.id));
            resultsEl.appendChild(row);
        }
    }

    function escapeHtml(str) {
        const div = document.createElement("div");
        div.textContent = str ?? "";
        return div.innerHTML;
    }

    async function postForm(url, fields) {
        const body = new URLSearchParams(fields);
        body.set("__RequestVerificationToken", antiforgeryToken);
        const resp = await fetch(url, {
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            body: body.toString(),
        });
        return resp.json();
    }

    async function assignPlayer(playerId) {
        hideError();
        const fields = { squadId, playerId };
        if (currentTargetSlotId) fields.formationPositionId = currentTargetSlotId;
        try {
            const result = await postForm("/Squads/AssignPlayer", fields);
            if (result.success) {
                window.location.reload();
            } else {
                showError(result.error || "Could not assign this player.");
            }
        } catch (err) {
            showError("Something went wrong - please try again.");
        }
    }

    async function removePlayer(squadPlayerId) {
        hideError();
        try {
            const result = await postForm("/Squads/RemovePlayer", { squadId, squadPlayerId });
            if (result.success) {
                window.location.reload();
            } else {
                showError(result.error || "Could not remove this player.");
            }
        } catch (err) {
            showError("Something went wrong - please try again.");
        }
    }

    document.querySelectorAll(".sb-slot[data-formation-position-id]").forEach((el) => {
        el.addEventListener("click", () => {
            const fpId = el.dataset.formationPositionId;
            const posId = el.dataset.positionId;
            openPicker(fpId, posId);
        });
    });

    document.querySelectorAll(".sb-bench-add").forEach((el) => {
        el.addEventListener("click", () => openPicker(null, null));
    });

    document.querySelectorAll(".sb-remove-btn").forEach((el) => {
        el.addEventListener("click", (e) => {
            e.stopPropagation();
            removePlayer(el.dataset.squadPlayerId);
        });
    });

    document.getElementById("sb-modal-close").addEventListener("click", closePicker);
    modalBackdrop.addEventListener("click", (e) => {
        if (e.target === modalBackdrop) closePicker();
    });
    filterForm.addEventListener("submit", (e) => {
        e.preventDefault();
        runSearch();
    });
    filterForm.addEventListener("input", debounce(runSearch, 350));

    function debounce(fn, ms) {
        let t;
        return (...args) => {
            clearTimeout(t);
            t = setTimeout(() => fn(...args), ms);
        };
    }
})();

// STOL Training - in-sim status display.
//
// Polls the STOL Training Tool desktop app's local status server
// (Core/PanelServer.cs) over plain HTTP and renders the same aligned-on-line
// indicator, takeoff/landing distances, and wind shown in the desktop app.

var STATUS_URL = "http://127.0.0.1:7865/status"; // 7865 = "STOL" on a phone keypad
var POLL_MS = 400;
var HEADER_BRAND_TEXT = "STOL Training";

var el = {
    headerLogo: document.getElementById("headerLogo"),
    clockValue: document.getElementById("clockValue"),
    alignedBar: document.getElementById("alignedBar"),
    takeoffValue: document.getElementById("takeoffValue"),
    touchdownValue: document.getElementById("touchdownValue"),
    stoppingValue: document.getElementById("stoppingValue"),
    landingValue: document.getElementById("landingValue"),
    scoreValue: document.getElementById("scoreValue"),
    windValue: document.getElementById("windValue"),
    windArrow: document.getElementById("windArrow"),
    aglValue: document.getElementById("aglValue"),
    ldgRateValue: document.getElementById("ldgRateValue"),
    remarksValue: document.getElementById("remarksValue"),
    connStatus: document.getElementById("connStatus"),
};

function formatClock(totalSeconds) {
    var s = Math.max(0, Math.floor(totalSeconds || 0));
    var m = Math.floor(s / 60);
    var r = s % 60;
    return (m < 10 ? "0" + m : m) + ":" + (r < 10 ? "0" + r : r);
}

function pickTextColor(hex) {
    if (!hex || hex[0] !== "#" || hex.length < 7) return "#000000";
    var r = parseInt(hex.substr(1, 2), 16);
    var g = parseInt(hex.substr(3, 2), 16);
    var b = parseInt(hex.substr(5, 2), 16);
    var luma = 0.2126 * r + 0.7152 * g + 0.0722 * b;
    return luma > 140 ? "#000000" : "#ffffff";
}

function render(data) {
    var connected = !!data.connected;

    if (el.connStatus) {
        el.connStatus.textContent = "Sim not connected";
        el.connStatus.classList.toggle("hidden", connected);
    }

    if (el.headerLogo) {
        // state stays "Unknown" server-side until a reference position is set
        // (see PanelStatus.State / Controller's stol.IsInit() gate) - keep the
        // brand text until then rather than showing that (too long for the
        // header) or a raw "Unknown".
        var hasReference = !!data.state && data.state !== "Unknown";
        el.headerLogo.textContent = hasReference ? data.state : HEADER_BRAND_TEXT;
    }

    if (el.alignedBar) {
        // Plain ASCII fallback only - Coherent GT's bundled font is missing
        // glyphs for characters like an em dash, which render as a "missing
        // character" box instead (seen while airborne, when the aligned text
        // from the desktop app is empty).
        var text = (data.aligned && data.aligned.text) ? data.aligned.text : "--";
        var color = (data.aligned && data.aligned.color) ? data.aligned.color : "#808080";
        el.alignedBar.textContent = text;
        el.alignedBar.style.backgroundColor = color;
        el.alignedBar.style.color = pickTextColor(color);
    }

    if (el.clockValue) {
        el.clockValue.textContent = formatClock(data.elapsedSeconds);
        el.clockValue.classList.toggle("running", !!data.timerRunning);
    }

    var unit = data.unit === "meters" ? "m" : (data.unit === "yard" ? "yd" : "ft");

    if (el.takeoffValue) {
        el.takeoffValue.textContent = data.hasTakeoff
            ? Math.round(data.takeoffDistance) + " " + unit
            : "--";
    }

    if (el.touchdownValue) {
        el.touchdownValue.textContent = data.hasTouchdown
            ? Math.round(data.touchdownDistance) + " " + unit
            : "--";
    }

    if (el.stoppingValue) {
        el.stoppingValue.textContent = data.hasLanding
            ? Math.round(data.stoppingDistance) + " " + unit
            : "--";
    }

    if (el.landingValue) {
        el.landingValue.textContent = data.hasLanding
            ? Math.round(data.landingDistance) + " " + unit
            : "--";
    }

    if (el.scoreValue) {
        // The outcome is already locked in the moment it happens (touching down
        // on/past the line, or a prop strike, can't be undone by anything that
        // happens afterward), so show it immediately rather than waiting for
        // the full stop. Prop strike takes priority if somehow both occur.
        var isPropStrike = !!data.isPropStrike;
        var isScratch = !!(data.hasTouchdown && data.isScratch);
        var isAlert = isPropStrike || isScratch;
        if (isPropStrike) {
            el.scoreValue.textContent = "PROP STRIKE!";
        } else if (isScratch) {
            el.scoreValue.textContent = "SCRATCH!";
        } else {
            el.scoreValue.textContent = data.hasLanding
                ? Math.round(data.score) + " " + unit
                : "--";
        }
        el.scoreValue.classList.toggle("scratch", isAlert);
        if (el.scoreValue.parentElement) {
            el.scoreValue.parentElement.classList.toggle("scratch", isAlert);
        }
    }

    if (el.ldgRateValue) {
        el.ldgRateValue.textContent = data.hasTouchdown
            ? Math.round(data.landingRateFpm) + " fpm"
            : "-- fpm";
    }

    if (el.remarksValue) {
        while (el.remarksValue.firstChild) {
            el.remarksValue.removeChild(el.remarksValue.firstChild);
        }
        if (data.remarks && data.remarks.length) {
            for (var i = 0; i < data.remarks.length; i++) {
                var chip = document.createElement("span");
                chip.className = "remarkChip sev" + (data.remarks[i].severity || 0);
                chip.textContent = data.remarks[i].type;
                el.remarksValue.appendChild(chip);
            }
        } else {
            el.remarksValue.textContent = "--";
        }
    }

    if (el.aglValue && typeof data.aglFt === "number") {
        el.aglValue.textContent = Math.round(data.aglFt) + " ft";
    }

    if (data.wind) {
        if (el.windValue) {
            el.windValue.textContent = data.wind.speedKt.toFixed(1) + " kt";
        }
        if (el.windArrow) {
            // Matches the desktop app's panelWind_Paint exactly: windDirTo =
            // (relativeDirDeg + 180) % 360, then the arrow is drawn from a
            // "start" point to an "end" point computed via
            // cos/sin(windDirTo - 90) with the start/end swapped - working
            // through that algebra, the arrowhead's actual compass bearing
            // works out to (180 - relativeDirDeg), not (relativeDirDeg + 180).
            // Those only coincide for pure head/tailwind (0/180) - for any
            // crosswind component the previous formula pointed the arrow
            // mirrored left/right versus the desktop widget.
            var towardsDeg = (180 - data.wind.relativeDirDeg + 360) % 360;
            el.windArrow.setAttribute("transform", "rotate(" + towardsDeg + " 32 32)");
        }
    }
}

function renderDisconnected() {
    if (el.connStatus) {
        el.connStatus.textContent = "STOL Training Tool app not running";
        el.connStatus.classList.remove("hidden");
    }
    if (el.headerLogo) el.headerLogo.textContent = HEADER_BRAND_TEXT;
    if (el.alignedBar) {
        el.alignedBar.textContent = "App not running";
        el.alignedBar.style.backgroundColor = "#808080";
        el.alignedBar.style.color = "#ffffff";
    }
    if (el.clockValue) {
        el.clockValue.textContent = "00:00";
        el.clockValue.classList.remove("running");
    }
    if (el.takeoffValue) el.takeoffValue.textContent = "--";
    if (el.touchdownValue) el.touchdownValue.textContent = "--";
    if (el.stoppingValue) el.stoppingValue.textContent = "--";
    if (el.landingValue) el.landingValue.textContent = "--";
    if (el.scoreValue) {
        el.scoreValue.textContent = "--";
        el.scoreValue.classList.remove("scratch");
        if (el.scoreValue.parentElement) {
            el.scoreValue.parentElement.classList.remove("scratch");
        }
    }
    if (el.windValue) el.windValue.textContent = "-- kt";
    if (el.aglValue) el.aglValue.textContent = "-- ft";
    if (el.ldgRateValue) el.ldgRateValue.textContent = "-- fpm";
    if (el.remarksValue) el.remarksValue.textContent = "--";
}

function poll() {
    fetch(STATUS_URL, { cache: "no-store" }).then(
        function (response) {
            if (!response.ok) throw new Error("status " + response.status);
            return response.json();
        },
        function (err) {
            throw err;
        }
    ).then(
        function (data) {
            render(data);
            setTimeout(poll, POLL_MS);
        },
        function () {
            renderDisconnected();
            setTimeout(poll, POLL_MS);
        }
    );
}

// Zoom control - identical technique to the VPForce TelemFFB panel: 'zoom'
// (not transform: scale) so layout/scroll extent actually reflows to match.
var SCALE_STEPS = [50, 75, 100, 125, 150, 175, 200, 250, 300];

function initScaleControl() {
    var label = document.getElementById("scaleLabel");
    var index = 2; // SCALE_STEPS[2] === 100%

    function apply() {
        var pct = SCALE_STEPS[index];
        label.textContent = pct + "%";
        document.body.style.zoom = pct / 100;
    }

    document.getElementById("scaleDown").addEventListener("click", function () {
        if (index > 0) {
            index -= 1;
            apply();
        }
    });
    document.getElementById("scaleUp").addEventListener("click", function () {
        if (index < SCALE_STEPS.length - 1) {
            index += 1;
            apply();
        }
    });
}

initScaleControl();
poll();

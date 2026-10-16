// Toolbar chrome for the STOL Training panel.
//
// This file only owns the panel window (show/hide/resize via ingame-ui).
// The actual live display lives in panel.html, loaded into the iframe below
// and polling the STOL Training Tool desktop app's local status server -
// see panel.js. Keeping this outer shell minimal matters: content-fit="true"
// measures whatever is placed directly inside <ingame-ui>, and real content
// there (rows, tiles, svg, etc.) made the panel balloon to fill the screen
// and become unresizable. An iframe has no such effect on the outer frame.
class IngamePanelStolTraining extends TemplateElement {
    constructor() {
        super(...arguments);

        this.panelActive = false;
        this.started = false;
        this.ingameUi = null;

        this.initialize();
    }

    connectedCallback() {
        super.connectedCallback();

        var self = this;
        this.ingameUi = this.querySelector("ingame-ui");

        this.iframeElement = document.getElementById("STOLTrainingPanelIframe");

        this.m_MainDisplay = document.querySelector("#MainDisplay");
        this.m_MainDisplay.classList.add("hidden");

        this.m_Footer = document.querySelector("#Footer");
        this.m_Footer.classList.add("hidden");

        if (this.ingameUi) {
            this.ingameUi.addEventListener("panelActive", function () {
                self.panelActive = true;
                if (self.iframeElement) {
                    // Path is absolute from the package's html_ui root, same
                    // convention as the /JS, /SCSS, /templates references above.
                    self.iframeElement.src = "/InGamePanels/STOLTrainingPanel/panel.html";
                }
            });
            this.ingameUi.addEventListener("panelInactive", function () {
                self.panelActive = false;
                if (self.iframeElement) {
                    self.iframeElement.src = "";
                }
            });
        }
    }

    initialize() {
        if (this.started) return;
        this.started = true;
    }

    disconnectedCallback() {
        super.disconnectedCallback();
    }
}

window.customElements.define("ingamepanel-stol-training", IngamePanelStolTraining);
checkAutoload();

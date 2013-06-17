using System;
using Awesomium.Core;
using Mogre;

using Game.States;
using System.Windows.Forms;

using API.Generic;

using Game.BaseApp;

namespace Game.GUIs {
    public class InGameMenu : GUI {
        private readonly StateManager mStateMgr;
        private static readonly Vector2 IMAGE_SIZE = new Vector2(500, 450);

        public InGameMenu(StateManager stateMgr)
            : base(((OgreForm.InitSize - IMAGE_SIZE * Cst.GUI_RATIO) / 2) - (Vector2.UNIT_Y * (IMAGE_SIZE * Cst.GUI_RATIO).y / 12), IMAGE_SIZE, "IngameMenu.html") {
            this.mStateMgr = stateMgr;
        }

        public override void onDocumentReady(object sender, UrlEventArgs e) {
            base.onDocumentReady(sender, e);

            JSObject j = OgreForm.webView.CreateGlobalJavascriptObject("IngameMenuObject");
            j.Bind("Menu", false, (sender1, args) => this.mStateMgr.RequestStatePop());
            j.Bind("back", false, HideBack);
            j.Bind("save", false, save);
        }

        private void HideBack(object sender, EventArgs e) {
            
        }

        private void save(object sender, EventArgs e) {
            
        }
    }
}

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

        public static Action Save;

        public InGameMenu(StateManager stateMgr)
            : base((OgreForm.InitSize - IMAGE_SIZE) / 2 - Vector2.UNIT_Y * 40, IMAGE_SIZE / 2.2f, "IngameMenu.html")
        {
            this.mStateMgr = stateMgr;
        }

        public override void onDocumentReady(object sender, UrlEventArgs e) {
            base.onDocumentReady(sender, e);

            JSObject j = OgreForm.webView.CreateGlobalJavascriptObject("IngameMenuObject");
            j.Bind("Menu", false, (sender1, args) => this.mStateMgr.RequestStatePop());
            j.Bind("back", false, (sender1, args) => HideBack());
            j.Bind("save", false, (sender1, args) => Save());
        }

        private void HideBack()
        {
            this.mStateMgr.MainState.User.SwitchGUIVisibility(false);
            GUI.Visible = false;
        }
    }
}

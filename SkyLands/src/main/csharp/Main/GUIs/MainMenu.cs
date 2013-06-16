using System;
using Awesomium.Core;
using Mogre;

using Game.States;
using System.Windows.Forms;


using Game.BaseApp;

namespace Game.GUIs {
    public class MainMenu : GUI {
        private readonly StateManager mStateMgr;

        public MainMenu(StateManager stateMgr)
            : base(new Vector2(0, 0), new Vector2(404, 404), "MainMenu.html", DockStyle.Fill) {
            this.mStateMgr = stateMgr;
        }

        public override void onDocumentReady(object sender, UrlEventArgs e) {
            base.onDocumentReady(sender, e);
            
            JSObject j = OgreForm.webView.CreateGlobalJavascriptObject("MainMenuJSObject");
            j.Bind("exit", false, (sender1, args) => this.mStateMgr.RequestStatePop());
            j.Bind("play", false, HideAndPlay);
        }

        private void HideAndPlay(object sender, EventArgs e) {
            Visible = false;
            new PlayMenu(this.mStateMgr);
        }
    }
}

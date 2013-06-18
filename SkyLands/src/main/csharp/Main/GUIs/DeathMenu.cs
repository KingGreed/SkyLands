using System;
using Awesomium.Core;
using Mogre;

using Game.States;
using System.Windows.Forms;


using Game.BaseApp;

namespace Game.GUIs {
    public class DeathMenu : GUI {
        private readonly StateManager mStateMgr;

        public DeathMenu(StateManager stateMgr)
            : base(new Vector2(0, 0), new Vector2(404, 404), "options.html", DockStyle.Fill) {
            this.mStateMgr = stateMgr;
        }

        public override void onDocumentReady(object sender, UrlEventArgs e) {
            base.onDocumentReady(sender, e);

            JSObject j = OgreForm.webView.CreateGlobalJavascriptObject("DeathObject");
            j.Bind("again", false, exit);
            j.Bind("exit",  false, again);
        }

        private void exit(object sender, EventArgs e) {
            Visible = false;
            new MainMenu(this.mStateMgr);
        }

        private void again(object sender, EventArgs e) {
            
        }

    }
}

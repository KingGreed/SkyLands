using System;
using Awesomium.Core;
using Mogre;

using Game.States;
using System.Windows.Forms;


using Game.BaseApp;

namespace Game.GUIs {
    public class OptionMenu : GUI {
        private readonly StateManager mStateMgr;

        public OptionMenu(StateManager stateMgr)
            : base(new Vector2(0, 0), new Vector2(404, 404), "options.html", DockStyle.Fill) {
            this.mStateMgr = stateMgr;
        }

        public override void onDocumentReady(object sender, UrlEventArgs e) {
            base.onDocumentReady(sender, e);

            JSObject j = OgreForm.webView.CreateGlobalJavascriptObject("OptionObject");
            j.Bind("back",    false, HideAndMain);
            j.Bind("Music",   false, MusicUpdate);
            j.Bind("Quality", false, QualityUpdate);
            j.Bind("Vsync",   false, VsyncUpdate);
        }

        private void HideAndMain(object sender, EventArgs e) {
            Visible = false;
            new MainMenu(this.mStateMgr);
        }

        private void MusicUpdate(object sender, EventArgs e) {
            
        }

        private void QualityUpdate(object sender, EventArgs e) {
            
        }
        private void VsyncUpdate(object sender, EventArgs e) {
            
        }

    }
}

using System;
using Awesomium.Core;
using Mogre;

using Game.States;
using System.Windows.Forms;

using Game.BaseApp;

namespace Game.GUIs {
    public class PlayMenu : GUI {
        private readonly StateManager mStateMgr;
        private string scenario;

        public PlayMenu(StateManager stateMgr, string scenar = "")
            : base(new Vector2(0, 0), new Vector2(404, 404), "play.html", DockStyle.Fill) {
            this.mStateMgr = stateMgr;

            this.scenario = scenar;
        }

        public override void onDocumentReady(object sender, UrlEventArgs e) {
            base.onDocumentReady(sender, e);
            JSObject j = OgreForm.webView.CreateGlobalJavascriptObject("PlayMenuJSObject");
            j.Bind("back", false, HideAndPlay);
            j.Bind("play", false, playGame);
            j.Bind("load", false, Load);
            j.Bind("scenario", false, Scenario);
        }

        private void playGame(object sender, EventArgs e) {

            this.mStateMgr.GameInfo = new World.GameInfo();
            this.mStateMgr.GameInfo.Scenario = this.scenario;
            this.mStateMgr.RequestStatePush(typeof(GameState));
        }

        private void HideAndPlay(object sender, EventArgs e) {
            Visible = false;
            new MainMenu(this.mStateMgr);
        }

        private void Load(object sender, EventArgs e) {
            this.mStateMgr.GameInfo = new World.GameInfo(true);
            this.mStateMgr.RequestStatePush(typeof(GameState));
            
        }

        private void Scenario(object sender, EventArgs e) {
            Visible = false;
            new ScenarioLoader(this.mStateMgr);
        }
    }
}

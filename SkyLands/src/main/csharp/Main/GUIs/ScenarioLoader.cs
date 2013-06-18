using System;
using Awesomium.Core;
using Mogre;

using Game.States;
using System.Windows.Forms;

using Game.BaseApp;
using System.IO;

namespace Game.GUIs {
    public class ScenarioLoader : GUI {
        private readonly StateManager mStateMgr;
        private string ScenarioSelected;

        ///TODO explore for scenarios

        public ScenarioLoader(StateManager stateMgr)
            : base(new Vector2(0, 0), new Vector2(404, 404), "ScenarioEditorChooseScenario.html", DockStyle.Fill) {
            this.mStateMgr = stateMgr;
        }

        public override void onDocumentReady(object sender, UrlEventArgs e) {
            base.onDocumentReady(sender, e);
            JSObject j = OgreForm.webView.CreateGlobalJavascriptObject("ScenarioLoaderObject");
            j.Bind("edit", false, edit);
            j.Bind("new", false, New);
            j.Bind("ok", false, ok);
        }

        private void ok(object sender, EventArgs e) {
            Visible = false;
            new MainMenu(this.mStateMgr);
        }

        private void New(object sender, JavascriptMethodEventArgs e) {

            this.ScenarioSelected = (string)e.Arguments[0];

            var StructuresfileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\" + this.ScenarioSelected + "\\";
            System.IO.Directory.CreateDirectory(StructuresfileName);
            FileStream f = System.IO.File.Create(StructuresfileName + "structures.scenario");
            f.Close();

            Visible = false;
            new StructuresMenu(this.mStateMgr, ScenarioSelected);      
        }

        private void edit(object sender, EventArgs e) {
            
        }

        private void update(object sender, JavascriptMethodEventArgs e) {
            this.ScenarioSelected = (string)e.Arguments[0];
        }
    }
}

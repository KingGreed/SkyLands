using System;
using System.Collections.Generic;
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
            if(j != null) {
                j.Bind("new", false, New);
                j.Bind("edit", false, edit);
                j.Bind("ok", false, ok);
            }

            string[] s = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\scenarios.txt");
            for(int i = 0; i < s.Length; i++) {
                string cmd = "addScenarios(\"" + s[i] + "\")";
                OgreForm.webView.ExecuteJavascript(cmd);
            }
        }

        private void ok(object sender, JavascriptMethodEventArgs e) {
            Visible = false;
            new PlayMenu(this.mStateMgr, (string)e.Arguments[0]);
        }


        private void New(object sender, JavascriptMethodEventArgs e) {

            this.ScenarioSelected = (string)e.Arguments[0];

            var StructuresfileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\" + this.ScenarioSelected + "\\";

            System.IO.Directory.CreateDirectory(StructuresfileName);
            
            StreamWriter s = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\" + this.ScenarioSelected + "\\structures.scenario", true);
            s.Close();

            s = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\scenarios.txt", true);
            s.WriteLine(this.ScenarioSelected);
            s.Close();

            Visible = false;
            new StructuresMenu(this.mStateMgr, this.ScenarioSelected);
        }

        private void edit(object sender, JavascriptMethodEventArgs e) {
            this.ScenarioSelected = (string)e.Arguments[0];
            Visible = false;
            new StructuresMenu(this.mStateMgr, this.ScenarioSelected);

        }

    }
}


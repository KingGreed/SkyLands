using System;
using System.Collections.Generic;
using Awesomium.Core;
using Mogre;
using System.IO;
using System.Drawing;

using Game.States;
using Game.World;
using System.Windows.Forms;

using API.Generic;

using Game.BaseApp;

namespace Game.GUIs {
    public class StructuresMenu : GUI {
        private readonly StateManager mStateMgr;
        private static readonly Vector2 IMAGE_SIZE = new Vector2(450, 900);
        public string scenarioName;
        private List<string> l = new List<string>();
        private List<string> scenarios = new List<string>();

        public StructuresMenu(StateManager stateMgr, string name)
            : base(new Vector2(0, 0), IMAGE_SIZE / 3.3f, "Structures.html") {
            this.mStateMgr = stateMgr;

            this.scenarioName = name;
            this.mStateMgr.StoryInfo.scenario = name;

        }

        public override void onDocumentReady(object sender, UrlEventArgs e) {
            base.onDocumentReady(sender, e);

            if(!OgreForm.webView.IsLive) { return; }

            JSObject j = OgreForm.webView.CreateGlobalJavascriptObject("StructuresObject");
            if(j != null) {
                j.Bind("ok",     false, ok);
                j.Bind("new",    false, New);
                j.Bind("update", false, disp);
            }

            string[] s = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\" + scenarioName + "\\structures.scenario");
            for(int i = 0; i < s.Length; i++) {
                scenarios.Add(s[i]);
                OgreForm.webView.ExecuteJavascript("addStructure(\"" + s[i] + "\")");
            }
        }

        private void disp(object sender, JavascriptMethodEventArgs e) {
            string name = (string)e.Arguments[0];
            this.mStateMgr.StoryInfo.name = name;
            this.mStateMgr.StoryInfo.pathToFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\" + this.scenarioName + "\\" + name;
            this.mStateMgr.RequestStatePush(typeof(StoryEditorState));
        }

        private void ok(object sender, JavascriptMethodEventArgs e) {
            string name = (string)e.Arguments[0];
            this.mStateMgr.StoryInfo.name = name;
            update();

            Visible = false;
            this.mStateMgr.MainState.User.SwitchGUIVisibility(false);
            StoryEditorWorld.sm = null;
        }

        private void Down(object sender, JavascriptMethodEventArgs e) {
            int index = Convert.ToInt32(e.Arguments[0].ToString());

            scenarios.Insert(index - 1, scenarios[index]);
            scenarios.RemoveAt(index + 1);
        }

        private void Up(object sender, JavascriptMethodEventArgs e) {
            int index = Convert.ToInt32(e.Arguments[0].ToString());

            scenarios.Insert(index + 1, scenarios[index]);
            scenarios.RemoveAt(index);
        }

        private void New(object sender, JavascriptMethodEventArgs e) {
            string name = (string)e.Arguments[0];
            this.mStateMgr.StoryInfo.name = name.Split('-')[0];
            this.mStateMgr.StoryInfo.pathToFile = "";

            this.scenarios.Add(name);
            this.update();

            //adding event file

            StreamWriter s = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\" + this.scenarioName + "\\" + this.scenarioName + ".event", true);
            s.Close();

            this.mStateMgr.RequestStatePush(typeof(StoryEditorState));
            Visible = false;
        }

        private void update() {
            StreamWriter f = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\" + scenarioName + "\\structures.scenario");

            for(int i = 0; i < scenarios.Count; i++) {
                f.WriteLine(scenarios[i]);
            }
            f.Close();

        }

    }
}
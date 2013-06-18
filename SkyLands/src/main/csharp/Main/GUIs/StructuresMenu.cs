using System;
using System.Collections.Generic;
using Awesomium.Core;
using Mogre;
using System.IO;
using System.Drawing;

using Game.States;
using System.Windows.Forms;

using API.Generic;

using Game.BaseApp;

namespace Game.GUIs {
    public class StructuresMenu : GUI {
        private readonly StateManager mStateMgr;
        private static readonly Vector2 IMAGE_SIZE = new Vector2(450, 900);
        public string scenarioName;
        private List<string> l = new List<string>();

        public StructuresMenu(StateManager stateMgr, string name)
            : base(new Vector2(0, 0), IMAGE_SIZE / 3.3f, "Structures.html") {
            this.mStateMgr = stateMgr;

            this.add(name);

        }

        public override void onDocumentReady(object sender, UrlEventArgs e) {
            base.onDocumentReady(sender, e);

            if(!OgreForm.webView.IsLive) { return; }

            JSObject j = OgreForm.webView.CreateGlobalJavascriptObject("StructuresObject");
            j.Bind("ok", false, ok);
            j.Bind("new", false, disp);
            j.Bind("disp", false, disp);

            for(int i = 0; i < l.Count; i++) {
                OgreForm.webView.ExecuteJavascript("addStructure(" + l[i] + ")");
            }

        }

        private void disp(object sender, JavascriptMethodEventArgs e) {
            
            string name = (string)e.Arguments[0];

            if (File.Exists(this.mStateMgr.StoryInfo.pathToFile))
            {
                this.mStateMgr.StoryInfo.pathToFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\" + scenarioName + "\\" + name + ".scenario";
                this.mStateMgr.RequestStatePush(typeof(StoryEditorState));
            }
            else
            {
                FileStream stream = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\" + scenarioName + "\\" + name + ".scenario");
                stream.Close();
                this.mStateMgr.RequestStatePush(typeof(StoryEditorState));
                this.mStateMgr.StoryInfo.pathToFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\" + scenarioName + "\\" + name + ".scenario";
            }
        }

        private void ok(object sender, JavascriptMethodEventArgs e) {
            string name = (string)e.Arguments[0];
        }

        private void New(object sender, JavascriptMethodEventArgs e) {
            string name = (string)e.Arguments[0];
        }

        private void add(string name) {
            string[] s = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\" + name + "\\structures.scenario");

            for(int i = 0; i < s.Length; i++) {
                if(s[i][0] == '#') {
                    l.Add(s[i].Split(' ')[5]);
                }
            }
        }
    }
}

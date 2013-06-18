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
    public class Edit : GUI {
        private readonly StateManager mStateMgr;
        private static readonly Vector2 IMAGE_SIZE = new Vector2(450, 900);
        private string name;
        private string path;
        private Vector3 loc;
        private Dictionary<Vector3, List<string>> d = new Dictionary<Vector3, List<string>>();


        public Edit(StateManager stateMgr, string name, string path, Vector3 pos)
            : base(new Vector2(0, 0), IMAGE_SIZE / 3.3f, "Structures_Edit.html") {
            this.mStateMgr = stateMgr;
            this.name = name;
            this.path = path;
            this.loc = pos;

            string[] s = File.ReadAllLines(this.path + this.name + ".event");

            for(int i = 0; i < s.Length; i++) {
                string[] line = s[i].Split(' ');
                if(line.Length >= 3) {
                    Vector3 v = new Vector3(Convert.ToInt32(line[0]), Convert.ToInt32(line[1]), Convert.ToInt32(line[2]));
                    if(d.ContainsKey(v)) {
                        d[v].Add(string.Join(" ", line, 3, line.Length - 3));
                    } else {
                        d.Add(new Vector3(Convert.ToInt32(line[0]), Convert.ToInt32(line[1]), Convert.ToInt32(line[2])),
                            new List<string> { string.Join(" ", line, 3, line.Length - 3) });
                    }
                }
            }

            

        }

        public override void onDocumentReady(object sender, UrlEventArgs e) {
            base.onDocumentReady(sender, e);

            if(!OgreForm.webView.IsLive) { return; }

            JSObject j = OgreForm.webView.CreateGlobalJavascriptObject("EditObject");
            if(j != null) {
                j.Bind("save", false, ok);
            }

            if(d.ContainsKey(this.loc)) {
                OgreForm.webView.ExecuteJavascript("emptyText()");
                foreach(string entry in d[this.loc]) {
                    OgreForm.webView.ExecuteJavascript("addLine(\"" + entry + "\")");
                }
                
            }
        }

        private void ok(object sender, JavascriptMethodEventArgs e) {
            string s = e.Arguments[0].ToString().TrimEnd();
            string[] ss = s.Split('\n');

            if(d.ContainsKey(this.loc)) {
                d[this.loc] = new List<string>(ss);
            }

            StreamWriter f = new StreamWriter(this.path + this.name + ".event");
            
            foreach(KeyValuePair<Vector3, List<string>> entry in this.d) {
                foreach(string str in entry.Value) {
                    f.Write("\n" + loc.x); f.Write(" ");
                    f.Write(loc.y); f.Write(" ");
                    f.Write(loc.z); f.Write(" ");

                    f.Write(str);
                }
            }

            f.Close();

            this.mStateMgr.MainState.User.SwitchGUIVisibility(false);
            Visible = false;

            StoryEditorWorld.ed = null;

        }
    }
}

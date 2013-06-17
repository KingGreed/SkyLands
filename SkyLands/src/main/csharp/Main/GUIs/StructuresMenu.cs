using System;
using System.Collections.Generic;
using Awesomium.Core;
using Mogre;
using System.IO;

using Game.States;
using System.Windows.Forms;

using API.Generic;

using Game.BaseApp;

namespace Game.GUIs {
    public class StructuresMenu : GUI {
        private readonly StateManager mStateMgr;
        private static readonly Vector2 IMAGE_SIZE = new Vector2(450, 900);
        private List<string> l = new List<string>();

        public StructuresMenu(StateManager stateMgr, string name)
            : base(((OgreForm.InitSize - IMAGE_SIZE * Cst.GUI_RATIO) / 2) - (Vector2.UNIT_Y * (IMAGE_SIZE * Cst.GUI_RATIO).y / 12), IMAGE_SIZE, "Structures.html") {
            this.mStateMgr = stateMgr;

            var StructuresfileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\" + name + "\\structures.scenario";
            StreamReader stream;

            try { stream = new StreamReader(StructuresfileName); } catch { throw new Exception("Could not read file : " + StructuresfileName); }

        }

        public override void onDocumentReady(object sender, UrlEventArgs e) {
            base.onDocumentReady(sender, e);

            for (int i = 0; i < l.Count; i++) {
                OgreForm.webView.ExecuteJavascript("addStructure(" + l[i] + ")");
            }

            JSObject j = OgreForm.webView.CreateGlobalJavascriptObject("StructuresObject");
            j.Bind("Menu", false, (sender1, args) => this.mStateMgr.RequestStatePop());
            j.Bind("back", false, HideBack);
            j.Bind("save", false, save);
        }

        private void HideBack(object sender, EventArgs e) {
            
        }

        private void save(object sender, EventArgs e) {
            
        }

        private void add(string name) {
            string[] s = File.ReadAllLines(name);

            for(int i = 0; i < s.Length; i++) {
                if(s[i][0] == '#') {
                    l.Add(s[i].Split(' ')[5]);
                }
            }
        }
    }
}

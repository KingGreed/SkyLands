using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Awesomium.Windows.Forms;

using Game.BaseApp;

using Mogre;

namespace Game.GUIs
{
    public class Selector : GUI {  
        public int SelectorPos { get; set; }
        public string Material { get; set; }
        public bool IsBullet { get; set; }

        public Selector()
            : base(new Vector2(0, 0), new Vector2(404, 44), "file://" + Directory.GetCurrentDirectory() + "/media/web/Selector.html") {
                this.IsBullet = true;
        }

        public override void onDocumentReady(object sender, Awesomium.Core.UrlEventArgs e) {
            OgreForm.webView.ExecuteJavascript("resize(2, 2)");
        }
    }
}

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
    public class Inventory : GUI {

        public Inventory()
            : base(new Vector2(0, 0), new Vector2(350, 330), "inventory.html") {
        }

        public override void onDocumentReady(object sender, Awesomium.Core.UrlEventArgs e) {
            OgreForm.webView.ExecuteJavascript("resize(2, 2)");
            OgreForm.webView.Show();
        }
    }
}

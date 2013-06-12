using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

using Mogre;

using Game.BaseApp;

namespace Game.GUIs {
    public class MainMenu : GUI {
        public MainMenu()
            : base(new Vector2(0, 0), new Vector2(404, 44), "file://" + Directory.GetCurrentDirectory() + "/media/web/MainMenu.html", DockStyle.Fill) {
        }

        public override void onDocumentReady(object sender, Awesomium.Core.UrlEventArgs e) {
            
        }
    }
}

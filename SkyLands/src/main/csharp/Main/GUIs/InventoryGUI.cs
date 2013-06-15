using System;
using Mogre;

using API.Generic;

using Game.BaseApp;

namespace Game.GUIs
{
    public class InventoryGUI : GUI {
        private static readonly Vector2 IMAGE_SIZE = new Vector2(192, 179);
        public static readonly Vector2 WANTED_SIZE = IMAGE_SIZE * Cst.GUI_RATIO;

        public InventoryGUI(Vector2 position) : base(position, WANTED_SIZE, "inventory.html") { }

        public override void onDocumentReady(object sender, Awesomium.Core.UrlEventArgs e) {
            string s = Convert.ToString(Cst.GUI_RATIO).Replace(',', '.');
            OgreForm.webView.ExecuteJavascript("resize(" + s + ", " + s + ")");
        }
    }
}

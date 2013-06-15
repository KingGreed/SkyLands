using Mogre;

using API.Generic;

using Game.BaseApp;

namespace Game.GUIs
{
    public class InventoryGUI : GUI {
        private static readonly Vector2 IMAGE_SIZE = new Vector2(192, 179);

        public InventoryGUI()
            : base(((OgreForm.InitSize - IMAGE_SIZE * Cst.GUI_RATIO) / 2) - (Vector2.UNIT_Y * (IMAGE_SIZE * Cst.GUI_RATIO).y / 12),
                   IMAGE_SIZE, "inventory.html") { }

        public override void onDocumentReady(object sender, Awesomium.Core.UrlEventArgs e) {
            base.onDocumentReady(sender, e);
            Visible = true;

            string s = "setBlockAt(" + 0 + ", " + "'grass3.jpg'" + ", " + 5 + ")";
            OgreForm.webView.ExecuteJavascript(s);
        }
    }
}

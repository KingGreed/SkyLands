using System;
using Mogre;

using Game.BaseApp;

namespace Game.GUIs
{
    public class Inventory : GUI {
        private static readonly Vector2 IMAGE_SIZE = new Vector2(192, 179);
        private const float ratio = 2.2f;
        public static readonly Vector2 WANTED_SIZE = IMAGE_SIZE * ratio;

        public Inventory(Vector2 position) : base(position, WANTED_SIZE, "inventory.html") { }

        public override void onDocumentReady(object sender, Awesomium.Core.UrlEventArgs e) {
            string s = Convert.ToString(ratio).Replace(',', '.');
            OgreForm.webView.ExecuteJavascript("resize(" + s + ", " + s + ")");
            OgreForm.webView.Show();
        }
    }
}

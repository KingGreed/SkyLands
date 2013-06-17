using System;
using Mogre;

using Awesomium.Core;

using API.Generic;

using Game.BaseApp;

namespace Game.GUIs
{
    public class InventoryGUI : GUI {
        private static readonly Vector2 IMAGE_SIZE = new Vector2(192, 179);

        private readonly Action mOnOpen, mOnCraft;

        public InventoryGUI(Action update, Action onCaft)
            : base(((OgreForm.InitSize - IMAGE_SIZE * Cst.GUI_RATIO) / 2) - (Vector2.UNIT_Y * (IMAGE_SIZE * Cst.GUI_RATIO).y / 12),
                   IMAGE_SIZE, "inventory.html")
        {
            this.mOnOpen = update;
            this.mOnCraft = onCaft;
        }

        public override void onDocumentReady(object sender, UrlEventArgs e) {
            base.onDocumentReady(sender, e);
            JSObject j = OgreForm.webView.CreateGlobalJavascriptObject("InventoryObject");
            j.Bind("craft", false, (s1, args) => this.mOnCraft());
            j.Bind("selectBarUpdate", false, (s2, args) => UpdateSelectBar(args));
            this.mOnOpen();
        }

        public static void UpdateSelectBar(JavascriptMethodEventArgs args)
        {
            int position = (int)args.Arguments[0] - 30;
            string[] temp = args.Arguments[1].ToString().Split('/');
            string imageName = temp[temp.Length - 1];
            int amount = 1;

            Selector.SetBlockAt(position, imageName, amount);
        }
    }
}

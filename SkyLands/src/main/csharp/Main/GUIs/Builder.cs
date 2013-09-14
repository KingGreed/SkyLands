using System;
using Awesomium.Core;
using Game.RTS;
using Game.csharp.Main.RTS.Buildings;
using Mogre;

using API.Generic;

using Game.BaseApp;

namespace Game.GUIs
{
    public class Builder : GUI
    {
        private static readonly Vector2 IMAGE_SIZE = new Vector2(192, 179);

        public static Action OnOpen;
        private BuildingManager mBuildingMgr;

        public Builder(BuildingManager buildingMgr)
            : base(((OgreForm.InitSize - IMAGE_SIZE * Cst.GUI_RATIO) / 2) - (Vector2.UNIT_Y * (IMAGE_SIZE * Cst.GUI_RATIO).y / 12),
                   IMAGE_SIZE, "builder.html")
        {
            this.mBuildingMgr = buildingMgr;
            JSObject j = OgreForm.webView.CreateGlobalJavascriptObject("BuilderObject");
            j.Bind("selectBarUpdate", false, (s2, args) => InventoryGUI.UpdateSelectBar(args));
            j.Bind("selectUpdate", false, (s2, args) => this.mBuildingMgr.OnBuildingSelection(args.Arguments[0]));
            j.Bind("update", false, (s2, args) => this.mBuildingMgr.OnDrop((int)args.Arguments[0], int.Parse(args.Arguments[1])));
        }

        public override void onDocumentReady(object sender, UrlEventArgs e)
        {
            base.onDocumentReady(sender, e);
            if (this.mBuildingMgr.HasActualBuilding() && OgreForm.webView.IsLive)
            {
                string selection = this.mBuildingMgr.GetActualBuilding().Selection;
                OgreForm.webView.ExecuteJavascript("setSelection('" + selection + "')");
                this.mBuildingMgr.OnBuildingSelection(selection);
            }
            OnOpen();
        }
    }
}

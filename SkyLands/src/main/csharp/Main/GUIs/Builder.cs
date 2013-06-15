using System;
using Mogre;

using API.Generic;

using Game.BaseApp;

namespace Game.GUIs
{
    public class Builder : GUI
    {
        private static readonly Vector2 IMAGE_SIZE = new Vector2(192, 179);

        private readonly Action mUpdate;

        public Builder(Action update)
            : base(((OgreForm.InitSize - IMAGE_SIZE * Cst.GUI_RATIO) / 2) - (Vector2.UNIT_Y * (IMAGE_SIZE * Cst.GUI_RATIO).y / 12),
                   IMAGE_SIZE, "builder.html")
        {
            this.mUpdate = update;
        }
        public override void onDocumentReady(object sender, Awesomium.Core.UrlEventArgs e)
        {
            base.onDocumentReady(sender, e);
            Visible = true;
            this.mUpdate();
        }
    }
}

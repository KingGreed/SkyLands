using System;
using System.Collections.Generic;
using Mogre;

using Miyagi.Common.Events;
using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Layout;
using Miyagi.UI.Controls.Styles;

using Game.States;

namespace Game.GUICreator
{
    class GameGUI : GUIFactory
    {
        public GameGUI(StateManager stateMgr) : base(stateMgr, "Game GUI") { }

        protected override void CreateGUI()
        {
            /* Cross image */
            PictureBox cross = new PictureBox();
            cross.AlwaysOnBottom = true;
            cross.Bitmap = new System.Drawing.Bitmap(@"../../src/Media/images/cross.png");
            cross.Size = new Size(16, 15);
            cross.Location = new Point(this.mOriginalWndSize.Width / 2 - cross.Size.Width, this.mOriginalWndSize.Height / 2 - cross.Size.Height);
            this.mGUI.Controls.Add(cross);
        }
    }
}

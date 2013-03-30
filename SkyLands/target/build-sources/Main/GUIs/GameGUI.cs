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
    public class GameGUI : GUIFactory
    {
        private PictureBox mCross;
        private InGameMenuGUI mIGMenu;

        public InGameMenuGUI IGMenu { get { return this.mIGMenu; } }
        
        public GameGUI(StateManager stateMgr) : base(stateMgr, "Game GUI")
        {
            this.mIGMenu = new InGameMenuGUI(stateMgr);
        }

        protected override void CreateGUI()
        {
            /* Cross image */
            this.mCross = new PictureBox();
            this.mCross.AlwaysOnBottom = true;
            this.mCross.Bitmap = new System.Drawing.Bitmap(@"./Media/images/cross.png");
            this.mCross.Size = new Size(16, 15);
            this.mCross.Location = new Point(this.mOriginalWndSize.Width / 2 - this.mCross.Size.Width / 2, this.mOriginalWndSize.Height / 2 - this.mCross.Size.Height / 2);
            this.mGUI.Controls.Add(this.mCross);
        }

        public void SwitchGUIForDebugCam()
        {
            this.mCross.Visible = !this.mCross.Visible;
        }

        public override void  Hide()
        {
            this.mIGMenu.Hide();
            base.Hide();
        }

        public override void Dispose()
        {
            this.mIGMenu.Dispose();
            base.Dispose();
        }

        public bool SwitchVisibleIGMenu()   // Retun whether it is shown or not
        {
            this.mIGMenu.SwithVisibility();

            this.mStateMgr.MiyagiMgr.CursorVisibility = this.mIGMenu.IsGUIVisible;

            return this.mIGMenu.IsGUIVisible;
        }
    }
}

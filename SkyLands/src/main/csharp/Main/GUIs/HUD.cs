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
    public class HUD : GUIFactory
    {
        private PictureBox mCross, mHeart, mCrystal;
        private InGameMenuGUI mIGMenu;
        private Label mLife, mRessources;

        public InGameMenuGUI IGMenu { get { return this.mIGMenu; } }
        
        public HUD(StateManager stateMgr) : base(stateMgr, "Game GUI")
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

            /* Heart image */
            this.mHeart = new PictureBox();
            this.mHeart.Bitmap = new System.Drawing.Bitmap(@"./Media/images/heart.png");
            this.mHeart.Size = new Size(34, 24);
            this.mHeart.Location = new Point(this.mOriginalWndSize.Width - 230, 20);
            this.mGUI.Controls.Add(this.mHeart);

            /* Life text */
            TextStyle style = new TextStyle();
            style.Alignment = Alignment.MiddleCenter;
            style.Font = this.mMiyagiMgr.Fonts["Small_BlueHighway"];
            style.ForegroundColour = new Colour(255, 255, 255, 255);
            this.mLife = new Label();
            this.mLife.Size = new Size(350, 50);
            this.mLife.Location = this.mHeart.Location + new Point(-50, -14);
            this.mLife.TextStyle = style;
            this.mGUI.Controls.Add(this.mLife);

            /* Crystal image */
            this.mCrystal = new PictureBox();
            this.mCrystal.Bitmap = new System.Drawing.Bitmap(@"./Media/images/CrystalBlock.png");
            this.mCrystal.Size = new Size(46,34);
            this.mCrystal.Location = this.mHeart.Location - new Point(200, 8);
            this.mGUI.Controls.Add(this.mCrystal);

            /* Ressource text */
            this.mRessources = new Label();
            this.mRessources.Size = new Size(200, 50);
            this.mRessources.Location = this.mCrystal.Location + new Point(-10, -8);
            this.mRessources.TextStyle = style;
            this.mGUI.Controls.Add(this.mRessources);
        }

        public void UpdateLife(float actLife, float maxLife)
        {
            this.mLife.Text = (int)actLife + " / " + (int)maxLife;
        }

        public void UpdateCrystal(int crystal)
        {
            this.mRessources.Text = crystal.ToString();
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
            this.mIGMenu.ShowSaveMessage(false);

            this.mStateMgr.MiyagiMgr.CursorVisibility = this.mIGMenu.IsGUIVisible;

            return this.mIGMenu.IsGUIVisible;
        }
    }
}

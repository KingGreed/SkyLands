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
        public enum Selection { FireCube, WaterCube, MagicCube, Grass, Dirt, Stone, Wood, Leaves, Sand, Construction }
        
        private PictureBox mCross, mHeart, mCrystal, mSelectBar, mSelector;
        private PictureBox[] mBoxes;
        private InGameMenuGUI mIGMenu;
        private Label mLife, mRessources;
        private Size mBoxSize;
        private int mSelectorLoc;
        private float mSpace;

        public InGameMenuGUI IGMenu { get { return this.mIGMenu; } }
        public Selection Selector { get { return (Selection)this.mSelectorLoc; } }
        
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

            /* SelectBar image */
            this.mSelectBar = new PictureBox();
            this.mSelectBar.Bitmap = new System.Drawing.Bitmap(@"./Media/images/selectBar.png");
            this.mSelectBar.Size = new Size(750, 60);
            this.mSelectBar.Location = new Point(this.mOriginalWndSize.Width / 2 - this.mSelectBar.Size.Width / 2, this.mOriginalWndSize.Height - 95);
            this.mGUI.Controls.Add(this.mSelectBar);

            /* Selector image */
            this.mSelector = new PictureBox();
            this.mSelector.Bitmap = new System.Drawing.Bitmap(@"./Media/images/selector.png");
            this.mSelector.Size = new Size(83, 60);
            this.mGUI.Controls.Add(this.mSelector);
            this.mSelectorLoc = 0;

            this.mBoxes = new PictureBox [10];
            this.mBoxSize = new Size(61, 42);
            this.mSpace = (float)this.mBoxSize.Width + 12.8f;
            for (int i = 0; i < 10; i++)
            {
                PictureBox b = new PictureBox();
                b.Size = this.mBoxSize;
                b.Location = this.mSelectBar.Location + new Point((int)(i * this.mSpace) + 10, 9);
                this.mGUI.Controls.Add(b);
                this.mBoxes[i] = b;
            }

            this.mBoxes[0].Bitmap = new System.Drawing.Bitmap(@"./Media/materials/textures/fire.png");
            this.mBoxes[1].Bitmap = new System.Drawing.Bitmap(@"./Media/materials/textures/waterCube.png");
            this.mBoxes[2].Bitmap = new System.Drawing.Bitmap(@"./Media/materials/textures/crystal.png");
            this.mBoxes[3].Bitmap = new System.Drawing.Bitmap(@"./Media/materials/textures/earth_side.jpg");
            this.mBoxes[4].Bitmap = new System.Drawing.Bitmap(@"./Media/materials/textures/dirt.jpg");
            this.mBoxes[5].Bitmap = new System.Drawing.Bitmap(@"./Media/materials/textures/stone.jpg");
            this.mBoxes[6].Bitmap = new System.Drawing.Bitmap(@"./Media/materials/textures/wood.png");
            this.mBoxes[7].Bitmap = new System.Drawing.Bitmap(@"./Media/materials/textures/leaves.png");
            this.mBoxes[8].Bitmap = new System.Drawing.Bitmap(@"./Media/materials/textures/sand.jpg");
            this.mBoxes[9].Bitmap = new System.Drawing.Bitmap(@"./Media/materials/textures/constructionBlockSide2.png");
            this.PlaceSelector();
        }

        public void UpdateLife(float actLife, float maxLife)
        {
            this.mLife.Text = (int)actLife + " / " + (int)maxLife;
        }

        public void UpdateCrystal(int crystal)
        {
            this.mRessources.Text = crystal.ToString();
        }

        public void MoveSelector(int dir)
        {
            int sign = (int)Mogre.Math.Sign(dir);
            this.mSelectorLoc += sign;
            if (this.mSelectorLoc < 0) { this.mSelectorLoc = 9; }
            else if (this.mSelectorLoc > 9) { this.mSelectorLoc = 0; }
            this.PlaceSelector();
        }

        private void PlaceSelector()
        {
            //this.mSelector.Location = this.mSelectBar.Location + new Point((int)(this.mSelectorLoc * (this.mBoxSize.Width - 11)), 0);
            this.mSelector.Location = this.mBoxes[this.mSelectorLoc].Location - new Point(7, 7);
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

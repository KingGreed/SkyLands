using System;
using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Styles;

using Game.States;

namespace Game.GUICreator
{
    public class HUD : MainGUI
    {
        public enum Selection { FireCube, WaterCube, MagicCube, Grass, Dirt, Stone, Wood, Leaves, Sand, Construction }

        private Label mLife, mRessources;

        public Selection Selector { get { return (Selection)this.mSelectorLoc; } }

        public HUD(StateManager stateMgr) : base(stateMgr, "Game GUI") { }

        protected override void CreateGUI()
        {
            base.CreateGUI();

            /* Cross image */
            PictureBox cross = new PictureBox
            {
                AlwaysOnBottom = true,
                Bitmap = new System.Drawing.Bitmap(@"./Media/images/cross.png"),
                Size = new Size(16, 15)
            };
            cross.Location = new Point(this.mOriginalWndSize.Width / 2 - cross.Size.Width / 2, this.mOriginalWndSize.Height / 2 - cross.Size.Height / 2);
            this.mGUI.Controls.Add(cross);

            /* Heart image */
            PictureBox heart = new PictureBox
            {
                Bitmap = new System.Drawing.Bitmap(@"./Media/images/heart.png"),
                Size = new Size(34, 24),
                Location = new Point(this.mOriginalWndSize.Width - 230, 20)
            };
            this.mGUI.Controls.Add(heart);

            /* Life text */
            TextStyle style = new TextStyle
            {
                Alignment = Alignment.MiddleCenter,
                Font = this.mMiyagiMgr.Fonts["Small_BlueHighway"],
                ForegroundColour = new Colour(255, 255, 255, 255)
            };
            this.mLife = new Label
            {
                Size = new Size(350, 50),
                Location = heart.Location + new Point(-50, -14),
                TextStyle = style
            };
            this.mGUI.Controls.Add(this.mLife);

            /* Crystal image */
            PictureBox crystal = new PictureBox
            {
                Bitmap = new System.Drawing.Bitmap(@"./Media/images/CrystalBlock.png"),
                Size = new Size(46, 34),
                Location = heart.Location - new Point(200, 8)
            };
            this.mGUI.Controls.Add(crystal);

            /* Ressource text */
            this.mRessources = new Label
            {
                Size = new Size(200, 50),
                Location = crystal.Location + new Point(-10, -8),
                TextStyle = style
            };
            this.mGUI.Controls.Add(this.mRessources);
        }

        protected override void SetImages()
        {
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
        }

        public override string GetMaterial()
        {
            return this.mSelectorLoc > 2 ? Enum.GetName(typeof(Selection), (Selection)this.mSelectorLoc) : "";
        }

        public void UpdateLife(float actLife, float maxLife)
        {
            this.mLife.Text = (int)actLife + " / " + (int)maxLife;
        }

        public void UpdateCrystal(int crystal)
        {
            this.mRessources.Text = crystal.ToString();
        }
    }
}

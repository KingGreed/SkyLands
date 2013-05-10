using Miyagi.Common.Data;
using Miyagi.UI.Controls;

using Game.States;

namespace Game.GUICreator
{
    public abstract class MainGUI : GUIFactory
    {
        private PictureBox mSelector;
        protected PictureBox[] mBoxes;
        protected int mSelectorLoc;

        protected MainGUI(StateManager stateMgr, string name) : base(stateMgr, name) {}

        protected override void CreateGUI()
        {
            /* SelectBar image */
            PictureBox selectBar = new PictureBox
            {
                Bitmap = new System.Drawing.Bitmap(@"./Media/images/selectBar.png"),
                Size = new Size(750, 60)
            };
            selectBar.Location = new Point(this.mOriginalWndSize.Width / 2 - selectBar.Size.Width / 2, this.mOriginalWndSize.Height - 95);
            this.mGUI.Controls.Add(selectBar);

            /* Selector image */
            this.mSelector = new PictureBox
            {
                Bitmap = new System.Drawing.Bitmap(@"./Media/images/selector.png"),
                Size = new Size(83, 60)
            };
            this.mGUI.Controls.Add(this.mSelector);
            this.mSelectorLoc = 0;

            this.mBoxes = new PictureBox[10];
            Size boxSize = new Size(61, 42);
            float space = boxSize.Width + 12.8f;
            for (int i = 0; i < 10; i++)
            {
                PictureBox b = new PictureBox
                {
                    Size = boxSize,
                    Location = selectBar.Location + new Point((int) (i*space) + 10, 9)
                };
                this.mGUI.Controls.Add(b);
                this.mBoxes[i] = b;
            }

            this.SetImages();
            this.PlaceSelector();
        }

        protected abstract void SetImages();
        public abstract string GetMaterial();

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
            this.mSelector.Location = this.mBoxes[this.mSelectorLoc].Location - new Point(7, 7);
        }
    }
}

using System;
using System.Collections.Generic;

using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Layout;
using Miyagi.Common.Events;
using Miyagi.UI.Controls.Styles;

using Game.States;

namespace Game.GUICreator
{
    public class SecondMenuGUI : GUIFactory
    {
        private Dictionary<string, Button> mButtons;
        private PictureBox mBackGround;
        private Label mLabel;
        private Button mBackButton;
        private string mSeed;
        private string mSizeX;
        private string mSizeY;

        public SecondMenuGUI(StateManager stateMgr, string name) : base(stateMgr, name) { }

        protected override void CreateGUI()
        {
            /* Panel */
            mBackGround = new PictureBox();
            //mBackGround.AlwaysOnBottom = true;
            mBackGround.Bitmap = new System.Drawing.Bitmap(@"../../src/Media/images/menuPlay.png");
            mBackGround.Size = this.mOriginalWndSize;
            this.mGUI.Controls.Add(mBackGround);

            /* Text */
            this.mLabel = new Label();
            this.mLabel.Text = "Choose an Island";
            this.mLabel.TextStyle.ForegroundColour = new Colour(255, 255, 255, 255);
            this.mLabel.Size = new Size(this.mBackGround.Size.Width, 50);
            this.mLabel.AutoSize = true;
            this.mLabel.AutoSizeMode = Miyagi.UI.AutoSizeMode.GrowAndShrink;
            this.mGUI.Controls.Add(this.mLabel);

            /* Buttons */
            TextStyle style = new TextStyle();
            style.Alignment = Alignment.MiddleCenter;
            style.Font = this.mMiyagiMgr.Fonts["Small_BlueHighway"];
            Size actualButtonSize = new Size(150, 60);
            int space = (this.mBackGround.Size.Width - 4 * actualButtonSize.Width) / 5;
            Point originalpos = this.mBackGround.Location + new Point(space, 65);

            this.mButtons = new Dictionary<string, Button>();
            for (int i = 0; i < Enum.GetValues(typeof(StateManager.TypeWorld)).Length; i++)
            {

                Button button = new Button();
                button.Size = actualButtonSize;
                button.Text = Enum.GetName(typeof(StateManager.TypeWorld), (StateManager.TypeWorld)i);
                button.TextStyle = style;
                button.Location = originalpos + new Point(i * (actualButtonSize.Width + space), 150);
                button.Skin = this.mMiyagiMgr.Skins["Button"];
                this.mGUI.Controls.Add(button);
                this.mButtons.Add(button.Text, button);
            }
            mBackButton = new Button();
            mBackButton.Size = actualButtonSize;
            mBackButton.Text = "BACK";
            mBackButton.TextStyle = style;
            mBackButton.Location = this.mBackGround.Location + new Point(this.mBackGround.Size.Width / 10, (this.mBackGround.Size.Height / 10) * 9);
            mBackButton.Skin = this.mMiyagiMgr.Skins["Button"];
            this.mGUI.Controls.Add(mBackButton);

            Miyagi.UI.Controls.TextBox textBoxSeed = new TextBox();
            textBoxSeed.Size = new Size(450, 50);
            textBoxSeed.Focused = true;
            textBoxSeed.ToolTipText = textBoxSeed.Text;
            textBoxSeed.TextStyle.ForegroundColour = new Colour(255, 255, 255, 255);
            textBoxSeed.TextStyle.Alignment = Alignment.MiddleCenter;
            textBoxSeed.Location = this.mBackGround.Location + new Point((this.mBackGround.Size.Width / 10) * 4 -40, (this.mBackGround.Size.Height / 10) * 4);
            textBoxSeed.Skin = this.mMiyagiMgr.Skins["Console"];
            this.mGUI.Controls.Add(textBoxSeed);
            textBoxSeed.Submit += new EventHandler<ValueEventArgs<string>>(textBoxSeed_Submit);

            Label Seed = new Label();
            Seed.Text = "Choose Seed";
            Seed.TextStyle.ForegroundColour = new Colour(255, 255, 255, 255);
            Seed.Size = new Size(50, 50);
            Seed.Location = this.mBackGround.Location + new Point((this.mBackGround.Size.Width / 10) * 3 - 110, (this.mBackGround.Size.Height / 10) * 4 + 10);
            Seed.AutoSize = true;
            Seed.AutoSizeMode = Miyagi.UI.AutoSizeMode.GrowAndShrink;
            this.mGUI.Controls.Add(Seed);
            //textBox.Text = "42";

            Miyagi.UI.Controls.TextBox textBoxSizeX = new TextBox();
            textBoxSizeX.Size = new Size(450, 50);
            textBoxSizeX.Focused = true;
            textBoxSizeX.ToolTipText = textBoxSizeX.Text;
            textBoxSizeX.TextStyle.ForegroundColour = new Colour(255, 255, 255, 255);
            textBoxSizeX.TextStyle.Alignment = Alignment.MiddleCenter;
            textBoxSizeX.Location = this.mBackGround.Location + new Point((this.mBackGround.Size.Width / 10) * 4 -40, (this.mBackGround.Size.Height / 10) * 4 + 70);
            textBoxSizeX.Skin = this.mMiyagiMgr.Skins["Console"];
            this.mGUI.Controls.Add(textBoxSizeX);
            textBoxSizeX.Submit += new EventHandler<ValueEventArgs<string>>(textBoxSizeX_Submit);

            Label SizeX = new Label();
            SizeX.Text = "Choose Width size";
            SizeX.TextStyle.ForegroundColour = new Colour(255, 255, 255, 255);
            SizeX.Size = new Size(50, 50);
            SizeX.Location = this.mBackGround.Location + new Point((this.mBackGround.Size.Width / 10) * 3 - 110, (this.mBackGround.Size.Height / 10) * 4 + 80);
            SizeX.AutoSize = true;
            SizeX.AutoSizeMode = Miyagi.UI.AutoSizeMode.GrowAndShrink;
            this.mGUI.Controls.Add(SizeX);

            Miyagi.UI.Controls.TextBox textBoxSizeY = new TextBox();
            textBoxSizeY.Size = new Size(450, 50);
            textBoxSizeY.Focused = true;
            textBoxSizeY.ToolTipText = textBoxSizeY.Text;
            textBoxSizeY.TextStyle.ForegroundColour = new Colour(255, 255, 255, 255);
            textBoxSizeY.TextStyle.Alignment = Alignment.MiddleCenter;
            textBoxSizeY.Location = this.mBackGround.Location + new Point((this.mBackGround.Size.Width / 10) * 4 - 40, (this.mBackGround.Size.Height / 10) * 4 + 140);
            textBoxSizeY.Skin = this.mMiyagiMgr.Skins["Console"];
            this.mGUI.Controls.Add(textBoxSizeY);
            textBoxSizeY.Submit += new EventHandler<ValueEventArgs<string>>(textBoxSizeY_Submit);

            Label SizeY = new Label();
            SizeY.Text = "Choose Height size";
            SizeY.TextStyle.ForegroundColour = new Colour(255, 255, 255, 255);
            SizeY.Size = new Size(50, 50);
            SizeY.Location = this.mBackGround.Location + new Point((this.mBackGround.Size.Width / 10) * 3 - 110, (this.mBackGround.Size.Height / 10) * 4 + 150);
            SizeY.AutoSize = true;
            SizeY.AutoSizeMode = Miyagi.UI.AutoSizeMode.GrowAndShrink;
            this.mGUI.Controls.Add(SizeY);
            
        }

        void textBoxSizeY_Submit(object sender, ValueEventArgs<string> e)
        {
            this.mSizeY = e.Data;
        }

        void textBoxSizeX_Submit(object sender, ValueEventArgs<string> e)
        {
            this.mSizeX = e.Data;
        }

        void textBoxSeed_Submit(object sender, ValueEventArgs<string> e)
        {
            this.mSeed = e.Data;
        }

        protected override void AfterResize()
        {
            this.mLabel.Location = this.mBackGround.Location + new Point((this.mBackGround.Size.Width - this.mLabel.Size.Width) / 2, 15);
        }

        public void SetListener(StateManager.TypeWorld button, EventHandler<MouseButtonEventArgs> del)
        {
            this.mButtons[Enum.GetName(typeof(StateManager.TypeWorld), button)].MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(del);
        }

        public void SetListenerBack(EventHandler<MouseButtonEventArgs> del)
        {
            this.mBackButton.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(del);
        }
    }
}
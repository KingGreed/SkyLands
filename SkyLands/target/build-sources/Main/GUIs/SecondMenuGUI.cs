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
using Game.World;

namespace Game.GUICreator
{
    public class SecondMenuGUI : GUIFactory
    {
        private Dictionary<string, Button> mButtons;
        private PictureBox mBackground;
        private Label mLabel;
        private Button mBackButton;
        private GameInfo mGameInfo;

        public SecondMenuGUI(StateManager stateMgr, GameInfo gameInfo) : base(stateMgr, "SecondMenu GUI")
        {
            this.mGameInfo = gameInfo;
        }

        protected override void CreateGUI()
        {
            /* Background */
            this.mBackground = new PictureBox();
            this.mBackground.AlwaysOnBottom = true;
            this.mBackground.Bitmap = new System.Drawing.Bitmap(@"./Media/images/menuPlay.png");
            this.mBackground.Size = this.mOriginalWndSize;
            this.mGUI.Controls.Add(this.mBackground);

            /* Text */
            this.mLabel = new Label();
            this.mLabel.Text = "Choose an Island";
            this.mLabel.TextStyle.ForegroundColour = new Colour(255, 255, 255, 255);
            this.mLabel.Size = new Size(this.mBackground.Size.Width, 50);
            this.mLabel.AutoSize = true;
            this.mLabel.AutoSizeMode = Miyagi.UI.AutoSizeMode.GrowAndShrink;
            this.mGUI.Controls.Add(this.mLabel);

            /* Islands Buttons */
            TextStyle style = new TextStyle();
            style.Alignment = Alignment.MiddleCenter;
            style.Font = this.mMiyagiMgr.Fonts["Small_BlueHighway"];
            Size actualButtonSize = new Size(150, 60);
            int space = (this.mBackground.Size.Width - 5 * actualButtonSize.Width) / 6;
            Point originalpos = this.mBackground.Location + new Point(space, 65);

            this.mButtons = new Dictionary<string, Button>();
            for (int i = 0; i < Enum.GetValues(typeof(GameInfo.TypeWorld)).Length; i++)
            {

                Button button = new Button();
                button.Size = actualButtonSize;
                button.Text = Enum.GetName(typeof(GameInfo.TypeWorld), (GameInfo.TypeWorld)i);
                button.TextStyle = style;
                button.Location = originalpos + new Point(i * (actualButtonSize.Width + space), 150);
                button.Skin = this.mMiyagiMgr.Skins["Button"];
                this.mGUI.Controls.Add(button);
                this.mButtons.Add(button.Text, button);
            }
            this.mBackButton = new Button();
            this.mBackButton.Size = actualButtonSize;
            this.mBackButton.Text = "BACK";
            this.mBackButton.TextStyle = style;
            this.mBackButton.Location = this.mBackground.Location + new Point(this.mBackground.Size.Width / 10, (this.mBackground.Size.Height / 10) * 9);
            this.mBackButton.Skin = this.mMiyagiMgr.Skins["Button"];
            this.mGUI.Controls.Add(this.mBackButton);

            /* Seed TextBox */
            TextBox textBoxSeed = new TextBox();
            textBoxSeed.Size = new Size(450, 50);
            textBoxSeed.TextStyle.ForegroundColour = new Colour(255, 255, 255, 255);
            textBoxSeed.TextStyle.Alignment = Alignment.MiddleCenter;
            textBoxSeed.Location = this.mBackground.Location + new Point((this.mBackground.Size.Width / 10) * 4 -40, (this.mBackground.Size.Height / 10) * 4);
            textBoxSeed.Skin = this.mMiyagiMgr.Skins["Console"];
            this.mGUI.Controls.Add(textBoxSeed);
            textBoxSeed.Submit += new EventHandler<ValueEventArgs<string>>(textBoxSeed_Submit);

            Label labelSeed = new Label();
            labelSeed.Text = "Choose Seed";
            labelSeed.TextStyle.ForegroundColour = new Colour(255, 255, 255, 255);
            labelSeed.Size = new Size(50, 50);
            labelSeed.Location = this.mBackground.Location + new Point((this.mBackground.Size.Width / 10) * 3 - (this.mBackground.Size.Width / 10) * 2, (this.mBackground.Size.Height / 10) * 4 + 10);
            labelSeed.AutoSize = true;
            labelSeed.AutoSizeMode = Miyagi.UI.AutoSizeMode.GrowAndShrink;
            this.mGUI.Controls.Add(labelSeed);

            /* Width TextBox */
            TextBox textBoxSizeX = new TextBox();
            textBoxSizeX.Size = new Size(450, 50);
            textBoxSizeX.TextStyle.ForegroundColour = new Colour(255, 255, 255, 255);
            textBoxSizeX.TextStyle.Alignment = Alignment.MiddleCenter;
            textBoxSizeX.Location = this.mBackground.Location + new Point((this.mBackground.Size.Width / 10) * 4 -40, (this.mBackground.Size.Height / 10) * 4 + 70);
            textBoxSizeX.Skin = this.mMiyagiMgr.Skins["Console"];
            this.mGUI.Controls.Add(textBoxSizeX);
            textBoxSizeX.Submit += new EventHandler<ValueEventArgs<string>>(textBoxSizeX_Submit);

            Label labelWidth = new Label();
            labelWidth.Text = "Choose Width";
            labelWidth.TextStyle.ForegroundColour = new Colour(255, 255, 255, 255);
            labelWidth.Size = new Size(50, 50);
            labelWidth.Location = this.mBackground.Location + new Point((this.mBackground.Size.Width / 10) * 3 - (this.mBackground.Size.Width / 10) * 2, (this.mBackground.Size.Height / 10) * 4 + 80);
            labelWidth.AutoSize = true;
            labelWidth.AutoSizeMode = Miyagi.UI.AutoSizeMode.GrowAndShrink;
            this.mGUI.Controls.Add(labelWidth);

            /* Height TextBox */
            TextBox textBoxSizeY = new TextBox();
            textBoxSizeY.Size = new Size(450, 50);
            textBoxSizeY.TextStyle.ForegroundColour = new Colour(255, 255, 255, 255);
            textBoxSizeY.TextStyle.Alignment = Alignment.MiddleCenter;
            textBoxSizeY.Location = this.mBackground.Location + new Point((this.mBackground.Size.Width / 10) * 4 - 40, (this.mBackground.Size.Height / 10) * 4 + 140);
            textBoxSizeY.Skin = this.mMiyagiMgr.Skins["Console"];
            this.mGUI.Controls.Add(textBoxSizeY);
            textBoxSizeY.Submit += new EventHandler<ValueEventArgs<string>>(textBoxSizeY_Submit);

            Label labelHeight = new Label();
            labelHeight.Text = "Choose Height";
            labelHeight.TextStyle.ForegroundColour = new Colour(255, 255, 255, 255);
            labelHeight.Size = new Size(50, 50);
            labelHeight.Location = this.mBackground.Location + new Point((this.mBackground.Size.Width / 10) * 3 - (this.mBackground.Size.Width / 10) * 2, (this.mBackground.Size.Height / 10) * 4 + 150);
            labelHeight.AutoSize = true;
            labelHeight.AutoSizeMode = Miyagi.UI.AutoSizeMode.GrowAndShrink;
            this.mGUI.Controls.Add(labelHeight);
        }

        private void textBoxSizeY_Submit(object sender, ValueEventArgs<string> e)
        {
            int sizeY;
            if (int.TryParse(e.Data, out sizeY))
            {
                Mogre.Vector2 size = this.mGameInfo.Size;
                size.y = sizeY;
                this.mGameInfo.Size = size;
            }
            this.mStateMgr.MyConsole.Enabled = false;
        }

        private void textBoxSizeX_Submit(object sender, ValueEventArgs<string> e)
        {
            int sizeX;
            if (int.TryParse(e.Data, out sizeX)) 
            {
                Mogre.Vector2 size = this.mGameInfo.Size;
                size.x = sizeX;
                this.mGameInfo.Size = size;
            }
            this.mStateMgr.MyConsole.Enabled = false;
        }

        private void textBoxSeed_Submit(object sender, ValueEventArgs<string> e)
        {
            int seed;
            if (int.TryParse(e.Data, out seed)) { this.mGameInfo.Seed = seed; }
            this.mStateMgr.MyConsole.Enabled = false;
        }

        protected override void AfterResize()
        {
            this.mLabel.Location = this.mBackground.Location + new Point((this.mBackground.Size.Width - this.mLabel.Size.Width) / 2, 15);
        }

        public void SetListener(GameInfo.TypeWorld button, EventHandler<MouseButtonEventArgs> del)
        {
            this.mButtons[Enum.GetName(typeof(GameInfo.TypeWorld), button)].MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(del);
        }

        public void SetListenerBack(EventHandler<MouseButtonEventArgs> del)
        {
            this.mBackButton.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(del);
        }
    }
}
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
        private PictureBox mBackGround;
        private Label mLabel;
        private Button mBackButton;
        private GameInfo mGameInfo;

        public SecondMenuGUI(StateManager stateMgr, GameInfo gameInfo) : base(stateMgr, "SecondMenu GUI")
        {
            this.mGameInfo = gameInfo;
        }

        protected override void CreateGUI()
        {
            /* Panel */
            this.mBackGround = new PictureBox();
            this.mBackGround.AlwaysOnBottom = true;
            this.mBackGround.Bitmap = new System.Drawing.Bitmap(@"../../src/Media/images/menuPlay.png");
            this.mBackGround.Size = this.mOriginalWndSize;
            this.mGUI.Controls.Add(this.mBackGround);

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
            this.mBackButton.Location = this.mBackGround.Location + new Point(this.mBackGround.Size.Width / 10, (this.mBackGround.Size.Height / 10) * 9);
            this.mBackButton.Skin = this.mMiyagiMgr.Skins["Button"];
            this.mGUI.Controls.Add(this.mBackButton);

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
            Seed.Location = this.mBackGround.Location + new Point((this.mBackGround.Size.Width / 10) * 3 - (this.mBackGround.Size.Width / 10) * 2, (this.mBackGround.Size.Height / 10) * 4 + 10);
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
            SizeX.Text = "Choose Width";
            SizeX.TextStyle.ForegroundColour = new Colour(255, 255, 255, 255);
            SizeX.Size = new Size(50, 50);
            SizeX.Location = this.mBackGround.Location + new Point((this.mBackGround.Size.Width / 10) * 3 - (this.mBackGround.Size.Width / 10) * 2, (this.mBackGround.Size.Height / 10) * 4 + 80);
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
            SizeY.Text = "Choose Height";
            SizeY.TextStyle.ForegroundColour = new Colour(255, 255, 255, 255);
            SizeY.Size = new Size(50, 50);
            SizeY.Location = this.mBackGround.Location + new Point((this.mBackGround.Size.Width / 10) * 3 - (this.mBackGround.Size.Width / 10) * 2, (this.mBackGround.Size.Height / 10) * 4 + 150);
            SizeY.AutoSize = true;
            SizeY.AutoSizeMode = Miyagi.UI.AutoSizeMode.GrowAndShrink;
            this.mGUI.Controls.Add(SizeY);
            
        }

        void textBoxSizeY_Submit(object sender, ValueEventArgs<string> e)
        {
            int sizeY;
            if (int.TryParse(e.Data, out sizeY))
            {
                Mogre.Vector2 size = this.mGameInfo.Size;
                size.y = sizeY;
                this.mGameInfo.Size = size;
            }
        }

        void textBoxSizeX_Submit(object sender, ValueEventArgs<string> e)
        {
            int sizeX;
            if (int.TryParse(e.Data, out sizeX)) 
            {
                Mogre.Vector2 size = this.mGameInfo.Size;
                size.x = sizeX;
                this.mGameInfo.Size = size;
            }
        }

        void textBoxSeed_Submit(object sender, ValueEventArgs<string> e)
        {
            int seed;
            if (int.TryParse(e.Data, out seed)) { this.mGameInfo.Seed = seed; }
        }

        protected override void AfterResize()
        {
            this.mLabel.Location = this.mBackGround.Location + new Point((this.mBackGround.Size.Width - this.mLabel.Size.Width) / 2, 15);
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
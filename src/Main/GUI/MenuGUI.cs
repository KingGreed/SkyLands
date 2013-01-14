using System;

using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Layout;
using Miyagi.Common.Events;
using Miyagi.UI.Controls.Styles;

namespace Game.GUICreator
{
	public class MenuGUI : GUIFactory
    {
        private Button mPlayButton, mExitButton;
        
        public EventHandler<MouseButtonEventArgs> MouseClickPlayButton
        {
            set { this.mPlayButton.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(value); }
        }
        public EventHandler<MouseButtonEventArgs> MouseClickExitButton
        {
            set { this.mExitButton.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(value); }
        }

        public MenuGUI(MiyagiMgr miyagiMgr, string name) : base(miyagiMgr, name) {}

        protected override void CreateGUI()
        {
            /* Backgroung image */
            PictureBox backgroung = new PictureBox();
            backgroung.AlwaysOnBottom = true;
            backgroung.Bitmap = new System.Drawing.Bitmap(@"../../src/Media/skins/background.bmp");
            backgroung.Size = this.WndSize;
            this.mGUI.Controls.Add(backgroung);
            
            /* Buttons */
            Point pos = new Point(this.WndSize.Width / 10, this.WndSize.Height / 4);
            Size size = new Size(250, 65);
            TextStyle style = new TextStyle();
            style.Alignment = Alignment.MiddleCenter;
            this.mPlayButton = new Button();
            this.mPlayButton.Size = size;
            this.mPlayButton.Text = "Play";
            this.mPlayButton.TextStyle = style;
            //this.mPlayButton.TextBounds = new Rectangle(new Point(), new Size()));
            this.mPlayButton.Location = pos;
            this.mPlayButton.Skin = this.mMiyagiMgr.Skins["Button"];
            this.mGUI.Controls.Add(this.mPlayButton);

            this.mExitButton = new Button();
            this.mExitButton.Size = size;
            this.mExitButton.Text = "Exit";
            this.mExitButton.TextStyle = style;
            this.mExitButton.Location = pos + new Point(0, (int)(size.Height * 1.3f));
            this.mExitButton.Skin = this.mMiyagiMgr.Skins["Button"];
            this.mGUI.Controls.Add(this.mExitButton);
        }
	}
}
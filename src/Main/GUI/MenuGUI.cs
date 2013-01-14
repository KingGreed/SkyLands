using System;

using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Layout;
using Miyagi.Common.Events;

namespace Game.GUICreator
{
	public class MenuGUI : GUIFactory
    {
        private Button mPlayButton;
        
        public EventHandler<MouseButtonEventArgs> MouseClickPlayButton
        {
            set { this.mPlayButton.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(value); }
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
            this.mPlayButton = new Button();
            this.mPlayButton.Size = new Size(90, 60);
            this.mPlayButton.Text = "Play";
            this.mPlayButton.Location = new Point(this.WndSize.Width / 10, this.WndSize.Height / 4);
            this.mPlayButton.Skin = this.mMiyagiMgr.Skins["Button"];
            this.mGUI.Controls.Add(this.mPlayButton);
        }
	}
}
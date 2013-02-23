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
    public class OptionsGUI : GUIFactory
    {
        public enum ButtonName { HighQuality, FullScreen, VSync, Music}
        private Dictionary<ButtonName, Button> mButtons;
        private Panel mPanel;
        private Label mLabel;
        private Button mBackButton;

        public Dictionary<ButtonName, Button> Buttons { get { return this.mButtons; } }

        public OptionsGUI(StateManager stateMgr, string name) : base(stateMgr, name) { }
        
        protected override void CreateGUI()
        {
            /* Panel */
            this.mPanel = new Panel();
            this.mPanel.AlwaysOnBottom = true;
            this.mPanel.Skin = this.mMiyagiMgr.Skins["PanelSkin"];
            this.mPanel.Size = new Size(800, 700);
            this.mPanel.Location = new Point(380, 100);
            this.mGUI.Controls.Add(this.mPanel);

            /* Buttons */
            TextStyle style = new TextStyle();
            style.Alignment = Alignment.MiddleCenter;
            style.Font = this.mMiyagiMgr.Fonts["Small_BlueHighway"];
            Size actualButtonSize = new Size(150, 60);
            int space = 30;
            Point originalpos = this.mPanel.Location + new Point(450, 150);

            this.mButtons = new Dictionary<ButtonName, Button>();
            for (int i = 0; i < Enum.GetValues(typeof(ButtonName)).Length; i++)
            {

                Button button = new Button();
                button.Size = actualButtonSize;
                button.Text = "ON/OFF";
                button.TextStyle = style;
                button.Location = originalpos + new Point(0, i * (actualButtonSize.Height + space));
                button.Skin = this.mMiyagiMgr.Skins["Button"];
                this.mGUI.Controls.Add(button);
                this.mButtons.Add((ButtonName)i, button);
            }
            mBackButton = new Button();
            mBackButton.Size = actualButtonSize;
            mBackButton.Text = "BACK";
            mBackButton.TextStyle = style;
            mBackButton.Location = this.mPanel.Location + new Point(this.mPanel.Size.Width /10, (this.mPanel.Size.Height /10) *9);
            mBackButton.Skin = this.mMiyagiMgr.Skins["Button"];
            this.mGUI.Controls.Add(mBackButton);
            
            /* Text */
            this.mLabel = new Label();
            this.mLabel.Text = "Options";
            this.mLabel.Size = new Size(this.mPanel.Size.Width, 60);
            this.mLabel.AutoSize = true;
            this.mLabel.AutoSizeMode = Miyagi.UI.AutoSizeMode.GrowAndShrink;
            this.mGUI.Controls.Add(this.mLabel);

            Label[] labels = new Label[4];
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i] = new Label();
                labels[i].Text = Enum.GetName(typeof(ButtonName), (ButtonName)i);
                labels[i].Size = new Size(this.mPanel.Size.Width / 2, 60);
                labels[i].AutoSize = true;
                labels[i].AutoSizeMode = Miyagi.UI.AutoSizeMode.GrowAndShrink;
                labels[i].Location = this.mButtons[(ButtonName)i].Location - new Point(labels[i].Size.Width * 3 , 0);
                this.mGUI.Controls.Add(labels[i]);
            }

        }

        protected override void AfterResize()
        {
            this.mLabel.Location = this.mPanel.Location + new Point((this.mPanel.Size.Width - this.mLabel.Size.Width) / 2, 15);
        }

        public void SetListener(ButtonName button, EventHandler<MouseButtonEventArgs> del)
        {
            this.mButtons[button].MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(del);
        }

        public void SetListenerBack(EventHandler<MouseButtonEventArgs> del)
        {
            this.mBackButton.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(del);
        }
    }
}
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
        private Button mBackButton;
        private Button mSaveButton;
        private Button mLoadButton;

        public Dictionary<ButtonName, Button> Buttons { get { return this.mButtons; } }

        public OptionsGUI(StateManager stateMgr, string name) : base(stateMgr, name) { }
        
        protected override void CreateGUI()
        {
            /* Panel */
            Panel panel = new Panel();
            panel.AlwaysOnBottom = true;
            panel.Skin = this.mMiyagiMgr.Skins["PanelSkin"];
            panel.Size = new Size(800, 700);
            panel.Location = new Point(380, 100);
            this.mGUI.Controls.Add(panel);

            /* Buttons */
            TextStyle style = new TextStyle();
            style.Alignment = Alignment.MiddleCenter;
            style.Font = this.mMiyagiMgr.Fonts["Small_BlueHighway"];
            Size actualButtonSize = new Size(150, 60);
            int space = 30;
            Point originalpos = panel.Location + new Point(450, 120);

            this.mButtons = new Dictionary<ButtonName, Button>();
            for (int i = 0; i < Enum.GetValues(typeof(ButtonName)).Length; i++)
            {
                Button button = new Button();
                button.Size = actualButtonSize;

                button.Text = "OFF";
                if ((ButtonName)i == ButtonName.HighQuality || (ButtonName)i == ButtonName.VSync)
                    button.Text = "ON";

                button.TextStyle = style;
                button.Location = originalpos + new Point(0, i * (actualButtonSize.Height + space));
                button.Skin = this.mMiyagiMgr.Skins["Button"];
                this.mGUI.Controls.Add(button);
                this.mButtons.Add((ButtonName)i, button);
            }

            this.mBackButton = new Button();
            this.mBackButton.Size = actualButtonSize;
            this.mBackButton.Text = "BACK";
            this.mBackButton.TextStyle = style;
            this.mBackButton.Location = panel.Location + new Point(panel.Size.Width /10, (panel.Size.Height /10) *9);
            this.mBackButton.Skin = this.mMiyagiMgr.Skins["Button"];
            this.mGUI.Controls.Add(this.mBackButton);

            this.mSaveButton = new Button();
            this.mSaveButton.Size = actualButtonSize;
            this.mSaveButton.Text = "SAVE";
            this.mSaveButton.TextStyle = style;
            this.mSaveButton.Location = panel.Location + new Point((panel.Size.Width / 10) * 5 + 50 , (panel.Size.Height / 10) * 7 + 20);
            this.mSaveButton.Skin = this.mMiyagiMgr.Skins["Button"];
            this.mGUI.Controls.Add(this.mSaveButton);

            this.mLoadButton = new Button();
            this.mLoadButton.Size = actualButtonSize;
            this.mLoadButton.Text = "LOAD";
            this.mLoadButton.TextStyle = style;
            this.mLoadButton.Location = panel.Location + new Point((panel.Size.Width / 10) * 2 + 50, (panel.Size.Height / 10) * 7 + 20);
            this.mLoadButton.Skin = this.mMiyagiMgr.Skins["Button"];
            this.mGUI.Controls.Add(this.mLoadButton);

            
            /* Text */
            Label label = new Label();
            label.Text = "Options";
            label.Size = new Size(panel.Size.Width, 60);
            label.AutoSize = true;
            label.AutoSizeMode = Miyagi.UI.AutoSizeMode.GrowAndShrink;
            label.Location = panel.Location + new Point(panel.Size.Width / 2 - 50, 0);
            this.mGUI.Controls.Add(label);

            Label[] labels = new Label[4];
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i] = new Label();
                labels[i].Text = Enum.GetName(typeof(ButtonName), (ButtonName)i);
                labels[i].Size = new Size(panel.Size.Width / 2, 60);
                labels[i].AutoSize = true;
                labels[i].AutoSizeMode = Miyagi.UI.AutoSizeMode.GrowAndShrink;
                labels[i].Location = this.mButtons[(ButtonName)i].Location + new Point(-250 , 10);
                this.mGUI.Controls.Add(labels[i]);
            }

        }

        /*protected override void AfterResize()
        {
            label.Location = panel.Location + new Point((panel.Size.Width - label.Size.Width) / 2, 15);
        }*/

        public void SetListener(ButtonName button, EventHandler<MouseButtonEventArgs> del)
        {
            this.mButtons[button].MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(del);
        }

        public void SetListenerBack(EventHandler<MouseButtonEventArgs> del)
        {
            this.mBackButton.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(del);
        }

        public void SetListenerSave(EventHandler<MouseButtonEventArgs> del)
        {
            this.mSaveButton.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(del);
        }

        public void SetListenerLoad(EventHandler<MouseButtonEventArgs> del)
        {
            this.mLoadButton.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(del);
        }
    }
}
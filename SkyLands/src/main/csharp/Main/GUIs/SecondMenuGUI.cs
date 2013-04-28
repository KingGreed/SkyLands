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
        public enum ButtonName { NewGame, Continue, Debug}
        
        private Button[] mButtons;
        private Panel mPanel;
        private Label mSaveMsg;

        public SecondMenuGUI(StateManager stateMgr) : base(stateMgr, "SecondMenu") { }

        protected override void CreateGUI()
        {
            /* Panel */
            this.mPanel = new Panel();
            this.mPanel.AlwaysOnBottom = true;
            this.mPanel.Skin = this.mMiyagiMgr.Skins["PanelSkin"];
            this.mPanel.Size = new Size(864, 220);
            this.mPanel.Location = new Point(346, 365);
            this.mGUI.Controls.Add(this.mPanel);

            /* Buttons */
            string[] names = (string[]) Enum.GetNames(typeof(ButtonName));
            this.mButtons = new Button[names.Length];
            TextStyle buttonStyle = new TextStyle();
            buttonStyle.Alignment = Alignment.MiddleCenter;
            buttonStyle.Font = this.mMiyagiMgr.Fonts["Small_BlueHighway"];
            Size buttonSize = new Size(150, 60);
            int space = (this.mPanel.Width - names.Length * buttonSize.Width) / (names.Length + 1);

            for (int i = 0; i < this.mButtons.Length; i++)
            {
                this.mButtons[i] = new Button();
                this.mButtons[i].Size = buttonSize;
                this.mButtons[i].Text = names[i];
                this.mButtons[i].TextStyle = buttonStyle;
                this.mButtons[i].Location = this.mPanel.Location + new Point((space + buttonSize.Width) * i + space, this.mPanel.Size.Height / 2 - this.mButtons[i].Size.Height / 2);
                this.mButtons[i].Skin = this.mMiyagiMgr.Skins["Button"];
                this.mGUI.Controls.Add(this.mButtons[i]);
            }

            TextStyle style = new TextStyle();
            style.Alignment = Alignment.MiddleCenter;
            style.Font = this.mMiyagiMgr.Fonts["Small_BlueHighway"];
            style.ForegroundColour = new Colour(255, 255, 255, 255);
            this.mSaveMsg = new Label();
            this.mSaveMsg.Text = "World successfully saved";
            this.mSaveMsg.Size = new Size(500, 50);
            this.mSaveMsg.Location = this.mPanel.Location + new Point(this.mPanel.Width / 2 - this.mSaveMsg.Width / 2, this.mPanel.Height - this.mSaveMsg.Size.Height);
            this.mSaveMsg.TextStyle = style;
            this.mSaveMsg.Visible = false;
            this.mGUI.Controls.Add(this.mSaveMsg);
        }

        public void ShowSaveMessage(bool show)
        {
            this.mSaveMsg.Visible = show;
        }

        public void SetListener(ButtonName name, EventHandler<MouseButtonEventArgs> del)
        {
            this.mButtons[(int)name].MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(del);
        }
    }
}
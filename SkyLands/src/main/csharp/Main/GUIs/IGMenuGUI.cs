using System;

using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.UI.Controls;
using Miyagi.Common.Events;
using Miyagi.UI.Controls.Styles;

using Game.States;

namespace Game.GUICreator
{
    public class IGMenuGUI : GUIFactory
    {
        public enum ButtonName { Menu, Options, Save}
        
        private Button[] mButtons;
        private Label mSaveMsg;

        public IGMenuGUI(StateManager stateMgr) : base(stateMgr, "IG menu") { }

        protected override void CreateGUI()
        {
            /* Panel */
            Panel panel = new Panel
            {
                AlwaysOnBottom = true,
                Skin = this.mMiyagiMgr.Skins["PanelSkin"],
                Size = new Size(864, 220),
                Location = new Point(346, 365)
            };
            this.mGUI.Controls.Add(panel);

            /* Buttons */
            string[] names = Enum.GetNames(typeof(ButtonName));
            this.mButtons = new Button[names.Length];
            TextStyle buttonStyle = new TextStyle
            {
                Alignment = Alignment.MiddleCenter,
                Font = this.mMiyagiMgr.Fonts["Small_BlueHighway"]
            };
            Size buttonSize = new Size(150, 60);
            int space = (panel.Width - names.Length * buttonSize.Width) / (names.Length + 1);

            for (int i = 0; i < this.mButtons.Length; i++)
            {
                this.mButtons[i] = new Button {Size = buttonSize, Text = names[i], TextStyle = buttonStyle};
                this.mButtons[i].Location = panel.Location + new Point((space + buttonSize.Width) * i + space, panel.Size.Height / 2 - this.mButtons[i].Size.Height / 2);
                this.mButtons[i].Skin = this.mMiyagiMgr.Skins["Button"];
                this.mGUI.Controls.Add(this.mButtons[i]);
            }

            TextStyle style = new TextStyle
            {
                Alignment = Alignment.MiddleCenter,
                Font = this.mMiyagiMgr.Fonts["Small_BlueHighway"],
                ForegroundColour = new Colour(255, 255, 255, 255)
            };
            this.mSaveMsg = new Label {Text = "World successfully saved", Size = new Size(500, 50)};
            this.mSaveMsg.Location = panel.Location + new Point(panel.Width / 2 - this.mSaveMsg.Width / 2, panel.Height - this.mSaveMsg.Size.Height);
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
            this.mButtons[(int)name].MouseClick += del;
        }
    }
}
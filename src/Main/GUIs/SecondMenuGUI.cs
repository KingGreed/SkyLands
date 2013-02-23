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
        private Panel mPanel;
        private Label mLabel;

        public SecondMenuGUI(StateManager stateMgr, string name) : base(stateMgr, name) { }

        protected override void CreateGUI()
        {
            /* Panel */
            this.mPanel = new Panel();
            this.mPanel.AlwaysOnBottom = true;
            this.mPanel.Skin = this.mMiyagiMgr.Skins["PanelSkin"];
            this.mPanel.Size = new Size(864, 220);
            this.mPanel.Location = new Point(346, 365);
            this.mGUI.Controls.Add(this.mPanel);

            /* Text */
            this.mLabel = new Label();
            this.mLabel.Text = "Choose an Island";
            this.mLabel.Size = new Size(this.mPanel.Size.Width, 50);
            this.mLabel.AutoSize = true;
            this.mLabel.AutoSizeMode = Miyagi.UI.AutoSizeMode.GrowAndShrink;
            this.mGUI.Controls.Add(this.mLabel);

            /* Buttons */
            TextStyle style = new TextStyle();
            style.Alignment = Alignment.MiddleCenter;
            style.Font = this.mMiyagiMgr.Fonts["Small_BlueHighway"];
            Size actualButtonSize = new Size(150, 60);
            int space = (this.mPanel.Size.Width - 4 * actualButtonSize.Width) / 5;
            Point originalpos = this.mPanel.Location + new Point(space, 65);

            this.mButtons = new Dictionary<string, Button>();
            for (int i = 0; i < Enum.GetValues(typeof(StateManager.TypeWorld)).Length; i++)
            {

                Button button = new Button();
                button.Size = actualButtonSize;
                button.Text = Enum.GetName(typeof(StateManager.TypeWorld), (StateManager.TypeWorld)i);
                button.TextStyle = style;
                button.Location = originalpos + new Point(i * (actualButtonSize.Width + space), 0);
                button.Skin = this.mMiyagiMgr.Skins["Button"];
                this.mGUI.Controls.Add(button);
                this.mButtons.Add(button.Text, button);
            }
        }

        protected override void AfterResize()
        {
            this.mLabel.Location = this.mPanel.Location + new Point((this.mPanel.Size.Width - this.mLabel.Size.Width) / 2, 15);
        }

        public void SetListener(StateManager.TypeWorld button, EventHandler<MouseButtonEventArgs> del)
        {
            this.mButtons[Enum.GetName(typeof(StateManager.TypeWorld), button)].MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(del);
        }
    }
}
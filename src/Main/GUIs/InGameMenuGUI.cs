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
    public class InGameMenuGUI : GUIFactory
    {
        Button mMainMenu, mOption;
        private Panel mPanel;

        public InGameMenuGUI(StateManager stateMgr) : base(stateMgr, "IG menu") { }

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
            TextStyle style = new TextStyle();
            style.Alignment = Alignment.MiddleCenter;
            style.Font = this.mMiyagiMgr.Fonts["Small_BlueHighway"];
            this.mMainMenu = new Button();
            this.mMainMenu.Size = new Size(150, 60);
            this.mMainMenu.Text = "Menu";
            this.mMainMenu.TextStyle = style;
            this.mMainMenu.Location = this.mPanel.Location + new Point(50, 50);
            this.mMainMenu.Skin = this.mMiyagiMgr.Skins["Button"];

            this.mOption = new Button();
            this.mOption.Size = new Size(150, 60);
            this.mOption.Text = "Menu";
            this.mOption.TextStyle = style;
            this.mOption.Location = this.mMainMenu.Location + new Point(this.mMainMenu.Size.Width + 50, 0);
            this.mOption.Skin = this.mMiyagiMgr.Skins["Button"];
            /*Size actualButtonSize = new Size(150, 60);
            int space = (this.mPanel.Size.Width - 4 * actualButtonSize.Width) / 5;
            Point originalpos = this.mPanel.Location + new Point(space, 65);

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
            }*/
        }

        public void SetListenerMenu(EventHandler<MouseButtonEventArgs> del)
        {
            this.mMainMenu.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(del);
        }

        public void SetListenerOption(EventHandler<MouseButtonEventArgs> del)
        {
            this.mOption.MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(del);
        }
    }
}
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
    public class TempMenuGUI : GUIFactory
    {
        private Dictionary<string, Button> mButtons;

        public TempMenuGUI(MiyagiMgr miyagiMgr, string name) : base(miyagiMgr, name) { }

        protected override void CreateGUI()
        {
            Size originalImgSize = new Size(1600, 900);
            float ratioX = (float)this.WndSize.Width / (float)originalImgSize.Width;
            float ratioY = (float)this.WndSize.Height / (float)originalImgSize.Height;

            /* Panel */
            Size originalPanelSize = new Size(864, 220);
            Point originalPanelPos = new Point(346, 365);
            Panel panel = new Panel();
            panel.AlwaysOnBottom = true;
            panel.Skin = this.mMiyagiMgr.Skins["PanelSkin"];
            panel.Size = new Size((int)(originalPanelSize.Width * ratioX), (int)(originalPanelSize.Height * ratioY));
            panel.Location = new Point((int)(originalPanelPos.X * ratioX), (int)(originalPanelPos.Y * ratioY));
            this.mGUI.Controls.Add(panel);

            /* Text */
            Size originalTextSize = new Size(205, 20);
            Label label = new Label();
            label.Text = "Choose an Island";
            label.AutoSize = true;
            label.Size = new Size((int)(originalTextSize.Width * ratioX), (int)(originalTextSize.Height * ratioY));
            Point originalTextPos = originalPanelPos + new Point((originalPanelSize.Width - originalTextSize.Width) / 2, 15);
            label.Location = new Point((int)(originalTextPos.X * ratioX), (int)(originalTextPos.Y * ratioY));
            this.mGUI.Controls.Add(label);

            /* Buttons */
            TextStyle style = new TextStyle();
            style.Alignment = Alignment.MiddleCenter;
            style.Font = this.mMiyagiMgr.Fonts["Small_BlueHighway"];
            Size originalButtonSize = new Size(150, 60);
            Size actualButtonSize = new Size((int)(originalButtonSize.Width * ratioX), (int)(originalButtonSize.Height * ratioY));
            int space = (panel.Size.Width - 4 * actualButtonSize.Width) / 5;
            Point originalpos = panel.Location + new Point(space, 65);

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

        public void SetListener(StateManager.TypeWorld button, EventHandler<MouseButtonEventArgs> del)
        {
            this.mButtons[Enum.GetName(typeof(StateManager.TypeWorld), button)].MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(del);
        }
    }
}
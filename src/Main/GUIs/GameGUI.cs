using System;
using System.Collections.Generic;

using Miyagi.Common.Events;
using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Layout;
using Miyagi.UI.Controls.Styles;

namespace Game.GUICreator
{
    public class GameGUI : GUIFactory
    {
        private TextBox mTextBox;
        
        public GameGUI(MiyagiMgr miyagiMgr, string name) : base(miyagiMgr, name) { }

        protected override void CreateGUI()
        {
            /* Text
            TextStyle style = new TextStyle();
            style.Alignment = Alignment.MiddleCenter;
            style.Font = this.mMiyagiMgr.Fonts["Small_BlueHighway"];
            Label label = new Label();
            label.Text = "World loading ...";
            label.Size = new Size(250, 50);
            label.Location = new Point(660, 515);
            label.TextStyle = style;
            this.mGUI.Controls.Add(label);*/

            this.mTextBox = new TextBox();
            this.mTextBox.AlwaysOnTop = true;
            this.mTextBox.MouseClick += OnConsoleMouseClick;
            this.mTextBox.Size = new Size(250, 50);
            this.mTextBox.Location = new Point(0, this.WndSize.Height - this.mTextBox.Size.Height);
        }

        private void OnConsoleMouseClick(object sender, MouseEventArgs e)
        {
            this.mTextBox.Enabled = true;
        }
    }
}
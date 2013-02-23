using System;
using System.Collections.Generic;

using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Layout;
using Miyagi.UI.Controls.Styles;

using Game.States;

namespace Game.GUICreator
{
    public class LoadingGUI : GUIFactory
    {
        public LoadingGUI(StateManager stateMgr, string name) : base(stateMgr, name) { }

        protected override void CreateGUI()
        {
            /* Text */
            TextStyle style = new TextStyle();
            style.Alignment = Alignment.MiddleCenter;
            style.Font = this.mMiyagiMgr.Fonts["Small_BlueHighway"];
            Label label = new Label();
            label.Text = "World loading ...";
            label.Size = new Size(250, 50);
            label.Location = new Point(660, 515);
            label.TextStyle = style;
            this.mGUI.Controls.Add(label);
        }
    }
}
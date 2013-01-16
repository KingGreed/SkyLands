using System;
using System.Collections.Generic;

using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Layout;
using Miyagi.Common.Events;
using Miyagi.UI.Controls.Styles;

namespace Game.GUICreator
{
    public class LoadingGUI : GUIFactory
    {
        public LoadingGUI(MiyagiMgr miyagiMgr, string name) : base(miyagiMgr, name) { }

        protected override void CreateGUI()
        {
            Size originalImgSize = new Size(1600, 900);
            float ratioX = (float)this.WndSize.Width / (float)originalImgSize.Width;
            float ratioY = (float)this.WndSize.Height / (float)originalImgSize.Height;

            /* Text */
            TextStyle style = new TextStyle();
            style.Alignment = Alignment.MiddleCenter;
            style.Font = this.mMiyagiMgr.Fonts["Small_BlueHighway"];
            Size originalTextSize = new Size(200, 70);
            Point originalTextPos = new Point(660, 515);
            Label label = new Label();
            label.Text = "World loading ...";
            label.Size = new Size((int)(originalTextSize.Width * ratioX), (int)(originalTextSize.Height * ratioY));
            label.Location = new Point((int)(originalTextPos.X * ratioX), (int)(originalTextPos.Y * ratioY));
            label.TextStyle = style;
            this.mGUI.Controls.Add(label);
        }
    }
}
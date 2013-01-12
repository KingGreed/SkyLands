using System;

using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.UI;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Layout;

namespace Game.GUICreator
{
	public class MenuGUI : GUIFactory
    {
        public MenuGUI(MiyagiManager miyagiMgr, string name) : base(miyagiMgr, name) {}

        protected override void CreateGUI()
        {
            /* A red label in the center of the screen */
            Label label = new Label();
            label.Size = new Size(150, 50);
            label.Location = new Point(this.Width / 2 - label.Size.Width / 2,
                                       this.Height / 2 - label.Size.Height / 2);
            label.TextStyle.ForegroundColour = new ColourDefinition(new Colour(255, 255, 0, 0));
            label.Text = "Press E to play";
            this.mGUI.Controls.Add(label);
        }
	}
}
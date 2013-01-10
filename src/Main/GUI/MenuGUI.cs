using System;

using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.UI;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Layout;

using Mogre;
using Game.States;

namespace Game.GUICreator {

	public class MenuGUI : GUIFactory {
        public MenuGUI(StateManager stateMgr, string name) : base(stateMgr, name) {}
        public override void createMenu() {
            /* A red label in the center of the screen */            
            Label label = new Label();
            label.Size = new Size(150, 50);
            label.Location = new Point((int) this.mStateMgr.Window.Width / 2 - label.Size.Width / 2,
                                       (int) this.mStateMgr.Window.Height / 2 - label.Size.Height / 2);
            label.TextStyle.ForegroundColour = new ColourDefinition(new Colour(255, 255, 0, 0));
            label.Text = "Press E to play";
            this.mGUI.Controls.Add(label);

            Mogre.LogManager.Singleton.DefaultLog.LogMessage("Menu Created");
        }

	}
}
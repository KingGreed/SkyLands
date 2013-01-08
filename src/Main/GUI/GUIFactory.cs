using System;

using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.UI;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Layout;

using Mogre;

using Game.State;

namespace Game.GUI {

	public abstract class GUIFactory {
		private GUI mMenuGUI;
        private MiyagiSystem mSystem;
        private StateManager mStateMgr;

        public GUIFactory(StateManager stateMgr, string name) {

            this.mStateMgr = stateMgr;
            this.mSystem   = this.mStateMgr.MiyagiManager.System;
            this.mMenuGUI  = new GUI("World GUI");

            this.CreateMenu();
            this.mSystem.GUIManager.GUIs.Add(this.mMenuGUI);
        }

        public void abstract createMenu();

	}
}
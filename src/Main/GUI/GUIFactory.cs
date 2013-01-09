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

	public abstract class GUIFactory {
		protected GUI mMenuGUI;
        protected MiyagiSystem mSystem;
        protected StateManager mStateMgr;

        public GUIFactory(StateManager stateMgr, string name) {

            this.mStateMgr = stateMgr;
            this.mSystem   = this.mStateMgr.MiyagiManager.System;
            this.mMenuGUI  = new GUI("World GUI");

            this.createMenu();
            this.mSystem.GUIManager.GUIs.Add(this.mMenuGUI);
        }

        public abstract void createMenu();

	}
}
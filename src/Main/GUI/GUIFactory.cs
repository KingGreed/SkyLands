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
		protected GUI mGUI;
        protected MiyagiSystem mSystem;
        protected StateManager mStateMgr;

        public GUIFactory(StateManager stateMgr, string name) {

            this.mStateMgr = stateMgr;
            this.mSystem   = this.mStateMgr.MiyagiManager.System;
            this.mGUI  = new GUI("World GUI");

            this.createMenu();
            this.mSystem.GUIManager.GUIs.Add(this.mGUI);
        }

        public abstract void createMenu();

        public void Shutdown() { this.mGUI.Dispose();       }
        public void Hide()     { this.mGUI.Visible = false; }
        public void Show()     { this.mGUI.Visible = true;  }

	}
}
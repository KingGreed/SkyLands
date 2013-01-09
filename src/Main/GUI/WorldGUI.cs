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

	public class WorldGUI : GUIFactory {

        private ProgressBar mProgress;

        public WorldGUI(StateManager stateMgr, string name) : base(stateMgr, name) {}
        public override void  createMenu() {
            this.mProgress = new ProgressBar();
            this.mProgress.Size = new Size(300, 50);
            //this.mProgress.Skin = skinDict["ProgressBar"];
            this.mProgress.Location = new Point(50, 50);
            this.mMenuGUI.Controls.Add(this.mProgress);
        }

	}
}
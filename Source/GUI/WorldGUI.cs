using System;

using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.UI;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Layout;

using Mogre;

namespace Game.GUI {

	public class WorldGUI : GUIFactory {

        private ProgressBar mProgress;

        p

        public void override createMenu() {
            progress = new ProgressBar();
            progress.Size = new Size(300, 50);
            progress.Skin = skinDict["ProgressBar"];
            progress.Location = new Point(50, 50);
            gui.Controls.Add(progress);
        }

        protected override void UpdateProgress(int i)
        {
            /*barval += 0.1f;
            progress.Value = (int)barval;*/

            system.Update();
            //base.UpdateScene(evt);
        }
	}
}
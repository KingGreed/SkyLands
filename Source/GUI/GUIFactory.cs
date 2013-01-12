using System;

using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.UI;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Layout;

using Game.States;

namespace Game.GUICreator
{
	public abstract class GUIFactory
    {
        protected MiyagiManager mMiyagiMgr;
        protected GUI mGUI;

        protected int Width  { get { return (int) this.mMiyagiMgr.WindowSize.x; } }
        protected int Height { get { return (int) this.mMiyagiMgr.WindowSize.y; } }

        public GUIFactory(MiyagiManager miyagiMgr, string name)
        {
            this.mMiyagiMgr = miyagiMgr;
            this.mGUI       = new GUI(name);
            this.CreateGUI();
            this.mMiyagiMgr.Add(this.mGUI);
        }

        protected abstract void CreateGUI();

        public void Dispose()  { this.mGUI.Dispose();       }
        public void Hide()     { this.mGUI.Visible = false; }
        public void Show()     { this.mGUI.Visible = true;  }

	}
}
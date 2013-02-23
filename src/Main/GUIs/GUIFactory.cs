using System;

using Miyagi.Common.Data;
using Miyagi.UI;

using Game.States;

namespace Game.GUICreator
{
	public abstract class GUIFactory
    {
        private StateManager mStateMgr;
        protected MiyagiMgr  mMiyagiMgr;
        protected GUI        mGUI;
        protected Size       mOriginalWndSize = new Size(1600, 900);

        protected Size WndSize  { get { return this.mMiyagiMgr.WndSize; } }
        private float RatioX    { get { return (float)this.WndSize.Width / (float)this.mOriginalWndSize.Width; } }
        private float RatioY    { get { return (float)this.WndSize.Height / (float)this.mOriginalWndSize.Height; } }
        
        public GUIFactory(StateManager stateMgr, string name)
        {
            this.mStateMgr = stateMgr;
            this.mMiyagiMgr = stateMgr.MiyagiMgr;
            this.mGUI       = new GUI(name);

            this.CreateGUI();
            this.mGUI.Resize(this.RatioX, this.RatioY);
            this.AfterResize();

            this.mMiyagiMgr.Add(this.mGUI);

            if (this.mStateMgr.MyConsole != null)
                this.mStateMgr.MyConsole.UpdateZOder(this.mGUI.ZOrder + 1);
        }

        protected abstract void CreateGUI();
        protected virtual void AfterResize() { }

        public void Dispose()  { this.mGUI.Dispose();       }
        public void Hide()     { this.mGUI.Visible = false; }
        public void Show()     { this.mGUI.Visible = true;  }
	}
}
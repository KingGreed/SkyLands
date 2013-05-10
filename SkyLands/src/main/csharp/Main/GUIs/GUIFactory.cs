using Miyagi.Common.Data;
using Miyagi.UI;

using Game.States;

namespace Game.GUICreator
{
	public abstract class GUIFactory
    {
        protected StateManager mStateMgr;
        protected MiyagiMgr  mMiyagiMgr;
        protected GUI        mGUI;
        protected Size       mOriginalWndSize = new Size(1600, 900);

        protected Size WndSize          { get { return this.mMiyagiMgr.WndSize; } }
        private float  RatioX           { get { return this.WndSize.Width / (float)this.mOriginalWndSize.Width; } }
        private float  RatioY           { get { return this.WndSize.Height / (float)this.mOriginalWndSize.Height; } }
        public bool    IsGUIVisible     { get { return this.mGUI.Visible; } }

	    protected GUIFactory(StateManager stateMgr, string name)
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

        public virtual void Dispose()  { this.mGUI.Dispose(); }
        public virtual void Hide()     { this.mGUI.Visible = false; }
        public virtual void Show()     { this.mGUI.Visible = true; }
        public void SwitchVisibility() { this.mGUI.Visible = !this.mGUI.Visible; }
	}
}
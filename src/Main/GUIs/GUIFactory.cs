using System;

using Miyagi.Common.Data;
using Miyagi.UI;

namespace Game.GUICreator
{
	public abstract class GUIFactory
    {
        protected MiyagiMgr mMiyagiMgr;
        protected GUI       mGUI;
        protected Size      mOriginalWndSize = new Size(1600, 900);

        protected Size WndSize  { get { return this.mMiyagiMgr.WndSize; } }
        private float RatioX    { get { return (float)this.WndSize.Width / (float)this.mOriginalWndSize.Width; } }
        private float RatioY    { get { return (float)this.WndSize.Height / (float)this.mOriginalWndSize.Height; } }
        
        public GUIFactory(MiyagiMgr miyagiMgr, string name)
        {
            this.mMiyagiMgr = miyagiMgr;
            this.mGUI       = new GUI(name);

            this.CreateGUI();
            this.mGUI.Resize(this.RatioX, this.RatioY);
            this.AfterResize();

            this.mMiyagiMgr.Add(this.mGUI);
        }

        protected abstract void CreateGUI();
        protected virtual void AfterResize() { }

        public void Dispose()  { this.mGUI.Dispose();       }
        public void Hide()     { this.mGUI.Visible = false; }
        public void Show()     { this.mGUI.Visible = true;  }

	}
}
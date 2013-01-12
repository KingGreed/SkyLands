using System;

using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.UI;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Layout;

namespace Game.GUICreator
{
    public class LoadingBarGUI : GUIFactory
    {
        private ProgressBar mProgress;
        private float mLoadingValue;

        public float Value
        {
            get { return this.mLoadingValue; }
            set
            {
                this.mLoadingValue = value;
                this.mProgress.Value = (int)this.mLoadingValue;
            }
        }

        public LoadingBarGUI(MiyagiManager miyagiMgr, string name) : base(miyagiMgr, name) {}

        protected override void CreateGUI()
        {
            this.mProgress = new ProgressBar();
            this.mLoadingValue = this.mProgress.Min;
            this.mProgress.Size = new Size(300, 50);
            this.mProgress.Skin = this.mMiyagiMgr.Skins["ProgressBar"];
            this.mProgress.Location = new Point(this.Width / 2 - this.mProgress.Size.Width / 2,
                                                this.Height / 2 - this.mProgress.Size.Height / 2);
            this.mGUI.Controls.Add(this.mProgress);
        }
    }
}
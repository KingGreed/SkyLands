using System;
using Miyagi.Common.Events;

using Game.GUICreator;

namespace Game.States
{
    public class LoadingState : State
    {
        private LoadingGUI mLoadingGUI;
        private bool mWaitOneFrame;

        public LoadingState(StateManager stateMgr) : base(stateMgr) { }

        protected override void Startup()
        {
            this.mWaitOneFrame = true;
            this.mLoadingGUI = new LoadingGUI(this.mStateMgr.MiyagiManager, "Loading GUI");
        }

        public override void Hide()
        {
            this.mLoadingGUI.Hide();
            this.mStateMgr.MiyagiManager.CursorVisibility = false;
        }

        public override void Show()
        {
            this.mLoadingGUI.Show();
            this.mStateMgr.MiyagiManager.CursorVisibility = false;
        }

        public override void Update(float frameTime) 
        {
            if (this.mWaitOneFrame) { this.mWaitOneFrame = false; return; }
            this.mStateMgr.RequestStatePop();
            this.mStateMgr.RequestStatePush(typeof(GameState));
        }

        protected override void Shutdown() { this.mLoadingGUI.Dispose(); }
    }
}

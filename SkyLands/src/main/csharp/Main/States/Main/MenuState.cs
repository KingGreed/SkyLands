using System;
using Mogre;

using Game.CharacSystem;
using Game.World;
using Game.BaseApp;

namespace Game.States
{
    class MenuState : State
    {
        public MenuState(StateManager stateMgr) : base(stateMgr, "Menu") { }
        
        protected override void Startup()
        {

        }

        public override void Show()
        {
            OgreForm.webView.Show();
        }

        public override void Hide()
        {
            OgreForm.webView.Hide();
        }

        public override void Update(float frameTime)
        {
            if (this.mStateMgr.Controller.HasActionOccured(Controller.UserAction.Start))
                this.mStateMgr.RequestStatePop();
            if (this.mStateMgr.Controller.HasActionOccured(Controller.UserAction.Jump))
                this.mStateMgr.RequestStatePush(typeof(GameState));
        }
        
        protected override void Shutdown()
        {

        }
    }
}

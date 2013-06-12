using Game.GUIs;

namespace Game.States
{
    class MenuState : State
    {
        public MenuState(StateManager stateMgr) : base(stateMgr, "Menu") { }
        
        public override void Show()
        {
            new MainMenu(this.mStateMgr);
        }

        public override void Update(float frameTime)
        {
            if (this.mStateMgr.Controller.HasActionOccured(Controller.UserAction.Start))
                this.mStateMgr.RequestStatePop();
            if (this.mStateMgr.Controller.HasActionOccured(Controller.UserAction.Jump))
                this.mStateMgr.RequestStatePush(typeof(GameState));
        }
    }
}

using Game.GUICreator;

namespace Game.States
{
    public class IGMenuState : State
    {
        private IGMenuGUI mGUI;

        public IGMenuState(StateManager stateMgr) : base(stateMgr, "IGMenuState") { }

        protected override void Startup()
        {
            this.mGUI = new IGMenuGUI(this.mStateMgr);
            this.mGUI.SetListener(IGMenuGUI.ButtonName.Menu, (sender, args) => this.mStateMgr.PopToMenu());
            this.mGUI.SetListener(IGMenuGUI.ButtonName.Options, (sender, args) => this.mStateMgr.RequestStatePush(typeof(OptionsState)));
            this.mGUI.SetListener(IGMenuGUI.ButtonName.Save, delegate { this.mStateMgr.MainState.Save(); this.mGUI.ShowSaveMessage(true); });
        }

        public override void Hide()
        {
            this.mStateMgr.MiyagiMgr.CursorVisibility = false;
        }

        public override void Show()
        {
            this.mGUI.Show();
            this.mStateMgr.MiyagiMgr.CursorVisibility = true;
        }

        public override void Update(float frameTime)
        {
            if (this.mStateMgr.Input.IsKeyDown(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(); }
        }

        protected override void Shutdown()
        {
            this.mGUI.Dispose();
            MainState mainState = this.mStateMgr.MainState;
            if (mainState != null)
                mainState.EnableMovement(true);
        }
    }
}

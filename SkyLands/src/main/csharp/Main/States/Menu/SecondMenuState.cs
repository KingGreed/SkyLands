using System;
using Miyagi.Common.Events;
using Miyagi.UI.Controls;

using Game.GUICreator;
using Mogre;
using Game.World;

namespace Game.States
{
    public class SecondMenuState : State
    {
        private SecondMenuGUI mSecondMenuGUI;

        public SecondMenuState(StateManager stateMgr) : base(stateMgr, "SecondMenu") { }

        protected override void Startup()
        {
            this.mSecondMenuGUI = new SecondMenuGUI(this.mStateMgr);
            this.mSecondMenuGUI.SetListener(SecondMenuGUI.ButtonName.NewGame, this.ClickNewGameButton);
            this.mSecondMenuGUI.SetListener(SecondMenuGUI.ButtonName.Continue, this.ClickContinueButton);
            this.mSecondMenuGUI.SetListener(SecondMenuGUI.ButtonName.Debug, this.ClickDebugButton);
        }

        public override void Hide()
        {
            this.mSecondMenuGUI.Hide();
        } 

        public override void Show()
        {
            this.mSecondMenuGUI.Show();
        }

        public override void Update(float frameTime)
        {
            if (this.mStateMgr.Input.IsKeyDown(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(); }
        }

        protected override void Shutdown() { this.mSecondMenuGUI.Dispose(); }

        private void ClickDebugButton(object obj, MouseButtonEventArgs arg) 
        {
            this.mStateMgr.RequestStatePush(typeof(DebugMenuState));
        }

        private void ClickContinueButton(object obj, MouseButtonEventArgs arg) 
        {
            this.mStateMgr.GameInfo = new GameInfo(true);
            this.mStateMgr.RequestStatePush(typeof(LoadingState));
        }

        private void ClickNewGameButton(object obj, MouseButtonEventArgs arg) 
        {
            this.mStateMgr.GameInfo = new GameInfo();
            this.mStateMgr.RequestStatePush(typeof(LoadingState));
        }
    }
}

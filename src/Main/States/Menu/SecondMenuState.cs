using System;
using Miyagi.Common.Events;

using Game.GUICreator;

namespace Game.States
{
    public class SecondMenuState : State
    {
        private SecondMenuGUI mTempMenuGUI;

        public SecondMenuState(StateManager stateMgr) : base(stateMgr) { }

        protected override void Startup()
        {
            this.mTempMenuGUI = new SecondMenuGUI(this.mStateMgr, "TempMenu GUI");
            this.mTempMenuGUI.SetListener(StateManager.TypeWorld.Sinus, this.ClickSinusButton);
            this.mTempMenuGUI.SetListener(StateManager.TypeWorld.Dome, this.ClickDomeButton);
            this.mTempMenuGUI.SetListener(StateManager.TypeWorld.Plain, this.ClickPlainButton);
            this.mTempMenuGUI.SetListener(StateManager.TypeWorld.Mountain, this.ClickMountainButton);
        }

        public override void Hide()
        {
            this.mStateMgr.MiyagiMgr.CursorVisibility = false;
        }

        public override void Show()
        {
            this.mTempMenuGUI.Show();
            this.mStateMgr.MiyagiMgr.CursorVisibility = true;
        }

        public override void Update(float frameTime)
        {
            if (this.mStateMgr.Input.IsKeyDown(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(); }
        }

        protected override void Shutdown() { this.mTempMenuGUI.Dispose(); }

        public void ClickButton()
        {
            //this.mStateMgr.RequestStatePop();
            this.mStateMgr.RequestStatePush(typeof(LoadingState));
        }
        private void ClickSinusButton(object obj, MouseButtonEventArgs arg)    { this.mStateMgr.ChosenWorld = StateManager.TypeWorld.Sinus; this.ClickButton(); }
        private void ClickDomeButton(object obj, MouseButtonEventArgs arg)     { this.mStateMgr.ChosenWorld = StateManager.TypeWorld.Dome; this.ClickButton(); }
        private void ClickPlainButton(object obj, MouseButtonEventArgs arg)    { this.mStateMgr.ChosenWorld = StateManager.TypeWorld.Plain; this.ClickButton(); }
        private void ClickMountainButton(object obj, MouseButtonEventArgs arg) { this.mStateMgr.ChosenWorld = StateManager.TypeWorld.Mountain; this.ClickButton(); }
    }
}

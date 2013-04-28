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
        private bool mIsFullScreen;

        public SecondMenuState(StateManager stateMgr) : base(stateMgr, "SecondMenu") { }

        protected override void Startup()
        {
            this.mGameInfo = new GameInfo();
            this.mGameInfo.Seed = 42;
            this.mGameInfo.Size = new Vector2(11, 11);

            this.mSecondMenuGUI = new SecondMenuGUI(this.mStateMgr, this.mGameInfo);
            this.mSecondMenuGUI.SetListener(GameInfo.TypeWorld.Dome, this.ClickDomeButton);
            this.mSecondMenuGUI.SetListener(GameInfo.TypeWorld.Plains, this.ClickPlainsButton);
            this.mSecondMenuGUI.SetListener(GameInfo.TypeWorld.Desert, this.ClickDesertButton);
            this.mSecondMenuGUI.SetListener(GameInfo.TypeWorld.Hills, this.ClickHillsButton);
            this.mSecondMenuGUI.SetListener(GameInfo.TypeWorld.Mountain, this.ClickMountainButton);
            this.mSecondMenuGUI.SetListenerBack(this.ClickBackButton);
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
        }

        private void ClickNewGameButton(object obj, MouseButtonEventArgs arg) 
        {
            this.mStateMgr.GameInfo = new GameInfo();
            this.mStateMgr.RequestStatePush(typeof(LoadingState));
        }
    }
}

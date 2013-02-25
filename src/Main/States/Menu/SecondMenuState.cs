using System;
using Miyagi.Common.Events;

using Game.GUICreator;
using Game.World;

namespace Game.States
{
    public class SecondMenuState : State
    {
        private SecondMenuGUI mSecondMenuGUI;
        private GameInfo mGameInfo;

        public SecondMenuState(StateManager stateMgr) : base(stateMgr) { }

        protected override void Startup()
        {
            this.mGameInfo = new GameInfo();
            this.mGameInfo.Seed = 42;
            this.mGameInfo.Size = new Mogre.Vector2(-1, -1);

            this.mSecondMenuGUI = new SecondMenuGUI(this.mStateMgr, this.mGameInfo);
            this.mSecondMenuGUI.SetListener(GameInfo.TypeWorld.Dome, this.ClickDomeButton);
            this.mSecondMenuGUI.SetListener(GameInfo.TypeWorld.Plains, this.ClickPlainsButton);
            this.mSecondMenuGUI.SetListener(GameInfo.TypeWorld.Hills, this.ClickHillsButton);
            this.mSecondMenuGUI.SetListener(GameInfo.TypeWorld.Mountain, this.ClickMountainButton);
            this.mSecondMenuGUI.SetListenerBack(this.ClickBackButton);
        }

        public override void Hide()
        {
            this.mStateMgr.MiyagiMgr.CursorVisibility = false;
        }

        public override void Show()
        {
            this.mSecondMenuGUI.Show();
            this.mStateMgr.MiyagiMgr.CursorVisibility = true;
        }

        public override void Update(float frameTime)
        {
            if (this.mStateMgr.Input.IsKeyDown(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(); }
        }

        protected override void Shutdown()
        {
            this.mSecondMenuGUI.Dispose();
            
        }

        public void ClickButton()
        {
            //this.mStateMgr.RequestStatePop();
            this.mStateMgr.RequestStatePush(typeof(LoadingState));
        }
        private void ClickBackButton(object obj, MouseButtonEventArgs arg) { this.mStateMgr.RequestStatePop(); }
        private void ClickDomeButton(object obj, MouseButtonEventArgs arg)
        {
            this.mGameInfo.Type = GameInfo.TypeWorld.Dome;
            Mogre.Vector2 newSize = this.mGameInfo.Size;
            if (newSize.x <= 0 || newSize.x > 30) { newSize.x = 3; }
            if (newSize.y <= 0 || newSize.y > 30) { newSize.y = 3; }
            this.mGameInfo.Size = newSize;

            this.mStateMgr.GameInfo = this.mGameInfo;
            this.ClickButton();
        }
        private void ClickPlainsButton(object obj, MouseButtonEventArgs arg)
        {
            this.mGameInfo.Type = GameInfo.TypeWorld.Plains;
            Mogre.Vector2 newSize = this.mGameInfo.Size;
            if (newSize.x <= 0 || newSize.x > 30) { newSize.x = 13; }
            if (newSize.y <= 0 || newSize.y > 30) { newSize.y = 13; }
            this.mGameInfo.Size = newSize;

            this.mStateMgr.GameInfo = this.mGameInfo;
            this.ClickButton();
        }
        private void ClickHillsButton(object obj, MouseButtonEventArgs arg)
        {
            this.mGameInfo.Type = GameInfo.TypeWorld.Hills;
            Mogre.Vector2 newSize = this.mGameInfo.Size;
            if (newSize.x <= 0 || newSize.x > 30) { newSize.x = 15; }
            if (newSize.y != newSize.x)           { newSize.y = newSize.x; }
            this.mGameInfo.Size = newSize;

            this.mStateMgr.GameInfo = this.mGameInfo;
            this.ClickButton();
        }
        private void ClickMountainButton(object obj, MouseButtonEventArgs arg)
        {
            this.mGameInfo.Type = GameInfo.TypeWorld.Mountain;
            Mogre.Vector2 newSize = this.mGameInfo.Size;
            if (newSize.x <= 0 || newSize.x > 30) { newSize.x = 15; }
            if (newSize.y <= 0 || newSize.y > 30) { newSize.y = 15; }
            this.mGameInfo.Size = newSize;

            this.mStateMgr.GameInfo = this.mGameInfo;
            this.ClickButton();
        }
    }
}

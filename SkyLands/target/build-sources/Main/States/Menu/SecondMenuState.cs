using System;
using Miyagi.Common.Events;
using Mogre;

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
            this.mGameInfo.Size = new Vector2(5, 5);

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
            this.mStateMgr.RequestStatePush(typeof(LoadingState));
        }
        private void ClickBackButton(object obj, MouseButtonEventArgs arg) { this.mStateMgr.RequestStatePop(); }
        private void ClickDomeButton(object obj, MouseButtonEventArgs arg)
        {
            this.mGameInfo.Type = GameInfo.TypeWorld.Dome;
            Vector2 newSize = this.mGameInfo.Size;
            if (newSize.x <= 0 || newSize.x > 30) { newSize.x = 4; }
            if (newSize.y != newSize.x)           { newSize.y = newSize.x; }
            this.mGameInfo.Size = newSize;

            this.mStateMgr.GameInfo = this.mGameInfo;
            this.ClickButton();
        }

        private void ClickPlainsButton(object obj, MouseButtonEventArgs arg)
        {
            this.mGameInfo.Type = GameInfo.TypeWorld.Plains;
            Vector2 newSize = this.mGameInfo.Size;
            if (newSize.x <= 2 || newSize.x >= 60) { newSize.x = 11; }
            if (newSize.y <= 2 || newSize.y >= 60) { newSize.y = 11; }
            this.mGameInfo.Size = newSize;

            this.mStateMgr.GameInfo = this.mGameInfo;
            this.ClickButton();
        }

        private void ClickDesertButton(object obj, MouseButtonEventArgs arg)
        {
            this.mGameInfo.Type = GameInfo.TypeWorld.Desert;
            Vector2 newSize = this.mGameInfo.Size;
            if (newSize.x <= 2 || newSize.x >= 60) { newSize.x = 13; }
            if (newSize.y <= 2 || newSize.y >= 60) { newSize.y = 13; }
            this.mGameInfo.Size = newSize;

            this.mStateMgr.GameInfo = this.mGameInfo;
            this.ClickButton();
        }

        private void ClickHillsButton(object obj, MouseButtonEventArgs arg)
        {
            this.mGameInfo.Type = GameInfo.TypeWorld.Hills;
            Vector2 newSize = this.mGameInfo.Size;
            if (newSize.x <= 2 || newSize.x >= 30) { newSize.x = 15; }
            if (newSize.y <= 2 || newSize.y >= 30) { newSize.y = 15; }
            this.mGameInfo.Size = newSize;

            this.mStateMgr.GameInfo = this.mGameInfo;
            this.ClickButton();
        }

        private void ClickMountainButton(object obj, MouseButtonEventArgs arg)
        {
            this.mGameInfo.Type = GameInfo.TypeWorld.Mountain;
            Vector2 newSize = this.mGameInfo.Size;
            if (newSize.x <= 2 || newSize.x > 30) { newSize.x = 15; }
            if (newSize.y <= 2 || newSize.y > 30) { newSize.y = 15; }
            this.mGameInfo.Size = newSize;

            this.mStateMgr.GameInfo = this.mGameInfo;
            this.ClickButton();
        }
    }
}

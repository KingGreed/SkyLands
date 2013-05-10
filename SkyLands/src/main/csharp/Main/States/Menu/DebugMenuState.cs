using System;
using Miyagi.Common.Events;
using Mogre;

using Game.GUICreator;
using Game.World;

namespace Game.States
{
    public class DebugMenuState : State
    {
        private DebugMenuGUI mSecondMenuGUI;

        public DebugMenuState(StateManager stateMgr) : base(stateMgr, "SecondMenu") { }

        protected override void Startup()
        {
            this.mStateMgr.GameInfo = new GameInfo();
            //this.mStateMgr.GameInfo.Size = new Vector2(-1, -1);

            this.mSecondMenuGUI = new DebugMenuGUI(this.mStateMgr, this.mStateMgr.GameInfo);
            this.mSecondMenuGUI.SetListener(GameInfo.TypeWorld.StoryEditor, this.ClickStoryEditorButton);
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

        private void ClickStoryEditorButton(object obj, MouseButtonEventArgs arg)
        {
            this.mStateMgr.GameInfo.IsInEditorMode = true;
            this.ClickButton();
        }

        private void ClickPlainsButton(object obj, MouseButtonEventArgs arg)
        {
            this.mStateMgr.GameInfo.Type = GameInfo.TypeWorld.Plains;
            Vector2 newSize = this.mStateMgr.GameInfo.Size;
            if (newSize.x <= 2 || newSize.x >= 25) { newSize.x = 11; }
            if (newSize.y <= 2 || newSize.y >= 25) { newSize.y = 11; }
            this.mStateMgr.GameInfo.Size = newSize;

            this.ClickButton();
        }

        private void ClickDesertButton(object obj, MouseButtonEventArgs arg)
        {
            this.mStateMgr.GameInfo.Type = GameInfo.TypeWorld.Desert;
            Vector2 newSize = this.mStateMgr.GameInfo.Size;
            if (newSize.x <= 2 || newSize.x >= 25) { newSize.x = 13; }
            if (newSize.y <= 2 || newSize.y >= 25) { newSize.y = 13; }
            this.mStateMgr.GameInfo.Size = newSize;

            this.ClickButton();
        }

        private void ClickHillsButton(object obj, MouseButtonEventArgs arg)
        {
            this.mStateMgr.GameInfo.Type = GameInfo.TypeWorld.Hills;
            Vector2 newSize = this.mStateMgr.GameInfo.Size;
            if (newSize.x <= 2 || newSize.x >= 25) { newSize.x = 15; }
            if (newSize.y <= 2 || newSize.y >= 25) { newSize.y = 15; }
            this.mStateMgr.GameInfo.Size = newSize;
            
            this.ClickButton();
        }

        private void ClickMountainButton(object obj, MouseButtonEventArgs arg)
        {
            this.mStateMgr.GameInfo.Type = GameInfo.TypeWorld.Mountain;
            Vector2 newSize = this.mStateMgr.GameInfo.Size;
            if (newSize.x <= 2 || newSize.x > 25) { newSize.x = 15; }
            if (newSize.y <= 2 || newSize.y > 25) { newSize.y = 15; }
            this.mStateMgr.GameInfo.Size = newSize;
            
            this.ClickButton();
        }
    }
}

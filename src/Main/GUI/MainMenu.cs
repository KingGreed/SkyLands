using System;
using Miyagi.Common.Events;

using Game.World;
using Game.States;
using Game.GUICreator;

namespace Game
{
    public class MainMenu : State
    {
        private MenuGUI mMenuGUI;

        public MainMenu(StateManager stateMgr) : base(stateMgr) { }

        public override bool Startup()
        {
            if (this.mIsStartedUp) { return false; }

            this.mIsStartedUp = true;
            this.mStateMgr.MiyagiManager.CursorVisibility = true;
            this.mMenuGUI = new MenuGUI(this.mStateMgr.MiyagiManager, "Menu GUI");
            this.mMenuGUI.MouseClickPlayButton = this.MouseClickPlayButton;

            Mogre.LogManager.Singleton.DefaultLog.LogMessage("Menu Created");
            
            return true;
        }

        private void MouseClickPlayButton(object obj, MouseButtonEventArgs arg)
        {
            this.mStateMgr.MiyagiManager.CursorVisibility = false;
            this.mStateMgr.RequestStatePush(typeof(MainWorld));
        }

        public override void Hide() { this.mMenuGUI.Hide(); this.mStateMgr.MiyagiManager.CursorVisibility = false; }

        public override void Show() { this.mMenuGUI.Show(); this.mStateMgr.MiyagiManager.CursorVisibility = true; }

        public override void Update(float frameTime)
        {
            if (this.mStateMgr.Input.IsKeyDown(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(); }
        }

        public override void Shutdown() { mMenuGUI.Dispose(); }
    }
}

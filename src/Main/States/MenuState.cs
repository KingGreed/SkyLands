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

        protected override void Startup()
        {
            this.mMenuGUI = new MenuGUI(this.mStateMgr.MiyagiManager, "Menu GUI");
            this.mMenuGUI.SetListener(MenuGUI.Buttons.Play, this.ClickPlayButton);
            this.mMenuGUI.SetListener(MenuGUI.Buttons.Exit, this.ClickExitButton);

            Mogre.LogManager.Singleton.DefaultLog.LogMessage("Menu Created");
        }

        private void ClickPlayButton(object obj, MouseButtonEventArgs arg)
        {
            //this.mStateMgr.OverlayVisibility = false;
            //this.mStateMgr.MiyagiManager.CursorVisibility = false;
            this.Hide();
            this.mStateMgr.RequestStatePush(typeof(GameState));
        }

        private void ClickExitButton(object obj, MouseButtonEventArgs arg) { this.mStateMgr.RequestStatePop(); }

        public override void Hide()
        { 
            this.mMenuGUI.Hide();
            this.mStateMgr.MiyagiManager.CursorVisibility = false;
            this.mStateMgr.OverlayVisibility = true;
        }

        public override void Show()
        { 
            this.mMenuGUI.Show();
            this.mStateMgr.MiyagiManager.CursorVisibility = true;
            this.mStateMgr.OverlayVisibility = false;
        }

        public override void Update(float frameTime)
        {
            if (this.mStateMgr.Input.IsKeyDown(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(); }
        }

        protected override void Shutdown() { mMenuGUI.Dispose(); }
    }
}

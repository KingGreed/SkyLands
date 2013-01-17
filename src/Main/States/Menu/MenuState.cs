using System;
using Miyagi.Common.Events;

using Game.GUICreator;

namespace Game.States
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

        private void ClickPlayButton(object obj, MouseButtonEventArgs arg) { this.mStateMgr.RequestStatePush(typeof(SecondMenuState)); }
        private void ClickExitButton(object obj, MouseButtonEventArgs arg) { this.mStateMgr.RequestStatePop(); }

        public override void Update(float frameTime)
        {
            if (this.mStateMgr.Input.IsKeyDown(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(); }
        }

        protected override void Shutdown() { mMenuGUI.Dispose(); }

        public override void Hide()
        { 
            //this.mMenuGUI.Hide();
            this.mMenuGUI.EnableButtons(false);
        }

        public override void Show()
        { 
            this.mMenuGUI.Show();
            this.mMenuGUI.EnableButtons(true);
            this.mStateMgr.MiyagiManager.CursorVisibility = true;
        }
    }
}

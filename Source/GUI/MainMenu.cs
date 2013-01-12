using System;

using Game.States;
using Game.GUICreator;

namespace Game
{
    public class MainMenu : State
    {
        MenuGUI mGUI;

        public MainMenu(StateManager stateMgr) : base(stateMgr)
        {
            this.mGUI = new MenuGUI(stateMgr.MiyagiManager, "Menu GUI");
            Mogre.LogManager.Singleton.DefaultLog.LogMessage("Menu Created");
        }

        public override bool Startup()
        {
            if (this.mIsStartedUp) { return false; }

            this.mIsStartedUp = true;
            
            
            return true;
        }

        public override void Hide() { this.mGUI.Hide(); }

        public override void Show() { this.mGUI.Show(); }

        public override void Update(float frameTime)
        {
            if (this.mStateMgr.Input.IsKeyDown(MOIS.KeyCode.KC_E)) { this.mStateMgr.RequestStatePush(typeof(World)); }
            if (this.mStateMgr.Input.IsKeyDown(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(); }
        }

        public override void Shutdown() { mGUI.Dispose(); }
    }
}

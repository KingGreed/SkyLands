using System;
using System.Collections.Generic;

using Game.States;
using Game.GUICreator;

using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.UI;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Layout;

namespace Game
{
    public class MainMenu : State
    {
        MenuGUI mMenuGUI;

        public MainMenu(StateManager stateMgr) : base(stateMgr)
        {
            this.mMenuGUI = new MenuGUI(stateMgr, "Menu GUI");

        }

        public override bool Startup()
        {
            if (this.mIsStartedUp) { return false; }

            this.mIsStartedUp = true;
            
            
            return true;
        }

        public override void Hide() { this.mMenuGUI.Hide(); }

        public override void Show() { this.mMenuGUI.Show(); }

        public override void Update(float frameTime)
        {
            if (this.mStateMgr.Input.IsKeyDown(MOIS.KeyCode.KC_E)) { this.mStateMgr.RequestStatePush(typeof(World)); }
            if (this.mStateMgr.Input.IsKeyDown(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(); }
        }

        public override void Shutdown() { mMenuGUI.Shutdown(); }
    }
}

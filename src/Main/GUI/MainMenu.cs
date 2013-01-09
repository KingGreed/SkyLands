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

        private void CreateMenu()
        {
            /* A red label in the center of the screen */            
            Label label = new Label();
            label.Size = new Size(150, 50);
            label.Location = new Point((int) this.mStateMgr.Window.Width / 2 - label.Size.Width / 2,
                                       (int) this.mStateMgr.Window.Height / 2 - label.Size.Height / 2);
            label.TextStyle.ForegroundColour = new ColourDefinition(new Colour(255, 255, 0, 0));
            label.Text = "Press E to play";
            this.mMenuGUI.Controls.Add(label);

            Mogre.LogManager.Singleton.DefaultLog.LogMessage("Menu Created");
        }

        public override void Hide() { this.mMenuGUI.Visible = false; }

        public override void Show() { this.mMenuGUI.Visible = true; }

        public override void Update(float frameTime)
        {
            if (this.mStateMgr.Input.IsKeyDown(MOIS.KeyCode.KC_E)) { this.mStateMgr.RequestStatePush(typeof(World)); }
            if (this.mStateMgr.Input.IsKeyDown(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(); }
        }

        public override void Shutdown()
        {
            mMenuGUI.Dispose();
        }
    }
}

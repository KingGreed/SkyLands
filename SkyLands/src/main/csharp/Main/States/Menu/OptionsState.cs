using System;
using Miyagi.Common.Events;
using Miyagi.UI.Controls;

using Game.GUICreator;

namespace Game.States
{
    public class OptionsState : State
    {
        private OptionsGUI mOptionsGUI;
        private bool mIsFullScreen;
        public OptionsState(StateManager stateMgr) : base(stateMgr) { }

        protected override void Startup()
        {
            this.mOptionsGUI = new OptionsGUI(this.mStateMgr, "Options GUI");
            this.mOptionsGUI.SetListener(OptionsGUI.ButtonName.FullScreen, this.ClickFullScreenButton);
            this.mOptionsGUI.SetListener(OptionsGUI.ButtonName.HighQuality, this.ClickQualityButton);
            this.mOptionsGUI.SetListener(OptionsGUI.ButtonName.Music, this.ClickMusicButton);
            this.mOptionsGUI.SetListener(OptionsGUI.ButtonName.VSync, this.ClickVSyncButton);
            this.mOptionsGUI.SetListenerBack(this.ClickBackButton);
            this.mIsFullScreen = this.mStateMgr.Window.IsFullScreen;
        }

        public override void Hide()
        {
            this.mStateMgr.MiyagiMgr.CursorVisibility = false;
        }

        public override void Show()
        {
            this.mOptionsGUI.Show();
            this.mStateMgr.MiyagiMgr.CursorVisibility = true;
        }

        public override void Update(float frameTime)
        {
            if (this.mStateMgr.Input.IsKeyDown(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(); }
        }

        protected override void Shutdown() { this.mOptionsGUI.Dispose(); }

        private void SwitchText(Button b) 
        {
            if (b.Text == "ON") {b.Text = "OFF";}
            else{b.Text = "ON";}
        }

        private void ClickBackButton(object obj, MouseButtonEventArgs arg) { this.mStateMgr.RequestStatePop();}
        private void ClickMusicButton(object obj, MouseButtonEventArgs arg) 
        {
            this.SwitchText(this.mOptionsGUI.Buttons[OptionsGUI.ButtonName.Music]);
        }
        private void ClickQualityButton(object obj, MouseButtonEventArgs arg) 
        {
            this.SwitchText(this.mOptionsGUI.Buttons[OptionsGUI.ButtonName.HighQuality]);
        }
        private void ClickVSyncButton(object obj, MouseButtonEventArgs arg) 
        {
            this.SwitchText(this.mOptionsGUI.Buttons[OptionsGUI.ButtonName.VSync]);
        }
        private void ClickFullScreenButton(object obj, MouseButtonEventArgs arg) 
        {
            this.mIsFullScreen = !this.mIsFullScreen;
            this.mStateMgr.Window.SetFullscreen(this.mIsFullScreen, this.mStateMgr.Window.Width, this.mStateMgr.Window.Height);
            this.SwitchText(this.mOptionsGUI.Buttons[OptionsGUI.ButtonName.FullScreen]);
        }
        //private void ClickScreenResolutionButton(object obj, MouseButtonEventArgs arg) { this.mStateMgr.Window.Resize(,); }
        }
}

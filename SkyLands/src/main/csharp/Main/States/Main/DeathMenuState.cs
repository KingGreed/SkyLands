using Game.GUIs;

namespace Game.States
{
    public class DeathMenuState : State
    {
        public DeathMenuState(StateManager stateMgr) : base(stateMgr, "DeathMenu") { }

        public override void Show()
        {
            new DeathMenu(this.mStateMgr);
            this.mStateMgr.Controller.CursorVisibility = true;
        }

        public override void Update(float frameTime)
        {
            /*if (this.mStateMgr.Controller.HasActionOccured(UserAction.Start))
            {
                if (OgreForm.webView.Source.AbsolutePath.Contains("MainMenu"))
                    this.mStateMgr.RequestStatePop();
                else
                {
                    GUI.Visible = false;
                    new MainMenu(this.mStateMgr);
                }
            }*/
        }
    }
}

using Game.CharacSystem;
using Game.GUICreator;

namespace Game.States
{
    public class StoryEditorState : MainState
    {
        public StoryEditorState(StateManager stateMgr) : base(stateMgr, "StoryEditor") { }

        protected override void Startup()
        {
            base.Startup();
            this.User.SwitchFreeCamMode();
        }

        protected override void AfterWorldCreation()
        {
            this.MainGUI = new StoryEditorGUI(this.mStateMgr);
            this.CharacMgr = new CharacMgr(this.mStateMgr, this.mWorld, this.User);
        }

        public override void Show()
        {
            base.Show();
            this.MainGUI.Show();
        }

        public override void Save()
        {
            this.mStateMgr.WriteOnConsole("TO DO : Save story editor");
        }
    }
}

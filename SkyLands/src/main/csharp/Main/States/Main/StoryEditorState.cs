using Game.CharacSystem;

namespace Game.States
{
    public class StoryEditorState : MainState
    {
        public StoryEditorState(StateManager stateMgr) : base(stateMgr, "StoryEditor") { }

        protected override void Startup()
        {
            base.Startup();
        }

        protected override void AfterWorldCreation()
        {
            this.CharacMgr = new CharacMgr(this.mStateMgr, this.World, this.User);
        }

        public override void Show()
        {
        }

        public override void Save()
        {
        }
    }
}

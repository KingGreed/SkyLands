using Game.CharacSystem;
using Game.World;

namespace Game.States
{
    public class StoryEditorState : MainState {
        public StoryEditorState(StateManager stateMgr) : base(stateMgr, "StoryEditor") { }

        protected override void Startup() {
            this.World = new StoryEditorWorld(this.mStateMgr);
            base.Startup();
            
        }

        protected override void AfterWorldCreation() {
            this.CharacMgr = new CharacMgr(this.mStateMgr, this.World, this.User);
            this.User.SwitchFreeCamMode();
        }

        public override void Show()
        {
        }

        public override void Save()
        {
        }
    }
}

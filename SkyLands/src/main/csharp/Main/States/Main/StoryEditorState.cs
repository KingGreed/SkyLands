using Game.BaseApp;
using Game.CharacSystem;
using Game.GUIs;
using Game.Input;
using Game.World;

namespace Game.States
{
    public class StoryEditorState : MainState {
        public StoryEditorState(StateManager stateMgr) : base(stateMgr, "StoryEditor") {
            this.mStateMgr.GameInfo.IsInEditorMode = true;
        }

        protected override void Startup() {
            this.World = new StoryEditorWorld(this.mStateMgr);
            base.Startup();
        }

        protected override void AfterWorldCreation() {
            this.CharacMgr = new CharacMgr(this.mStateMgr, this.World, this.User);
        }

        public override void Show() {
            base.Show();
            GUI.Visible = true;
            User.SwitchGUIVisibility(true);
        }

        public override void Save() {
        }

        public override void OpenMainGUI()
        {
            new StructuresMenu(this.mStateMgr, this.mStateMgr.StoryInfo.pathToFile);
        }

        public override void Update(float frameTime) {
            if(this.mStateMgr.Controller.HasActionOccured(UserAction.Start)) {
                this.mStateMgr.RequestStatePop();
                return;
            }
            base.Update(frameTime);
        }
    }
}

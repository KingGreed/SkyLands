namespace Game.States
{
    public abstract class State
    {
        protected StateManager mStateMgr;
        protected bool mIsStartedUp;
        protected string mName;

        public string Name { get { return this.mName; } }

        protected State(StateManager stateMgr, string name)
        {
            this.mStateMgr = stateMgr;
            this.mIsStartedUp = false;
            this.mName = name;
        }

        public bool StartupState()
        {
            if (this.mIsStartedUp) { return false; }
            this.mIsStartedUp = true;
            this.Startup();
            //this.mStateMgr.MyConsole.GUI.ForceRedraw();
            return true;
        }

        protected abstract void Startup();

        public virtual void Hide() {}

        public virtual void Show() {}

        public abstract void Update(float frameTime);

        public void ShutdownState()
        {
            if (this.mStateMgr == null) { return; }
            this.Shutdown();
            this.mIsStartedUp = false;
        }

        protected abstract void Shutdown();
    }
}

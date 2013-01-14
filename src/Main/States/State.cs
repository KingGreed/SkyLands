using System;


namespace Game.States
{
    public abstract class State
    {
        protected StateManager mStateMgr;
        protected bool mIsStartedUp;
        
        public State(StateManager stateMgr)
        {
            this.mStateMgr = stateMgr;
            this.mIsStartedUp = false;
        }

        public bool StartupState()
        {
            if (this.mIsStartedUp) { return false; }
            this.mIsStartedUp = true;
            this.Startup();
            return true;
        }

        protected abstract void Startup();

        public abstract void Hide();

        public abstract void Show();

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

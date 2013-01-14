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

        public bool Startup()
        {
            if (this.mIsStartedUp) { return false; }
            this.mIsStartedUp = true;
            this.StartUp();
            return true;
        }

        protected abstract void StartUp();

        public abstract void Hide();

        public abstract void Show();

        public abstract void Update(float frameTime);

        public abstract void Shutdown();
    }
}

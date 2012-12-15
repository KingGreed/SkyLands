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

        /* A State is started up in the StateManager */
        public abstract bool Startup();

        public abstract void Hide();

        public abstract void Show();

        public abstract void Update(float frameTime);

        public abstract void Shutdown();
    }
}

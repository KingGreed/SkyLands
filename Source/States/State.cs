using System;


namespace Game.States
{
    public abstract class State
    {
        protected StateManager mStateMgr;
        protected bool mIsStartedUp;
        
        public State()
        {
            this.mStateMgr = null;
            this.mIsStartedUp = false;
        }

        /* A State is started up in the StateManager */
        public abstract bool Startup(StateManager stateMgr);

        public abstract void Shutdown();

        public abstract void Update(float frameTime);
    }
}

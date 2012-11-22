using System;

namespace Game.States
{
    public abstract class State
    {
        protected StateManager mStateMgr;
        protected bool isStartedUp;
        
        public State()
        {
            mStateMgr = null;
            isStartedUp = false;
        }

        /* A State is Startup in the StateManager */
        public abstract bool Startup(StateManager stateMgr);

        public abstract void Shutdown();

        public abstract void Update(float frameTime);
    }
}

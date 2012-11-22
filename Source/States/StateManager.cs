using System;
using System.Reflection;
using System.Collections.Generic;

namespace Game.States
{
    public class StateManager
    {
        private OgreManager mEngine;
        private MoisManager mInput;

        private Stack<State> mStateStack;
        private bool mShutdown;

        public OgreManager Engine
        {
            get { return mEngine; }
        }

        public MoisManager Input
        {
            get { return mInput; }
        }

        public bool ShuttingDown
        {
            get { return mShutdown; }
        }

        public StateManager(OgreManager engine)
        {
            mEngine = engine;
            mInput = new MoisManager();
            mStateStack = new Stack<State>();
            mShutdown = false;
        }

        public bool Startup(State firstState)
        {
            mShutdown = false;

            if (!mInput.Startup(mEngine.WindowHandle, (int)mEngine.Window.Width, (int)mEngine.Window.Height))
                return false;

            if (mStateStack.Count != 0)
                return false;

            if (!PushState(firstState))
                return false;

            return true;
        }

        public void Shutdown()
        {
            mInput.Shutdown();

            foreach (State state in mStateStack)
                state.Shutdown();
        }

        public void Update(long frameTime)
        {
            mInput.Update();

            if (mStateStack.Count != 0)
                mStateStack.Peek().Update(frameTime);
        }

        /* Add a State to the stack and start it up */
        public bool PushState(State newState)
        {
            if (newState == null)
                return false;
            else
            {
                if (!newState.Startup(this))
                    return false;
                
                mStateStack.Push(newState);
                return true;
            }
        }

        public bool PopState()
        {
            if (mStateStack.Count <= 1)  // Can't pop the first State
                return false;
            
            mStateStack.Pop();
            return true;
        }

        public void RequestShutdown()
        {
            mShutdown = true;
        }
    }
}

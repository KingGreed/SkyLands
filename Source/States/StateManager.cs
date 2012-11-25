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
        private Type mNewState;
        private bool mShutdown;
        private bool mIsPopRequested;

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
            mIsPopRequested = false;
        }

        public bool Startup(Type firstState)
        {
            mShutdown = false;
            mIsPopRequested = false;

            if (!mInput.Startup(mEngine.WindowHandle, (int)mEngine.Window.Width, (int)mEngine.Window.Height))
                return false;

            if (mStateStack.Count != 0)
                return false;

            if (!RequestStatePush(firstState))
                return false;

            return true;
        }

        public void Shutdown()
        {
            mInput.Shutdown();

            while (mStateStack.Count > 0)
                PopState();
        }

        public void Update(float frameTime)
        {
            if (mShutdown)
            {
                Shutdown();
                return;
            }
            
            mInput.Update();

            if (mIsPopRequested)
                PopState();

            if (mNewState != null)  // A pushState was requested
            {
                State newState = null;
                
                // Use reflection to get new state class default constructor
                ConstructorInfo constructor = mNewState.GetConstructor(Type.EmptyTypes);

                // Try to create an object from the requested state class
                if (constructor != null)
                    newState = (State)constructor.Invoke(null);

                if (newState != null)
                    PushState(newState);

                mNewState = null;
                mInput.Clear();
            }

            if (mStateStack.Count > 0)
                mStateStack.Peek().Update(frameTime);
        }

        /* Add a State to the stack and start it up */
        private bool PushState(State newState)
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

        private void PopState()
        {
            if (mStateStack.Count > 0)
            {
                mStateStack.Peek().Shutdown();
                mStateStack.Pop();
            }

            mIsPopRequested = false;
        }

        public bool RequestStatePop()
        {
            if (mStateStack.Count <= 1)  // User can't pop the first State
                return false;
            
            mIsPopRequested = true; // Will pop the state in Update()
            return true;
        }

        public bool RequestStatePush(Type newState)
        {
            // new state class must be derived from base class "State"
            if (newState == null || !newState.IsSubclassOf(typeof(State)))
                return false;

            mNewState = newState;   // Will push the state in Update()
            return true;
        }

        public void RequestShutdown()
        {
            mShutdown = true;
        }
    }
}

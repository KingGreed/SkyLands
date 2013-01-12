using System;
using System.Reflection;
using System.Collections.Generic;
using Mogre;

using Game.BaseApp;
using Game.GUICreator;

namespace Game.States
{
    public abstract class StateManager : BaseApplication
    {
        private Stack<State> mStateStack;
        private Type mNewState;
        private bool mIsPopRequested;

        public Root Root                   { get { return this.mRoot; } }
        public SceneManager SceneManager   { get { return this.mSceneMgr; } }
        public RenderWindow Window         { get { return this.mWindow; } }
        public MoisManager Input           { get { return this.mInput; } }
        public MiyagiManager MiyagiManager { get { return this.mMiyagiMgr; } }
        public Camera Camera               { get { return this.mCam; } }
        public Viewport Viewport           { get { return this.mViewport; } }

        protected override void Create()
        {
            LogManager.Singleton.DefaultLog.LogMessage("***********************Program\'s Log***********************");
            this.mStateStack = new Stack<State>();
            this.mNewState = null;
            this.mIsPopRequested = false;
            this.RequestStatePush(typeof(MainMenu));
        }

        protected override void Update(FrameEvent evt)
        {
            float floatTime = ((FrameEvent)evt).timeSinceLastFrame;            
            
            if (this.mIsShutDownRequested)
            {
                this.Shutdown();
                LogManager.Singleton.DefaultLog.LogMessage("***********************End of Program\'s Log***********************");
                return;
            }
            
            if (this.mNewState != null)  // A pushState was requested
            {
                State newState = null;
                
                // Use reflection to get new state class default constructor
                ConstructorInfo constructor = this.mNewState.GetConstructor(new Type[1] {typeof(StateManager)});

                // Try to create an object from the requested state class
                if (constructor != null)
                    newState = (State)constructor.Invoke(new StateManager[1] {this});

                if (newState != null)
                    this.PushState(newState);

                this.mNewState = null;
            }

            if (this.mStateStack.Count > 0)
                this.mStateStack.Peek().Update(floatTime);

            if (this.mIsPopRequested)
                this.PopState();
        }

        /* Add a State to the stack and start it up */
        private bool PushState(State newState)
        {
            if (newState == null)
                return false;
            else
            {
                LogManager.Singleton.DefaultLog.LogMessage("Try to start up state : " + newState.ToString());
                if (!newState.Startup())
                {
                    LogManager.Singleton.DefaultLog.LogMessage("ERROR : Failed to start up state : " + newState.ToString());
                    return false;
                }

                if (this.mStateStack.Count > 0) { this.mStateStack.Peek().Hide(); }
                this.mStateStack.Push(newState);

                this.mInput.Clear();

                return true;
            }
        }

        private void PopState()
        {
            if (this.mStateStack.Count > 0)
            {
                string stateName = this.mStateStack.Peek().ToString();
                this.mStateStack.Peek().Shutdown();
                this.mStateStack.Pop();
                this.mInput.Clear();
                if (this.mStateStack.Count > 0) { this.mStateStack.Peek().Show(); }
                LogManager.Singleton.DefaultLog.LogMessage("Popped state : " + stateName);
            }

            this.mIsPopRequested = false;
        }

        public void RequestStatePop()
        {
            if (this.mStateStack.Count > 1) { this.mIsPopRequested = true; } // Will pop the state in Update()
            else                            { this.mIsShutDownRequested = true; }   // Will ShutDown in Update()
        }

        public bool RequestStatePush(Type newState)
        {
            // new state class must be derived from base class "State"
            if (newState == null || !newState.IsSubclassOf(typeof(State))) { return false; }

            this.mNewState = newState;   // Will push the state in Update()
            return true;
        }

        protected override void Shutdown()
        {
            while (this.mStateStack.Count > 0) { this.PopState(); }
            base.Shutdown();
        }
    }
}

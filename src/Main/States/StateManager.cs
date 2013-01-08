using System;
using System.Reflection;
using System.Collections.Generic;
using Mogre;

using Game.BaseApp;
using Game.GUISystem;

namespace Game.States
{
    public class StateManager
    {
        private Root mRoot;
        private SceneManager mSceneMgr;
        private RenderWindow mWindow;
        private MoisManager mInput;
        private MiyagiManager mMiyagiMgr;
        private Camera mCam;
        private Viewport mViewport;
        private Stack<State> mStateStack;
        private Type mNewState;
        private bool mIsPopRequested;
        private bool mIsShuttedDown;

        public Root Root                   { get { return this.mRoot; } }
        public SceneManager SceneManager   { get { return this.mSceneMgr; } }
        public RenderWindow Window         { get { return this.mWindow; } }
        public MoisManager Input           { get { return this.mInput; } }
        public MiyagiManager MiyagiManager { get { return this.mMiyagiMgr; } }
        public Camera Camera               { get { return this.mCam; } }
        public Viewport Viewport           { get { return this.mViewport; } }
        public bool IsShuttedDown          { get { return this.mIsShuttedDown; } }

        public StateManager(Root root, SceneManager sceneMgr, MoisManager input, RenderWindow window)
        {
            this.mRoot = root;
            this.mSceneMgr = sceneMgr;
            this.mWindow = window;
            this.mInput = input;
            this.mMiyagiMgr = new MiyagiManager();
            this.mCam = null;
            this.mStateStack = new Stack<State>();
            this.mIsPopRequested = false;
            this.mIsShuttedDown = false;
        }

        public bool Startup(Type firstState)
        {
            this.mIsPopRequested = false;
            this.mIsShuttedDown = false;

            this.CreateCamera(); this.CreateViewports();

            this.mMiyagiMgr.Startup(this.mInput);

            if (this.mStateStack.Count != 0)
                return false;

            if (!RequestStatePush(firstState))
                return false;

            return true;
        }

        private void CreateCamera()
        {
            this.mCam = this.mSceneMgr.CreateCamera("MainCamera");
            this.mCam.NearClipDistance = 5;
            this.mCam.FarClipDistance = 3000;
        }

        private void CreateViewports()
        {
            this.mViewport = this.mWindow.AddViewport(this.mCam);
            this.mViewport.BackgroundColour = ColourValue.Black;

            // Alter the camera aspect ratio to match the viewport
            this.mCam.AspectRatio = (this.mViewport.ActualWidth / this.mViewport.ActualHeight);
        }

        public void Update(float frameTime)
        {
            if (this.mIsShuttedDown)
                return;
            
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

            this.mMiyagiMgr.Update();

            if (this.mStateStack.Count > 0)
                this.mStateStack.Peek().Update(frameTime);

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
            else                            { this.Shutdown(); }
        }

        public bool RequestStatePush(Type newState)
        {
            // new state class must be derived from base class "State"
            if (newState == null || !newState.IsSubclassOf(typeof(State)))
                return false;

            this.mNewState = newState;   // Will push the state in Update()
            return true;
        }

        public void Shutdown()
        {
            while (this.mStateStack.Count > 0)
                PopState();

            mMiyagiMgr.ShutDown();
            this.mIsShuttedDown = true;
        }
    }
}

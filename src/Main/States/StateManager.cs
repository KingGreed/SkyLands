using System;
using System.Reflection;
using System.Collections.Generic;
using Mogre;

using Game.BaseApp;
using Game.GUICreator;
using Game.Display;

namespace Game.States
{
    public abstract class StateManager : BaseApplication
    {
        public enum TypeWorld { Sinus, Dome, Plain, Mountain }

        private MiyagiMgr    mMiyagiMgr;        
        private Stack<State> mStateStack;
        private Stack<Type>  mNewStates;
        private int          mPopRequested;
        private TypeWorld    mChosenWorld;
        private bool         mWaitOneFrame;

        public Root         Root          { get { return this.mRoot; } }
        public SceneManager SceneManager  { get { return this.mSceneMgr; } }
        public RenderWindow Window        { get { return this.mWindow; } }
        public MoisManager  Input         { get { return this.mInput; } }
        public MiyagiMgr    MiyagiManager { get { return this.mMiyagiMgr; } }
        public Camera       Camera        { get { return this.mCam; } }
        public Viewport     Viewport      { get { return this.mViewport; } }
        public TypeWorld    ChosenWorld   { get { return this.mChosenWorld; } set { this.mChosenWorld = value; } }
        public int          NumberState   { get { return this.mStateStack.Count; } }

        protected override void Create()
        {
            GraphicBlock.generateFace();
            LogManager.Singleton.DefaultLog.LogMessage("***********************Program\'s Log***********************");
            this.mMiyagiMgr = new MiyagiMgr(this.mInput, new Vector2(this.mWindow.Width, this.mWindow.Height));
            this.mStateStack = new Stack<State>();
            this.mNewStates = new Stack<Type>();
            this.mPopRequested = 0;
            this.mChosenWorld = TypeWorld.Plain;
            this.mWaitOneFrame = false;
            this.RequestStatePush(typeof(MainMenu));
        }

        protected override void Update(FrameEvent evt)
        {
            if (this.mIsShutDownRequested) { return; }

            float floatTime = ((FrameEvent)evt).timeSinceLastFrame;            

            this.mMiyagiMgr.Update();

            while(this.mPopRequested > 0) { this.PopState(); }

            for (int i = 0; i < this.mNewStates.Count; i++)
            {
                State newState = null;

                // Use reflection to get new state class default constructor
                ConstructorInfo constructor = this.mNewStates.Pop().GetConstructor(new Type[] { typeof(StateManager) });

                // Try to create an object from the requested state class
                if (constructor != null) { newState = (State)constructor.Invoke(new StateManager[] { this }); }
                if (newState != null)    { this.PushState(newState); }
            }

            if (this.mInput.WasKeyPressed(MOIS.KeyCode.KC_F2)) { this.OverlayVisibility = !this.OverlayVisibility; }

            if      (this.mWaitOneFrame)         { this.mWaitOneFrame = false; }
            else if (this.mStateStack.Count > 0) { this.mStateStack.Peek().Update(floatTime); }
        }

        /* Add a State to the stack and start it up */
        private bool PushState(State newState)
        {
            if (newState == null)
                return false;
            else
            {
                LogManager.Singleton.DefaultLog.LogMessage("--- Try to start up state : " + newState.ToString());
                if (!newState.StartupState())
                {
                    LogManager.Singleton.DefaultLog.LogMessage("ERROR : Failed to start up state : " + newState.ToString());
                    return false;
                }

                if (this.mStateStack.Count > 0) { this.mStateStack.Peek().Hide(); }
                    this.mStateStack.Push(newState);
                this.mStateStack.Peek().Show();

                this.mInput.Clear();

                return true;
            }
        }

        private void PopState()
        {
            if (this.mStateStack.Count > 0)
            {
                string stateName = this.mStateStack.Peek().ToString();
                this.mStateStack.Peek().ShutdownState();
                this.mStateStack.Pop();
                this.mInput.Clear();
                if (this.mStateStack.Count > 0) { this.mStateStack.Peek().Show(); }
                LogManager.Singleton.DefaultLog.LogMessage("--- Popped state : " + stateName);
            }

            this.mPopRequested--;
        }

        public void RequestStatePop(int pop = 1)
        {
            if (this.mStateStack.Count > 1) { this.mPopRequested = pop; } // Will pop the state in Update()
            else                            { this.mIsShutDownRequested = true; }   // Will ShutDown in Update()
        }

        public void RequestStatePush(params Type[] newStates)
        {
            foreach (Type newState in newStates)
                if (newState != null && newState.IsSubclassOf(typeof(State))) { this.mNewStates.Push(newState); }
            this.mWaitOneFrame = true;
        }

        protected override void Shutdown()
        {
            LogManager.Singleton.DefaultLog.LogMessage("***********************End of Program\'s Log***********************");
            while (this.mStateStack.Count > 0) { this.PopState(); }
            this.mMiyagiMgr.ShutDown();
            this.mInput.Shutdown(); 
            this.mRoot.Dispose();
        }
    }
}

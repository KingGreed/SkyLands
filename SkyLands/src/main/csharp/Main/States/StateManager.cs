using System;
using System.Reflection;
using System.Collections.Generic;
using Mogre;

using Game.BaseApp;
using Game.Display;
using Game.World;

namespace Game.States
{
    public abstract class StateManager : OgreForm
    {
        private Stack<State> mStateStack;
        private Stack<Type>  mNewStates;
        private int          mPopRequested;
        private bool         mWaitOneFrame;

        public Root         Root        { get { return this.mRoot; } }
        public SceneManager SceneMgr    { get { return this.mSceneMgr; } }
        public RenderWindow Window      { get { return this.mWindow; } }
        public Controller   Controller  { get { return this.mController; } }
        public Camera       Camera      { get { return this.mCam; } }
        public Viewport     Viewport    { get { return this.mViewport; } }
        public int          NumberState { get { return this.mStateStack.Count; } }
        public GameInfo     GameInfo    { get; set; }
        public MainState    MainState   { get; private set; }

        protected override void Create() {


            GraphicBlock.generateFace();
            LogManager.Singleton.DefaultLog.LogMessage("***********************Program\'s Log***********************");
            //LogManager.Singleton.DefaultLog.LogDetail = LoggingLevel.LL_LOW;
            this.SceneMgr.ShadowTechnique = ShadowTechnique.SHADOWDETAILTYPE_INTEGRATED;
            this.SceneMgr.ShadowFarDistance = 400;
            
            this.mStateStack = new Stack<State>();
            this.mNewStates = new Stack<Type>();
            this.mPopRequested = 0;
            this.mWaitOneFrame = false;
            this.GameInfo = new GameInfo(false);
            this.RequestStatePush(typeof(GameState));
        }

        protected override void Update(FrameEvent evt)
        {
            if (this.mIsShutDownRequested) { return; }

            float frameTime = evt.timeSinceLastFrame;
            if (frameTime > 0.1) { return; }

            while(this.mPopRequested > 0) { this.PopState(); }

            for (int i = 0; i < this.mNewStates.Count; i++)
            {
                State newState = null;

                // Use reflection to get new state class default constructor
                ConstructorInfo constructor = this.mNewStates.Pop().GetConstructor(new Type[] { typeof(StateManager) });

                // Try to create an object from the requested state class
                if (constructor != null) { newState = (State)constructor.Invoke(new StateManager[] {this}); }
                if (newState != null)    { this.PushState(newState); }
            }

            //if (this.mInput.WasKeyPressed(MOIS.KeyCode.KC_F2)) { this.OverlayVisibility = !this.OverlayVisibility; }

            if (this.mWaitOneFrame) { this.mWaitOneFrame = false; }
            else if (this.mStateStack.Count > 0) { this.mStateStack.Peek().Update(frameTime); }
        }

        /* Add a State to the stack and start it up */
        private void PushState(State newState)
        {
            if (newState == null) {return;}

            LogManager.Singleton.DefaultLog.LogMessage("--- Try to start up state : " + newState);

            if (newState.GetType().IsSubclassOf(typeof(MainState)))
                this.MainState = (MainState)newState;

            if (!newState.StartupState())
            {
                LogManager.Singleton.DefaultLog.LogMessage("ERROR : Failed to start up state : " + newState);
                return;
            }

            if (this.mStateStack.Count > 0) { this.mStateStack.Peek().Hide(); }

            this.mStateStack.Push(newState);
            this.mStateStack.Peek().Show();

            this.mController.Clear();
        }

        private void PopState()
        {
            if (this.mStateStack.Count > 0)
            {
                string stateName = this.mStateStack.Peek().Name;
                this.mStateStack.Peek().ShutdownState();
                this.mStateStack.Pop();
                this.mController.Clear();
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

        public void PopToMenu() { this.RequestStatePop(this.NumberState - 1);}

        public void RequestStatePush(params Type[] newStates)
        {
            foreach (Type newState in newStates)
                if (newState != null && newState.IsSubclassOf(typeof(State))) { this.mNewStates.Push(newState); }
            this.mWaitOneFrame = true;
        }

        protected override void Shutdown()
        {
            LogManager.Singleton.DefaultLog.LogMessage("***********************End of Program\'s Log***********************");
            base.Shutdown();
            while (this.mStateStack.Count > 0) { this.PopState(); }
        }
    }
}

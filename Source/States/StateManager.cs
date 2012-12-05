﻿using System;
using System.Reflection;
using System.Collections.Generic;
using Mogre;

using Game.BaseApp;


namespace Game.States
{
    public class StateManager
    {
        private SceneManager mSceneMgr;
        private MoisManager mInput;
        private RenderWindow mWindow;
        private Stack<State> mStateStack;
        private Type mNewState;
        private bool mIsPopRequested;

        public SceneManager SceneManager
        {
            get { return mSceneMgr; }
        }

        public MoisManager Input
        {
            get { return mInput; }
        }

        public RenderWindow Window
        {
            get { return mWindow; }
        }

        public StateManager(SceneManager sceneMgr, MoisManager input, RenderWindow window)
        {
            mSceneMgr = sceneMgr;
            mInput = input;
            mWindow = window;
            mStateStack = new Stack<State>();
            mIsPopRequested = false;
        }

        public bool Startup(Type firstState)
        {
            mIsPopRequested = false;

            if (mStateStack.Count != 0)
                return false;

            if (!RequestStatePush(firstState))
                return false;

            return true;
        }

        public void Update(float frameTime)
        {
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
                LogManager.Singleton.DefaultLog.LogMessage("Start up state : " + newState.ToString());
                
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

        public void Shutdown()
        {
            while (mStateStack.Count > 0)
                PopState();
        }
    }
}
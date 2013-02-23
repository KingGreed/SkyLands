using System;
using System.Collections.Generic;
using Mogre;

using Game.World;
using Game.States;
using Game.MyGameConsole;

namespace Game.CharacSystem
{
    public class CharacMgr
    {
        private List<VanillaCharacter> mCharacList;
        private MainPlayerCamera       mMainPlayerCam;
        private StateManager           mStateMgr;
        private MoisManager            mInput;
        private MainWorld              mWorld;
        private string                 mMeshName = "Sinbad.mesh";

        public StateManager     StateMgr      { get { return this.mStateMgr; } }
        public SceneManager     SceneMgr      { get { return this.mStateMgr.SceneMgr; } }
        public MoisManager      Input         { get { return this.mInput; } }
        public MainWorld        World         { get { return this.mWorld; } }
        public MainPlayerCamera MainPlayerCam { get { return this.mMainPlayerCam; } }

        public CharacMgr(StateManager stateMgr, MainWorld world)
        {
            this.mStateMgr = stateMgr;
            this.mInput = stateMgr.Input;
            this.mWorld = world;
            this.mCharacList = new List<VanillaCharacter>();
        }

        public void AddCharacter(CharacterInfo info)
        {
            string type;

            if (this.mCharacList.Count == 0 || info.IsPlayer)
            {
                type = "Player";
                this.mCharacList.Add(new VanillaPlayer(this, this.mMeshName, info, this.mInput));
                if (this.mCharacList.Count == 1)
                {
                    this.mMainPlayerCam = new MainPlayerCamera(this.mStateMgr.Camera, (this.mCharacList[0] as VanillaPlayer), this.mStateMgr.Window.Width, this.mStateMgr.Window.Height);
                    (this.GetCharacter() as VanillaPlayer).AttachCamera(this.mMainPlayerCam);
                }
            }
            else
            {
                type = "NonPlayer";
                this.mCharacList.Add(new VanillaNonPlayer(this, this.mMeshName, info));
            }

            LogManager.Singleton.DefaultLog.LogMessage(type + " " + info.Name + " added");
        }

        public VanillaCharacter GetCharacter(int index = 0) { return this.mCharacList[index]; }    // By default return the main player

        public void Update(float frameTime)
        {
            foreach (VanillaCharacter charac in this.mCharacList)
                charac.Update(frameTime);

            this.mMainPlayerCam.Update();
        }

        public void Dispose()
        {
            this.mCharacList.Clear();
            this.mMainPlayerCam.Dispose();
        }
    }
}

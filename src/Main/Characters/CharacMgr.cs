using System;
using System.Collections.Generic;
using Mogre;

using Game.World;

namespace Game.CharacSystem
{
    public class CharacMgr
    {
        public static float YAW_SENSIVITY = 0.15f;   // Temp
        public static float PITCH_SENSIVITY = 0.15f;   // Temp

        private List<VanillaCharacter>  mCharacList;
        private MainPlayerCamera mMainPlayerCam;
        private SceneManager     mSceneMgr;
        private MoisManager      mInput;
        private MainWorld        mWorld;
        private string           mMeshName = "Sinbad.mesh";

        public SceneManager     SceneMgr      { get { return this.mSceneMgr; } }
        public MoisManager      Input         { get { return this.mInput; } }
        public MainWorld        World         { get { return this.mWorld; } }
        public MainPlayerCamera MainPlayerCam { get { return this.mMainPlayerCam; } }

        public CharacMgr(SceneManager sceneMgr, MoisManager input, MainWorld world, Camera cam)
        {
            this.mSceneMgr = sceneMgr;
            this.mInput = input;
            this.mWorld = world;
            this.mMainPlayerCam = new MainPlayerCamera(cam);
            this.mCharacList = new List<VanillaCharacter>();
        }

        public void AddCharacter(CharacterInfo info)
        {
            string type;

            if (this.mCharacList.Count == 0 || info.IsPlayer)
            {
                type = "Player";
                this.mCharacList.Add(new VanillaPlayer(this, this.mMeshName, info, this.mInput));
                if (this.mCharacList.Count == 1) { this.mMainPlayerCam.AttachToPlayer(this.mCharacList[0] as VanillaPlayer); }
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

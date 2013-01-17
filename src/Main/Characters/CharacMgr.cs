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
        
        private List<Character>  mCharacList = new List<Character>();
        private MainPlayerCamera mMainPlayerCam;
        private SceneManager     mSceneMgr;
        private MoisManager      mInput;
        private MainWorld        mWorld;

        public MainPlayerCamera MainPlayerCam { get { return this.mMainPlayerCam; } }

        public CharacMgr(SceneManager sceneMgr, MoisManager input, MainWorld world, Camera cam)
        {
            this.mSceneMgr = sceneMgr;
            this.mInput = input;
            this.mWorld = world;
            this.mMainPlayerCam = new MainPlayerCamera(cam);
        }

        public void AddPlayer(string meshName, CharacterInfo info)
        {
            this.mCharacList.Add(new Player(this.mSceneMgr, meshName, info, this.mInput, this.mWorld));
            if (this.mCharacList.Count == 1) { this.mMainPlayerCam.AttachToPlayer((Player)this.mCharacList[0]); }

            LogManager.Singleton.DefaultLog.LogMessage("Player added");
        }

        public Character GetCharacter(int index = 0) { return this.mCharacList[index]; }    // By default return the main player

        public void Update(float frameTime)
        {
            foreach (Character charac in this.mCharacList)
                charac.Update(frameTime);

            this.mMainPlayerCam.Update();
        }
    }
}

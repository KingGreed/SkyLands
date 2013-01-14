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
        
        private List<Character> mCharacList = new List<Character>();
        private MainPlayerCamera mMainPlayerCam;

        public MainPlayerCamera MainPlayerCam { get { return this.mMainPlayerCam; } }
        
        public CharacMgr(Camera cam)
        {
            this.mMainPlayerCam = new MainPlayerCamera(cam);
        }

        public void AddPlayer(SceneManager sceneMgr, string meshName, CharacterInfo info, MoisManager input, MainWorld world)
        {
            this.mCharacList.Add(new Player(sceneMgr, meshName, info, input, world));
            if (this.mCharacList.Count == 1) { this.mMainPlayerCam.AttachToPlayer((Player)this.mCharacList[0]); }

            LogManager.Singleton.DefaultLog.LogMessage("Player added");
        }

        public Character GetCharacter(int index = 0)   // By default, return the main character which is the actual player
        {
            return this.mCharacList[index];
        }

        public void Update(float frameTime)
        {
            foreach (Character charac in this.mCharacList)
                charac.Update(frameTime);

            this.mMainPlayerCam.Update();
        }
    }
}

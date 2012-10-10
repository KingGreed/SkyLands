using System;
using System.Collections.Generic;
using Mogre;

using Game.Terrain;


namespace Game.PlayerSystem
{
    class PlayerMgr
    {
        protected World mWorldRef;
        protected SceneManager mSceneMgr;

        Dictionary<string, Player> mPlayers = new Dictionary<string, Player>();

        public PlayerMgr(ref World worldRef, ref SceneManager sceneMgr)
        {
            this.mWorldRef = worldRef;
            this.mSceneMgr = sceneMgr;
        }

        public void addPlayer(string playerName, Vector3 coord) {
            try { 
                this.mPlayers.Add(playerName, new Sinbad(ref this.mWorldRef, ref coord, ref this.mSceneMgr));
            }
            catch (ArgumentException)
            {
                LogManager.Singleton.DefaultLog.LogMessage("An element with name " + playerName + "already exists.");
            }
        }
    }
}

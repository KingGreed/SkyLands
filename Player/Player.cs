using System;
using System.Collections.Generic;
using Mogre;

using Game.Terrain;

namespace Game.PlayerSystem
{
    abstract class Player
    {
        protected World mWorldRef;
        protected Vector3 mCoord;
        protected SceneManager mSceneMgr;
        
        protected string mPLayerName {get; private set;} 

        public Player(ref World worldRef, ref Vector3 coord, ref SceneManager sceneMgr){
            this.mWorldRef = worldRef;
            this.mCoord = coord;
            this.mSceneMgr = sceneMgr;
        }

        public void addPlayer(string name) { }
    }
}

using System;
using Mogre;

namespace Game.CharacSystem
{
    public struct CharacterInfo
    {
        public static int lastPlayerId = 0;
        private int     mId;
        private string  mName;
        private Vector3 mSpawnPoint;
        private Vector3 mIslandLoc;
        private float   mLife;
        private bool    mIsPlayer;

        public int     Id         { get { return this.mId; } }
        public string  Name       { get { return this.mName; } }
        public Vector3 SpawnPoint { get { return this.mSpawnPoint; } set { this.mSpawnPoint = value; } }
        public Vector3 IslandLoc  { get { return this.mIslandLoc; } set { this.mIslandLoc = value; } }
        public float   Life       { get { return this.mLife; }       set { this.mLife = value; } }
        public bool    IsPlayer   { get { return this.mIsPlayer; } }

        public CharacterInfo(string name, bool isPlayer = false, float life = 100)
        {
            this.mId = lastPlayerId;
            lastPlayerId++;
            this.mName = name;
            this.mSpawnPoint = Vector3.ZERO;
            this.mIslandLoc = Vector3.ZERO;
            this.mIsPlayer = isPlayer;
            this.mLife = life;
        }
    }
}

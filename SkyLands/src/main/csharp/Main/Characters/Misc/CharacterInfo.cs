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
        private float   mLife;
        private bool    mIsPlayer;

        public int     Id         { get { return this.mId; } }
        public string  Name        { get { return this.mName; }       set { this.mName = value; } }
        public Vector3 SpawnPoint { get { return this.mSpawnPoint; } set { this.mSpawnPoint = value; } }
        public float   Life       { get { return this.mLife; }       set { this.mLife = value; } }
        public bool    IsPlayer   { get { return this.mIsPlayer; } }

        public CharacterInfo(string name, bool isPlayer = false, float life = -1)
        {
            this.mId = lastPlayerId;
            lastPlayerId++;
            this.mName = name;
            this.mSpawnPoint = Vector3.ZERO;
            this.mIsPlayer = isPlayer;
            if (life == -1) { life = this.mIsPlayer ? VanillaPlayer.DEFAULT_PLAYER_LIFE : VanillaNonPlayer.DEFAULT_NPC_LIFE; }
            this.mLife = life;
        }

        public static void ResetID() { lastPlayerId = 0; }
    }
}

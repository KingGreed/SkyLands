using Mogre;

namespace Game.CharacSystem
{
    public struct CharacterInfo
    {
        private static int mLastPlayerId = 0;
        private int     mId;
        private string  mName;
        private Vector3 mSpawnPoint;
        private float   mLife, mMana;
        private bool    mIsPlayer;

        public int     Id         { get { return this.mId; } }
        public string  Name       { get { return this.mName; }       set { this.mName = value; } }
        public Vector3 SpawnPoint { get { return this.mSpawnPoint; } set { this.mSpawnPoint = value; } }
        public float   Life       { get { return this.mLife; }       set { this.mLife = value; } }
        public float   Mana       { get { return this.mMana; }       set { this.mMana = value; } }
        public bool    IsPlayer   { get { return this.mIsPlayer; } }

        public CharacterInfo(string name, bool isPlayer = false)
        {
            this.mId = mLastPlayerId;
            mLastPlayerId++;
            this.mName = name;
            this.mSpawnPoint = Vector3.ZERO;
            this.mIsPlayer = isPlayer;
            this.mLife = this.mIsPlayer ? VanillaPlayer.DEFAULT_PLAYER_LIFE : VanillaNonPlayer.DEFAULT_NPC_LIFE;
            this.mMana = this.mIsPlayer ? VanillaPlayer.DEFAULT_PLAYER_MANA : 0;
        }

        public static void ResetID() { mLastPlayerId = 0; }
    }
}

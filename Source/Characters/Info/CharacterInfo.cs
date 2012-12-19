using System;
using Mogre;

namespace Game.CharacSystem
{
    struct CharacterInfo
    {
        private string mName;
        private Vector3 mSpawnPoint;
        private float mLife;

        public string Name { get { return this.mName; } set { this.mName = value; } }
        public Vector3 SpawnPoint { get { return this.mSpawnPoint; } set { this.mSpawnPoint = value; } }
        public float Life { get { return this.mLife; } set { this.mLife = value; } }

        public CharacterInfo(string name, Vector3 spawnPoint, float life = 100)
        {
            this.mName = name;
            this.mSpawnPoint = spawnPoint;
            this.mLife = life;
        }
    }
}

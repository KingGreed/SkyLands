using System;
using System.Collections.Generic;
using Mogre;


namespace Game.CharacSystem
{
    struct CharacterInfo
    {
        public string name;
        public Vector3 spawnPoint;
        public float life;

        public CharacterInfo(string gName, Vector3 gSpawnPoint, float gLife = 100)
        { 
            this.name = gName;
            this.spawnPoint = gSpawnPoint;
            this.life = gLife;
        }
    }
    
    /* Mother class of Player and NonPlayer */
    abstract class Character
    {
        protected Race mRace;
        protected CharacterInfo mInfo;

        public Character(Race charac, CharacterInfo info)
        {
            this.mRace = charac;
            this.mInfo = info;

            this.mRace.Node.SetPosition(info.spawnPoint.x, info.spawnPoint.y, info.spawnPoint.z);
        }

        public void Update(float frameTime)
        {
            this.mRace.UpdateAnimation(frameTime);
        }

        public Vector3 Move(Vector3 direction)  // Return the vector of the effective move
        {
            return Vector3.ZERO;
        }

        public void Jump()
        {

        }
    }
}

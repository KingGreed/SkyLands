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

        public Character(Race race, CharacterInfo info)
        {
            this.mRace = race;
            this.mInfo = info;

            this.mRace.SetPosition(info.spawnPoint);
        }

        public void Update(float frameTime)
        {
            this.mRace.Update(frameTime);
        }

        protected void Move(Vector3 direction)
        {
            mRace.ChangeDirection(direction);

            if (direction.z != 0 && mRace.CurrentAnim != Race.AnimType.run)
                mRace.ChangeAnimation(Race.AnimType.run);

            if (direction == Vector3.ZERO && mRace.CurrentAnim != Race.AnimType.idle)
                mRace.ChangeAnimation(Race.AnimType.idle);
        }

        protected void Jump()
        {

        }
    }
}

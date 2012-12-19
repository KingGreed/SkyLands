using System;
using System.Collections.Generic;
using Mogre;


namespace Game.CharacSystem
{
    /* Mother class of Player and NonPlayer */
    abstract class Character
    {
        protected Race mRace;
        protected CharacterInfo mCharInfo;
        protected MovementInfo mMovementInfo;

        public Character(Race race, CharacterInfo charInfo)
        {
            this.mRace = race;
            this.mCharInfo = charInfo;
            this.mMovementInfo = new MovementInfo();

            this.mRace.FeetPosition = this.mCharInfo.SpawnPoint;
        }

        public virtual void Update(float frameTime)
        {
            this.mRace.Update(frameTime, this.mMovementInfo);
            this.mMovementInfo = new MovementInfo();
        }
    }
}

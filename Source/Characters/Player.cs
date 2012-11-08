using System;
using System.Collections.Generic;
using Mogre;

namespace Game.CharacSystem
{
    /* Needs to be completed for multiplayer *///Not needed
    class Player : Character
    {
        /* Constructors of Player has to be called by the CharacMgr *///Not needed or created doc comments. But still not needed
        public Player(Breed charac, CharacterInfo info)
        {
            mBreed = charac;

            mInfo = info;
            update();
        }
    }
}

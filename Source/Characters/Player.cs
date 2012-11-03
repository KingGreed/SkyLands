using System;
using System.Collections.Generic;
using Mogre;

namespace Game.CharacSystem
{
<<<<<<< HEAD
    /* Needs to be completed for multiplayer *///Not needed
    class Player : Character
    {
        /* Constructors of Player has to be called by the CharacMgr *///Not needed or created doc comments. But still not needed
=======
    /* Needs to be completed for multiplayer */
    class Player : Character
    {
        /* Constructors of Player has to be called by the CharacMgr */
>>>>>>> 5c3cc93d3dff01cb6a4c43c92d95002796543de9
        public Player(Breed charac, CharacterInfo info)
        {
            mBreed = charac;

            mInfo = info;
            update();
        }
    }
}

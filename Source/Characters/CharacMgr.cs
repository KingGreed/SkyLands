using System;
using System.Collections.Generic;
using Mogre;

using Game;

namespace Game.CharacSystem
{
    class CharacMgr
    {
        private World mWorldRef;
        private List<Character> mCharacList = new List<Character>();

        public CharacMgr(ref World worldRef)
        {
            mWorldRef = worldRef;
        }

        public void addPlayer(Breed breed, CharacterInfo info)
        {
            mCharacList.Add(new Player(breed, info));
        }

        public Character getCharacter(int index = 0)   // By default, return the main character which is the player
        {
            return mCharacList[index];
        }
    }
}

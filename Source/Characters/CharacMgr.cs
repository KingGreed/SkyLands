using System;
using System.Collections.Generic;
using Mogre;

using Game;

namespace Game.CharacSystem
{
    class CharacMgr
    {
        private List<Character> mCharacList = new List<Character>();

        public CharacMgr()
        {
        }

        public void addPlayer(Race breed, CharacterInfo info)
        {
            mCharacList.Add(new Player(breed, info));
        }

        public Character getCharacter(int index = 0)   // By default, return the main character which is the player
        {
            return mCharacList[index];
        }
    }
}

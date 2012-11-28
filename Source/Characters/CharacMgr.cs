using System;
using System.Collections.Generic;
using Mogre;

namespace Game.CharacSystem
{
    class CharacMgr
    {
        private List<Character> mCharacList = new List<Character>();

        public CharacMgr()
        {
        }

        public void AddPlayer(Race breed, CharacterInfo info)
        {
            this.mCharacList.Add(new Player(breed, info));
        }

        public Character GetCharacter(int index = 0)   // By default, return the main character which is the player
        {
            return this.mCharacList[index];
        }

        public void Update(float frameTime)
        {
            for (int i = 0; i < this.mCharacList.Count; i++)
                this.mCharacList[i].Update(frameTime);
        }
    }
}

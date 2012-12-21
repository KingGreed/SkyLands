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

        public void AddPlayer(SceneManager sceneMgr, string meshName, CharacterInfo info, MoisManager input = null,  Camera cam = null)
        {
            this.mCharacList.Add(new Player(sceneMgr, meshName, info, input, cam));

            LogManager.Singleton.DefaultLog.LogMessage("Player added");
        }

        public Character GetCharacter(int index = 0)   // By default, return the main character which is the player
        {
            return this.mCharacList[index];
        }

        public void Update(float frameTime)
        {
            for (int i = 0; i < this.mCharacList.Count; i++)
            {
                if (this.mCharacList[i].GetType() == typeof(Player))
                {
                    Player player = (Player)this.mCharacList[i];
                    player.Update(frameTime);
                }
                else
                {
                    NonPlayer nonPlayer = (NonPlayer)this.mCharacList[i];
                    nonPlayer.Update(frameTime);
                }
            }
        }
    }
}

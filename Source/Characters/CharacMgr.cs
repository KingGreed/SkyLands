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

        public void AddPlayer(Race race, CharacterInfo info, SceneManager sceneMgr = null, string cameraName = null)
        {
            this.mCharacList.Add(new Player(race, info, sceneMgr, cameraName));

            LogManager.Singleton.DefaultLog.LogMessage("Player added");
        }

        public Character GetCharacter(int index = 0)   // By default, return the main character which is the player
        {
            return this.mCharacList[index];
        }

        public void Update(float frameTime, MoisManager input)
        {
            for (int i = 0; i < this.mCharacList.Count; i++)
            {
                if (this.mCharacList[i].GetType() == typeof(Player))
                {
                    Player player = (Player)this.mCharacList[i];

                    if (i == 0)
                        player.Update(frameTime, input);
                    else
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

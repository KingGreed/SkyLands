using System;
using System.Collections.Generic;
using Mogre;

namespace Game.CharacSystem
{
    struct CharacterInfo
    {
        public string name;
        public float life;

        public CharacterInfo(string gName, float gLife = 100) { name = gName; life = gLife;}
    }
    
    /* Mother class of Player and NonPlayer */
    abstract class Character
    {
        public Breed mBreed { get; protected set; }
        public CharacterInfo mInfo { get; protected set; }

        public void update()
        {
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

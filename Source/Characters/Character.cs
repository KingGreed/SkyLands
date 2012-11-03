using System;
using System.Collections.Generic;
using Mogre;

namespace Game.CharacSystem
{
    struct CharacterInfo
    {
        public string name;
        public float life;
        public Vector3 coord;

<<<<<<< HEAD
        public CharacterInfo(string _name, Vector3 _coord,  float _life = 100) { name = _name; life = _life; coord = _coord; }//Never use "_" before var names
=======
        public CharacterInfo(string _name, Vector3 _coord,  float _life = 100) { name = _name; life = _life; coord = _coord; }
>>>>>>> 5c3cc93d3dff01cb6a4c43c92d95002796543de9
    }
    
    /* Mother class of Player and NonPlayer */
    abstract class Character
    {
        public Breed mBreed { get; protected set; }
        public CharacterInfo mInfo { get; protected set; }

        public void update()
        {
            mBreed.mNode.Position = mInfo.coord;
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

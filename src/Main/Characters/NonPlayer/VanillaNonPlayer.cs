using System;
using Mogre;

using Game.World;

namespace Game.CharacSystem
{
    /* Known as NPC : call it NPC then! Everyybody knows what an NPC is. */
    class VanillaNonPlayer : VanillaCharacter
    {
        public VanillaNonPlayer(CharacMgr characMgr, string meshName, CharacterInfo info) : base(characMgr, meshName, info)
        {

        }

        public new void Update(float frameTime)
        {
        }
    }
}

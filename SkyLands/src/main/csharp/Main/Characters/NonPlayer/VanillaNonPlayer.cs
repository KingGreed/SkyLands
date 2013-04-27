using System;
using Mogre;

using Game.World;

namespace Game.CharacSystem
{
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

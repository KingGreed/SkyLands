using System;
using Mogre;

using Game.World;

namespace Game.CharacSystem
{
    /* Known as NPC : call it NPC then! Everyybody knows what an NPC is. */
    class NonPlayer : Character
    {
        public NonPlayer(SceneManager sceneMgr, string meshName, CharacterInfo info, MainWorld world) : base(sceneMgr, meshName, info, false, world)
        {

        }

        public new void Update(float frameTime)
        {
        }
    }
}

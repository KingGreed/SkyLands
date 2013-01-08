using System;
using Mogre;


namespace Game.CharacSystem
{
    /* Known as NPC : call it NPC then! Everyybody knows what an NPC is. */
    class NonPlayer : Character
    {
        public NonPlayer(SceneManager sceneMgr, string meshName, CharacterInfo info) : base(sceneMgr, meshName, info, false)
        {

        }

        public new void Update(float frameTime)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.States
{
    class GameState : State
    {
        public GameState() : base()
        {

        }

        public override bool Startup(StateManager stateMgr)
        {
            if (isStartedUp)
                return false;

            mStateMgr = stateMgr;
            isStartedUp = true;

            return true;
        }

        public override void Shutdown()
        {
            if (mStateMgr == null)
                return;
            
            mStateMgr = null;
            isStartedUp = false;
        }

        public override void Update(float frameTime)
        {
            if (mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_ESCAPE))
                mStateMgr.RequestShutdown();
        }
    }
}

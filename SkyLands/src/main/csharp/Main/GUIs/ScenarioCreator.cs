using System;
using Awesomium.Core;
using Mogre;

using Game.States;
using System.Windows.Forms;


using Game.BaseApp;


namespace Game.GUIs {
    class ScenarioCreator : GUI {
        private readonly StateManager mStateMgr;

        public ScenarioCreator(StateManager stateMgr)
            : base(new Vector2(0, 0), new Vector2(437, 900), "scenario.html") {
            this.mStateMgr = stateMgr;
        }
    }
}

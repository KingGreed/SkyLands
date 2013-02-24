using System;
using System.Collections.Generic;
using Mogre;

using Miyagi.Common.Events;
using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Layout;
using Miyagi.UI.Controls.Styles;

using Game.GUICreator;
using Game.States;

namespace Game.IGConsole
{
    class GameGUI : GUIFactory
    {
        public GameGUI(StateManager stateMgr) : base(stateMgr, "Game GUI")
        {
            
        }

        protected override void CreateGUI()
        {
            //throw new NotImplementedException();
        }
    }
}

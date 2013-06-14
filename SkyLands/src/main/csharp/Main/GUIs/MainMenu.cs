using System.IO;
using System.Windows.Forms;

using Awesomium.Core;
using Mogre;

using Game.States;
using Game.BaseApp;

namespace Game.GUIs {
    public class MainMenu : GUI {
        public MainMenu(StateManager stateMgr)
            : base(new Vector2(0, 0), new Vector2(404, 44), "MainMenu.html") {
            ((JSObject)stateMgr.WebView.CreateGlobalJavascriptObject("MainMenuJSObject")).Bind("exit", false, (sender, args) => stateMgr.RequestStatePop());
        }

        public override void onDocumentReady(object sender, Awesomium.Core.UrlEventArgs e) {
            
        }
    }
}

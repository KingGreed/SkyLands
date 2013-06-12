using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

using Mogre;

using Awesomium.Core;
using Awesomium.Windows.Forms;

namespace Game.BaseApp {
    public abstract class GUI {

        public GUI(Vector2 pos, Vector2 size, string url) {

            OgreForm.webView.Location = new Point((int)pos.x,  (int)pos.y);
            OgreForm.webView.Size     = new Size ((int)size.x, (int)size.y);

            OgreForm.webView.Source = new Uri("file://" + Directory.GetCurrentDirectory() + "/media/web/MainMenu.html");
            OgreForm.webView.DocumentReady += this.whenDocumentReady;
        }

        public GUI(Vector2 pos, Vector2 size, string url, DockStyle d) : this(pos, size, url) {
            this.setDock(d);
        }

        public void setDock(DockStyle d) { OgreForm.webView.Dock = d; }

        public void resize(float x, float y) {
            ///TODO resize method (replace widget)
            OgreForm.webView.ExecuteJavascript("resize(" + x + ", " + y + ")");
        }


        public void whenDocumentReady(object sender, UrlEventArgs e) {
            this.onDocumentReady(sender, e);
            OgreForm.webView.DocumentReady -= this.whenDocumentReady;
        }

        public abstract void onDocumentReady(object sender, UrlEventArgs e);
    }
}

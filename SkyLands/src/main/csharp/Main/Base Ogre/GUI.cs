using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Mogre;

using Awesomium.Core;
using Awesomium.Windows.Forms;

using API.Generic;

namespace Game.BaseApp {
    public abstract class GUI {
        public static List<WebControl> WebControls = new List<WebControl>();
        public static bool Visible {
            get { return OgreForm.webView.Visible; }
            set { OgreForm.webView.Visible = value; }
        }

        protected Vector2 mInitSize;

        protected GUI(Vector2 pos, Vector2 size, string url) {
            this.mInitSize = size;

            pos *= OgreForm.Ratio;
            size *= OgreForm.Ratio * Cst.GUI_RATIO;
            OgreForm.webView.Location = new Point((int)pos.x,  (int)pos.y);
            OgreForm.webView.Size     = new Size ((int)size.x, (int)size.y);

            OgreForm.webView.Source = new Uri("file://" + Directory.GetCurrentDirectory() + "/media/web/" + url);
            OgreForm.webView.DocumentReady += this.whenDocumentReady;

            this.setDock(DockStyle.None);
        }

        protected GUI(Vector2 pos, Vector2 size, string url, DockStyle d) : this(pos, size, url) {
            this.setDock(d);
        }

        public void setDock(DockStyle d) { 
            OgreForm.webView.Dock = d;
        }

        public void whenDocumentReady(object sender, UrlEventArgs e) {  //TODO : Error when changing island. All the onDocumentReady are called again
            if(OgreForm.webView == null || !OgreForm.webView.IsLive) { return; }
            this.onDocumentReady(sender, e);
            OgreForm.webView.DocumentReady -= this.whenDocumentReady;
            Visible = true;
        }

        public virtual void onDocumentReady(object sender, UrlEventArgs e)
        {
            Vector2 ratio = new Vector2(OgreForm.webView.Size.Width / this.mInitSize.x, OgreForm.webView.Size.Height / this.mInitSize.y);
            ResizeJavascript(OgreForm.webView, ratio);
        }

        public static void resize(WebControl webControl, float x, float y) {
            webControl.Location = new Point((int)(webControl.Location.X * x),
                                            (int)(webControl.Location.Y * y));
            webControl.Size = new Size((int)(webControl.Size.Width * x),
                                       (int)(webControl.Size.Height * y));
            ResizeJavascript(webControl, x, y);
        }

        public static void ResizeJavascript(WebControl webControl, Vector2 ratio) { ResizeJavascript(webControl, ratio.x, ratio.y); }
        public static void ResizeJavascript(WebControl webControl, float x, float y)
        {
            webControl.ExecuteJavascript("resize(" + Convert.ToString(x).Replace(',', '.') + ", "
                                                   + Convert.ToString(y).Replace(',', '.') + ")");
        }
    }
}

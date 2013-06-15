using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

using Mogre;

using Awesomium.Core;

namespace Game.BaseApp {
    public abstract class GUI {
        public static List<GUI> GUIs = new List<GUI>();
        private bool mVisible;

        public bool Visible {
            get { return this.mVisible; }
            set
            {
                this.mVisible = value;
                if (this.mVisible) { OgreForm.webView.Show(); }
                else               { OgreForm.webView.Hide(); }
            }
        }

        protected GUI(Vector2 pos, Vector2 size, string url) {

            OgreForm.webView.Location = new Point((int)pos.x,  (int)pos.y);
            OgreForm.webView.Size     = new Size ((int)size.x, (int)size.y);

            OgreForm.webView.Source = new Uri("file://" + Directory.GetCurrentDirectory() + "/media/web/" + url);
            OgreForm.webView.DocumentReady += this.whenDocumentReady;

            this.setDock(DockStyle.None);
            GUIs.Add(this);

            this.mVisible = true;
        }

        protected GUI(Vector2 pos, Vector2 size, string url, DockStyle d) : this(pos, size, url) {
            this.setDock(d);
        }

        public void setDock(DockStyle d) { OgreForm.webView.Dock = d; }

        public void resize(float x, float y, float ratioTotalX, float ratioTotalY) {
            OgreForm.webView.Location = new Point((int)(OgreForm.webView.Location.X * ratioTotalX),
                                                  (int)(OgreForm.webView.Location.Y * ratioTotalY));
            OgreForm.webView.ExecuteJavascript("resize(" + Convert.ToString(x).Replace(',', '.') + ", "
                                                         + Convert.ToString(y).Replace(',', '.') + ")");
        }


        public void whenDocumentReady(object sender, UrlEventArgs e) {
            if(OgreForm.webView == null || !OgreForm.webView.IsLive) { return; }
            this.onDocumentReady(sender, e);
            OgreForm.webView.DocumentReady -= this.whenDocumentReady;
        }

        public abstract void onDocumentReady(object sender, UrlEventArgs e);
    }
}

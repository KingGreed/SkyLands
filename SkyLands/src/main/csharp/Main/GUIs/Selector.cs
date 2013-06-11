using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Awesomium.Windows.Forms;

namespace Game.GUIs
{
    public class Selector
    {
        private WebControl mWebView;
        
        public int SelectorPos { get; set; }
        public string Material { get; set; }
        public bool IsBullet { get; set; }

        public Selector(WebControl webView)
        {
            this.mWebView = webView;
        }
    }
}

using System;
using System.Collections.Generic;

using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.UI;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Layout;

namespace Game.GUICreator
{
    public class MiyagiMgr
    {
        private MiyagiSystem             mSystem;
        private Cursor                   mCursor;
        private Dictionary<string, Skin> mSkins;
        private Dictionary<string, Font> mFonts;
        private Mogre.Vector2            mWindowSize;
        //private int                      mZOrder;

        public Dictionary<string, Skin> Skins { get { return this.mSkins; } }
        public Dictionary<string, Font> Fonts { get { return this.mFonts; } }
        public Size WndSize                   { get { return new Size((int)this.mWindowSize.x, (int)this.mWindowSize.y); } }
        public bool CursorVisibility          { get { return this.mCursor.Visible; } set { this.mCursor.Visible = value; } }
        //public int  ZOrder                    { get { return this.mZOrder; } set { this.mZOrder = value; } }

        public MiyagiMgr(MoisManager input, Mogre.Vector2 windowSize)
        {
            this.mSystem = new MiyagiSystem("Mogre");
            this.mFonts = new Dictionary<string, Font>();
            this.mSkins = new Dictionary<string, Skin>();
            this.mWindowSize = windowSize;
            //this.mZOrder = 0;

            this.mSystem.PluginManager.LoadPlugin(@"Miyagi.Plugin.Input.Mois.dll", input.KeyBoard, input.Mouse);

            mSkins = new Dictionary<string, Skin>();
            foreach (Skin skin in Skin.CreateFromXml("./Media/skins/BasicSkins.xml"))
                mSkins.Add(skin.Name, skin);

            foreach(Font font in TrueTypeFont.CreateFromXml("./Media/fonts/TrueTypeFonts.xml", this.mSystem))
                this.mFonts.Add(font.Name, font);
            Font.Default = this.mFonts["Medium_BlueHighway"];

            this.mCursor = new Cursor(this.mSkins["cursorSkin"], new Size(30, 30), new Point(0, 0), true);
            this.mSystem.GUIManager.Cursor = this.mCursor;
        }
        public void AllGuisVisibility(bool isVisible)
        {
            foreach (GUI gui in this.mSystem.GUIManager.GUIs) { gui.Visible = isVisible; }
        }

        public void Add(GUI gui) { this.mSystem.GUIManager.GUIs.Add(gui); }
        public void Update()     { this.mSystem.Update(); }
        public void ShutDown()   { this.mSystem.Dispose(); }
    }
}
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
    public class MiyagiManager
    {
        private MiyagiSystem mMiyagiSys;
        private Dictionary<string, Skin> mSkins;
        private Dictionary<string, Font> mFonts;
        private Mogre.Vector2 mWindowSize;

        public Dictionary<string, Skin> Skins { get { return this.mSkins; } }
        public Dictionary<string, Font> Fonts { get { return this.mFonts; } }
        public Mogre.Vector2 WindowSize       { get { return this.mWindowSize; } }

        public MiyagiManager(MoisManager input, Mogre.Vector2 windowSize)
        {
            this.mMiyagiSys = new MiyagiSystem("Mogre");
            this.mFonts = new Dictionary<string, Font>();
            this.mSkins = new Dictionary<string, Skin>();
            this.mWindowSize = windowSize;

            this.mMiyagiSys.PluginManager.LoadPlugin(@"Miyagi.Plugin.Input.Mois.dll", input.KeyBoard, input.Mouse);

            mSkins = new Dictionary<string, Skin>();
            foreach (Skin skin in Skin.CreateFromXml("../../Media/skins/DefaultSkin.xml"))
                mSkins.Add(skin.Name, skin);

            foreach (Font font in TrueTypeFont.CreateFromXml("../../src/Media/fonts/TrueTypeFonts.xml", this.mMiyagiSys))
                this.mFonts.Add(font.Name, font);
            Font.Default = this.mFonts["BlueHighway"];
        }

        public void Add(GUI gui) { this.mMiyagiSys.GUIManager.GUIs.Add(gui); }
        public void Update()     { this.mMiyagiSys.Update(); }
        public void ShutDown()   { this.mMiyagiSys.Dispose(); }
    }
}
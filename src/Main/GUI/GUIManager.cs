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
        private MiyagiSystem mGUISystem;
        private Dictionary<string, Skin> mSkins;
        private Dictionary<string, Font> mFonts;

        public MiyagiSystem System { get { return this.mGUISystem; } }

        public MiyagiManager()
        {
            this.mGUISystem = new MiyagiSystem("Mogre");
            this.mFonts = new Dictionary<string, Font>();
            this.mSkins = new Dictionary<string, Skin>();
        }

        public bool Startup(MoisManager input)
        {
            this.mGUISystem.PluginManager.LoadPlugin(@"Miyagi.Plugin.Input.Mois.dll", input.KeyBoard, input.Mouse);

            foreach (Font font in TrueTypeFont.CreateFromXml("../../Media/fonts/TrueTypeFonts.xml", this.mGUISystem))
                this.mFonts.Add(font.Name, font);
            Font.Default = this.mFonts["BlueHighway"];
            
            
            return true;
        }

        public void Update() { mGUISystem.Update(); }

        public void ShutDown() { this.mGUISystem.Dispose(); }
    }
}
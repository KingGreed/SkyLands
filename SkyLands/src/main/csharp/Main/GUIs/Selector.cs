﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mogre;

using API.Generic;

using Game.BaseApp;
using Game.World.Generator;
using Game.CharacSystem;

namespace Game.GUIs
{
    public static class Selector {
        public static readonly Vector2 IMAGE_SIZE = new Vector2(202, 22);
        public static readonly Vector2 WANTED_SIZE = IMAGE_SIZE * Cst.GUI_RATIO;
        public static readonly Dictionary<byte, string> MagicCubes = new Dictionary<byte, string>
        { { 252, "fire.png" }, { 253, "waterCube.png" }, { 254, "crystal.png" } };
        
        private static int pos;

        public static byte SelectedId { get; private set; }
        public static bool IsBullet { get; private set; }
        public static int SelectorPos
        {
            get { return pos; }
            set
            {
                pos = value;
                OgreForm.SelectBar.ExecuteJavascript("setSelectorPosition(" + pos + ")");
                string imgName = OgreForm.SelectBar.ExecuteJavascriptWithResult("getMaterialAt(" + pos + ")");
                if (imgName == "blank.png") { SelectedId = 0; IsBullet = false; return; }

                foreach (byte b in MagicCubes.Keys.Where(b => MagicCubes[b] == imgName))
                {
                    SelectedId = b;
                    IsBullet = true;
                    return;
                }

                SelectedId = VanillaChunk.textureToBlock[imgName].getId();
                IsBullet = false;
            }
        }

        public static void Init(Inventory inventory)
        {
            for (int i = 0; i < 10; i++)
                SetBlockAt(i, "blank.png");

            List<byte> keys = new List<byte>(Inventory.MagicCubes.Keys);
            for (int i = 0; i < Inventory.MagicCubes.Count; i++)
                inventory.addAt(new Slot(1, keys[i]), i, 3);

            // TODO : Temp
            inventory.addAt(new Slot(5, 7), 3, 3);
            inventory.addAt(new Slot(10, 4), 0, 0);
            inventory.addAt(new Slot(30, 13), 1, 0);
            inventory.addAt(new Slot(5, 2), 2, 0);
            inventory.addAt(new Slot(50, 3), 3, 0);

            IsBullet = false;
            SelectorPos = 0;
        }

        public static void Resize(Vector2 newSize) {
            OgreForm.SelectBar.Size = new Size((int)newSize.x, (int)newSize.y);
        }

        public static void SetBlockAt(int position, string imgName, int amount = 1) // imgName = "" to change amount
        {
            if(imgName != "")
                OgreForm.SelectBar.ExecuteJavascript("setMaterialAt(" + position + ", '" + imgName + "')");
            if(imgName != "blank.png" && amount > 0)
                OgreForm.SelectBar.ExecuteJavascript("setAmountAt(" + position + ", " + amount + ")");
            if (position == pos) { SelectorPos = pos; } // Actualise the static infos
        }
    }
}

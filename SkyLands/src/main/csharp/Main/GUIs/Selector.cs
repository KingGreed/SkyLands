using System.Collections.Generic;
using System.Drawing;
using Game.CharacSystem;
using Mogre;

using API.Generic;

using Game.BaseApp;
using Game.World.Generator;

namespace Game.GUIs
{
    public static class Selector {
        public static readonly Vector2 IMAGE_SIZE = new Vector2(202, 22);
        public static readonly Vector2 WANTED_SIZE = IMAGE_SIZE * Cst.GUI_RATIO;
        public static readonly Dictionary<string, string> magicCubes = 
            new Dictionary<string, string> { { "fire.png", "fireball" },
                                             { "waterCube.png", "waterCube" },
                                             { "crystal.png", "magicball" } };
        
        private static int pos;

        public static string Material { get; private set; }
        public static bool IsBullet { get; private set; }
        public static int SelectorPos
        {
            get { return pos; }
            set
            {
                pos = value;
                OgreForm.SelectBar.ExecuteJavascript("setSelectorPosition(" + pos + ")");
                string imgName = OgreForm.SelectBar.ExecuteJavascriptWithResult("getMaterialAt(" + pos + ")");
                if (imgName == "blank.png") { Material = ""; IsBullet = false; return; }
                if (magicCubes.ContainsKey(imgName))
                {
                    Material = magicCubes[imgName];
                    IsBullet = true;
                }
                else
                {
                    Material = VanillaChunk.textureToBlock[imgName].getMaterial();
                    IsBullet = false;
                }
            }
        }

        public static void Init(Inventory inventory)
        {
            for (int i = 0; i < 10; i++)
                SetMaterialAt(i, "blank.png");

            List<byte> keys = new List<byte>(inventory.MagicCubes.Keys);
            for (int i = 0; i < inventory.MagicCubes.Count; i++)
                inventory.addAt(new Slot(1, keys[i]), i, 3);

            IsBullet = false;
            SelectorPos = 0;
        }

        public static void SetMaterialAt(int position, string imgName)
        {
            OgreForm.SelectBar.ExecuteJavascript("setMaterialAt(" + position + ", '" + imgName + "')");
            SelectorPos = pos;
        }
    }
}

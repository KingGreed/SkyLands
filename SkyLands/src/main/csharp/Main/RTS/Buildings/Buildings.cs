using System.Collections.Generic;
using Awesomium.Core;
using Game.BaseApp;
using Game.World.Generator;
using Mogre;

using Game.World.Blocks;
using API.Geo.Cuboid;

namespace Game.RTS
{
    public abstract class Building
    {
        public static List<Building> Buildings = new List<Building>();
        public static Island Island;
        private const string ghostBlock = "GhostBlock";

        protected byte[, ,] mBuilding;

        public Vector3 Position { get; protected set; }
        public Vector3 Size { get; protected set; }
        public Dictionary<byte, int> mNeededRessources;

        protected Building(Vector3 pos)
        {
            this.Position = pos;
            this.mNeededRessources = new Dictionary<byte, int>();
            this.Create();

            int i = 40;
            foreach (byte b in this.mNeededRessources.Keys)
            {
                GUI.SetBlockAt(i, VanillaChunk.staticBlock[VanillaChunk.byteToString[b]].getItemTexture(), this.mNeededRessources[b]);
                OgreForm.webView.ExecuteJavascript("setOpacity(0.5, " + i + ")");
                i++;
            }

            this.Build(true);
        }

        protected abstract void Create();

        public void Build(bool ghost = false)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int x = 0; x < this.Size.x; x++)
                {
                    for (int y = 0; y < this.Size.y; y++)
                    {
                        for (int z = 0; z < this.Size.z; z++)
                        {
                            if (this.mBuilding[x, y, z] == 0 || (x == 0 && y == 0 && z == 0)) { continue; }
                            string name = ghost ? ghostBlock : VanillaChunk.staticBlock[VanillaChunk.byteToString[this.mBuilding[x, y, z]]].getName();
                            Vector3 location = this.Position + new Vector3(x, y, z);
                            if (i == 0)
                            {
                                Island.removeFromScene(location);
                                Island.setBlockAt(location, name, true);
                            }
                            else { Island.addBlockToScene(location, name); }
                        }
                    }
                }
            }
        }

        public static void OnBuildingSelected(string building, Vector3 constructionBlockPos)
        {
            switch (building)
            {
                case "HQ":
                    Buildings.Add(new HeadQuarter(constructionBlockPos));
                    break;
            }
        }
    }
}

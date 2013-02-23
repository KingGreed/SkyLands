using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Geo.Cuboid;

using Game.World.Generator;


using Mogre;

namespace Game.World.Display
{
    public class ThreadTerrainRegenerate {
        private Island    mIsland;
        private MainWorld mWorld;

        public ThreadTerrainRegenerate(Island current, MainWorld world) {
            this.mIsland = current;
            this.mWorld  = world;
        }

        public void regenIsland() {
            LogManager.Singleton.DefaultLog.LogMessage("Regenerating Island !");
            List<string> multiToRegenerate = new List<string>();

            for(int i = 0; i < this.mIsland.blocksAdded.Count; i++) {
                this.mIsland.multiList[VanillaChunk.materialToString[this.mIsland.blocksAdded[i].blockName]].addBlock(this.mIsland.blocksAdded[0].position);
                if(!multiToRegenerate.Contains(VanillaChunk.materialToString[this.mIsland.blocksAdded[i].blockName])) {
                    multiToRegenerate.Add(VanillaChunk.materialToString[this.mIsland.blocksAdded[i].blockName]);
                }
            }
            this.mIsland.blocksAdded   = new List<API.Generic.PositionFaceAndName>();
            this.mIsland.blocksDeleted = new List<API.Generic.PositionFaceAndName>();

            string newName;
            char   num;

            foreach(string multi in multiToRegenerate) {
                LogManager.Singleton.DefaultLog.LogMessage("Regen : " + multi);
                this.mIsland.multiList[multi].regen();
                this.mIsland.multiList[multi].display(this.mIsland, mWorld);
                this.mIsland.Node.RemoveAndDestroyChild("MultiBlockNode-" + this.mIsland.multiList[multi].getName());
                newName = this.mIsland.multiList[multi].getName();

                num = (char)((Convert.ToInt32(newName[newName.Length - 1]) + 1) % 10);
                newName.Remove(newName.Length - 1);

                this.mIsland.multiList[multi].setName(newName + num);
                this.mIsland.multiList[multi].addMultiToScene();
            }
            this.mIsland.Node.GetChild(0).RemoveAllChildren();
            LogManager.Singleton.DefaultLog.LogMessage("Thread finished");



        }
    }
}

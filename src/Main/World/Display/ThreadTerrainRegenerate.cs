using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Geo.Cuboid;

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
                this.mIsland.multiList[this.mIsland.blocksAdded[i].blockName].addBlock(this.mIsland.blocksAdded[0].position);
                if(!multiToRegenerate.Contains(this.mIsland.blocksAdded[i].blockName)) {
                    multiToRegenerate.Add(this.mIsland.blocksAdded[i].blockName);
                }
            }
            string newName;
            char   num;

            foreach(string multi in multiToRegenerate) {
                this.mIsland.multiList[multi].display(this.mIsland, mWorld);
                this.mIsland.Node.RemoveAndDestroyChild("MultiBlockNode" + this.mIsland.multiList[multi].getName());
                newName = this.mIsland.multiList[multi].getName();

                num = (char)((Convert.ToInt32(newName[newName.Length - 1]) + 1) % 10);
                newName.Remove(newName.Length - 1);

                this.mIsland.multiList[multi].setName(newName + num);
                this.mIsland.multiList[multi].addMultiToScene();
            }
            this.mIsland.Node.GetChild(0).RemoveAllChildren();



        }
    }
}

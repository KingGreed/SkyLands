using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Geo.Cuboid;

using Game.World;
using Game.World.Blocks;

using Mogre;


namespace Game.CharacSystem {

    class AStar {
        private Island mIsland;
        public Queue<Vector3> mPath;

        public AStar(Island curr) {
            this.mIsland = curr;
            this.mPath = new Queue<Vector3>();
        }

        public void goTo(Vector3 begin, Vector3 end) {

            mPath.Enqueue(end);
            /*while (begin != end) {
                if (begin.x < end.x && this.mIsland.getBlock(begin + 1 * Vector3.UNIT_X, false) is AirBlock) {
                    begin += 1 * Vector3.UNIT_X;
                    this.mPath.Enqueue(begin);
                    continue;
                } else if (begin.x > end.x && this.mIsland.getBlock(begin - 1 * Vector3.UNIT_X, false) is AirBlock) {
                    begin -= 1 * Vector3.UNIT_X;
                    this.mPath.Enqueue(begin);
                    continue;
                } else if (begin.z < end.z && this.mIsland.getBlock(begin + 1 * Vector3.UNIT_Z, false) is AirBlock) {
                    begin += 1 * Vector3.UNIT_Z;
                    this.mPath.Enqueue(begin);
                    continue;
                } else if (begin.z > end.z && this.mIsland.getBlock(begin - 1 * Vector3.UNIT_Z, false) is AirBlock) {
                    begin -= 1 * Vector3.UNIT_Z;
                    this.mPath.Enqueue(begin);
                    continue;
                }
                break;
            }
            /*
            Block[]   adjacentBlocks;
            Vector3[] adjacentPos;

            while(begin != end) {

                adjacentPos = new Vector3[] {
                    Vector3.UNIT_X,
                    -Vector3.UNIT_X,
                    Vector3.UNIT_Y,
                    -Vector3.UNIT_Y,
                };
                adjacentBlocks = new Block[] {
                    this.mIsland.getBlock(begin + Vector3.UNIT_X, false),
                    this.mIsland.getBlock(begin - Vector3.UNIT_X, false),
                    this.mIsland.getBlock(begin + Vector3.UNIT_Y, false),
                    this.mIsland.getBlock(begin - Vector3.UNIT_Y, false),
                };
                
                short min = 0;

                for (short i = 1; i < 4; i++)
                {
                    if (!(adjacentBlocks[i] is AirBlock)) { continue; }
                    if ((adjacentPos[min] - end).Length > (adjacentPos[i] - end).Length)
                    {
                        min = i;
                    }

                }

                if (!(adjacentBlocks[min] is AirBlock)) { this.mPath = new Queue<Vector3>(); return; }
                else { this.mPath.Enqueue(adjacentPos[min]); }

                begin = adjacentPos[min];
            }*/
        }
    }
}

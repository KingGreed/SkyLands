using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Geo.Cuboid;

using Mogre;


namespace Game.src.Main.Characters
{

    struct Cell {

        public Vector3 loc;
        public int     movementCost;
        public int     distanceToPoint;

        public Cell(Vector3 loc, int movementCost, int distanceToPoint) {
            this.loc             = loc;
            this.movementCost    = movementCost;
            this.distanceToPoint = distanceToPoint
        }

        public int getHeuristicCost() { return this.movementCost + this.distanceToPoint; }
    }

    class AStar
    {
        private Island mIsland;
        private int    mHeight;
        List<Vector3>  mPath;

        public AStar(Island curr, int y) {
            this.mIsland = curr;
            this.mHeight = y;
            this.mPath = new List<cell>();
        }

        public void goTo(Vector3 begin, Vector3 end) {

        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using API.Geo.Cuboid;

using Game.World.Blocks;

using Math = System.Math;

namespace Game.Characters.IA {
    class Node {
        public Vector3 pos;
        public int cost;
        public Node parent;

        public Node(Vector3 destination, Vector3 pos, Node parent = null) { this.pos = pos; this.setCost(destination); this.parent = parent; }

        private void setCost(Vector3 destination) {
            this.cost = (int) (Math.Abs(this.pos.x - destination.x) + Math.Abs(this.pos.z - destination.z));
        }

        public List<Node> getAdjacent(Island current, Vector3 destination) {
            Vector3[] adjacent = new Vector3[] {
                Vector3.UNIT_X, Vector3.UNIT_Z,
                Vector3.NEGATIVE_UNIT_X, Vector3.NEGATIVE_UNIT_Z,
                Vector3.UNIT_X + Vector3.UNIT_Z, Vector3.UNIT_X + Vector3.NEGATIVE_UNIT_Z,
                Vector3.NEGATIVE_UNIT_X + Vector3.UNIT_Z, Vector3.NEGATIVE_UNIT_X + Vector3.NEGATIVE_UNIT_Z,
            };

            List<Node> returnList = new List<Node>();

            Vector3 adj = Vector3.ZERO;
            
                for(int i = 0; i < adjacent.Length; i++) {
                    adj = adjacent[i] + this.pos;
                    if(current.getBlock(adj, false) is AirBlock && !(current.getBlock(adj + Vector3.NEGATIVE_UNIT_Y, false) is AirBlock)) {
                        returnList.Add(new Node(destination, adj, this));
                    } else if(current.getBlock(adj + Vector3.UNIT_Y, false) is AirBlock && !(current.getBlock(adj, false) is AirBlock)) {
                        returnList.Add(new Node(destination, adj + Vector3.UNIT_Y, this));
                    } else if(current.getBlock(adj - Vector3.UNIT_Y, false) is AirBlock && !(current.getBlock(adj - 2 * Vector3.UNIT_Y, false) is AirBlock)) {
                        returnList.Add(new Node(destination, adj - Vector3.UNIT_Y, this));
                    }
                }
            //}
            return returnList;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using API.Geo.Cuboid;

using Game.Characters.IA.Misc;

namespace Game.Characters.IA {
    class PathFinder {
        private AstarLinkedList<Vector3> mGoal;

        public AstarLinkedList<Vector3> Goal { get { return this.mGoal; } }

        public PathFinder(Vector3 destination, Vector3 start, Island current) {
            this.mGoal = this.findPathTo(destination, start, current);
        }


        public AstarLinkedList<Vector3> findPathTo(Vector3 destination, Vector3 start, Island current) {
            List<Node> returnList = new List<Node>();

            NodeList<Node> open   = new NodeList<Node>();
            NodeList<Node> closed = new NodeList<Node>();
            List<Node> possibleNodes = new List<Node>();
            List<Node> adjacentNodes;

            start.y -= 1;

            Node curr;

            open.Add(new Node(destination, start));

            while(open.Count > 0) {
                curr = open[0];
                open.RemoveAt(0); closed.Add(curr);

                if(curr.pos == destination) {
                    AstarLinkedList<Vector3> solution = new AstarLinkedList<Vector3>();
                    while(curr.parent != null) {
                        curr.pos.y += 1;
                        solution.AddFirst(curr.pos);
                        curr = curr.parent;
                    }
                    return solution;
                }

                adjacentNodes = curr.getAdjacent(current, destination);
                possibleNodes.AddRange(adjacentNodes);

                for(int i = 0; i < possibleNodes.Count; i++) {
                    if(!closed.Contains(possibleNodes[i])) {
                        if(open.Contains(possibleNodes[i])) {
                            if(possibleNodes[i].cost < open[possibleNodes[i]].cost) {
                                open[possibleNodes[i]].parent = curr;
                            }
                        } else { open.DichotomicInsertion(possibleNodes[i]); }
                    }
                }
            }
            return null;
        }
    }
}

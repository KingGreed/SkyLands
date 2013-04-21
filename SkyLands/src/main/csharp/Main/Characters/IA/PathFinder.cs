using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

using API.Geo.Cuboid;

using Game.Characters.IA.Misc;

namespace Game.Characters.IA {
    class PathFinder {
        public AstarLinkedList<Vector3> goal;

        public PathFinder(Vector3 destination, Vector3 start, Island current) {
            this.goal = this.findPathTo(destination, start, current);
        }


        public AstarLinkedList<Vector3> findPathTo(Vector3 destination, Vector3 start, Island current) {
            List<Node> returnList = new List<Node>();

            NodeList<Node> open   = new NodeList<Node>();
            NodeList<Node> closed = new NodeList<Node>();
            List<Node> possibleNodes = new List<Node>();
            List<Node> adjacentNodes;

            destination.y = start.y;

            Node curr;

            open.Add(new Node(destination, start));

            while(open.Count > 0) {
                curr = open[0];
                open.RemoveAt(0); closed.Add(curr);

                if(curr.pos == destination) {
                    AstarLinkedList<Vector3> solution = new AstarLinkedList<Vector3>();
                    while(curr.parent != null) {
                        solution.AddFirst(curr.pos);
                        curr = curr.parent;
                    }
                    return solution;
                }

                adjacentNodes = curr.getAdjacent(current, destination);

                // on ajoute cette liste a notre variable static qui contient l'ensemble des listes adjacentes (gestion de l'affichage)
                possibleNodes.AddRange(adjacentNodes);


                /***************************************/
                /* Ajout des noeuds adjacents candidat */
                /***************************************/
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

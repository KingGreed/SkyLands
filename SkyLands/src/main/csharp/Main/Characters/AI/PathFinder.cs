using System.Collections.Generic;
using System.Linq;
using Mogre;

using API.Geo.Cuboid;
using API.Generic;

using Game.World;

namespace Game.CharacSystem.AI
{
    class PathFinder
    {
        private const int MAX_SOLUTION_SIZE = 100;
        private const int NB_ITER_PER_UPDATE = 5, MAX_NB_ITER = 500;

        private readonly Island         mCurrent;
        private readonly NodeList<Node> mOpen;
        private readonly NodeList<Node> mClosed;
        private readonly List<Node>     mPossibleNodes;
        private readonly Vector3        mAbsDestination, mRelDestination;
        private int                     mSolutionSize, mNbTotIter;

        public bool HaveGiveUp { get; private set; }

        public PathFinder(Vector3 destination, Vector3 start, Island current)   // dest in abs value, start rel
        {
            this.mOpen = new NodeList<Node>();
            this.mClosed = new NodeList<Node>();
            this.mPossibleNodes = new List<Node>();
            this.mSolutionSize = 0;
            this.mNbTotIter = 0;
            this.mAbsDestination = destination;
            this.mRelDestination = MainWorld.getRelativeFromAbsolute(this.mAbsDestination);
            this.mCurrent = current;

            this.mOpen.Add(new Node(this.mRelDestination, start));
        }

        public Stack<Vector3> ContinuePathFinding()
        {
            int i = 0;

            while (++i < NB_ITER_PER_UPDATE && this.mOpen.Count > 0)
            {
                this.mNbTotIter++;
                Node curr = this.mOpen[0];
                this.mOpen.RemoveAt(0);
                this.mClosed.Add(curr);

                this.HaveGiveUp = this.mNbTotIter >= MAX_NB_ITER || this.mSolutionSize >= MAX_SOLUTION_SIZE;
                if (curr.pos == this.mRelDestination || this.HaveGiveUp)
                {
                    Stack<Vector3> path = new Stack<Vector3>();
                    path.Push(this.mAbsDestination); // Push the exact destination for the first one
                    while (curr != null)
                    {
                        path.Push(Cst.CUBE_SIDE * (curr.pos + new Vector3(0.5f, 0, -0.5f)));  // Push the center of the block
                        curr = curr.parent;
                    }
                    return path;
                }

                this.mPossibleNodes.AddRange(curr.getAdjacent(this.mCurrent, this.mRelDestination));

                foreach (Node possibleNode in this.mPossibleNodes.Where(node => !this.mClosed.Contains(node)))
                {
                    if (this.mOpen.Contains(possibleNode))
                    {
                        if (possibleNode.cost < this.mOpen[possibleNode].cost)
                        {
                            this.mOpen[possibleNode].parent = curr;
                            this.mSolutionSize++;
                        }
                    }
                    else
                        this.mOpen.DichotomicInsertion(possibleNode);
                }
            }

            return new Stack<Vector3>();
        }
    }
}

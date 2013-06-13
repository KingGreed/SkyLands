using System.Collections.Generic;

namespace Game.CharacSystem.AI
{
    class NodeList<T> : List<T> where T : Node // truc hyper cheaté qui permet de dire que la NodeList dispose de toutes les méthodes de List
                                               // et que le type générique T est un node
    {
        public new bool Contains(T node) { return this[node] != null; }
        public T this[T node] {
            get {
                for(int i = 0; i < this.Count; i++) { if(this[i].pos == node.pos) { return this[i]; } }
                return default(T);
            }
        }
        public void DichotomicInsertion(T node) {
            int left = 0, center = 0, right = this.Count - 1;

            while (left <= right) {
                center = (left + right) / 2;
                if(node.cost < this[center].cost) { right = center - 1; }
                else if(node.cost > this[center].cost) { left = center + 1; }
                else { left = center; break; }
            }
            this.Insert(left, node);
        }
    }
}
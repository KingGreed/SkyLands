using Mogre;

namespace Game.CharacSystem.AI
{
    class AstarLinkedList<T> {
        private AstarNode<T> head = null;
        private int size = 0;

        public int Size          { get { return this.size; } }
        public AstarNode<T> Head { get { return this.head; } }

        public void AddFirst(T node) {
            this.size++;
            AstarNode<T> temp = this.head;
            this.head = new AstarNode<T>(node);
            this.head.setNextNode(temp);
        }

        public bool IsExist(T node) {
            AstarNode<T> temp = this.head;
            do {
                if(temp.Data.Equals(node)) { return true; }
            } while((temp = temp.NextNode) != null);
            return false;
        }

        public void AddEnd(T node) {
            this.size++;
            AstarNode<T> temp = this.head;
            while((temp = temp.NextNode) != null);
            temp.setNextNode(new AstarNode<T>(node));
        }

        public void RemoveFirst() { this.head = this.head.NextNode; this.size--; }

        public void Remove(T node) {
            AstarNode<T> temp = this.head;
            if(this.Head.Data.Equals(node)) { this.Head.setNextNode(this.Head.NextNode); }
            while(temp.NextNode != null && !temp.NextNode.Data.Equals(node)) { temp = temp.NextNode; }
            if(temp.NextNode != null) { temp.setNextNode(temp.NextNode.NextNode); this.size--; }
        }

        public void Clear() { this.head = null; }

        public void DisplayNodes() {
            LogManager.Singleton.DefaultLog.LogMessage("This list contains : " + this.size);
            AstarNode<T> temp = this.head;
            while((temp = temp.NextNode) != null) { LogManager.Singleton.DefaultLog.LogMessage(temp.Data.ToString()); }
        }
    }
}

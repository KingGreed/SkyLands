using Mogre;

namespace API.Geo.Cuboid
{

    public abstract class Block {
        public abstract string getName    ();
        public abstract string getMaterial();
        public abstract byte   getId();
        public abstract int    getMeshType();

        public abstract string getFace(int i);

        public abstract void onCreation(Vector3 position);
        public abstract void onDeletion();

        public abstract bool onRightClick();
        public abstract bool onLeftClick();

        public abstract void onBlockEnter(API.Ent.Entity e, Vector3 position);
        public abstract void onBlockLeave(API.Ent.Entity e, Vector3 position);




    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Ent;
using API.Geo;
using API.Generic;

using Mogre;

using Material = API.Generic.Material;

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

        public abstract void onRightClick();
        public abstract void onLeftClick();



    }
}
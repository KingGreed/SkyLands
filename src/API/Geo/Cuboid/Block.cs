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

    public interface Block {
        string getName    ();
        string getMaterial();
        byte   getId      ();

        string      getFace (int i);


        void onCreation();
        void onDeletion();

        void onRightClick();
        void onLeftClick ();



    }
}
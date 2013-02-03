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

        bool hasVisibleFaceAt(BlockFace face);
        void setVisibleFaceAt(BlockFace face, bool val);

        BlockFace[] getFaces();
        string[] getComposingFaces();


    }
}
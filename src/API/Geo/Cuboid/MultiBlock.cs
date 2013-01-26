using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;
using Material = API.Generic.Material;

namespace API.Geo.Cuboid
{
    interface MultiBlock
    {

        /*
         * @param the Multiblock's material
         */
        MultiBlock(Material mat);

        /*
         * @param the block's location
         */
        void addBlock(Vector3 loc);

        List<Vector3>   getBlockList();
        public Material getMaterial ();
    }
}

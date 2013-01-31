using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;
using Material = API.Generic.Material;

namespace API.Geo.Cuboid
{
    public interface MultiBlock
    {


        /*
         * @param the block's location
         */
        void addBlock(Vector3 loc);

        List<Vector3> getBlockList();
        Material      getMaterial ();
        bool          Contains    (Vector3 loc);

        void display(SceneManager sceneMgr, Island currentIsland, World currentWorld);
    }
}

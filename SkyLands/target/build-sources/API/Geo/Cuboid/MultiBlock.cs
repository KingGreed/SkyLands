using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;
using Material = API.Generic.Material;

namespace API.Geo.Cuboid
{
    public interface MultiBlock {
        /*
         * @param the block's location
         */
        void addBlock(Vector3 loc);

        string getMaterial ();

        /*
         * Warning does not remove item from List, only from Scene
         * */
        void display(Island currentIsland, World currentWorld);
        void addMultiToScene();
        void removeFromScene(Vector3 item, int numFaces);


        string getName();
        void   setName(string name);
    }
}

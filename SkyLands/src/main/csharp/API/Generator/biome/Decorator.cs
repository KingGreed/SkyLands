using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Ent;
using API.Geo;
using API.Geo.Cuboid;
using API.Generic;

using Mogre;

namespace API.Generator
{
    public abstract class Decorator : Populator {

        public Vector3 findRandomPoint(Island curr, Random rd, string restriction ="") {
            int x, y, z;

            int t = 0;

            do {
                x = rd.Next() % (int)(curr.getSize().x * 16);
                z = rd.Next() % (int)(curr.getSize().z * 16);
                y = curr.getSurfaceHeight(x, z, restriction);
                t++;
            } while(y == -1 && t <= 10);


            if(y == -1) { return Vector3.ZERO; }

            return new Vector3(x, y, z);
        }
    }
}
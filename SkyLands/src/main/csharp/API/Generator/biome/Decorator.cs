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

        public Vector3 findRandomPoint(Island curr, Random rd) {
            int x, y, z;
            do {
                x = rd.Next() % (int)(curr.getSize().x * 16);
                z = rd.Next() % (int)(curr.getSize().z * 16);
                y = curr.getSurfaceHeight(x, z);
            } while(y == -1);

            return new Vector3(x, y, z);
        }
    }
}
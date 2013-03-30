using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace API.Math
{
    public class Point
    {
        public int x, y, z;

        public Point(int x, int y, int z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Point(float x, float y, float z) {
            this.x = (int) System.Math.Floor(x);
            this.y = (int) System.Math.Floor(y);
            this.z = (int) System.Math.Floor(z);
        }


    }
}

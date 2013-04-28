using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Geo.Cuboid;
using API.Generator;
using API.Generic;

using Mogre;

using Game.Display;
using Game.World.Blocks;
using Material = API.Generic.Material;

namespace Game.World.Generator
{
    public class VanillaMultiVerticalHalfBlock : MultiBlock {

        public static Vector2[] textureCoord =
                new Vector2[] {
                    new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0),
                    new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 1),
                    new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0),
                    new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0),
                    new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 1),
                    new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0)
                };
        //see API.generic.BlockFace
        public static Vector3[] blockPointCoords = 
            new Vector3[] {
                new Vector3(0, CUBE_SIDE, 0),             new Vector3(0, 0, 0),                              new Vector3(CUBE_SIDE, 0, 0),                      new Vector3(CUBE_SIDE, CUBE_SIDE, 0),
                new Vector3(0, 0, -CUBE_SIDE / 2),        new Vector3(0, CUBE_SIDE, -CUBE_SIDE / 2),         new Vector3(CUBE_SIDE, CUBE_SIDE, -CUBE_SIDE / 2), new Vector3(CUBE_SIDE, 0, -CUBE_SIDE / 2),
                new Vector3(CUBE_SIDE, CUBE_SIDE, 0),     new Vector3(CUBE_SIDE, CUBE_SIDE, -CUBE_SIDE / 2), new Vector3(0, CUBE_SIDE, -CUBE_SIDE / 2),         new Vector3(0, CUBE_SIDE, 0),
                new Vector3(0, 0, 0),                     new Vector3(0, 0, -CUBE_SIDE / 2),                 new Vector3(CUBE_SIDE, 0, -CUBE_SIDE / 2),         new Vector3(CUBE_SIDE, 0, 0),
                new Vector3(0, 0, 0),                     new Vector3(0, CUBE_SIDE, 0),                      new Vector3(0, CUBE_SIDE, -CUBE_SIDE / 2),         new Vector3(0, 0, -CUBE_SIDE / 2),
                new Vector3(CUBE_SIDE, 0, 0),             new Vector3(CUBE_SIDE, 0, -CUBE_SIDE / 2),         new Vector3(CUBE_SIDE, CUBE_SIDE, -CUBE_SIDE / 2), new Vector3(CUBE_SIDE, CUBE_SIDE, 0),
            };

        public static Vector3[] normals =
            new Vector3[] {
                new Vector3(0, 0, -1),
                new Vector3(0, 0, 1),
                new Vector3(0, 1, 0),
                new Vector3(0, 1, 0),
                new Vector3(-1, 0, 0),
                new Vector3(-1, 0, 0),
            };

        public override Vector3 getBlockPointsCoord(int face) { return blockPointCoords[face]; }
        public override Vector2 getTextureCoord(int face)     { return textureCoord[face];     }
        public override Vector3 getNormals(int face)          { return normals[face];          }

        public VanillaMultiVerticalHalfBlock(string mat, Island current, API.Geo.World mainWorld) : base(mat, current, mainWorld) { }
    }
}

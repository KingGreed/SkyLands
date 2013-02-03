using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Geo;
using API.Geo.Cuboid;
using API.Ent;
using API.Generic;

using Mogre;

using Entity   = API.Ent.Entity;
using Material = API.Generic.Material; 



namespace Game.World.Blocks
{
    public abstract class VanillaBlock : Block
    {
        protected string mName;
        protected string mMaterial;
        private bool[]  mVisible = new bool[6];

        protected static int CUBE_SIDE = MainWorld.CUBE_SIDE;

        public BlockFace[] CubeFaces = 
            new BlockFace[] {
                BlockFace.frontFace, BlockFace.backFace,
                BlockFace.upperFace, BlockFace.underFace,
                BlockFace.leftFace,  BlockFace.rightFace
            };

        public string getName    () { return this.mName;     }
        public string getMaterial() { return this.mMaterial; }


        public bool hasVisibleFaceAt(BlockFace face)             { return this.mVisible[(int)face]; }
        public void setVisibleFaceAt(BlockFace face, bool val)   { this.mVisible[(int)face] = val;  }

        public virtual string[] getComposingFaces() { return new string[1] { this.mName }; }
        public virtual BlockFace[] getFaces() { return this.CubeFaces; }


    }
}

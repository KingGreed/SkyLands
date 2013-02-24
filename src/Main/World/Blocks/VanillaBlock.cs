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
        protected byte   mId = 255;//Warning do not set 255 as a value

        protected static int CUBE_SIDE = MainWorld.CUBE_SIDE;

        public BlockFace[] CubeFaces = 
            new BlockFace[] {
                BlockFace.frontFace, BlockFace.backFace,
                BlockFace.upperFace, BlockFace.underFace,
                BlockFace.leftFace,  BlockFace.rightFace
            };

        public string getName    () { return this.mName;     }
        public string getMaterial() { return this.mMaterial; }
        public byte   getId      () { return this.mId;       }


        public virtual string[] getComposingFaces() { return new string[1] { this.mName }; }
        public virtual BlockFace[] getFaces() { return this.CubeFaces; }

    }
}

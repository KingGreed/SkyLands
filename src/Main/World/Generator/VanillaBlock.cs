using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Geo;
using API.Geo.Cuboid;
using API.Ent;

using Mogre;

using Entity   = API.Ent.Entity;
using Material = API.Generic.Material; 


namespace Game.World.Generator
{
    class VanillaBlock : Block
    {
        private Vector3 mLoc;
        private Vector3 mchunkLoc;

        private Material mType;

        public VanillaBlock(Vector3 location, Vector3 chunkPosition) {
            this.mLoc      = location;
            this.mchunkLoc = chunkPosition;
        }

        public Vector3  getPosition()                  { return this.mLoc;      }
	    public Vector3  getChunkLocation()             { return this.mchunkLoc; }
	    public Material getMaterial()                  { return this.mType;     }
	    public void     setMaterial(Material material) { this.mType = material; }

        public bool IsAir() { return (mType == Material.AIR); }

    }
}

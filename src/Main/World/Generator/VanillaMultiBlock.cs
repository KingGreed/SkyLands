using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Geo.Cuboid;
using API.Generator;

using Mogre;

using Game.Display;
using Material = API.Generic.Material;


namespace Game.World.Generator
{
    public class VanillaMultiBlock : MultiBlock
    {
        private List<Vector3> mList;
        private Material      mMaterial;

        public VanillaMultiBlock(Material mat) {
            this.mList     = new List<Vector3>();
            this.mMaterial = mat;
        }

        public void addBlock(Vector3 loc) { this.mList.Add(loc); }

        public List<Vector3> getBlockList() { return this.mList; }

        public Material getMaterial() { return this.mMaterial; }
    }
}

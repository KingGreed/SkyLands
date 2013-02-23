using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace API.Generic
{
    public struct PositionFaceAndName {
        public Vector3   position;
        public BlockFace face;
        public string    blockName;

        public PositionFaceAndName(Vector3 loc, BlockFace face, string name) {
            this.position  = loc;
            this.face      = face;
            this.blockName = name;
        }
    }
}

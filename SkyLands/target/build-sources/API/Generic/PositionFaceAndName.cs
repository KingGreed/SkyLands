using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace API.Generic
{
    public class PositionFaceAndStatus {
        public Vector3   position;
        public BlockFace face;
        public bool      status;

        public PositionFaceAndStatus(Vector3 loc, BlockFace face) {
            this.position  = loc;
            this.face      = face;
            this.status    = true;//exists
        }

        public void setStatus(bool val) { this.status = val; }
    }
}

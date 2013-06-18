using System;

namespace Game.World.Blocks {
    public class BlueDoor : VanillaBlock, Air, TransparentBlock {
        public BlueDoor()
        {
            this.mName = "BlueDoor";
            this.mMaterial = "cube/CrystalGate/blue";
            this.mId = 32;
        }

        public override bool onLeftClick() { return false; }
        public override bool onRightClick() { return false; }
    }
}

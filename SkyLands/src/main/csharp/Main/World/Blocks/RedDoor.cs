using System;

namespace Game.World.Blocks {
    public class RedDoor : VanillaBlock, Air, TransparentBlock {
        public RedDoor()
        {
            this.mName = "RedDoor";
            this.mMaterial = "cube/CrystalGate/red";
            this.mId = 31;
        }

        public override bool onLeftClick() { return false; }
        public override bool onRightClick() { return false; }
    }
}

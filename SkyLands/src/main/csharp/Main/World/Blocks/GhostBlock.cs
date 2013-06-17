using System;

namespace Game.World.Blocks {
    class GhostBlock : VanillaBlock, Air, TransparentBlock {
        public GhostBlock()
        {
            this.mName = "GhostBlock";
            this.mMaterial = "cube/CrystalGate/green";
            this.mId = 30;
        }

        public override bool onLeftClick() { return false; }
        public override bool onRightClick() { return false; }
    }
}

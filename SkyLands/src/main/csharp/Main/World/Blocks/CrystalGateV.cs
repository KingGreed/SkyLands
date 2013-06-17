using System;

namespace Game.World.Blocks {
    class CrystalGateV : CrystalGate, Air, TransparentBlock {
        public CrystalGateV() {

            string[] s = new string[]{ "red", "blue" };

            this.mName = "CrystalGate";
            this.mMaterial = "cube/CrystalGate/" + s[new Random().Next(0, s.Length)];
            this.mId = 19;
        }

    }
}

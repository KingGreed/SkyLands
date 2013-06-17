using System;

namespace Game.World.Blocks {
    class CrystalGateH : CrystalGate, Air, TransparentBlock {
        public CrystalGateH() {

            string[] s = new string[]{ "red", "blue" };

            this.mName = "CrystalGate H";
            this.mMaterial = "cube/CrystalGate/" + s[new Random().Next(0, s.Length)];
            this.mId = 15;
            this.mMeshType = 0;
        }

    }
}

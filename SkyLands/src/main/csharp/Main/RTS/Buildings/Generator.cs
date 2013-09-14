using Mogre;

using API.Geo.Cuboid;
using Game.States;

namespace Game.RTS
{
    public class Generator : Building
    {
        public const int sizeX = 2, sizeY = 7, sizeZ = 2;

        public Generator(StateManager stateMgr, Island island, VanillaRTS rts, Vector3 position)
            : base(stateMgr, island, rts, "G", position) { }
        public Generator(StateManager stateMgr, Island island, VanillaRTS rts)
            : base(stateMgr, island, rts, "G") { }
        protected override void Init()
        {
            this.Size = new Vector3(sizeX, sizeY, sizeZ);
            this.mBuilding = new byte[sizeX, sizeY, sizeZ];
            this.mYDiff = 1;
        }

        protected override void Create()
        {
            base.Create();

            const byte smoothScarvedStone = 21; 
            const byte glass = 22;

            for (int y = 0; y < sizeY - 1; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        byte b = glass;
                        if (y <= 1 || y == sizeY - 2) { b = smoothScarvedStone; }
                        this.mBuilding[x, y, z] = b;
                    }
                }
            }
            this.mBuilding[0, sizeY - 1, 0] = this.mColoredBlock;
        }

        protected override void OnBuild()
        {
            base.OnBuild();
            this.RTS.Capacity += 5;
        }
    }
}

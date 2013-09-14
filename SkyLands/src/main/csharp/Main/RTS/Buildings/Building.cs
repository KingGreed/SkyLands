using System.Linq;
using System.Collections.Generic;

using Game.BaseApp;
using Game.World.Display;
using Game.CharacSystem;
using Game.States;
using Game.World.Generator;
using Mogre;

using Game.World.Blocks;
using API.Geo.Cuboid;

namespace Game.RTS {
    public abstract class Building {
        private const string GHOST_BLOCK = "GhostBlock";

        protected StateManager mStateMgr;
        protected Island mIsland;
        protected byte mColoredBlock;
        protected byte[, ,] mBuilding;
        protected int mYDiff, mSymetricFactor = 1;

        public string Selection { get; set; }
        public VanillaRTS RTS { get; private set; }
        public Vector3 Position { get; protected set; }
        public Vector3 Size { get; protected set; }
        public bool Built { get; set; }
        public Dictionary<byte, int> ActualRessources, NeededRessources;
        private Vector3 RealPos { get { return this.Position - Vector3.UNIT_Y * this.mYDiff; } }
        private bool mIsCreated, mIsGhostBuilt;

        protected Building(StateManager stateMgr, Island island, VanillaRTS rts, string selection) : this(stateMgr, island, rts, selection, -Vector3.UNIT_SCALE) { }
        protected Building(StateManager stateMgr, Island island, VanillaRTS rts, string selection, Vector3 position) {
            this.mStateMgr = stateMgr;
            this.mIsland = island;
            this.RTS = rts;
            this.mColoredBlock = (byte)((this.RTS.Faction == Faction.Blue) ? 32 : 31);
            this.NeededRessources = new Dictionary<byte, int>();
            this.Position = position == -Vector3.UNIT_SCALE ? this.mStateMgr.MainState.User.SelectedBlockPos : position;
            this.Selection = selection;
            this.NeededRessources = this.mStateMgr.MainState.BuildingMgr.GetNeededRessources(this.Selection);
            this.Init();
        }

        protected abstract void Init();
        protected virtual void Create() {
            this.mIsCreated = true;
            if (this.IsCharactInBat(this.mStateMgr.MainState.CharacMgr.MainPlayer))
                this.mSymetricFactor = -1;
        }

        public void DrawRemainingRessource() {
            int i = 40;
            foreach (byte b in this.NeededRessources.Keys) {
                GUI.SetBlockAt(i, VanillaChunk.staticBlock[VanillaChunk.byteToString[b]].getItemTexture(),
                    this.NeededRessources[b] - (this.ActualRessources.ContainsKey(b) ? this.ActualRessources[b] : 0));
                i++;
            }
        }

        public bool IsCharactInBat(VanillaCharacter charac) {
            for (int i = 0; i < 2; i++) {
                Vector3 blockPos = charac.BlockPosition + Vector3.UNIT_Y * i;
                if (this.RealPos.x <= blockPos.x && blockPos.x < this.RealPos.x + this.Size.x &&
                   this.RealPos.y <= blockPos.y && blockPos.y < this.RealPos.y + this.Size.y &&
                   this.RealPos.z <= blockPos.z && blockPos.z < this.RealPos.z + this.Size.z)
                    return true;
            }
            return false;
        }

        public void WaitForRessources() {
            this.Create();
            this.BuildGhost();
        }

        private void BuildGhost() {
            if (this.mIsGhostBuilt) { return; }
            this.mIsGhostBuilt = true;
            this.mIsland.stopCleaning();
            for (int x = 0; x < this.Size.x; x++) {
                for (int y = 0; y < this.Size.y; y++) {
                    for (int z = 0; z < this.Size.z; z++) {
                        Vector3 pos = this.RealPos + new Vector3(x, 0, z) * this.mSymetricFactor + Vector3.UNIT_Y * y;
                        if (this.mBuilding[x, y, z] != 0 && this.mIsland.getBlock(pos, false) is Air)
                            this.mIsland.setBlockAt(pos, GHOST_BLOCK, true);
                    }
                }
            }
            this.mIsland.clean();
        }

        public void Build() {

            //RedCreation
            ParticleGenerator.mkParticle(this.mStateMgr.SceneMgr, this.mIsland.mWorld.getDisplayCoords(this.mIsland.getPosition(), this.RealPos) + Vector3.UNIT_Y * 3, "RedCreation");

            if (!this.mIsCreated) { this.Create(); }
            this.mIsland.stopCleaning();
            for (int x = 0; x < this.Size.x; x++) {
                for (int y = 0; y < this.Size.y; y++) {
                    for (int z = 0; z < this.Size.z; z++) {
                        Vector3 pos = this.RealPos + new Vector3(x, 0, z) * this.mSymetricFactor + Vector3.UNIT_Y * y;
                        if (this.mBuilding[x, y, z] == 99)
                            this.mIsland.removeBlock(pos);
                        else if (this.mBuilding[x, y, z] != 0) {
                            string name = VanillaChunk.staticBlock[VanillaChunk.byteToString[this.mBuilding[x, y, z]]].getName();
                            this.mIsland.setBlockAt(pos, name, true);
                        }
                    }
                }
            }
            this.OnBuild();
            this.mIsland.clean();
        }

        protected virtual void OnBuild() {
            this.mStateMgr.MainState.BuildingMgr.ActConsBlockPos = -Vector3.UNIT_SCALE;
            User.RequestBuilderClose = true;
            this.Built = true;
            this.RTS.AddBuilding(this);
        }

        public void OnDrop(int pos, int newAmount) {
            string imgName = GUI.GetImageAt(pos);
            byte b = VanillaChunk.textureToBlock[imgName].getId();
            if (!this.ActualRessources.ContainsKey(b) || !this.NeededRessources.ContainsKey(b)) { return; }
            this.ActualRessources[b] = this.NeededRessources[b] - newAmount;

            if (this.NeededRessources.Keys.Any(key => this.NeededRessources[key] > this.ActualRessources[key])) { return; }
            this.Build();
        }
    }
}
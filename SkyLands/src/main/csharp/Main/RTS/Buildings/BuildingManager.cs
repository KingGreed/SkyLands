using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using API.Geo.Cuboid;
using Game.RTS;
using Game.States;
using Game.World.Blocks;
using Mogre;

namespace Game.csharp.Main.RTS.Buildings
{
    public class BuildingManager
    {
        private readonly StateManager mStateMgr;
        private readonly RTSManager mRTSManager;
        private readonly Island mIsland;
        public Dictionary<Vector3, Building> Buildings { get; set; }

        public Vector3 ActConsBlockPos { get; set; }

        public BuildingManager(StateManager stateMgr, Island island, RTSManager RTSMgr)
        {
            this.mStateMgr = stateMgr;
            this.mIsland = island;
            this.mRTSManager = RTSMgr;
            this.Buildings = new Dictionary<Vector3, Building>();
        }

        public Dictionary<byte, int> GetNeededRessources(string selection)
        {
            Dictionary<byte, int> ressources = new Dictionary<byte, int>();
            switch (selection)
            {
                case "HQ":
                    ressources.Add(2, 5);   // dirt
                    ressources.Add(13, 20); // planks
                    break;

                case "CD":
                    ressources.Add(2, 10); // dirt
                    ressources.Add(3, 10); // stone
                    break;

                case "RF":
                    ressources.Add(3, 20); // stone
                    break;

                case "G":
                    ressources.Add(2, 5);   // dirt
                    ressources.Add(3, 15); // stone
                    break;
            }

            return ressources;
        }

        public void OnBuildingSelection(string building)
        {
            Building b = this.Buildings.FirstOrDefault(keyValPair => keyValPair.Key == this.ActConsBlockPos).Value;
            Dictionary<byte, int> actualRessources = null;
            if (b != null)
            {
                if (b.Built) { return; }
                actualRessources = b.ActualRessources;
            }

            switch (building)
            {
                case "HQ":
                    b = new HeadQuarter(this.mStateMgr, this.mIsland, this.mRTSManager.PlayerRTS);
                    break;

                case "CD":
                    b = new CrystalDrill(this.mStateMgr, this.mIsland, this.mRTSManager.PlayerRTS);
                    break;

                case "RF":
                    b = new RobotFactory(this.mStateMgr, this.mIsland, this.mRTSManager.PlayerRTS);
                    break;

                case "G":
                    b = new Generator(this.mStateMgr, this.mIsland, this.mRTSManager.PlayerRTS);
                    break;
            }

            if (actualRessources == null)
                actualRessources = b.NeededRessources.Keys.ToDictionary(key => key, key => 0);
            b.ActualRessources = actualRessources;
            if(!this.Buildings.ContainsKey(this.ActConsBlockPos))
                this.Buildings.Add(this.ActConsBlockPos, b);
            b.DrawRemainingRessource();
        }

        public void OnDrop(int pos, int newAmount) { this.Buildings[this.ActConsBlockPos].OnDrop(pos, newAmount); }
        public bool HasActualBuilding() { return this.Buildings.ContainsKey(this.ActConsBlockPos); }
        public Building GetActualBuilding() { return this.Buildings[this.ActConsBlockPos]; }
    }
}

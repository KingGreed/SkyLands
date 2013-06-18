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
        private readonly Island mIsland;
        public Dictionary<ConstructionBlock, Building> Buildings { get; set; }
        public bool mAllowCstrBlockToNull { get; set; }

        private ConstructionBlock mActConstBlock;
        public ConstructionBlock ActConstBlock {
            get { return this.mActConstBlock; }
            set
            {
                if (this.mActConstBlock != value && this.mAllowCstrBlockToNull)
                {
                    this.mActConstBlock = value;
                    this.mAllowCstrBlockToNull = false;
                }
            }
        }

        public BuildingManager(StateManager stateMgr, Island island)
        {
            this.mStateMgr = stateMgr;
            this.mIsland = island;
            this.Buildings = new Dictionary<ConstructionBlock, Building>();
            this.mAllowCstrBlockToNull = true;
        }

        public void OnBuildingSelection(string building, Vector3 constructionBlockPos)
        {
            Console.WriteLine("OnBuildingSelection : " + building);
            switch (building)
            {
                case "HQ":
                    this.Buildings[this.ActConstBlock] = new HeadQuarter(this.mStateMgr, this.mIsland, constructionBlockPos, Faction.Blue, building);
                    break;

                case "CD":
                    this.Buildings[this.ActConstBlock] = new CrystalDrill(this.mStateMgr, this.mIsland, constructionBlockPos, Faction.Blue, building);
                    break;

                case "RF":
                    this.Buildings[this.ActConstBlock] = new RobotFactory(this.mStateMgr, this.mIsland, constructionBlockPos, Faction.Blue, building);
                    break;
            }
        }

        public void OnDrop(int pos, int newAmount) { this.Buildings[this.ActConstBlock].OnDrop(pos, newAmount); }
        public bool HasActualBuilding() { return this.ActConstBlock != null && this.Buildings.ContainsKey(this.ActConstBlock); }
        public Building GetActualBuilding() { return this.Buildings[this.ActConstBlock]; }
    }
}

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
        private readonly Dictionary<ConstructionBlock, Building> mBuildings = new Dictionary<ConstructionBlock, Building>();

        public BuildingManager(StateManager stateMgr, Island island)
        {
            this.mStateMgr = stateMgr;
            this.mIsland = island;
            this.mBuildings = new Dictionary<ConstructionBlock, Building>();
        }

        public void OnBuildingSelection(string building, Vector3 constructionBlockPos)
        {
            System.Console.WriteLine("OnBuildingSelection : " + building);
            User.ActConstrBlock.Selection = building;
            switch (building)
            {
                case "HQ":
                    User.ActConstrBlock.Building = new HeadQuarter(this.mStateMgr, this.mIsland, constructionBlockPos);
                    break;

                case "CD":
                    User.ActConstrBlock.Building = new CrystalDrill(this.mStateMgr, this.mIsland, constructionBlockPos);
                    break;

                case "RF":
                    User.ActConstrBlock.Building = new RobotFactory(this.mStateMgr, this.mIsland, constructionBlockPos);
                    break;
            }
        }

        public void AddBuilding(ConstructionBlock constr)
        {
            this.mBuildings.Add(constr, null);
        }
    }
}

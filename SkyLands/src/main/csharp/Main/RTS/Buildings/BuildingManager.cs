using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using API.Geo.Cuboid;
using Game.RTS;
using Game.States;
using Mogre;

namespace Game.csharp.Main.RTS.Buildings
{
    public class BuildingManager
    {
        private StateManager mStateMgr;
        private List<Building> mBuildings = new List<Building>();
        private Island mIsland;

        public BuildingManager(StateManager stateMgr, Island island)
        {
            this.mStateMgr = stateMgr;
            this.mIsland = island;
            this.mBuildings = new List<Building>();
        }


        public void OnBuildingSelected(string building, Vector3 constructionBlockPos)
        {
            User.ActConstrBlock.Selection = building;
            switch (building)
            {
                case "HQ":
                    User.ActConstrBlock.Building = new HeadQuarter(this.mStateMgr, this.mIsland, constructionBlockPos);
                    break;

                case "CD":
                    User.ActConstrBlock.Building = new CrystalDrill(this.mStateMgr, this.mIsland, constructionBlockPos);
                    break;
            }

            this.mBuildings.Add(User.ActConstrBlock.Building);
        }

    }
}

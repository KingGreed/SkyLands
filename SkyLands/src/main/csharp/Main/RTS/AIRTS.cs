using System;
using Game.States;
using Game.World.Blocks;
using Mogre;

namespace Game.RTS
{
    public class AIRTS : VanillaRTS
    {
        private float mTimer;
        private Vector3 mBase;
        private Random mRandom;
        
        public AIRTS(StateManager stateMgr, RTSManager RTSMgr) : base(stateMgr, RTSMgr)
        {
            this.Faction = Faction.Red;
            Vector3 isldSize = this.mStateMgr.MainState.World.getIsland().getSize();
            Vector2[] corner = new Vector2[] { Vector2.ZERO, new Vector2(0, isldSize.z),
                new Vector2(isldSize.x, 0), new Vector2(isldSize.x, isldSize.z) };

            Vector2 coin = (16 * corner[new Random().Next(0, 4)] + 10 * Vector2.UNIT_SCALE) / 1.3f;
            this.mBase = new Vector3(coin.x, 16 * isldSize.y, coin.y);
            this.SetSafeYPoint();
            this.mRandom = new Random();
        }

        private void SetSafeYPoint()
        {
            while (this.mStateMgr.MainState.World.getIsland().getBlock(this.mBase, false) is Air && this.mBase.y > 0)
                this.mBase.y--;
            this.mBase.y++;
        }

        public override void MyUpdate(float frametime)
        {
            this.mTimer += frametime;
            if (this.mTimer < 10 + this.mRandom.Next(-5, 5)) { return; }
            this.mTimer = 0;
            string goal = "";

            if (!this.Buildings.Exists(b => b.Selection == "HQ"))                       { goal = "HQ"; }
            else if (this.mRTSMgr.PlayerRTS.CrystalSpeed > 1.3 * this.CrystalSpeed)     { goal = "CD"; }
            else if (this.mRTSMgr.PlayerRTS.Capacity > 1.3 * this.Capacity)             { goal = "G"; }
            else if (this.mRTSMgr.PlayerRTS.Buildings.FindAll(b => b.Selection == "RF")
                .Count > 1.3 * this.Buildings.FindAll(b => b.Selection == "RF").Count)  { goal = "RF"; }
            else if (this.mRTSMgr.PlayerRTS.AmountUnits > 1.3f * this.AmountUnits)
            {
                RobotFactory fact = this.Buildings.Find(b => b.Selection == "RF") as RobotFactory;
                if (fact != null) { fact.CreateUnit(); }
            }
            else
            {
                string[] possibilities = new string[] { "CD", "G", "RF"};
                goal = possibilities[this.mRandom.Next(0, 3)];
            }
            if (this.mRTSMgr.PlayerRTS.Buildings.Count < this.Buildings.Count + 5) { return; }
            Building building = null;
            switch (goal)
            {
                case "HQ":
                    building = new HeadQuarter(this.mStateMgr, this.mStateMgr.MainState.World.getIsland(), this, this.mBase);
                    break;
                case "CD":
                    building = new CrystalDrill(this.mStateMgr, this.mStateMgr.MainState.World.getIsland(), this, this.mBase);
                    break;
                case "RF":
                    building = new RobotFactory(this.mStateMgr, this.mStateMgr.MainState.World.getIsland(), this, this.mBase);
                    break;
                case "G":
                    building = new Generator(this.mStateMgr, this.mStateMgr.MainState.World.getIsland(), this, this.mBase);
                    break;
            }
            if (building != null)
            {
                building.Build();
                this.Buildings.Add(building);
                this.ReplaceBasePoint();
            }

            /*foreach (VanillaNonPlayer npc in this.mRTSMgr.CharacMgr.GetFactionCharacters(this.Faction))
            {
                
            }*/
        }

        private void ReplaceBasePoint()
        {
            Vector3 newBase = this.mBase;
            const float radius = 8;

            bool ok = false;
            while (!ok)
            {
                int essai = 0;
                while (!ok && essai < 5)
                {
                    ok = this.Buildings.TrueForAll(b => (b.Position - newBase).Length > radius);
                    float angle = new Random().Next(0, 360);
                    newBase = this.mBase + new Vector3((float)System.Math.Cos(angle), 0, (float)System.Math.Sin(angle)) * radius;
                    essai++;
                }
            }
            while (this.mStateMgr.MainState.World.getIsland().getBlock(newBase, false) is Air)
                newBase.y--;
            while (!(this.mStateMgr.MainState.World.getIsland().getBlock(newBase, false) is Air))
                newBase.y++;
            this.mBase = newBase;
        }
    }
}

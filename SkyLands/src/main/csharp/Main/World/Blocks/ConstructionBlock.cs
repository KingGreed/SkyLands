using System.Collections.Generic;

using Game.BaseApp;
using Game.RTS;
using Game.World.Generator;

namespace Game.World.Blocks
{
    public class ConstructionBlock : VanillaBlock
    {
        public string Selection { get; set; }
        public Building Building { get; set; }
        public Dictionary<byte, int> RemainingRessources { get; private set; }

        public Faction Faction { get; set; }

        public ConstructionBlock()
        {
            this.mName = "Construction";
            this.mMaterial = "";
            this.mItemTexture = "constructionBlockSide2.png";
            this.mId = 7;
            this.Selection = "";
            this.RemainingRessources = new Dictionary<byte, int>();
            this.Faction = Faction.Red;
        }

        public override string getFace(int i) {
            switch (i) {
                case 0: return "cube/construction/side2";
                case 1: return "cube/construction/side2";
                case 2: return "cube/construction/top";
                case 3: return "cube/construction/bottom";
                case 4: return "cube/construction/side1";
                case 5: return "cube/construction/side1";
            }
            return "";
        }

        public override bool onLeftClick()
        {
            User.OpenBuilder = true;
            User.ActConstrBlock = this;
            return false;
        }

        public void AddRemainingRessource(byte b, int amount)
        {
            if (!this.RemainingRessources.ContainsKey(b))
                this.RemainingRessources.Add(b, amount);
        }

        public void DrawRemainingRessource()
        {
            if (this.Selection == "") { return; }

            int i = 40;
            foreach (byte b in this.RemainingRessources.Keys)
            {
                GUI.SetBlockAt(i, VanillaChunk.staticBlock[VanillaChunk.byteToString[b]].getItemTexture(), this.RemainingRessources[b]);
                i++;
            }
        }
    }
}

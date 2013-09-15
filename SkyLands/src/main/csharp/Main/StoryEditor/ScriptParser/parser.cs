using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;

using Game.World.Display;
using Game.States;
using Game.CharacSystem;

using Mogre;

namespace Script {
    public class Parser {

        public StateManager st;

        public Parser(StateManager s) {
            this.st = s;
        }

        public string Parse(string command) {

            string[] operands = command.Substring(command.IndexOf(' ') + 1).Split(' ');
            string Operator = command.Substring(0, command.IndexOf(' '));


            switch(Operator) {
                case "mkParticleAt":
                    if(operands.Length != 4) { return "500 missing (or too much) parameter(s)"; }
                    this.mkParticleAt(Convert.ToInt32(operands[0]), Convert.ToInt32(operands[1]), Convert.ToInt32(operands[2]), operands[3]);
                    return "200";

                case "mkDialogue":
                    if(operands.Length != 1) { return "500 missing (or too much) parameter(s)"; }
                    this.mkDialogue(operands[0]);
                    return "200";

                case "mkSound":
                    if(operands.Length != 1) { return "500 missing (or too much) parameter(s)"; }
                    this.mkSound(operands[0]);
                    return "200";

                case "spawnUnit":
                    if(operands.Length != 5) { return "500 missing (or too much) parameter(s)"; }
                    this.spawnUnit(Convert.ToInt32(operands[0]), Convert.ToInt32(operands[1]), Convert.ToInt32(operands[2]), Convert.ToInt32(operands[3]), operands[4]);
                    return "200";

                case "addRessource":
                    if(operands.Length != 2) { return "500 missing (or too much) parameter(s)"; }
                    this.addRessource(Convert.ToInt32(operands[0]), Convert.ToInt32(operands[1]));
                    return "200";

                case "addLife":
                    if(operands.Length != 1) { return "500 missing (or too much) parameter(s)"; }
                    this.addLife(Convert.ToInt32(operands[0]));
                    return "200";


                default: return "404, missing function";
            }
        }

        public void mkParticleAt(int x, int y, int z, string name) {
            ParticleGenerator.mkParticle(st.SceneMgr, new Vector3(x, y, z), name);
        }

        public void mkDialogue(string d) {

        }

        public void mkSound(string s) { new SoundPlayer(s); }

        public void spawnUnit(int x, int y, int z, int faction, string material) {
            st.MainState.World.createAndSpawnEntity(new Vector3(x, y, z), new CharacterInfo("Robot" + Guid.NewGuid(), (Game.RTS.Faction)faction));
        }

        public void addRessource(int faction, int amount) {
            if(faction == 0) {
                
            }
        }

        public void addLife(int amount) {
            st.MainState.CharacMgr.MainPlayer.Heal(amount);
        }

    }
}

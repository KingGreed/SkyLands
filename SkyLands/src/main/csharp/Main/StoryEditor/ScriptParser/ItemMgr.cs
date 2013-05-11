using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mogre;

namespace Game.StoryEditor.ScriptParser {
    class ItemMgr {
        public List<Item> l;
        public string[] files = new string[] { "onLeftClick.event", "onRightClick.event", "onCreation.event",
                                                "onDeletion.event" , "onBleckEnter.event", "onBlockLeave.event" };
        private string scenario;

        public ItemMgr(string scenarioName, bool load = false) {
            this.scenario = scenarioName;
            this.l = new List<Item>();
            if(load) {
                foreach(string file in files) {
                    StreamReader stream;
                    string fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\" +
                            scenarioName + "\\" + file;

                    try   { stream = new StreamReader(fileName); }
                    catch { throw new Exception("Could not read file : " + fileName); }
                    while(stream.BaseStream.Position != stream.BaseStream.Length) {

                        string[] pos = stream.ReadLine().Split(',');
                        int[] coord = new int[pos.Length];
                        for(int i = 0; i < pos.Length; i++) { coord[i] = Convert.ToInt32(pos[i]); }
                        l.Add(new Item(scenario, new Vector3(coord[0], coord[1], coord[2]), file, stream.ReadLine()));
                    }

                }
            }
        }

        /// <param name="evt">refers to the position of the event in the files array of ItemMgr</param>
        public void addEvent(int evt, Vector3 pos, string text) {
            this.l.Add(new Item(this.scenario, pos, this.files[evt], text));
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using Mogre;


namespace Game.StoryEditor.ScriptParser {
    public class Item {
        protected string text;
        protected string fileName;
        protected Vector3 position;


        public Item(string scenarioName, Vector3 position, string evt, string txt) {
            this.fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\" +
                            scenarioName + "\\" + evt;
            this.position = position;
            this.text = txt;
        }

        public void writeToFile() {
            using(TextWriter writer = new StreamWriter(this.fileName, true)) {
                writer.WriteLine(this.position.x + "," + this.position.y + "," + this.position.z);
                writer.WriteLine(this.text);
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.StoryEditor {
    public class StoryEditorInfo {
        public string pathToFile;
        public string name;
        public string scenario;

        public StoryEditorInfo(string path = "", string name = "", string scenar = "") {
            this.pathToFile = path;
            this.name = name;
            this.scenario = scenar;

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.StoryEditor {
    public class StoryEditorInfo {
        public string pathToFile;

        public StoryEditorInfo(string path = "") {
            this.pathToFile = path;
        }
    }
}

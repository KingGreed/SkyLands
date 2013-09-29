using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Mogre;

using API.Generic;

using Game.BaseApp;

namespace Game.GUIs {
    public static class Hud {
        public static readonly Vector2 IMAGE_SIZE = new Vector2(800, 29);
        public static readonly Vector2 WANTED_SIZE = IMAGE_SIZE * Cst.GUI_RATIO;

        public static void Init(string[] buttons, string[] inputs) {
            if (buttons.Length != 2 || inputs.Length != 3) {
                throw new ArgumentException("array size is wrong");
            }

            for (int i = 0; i < 2; i++)
            {
                string fun = "setButton(" + (i + 1) + ", '" + buttons[i] + "')";
                OgreForm.Hud.ExecuteJavascript(fun);
            }

            SetInput(inputs);
        }

        public static void SetInput(string[] input)
        {
            if (!OgreForm.Hud.IsDocumentReady) { return; }
            for (int i = 0; i < 3; i++)
            {
                string fun = "setInput(" + (i + 1) + ", '" + input[i] + "')";
                OgreForm.Hud.ExecuteJavascript(fun);
            }
        }

        public static void Resize(Vector2 newSize) {
            if (OgreForm.Hud.IsDocumentReady)
                OgreForm.Hud.Size = new Size((int)newSize.x, (int)newSize.y);
        }
    
    }
}

using System.Drawing;
using Mogre;

using API.Generic;

using Game.BaseApp;

namespace Game.GUIs
{
    public static class Selector {
        public static readonly Vector2 IMAGE_SIZE = new Vector2(202, 22);
        public static readonly Vector2 WANTED_SIZE = IMAGE_SIZE * Cst.GUI_RATIO;

        private static int pos = 0;

        public static string Material { get; private set; }
        public static bool IsBullet { get; private set; }
        public static int SelectorPos
        {
            get { return pos; }
            set
            {
                if (pos != value)
                {
                    pos = value;
                    OgreForm.SelectBar.ExecuteJavascript("setSelectorPosition(" + pos + ")");
                }
            }
        }

        static Selector()
        {
            IsBullet = true;
        }

        public static void Resize(Vector2 newSize)
        {
            GUI.ResizeJavascript(OgreForm.SelectBar, OgreForm.SelectBar.Size.Width / newSize.x,
                                                     OgreForm.SelectBar.Size.Height / newSize.y);
            OgreForm.SelectBar.Size = new Size((int)newSize.x, (int)newSize.y);
        }
    }
}

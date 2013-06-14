using System.Drawing;

using Game.BaseApp;

using Mogre;

namespace Game.GUIs
{
    public static class Selector {
        public static readonly Size SELECTOR_SIZE = new Size(404, 44);

        static Selector()
        {
            SelectorPos = 0;
            IsBullet = true;
        }

        public static int SelectorPos { get; set; }
        public static string Material { get; set; }
        public static bool IsBullet { get; set; }
    }
}

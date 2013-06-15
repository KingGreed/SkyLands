using API.Generic;

using Mogre;

namespace Game.GUIs
{
    public static class Selector {
        private static readonly Vector2 IMAGE_SIZE = new Vector2(202, 22);
        public static readonly Vector2 WANTED_SIZE = IMAGE_SIZE * Cst.GUI_RATIO;

        static Selector()
        {
            SelectorPos = 0;
            IsBullet = true;
        }

        public static int    SelectorPos { get; set; }
        public static string Material    { get; set; }
        public static bool   IsBullet    { get; set; }
    }
}

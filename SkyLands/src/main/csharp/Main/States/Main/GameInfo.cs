using Mogre;

namespace Game.World
{
    public class GameInfo
    {
        public enum TypeWorld { StoryEditor, Plains, Desert, Hills, Mountain }

        public int       Seed { get; set; }
        public Vector2   Size { get; set; }
        public TypeWorld Type { get; set; }
        public bool      Load { get; private set; }
        public bool      IsInEditorMode { get; set; }

        public GameInfo(bool load = false)
        {
            this.Seed = 42;
            this.Type = TypeWorld.Hills;
            this.Size = new Vector2(11, 11);
            this.Load = load;
            this.IsInEditorMode = false;
        }
    }
}

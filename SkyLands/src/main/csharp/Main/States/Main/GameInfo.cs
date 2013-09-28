using Mogre;

namespace Game.World
{
    public enum TypeWorld { Plains, Desert, Hills, Mountain }

    public class GameInfo {
        public int       Seed { get; set; }
        public Vector2   Size { get; set; }
        public TypeWorld Type { get; set; }
        public bool      Load { get; private set; }
        public bool      IsInEditorMode { get; set; }
        public string    Scenario       { get; set; }


        public GameInfo(bool load = false, bool editorMode = false, string scenario = "")
        {
            this.Seed = 42;
            this.Type = TypeWorld.Hills;
            this.Size = new Vector2(11, 11);
            this.Load = load;
            this.IsInEditorMode = editorMode;
            this.Scenario = scenario;
        }
    }
}

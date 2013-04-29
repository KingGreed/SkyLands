using System;
using Mogre;

namespace Game.World
{
    public class GameInfo
    {
        public enum TypeWorld { Dome = 0, Plains, Desert, Hills, Mountain }

        private int mSeed;
        private Vector2 mSize;
        private TypeWorld mType;
        private bool mLoad;

        public int Seed { get { return this.Seed; } set { this.mSeed = value; } }
        public Vector2 Size { get { return this.mSize; } set { this.mSize = value; } }
        public TypeWorld Type { get { return this.mType; } set { this.mType = value; } }
        public bool      Load { get { return this.mLoad; } }


        public GameInfo(bool load = false)
        {
            this.mSeed = 42;
            this.mType = GameInfo.TypeWorld.Hills;
            this.mSize = new Vector2(11, 11);
            this.mLoad = load;
        }
    }
}

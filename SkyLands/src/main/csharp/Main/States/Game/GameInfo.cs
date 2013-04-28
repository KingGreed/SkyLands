using System;
using Mogre;

namespace Game.World
{
    public class GameInfo
    {
        public enum TypeWorld { Dome, Plains, Desert, Hills, Mountain }

        private int mSeed;
        private Vector2 mSize;
        private TypeWorld mType;

        public int Seed { get { return this.Seed; } set { this.mSeed = value; } }
        public Vector2 Size { get { return this.mSize; } set { this.mSize = value; } }
        public TypeWorld Type { get { return this.mType; } set { this.mType = value; } }

        public GameInfo()
        {
            this.mSeed = 42;
            this.mType = GameInfo.TypeWorld.Hills;
            this.mSize = new Vector2(15, 15);
        }
    }
}

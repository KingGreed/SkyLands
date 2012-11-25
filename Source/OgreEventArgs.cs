using System;

namespace Game
{
    public class OgreEventArgs : EventArgs
    {
        private int mWidth;
        private int mHeight;

        public int Width { get { return mWidth; } }

        public int Height { get { return mHeight; } }

        public OgreEventArgs() : this(0, 0)
        {
        }

        public OgreEventArgs(int width, int height)
        {
            mWidth = width;
            mHeight = height;
        }
    }
}

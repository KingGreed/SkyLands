using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Animation
{
    public enum AnimName : byte { IdleBase, IdleTop, RunBase, RunTop, JumpStart, JumpLoop, JumpEnd, Dance }

    public class Anim
    {
        private AnimName mName;
        private float mTimeFactor;

        public AnimName Name { get { return this.mName; } }

        public Anim(AnimName name, float timeFactor = 1)
        {
            this.mName = name;
            this.mTimeFactor = timeFactor;
        }
    }
}

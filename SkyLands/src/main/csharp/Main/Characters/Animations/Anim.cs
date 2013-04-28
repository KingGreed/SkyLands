using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Animation
{
    public class Anim
    {
        private string mName;
        private float mTimeFactor;
        private bool mKeepActive;

        public string Name { get { return this.mName; } }
        public float TimeFactor { get { return this.mTimeFactor; } }
        public bool KeepActive { get { return this.mKeepActive; } }

        public Anim(string name, float timeFactor = 1, bool keepActive = false)
        {
            this.mName = name;
            this.mTimeFactor = timeFactor;
            this.mKeepActive = keepActive;
        }
    }
}

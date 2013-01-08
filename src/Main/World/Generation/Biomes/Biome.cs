using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;
using Game.Terrain;
using Game.Material;

namespace Game.Generation.Biomes
{
    public abstract class Biome {
        private int mId;

        protected float mMin;
	    protected float mMax;

        protected List<Materials> mGroundCover;

        public Biome(int id) {
            this.mId = id;
            this.mGroundCover = new List<Materials>();
        }

        protected void setMinMax(float min, float max) {
		    this.mMin = min;
		    this.mMax = max;
	    }

	    public float getMin() { return mMin; }
	    public float getMax() { return mMax; }

        protected void addTopCover(params Materials[] mtrNames){
            foreach(Materials mtrName in mtrNames) { mGroundCover.Add(mtrName); } 
        }



    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Ent;
using API.Geo;
using API.Geo.Cuboid;
using API.Generic;

using Mogre;

namespace API.Generator
{

    /**
     * Defines an abstract biome.
     */
    public abstract class Biome {
	    protected byte id;

        protected double min, max;

	    protected List<Decorator> mDecorators  = new List<Decorator>();
        protected List<string>    mGroundCover = new List<string>();

	    public Biome(byte id, params Decorator[] decorators) {
		    this.mDecorators.AddRange(decorators.OfType<Decorator>().ToList());
            this.id = id;
	    }

	    public void decorate(Island curr, Random random) {
		    foreach(Decorator b in this.mDecorators) {
			    b.populate(curr, random);
		    }
	    }

        protected void setMinMax(double min, double max) {
		    this.min = min;
		    this.max = max;
	    }

        public double getMin() { return this.min; }
        public double getMax() { return this.max; }

        public List<string> getGroundCover() { return this.mGroundCover; }
    }
}
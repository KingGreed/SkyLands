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
	    private int id;
	    private bool registered = false;
	    List<Decorator> decorators = new List<Decorator>();

	    public Biome(params Decorator[] decorators) {
		    this.decorators.AddRange(decorators.OfType<Decorator>().ToList());
	    }

	    public void decorate(Chunk chunk, Random random) {
		    foreach(Decorator b in this.decorators) {
			    b.populate(chunk, random);
		    }
	    }

	    protected void setId(int id) {
		    if (!registered) {
			    this.id = id;
			    registered = true;
		    }
	    }
        
	    public abstract String getName();
    }
}
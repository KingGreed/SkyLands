using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace API.Ent
{
    public interface Character : Entity {
	    /**
	     * Gets the player's name.
	     * @return the player's name
	     */
	    String getName();

	    /**
	     * Gets the player's display name. This method is thread-safe.
	     * @return the player's display name
	     */
	    String getDisplayName();

	    /**
	     * Sets the player's display name. This method is thread-safe.
	     * @param name the player's new display name
	     */
	    void setDisplayName(String name);


	    /**
	     * Teleports the player to the given location and inform's the player's client
	     * 
	     * @param loc the new location
	     */
	    void teleport(Vector3 loc);

	    /**
	     * Immediately saves the players state to disk
	     * @return true if successful
	     */
	    bool save();

        /**
	     * Gets the player's spawn point.
	     * @return the player's spawn point
	     */
        Vector3 getSpawnPoint();
       
        void setSpawnPoint(Vector3 loc);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Geo.Cuboid;

namespace API.Ent
{
    public interface Entity {
	    /**
	     * Gets the current ID of this entity within the current game session
	     * @return The entities' id.
	     */
	    int getId();

	    /**
	     * Removes the entity. This takes effect at the next snapshot.
	     */
	    void remove();

	    /**
	     * True if the entity is removed.
	     * @return removed
	     */
	    bool isRemoved();

	    /**
	     * Returns true if this entity is spawned.
	     * @return spawned
	     */
	    bool isSpawned();

	    /**
	     * Sets whether or not the entity should be saved.<br/>
	     * @param savable True if the entity should be saved, false if not
	     */
	    void setSavable(bool savable);

	    /**
	     * Returns true if this entity should be saved.
	     * @return savable
	     */
	    bool isSavable();

	    /**
	     * Sets the maximum distance at which the entity can be seen.<br/>
	     * <br/>
	     * The actual view distance used by the server may not be exactly the value that is set.<br/>
	     * @param distance in blocks at which the entity can be seen
	     */
	    void setViewDistance(int distance);

	    /**
	     * Gets the maximum distance at which the entity can be seen.<br/>
	     * @return the distance in blocks at which the entity can be seen
	     */
	    int getViewDistance();
	    /**
	     * Gets the {@link Chunk} this entity resides in, or null if removed.
	     * @return chunk the entity is in, or null if removed.
	     */
	    Chunk getChunk();

	    /**
	     * Gets the region the entity is associated and managed with, or null if removed.
	     * @return region the entity is in.
	     */
	    Island getRegion();

	    /**
	     * Interact with this entity.
	     */

        void setIsPushedByArcaneLevitator(bool value);
    }
}

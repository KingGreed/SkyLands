using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Ent;
using Entity = API.Ent.Entity;
using Mogre;

namespace API.Geo
{
    public interface World {

	    /**
	     * Gets the name of the world
	     * @return the name of the world
	     */
	    String getName();

        void   setName(string name);

	    /**
	     * Gets the age of the world in ms. This count cannot be modified, and
	     * increments on every tick
	     * @return the world's age in ms
	     */
	    long getAge();

	    /**
	     * Gets the height of the highest block in the given (x, z) column.<br>
	     * <br>
	     * Blocks which are completely transparent are ignored.
	     * @param x the block x coordinate of the column
	     * @param z the block z coordinate of the column
	     * @return the highest of the highest block
	     */
	    int getSurfaceHeight(int x, int z, Vector3 islandLoc);


	    /**
	     * Create a new Entity for initialization
	     * <p/>
	     * This does not add the Entity to the server. You must call
	     * {@link #spawnEntity(Entity)} to simulate the Entity in the world
	     * @param point The point to spawn the Entity
	     * @param component The component to give the Entity.
	     * @return The created entity
	     */
	    Entity createEntity(Vector3 point, Entity type);

	    /**
	     * Create a new Entity for initialization
	     * <p/>
	     * This does not add the Entity to the server. You must call
	     * {@link #spawnEntity(Entity)} to simulate the Entity in the world
	     * @param point The point to spawn the Entity
	     * @param prefab The entity prefab.
	     * @return The created entity
	     */
	    Entity createAndSpawnEntity(Vector3 point, Entity e);

	    /**
	     * Add a created entity to the world for simulation and syncing to clients
	     * @param e The entity to spawn
	     */
	    void spawnEntity(Entity e);


	    /**
	     * Gets the world's spawn point
	     * @return the spawn point
	     */
	    Vector3 getSpawnPoint();

	    /**
	     * Sets the world's spawn point
	     * @param transform the Transform of the spawn point
	     */
	    void setSpawnPoint(Vector3 transform);

	    /**
	     * Gets the world's seed. This value is immutable and set at world creation
	     * @return the seed
	     */
	    long getSeed();

	    /**
	     * Gets the height of this world in blocks.
	     * @return The height of this world in blocks
	     */
	    int getHeight();

        /**
         * @param absolute coord of the Island
         * @param relative coords in the Island
	     * @return the display coords of an relative position
	     */
	    Vector3 getDisplayCoords(Vector3 island, Vector3 relativeLocation);


	    /**
	     * Gets a set of all players on active on this world
	     * @return all players on this world
	     */
	    List<Character> getPlayers();

	    /**
	     * Gets a list of nearby entities of the point, inside of the range
	     * @param position of the center
	     * @param ignore Entity to ignore
	     * @param range to look for
	     * @return the list of nearby entities (or empty if none)
	     */
	    List<Entity> getNearbyEntities(Vector3 position, Entity ignore, int range);

	    /**
	     * Gets a set of nearby players to the point, inside of the range
	     * @param position of the center
	     * @param range to look for
	     * @return A set of nearby Players
	     */
	    List<Entity> getNearbyEntities(Vector3 position, int range);

	    /**
	     * Gets a set of nearby players to the entity, inside of the range
	     * @param entity marking the center and which is ignored
	     * @param range to look for
	     * @return A set of nearby Players
	     */
	    List<Entity> getNearbyEntities(Entity entity, int range);

	    /**
	     * Gets the absolute closest player from the specified point within a specified range.
	     * @param position to search from
	     * @param ignore to ignore while searching
	     * @param range to search
	     * @return nearest player
	     */
	    Entity getNearestEntity(Vector3 position, Entity ignore, int range);

	    /**
	     * Gets the absolute closest player from the specified point within a specified range.
	     * @param position center of search
	     * @param range to search
	     * @return nearest player
	     */
	    Entity getNearestEntity(Vector3 position, int range);

	    /**
	     * Gets the absolute closest player from the specified point within a specified range.
	     * @param entity to search from
	     * @param range to search
	     * @return nearest player
	     */
	    Entity getNearestEntity(Entity entity, int range);

	    /**
	     * Gets a set of nearby players to the point, inside of the range.
	     * The search will ignore the specified entity.
	     * @param position of the center
	     * @param ignore Entity to ignore
	     * @param range to look for
	     * @return A set of nearby Players
	     */
	    List<Character> getNearbyPlayers(Vector3 position, Character ignore, int range);

	    /**
	     * Gets a set of nearby players to the point, inside of the range
	     * @param position of the center
	     * @param range to look for
	     * @return A set of nearby Players
	     */
	    List<Character> getNearbyPlayers(Vector3 position, int range);

	    /**
	     * Gets a set of nearby players to the entity, inside of the range
	     * @param entity marking the center and which is ignored
	     * @param range to look for
	     * @return A set of nearby Players
	     */
	    List<Character> getNearbyPlayers(Entity entity, int range);

	    /**
	     * Gets the absolute closest player from the specified point within a specified range.
	     * @param position to search from
	     * @param ignore to ignore while searching
	     * @param range to search
	     * @return nearest player
	     */
	    Character getNearestPlayer(Vector3 position, Character ignore, int range);

	    /**
	     * Gets the absolute closest player from the specified point within a specified range.
	     * @param position center of search
	     * @param range to search
	     * @return nearest player
	     */
	    Character getNearestPlayer(Vector3 position, int range);

	    /**
	     * Gets the absolute closest player from the specified point within a specified range.
	     * @param entity to search from
	     * @param range to search
	     * @return nearest player
	     */
	    Character getNearestPlayer(Entity entity, int range);
	    /**
	     * Unloads the world from the server. Undefined behavior will occur
	     * if any players are currently alive on the world while it is being
	     * unloaded.
	     *
	     * @param save
	     */
	    void unload(bool save);


	    /**
	     * Saves all world data to world data file.
	     * <p>
	     * Note: World data does not include chunks, regions, or other data.
	     * World data pertains to world age, world name, and world data maps.
	     * </p>
	     */
	    void save();
    }
}

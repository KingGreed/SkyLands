using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Ent;
using API.Geo;
using API.Generic;

using Mogre;

using Material = API.Generic.Material;

namespace API.Geo.Cuboid
{

    public interface Block {

	    /**
	     * Gets the {@link Point} position of this block in the world
	     * 
	     * @return the position
	     */
	    Vector3 getPosition();

	    /**
	     * Gets the {@link Chunk} this block is in
	     * 
	     * @return the Chunk
	     */
	    Chunk getChunk();

	    /**
	     * Gets the {@link Region} this block is in
	     * 
	     * @return the Region
	     */
	    Island getRegion();

	    /**
	     * Gets the x-coordinate of this block
	     * 
	     * @return the x-coordinate
	     */
	    int getX();

	    /**
	     * Gets the y-coordinate of this block
	     * 
	     * @return the y-coordinate
	     */
	    int getY();

	    /**
	     * Gets the z-coordinate of this block
	     * 
	     * @return the z-coordinate
	     */
	    int getZ();

	    /**
	     * Translates this block using the offset and distance given
	     * 
	     * @param offset BlockFace to translate
	     * @param distance to translate
	     * @return a new Block instance
	     */
	    Block translate(BlockFace offset, int distance);

	    /**
	     * Translates this block using the offset given
	     * 
	     * @param offset BlockFace to translate
	     * @return a new Block instance
	     */
	    Block translate(BlockFace offset);

	    /**
	     * Translates this block using the offset given
	     * 
	     * @param offset Vector to translate
	     * @return a new Block instance
	     */
	    Block translate(Vector3 offset);


	    Material getMaterial();



	    /**
	     * Sets the material of this block
	     *
	     * @param material to set to
	     * @return whether the material set was successful
	     */
	    bool setMaterial(Material material);


	    /**
	     * Sets the block light level to the given light level<br><br>
	     * <b>Note: For persistence, alter block material light levels instead</b>
	     *
	     * @param light level to set to
	     * @return this Block
	     */
	    Block setBlockLight(byte level);

	    /**
	     * Gets the block light level
	     *
	     * @return the block light level
	     */
	    byte getBlockLight();
    }
}
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
         * Note that this is the chunks Position relativly to the Island
         * 
	     * @return the Chunk
	     */
	    Vector3 getChunkLocation();

        /**
	     * Gets the {@link Material} this block has
	     * 
	     * @return the Material
	     */
	    Material getMaterial();

	    /**
	     * Sets the material of this block
	     *
	     * @param material to set to
	     * @return whether the material set was successful
	     */
	    void setMaterial(Material material);


        /**
	     * @return whether the material is air or not
	     */
        bool isAir();

        /**
	     * @return whether the material is not air or is
	     */
        bool isNotAir();

        /**
	     * @return whether the material is the same as that or not
	     */
        bool hasSameMaterialThan(Material that);

        bool hasVisibleFaceAt(BlockFace face);
        void setVisibleFaceAt(BlockFace face, bool val);
    }
}
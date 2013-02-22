using System;
using System.Collections.Generic;
using System.Threading;

using API.Ent;
using API.Geo;
using API.Geo.Cuboid;
using API.Generator;

using Entity    = API.Ent.Entity;
using Character = API.Ent.Character; 

using Game.World.Generator;
using Game.States;
using Game.Sky;
using Game.World.Blocks;
using Game.World.Generator.Biomes;
using Game.CharacSystem;

using Mogre;


namespace Game.World
{
    public class MainWorld : API.Geo.World
    {
        private string   mName;
        private long     mAge;
        private long     mSeed;
        private Vector3  mSpawnPoint;
        public const int MaxHeight = 256; // must be a multiple of 16
        public const int CHUNK_SIDE = 16;
        public const int CUBE_SIDE = 50;

        private List<Entity> mEntityList;
        private Dictionary<Vector3, Island> mIslandList;

        private StateManager  mStateMgr;
        private SkyMgr        mSkyMgr;
        private Thread        mTerrainThread; 

        public MainWorld(StateManager stateMgr)
        {
            this.mEntityList = new List<Entity>();
            this.mAge        = 0;
            this.mSeed       = 42;

            this.mStateMgr = stateMgr;

            this.mIslandList = new Dictionary<Vector3, Island>();
            SceneNode node = this.mStateMgr.SceneManager.RootSceneNode.CreateChildSceneNode(Vector3.ZERO);
            Island island;
            if (this.mStateMgr.ChosenWorld == StateManager.TypeWorld.Dome)            { island = new DomeIsland(node, new Vector2(3, 3), this); }
            else if (this.mStateMgr.ChosenWorld == StateManager.TypeWorld.Plain)      { island = new RandomIsland(node, new Vector2(13, 13), new Plains(),    this); }
            else if (this.mStateMgr.ChosenWorld == StateManager.TypeWorld.Plain)      { island = new RandomIsland(node, new Vector2(13, 13), new Hills(),     this); }
            else  /*(this.mStateMgr.ChosenWorld == StateManager.TypeWorld.Mountain)*/ { island = new RandomIsland(node, new Vector2(6, 6),   new Mountains(), this); }
            this.mIslandList.Add(Vector3.ZERO, island);
            this.mIslandList[Vector3.ZERO].display();

            this.mSkyMgr = new SkyMgr(this.mStateMgr);

            this.setSafeSpawnPoint(Vector3.ZERO);
        }


        //get

        public string  getName()       { return this.mName;       }
        public long    getAge()        { return this.mAge;        }
        public long    getSeed()       { return this.mSeed;       }
        public Vector3 getSpawnPoint() { return this.mSpawnPoint; }
        public int     getHeight()     { return MaxHeight;        }
        
        public SceneManager getSceneMgr()        { return this.mStateMgr.SceneManager; }
        public Island getIslandAt(Vector3 loc)   { return this.mIslandList[loc]; }
        
        public int     getSurfaceHeight(int x, int z, Vector3 islandLoc)                       { return this.mIslandList[islandLoc].getSurfaceHeight(x, z); }
        public Vector3 getDisplayCoords(Vector3 island, Vector3 relativeLocation)              { return (relativeLocation * CUBE_SIDE) + island; }

	    public List<Entity> getNearbyEntities(Vector3 position, Entity ignore, int range)      { throw new NotImplementedException(); }
	    public List<Entity> getNearbyEntities(Vector3 position, int range)                     { throw new NotImplementedException(); }
	    public List<Entity> getNearbyEntities(Entity entity, int range)                        { throw new NotImplementedException(); }

        public Entity getNearestEntity(Vector3 position, Entity ignore, int range)             { throw new NotImplementedException(); }
	    public Entity getNearestEntity(Vector3 position, int range)                            { throw new NotImplementedException(); }
	    public Entity getNearestEntity(Entity entity, int range)                               { throw new NotImplementedException(); }
	    
        
        public List<Character> getNearbyPlayers(Vector3 position, Character ignore, int range) { throw new NotImplementedException(); }
	    public List<Character> getNearbyPlayers(Vector3 position, int range)                   { throw new NotImplementedException(); }
	    public List<Character> getNearbyPlayers(Entity entity, int range)                      { throw new NotImplementedException(); }
	    
        public Character getNearestPlayer(Vector3 position, Character ignore, int range)       { throw new NotImplementedException(); }
	    public Character getNearestPlayer(Vector3 position, int range)                         { throw new NotImplementedException(); }
	    public Character getNearestPlayer(Entity entity, int range)                            { throw new NotImplementedException(); }
        

        public void setName(string name) { this.mName =  name;}
        public void setSafeSpawnPoint(Vector3 islandLoc)  {
            Random random = new Random();
            int x, y, z;
            while(true) {
                x = random.Next((int)this.mIslandList[islandLoc].getSize().x * CHUNK_SIDE);
                z = random.Next((int)this.mIslandList[islandLoc].getSize().z * CHUNK_SIDE);

                y = this.getSurfaceHeight(x, z, islandLoc);
                if(y != -1) {
                    LogManager.Singleton.DefaultLog.LogMessage("\n \n New SpawnPoint at : " + new Vector3(x * CUBE_SIDE, y * CUBE_SIDE, z * CUBE_SIDE).ToString());
                    this.mSpawnPoint = islandLoc + new Vector3(x * CUBE_SIDE, y * CUBE_SIDE, z * CUBE_SIDE);
                    break;
                }
            }
        }
	    public Entity createEntity(Vector3 point, Entity type) { throw new NotImplementedException(); }
	    public Entity createAndSpawnEntity(Vector3 point, Entity e) { throw new NotImplementedException(); }
	    public void spawnEntity(Entity e) { throw new NotImplementedException(); }
	    public void setSpawnPoint(Vector3 transform) { throw new NotImplementedException(); }
	    public List<Character> getPlayers() { throw new NotImplementedException(); }

	    public void unload(bool save) { throw new NotImplementedException(); }
	    public void save() { throw new NotImplementedException(); }

        public bool HasCharacCollision(Vector3[] hitBlocks, Vector3 islandLoc, CubeFace collisionSide)  // hitBlocks should be of size 4
        {
            int[] indexToTest;
            if      (collisionSide == CubeFace.underFace)   { indexToTest = new int[] { 0, 1, 2, 3 }; }
            else if (collisionSide == CubeFace.upperFace)   { indexToTest = new int[] { 4, 5, 6, 7 }; }
            else if (collisionSide == CubeFace.leftFace)    { indexToTest = new int[] { 0, 3, 4, 7 }; }
            else if (collisionSide == CubeFace.rightFace)   { indexToTest = new int[] { 1, 2, 5, 6 }; }
            else if (collisionSide == CubeFace.backFace)    { indexToTest = new int[] { 2, 3, 6, 7 }; }
            else  /*(collisionSide == CubeFace.frontFace)*/ { indexToTest = new int[] { 0, 1, 4, 5 }; }

            int nbCollision = 0;
            foreach (Vector3 blockPos in hitBlocks)
            {
                if (this.hasBlockCollision(blockPos, islandLoc))
                {
                    if (nbCollision >= 3) { return true; }
                    else                  { nbCollision++; }
                }
            }

            return false;
        }

        private bool hasBlockCollision(Vector3 blockPos, Vector3 islandLoc)
        {
            blockPos += this.mIslandList[islandLoc].getPosition();
            blockPos /= CUBE_SIDE;

            blockPos.x = Mogre.Math.IFloor(blockPos.x);
            blockPos.y = Mogre.Math.IFloor(blockPos.y);
            blockPos.z = Mogre.Math.IFloor(blockPos.z);

            Block block = this.mIslandList[islandLoc].getBlock(blockPos, false);
            return !(block == null || block is AirBlock);
        }

        public Vector3 GetBlockAbsPosFromAbs(VanillaCharacter charac)             { return this.GetBlockAbsPosFromAbs(charac.FeetPosition, charac.Info.IslandLoc); }
        public Vector3 GetBlockAbsPosFromAbs(Vector3 absCoord, Vector3 islandLoc) {
            Vector3 blockPos, chunkPos;
            this.mIslandList[islandLoc].getBlockCoord(absCoord / CUBE_SIDE, out blockPos, out chunkPos);
            if (blockPos == -Vector3.UNIT_SCALE || chunkPos == -Vector3.UNIT_SCALE) { return -Vector3.UNIT_SCALE;}
            else { return (chunkPos * CHUNK_SIDE + blockPos) * CUBE_SIDE; }
        }

        public void Update(float frameTime) {
            this.mSkyMgr.Update();

            foreach (KeyValuePair<Vector3, Island> pair in this.mIslandList) {
                if(pair.Value.getNumOfBlocksAddedAndRemoved() > 20) {

                }
            }
        }

        public void Shutdown() {
            this.mSkyMgr.Shutdown();
            this.mStateMgr.SceneManager.ClearScene();
        }
    }
}
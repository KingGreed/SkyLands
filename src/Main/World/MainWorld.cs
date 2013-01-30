using System;
using System.Collections.Generic;

using API.Ent;
using API.Geo;
using API.Geo.Cuboid;
using API.Generator;

using Entity    = API.Ent.Entity;
using Character = API.Ent.Character; 

using Game.World.Generator;
using Game.States;
using Game.Sky;
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
        
        private SkyMgr mSkyMgr;

        public MainWorld(StateManager stateMgr)
        {
            this.mEntityList = new List<Entity>();
            this.mAge        = 0;
            this.mSeed       = 42;

            this.mIslandList = new Dictionary<Vector3, Island>();
            if      (stateMgr.ChosenWorld == StateManager.TypeWorld.Sinus)      { this.mIslandList.Add(new Vector3(0, 0, 0), new SinusIsland (new Vector3(0, 0, 0), new Vector2(3, 3))); }
            else if (stateMgr.ChosenWorld == StateManager.TypeWorld.Dome)       { this.mIslandList.Add(new Vector3(0, 0, 0), new DomeIsland  (new Vector3(0, 0, 0), new Vector2(6, 6))); }
            else if (stateMgr.ChosenWorld == StateManager.TypeWorld.Plain)      { this.mIslandList.Add(new Vector3(0, 0, 0), new RandomIsland(new Vector3(0, 0, 0), new Vector2(6, 6), new Vector2(90, 53))); }
            else  /*(stateMgr.ChosenWorld == StateManager.TypeWorld.Mountain)*/ { this.mIslandList.Add(new Vector3(0, 0, 0), new RandomIsland(new Vector3(0, 0, 0), new Vector2(6, 6), new Vector2(32.5f, 256))); }
            this.mIslandList[new Vector3(0, 0, 0)].display(stateMgr.SceneManager);

            this.mSpawnPoint = Vector3.ZERO;
            this.setSafeSpawnPoint(new Vector3(0, 0, 0));
            this.mSkyMgr = new SkyMgr(stateMgr); LogManager.Singleton.DefaultLog.LogMessage("Sky Created");
        }


        //get

        public string  getName()       { return this.mName;       }
        public long    getAge()        { return this.mAge;        }
        public long    getSeed()       { return this.mSeed;       }
        public Vector3 getSpawnPoint() { return this.mSpawnPoint; }
        public int     getHeight()     { return MaxHeight;        }
        
        public int getSurfaceHeight(int x, int z, Vector3 islandLoc) {
            return this.mIslandList[islandLoc].getSurfaceHeight(x, z);
        }

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

        public bool HasCharacCollision(Vector3[] hitBlocks, CubeFace collisionSide)
        {
            List<Vector3> coordsToTest = new List<Vector3>();
            if (collisionSide != CubeFace.upperFace) { coordsToTest.Add(hitBlocks[0]); }
            if (collisionSide != CubeFace.underFace) { coordsToTest.Add(hitBlocks[1]); }

            foreach (Vector3 blockPos in coordsToTest)
                if (this.hasBlockCollision(blockPos, collisionSide)) 
                    return true;

            return false;
        }

        private bool hasBlockCollision(Vector3 absBlockPos, CubeFace collisionSide)
        {
            absBlockPos += this.mIslandList[new Vector3(0, 0, 0)].getPosition();

            if      (collisionSide == CubeFace.rightFace)  { absBlockPos.x++; }
            else if (collisionSide == CubeFace.leftFace)   { absBlockPos.x--; }
            else if (collisionSide == CubeFace.upperFace)  { absBlockPos.y++; }
            else if (collisionSide == CubeFace.underFace)  { absBlockPos.y--; }
            else if (collisionSide == CubeFace.frontFace)  { absBlockPos.z++; }
            else  /*(collisionSide == CubeFace.backFace)*/ { absBlockPos.z--; }

            Block block = this.mIslandList[new Vector3(0, 0, 0)].getBlock(absBlockPos);
            return !(block == null || block.IsAir());
        }

        public Vector3 GetBlockAbsPosFromAbs(Vector3 absCoord)
        {
            Block block = this.mIslandList[new Vector3(0, 0, 0)].getBlock(absCoord / CUBE_SIDE);
            if (block != null) { return block.getPosition() * CUBE_SIDE; } // Returns the down back left corner
            else               { return -Vector3.UNIT_SCALE; }
        }

        public void Update()   { this.mSkyMgr.Update(); }
        public void Shutdown() { this.mSkyMgr.Shutdown(); }
    }
}

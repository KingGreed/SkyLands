using System;
using System.Collections.Generic;

using API.Ent;
using API.Geo;
using API.Geo.Cuboid;
using API.Generator;

using Entity    = API.Ent.Entity;
using Character = API.Ent.Character; 

using Game.CharacSystem;
using Game.States;
using Game.World.Generator;
using Game.Sky;

using Mogre;


namespace Game.World
{
    public partial class MainWorld : State, API.Geo.World
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
        private CharacMgr mCharacMgr;
        private DebugMode mDebugMode;

        public MainWorld(StateManager stateMgr) : base(stateMgr) {
            
            this.mEntityList = new List<Entity>();
            this.mAge        = 0;
            this.mSeed       = 42;

            this.mSpawnPoint = Vector3.ZERO;
            this.mIslandList = new Dictionary<Vector3, Island>();

            this.mSkyMgr = new SkyMgr(this.mStateMgr);

        }

        public override bool Startup(){
            if (this.mIsStartedUp) { return false; }

            this.mIsStartedUp = true;

            this.mIslandList.Add(new Vector3(0, 0, 0), new DomeIsland(new Vector3(0, 0, 0), new Vector2(4, 4)));
            this.mIslandList[new Vector3(0, 0, 0)].display(this.mStateMgr.SceneManager);

            this.setSafeSpawnPoint(new Vector3(0, 0, 0));
            this.mSkyMgr.CreateSky(); LogManager.Singleton.DefaultLog.LogMessage("Sky Created");

            this.mCharacMgr = new CharacMgr(this.mStateMgr.Camera);
            this.mCharacMgr.AddPlayer(this.mStateMgr.SceneManager, "Sinbad.mesh",
                                      new CharacterInfo("Sinbad", new Vector3(300, 7000, 1000)),
                                      this.mStateMgr.Input, this);
            this.mDebugMode = new DebugMode(this.mStateMgr.Input, this.mCharacMgr);
            this.mSkyMgr.AddListeners();

            return true;
        }

        //get

        public string  getName()       { return this.mName;       }
        public long    getAge()        { return this.mAge;        }
        public long    getSeed()       { return this.mSeed;       }
        public Vector3 getSpawnPoint() { return this.mSpawnPoint; }
        public int     getHeight()     { return MaxHeight;        }
        
        public int getSurfaceHeight(int x, int z, Vector3 islandLoc) {
            for(int y = 0; y < this.mIslandList[islandLoc].getSize().y * CHUNK_SIDE; y++) { 
                if(!this.mIslandList[islandLoc].getBlock(x, y, z).IsAir()) {
                    return y; 
                }
            }
            return -1;
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
                    LogManager.Singleton.DefaultLog.LogMessage("\n \n New SpawnPoint at : " + new Vector3(x * CHUNK_SIDE, y * CHUNK_SIDE, z * CHUNK_SIDE).ToString());
                    this.mSpawnPoint = new Vector3(x * CHUNK_SIDE, 1000, z * CHUNK_SIDE);
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

        public bool hasCollision(Vector3 blockPos, CubeFace collisionSide)
        {

            blockPos -= this.mIslandList[new Vector3(0, 0, 0)].getPosition();

            blockPos /= CUBE_SIDE;

            if      (collisionSide == CubeFace.leftFace)    { blockPos.x--; }
            else if (collisionSide == CubeFace.rightFace)   { blockPos.x++; }
            else if (collisionSide == CubeFace.frontFace)   { blockPos.z++; }
            else if (collisionSide == CubeFace.backFace)    { blockPos.z--; }
            else if (collisionSide == CubeFace.underFace)   { blockPos.y--; }
            else  /*(collisionSide == CubeFace.upperFace)*/ { blockPos.y++; }


            Block block = this.mIslandList[new Vector3(0, 0, 0)].getBlock(blockPos);
            if (block != null && !block.IsAir()) { return true; }

            return false;
        }

    }
}

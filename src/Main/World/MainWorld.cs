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
using Game.World.Blocks;

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

        private StateManager    mStateMgr;
        private SkyMgr          mSkyMgr;
        private MogreNewt.World mNewtonWorld;

        public MogreNewt.World NwtWorld { get { return this.mNewtonWorld; } }

        public MainWorld(StateManager stateMgr)
        {
            this.mEntityList = new List<Entity>();
            this.mAge        = 0;
            this.mSeed       = 42;

            this.mStateMgr = stateMgr;
            this.mNewtonWorld = new MogreNewt.World();
            this.mNewtonWorld.SetFrictionModel(MogreNewt.World.FrictionModelMode.FM_ADAPTIVE);
            this.mNewtonWorld.SetMinimumFrameRate(45);
            //this.mNewtonWorld.

            this.mIslandList = new Dictionary<Vector3, Island>();
            Island island;
            if      (this.mStateMgr.ChosenWorld == StateManager.TypeWorld.Dome)       { island = new DomeIsland  (Vector3.ZERO, new Vector2(3, 3), this); }
            else if (this.mStateMgr.ChosenWorld == StateManager.TypeWorld.Plain)      { island = new RandomIsland(Vector3.ZERO, new Vector2(6, 6), new Vector2(90, 53), this); }
            else  /*(this.mStateMgr.ChosenWorld == StateManager.TypeWorld.Mountain)*/ { island = new RandomIsland(Vector3.ZERO, new Vector2(6, 6), new Vector2(32.5f, 256), this); }
            this.mIslandList.Add(Vector3.ZERO, island);
            this.mIslandList[Vector3.ZERO].display(this.mStateMgr.SceneManager);

            this.mSkyMgr = new SkyMgr(this.mStateMgr);

            this.setSafeSpawnPoint(Vector3.ZERO);
        }


        //get

        public string  getName()       { return this.mName;       }
        public long    getAge()        { return this.mAge;        }
        public long    getSeed()       { return this.mSeed;       }
        public Vector3 getSpawnPoint() { return this.mSpawnPoint; }
        public int     getHeight()     { return MaxHeight;        }
        
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

        public void Update(float frameTime)
        {
            this.mNewtonWorld.Update(frameTime);
            this.mSkyMgr.Update();
        }

        public void Shutdown()
        {
            this.mNewtonWorld.Dispose();
            this.mSkyMgr.Shutdown();
            this.mStateMgr.SceneManager.ClearScene();
        }
    }
}

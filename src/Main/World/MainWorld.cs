using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Ent;
using API.Geo;
using API.Geo.Cuboid;
using API.Generator;

using Game.States;
using Game.World.Generator;

using Mogre;

using Entity = API.Ent.Entity;

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

        private List<Entity> mEntityList;
        private List<Island> mIslandList;


        public MainWorld(StateManager stateMgr) : base(stateMgr) {
            
            this.mEntityList = new List<Entity>();
            this.mAge        = 0;
            this.mSeed       = 42;

            this.mSpawnPoint = Vector3.ZERO;
            this.mIslandList = new List<Island>();

        }

        public override bool Startup(){
            if (this.mIsStartedUp) { return false; }

            this.mIsStartedUp = true;

            this.mIslandList.Add(new DomeIsland(new Vector3(0, 0, 0), new Vector2(3, 3)));

            return true;
        }

        //get

        public string  getName()       { return this.mName;       }
        public long    getAge()        { return this.mAge;        }
        public long    getSeed()       { return this.mSeed;       }
        public Vector3 getSpawnPoint() { return this.mSpawnPoint; }
        public int     getHeight()     { return MaxHeight;        }
        
        public int getSurfaceHeight(int x, int z)                                              { throw new NotImplementedException(); }

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


        public void setName() { throw new NotImplementedException(); }
	    public Entity createEntity(Vector3 point, Entity type) { throw new NotImplementedException(); }
	    public Entity createAndSpawnEntity(Vector3 point, Entity e) { throw new NotImplementedException(); }
	    public void spawnEntity(Entity e) { throw new NotImplementedException(); }
	    public void setSpawnPoint(Vector3 transform) { throw new NotImplementedException(); }
	    public List<Character> getPlayers() { throw new NotImplementedException(); }

	    public void unload(bool save) { throw new NotImplementedException(); }
	    public void save() { throw new NotImplementedException(); }
    }
}

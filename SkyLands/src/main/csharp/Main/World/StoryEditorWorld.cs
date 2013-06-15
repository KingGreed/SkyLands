using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

using API.Ent;
using API.Geo;
using API.Geo.Cuboid;
using API.Generator;
using API.Generic;

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
    public class StoryEditorWorld : API.Geo.World {
        private string   mName;
        private long     mAge;
        private long     mSeed;
        private Vector3 mSpawnPoint;

        private Island   mIslandLoaded;

        private StateManager  mStateMgr;
        private SkyMgr        mSkyMgr;

        public StoryEditorWorld(StateManager stateMgr) {
            this.mAge  = 0;
            this.mSeed = 42;
            this.mName = "SinglePlayer";

            this.mStateMgr = stateMgr;
            this.mStateMgr.SceneMgr.AmbientLight = ColourValue.ZERO;


            SceneNode node = this.mStateMgr.SceneMgr.RootSceneNode.CreateChildSceneNode(Vector3.ZERO);
            this.mIslandLoaded = new FlatIsland(node, new Vector2(1, 1), this);

            this.mSkyMgr = new SkyMgr(this.mStateMgr);

        }

        //get

        public void populate() { this.mIslandLoaded.populate(); }
        public void display()  { this.mIslandLoaded.display(); }

        public string  getName()          { return this.mName;       }
        public long    getAge()           { return this.mAge;        }
        public long    getSeed()          { return this.mSeed;       }
        public Vector3 getSpawnPoint()    { return this.mSpawnPoint; }
        public int getHeight()            { return Cst.MaxHeight;    }
        public StateManager getStateMgr() { return this.mStateMgr;   }


        public SceneManager getSceneMgr()        { return this.mStateMgr.SceneMgr; }
        public SceneNode getAScenNode()          { return this.mStateMgr.SceneMgr.RootSceneNode.CreateChildSceneNode(Vector3.ZERO); }
        public Island getIsland()                { return this.mIslandLoaded; }
        public void setIsland(Island newIsland)  { this.mIslandLoaded = newIsland; }
        
        public int     getSurfaceHeight(int x, int z)                                          { return this.mIslandLoaded.getSurfaceHeight(x, z); }
        public Vector3 getDisplayCoords(Vector3 island, Vector3 relativeLocation)              { return (relativeLocation * Cst.CUBE_SIDE) + island; }

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

        public void unloadIsland() { this.mIslandLoaded = null; }
        

        public void setName(string name) { this.mName =  name;}
        public void setSafeSpawnPoint()  {
            Random random = new Random();
            int x, y, z;
            while(true) {
                x = random.Next((int)this.mIslandLoaded.getSize().x * Cst.CHUNK_SIDE);
                z = random.Next((int)this.mIslandLoaded.getSize().z * Cst.CHUNK_SIDE);

                y = this.getSurfaceHeight(x, z);
                if(y != -1) {
                    this.mSpawnPoint = new Vector3(x + 0.5f, y, z - 0.5f) * Cst.CUBE_SIDE;  // the spawnpoint is at the center of the horizontal faces
                    LogManager.Singleton.DefaultLog.LogMessage("New SpawnPoint at : " + this.mSpawnPoint.ToString());
                    break;
                }
            }
        }



        public void createAndSpawnEntity(Vector3 point, CharacterInfo c) {
            this.mStateMgr.MainState.CharacMgr.AddCharacter(c);
        }

	    public List<Character> getPlayers() { throw new NotImplementedException(); }
        public bool HasPointCollision(Vector3 relBlockPos) { throw new NotImplementedException(); }

        public static Vector3 getRelativeFromAbsolute(Vector3 absCoord) {
            absCoord /= Cst.CUBE_SIDE;
            absCoord.x = Mogre.Math.IFloor(absCoord.x);
            absCoord.y = Mogre.Math.IFloor(absCoord.y);
            absCoord.z = Mogre.Math.ICeil(absCoord.z);

            return absCoord;
        }

        public void onBlockEnter(Vector3 blockCoord, Entity e) {
            this.mIslandLoaded.getBlock(blockCoord, false).onBlockEnter(e, blockCoord);
        }
        public void onBlockLeave(Vector3 blockCoord, Entity e) {
            this.mIslandLoaded.getBlock(blockCoord, false).onBlockLeave(e, blockCoord);
        }

        public void onCreation(Vector3 relCoord) {
            this.mIslandLoaded.getBlock(relCoord, false).onCreation(relCoord);
        }
        public void onDeletion(Vector3 relCoord) {
            this.mIslandLoaded.getBlock(relCoord, false).onDeletion();
        }

        public bool onRightClick(Vector3 relCoord) {
            return this.mIslandLoaded.getBlock(relCoord, false).onRightClick();
        }

        public bool onLeftClick(Vector3 relCoord) {
            return this.mIslandLoaded.getBlock(relCoord, false).onLeftClick();
        }


	    public void unload(bool save) { throw new NotImplementedException(); }

	    public void save(Entity e) {
            e.getIsland().save();
        }

        public void load() {

        }

        public void Update(float frameTime) { this.mSkyMgr.Update(); }

        public static Vector3 AbsToRelative(Vector3 v)
        {
            v /= Cst.CUBE_SIDE;
            v.x = Mogre.Math.IFloor(v.x);
            v.y = Mogre.Math.IFloor(v.y);
            v.z = Mogre.Math.ICeil(v.z);
            return v;
        }

        public void Shutdown() {
            this.mSkyMgr.Shutdown();
            this.mStateMgr.SceneMgr.ClearScene();
        }
    }
}
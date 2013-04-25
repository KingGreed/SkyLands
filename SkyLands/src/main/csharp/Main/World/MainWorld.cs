using System;
using System.Collections.Generic;
using System.Threading;

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
using Game.World.Display;
using Game.World.Generator.Biomes;
using Game.CharacSystem;
using Game.Characters.Misc;

using Mogre;


namespace Game.World
{
    public class MainWorld : API.Geo.World
    {
        private string   mName;
        private long     mAge;
        private long     mSeed;
        private Vector3  mSpawnPoint;

        private List<Entity> mEntityList;
        private Dictionary<Vector3, Island> mIslandList;

        private StateManager  mStateMgr;
        private SkyMgr        mSkyMgr;
        private LaserCube l  = new LaserCube();
        //private Thread        workerThread;

        public MainWorld(StateManager stateMgr)
        {
            this.mEntityList = new List<Entity>();
            this.mAge        = 0;
            this.mSeed       = 42;

            this.mStateMgr = stateMgr;
            this.mStateMgr.SceneMgr.AmbientLight = ColourValue.ZERO;

            this.mIslandList = new Dictionary<Vector3, Island>();
            SceneNode node = this.mStateMgr.SceneMgr.RootSceneNode.CreateChildSceneNode(Vector3.ZERO);
            Island island;

            GameInfo info = this.mStateMgr.GameInfo;
            if      (info.Type == GameInfo.TypeWorld.Dome)       { island = new DomeIsland(node, info.Size, this); }
            else if (info.Type == GameInfo.TypeWorld.Plains)     { island = new RandomIsland(node, info.Size, new Plains(), this); }
            else if (info.Type == GameInfo.TypeWorld.Desert)     { island = new RandomIsland(node, info.Size, new Desert(), this); }
            else if (info.Type == GameInfo.TypeWorld.Hills)      { island = new RandomIsland(node, info.Size, new Hills(), this); }
            else  /*(info.Type == GameInfo.TypeWorld.Mountain)*/ { island = new RandomIsland(node, info.Size, new Mountains(), this); }

            this.mIslandList.Add(Vector3.ZERO, island);
            this.mIslandList[Vector3.ZERO].display();

            this.mSkyMgr = new SkyMgr(this.mStateMgr);

            this.setSafeSpawnPoint(Vector3.ZERO);

            Light lightGV = this.mStateMgr.SceneMgr.CreateLight( "LightGV" );
            lightGV.Type = Light.LightTypes.LT_POINT;
            lightGV.Position       = this.mSpawnPoint;
            lightGV.DiffuseColour  = new ColourValue(1.0f, 1.0f, 1.0f);
            lightGV.SpecularColour = new ColourValue(1.0f, 1.0f, 1.0f);
            lightGV.SetAttenuation(10000f, 1.0f, 0.0045f, 0.00075f);
            lightGV.CastShadows = false;

            LogManager.Singleton.DefaultLog.LogMessage("2");
            l.makeFireCubeAt(this.mSpawnPoint + new Vector3(100, 200, 0), this.mStateMgr.SceneMgr);
            LogManager.Singleton.DefaultLog.LogMessage("3");


            /*StaticRectangle.DisplayRectangle(this.mSpawnPoint, 5, 5, 16*CHUNK_SIDE*CUBE_SIDE, node);
            Vector3 pos = new Vector3((this.mIslandList[new Vector3(0, 0, 0)].getSize().x + 3) * CHUNK_SIDE * CUBE_SIDE, 0, (this.mIslandList[new Vector3(0, 0, 0)].getSize().z + 3) * CHUNK_SIDE * CUBE_SIDE);

            node = this.mStateMgr.SceneManager.RootSceneNode.CreateChildSceneNode(pos);

            island = new RandomIsland(node, new Vector2(13, 13), new Hills(), this);
            island.display();
            this.mIslandList.Add(pos, island);
            */
        }


        //get

        public string  getName()       { return this.mName;       }
        public long    getAge()        { return this.mAge;        }
        public long    getSeed()       { return this.mSeed;       }
        public Vector3 getSpawnPoint() { return this.mSpawnPoint; }
        public int getHeight()         { return Cst.MaxHeight;    }
        
        public SceneManager getSceneMgr()        { return this.mStateMgr.SceneMgr; }
        public Island getIslandAt(Vector3 loc)   { return this.mIslandList[loc]; }
        
        public int     getSurfaceHeight(int x, int z, Vector3 islandLoc)                       { return this.mIslandList[islandLoc].getSurfaceHeight(x, z); }
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
        

        public void setName(string name) { this.mName =  name;}
        public void setSafeSpawnPoint(Vector3 islandLoc)  {
            Random random = new Random();
            int x, y, z;
            while(true) {
                x = random.Next((int)this.mIslandList[islandLoc].getSize().x * Cst.CHUNK_SIDE);
                z = random.Next((int)this.mIslandList[islandLoc].getSize().z * Cst.CHUNK_SIDE);

                y = this.getSurfaceHeight(x, z, islandLoc);
                if(y != -1) {
                    LogManager.Singleton.DefaultLog.LogMessage("\n \n New SpawnPoint at : " + new Vector3(x * Cst.CUBE_SIDE, y * Cst.CUBE_SIDE, z * Cst.CUBE_SIDE).ToString());
                    this.mSpawnPoint = islandLoc + new Vector3(x * Cst.CUBE_SIDE, y * Cst.CUBE_SIDE, z * Cst.CUBE_SIDE);
                    break;
                }
            }
        }

        

	    public Entity createEntity(Vector3 point, Entity type) { throw new NotImplementedException(); }
	    public Entity createAndSpawnEntity(Vector3 point, Entity e) { throw new NotImplementedException(); }
	    public void spawnEntity(Entity e) { throw new NotImplementedException(); }
	    public void setSpawnPoint(Vector3 transform) { throw new NotImplementedException(); }
	    public List<Character> getPlayers() { throw new NotImplementedException(); }

        public Vector3 getRelativeFromAbsolute(Vector3 absCoord) {
            absCoord /= Cst.CUBE_SIDE;
            absCoord.x = Mogre.Math.IFloor(absCoord.x);
            absCoord.y = Mogre.Math.IFloor(absCoord.y);
            absCoord.z = Mogre.Math.ICeil(absCoord.z);

            return absCoord;
        }

        public void onCreation(Vector3 absCoord, Vector3 island) {
            this.mIslandList[island].getBlock(this.getRelativeFromAbsolute(absCoord), false).onCreation(absCoord);
        }
        public void onDeletion(Vector3 absCoord, Vector3 island) {
            this.mIslandList[island].getBlock(this.getRelativeFromAbsolute(absCoord), false).onDeletion();
        }

        public void onRightClick(Vector3 absCoord, Vector3 island) {
            this.mIslandList[island].getBlock(this.getRelativeFromAbsolute(absCoord), false).onRightClick();
        }

        public void onLeftClick(Vector3 absCoord, Vector3 island) {
            this.mIslandList[island].getBlock(this.getRelativeFromAbsolute(absCoord), false).onLeftClick();
        }


	    public void unload(bool save) { throw new NotImplementedException(); }
	    public void save() { throw new NotImplementedException(); }

        public bool HasPointCollision(ref Vector3 blockPos, Vector3 islandLoc) // the argument blockPos is in absolute coord, it becomes relative
        {
            blockPos += this.mIslandList[islandLoc].getPosition();
            blockPos = AbsToRelative(blockPos);

            Block block = this.mIslandList[islandLoc].getBlock(blockPos, false);
            return !(block == null || block is Air);
        }

        public void generateIslandThreaded()
        {
            Vector3 pos = new Vector3(this.mIslandList[new Vector3(0, 0, 0)].getPosition().x * Cst.CHUNK_SIDE + 3 * Cst.CHUNK_SIDE, 0, this.mIslandList[new Vector3(0, 0, 0)].getPosition().z * Cst.CHUNK_SIDE + 3 * Cst.CHUNK_SIDE);
            /*SceneNode node = this.mStateMgr.SceneManager.RootSceneNode.CreateChildSceneNode(Vector3.ZERO);
            this.mIslandList.Add(pos, new RandomIsland(node, new Vector2(13, 13), new Hills(), this));

            workerThread = new Thread(this.mIslandList[pos].display);*/
        }

        public void Update(float frameTime) { this.mSkyMgr.Update(); l.update(frameTime); }

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
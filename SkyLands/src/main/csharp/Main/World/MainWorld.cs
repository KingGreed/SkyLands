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
using Game.World.Display;
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

        private Island   mIslandLoaded;

        private StateManager  mStateMgr;
        private SkyMgr        mSkyMgr;

        public MainWorld(StateManager stateMgr)
        {
            this.mAge        = 0;
            this.mSeed       = 42;
            this.mName = Guid.NewGuid().ToString();

            this.mStateMgr = stateMgr;
            this.mStateMgr.SceneMgr.AmbientLight = ColourValue.ZERO;

            SceneNode node = this.mStateMgr.SceneMgr.RootSceneNode.CreateChildSceneNode(Vector3.ZERO);

            GameInfo info = this.mStateMgr.GameInfo;
            if      (info.Type == GameInfo.TypeWorld.Dome)       { this.mIslandLoaded = new DomeIsland(node, info.Size, this); }
            else if (info.Type == GameInfo.TypeWorld.Plains)     { this.mIslandLoaded = new RandomIsland(node, info.Size, new Plains(), this); }
            else if (info.Type == GameInfo.TypeWorld.Desert)     { this.mIslandLoaded = new RandomIsland(node, info.Size, new Desert(), this); }
            else if (info.Type == GameInfo.TypeWorld.Hills)      { this.mIslandLoaded = new RandomIsland(node, info.Size, new Hills(), this); }
            else  /*(info.Type == GameInfo.TypeWorld.Mountain)*/ { this.mIslandLoaded = new RandomIsland(node, info.Size, new Mountains(), this); }

            this.mSkyMgr = new SkyMgr(this.mStateMgr);
        }


        //get

        public void populate() { this.mIslandLoaded.populate(); this.setSafeSpawnPoint(); }
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
                    LogManager.Singleton.DefaultLog.LogMessage("New SpawnPoint at : " + new Vector3(x * Cst.CUBE_SIDE, y * Cst.CUBE_SIDE, z * Cst.CUBE_SIDE).ToString());
                    this.mSpawnPoint = new Vector3(x * Cst.CUBE_SIDE, y * Cst.CUBE_SIDE, z * Cst.CUBE_SIDE);
                    break;
                }
            }
        }



        public void createEntity(Vector3 point, Entity type) { type.getIsland().mEntitiesInIsland.Add(type); }
        public void createAndSpawnEntity(Vector3 point, Entity e) { throw new NotImplementedException(); }
	    public void spawnEntity(Entity e) { throw new NotImplementedException(); }
	    public List<Character> getPlayers() { throw new NotImplementedException(); }

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
            this.mIslandLoaded.getBlock(MainWorld.getRelativeFromAbsolute(relCoord), false).onCreation(relCoord);
        }
        public void onDeletion(Vector3 relCoord) {
            this.mIslandLoaded.getBlock(MainWorld.getRelativeFromAbsolute(relCoord), false).onDeletion();
        }

        public bool onRightClick(Vector3 relCoord) {
            return this.mIslandLoaded.getBlock(MainWorld.getRelativeFromAbsolute(relCoord), false).onRightClick();
        }

        public bool onLeftClick(Vector3 relCoord) {
            return this.mIslandLoaded.getBlock(MainWorld.getRelativeFromAbsolute(relCoord), false).onLeftClick();
        }


	    public void unload(bool save) { throw new NotImplementedException(); }

	    public void save(Entity e) {

            var playerFileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\" +
                this.getName() + "-player-" + ".sav";

            new FileInfo(playerFileName).Directory.Create();
            Stream stream;
            BinaryWriter writter;

            try { stream = new FileStream(playerFileName, FileMode.Create, FileAccess.Write); writter = new BinaryWriter(stream); }
            catch { throw new Exception("Could not read file : " + playerFileName); }


            writter.Write(e.getPosition().x);
            writter.Write(e.getPosition().y);
            writter.Write(e.getPosition().z);

            writter.Close();
            stream.Close();

            e.getIsland().save();
        }

        public bool HasPointCollision(ref Vector3 blockPos) // the argument blockPos is in absolute coord, it becomes relative
        {
            blockPos += this.mIslandLoaded.getPosition();
            blockPos = AbsToRelative(blockPos);

            Block block = this.mIslandLoaded.getBlock(blockPos, false);
            return !(block == null || block is Air);
        }

        public void generateIslandThreaded()
        {
            Vector3 pos = new Vector3(this.mIslandLoaded.getPosition().x * Cst.CHUNK_SIDE + 3 * Cst.CHUNK_SIDE, 0, this.mIslandLoaded.getPosition().z * Cst.CHUNK_SIDE + 3 * Cst.CHUNK_SIDE);
            /*SceneNode node = this.mStateMgr.SceneManager.RootSceneNode.CreateChildSceneNode(Vector3.ZERO);
            this.mIslandList.Add(pos, new RandomIsland(node, new Vector2(13, 13), new Hills(), this));

            workerThread = new Thread(this.mIslandList[pos].display);*/
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
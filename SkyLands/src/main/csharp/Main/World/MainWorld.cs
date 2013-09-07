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
using Game.World.Display;

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

        private Dictionary<Vector3, List<string>> d = new Dictionary<Vector3, List<string>>();

        private int mScenarioAdvancement = 0;

        public MainWorld(StateManager stateMgr) {
            this.mAge  = 0;
            this.mSeed = 42;
            this.mName = "SinglePlayer";

            this.mStateMgr = stateMgr;
            this.mStateMgr.SceneMgr.AmbientLight = ColourValue.ZERO;
            
            GameInfo info = this.mStateMgr.GameInfo;
            if(!info.Load) {
                SceneNode node = this.mStateMgr.SceneMgr.RootSceneNode.CreateChildSceneNode(Vector3.ZERO);
                if      (info.Type == TypeWorld.Plains) { this.mIslandLoaded = new RandomIsland(node, info.Size, new Plains(),    this); }
                else if (info.Type == TypeWorld.Desert) { this.mIslandLoaded = new RandomIsland(node, info.Size, new Desert(),    this); }
                else if (info.Type == TypeWorld.Hills)  { this.mIslandLoaded = new RandomIsland(node, info.Size, new Hills(),     this); }
                else                                    { this.mIslandLoaded = new RandomIsland(node, info.Size, new Mountains(), this); }

                if(info.Scenario != "") {
                    VanillaIsland isl = (VanillaIsland)this.mIslandLoaded;
                    isl.loadStructures(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\" + info.Scenario + "\\");

                    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\" + info.Scenario + "\\";

                    string[] s = File.ReadAllLines(path + "structures.scenario");
                    for(int i = 0; i < s.Length; i++) {
                        string[] ss = File.ReadAllLines(path + s[i] + ".event");
                        
                        for(int j = 0; j < s.Length; j++) {
                            string[] line = s[j].Split(' ');

                            if(line.Length >= 3) {
                                Vector3 v = new Vector3(Convert.ToInt32(line[0]), Convert.ToInt32(line[1]), Convert.ToInt32(line[2]));
                                
                                if(d.ContainsKey(v)) {
                                    d[v].Add(string.Join(" ", line, 3, line.Length - 3));
                                } else {
                                    d.Add(new Vector3(Convert.ToInt32(line[0]), Convert.ToInt32(line[1]), Convert.ToInt32(line[2])),
                                        new List<string> { string.Join(" ", line, 3, line.Length - 3) });
                                }
                            }
                        }
                    }
                }

            } else {
                this.load();
            }
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

        public static Vector3 getRelativeFromAbsolute(Vector3 absCoord) {
            absCoord /= Cst.CUBE_SIDE;
            absCoord.x = Mogre.Math.IFloor(absCoord.x);
            absCoord.y = Mogre.Math.IFloor(absCoord.y);
            absCoord.z = Mogre.Math.ICeil(absCoord.z);

            return absCoord;
        }
        SceneNode n;

        public void onBlockEnter(Vector3 blockCoord, Entity e) {
            
            if(d.ContainsKey(blockCoord)) {
                foreach(string s in d[blockCoord]) {
                    new Script.Parser(this.mStateMgr).Parse(s); 
                }
            }
            if(blockCoord.y >= 200 && n == null) {
                n = ParticleGenerator.mkParticle(this.mStateMgr.SceneMgr, blockCoord + Vector3.UNIT_Y * 600, "Snow");
            }

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

            var playerFileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\" +
                this.getName() + "-player" + ".sav";

            DirectoryInfo directoryInfo = new FileInfo(playerFileName).Directory;
	        if (directoryInfo != null) { directoryInfo.Create(); }


	        using(TextWriter writer = new StreamWriter(playerFileName)) {
                writer.WriteLine(e.getIsland().getBiome().getId());
                writer.WriteLine(e.getPosition().x);
                writer.WriteLine(e.getPosition().y);
                writer.WriteLine(e.getPosition().z);
            }



            e.getIsland().save();
        }

        public void load() {
            var playerFileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SkyLands\\" + this.getName() + "-player" + ".sav";

            StreamReader stream;

            try { stream = new StreamReader(playerFileName); }
            catch { throw new Exception("Could not read file : " + playerFileName); }

            TypeWorld g = (TypeWorld)Convert.ToInt32(stream.ReadLine());

            if (g == TypeWorld.Plains)      { this.mIslandLoaded = new RandomIsland(this.getAScenNode(), new Plains(),    this); }
            else if (g == TypeWorld.Desert) { this.mIslandLoaded = new RandomIsland(this.getAScenNode(), new Desert(),    this); }
            else if (g == TypeWorld.Hills)  { this.mIslandLoaded = new RandomIsland(this.getAScenNode(), new Hills(),     this); }
            else                            { this.mIslandLoaded = new RandomIsland(this.getAScenNode(), new Mountains(), this); }



        }

        public bool HasPointCollision(Vector3 relBlockPos)
        {
            relBlockPos += this.mIslandLoaded.getPosition();

            Block block = this.mIslandLoaded.getBlock(relBlockPos, false);
            return !(block == null || block is Air);
        }

        public void generateIslandThreaded()
        {
            //Vector3 pos = new Vector3(this.mIslandLoaded.getPosition().x * Cst.CHUNK_SIDE + 3 * Cst.CHUNK_SIDE, 0, this.mIslandLoaded.getPosition().z * Cst.CHUNK_SIDE + 3 * Cst.CHUNK_SIDE);
            /*SceneNode node = this.mStateMgr.SceneManager.RootSceneNode.CreateChildSceneNode(Vector3.ZERO);
            this.mIslandList.Add(pos, new RandomIsland(node, new Vector2(13, 13), new Hills(), this));

            workerThread = new Thread(this.mIslandList[pos].display);*/
        }

        private int t(string s) { return Convert.ToInt32(s); }

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
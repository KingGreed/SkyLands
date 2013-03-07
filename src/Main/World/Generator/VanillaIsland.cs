using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


using API.Geo.Cuboid;
using API.Generator;
using API.Generic;

using Mogre;

using Game.Display;
using Game.World.Blocks;

using Material = API.Generic.Material;


namespace Game.World.Generator
{
    public abstract class VanillaIsland : Island
    {
        static Block defaultBlock = new AirBlock();

        public VanillaIsland(SceneNode node, Vector2 size, MainWorld currentWorld) : base(node, size, currentWorld) {
            foreach (KeyValuePair<string, Block> pair in VanillaChunk.staticBlock) {
                if(!(pair.Value is AirBlock)) {
                    for (int i = 0; i < 6; i++) {
                        if (!this.multiList.ContainsKey(pair.Value.getFace(i))) {
                            this.multiList.Add(pair.Value.getFace(i), new VanillaMultiBlock(pair.Value.getFace(i)));
                        }
                    }
                }
            }


        }
       
        public override void initChunks(Vector2 size) {
            for(int x = 0; x < size.x; x++) {
                for(int z = 0; z < size.y; z++) {
                    this.mChunkList.Add(new Vector3(x, 0, z), new VanillaChunk(new Vector3(16,16,16), new Vector3(x, 0, z), this));
                }
            }
        }

        public override int getSurfaceHeight(int x, int z, string restriction = "") {
            for(int y = (int)this.mIslandSize.y * MainWorld.CHUNK_SIDE; y != 0 ; y--) { 
                if(!(this.getBlock(x, y, z, false) is AirBlock)) {
                    if (restriction == "") { return y + 1; }
                    else if (this.getBlock(x, y, z, false).getName() != restriction) { continue; }
                    else { return y + 1; }
                }
            }
            return -1;
        }

        public override bool hasBlock(int x, int y, int z) { throw new NotImplementedException(); }


        public override void display() {

            Block curr;
            LogManager.Singleton.DefaultLog.LogMessage("Now displaying");

            for(int x = 0; x < this.mIslandSize.x * MainWorld.CHUNK_SIDE; x++) {
                for(int y = 0; y < this.mIslandSize.y * MainWorld.CHUNK_SIDE; y++) {
                    for(int z = 0; z < this.mIslandSize.z * MainWorld.CHUNK_SIDE; z++) {
                        curr = this.getBlock(x, y, z, false);
                        if(!(curr is AirBlock) && this.setVisibleFaces(new Vector3(x, y, z), curr)) {
                            for(int i = 0; i < 6; i++) {
                                if(this.hasVisiblefaceAt(x, y, z, (BlockFace) i)) {
                                    this.multiList[curr.getFace(i)].addBlock(new Vector3(x, y, z));
                                }
                            }

                        }
                    }
                }
            }
            foreach(KeyValuePair<string, MultiBlock> pair in this.multiList) {
                pair.Value.display(this, this.mWorld);
                pair.Value.addMultiToScene();
            }
        }


        //For optimization purpose returns wether the block has visible faces
        public bool setVisibleFaces(Vector3 blockCoord, Block curr)
        {

            if (curr is AirBlock) { return false; }
            bool hasVisiblefaces = false;
            Dictionary<BlockFace, Vector3> coordToCheck = new Dictionary<BlockFace,Vector3>();

            coordToCheck.Add(BlockFace.rightFace, new Vector3(blockCoord.x + 1, blockCoord.y,     blockCoord.z));
            coordToCheck.Add(BlockFace.leftFace,  new Vector3(blockCoord.x - 1, blockCoord.y,     blockCoord.z));
            coordToCheck.Add(BlockFace.upperFace, new Vector3(blockCoord.x,     blockCoord.y + 1, blockCoord.z));
            coordToCheck.Add(BlockFace.underFace, new Vector3(blockCoord.x,     blockCoord.y - 1, blockCoord.z));
            coordToCheck.Add(BlockFace.frontFace, new Vector3(blockCoord.x,     blockCoord.y,     blockCoord.z + 1));
            coordToCheck.Add(BlockFace.backFace,  new Vector3(blockCoord.x,     blockCoord.y,     blockCoord.z - 1));


            foreach (KeyValuePair<BlockFace, Vector3> keyVal in coordToCheck)
            {
                if (this.getBlock(keyVal.Value, false) is AirBlock) { 
                    this.setVisibleFaceAt(blockCoord, keyVal.Key, true); hasVisiblefaces = true; 
                }
            }
            return hasVisiblefaces;
        }

        private void setVisibleFaceAt(Vector3 loc, BlockFace face, bool val) {
            this.setVisibleFaceAt((int)loc.x, (int)loc.y, (int)loc.z, face, val); 
        }

        public override void setVisibleFaceAt(int x, int y, int z, BlockFace face, bool val) {
            Chunk blockChunk = this.getChunkFromBlock(x, y, z);
            if(blockChunk != null) {
                blockChunk.setVisibleFaceAt(x % 16, y % 16, z % 16, face, val);
            }
        }

        public override bool hasVisiblefaceAt(int x, int y, int z, BlockFace face) {
            Chunk blockChunk = this.getChunkFromBlock(x, y, z);
            if(blockChunk != null) {
                return blockChunk.hasVisibleFaceAt(x % 16, y % 16, z % 16, face);
            }
            return true;
        }

        public override Block getBlock(int x, int y, int z, bool force) {
            if(x < 0 || y < 0 || z < 0) { return defaultBlock; }

            if(force && y > this.mIslandSize.y * MainWorld.CHUNK_SIDE) {
                this.mIslandSize.y = (int) System.Math.Ceiling((float)y / 16f);
            }

            Vector3 chunkLocation = getChunkCoordFromRelative(x, y, z), 
                    blockLocation = getBlockCoordFromRelative(x, y, z);

            if(this.hasChunk(chunkLocation)) { return this.mChunkList[chunkLocation].getBlock(blockLocation); }
            else if(force) {
                this.mChunkList.Add(chunkLocation, new VanillaChunk(new Vector3(16,16,16), chunkLocation, this));
                return this.mChunkList[chunkLocation].getBlock(blockLocation);
            }
            else { return defaultBlock; }
        }

        public override void getBlockCoord(Vector3 pos, out Vector3 blockPos, out Vector3 chunkPos)
        {
            chunkPos = getChunkCoordFromRelative((int)pos.x, (int)pos.y, (int)pos.z);
            blockPos = getBlockCoordFromRelative((int)pos.x, (int)pos.y, (int)pos.z);

            if (!this.hasChunk(chunkPos) || pos.x < 0 || pos.y < 0 || pos.z <0)
            {
                chunkPos = -Vector3.UNIT_SCALE;
                blockPos = -Vector3.UNIT_SCALE;
            }
        }

        public override Vector3 getBlockCoord(int x, int y, int z) {
            if(x < 0 || y < 0 || z < 0) { return -Vector3.UNIT_SCALE; }
            
            Vector3 chunkLocation = getChunkCoordFromRelative(x, y, z), 
                    blockLocation = getBlockCoordFromRelative(x, y, z);

            if(this.hasChunk(chunkLocation)) { return blockLocation; }
            else                             { return -Vector3.UNIT_SCALE; }
        }

        public override Chunk getChunkFromBlock(int x, int y, int z) {
            if(x < 0 || y < 0 || z < 0) { return null; }


            Vector3 chunkLocation = getChunkCoordFromRelative(x, y, z);

            if(this.hasChunk(chunkLocation)) { return this.mChunkList[chunkLocation]; }
            else                             { return null; }
        }

        private Vector3 getBlockCoordFromRelative(int x, int y, int z) { return new Vector3(x % 16, y % 16, z % 16); }
        private Vector3 getChunkCoordFromRelative(int x, int y, int z) { return new Vector3(x / 16, y / 16, z / 16); }

        public override void setBlockAt(int x, int y, int z, string material, bool force) {
            if(x < 0 || y < 0 || z < 0) { return; }

            if(force && y > this.mIslandSize.y * MainWorld.CHUNK_SIDE) {
                this.mIslandSize.y = (int) System.Math.Ceiling((float)y / 16f);
            }

            Vector3 chunkLocation = getChunkCoordFromRelative(x, y, z), 
                    blockLocation = getBlockCoordFromRelative(x, y, z);

            if (this.hasChunk(chunkLocation)) { this.mChunkList[chunkLocation].setBlock(blockLocation, material); }
            else if(force) {
                this.mChunkList.Add(chunkLocation, new VanillaChunk(MainWorld.CHUNK_SIDE * Vector3.UNIT_SCALE, chunkLocation, this));
                this.mChunkList[chunkLocation].setBlock(blockLocation, material);
            } 
        }

        public override void setBlockAt(int x, int y, int z, byte material, bool force) { this.setBlockAt(x, y, z, VanillaChunk.byteToString[material], force); }

        public override void removeFromScene(Vector3 item) {
            Block curr = this.getBlock(item, false);
            if (curr is AirBlock) { return; }

            bool isInAdded = false;
            Dictionary<string, int> faces = new Dictionary<string, int>();

            for (int i = 0; i < 6; i++) {
                if (this.isInBlocksAdded(item, (BlockFace)i)) {
                    isInAdded = true;
                } else {
                    if (this.hasVisiblefaceAt((int)item.x, (int)item.y, (int)item.z, (BlockFace)i)) {
                        if (!faces.ContainsKey(curr.getFace(i))) { faces.Add(curr.getFace(i), 1); }
                        else { faces[curr.getFace(i)]++; }
                    }
                }
            }

            foreach (KeyValuePair<string, int> pair in faces) {
                LogManager.Singleton.DefaultLog.LogMessage("item : " + pair.Key + " has value : " + pair.Value);
                this.multiList[pair.Key].removeFromScene(item, pair.Value);
            }

            if (isInAdded) {
                string cubeNodeName = "Node-" + item.x * MainWorld.CUBE_SIDE + "-" + item.y * MainWorld.CUBE_SIDE + "-" + item.z * MainWorld.CUBE_SIDE;
                this.mFaceNode.RemoveChild(cubeNodeName);
            }

            this.setBlockAt((int) item.x, (int) item.y, (int) item.z, "Air", false);

            refreshBlock(new Vector3(item.x,   item.y-1, item.z));
            refreshBlock(new Vector3(item.x,   item.y+1, item.z));
            refreshBlock(new Vector3(item.x+1, item.y,   item.z));
            refreshBlock(new Vector3(item.x-1, item.y,   item.z));
            refreshBlock(new Vector3(item.x,   item.y,   item.z+1));
            refreshBlock(new Vector3(item.x,   item.y,   item.z-1));

        }

        public override void refreshBlock(Vector3 relativePos) {

            Block curr = this.getBlock(relativePos, false);
            bool[] currentvisibleFaces = new bool[6];
            bool isVisible;
            int i = 0;

            foreach (BlockFace face in Enum.GetValues(typeof(BlockFace))) {
                currentvisibleFaces[i] = this.hasVisiblefaceAt((int) relativePos.x, (int) relativePos.y, (int) relativePos.z, face);
                i++;
            }
            
            this.setVisibleFaces(relativePos, curr);

            for (int j = 0; j < 6; j++) {
                isVisible = this.hasVisiblefaceAt((int) relativePos.x, (int) relativePos.y, (int) relativePos.z, (BlockFace)j);
                if(isVisible && isVisible != currentvisibleFaces[j]) {
                    this.addFaceToScene((BlockFace)j, relativePos, curr.getFace(j));
                }
            }

        }

        public override void addBlockToScene(Vector3 relativePos, string material) {
            this.setBlockAt((int)relativePos.x, (int)relativePos.y, (int)relativePos.z, material, true);
            
            Block curr = this.getBlock(relativePos, false);

            if(this.setVisibleFaces(relativePos, curr)) {
                for(int i = 0; i < 6; i++) {
                    
                    if (this.hasVisiblefaceAt((int)relativePos.x, (int)relativePos.y, (int)relativePos.z, (BlockFace) i)) {
                        this.addFaceToScene((BlockFace) i, relativePos, curr.getFace(i));
                    }
                }
            }

        }

        public override void addFaceToScene(BlockFace face, Vector3 relativePos, string material) {

            this.blocksAdded.Add(new PositionFaceAndStatus(relativePos, face));
            
            relativePos += this.getPosition();
            relativePos *= MainWorld.CUBE_SIDE;

            string cubeNodeName = "Node-" + relativePos.x + "-" + relativePos.y + "-" + relativePos.z;
            SceneNode blockNode;
            string faceName, faceEntName;
            Entity ent;

            if(this.mWorld.getSceneMgr().HasSceneNode(cubeNodeName)) {
                blockNode = this.mWorld.getSceneMgr().GetSceneNode(cubeNodeName);
            } else {
                blockNode = this.mFaceNode.CreateChildSceneNode(cubeNodeName, relativePos);
            }
            
            faceName = GraphicBlock.getFaceName(face);
            faceEntName = "face-" + relativePos.x + "-" + relativePos.y + "-" + relativePos.z + "-" + faceName;

            ent = this.mWorld.getSceneMgr().CreateEntity(faceEntName, faceName);
            ent.SetMaterialName(material);
            blockNode.AttachObject(ent);
        }



        public override void save() {
            var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Roaming/SkyLands/",
                this.mWorld.getName(), "Island-", this.mNode.Position.x.ToString(), "-",
                this.mNode.Position.y.ToString(), "-", this.mNode.Position.z.ToString());

            Stream writer;

            try { writer = new FileStream(fileName, FileMode.Create, FileAccess.Write); }
            catch { throw new Exception("Could not read file : " + fileName); }

            writer.WriteByte((byte)this.mIslandSize.x);
            writer.WriteByte((byte)this.mIslandSize.z);
            writer.WriteByte((byte)this.mIslandSize.y);

            for(int x = 0; x < this.mIslandSize.x * MainWorld.CHUNK_SIDE; x++) {
                for(int z = 0; z < this.mIslandSize.z * MainWorld.CHUNK_SIDE; z++) {
                    for(int y = 0; y < this.mIslandSize.y * MainWorld.CHUNK_SIDE; y++) {
                        writer.WriteByte(this.getBlock(new Vector3(x, y, z), false).getId());
                    }
                }
            }

            writer.Close();
        }

        public void load() {
            var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Roaming/SkyLands/",
                this.mWorld.getName(), "Island-", this.mNode.Position.x.ToString(), "-",
                this.mNode.Position.y.ToString(), "-", this.mNode.Position.z.ToString());

            FileStream reader;
            
            try { reader = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None); }
            catch { throw new Exception("Could not read file : " + fileName); }

            this.mIslandSize.x = reader.ReadByte();
            this.mIslandSize.z = reader.ReadByte();
            this.mIslandSize.y = reader.ReadByte();

            for(int x = 0; x < this.mIslandSize.x * MainWorld.CHUNK_SIDE; x++) {
                for(int z = 0; z < this.mIslandSize.z * MainWorld.CHUNK_SIDE; z++) {
                    for(int y = 0; y < this.mIslandSize.y * MainWorld.CHUNK_SIDE; y++) {
                        this.setBlockAt(x, y, z, (byte)reader.ReadByte(), true);
                    }
                }
            }

            reader.Close();
        }

        public override string getMaterialFromName(string name) {
            return VanillaChunk.staticBlock[name].getMaterial();
        }
    }
}

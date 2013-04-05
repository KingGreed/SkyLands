using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game.World.Generator;
using API.Generic;

using Mogre;

namespace Game.Characters.Misc {
    class LaserCube {
        private SceneNode node;
        private double timeSinceCreation = 0;
        private bool created = false;

        public void makeFireCubeAt(Vector3 loc, SceneManager mgr) {
            int faceNumber = 0;
            ManualObject ball = new ManualObject("fireBall-" + Guid.NewGuid().ToString());
            ball.Begin("fireCube", RenderOperation.OperationTypes.OT_TRIANGLE_LIST);
            foreach(BlockFace face in Enum.GetValues(typeof(BlockFace))) {
                for(int i = 0; i < 4; i++) {
                    ball.Position(VanillaMultiBlock.blockPointCoords[(int)face * 4 + i]); ball.TextureCoord(VanillaMultiBlock.textureCoord[(int)face * 4 + i]);
                    ball.Normal(VanillaMultiBlock.normals[(int)face]);
                    faceNumber++;
                }
                ball.Quad((uint)faceNumber - 4, (uint)faceNumber - 3, (uint)faceNumber - 2, (uint)faceNumber - 1);
            }
            ball.End();

            this.node = mgr.RootSceneNode.CreateChildSceneNode(loc);
            this.node.AttachObject(ball);
            this.created = true;
        }

        public void update(float frameTime) {
            if(this.created) {
                this.timeSinceCreation += frameTime;

            }
        }

       
    }
}

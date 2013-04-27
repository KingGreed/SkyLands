using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game.World.Generator;
using API.Generic;
using Game.CharacSystem;

using Mogre;

namespace Game.Characters.Misc
{
    class LaserCube
    {
        private const float SCALE_SPEED = 1.1f;
        private const float MAX_SCALE = 3;
        
        private SceneManager mSceneMgr;
        private VanillaPlayer mPlayer;
        private SceneNode mNode;
        private double mTimeSinceCreation = 0;
        private bool mCreated = false;

        public LaserCube(SceneManager sceneMgr, VanillaPlayer player)
        {
            this.mSceneMgr = sceneMgr;
            this.mPlayer = player;
        }

        public void Grow(float frameTime)
        {
            if (!mCreated)
                this.Create();
            else
            {
                this.mNode.Scale(SCALE_SPEED * Vector3.UNIT_SCALE);
                if (this.mNode.GetScale().x >= MAX_SCALE)
                {
                    this.mNode.Scale(MAX_SCALE * Vector3.UNIT_SCALE);
                    this.Burst();
                }
            }
        }

        public void Burst()
        {
            this.mNode.Dispose();
            this.mCreated = false;
        }

        private void Create()
        {
            int faceNumber = 0;
            ManualObject ball = new ManualObject("fireBall-" + Guid.NewGuid().ToString());
            ball.Begin("fireCube", RenderOperation.OperationTypes.OT_TRIANGLE_LIST);
            foreach(BlockFace face in Enum.GetValues(typeof(BlockFace))) {
                for(int i = 0; i < 4; i++) {
                    ball.Position(VanillaMultiBlock.blockPointCoords[(int)face * 4 + i]/10); ball.TextureCoord(VanillaMultiBlock.textureCoord[(int)face * 4 + i]);
                    ball.Normal(VanillaMultiBlock.normals[(int)face]);
                    faceNumber++;
                }
                ball.Quad((uint)faceNumber - 4, (uint)faceNumber - 3, (uint)faceNumber - 2, (uint)faceNumber - 1);
            }
            ball.End();

            /* Determine the location */
            Ray ray = this.mPlayer.Camera.GetCameraToViewportRay(0.5f, 0.5f);
            ray.Origin = this.mPlayer.FeetPosition + Vector3.UNIT_Y * VanillaCharacter.CHARAC_SIZE / 1.2f;

            this.mNode = this.mSceneMgr.RootSceneNode.CreateChildSceneNode(ray.GetPoint(50));
            this.mNode.AttachObject(ball);
            this.mCreated = true;
        }
    }
}

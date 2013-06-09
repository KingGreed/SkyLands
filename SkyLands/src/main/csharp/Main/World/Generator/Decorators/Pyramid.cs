using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generator;
using API.Geo.Cuboid;
using API.Generic;

using Game.World.Blocks;
using Game.CharacSystem;
using Game.States;

using Mogre;

namespace Game.World.Generator.Decorators
{
    class Pyramid : Decorator
    {
        public Pyramid() { }
        private int _width = 14;

        public override void populate(Island curr, Random random)
        {
            Vector3 pos = this.findRandomPoint(curr, random);
            int tmp = 0;

            string brick = "Sand";

            curr.setBlockAt((int)pos.x, (int)pos.y, (int)pos.z, brick, true);
            for (int y = 0; y < 18; y++)
            {
                for (int x = 0 + tmp; x < _width - tmp; x++)
                {
                    for (int z = 0 + tmp; z < _width - tmp; z++) {

                        if (y == 0 || y == 7)
                            curr.setBlockAt((int)pos.x + x, (int)pos.y + y - 7, (int)pos.z + z, brick, true);
                        else if ((x == 0 || z == 0 || x == _width - 1 || z == _width - 1) && y <= 6)
                            curr.setBlockAt((int)pos.x + x, (int)pos.y + y - 7, (int)pos.z + z, brick, true);
                        else if (y > 7)
                            curr.setBlockAt((int)pos.x + x, (int)pos.y + y - 7, (int)pos.z + z, brick, true);
                        else
                            curr.setBlockAt((int)pos.x + x, (int)pos.y + y - 7, (int)pos.z + z, "Air", true);
                    }
                }
                if (y >= 7)
                    tmp++;
            }

            CharacterInfo iaInfo = new CharacterInfo("Robot-" + Guid.NewGuid().ToString(), false);

            iaInfo.SpawnPoint = new Vector3(pos.x + _width / 2 - 4, (int)pos.y - 5, (int)pos.z + _width / 2) * API.Generic.Cst.CUBE_SIDE;
            curr.mWorld.getStateMgr().MainState.CharacMgr.AddCharacter(iaInfo);

            iaInfo = new CharacterInfo("Robot-" + Guid.NewGuid().ToString(), false);
            iaInfo.SpawnPoint = new Vector3(pos.x + _width / 2 + 4, (int)pos.y - 5, (int)pos.z + _width / 2) * API.Generic.Cst.CUBE_SIDE;
            curr.mWorld.getStateMgr().MainState.CharacMgr.AddCharacter(iaInfo);


            Portals.makePortal(1, curr, new Vector3((int)pos.x + _width / 2 - 2, (int)pos.y - 7, (int)pos.z + _width / 2 - 3));

            int tmp2 = 0;
            for (int y = 1; y < 6; y++)
            {
                for (int x = 0; x < 2; x++)
                {
                    for (int z = tmp2; z < tmp2 + 2; z++)
                    {
                        curr.setBlockAt((int)pos.x + _width / 2 - 1 + x, (int)pos.y + y - 8, (int)pos.z + _width - 1 + z, brick, true);
                        curr.setBlockAt((int)pos.x + _width / 2 - 1 + x, (int)pos.y + y - 7, (int)pos.z + _width - 1 + z, "Air", true);
                        curr.setBlockAt((int)pos.x + _width / 2 - 1 + x, (int)pos.y + y - 5, (int)pos.z + _width - 1 + z, "Air", true);
                        curr.setBlockAt((int)pos.x + _width / 2 - 1 + x, (int)pos.y + y - 6, (int)pos.z + _width - 1 + z, "Air", true);

                    }
                }
                tmp2++;
            }
        }
    }
}

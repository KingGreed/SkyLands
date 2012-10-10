﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Terrain
{
    class Block
    {
        byte mBlockType;

        public Block(byte blockType) {
            this.mBlockType = blockType;
        }

        public bool isAir() { return (this.mBlockType == 0); }
        public bool isNotAir() { return (this.mBlockType != 0); }

    }
}

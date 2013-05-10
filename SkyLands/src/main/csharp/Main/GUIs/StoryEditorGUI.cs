using System;
using System.Drawing;

using Game.States;

namespace Game.GUICreator
{
    public class StoryEditorGUI : MainGUI
    {
        public enum Selection { Grass, Dirt, Stone, Wood, Leaves, Sand, Cactus, Construction, Glass, Levitator }
        
        public Selection Selector { get { return (Selection)this.mSelectorLoc; } }

        public StoryEditorGUI(StateManager stateMgr) : base(stateMgr, "StoryEditor GUI") { }

        protected override void SetImages()
        {
            this.mBoxes[0].Bitmap = new Bitmap(@"./Media/materials/textures/earth_side.jpg");
            this.mBoxes[1].Bitmap = new Bitmap(@"./Media/materials/textures/dirt.jpg");
            this.mBoxes[2].Bitmap = new Bitmap(@"./Media/materials/textures/stone.jpg");
            this.mBoxes[3].Bitmap = new Bitmap(@"./Media/materials/textures/wood.png");
            this.mBoxes[4].Bitmap = new Bitmap(@"./Media/materials/textures/leaves.png");
            this.mBoxes[5].Bitmap = new Bitmap(@"./Media/materials/textures/sand.jpg");
            this.mBoxes[6].Bitmap = new Bitmap(@"./Media/materials/textures/cactus.png");
            this.mBoxes[7].Bitmap = new Bitmap(@"./Media/materials/textures/constructionBlockSide2.png");
            this.mBoxes[8].Bitmap = new Bitmap(@"./Media/materials/textures/glass.png");
            this.mBoxes[9].Bitmap = new Bitmap(@"./Media/materials/textures/LevitatorTop.png");
        }

        public override string GetMaterial()
        {
            return Enum.GetName(typeof (Selection), (Selection) this.mSelectorLoc);
        }
    }
}

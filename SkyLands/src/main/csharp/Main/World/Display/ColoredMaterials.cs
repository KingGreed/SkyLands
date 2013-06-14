using System;
using Mogre;

namespace Game.World.Display
{
    public static class ColoredMaterials
    {
        public const string RESOURCE_GROUP_NAME = "colors";
        public const string RED = "red", GREEN = "green", BLUE = "blue", YELLOW = "yellow";

        static ColoredMaterials()
        {
            if (ResourceGroupManager.Singleton.ResourceGroupExists(RESOURCE_GROUP_NAME) == false)
                ResourceGroupManager.Singleton.CreateResourceGroup(RESOURCE_GROUP_NAME);

            CreateColoredMaterial(1, 0, 0, RED);
            CreateColoredMaterial(0, 1, 0, GREEN);
            CreateColoredMaterial(0, 0, 1, BLUE);
            CreateColoredMaterial(1, 0.95f, 0.2f, YELLOW);
        }

        public static void Init() {}

        public static string CreateColoredMaterial(float r, float g, float b, string name = "")
        {
            if (name == "") { name = Guid.NewGuid().ToString(); }

            MaterialPtr moMaterial = MaterialManager.Singleton.Create(name, RESOURCE_GROUP_NAME);
            moMaterial.ReceiveShadows = false;
            moMaterial.GetTechnique(0).SetLightingEnabled(true);
            moMaterial.GetTechnique(0).GetPass(0).SetDiffuse(r, g, b, 0);
            moMaterial.GetTechnique(0).GetPass(0).SetAmbient(r, g, b);
            moMaterial.GetTechnique(0).GetPass(0).SetSelfIllumination(r, g, b);
            moMaterial.Dispose();  // dispose pointer, not the material

            return name;
        }
    }
}

using System;

namespace Game
{
    [Serializable]
    public class ResourceConfig
    {
        public string Group;
        public string Directory;
        public string PackFile;

        internal ResourceConfig()
        {
            Group = string.Empty;
            Directory = string.Empty;
            PackFile = string.Empty;
        }
    }
}

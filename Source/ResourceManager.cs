using System;
using System.Collections.Generic;
using System.IO;
using Mogre;

namespace Game
{
    public class ResourceManager
    {
        private string mDefaultGroup;

        internal ResourceManager()
        {
            mDefaultGroup = "General";
        }

        internal bool Startup(string configFile)
        {
            if (!InitResourcePacks(configFile))
                return false;

            InitGroup(mDefaultGroup);
            LoadGroup(mDefaultGroup);
            return true;
        }

        internal void Shutdown()
        {
            if (ResourceGroupManager.Singleton != null)
            {
                ResourceGroupManager.Singleton.UnloadResourceGroup(mDefaultGroup);

                StringVector groups = ResourceGroupManager.Singleton.GetResourceGroups();
                foreach (string group in groups)
                {
                    if (ResourceGroupManager.Singleton.IsResourceGroupLoaded(group))
                        ResourceGroupManager.Singleton.UnloadResourceGroup(group);

                    ResourceGroupManager.Singleton.ClearResourceGroup(group);
                }

                ResourceGroupManager.Singleton.ShutdownAll();
            }
        }

        internal void Update()
        {
        }

        private bool InitResourcePacks(string filename)
        {
            string resPath;

            List<ResourceConfig> resPackList = LoadResourceConfig(filename);

            // Register packs with engine
            foreach (ResourceConfig res in resPackList)
            {
                resPath = res.Directory;    // check directory first
                if (Directory.Exists(resPath))
                {
                    ResourceGroupManager.Singleton.AddResourceLocation(resPath, "FileSystem", res.Group, true);
                    continue;
                }

                resPath = res.PackFile; // check zip pack file next
                if (File.Exists(resPath))
                {
                    ResourceGroupManager.Singleton.AddResourceLocation(resPath, "Zip", res.Group, true);
                    continue;
                }
            }

            return true;
        }

        private List<ResourceConfig> LoadResourceConfig(string filename)
        {
            List<ResourceConfig> resList = new List<ResourceConfig>();

            string[] lines = File.ReadAllLines(filename);

            foreach (string line in lines)
            {
                if (line.Trim().StartsWith("#"))
                    continue;

                string[] fields = line.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                if (fields.Length < 3)
                    continue;

                ResourceConfig res = new ResourceConfig();

                res.Group = fields[0].Trim();
                res.Directory = fields[1].Trim();
                res.PackFile = fields[2].Trim();
                resList.Add(res);
            }

            return resList;
        }

        public void InitGroup(string groupName)
        {
            if (!ResourceGroupManager.Singleton.IsResourceGroupInitialised(groupName))
                ResourceGroupManager.Singleton.InitialiseResourceGroup(groupName);
        }

        public void LoadGroup(string groupName)
        {
            if (!ResourceGroupManager.Singleton.IsResourceGroupLoaded(groupName))
                ResourceGroupManager.Singleton.LoadResourceGroup(groupName);
        }

        public void UnloadGroup(string groupName)
        {
            if (groupName == mDefaultGroup)
                return;

            if (ResourceGroupManager.Singleton.IsResourceGroupLoaded(groupName))
                ResourceGroupManager.Singleton.UnloadResourceGroup(groupName);
        }

        public bool CheckResourceExists(string groupName, string fileName)
        {
            return ResourceGroupManager.Singleton.ResourceExists(groupName, fileName);
        }

        public byte[] ReadResource(string groupName, string fileName)
        {
            if (!CheckResourceExists(groupName, fileName))
                return null;

            DataStreamPtr streamPtr = ResourceGroupManager.Singleton.OpenResource(fileName, groupName);
            uint length = streamPtr.Size();
            uint readLength = 0;

            byte[] buffer = new byte[(int)length];
            if (length != 0)
            {
                unsafe
                {
                    fixed (byte* bufferPtr = &buffer[0])
                    {
                        readLength = streamPtr.Read(bufferPtr, length);
                    }
                }
            }

            if (readLength != length)
                buffer = null;

            streamPtr.Close();

            return buffer;
        }

        public string[] GetResourceGroups()
        {
            StringVector groups = ResourceGroupManager.Singleton.GetResourceGroups();

            string[] res = new string[groups.Count];

            for (int i = 0; i < groups.Count; ++i)
                res[i] = groups[i];

            return res;
        }

        public string[] GetResourcesInGroup(string group)
        {
            FileInfoList resources = ResourceGroupManager.Singleton.ListResourceFileInfo(group);

            string[] res = new string[resources.Count];

            for (int i = 0; i < resources.Count; ++i)
            {
                FileInfo_NativePtr resource = resources[i];
                res[i] = resource.filename;
            }

            return res;
        }
    }
}

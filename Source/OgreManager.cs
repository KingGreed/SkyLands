using System;
using System.Collections.Generic;
using System.IO;
using Mogre;

namespace Game
{
    public struct WindowInfo
    {
        public string name;
        public uint width, height;
        public bool isFullScreen;
        public NameValuePairList param;

        public WindowInfo(string givenName, int givenWidth, int givenHeight, bool givenIsFullScreen, NameValuePairList givenParam)
        {
            name = givenName;
            width = (uint) givenWidth;
            height = (uint) givenHeight;
            isFullScreen = givenIsFullScreen;
            param = givenParam;
        }
    }
    
    public class OgreManager
    {
        private Root mRoot;
        private RenderWindow mWindow;
        private SceneManager mSceneMgr;
        private Camera mCamera;
        private Viewport mViewport;
        private bool mRenderingActive;
        private ResourceManager mResourceMgr;
        private IntPtr mWindowHandle;

        public bool RenderingActive
        {
            get { return mRenderingActive; }
        }

        public RenderWindow Window
        {
            get { return mWindow; }
        }

        public IntPtr WindowHandle
        {
            get { return mWindowHandle; }
        }

        public SceneManager SceneMgr
        {
            get { return mSceneMgr; }
        }

        public Camera Camera
        {
            get { return mCamera; }
        }

        // events raised when direct 3D device is lost or restored
        public event EventHandler<OgreEventArgs> DeviceLost;
        public event EventHandler<OgreEventArgs> DeviceRestored;

        internal OgreManager()
        {
            mRoot = null;
            mWindow = null;
            mSceneMgr = null;
            mCamera = null;
            mViewport = null;
            mRenderingActive = false;
            mResourceMgr = null;
            mWindowHandle = IntPtr.Zero;
        }

        internal bool Startup(WindowInfo windowInfo)
        {
            if (mRoot != null)
                return false;

            mRoot = new Root("plugins.cfg", "settings.cfg", "mogre.log");

            RenderSystem renderSys = mRoot.GetRenderSystemByName("Direct3D9 Rendering Subsystem");
            mRoot.RenderSystem = renderSys;


            mRoot.RenderSystem.EventOccurred += OnRenderSystemEventOccurred;    // register event to get notified when application lost or regained focus

            mRoot.Initialise(false);

            mWindow = mRoot.CreateRenderWindow(windowInfo.name, windowInfo.width, windowInfo.height, windowInfo.isFullScreen, windowInfo.param);
            mWindow.GetCustomAttribute("WINDOW", out mWindowHandle);

            mSceneMgr = mRoot.CreateSceneManager(SceneType.ST_GENERIC, "DefaultSceneManager");

            mCamera = mSceneMgr.CreateCamera("DefaultCamera");
            mCamera.AutoAspectRatio = true;
            mCamera.NearClipDistance = 1.0f;
            mCamera.FarClipDistance = 1000.0f;

            mViewport = mWindow.AddViewport(mCamera);

            mResourceMgr = new ResourceManager();
            if (!mResourceMgr.Startup("../Release/resources.cfg"))
                return false;

            mRenderingActive = true;

            return true;
        }

        internal void Shutdown()
        {
            if (mResourceMgr != null)
            {
                mResourceMgr.Shutdown();
                mResourceMgr = null;
            }

            if (mRoot != null)
            {
                // deregister event to get notified when application lost or regained focus
                mRoot.RenderSystem.EventOccurred -= OnRenderSystemEventOccurred;

                mRoot.Dispose();
            }
            mRoot = null;

            mWindowHandle = IntPtr.Zero;
            mWindow = null;
            mSceneMgr = null;
            mCamera = null;
            mViewport = null;
            mRenderingActive = false;
        }

        internal void Update()
        {
            if (mRoot == null)
                return;

            // process windows event queue (only if no external window is used)
            WindowEventUtilities.MessagePump();

            // render next frame
            if (mRenderingActive)
                mRoot.RenderOneFrame();
        }

        private void OnRenderSystemEventOccurred(string eventName, Const_NameValuePairList parameters)
        {
            EventHandler<OgreEventArgs> evt = null;
            OgreEventArgs args;

            switch (eventName)
            {
                case "DeviceLost":
                    evt = DeviceLost;

                    args = new OgreEventArgs();
                    break;

                case "DeviceRestored":
                    uint width;
                    uint height;
                    uint depth;

                    evt = DeviceRestored;

                    mWindow.GetMetrics(out width, out height, out depth);

                    args = new OgreEventArgs((int)width, (int)height);
                    break;

                default:
                    return;
            }

            if (evt != null)
                evt(this, args);
        }

        internal SceneNode CreateSimpleObject(string name, string mesh)
        {
            if (mSceneMgr.HasEntity(name) || mSceneMgr.HasSceneNode(name))
                return null;

            Entity entity;
            try
            {
                entity = mSceneMgr.CreateEntity(name, mesh);
            }
            catch
            {
                return null;
            }

            SceneNode node = mSceneMgr.CreateSceneNode(name);

            node.AttachObject(entity);

            return node;
        }

        internal void DestroyObject(SceneNode node)
        {
            if (node.Parent != null)
                node.Parent.RemoveChild(node);

            // first remove all child nodes (they are not destroyed here !)
            node.RemoveAllChildren();

            // create a list of references to attached objects
            List<MovableObject> objList = new List<MovableObject>();

            // get number of attached objects
            ushort count = node.NumAttachedObjects();

            // get all attached objects references
            for (ushort i = 0; i < count; ++i)
                objList.Add(node.GetAttachedObject(i));

            // detach all objects from node
            node.DetachAllObjects();

            // destroy all previously attached objects
            foreach (MovableObject obj in objList)
                mSceneMgr.DestroyMovableObject(obj);

            // destroy scene node
            mSceneMgr.DestroySceneNode(node);
        }

        internal void AddObjectToScene(SceneNode node)
        {
            if (node.Parent != null)
            {
                if (node.Parent == mSceneMgr.RootSceneNode)
                    return;

                node.Parent.RemoveChild(node);
            }

            mSceneMgr.RootSceneNode.AddChild(node);
        }

        internal void AddObjectToObject(SceneNode node, SceneNode newParent)
        {
            if (node.Parent != null)
            {
                if (node.Parent == newParent)
                    return;

                node.Parent.RemoveChild(node);
            }

            newParent.AddChild(node);
        }

        internal void RemoveObjectFromScene(SceneNode node)
        {
            if (node.Parent != null)
                node.Parent.RemoveChild(node);
        }
    }
}

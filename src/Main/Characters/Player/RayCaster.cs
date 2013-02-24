using System;
using System.Collections.Generic;
using Mogre;

using Game.IGConsole;

namespace Game.CharacSystem
{
    class RayCaster
    {
        private uint mWndWidth, mWndHeight;
        private Camera mCam;
        private RaySceneQuery mRayQuery;

        public Camera Camera { get { return this.mCam; } }

        public RayCaster(SceneManager sceneMgr, Camera cam, uint wndWidth, uint wndHeight)
        {
            this.mCam = cam;
            this.mWndWidth = wndWidth;
            this.mWndHeight = wndHeight;


            this.mRayQuery = sceneMgr.CreateRayQuery(new Ray(), SceneManager.WORLD_GEOMETRY_TYPE_MASK);
            this.mRayQuery.SetSortByDistance(true);
        }

        public bool CastRay(MyConsole console, Ray ray, SceneNode startingNode, out float dist, out Vector3 position)    // set the first position entity in position
        {
            this.mRayQuery.Ray = ray;
            dist = 0;
            position = Vector3.ZERO;

            //console.WriteLine(this.mRayQuery.Ray.Direction);

            RaySceneQueryResult rayResult = this.mRayQuery.Execute();
            if (rayResult.Count <= 0) // raycast did not hit an objects bounding box
                return false;

            //RaySceneQueryResult results = this.mRayQuery.GetLastResults(;

            foreach(RaySceneQueryResultEntry result in rayResult)
            {
                if (result.distance > 0 && result.movable.MovableType == "ManualObject")
                {
                    dist = result.distance;
                    position = this.mRayQuery.Ray.GetPoint(dist);
                    return true;
                }
            }

            return false;
        }
    }
}

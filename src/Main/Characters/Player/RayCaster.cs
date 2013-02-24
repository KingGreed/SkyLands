using System;
using System.Collections.Generic;
using Mogre;

namespace Game.CharacSystem
{
    class RayCaster
    {
        private uint mWndWidth, mWndHeight;
        private Camera mCam;
        private RaySceneQuery mRayQuery;

        public RayCaster(SceneManager sceneMgr, Camera cam, uint wndWidth, uint wndHeight)
        {
            this.mCam = cam;
            this.mWndWidth = wndWidth;
            this.mWndHeight = wndHeight;


            this.mRayQuery = sceneMgr.CreateRayQuery(new Ray(), SceneManager.WORLD_GEOMETRY_TYPE_MASK);
            this.mRayQuery.SetSortByDistance(true);
        }

        public bool IsBlockInRay(SceneManager sceneMgr, SceneNode startingNode, float maxDist, out Vector3 position)    // set the first position entity in position
        {
            Ray ray = mCam.GetCameraToViewportRay(0.5f, 0.5f);
            this.mRayQuery.Ray = ray;
            position = Vector3.ZERO;

            RaySceneQueryResult rayresult = this.mRayQuery.Execute();
            if (rayresult.Count <= 0) // raycast did not hit an objects bounding box
                return false;

            //RaySceneQueryResult results = this.mRayQuery.GetLastResults();

            foreach(RaySceneQueryResultEntry result in rayresult)
            {
                if (result.distance > 0 && result.distance <= maxDist && result.movable.MovableType == "ManualObject")
                {
                    position = ray.GetPoint(result.distance);
                    return true;
                }
            }

            return false;

            //results.RemoveLast();
            /*while(results.Count > 0 && results.Front.distance == 0)
                results.Erase(0);*/

            //foreach (RaySceneQueryResultEntry result in results)
            //{
                // stop checking if we have found a raycast hit that is closer
                // than all remaining entities
                //if ((closestDistance >= 0.0f) && (closestDistance < result.distance)) { break; }
                

                // only check this result if its a hit against an entity
                /*if ((result.movable != null) && (result.movable.MovableType == "Entity"))
                {
                    Entity pentity = (Entity)result.movable; // get the entity to check

                    /*bool continueLoop = false;
                    foreach(Entity ent in startingNode.GetAttachedObjectIterator())
                        if (ent.Name == pentity.Name) { continueLoop = true; break; }
                    if (continueLoop) { continue; }*

                    // mesh data to retrieve 
                    uint vertex_count = 0;
                    uint index_count = 0;
                    Vector3[] vertices = new Vector3[0];
                    UInt64[] indices = new UInt64[0];

                    // get the mesh information
                    GetMeshInformation(pentity.GetMesh(),
                        ref vertex_count, ref vertices, ref index_count, ref indices,
                        pentity.ParentNode._getDerivedPosition(),    // WorldPosition
                        pentity.ParentNode._getDerivedOrientation(), // WorldOrientation
                        pentity.ParentNode.GetScale());

                    int ncf = -1; // new_closest_found

                    // test for hitting individual triangles on the mesh
                    for (int i = 0; i < (int)index_count; i += 3)
                    {
                        // check for a hit against this triangle
                        Pair<bool, float> hit = Mogre.Math.Intersects(ray, vertices[indices[i]],
                            vertices[indices[i + 1]], vertices[indices[i + 2]], true, false);

                        // if it was a hit check if its the closest
                        if (hit.first)
                        {
                            if ((closestDistance < 0.0f) ||
                                (hit.second < closestDistance))
                            {
                                // this is the closest so far, save it off
                                closestDistance = hit.second;
                                ncf = i;
                            }
                        }
                    }

                    if (ncf > -1)
                        closestResult = ray.GetPoint(closestDistance);

                    // free the verticies and indicies memory
                    vertices = null;
                    indices = null;
                }
            }

            //  Udate the result if we found a new closest raycast for this object.
            if (closestDistance >= 0.0f)
            {
                position = new Vector3(closestResult.x, closestResult.y, closestResult.z);
                return true;
            }
            else { return false; }*/
            }
        }
}

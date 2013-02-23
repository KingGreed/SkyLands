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
        
        public bool RayCast(SceneNode startingNode, ref Vector3 position, ref Vector3 normalPos)
        {
            Ray ray = mCam.GetCameraToViewportRay(0.5f, 0.5f);
            this.mRayQuery.Ray = ray;

            RaySceneQueryResult rayresult = this.mRayQuery.Execute();
            if (rayresult.Count <= 0) // raycast did not hit an objects bounding box
                return false;

            float closestDistance = -1.0f;
            Vector3 closestResult = Vector3.ZERO, vNormal = Vector3.ZERO;

            foreach (RaySceneQueryResultEntry result in this.mRayQuery.GetLastResults())
            {
                // stop checking if we have found a raycast hit that is closer
                // than all remaining entities
                if ((closestDistance >= 0.0f) && (closestDistance < result.distance)) { break; }

                // only check this result if its a hit against an entity
                if ((result.movable != null) && (result.movable.MovableType == "Entity"))
                {
                    Entity pentity = (Entity)result.movable; // get the entity to check

                    bool continueLoop = false;
                    foreach(Entity ent in startingNode.GetAttachedObjectIterator())
                        if (ent.Name == pentity.Name) { continueLoop = true; break; }
                    if (continueLoop) { continue; }

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
                    {
                        closestResult = ray.GetPoint(closestDistance);
                        // if you don't need the normal, comment this out; you'll save some CPU cycles.
                        Vector3 v1 = vertices[indices[ncf]] - vertices[indices[ncf + 1]];
                        Vector3 v2 = vertices[indices[ncf + 2]] - vertices[indices[ncf + 1]];
                        vNormal = v1.CrossProduct(v2);
                    }

                    // free the verticies and indicies memory
                    vertices = null;
                    indices = null;
                }
            }

            // if we found a new closest raycast for this object, update the
            // closest_result before moving on to the next object.
            if (closestDistance >= 0.0f)
            {
                position = new Vector3(closestResult.x, closestResult.y, closestResult.z);
                normalPos = vNormal / vNormal.Normalise();

                return true;
            }
            else { return false; }
        }

        public unsafe void GetMeshInformation(MeshPtr mesh, ref uint vertex_count, ref Vector3[] vertices, ref uint index_count,
                                              ref UInt64[] indices, Vector3 position, Quaternion orientation, Vector3 scale)
        {
            bool added_shared = false;
            uint current_offset = 0;
            uint shared_offset = 0;
            uint next_offset = 0;
            uint index_offset = 0;

            vertex_count = index_count = 0;

            // Calculate how many vertices and indices we're going to need
            for (ushort i = 0; i < mesh.NumSubMeshes; ++i)
            {
                SubMesh submesh = mesh.GetSubMesh(i);

                // We only need to add the shared vertices once
                if (submesh.useSharedVertices)
                {
                    if (!added_shared)
                    {
                        vertex_count += mesh.sharedVertexData.vertexCount;
                        added_shared = true;
                    }
                }
                else
                    vertex_count += submesh.vertexData.vertexCount;

                // Add the indices
                index_count += submesh.indexData.indexCount;
            }

            // Allocate space for the vertices and indices
            vertices = new Vector3[vertex_count];
            indices = new UInt64[index_count];
            added_shared = false;

            // Run through the submeshes again, adding the data into the arrays
            for (ushort i = 0; i < mesh.NumSubMeshes; ++i)
            {
                SubMesh submesh = mesh.GetSubMesh(i);
                VertexData vertex_data = submesh.useSharedVertices ? mesh.sharedVertexData : submesh.vertexData;

                if (!submesh.useSharedVertices || (submesh.useSharedVertices && !added_shared))
                {
                    if (submesh.useSharedVertices)
                    {
                        added_shared = true;
                        shared_offset = current_offset;
                    }

                    VertexElement posElem =
                        vertex_data.vertexDeclaration.FindElementBySemantic(VertexElementSemantic.VES_POSITION);
                    HardwareVertexBufferSharedPtr vbuf =
                        vertex_data.vertexBufferBinding.GetBuffer(posElem.Source);

                    byte* vertex = (byte*)vbuf.Lock(HardwareBuffer.LockOptions.HBL_READ_ONLY);
                    float* pReal;

                    for (int j = 0; j < vertex_data.vertexCount; ++j, vertex += vbuf.VertexSize)
                    {
                        posElem.BaseVertexPointerToElement(vertex, &pReal);
                        Vector3 pt = new Vector3(pReal[0], pReal[1], pReal[2]);
                        vertices[current_offset + j] = (orientation * (pt * scale)) + position;
                    }
                    
                    vbuf.Unlock();
                    vbuf.Dispose();
                    next_offset += vertex_data.vertexCount;
                }

                IndexData index_data = submesh.indexData;
                uint numTris = index_data.indexCount / 3;
                HardwareIndexBufferSharedPtr ibuf = index_data.indexBuffer;

                // UNPORTED line of C++ code (because ibuf.IsNull() doesn't exist in C#)
                // if( ibuf.isNull() ) continue
                // need to check if index buffer is valid (which will be not if the mesh doesn't have triangles like a pointcloud)

                bool use32bitindexes = (ibuf.Type == HardwareIndexBuffer.IndexType.IT_32BIT);

                uint* pLong = (uint*)ibuf.Lock(HardwareBuffer.LockOptions.HBL_READ_ONLY);
                ushort* pShort = (ushort*)pLong;
                uint offset = submesh.useSharedVertices ? shared_offset : current_offset;
                if (use32bitindexes)
                {
                    for (int k = 0; k < index_data.indexCount; ++k)
                        indices[index_offset++] = (UInt64)pLong[k] + (UInt64)offset;
                }
                else
                {
                    for (int k = 0; k < index_data.indexCount; ++k)
                        indices[index_offset++] = (UInt64)pShort[k] + (UInt64)offset;
                }
                
                ibuf.Unlock();
                ibuf.Dispose();
                current_offset = next_offset;
            }

            mesh.Dispose();
        }
    }
}

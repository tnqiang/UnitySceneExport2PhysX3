using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

namespace UnityPhysXExport
{
    public static class ExportScene  {


        [StructLayout(LayoutKind.Sequential)]
        struct PxTransform
        {
            public Quaternion q;
            public Vector3 p;
        }

        enum PxGeometryType
        {
            eSPHERE,
            ePLANE,
            eCAPSULE,
            eBOX,
            eCONVEXMESH,
            eTRIANGLEMESH,
            eHEIGHTFIELD,

            eGEOMETRY_COUNT,	//!< internal use only!
            eINVALID = -1,	//!< internal use only!
        };

        enum PxShapeFlag
    	{
    		/**
    		\brief The shape will partake in collision in the physical simulation.

    		\note It is illegal to raise the eSIMULATION_SHAPE and eTRIGGER_SHAPE flags.
    		In the event that one of these flags is already raised the sdk will reject any 
    		attempt to raise the other.  To raise the eSIMULATION_SHAPE first ensure that 
    		eTRIGGER_SHAPE is already lowered.

    		\note This flag has no effect if simulation is disabled for the corresponding actor (see #PxActorFlag::eDISABLE_SIMULATION).

    		@see PxSimulationEventCallback.onContact() PxScene.setSimulationEventCallback() PxShape.setFlag(), PxShape.setFlags()
    		*/
    		eSIMULATION_SHAPE				= (1<<0),

    		/**
    		\brief The shape will partake in scene queries (ray casts, overlap tests, sweeps, ...).
    		*/
    		eSCENE_QUERY_SHAPE				= (1<<1),

    		/**
    		\brief The shape is a trigger which can send reports whenever other shapes enter/leave its volume.

    		\note Triangle meshes and heightfields can not be triggers. Shape creation will fail in these cases.

    		\note Shapes marked as triggers do not collide with other objects. If an object should act both
    		as a trigger shape and a collision shape then create a rigid body with two shapes, one being a 
    		trigger shape and the other a collision shape. 	It is illegal to raise the eTRIGGER_SHAPE and 
    		eSIMULATION_SHAPE flags on a single PxShape instance.  In the event that one of these flags is already 
    		raised the sdk will reject any attempt to raise the other.  To raise the eTRIGGER_SHAPE flag first 
    		ensure that eSIMULATION_SHAPE flag is already lowered.

    		\note Shapes marked as triggers are allowed to participate in scene queries, provided the eSCENE_QUERY_SHAPE flag is set. 

    		\note This flag has no effect if simulation is disabled for the corresponding actor (see #PxActorFlag::eDISABLE_SIMULATION).

    		@see PxSimulationEventCallback.onTrigger() PxScene.setSimulationEventCallback() PxShape.setFlag(), PxShape.setFlags()
    		*/
    		eTRIGGER_SHAPE					= (1<<2),

    		/**
    		\brief Enable debug renderer for this shape

    		@see PxScene.getRenderBuffer() PxRenderBuffer PxVisualizationParameter
    		*/
    		eVISUALIZATION					= (1<<3),

    		/**
    		\brief Sets the shape to be a particle drain.
    		*/
    		ePARTICLE_DRAIN					= (1<<4)
    	};

        [StructLayout(LayoutKind.Sequential)]
        struct PxBoxGeometry 
        {
            public PxGeometryType mType; 
            public Vector3 halfExtents;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct PxSphereGeometry
        {
            public PxGeometryType mType;
            public float radius;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct PxCapsuleGeometry
        {
            public PxGeometryType mType;
            public float radius;
            public float halfHeight;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct PxFilterData
        {
    	    public Int32 word0;
            public Int32 word1;
            public Int32 word2;
            public Int32 word3;
        };

        [DllImport("PxSerialization")]
        static extern void initPhysics();

        [DllImport("PxSerialization")]
        static extern void cleanupPhysics();

        [DllImport("PxSerialization")]
        static extern IntPtr createCollection();

        [DllImport("PxSerialization")]
        static extern void addCollectionObject(IntPtr collection, IntPtr obj, Int64 id);

        [DllImport("PxSerialization")]
        static extern IntPtr createMaterial(float staticFriction, float dynamicFriction, float restitution);

        [DllImport("PxSerialization")]
        static extern IntPtr createShape(IntPtr  geometry, IntPtr material);

        [DllImport("PxSerialization")]
        static extern void setOwnerClient(IntPtr actor, int inClient);

        [DllImport("PxSerialization")]
        static extern void setName(IntPtr actor, IntPtr name);

        [DllImport("PxSerialization")]
        static extern IntPtr createMeshShape(IntPtr points, int point_count, IntPtr triangles, int triangle_count, bool convex, IntPtr material);

        [DllImport("PxSerialization")]
        static extern void attachShape(IntPtr actor, IntPtr shape);

        [DllImport("PxSerialization")]
        static extern void detachShape(IntPtr actor, IntPtr shape);

        [DllImport("PxSerialization")]
        static extern void setShapeFlag(IntPtr shape, PxShapeFlag flag, bool value);

        [DllImport("PxSerialization")]
        static extern void setLocalPose(IntPtr shape, IntPtr transform);

        [DllImport("PxSerialization")]
        static extern IntPtr createStatic(IntPtr transform, IntPtr geometry, IntPtr material);

        [DllImport("PxSerialization")]
        static extern IntPtr createDynamic(IntPtr transform, IntPtr shape, float density);

        [DllImport("PxSerialization")]
        static extern IntPtr createRigidStatic(IntPtr transform);

        [DllImport("PxSerialization")]
        static extern IntPtr createRigidDynamic(IntPtr transform, float mass, float drag, float angularDrag, bool useGravity, bool isKinematic);

        [DllImport("PxSerialization")]
        static extern void complete(IntPtr collection, IntPtr exceptFor, bool followJoints);

        [DllImport("PxSerialization")]
        static extern void serializeCollection(IntPtr collection, IntPtr externalRefs, IntPtr filename, Boolean toBinary);

        [DllImport("PxSerialization")]
        static extern void release(IntPtr p);

        [DllImport("PxSerialization")]
        static extern void setShapeName(IntPtr shape, IntPtr name);

        [DllImport("PxSerialization")]
        static extern void setShapeContactOffset(IntPtr shape, float offset);

        [DllImport("PxSerialization")]
        static extern void setShapeQueryFilterData(IntPtr shape, IntPtr data);

        [DllImport("PxSerialization")]
        static extern void setShapeSimulationFilterData(IntPtr shape, IntPtr data);

        static void SetShapeQueryFilterData(IntPtr shape, PxFilterData data)
        {
            IntPtr dataPtr = Marshal.AllocHGlobal(Marshal.SizeOf(data));
            Marshal.StructureToPtr(data, dataPtr, false);
            setShapeQueryFilterData(shape, dataPtr);
            Marshal.FreeHGlobal(dataPtr);
        }

        static void SetShapeQueryFilterData(IntPtr shape, Collider collider)
        {
            PxFilterData data;
            int layer = collider.gameObject.layer;
            data.word0 = 1 << layer;
            data.word1 = collider.isTrigger ? 1 : 0;
            data.word2 = data.word3 = 0;
            SetShapeQueryFilterData(shape, data);
        }

        static private Dictionary<Vector3, IntPtr> m_materialCache = new Dictionary<Vector3, IntPtr>();
        static IntPtr CreateMaterial(PhysicMaterial material)
        {
            Vector3 key = new Vector3(material.staticFriction, material.dynamicFriction, material.bounciness);
            IntPtr ptr;
            if (m_materialCache.TryGetValue(key, out ptr))
            {
                Debug.Log("CreateMaterial got cached material");
                return ptr;
            }
            ptr = createMaterial(material.staticFriction, material.dynamicFriction, material.bounciness);
            m_materialCache.Add(key, ptr);
            return ptr;
        }

        static IntPtr CreateRigidDynamic(Rigidbody rigidBody)
        {
            Transform transform = rigidBody.transform;
            PxTransform pxTransform;
            pxTransform.p = transform.position;
            pxTransform.q = transform.rotation;
            IntPtr transformPtr = Marshal.AllocHGlobal(Marshal.SizeOf(pxTransform));
            Marshal.StructureToPtr(pxTransform, transformPtr, false);

            IntPtr rigidDynamic = createRigidDynamic(transformPtr, rigidBody.mass, rigidBody.drag, rigidBody.angularDrag, rigidBody.useGravity, rigidBody.isKinematic);

            Marshal.FreeHGlobal(transformPtr);
            return rigidDynamic;
        }

        static IntPtr CreateRigidStatic(Transform transform)
        {
            PxTransform pxTransform;
            pxTransform.p = transform.position;
            pxTransform.q = transform.rotation;
            IntPtr transformPtr = Marshal.AllocHGlobal(Marshal.SizeOf(pxTransform));
            Marshal.StructureToPtr(pxTransform, transformPtr, false);

            IntPtr rigidDynamic = createRigidStatic(transformPtr);

            Marshal.FreeHGlobal(transformPtr);
            return rigidDynamic;
        }

        static void SetLocalPose(IntPtr shape, PxTransform pxTransform)
        {
            IntPtr transformPtr = Marshal.AllocHGlobal(Marshal.SizeOf(pxTransform));
            Marshal.StructureToPtr(pxTransform, transformPtr, false);

            setLocalPose(shape, transformPtr);

            Marshal.FreeHGlobal(transformPtr);
        }

        
        static void SetShapeName(Collider collider, IntPtr shape)
        {
            String name = collider.name;
            setShapeName(shape, Marshal.StringToHGlobalAnsi(name));
        }

        // PxController
        static void ExportCharaterController(IntPtr collection, CharacterController characterController)
        {
            // TODO
        }

        /// <summary>
        /// 功能函数，获取 hierarchy 名字
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        static string GetGameObjectNameInHierarchy(Transform go)
        {
            if (go == null) return string.Empty;
            else
            {
                return GetGameObjectNameInHierarchy(go.parent) + "/" + go.name;
            }
        }

        // PxShape with PxBoxGeometry
        static IntPtr CreateBoxCollider(BoxCollider boxCollider)
        {
            PxBoxGeometry geo;
            geo.mType = PxGeometryType.eBOX;

            geo.halfExtents.x = boxCollider.size.x * Mathf.Abs(boxCollider.transform.lossyScale.x) / 2.0f;
            geo.halfExtents.y = boxCollider.size.y * Mathf.Abs(boxCollider.transform.lossyScale.y) / 2.0f;
            geo.halfExtents.z = boxCollider.size.z * Mathf.Abs(boxCollider.transform.lossyScale.z) / 2.0f;
            if(boxCollider.size .x < 0 || boxCollider.size.y < 0 || boxCollider.size.z < 0)
            {
                Debug.LogError(string.Format("BoxCollider {0} 的大小为负数, GameObject Name: {1}，请检查", boxCollider.name, GetGameObjectNameInHierarchy(boxCollider.transform)));
            }

            IntPtr geoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(geo));
            Marshal.StructureToPtr(geo, geoPtr, false);

            IntPtr shape = createShape(geoPtr, CreateMaterial(boxCollider.material));
            Marshal.FreeHGlobal(geoPtr);

            /*
             * Trigger shapes play no part in the simulation of the scene (though they can be configured to participate in scene queries). 
             * Instead, their role is to report that there has been an overlap with another shape. Contacts are not generated for the intersection,
             * and as a result contact reports are not available for trigger shapes. Further, because triggers play no part in the simulation,
             * the SDK will not allow the the eSIMULATION_SHAPE eTRIGGER_SHAPE flags to be raised simultaneously; that is, 
             * if one flag is raised then attempts to raise the other will be rejected, and an error will be passed to the error stream.
             */
            if (boxCollider.isTrigger)
            {
                setShapeFlag(shape, PxShapeFlag.eSIMULATION_SHAPE, false);
            }
            setShapeFlag(shape, PxShapeFlag.eTRIGGER_SHAPE, boxCollider.isTrigger);
            SetShapeName(boxCollider, shape);
            setShapeContactOffset(shape, boxCollider.contactOffset);
            SetShapeQueryFilterData(shape, boxCollider);
            return shape;
        }

        // PxShape with PxSphereGeometry
        static IntPtr CreateSphereCollider(SphereCollider sphereCollider)
        {
            PxSphereGeometry geo;
            geo.mType = PxGeometryType.eSPHERE;
            float max_scale = Math.Max(Math.Max(Mathf.Abs(sphereCollider.transform.localScale.x),
                Mathf.Abs(sphereCollider.transform.localScale.y)),
                Mathf.Abs(sphereCollider.transform.localScale.z));
            geo.radius = sphereCollider.radius * max_scale;

            IntPtr geoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(geo));
            Marshal.StructureToPtr(geo, geoPtr, false);

            IntPtr shape = createShape(geoPtr, CreateMaterial(sphereCollider.material));
            Marshal.FreeHGlobal(geoPtr);
            if (sphereCollider.isTrigger)
            {
                setShapeFlag(shape, PxShapeFlag.eSIMULATION_SHAPE, false);
            }
            setShapeFlag(shape, PxShapeFlag.eTRIGGER_SHAPE, sphereCollider.isTrigger);
            SetShapeName(sphereCollider, shape);
            setShapeContactOffset(shape, sphereCollider.contactOffset);
            SetShapeQueryFilterData(shape, sphereCollider);
            return shape;
        }

        // PxShape with PxCapsuleGeometry
        static IntPtr CreateCapsuleCollider(CapsuleCollider capsuleCollider)
        {
            PxCapsuleGeometry geo;
            geo.mType = PxGeometryType.eCAPSULE;
            geo.radius = capsuleCollider.radius * Mathf.Max(Mathf.Abs(capsuleCollider.transform.localScale.x), Mathf.Abs(capsuleCollider.transform.localScale.z));
            geo.halfHeight = capsuleCollider.height * capsuleCollider.transform.localScale.y / 2.0f - geo.radius;
            if (geo.halfHeight <= 0)
            {
                Debug.LogWarning(string.Format("CapsuleCollider {0} 的大小为负数, GameObject Name: {1}，将设置为0.001", capsuleCollider.name, GetGameObjectNameInHierarchy(capsuleCollider.transform)));
                geo.halfHeight = 0.001f;
            }

            IntPtr geoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(geo));
            Marshal.StructureToPtr(geo, geoPtr, false);

            IntPtr shape = createShape(geoPtr, CreateMaterial(capsuleCollider.material));
            Marshal.FreeHGlobal(geoPtr);
            if (capsuleCollider.isTrigger)
            {
                setShapeFlag(shape, PxShapeFlag.eSIMULATION_SHAPE, false);
            }
            setShapeFlag(shape, PxShapeFlag.eTRIGGER_SHAPE, capsuleCollider.isTrigger);
            SetShapeName(capsuleCollider, shape);
            setShapeContactOffset(shape, capsuleCollider.contactOffset);
            SetShapeQueryFilterData(shape, capsuleCollider);
            return shape;
        }

        unsafe static IntPtr CreateMeshCollider(MeshCollider meshCollider)
        {
            Mesh mesh = meshCollider.sharedMesh;
            if (mesh == null || mesh.vertices.Length == 0)
            {
                Debug.LogError(meshCollider + "上面的mesh丢失了，请检查");
                return IntPtr.Zero;
            }
            Vector3 scale = meshCollider.transform.lossyScale;
            Vector3[] meshVertices = new Vector3[mesh.vertices.Length];
            for(int i=0; i<mesh.vertices.Length; ++i)
            {
                Vector3 vertice = mesh.vertices[i];
                vertice.x *= scale.x;
                vertice.y *= scale.y;
                vertice.z *= scale.z;
                meshVertices[i] = vertice;
            }

            IntPtr shape = IntPtr.Zero;
            fixed (Vector3* vertices = meshVertices)
            {
                fixed (int* triangles = mesh.triangles)
                {
                    shape = createMeshShape((IntPtr)vertices, mesh.vertexCount, (IntPtr)triangles, mesh.triangles.Length/3, meshCollider.convex, CreateMaterial(meshCollider.material));
                }
            }

            if (shape == IntPtr.Zero)
            {
                Debug.LogError(string.Format("Export {0} Error", GetGameObjectNameInHierarchy(meshCollider.gameObject.transform)));
                return IntPtr.Zero;
            }

            if (meshCollider.isTrigger)
            {
                setShapeFlag(shape, PxShapeFlag.eSIMULATION_SHAPE, false);
            }
            setShapeFlag(shape, PxShapeFlag.eTRIGGER_SHAPE, meshCollider.isTrigger);
            SetShapeName(meshCollider, shape);
            setShapeContactOffset(shape, meshCollider.contactOffset);
            SetShapeQueryFilterData(shape, meshCollider);
            return shape;
        }

        static bool CanExport(GameObject go)
        {
            return go.activeInHierarchy;
        }

        static List<IntPtr> CreatePxShapes(GameObject go)
        {
            List<IntPtr> shapes = new List<IntPtr>();
            //Debug.Log("CreatePxShapes got name " + go.name);

            Transform rootTransform = go.transform.root;
            Transform goTransform = go.transform;

            BoxCollider[] boxColliders = go.GetComponents<BoxCollider>();
            foreach (BoxCollider boxCollider in boxColliders)
            {
                IntPtr shape = CreateBoxCollider(boxCollider);
                PxTransform transform;
                transform.p = rootTransform.InverseTransformPoint(goTransform.TransformPoint(boxCollider.center));
                transform.q = Quaternion.Inverse(rootTransform.rotation) * goTransform.rotation;
                SetLocalPose(shape, transform);
                shapes.Add(shape);
            }

            SphereCollider[] sphereColliders = go.GetComponents<SphereCollider>();
            foreach (SphereCollider sphereCollider in sphereColliders)
            {
                IntPtr shape = CreateSphereCollider(sphereCollider);
                PxTransform transform;
                transform.p = rootTransform.InverseTransformPoint(goTransform.TransformPoint(sphereCollider.center));
                transform.q = Quaternion.Inverse(rootTransform.rotation) * goTransform.rotation;
                SetLocalPose(shape, transform);
                shapes.Add(shape);
            }

            CapsuleCollider[] capsuleColliders = go.GetComponents<CapsuleCollider>();
            foreach (CapsuleCollider capsuleCollider in capsuleColliders)
            {
                IntPtr shape = CreateCapsuleCollider(capsuleCollider);
                PxTransform transform;
                transform.p = rootTransform.InverseTransformPoint(goTransform.TransformPoint(capsuleCollider.center));
                transform.q = Quaternion.Inverse(rootTransform.rotation) * goTransform.rotation * Quaternion.AngleAxis(90, new Vector3(0, 0, 1));
                SetLocalPose(shape, transform);
                shapes.Add(shape);
            }

            MeshCollider[] meshColliders = go.GetComponents<MeshCollider>();
            foreach (MeshCollider meshCollider in meshColliders)
            {
                IntPtr shape = CreateMeshCollider(meshCollider);
                if(shape != IntPtr.Zero)
                {
                    PxTransform transform;
                    transform.p = rootTransform.InverseTransformPoint(goTransform.position);
                    transform.q = Quaternion.Inverse(rootTransform.rotation) * goTransform.rotation;
                    SetLocalPose(shape, transform);
                    shapes.Add(shape);
                }
            }

            for (int i = 0; i < go.transform.childCount; i++)
            {
                if (CanExport(go.transform.GetChild(i).gameObject))
                {
                    shapes.AddRange(CreatePxShapes(go.transform.GetChild(i).gameObject));
                }
            }
            return shapes;
        }

        static void ExportRootGameObject(IntPtr collection, GameObject go)
        {
            List<IntPtr> shapes = CreatePxShapes(go);
            Rigidbody rigidBody = go.GetComponent<Rigidbody>();

            if (shapes.Count == 0 && rigidBody == null)
            {
                return;
            }

            IntPtr pxRigidBody = IntPtr.Zero;
            if (rigidBody != null)
            {
                pxRigidBody = CreateRigidDynamic(rigidBody);
             
            }
            else
            {
                pxRigidBody = CreateRigidStatic(go.transform);
            }

            setName(pxRigidBody, Marshal.StringToHGlobalAnsi(go.name));

            foreach (IntPtr shape in shapes)
            {
                attachShape(pxRigidBody, shape);
            }

            addCollectionObject(collection, pxRigidBody, 0);
        }

        public static bool Export(string scenePath, string outputPath)
        {
            EditorSceneManager.OpenScene(scenePath);
            Scene scene = EditorSceneManager.GetSceneByPath(scenePath);
            if (scene.IsValid())
            {
                return ExportCurrentScene(outputPath);
            }
            else
            {
                return false;
            }
        }

        public static bool ExportCurrentScene(string outputPath)
        {
            try{
                initPhysics();
                IntPtr collection = createCollection();
                if (collection == IntPtr.Zero)
                {
                    Debug.LogError("Create Collection Fail");
                    return false;
                }

                Scene scene = SceneManager.GetActiveScene();
                GameObject[] gameObjectArray = scene.GetRootGameObjects();
                foreach (GameObject go in gameObjectArray)
                {
                    if (CanExport(go))
                    {
                        ExportRootGameObject(collection, go);
                    }
                }

                complete(collection, IntPtr.Zero, false);

                    string filename = outputPath + "/" + scene.name + ".xml";
                serializeCollection(collection, IntPtr.Zero, Marshal.StringToHGlobalAnsi(filename), false);
                return true;
            }
            catch(Exception e)
            {
                Debug.LogError(string.Format("Export Scene: {0} error, error msg: {1}", EditorSceneManager.GetActiveScene().path, e.ToString()));
                return false;
            }
        }
    }
}
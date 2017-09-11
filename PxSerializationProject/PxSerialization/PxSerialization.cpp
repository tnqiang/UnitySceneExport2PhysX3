// PxSerialization.cpp : 定义 DLL 应用程序的导出函数。
//

#include "PxSerialization.h"

static PxDefaultAllocator*		    gAllocator = NULL;
static PxDefaultErrorCallback*	    gErrorCallback = NULL;

static PxFoundation*			    gFoundation = NULL;
static PxPhysics*				    gPhysics	= NULL;
static PxCooking*				    gCooking	= NULL;
static PxSerializationRegistry*     gSerializationRegistry = NULL;

static PxDefaultCpuDispatcher*	    gDispatcher = NULL;
static PxScene*                     gScene		= NULL;

extern "C" {

PXSERIALIZATION_API void initPhysics()
{
	gAllocator = new PxDefaultAllocator();
	gErrorCallback = new PxDefaultErrorCallback();
	gFoundation = PxCreateFoundation(PX_PHYSICS_VERSION, *gAllocator, *gErrorCallback);

	gPhysics = PxCreatePhysics(PX_PHYSICS_VERSION, *gFoundation, PxTolerancesScale(), true);
	PxInitExtensions(*gPhysics);

	PxSceneDesc sceneDesc(gPhysics->getTolerancesScale());
	sceneDesc.gravity = PxVec3(0, -9.81f, 0);	
	gDispatcher = PxDefaultCpuDispatcherCreate(1);
	sceneDesc.cpuDispatcher	= gDispatcher;
	sceneDesc.filterShader	= PxDefaultSimulationFilterShader;
	gScene = gPhysics->createScene(sceneDesc);

	gCooking = PxCreateCooking(PX_PHYSICS_VERSION, *gFoundation, PxCookingParams(PxTolerancesScale()));	

	gSerializationRegistry = physx::PxSerialization::createSerializationRegistry(*gPhysics);
}

PXSERIALIZATION_API void cleanupPhysics()
{	
	gSerializationRegistry->release();
	gScene->release();
	gDispatcher->release();
	PxCloseExtensions();

	gPhysics->release();	// releases of all objects	
	gCooking->release();

	gFoundation->release();

	delete gErrorCallback;
	delete gAllocator;
}

PXSERIALIZATION_API PxCollection* createCollection()
{
	return PxCreateCollection();
}

PXSERIALIZATION_API void addCollectionObject(PxCollection* collection, PxBase* object, PxSerialObjectId id)
{
	return collection->add(*object, id);
}

PXSERIALIZATION_API PxMaterial*  createMaterial(PxReal staticFriction, PxReal dynamicFriction, PxReal restitution)
{
	return gPhysics->createMaterial(staticFriction, dynamicFriction, restitution);
}

PXSERIALIZATION_API PxShape*  createShape(const PxGeometry* geometry, const PxMaterial* material)
{
	return gPhysics->createShape(*geometry, *material);
}

PXSERIALIZATION_API void attachShape(PxRigidActor* actor, PxShape* shape)
{
	actor->attachShape(*shape);
}

PXSERIALIZATION_API void detachShape(PxRigidActor* actor, PxShape* shape)
{
	actor->detachShape(*shape);
}

PXSERIALIZATION_API void setShapeFlag(PxShape* shape, PxShapeFlag::Enum flag, bool value)
{
	shape->setFlag(flag, value);
}

PXSERIALIZATION_API void setLocalPose(PxShape* shape, const PxTransform* transform)
{
	shape->setLocalPose(*transform);
}

PXSERIALIZATION_API PxRigidStatic*	createStatic(const PxTransform* transform, const PxGeometry* geometry, PxMaterial* material)
{
	//return PxCreateStatic(*gPhysics, *transform, *geometry, *material);
	return NULL;
}

PXSERIALIZATION_API PxRigidDynamic*	createDynamic( const PxTransform* transform, PxShape* shape, PxReal density)
{
	return PxCreateDynamic(*gPhysics, *transform, *shape, density);
}

PXSERIALIZATION_API PxRigidStatic* createRigidStatic(const PxTransform* transform)
{
	return gPhysics->createRigidStatic(*transform);
}

PXSERIALIZATION_API PxRigidDynamic*	createRigidDynamic(const PxTransform* transform, PxReal mass, PxReal drag, PxReal angularDrag, bool useGravity, bool isKinematic)
{
	PxRigidDynamic* rigidDynamic = gPhysics->createRigidDynamic(*transform);
	if (rigidDynamic == NULL)
	{
		return NULL;
	}
	rigidDynamic->setMass(mass);
	rigidDynamic->setLinearDamping(drag);
	rigidDynamic->setAngularDamping(angularDrag);
	rigidDynamic->setActorFlag(PxActorFlag::eDISABLE_GRAVITY, !useGravity);
	rigidDynamic->setRigidBodyFlag(PxRigidBodyFlag::eKINEMATIC, isKinematic);
	return rigidDynamic;
}

PXSERIALIZATION_API void complete(PxCollection* collection, const PxCollection* exceptFor, bool followJoints)
{
	PxSerialization::complete(*collection, *gSerializationRegistry, exceptFor, followJoints);
}

PXSERIALIZATION_API void release(PxBase* p)
{
	p->release();
}

PXSERIALIZATION_API void serializeCollection(PxCollection* collection, PxCollection* externalRefs, const char* filename, bool toBinary)
{
	PxDefaultFileOutputStream outputStream(filename);
	if (!outputStream.isValid())
	{
		return;
	}

	bool bret;
	if (toBinary)
	{
		bret = PxSerialization::serializeCollectionToBinary(outputStream, *collection, *gSerializationRegistry, externalRefs);
	}
	else
	{
		bret = PxSerialization::serializeCollectionToXml(outputStream, *collection, *gSerializationRegistry, gCooking, externalRefs);
	}
}

PXSERIALIZATION_API void setOwnerClient(PxActor* actor, PxClientID inClient)
{
	actor->setOwnerClient(inClient);
}

PXSERIALIZATION_API void setName(PxActor* actor, const char* name)
{
	actor->setName(name);
}

PXSERIALIZATION_API PxShape* createMeshShape(const PxVec3* points, PxI32 point_count, const PxU32* triangles, PxI32 triangle_count, bool convex, const PxMaterial* material)
{
	if (convex)
	{
		PxConvexMeshDesc convexDesc;
		convexDesc.points.count             = point_count;
		convexDesc.points.stride            = sizeof(PxVec3);
		convexDesc.points.data              = points;
		convexDesc.flags                    = PxConvexFlag::eCOMPUTE_CONVEX;

		PxDefaultMemoryOutputStream buf;
		if(!gCooking->cookConvexMesh(convexDesc, buf))
			return NULL;
		PxDefaultMemoryInputData input(buf.getData(), buf.getSize());
		PxConvexMesh* convexMesh = gPhysics->createConvexMesh(input);

		return gPhysics->createShape(PxConvexMeshGeometry(convexMesh), *material);
	}
	else
	{
		PxTriangleMeshDesc meshDesc;
		meshDesc.points.count           = point_count;
		meshDesc.points.stride          = sizeof(PxVec3);
		meshDesc.points.data            = points;

		meshDesc.triangles.count        = triangle_count;
		meshDesc.triangles.stride       = 3*sizeof(PxU32);
		meshDesc.triangles.data         = triangles;

		PxDefaultMemoryOutputStream writeBuffer;
		if(!gCooking->cookTriangleMesh(meshDesc, writeBuffer))
			return NULL;

		PxDefaultMemoryInputData readBuffer(writeBuffer.getData(), writeBuffer.getSize());
		PxTriangleMesh* triangleMesh = gPhysics->createTriangleMesh(readBuffer);

		return gPhysics->createShape(PxTriangleMeshGeometry(triangleMesh), *material);
	}
}

PXSERIALIZATION_API PxShape* CreateHeightField(const PxI16* heights, PxI32 width, PxI32 height, float thickness, float scaleX, float scaleY, float scaleZ, const PxMaterial* material)
{
	PxHeightFieldDesc desc;
	desc.nbRows = width;
	desc.nbColumns = height;
	desc.samples.stride = 4;
	desc.thickness = thickness;

	PxHeightFieldSample* pxSample = new PxHeightFieldSample[width * height];
	if (pxSample == NULL) return NULL;

	PxHeightFieldSample sample;
	sample.materialIndex0 = 0;
	sample.materialIndex1 = 0;
	sample.setTessFlag();

	for (size_t i = 0; i<width*height; ++i)
	{
		sample.height = heights[i];
		pxSample[i] = sample;
	}
	desc.samples.data = pxSample;

	PxDefaultMemoryOutputStream writeBuffer;
	if (!gCooking->cookHeightField(desc, writeBuffer))
	{
		return NULL;
	}

	PxDefaultMemoryInputData readBuffer(writeBuffer.getData(), writeBuffer.getSize());
	PxHeightField* hf = gPhysics->createHeightField(readBuffer);
	if (hf == NULL) return NULL;
	PxMeshGeometryFlags flags;

	return gPhysics->createShape(PxHeightFieldGeometry(hf, flags, scaleY, scaleX, scaleZ), *material);
}

    PXSERIALIZATION_API void setShapeName(PxShape* shapePtr, const char *shapeName)
    {
        if (shapePtr)
            shapePtr->setName(shapeName);
    }
    
    PXSERIALIZATION_API void setShapeContactOffset(PxShape* shapePtr, float offset)
    {
        if (shapePtr)
            shapePtr->setContactOffset(offset);
    }
    
    PXSERIALIZATION_API void setShapeQueryFilterData(PxShape* shapePtr, PxFilterData* data)
    {
        if (shapePtr)
            shapePtr->setQueryFilterData(*data);
    }
    
    PXSERIALIZATION_API void setShapeSimulationFilterData(PxShape* shapePtr, PxFilterData* data)
    {
        if (shapePtr)
            shapePtr->setSimulationFilterData(*data);
    }

}

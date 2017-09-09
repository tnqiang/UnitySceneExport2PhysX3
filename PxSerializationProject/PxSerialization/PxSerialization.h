// 下列 ifdef 块是创建使从 DLL 导出更简单的
// 宏的标准方法。此 DLL 中的所有文件都是用命令行上定义的 PXSERIALIZATION_EXPORTS
// 符号编译的。在使用此 DLL 的
// 任何其他项目上不应定义此符号。这样，源文件中包含此文件的任何其他项目都会将
// PXSERIALIZATION_API 函数视为是从 DLL 导入的，而此 DLL 则将用此宏定义的
// 符号视为是被导出的。

#ifdef _WIN32
#ifdef PXSERIALIZATION_EXPORTS
#define PXSERIALIZATION_API  __declspec(dllexport)
#else
#define PXSERIALIZATION_API __declspec(dllimport)
#endif
#else
#define PXSERIALIZATION_API
#endif

#include "PxPhysicsAPI.h"

using namespace physx;
extern "C" {

PXSERIALIZATION_API void initPhysics();

PXSERIALIZATION_API void cleanupPhysics();

PXSERIALIZATION_API PxCollection* createCollection();

PXSERIALIZATION_API void addCollectionObject(PxCollection* collection, PxBase* object, PxSerialObjectId id);

PXSERIALIZATION_API PxMaterial* createMaterial(PxReal staticFriction, PxReal dynamicFriction, PxReal restitution);

PXSERIALIZATION_API PxShape* createShape(const PxGeometry* geometry, const PxMaterial* material);

PXSERIALIZATION_API PxShape* createMeshShape(const PxVec3* points, PxI32 point_count, const PxU32* triangles, PxI32 triangle_count, bool convex, const PxMaterial* material);

PXSERIALIZATION_API PxShape* CreateHeightField(const PxI16* heights, PxI32 width, PxI32 height, float thickness, float scaleX, float scaleY, float scaleZ, const PxMaterial* material);

PXSERIALIZATION_API void attachShape(PxRigidActor* actor, PxShape* shape);

PXSERIALIZATION_API void detachShape(PxRigidActor* actor, PxShape* shape);

PXSERIALIZATION_API void setShapeFlag(PxShape* shape, PxShapeFlag::Enum flag, bool value);

PXSERIALIZATION_API void setLocalPose(PxShape* shape, const PxTransform* transform);

PXSERIALIZATION_API PxRigidStatic*	createStatic(const PxTransform* transform, const PxGeometry* geometry, PxMaterial* material);

PXSERIALIZATION_API PxRigidDynamic*	createDynamic( const PxTransform* transform, PxShape* shape, PxReal density);

PXSERIALIZATION_API PxRigidStatic*	createRigidStatic(const PxTransform* transform);

PXSERIALIZATION_API PxRigidDynamic*	createRigidDynamic(const PxTransform* transform, PxReal mass, PxReal drag, PxReal angularDrag, bool useGravity, bool isKinematic);

PXSERIALIZATION_API void complete(PxCollection* collection, const PxCollection* exceptFor, bool followJoints);

PXSERIALIZATION_API void serializeCollection(PxCollection* collection, PxCollection* externalRefs, const char* filename, bool toBinary);

PXSERIALIZATION_API void release(PxBase* p);

PXSERIALIZATION_API void setOwnerClient(PxActor* actor, PxClientID inClient);

PXSERIALIZATION_API void setName(PxActor* actor, const char* name);

PXSERIALIZATION_API void setShapeContactOffset(PxShape* shapePtr, float offset);

PXSERIALIZATION_API void setShapeQueryFilterData(PxShape* shapePtr, PxFilterData* data);

PXSERIALIZATION_API void setShapeSimulationFilterData(PxShape* shapePtr, PxFilterData* data);
}

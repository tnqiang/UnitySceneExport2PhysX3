#pragma once
#include <ctype.h>
#include <iostream>
#include <fstream>

#include "PxPhysicsAPI.h"
#include "characterkinematic/PxControllerManager.h"
#include "characterkinematic/PxControllerBehavior.h"

using namespace physx;

#define CONTACT_OFFSET			    0.01f
//	#define CONTACT_OFFSET			0.1f
//	#define STEP_OFFSET				0.01f
#define STEP_OFFSET				    0.05f
//	#define STEP_OFFSET				0.1f
//	#define STEP_OFFSET				0.2f

//	#define SLOPE_LIMIT				0.8f
#define SLOPE_LIMIT				    0.0f
//	#define INVISIBLE_WALLS_HEIGHT	6.0f
#define INVISIBLE_WALLS_HEIGHT	    0.0f
//	#define MAX_JUMP_HEIGHT			4.0f
#define MAX_JUMP_HEIGHT			    0.0f

static const float kScaleFactor      = 1.0f;
static const float kStandingSize     = 20.00f * kScaleFactor;
static const float kCrouchingSize    = 5.0f   * kScaleFactor;
static const float kControllerRadius = 3.0f   * kScaleFactor;

class PhysicsSceneManager
{
public:
	bool InitPhysics();
	void CleanPhysics();

	void SetupPvdDebug();
	void StepPhysics(bool interactive);

	bool ParseFromCollectionFile();

private:
	PxFoundation*                m_foundation;
	PxPhysics*                   m_physics;
	PxVisualDebuggerConnection * m_pvdCon;
	PxCooking*                   m_cooking;
	PxScene*                     m_scene;
	PxMaterial*                  m_material;
	PxControllerManager*         m_controller_mgr;
	PxDefaultAllocator           m_allocator;
	PxDefaultErrorCallback	     m_error_callback;
	PxDefaultCpuDispatcher*      m_dispatcher;
	PxSerializationRegistry*	 m_registery;
};

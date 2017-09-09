#include "physics_scene_manager.h"
#include "characterkinematic/PxController.h"
#include <vector>

bool PhysicsSceneManager::InitPhysics()
{
	//foundation and physics must be created at initialization stage
	m_foundation = PxCreateFoundation(PX_PHYSICS_VERSION, m_allocator, m_error_callback);
	if (m_foundation == NULL)
	{
		std::cerr << "PxCreateFoundation failed!" << std::endl;
		exit(0);
	}

	PxProfileZoneManager *profileZoneMgr = &PxProfileZoneManager::createProfileZoneManager(m_foundation);
	m_physics = PxCreatePhysics(PX_PHYSICS_VERSION, *m_foundation, PxTolerancesScale(), true, profileZoneMgr);
	if (m_physics == NULL)
	{
		std::cerr << "PxCreatePhysics failed!" << std::endl;
		exit(0);
	}

	m_cooking = PxCreateCooking(PX_PHYSICS_VERSION, *m_foundation, PxCookingParams(PxTolerancesScale()));
	if (m_cooking == NULL)
	{
		std::cerr << "PxCreateCooking failed!" << std::endl;
		exit(0);
	}

	SetupPvdDebug();

	//scene descriptor
	PxSceneDesc sceneDesc(m_physics->getTolerancesScale());
	sceneDesc.gravity       = PxVec3(0.0f, -9.18f, 0.0f);
	m_dispatcher            = PxDefaultCpuDispatcherCreate(2);
	sceneDesc.cpuDispatcher = m_dispatcher;
	
	sceneDesc.filterShader  = PxDefaultSimulationFilterShader;

	//create scene and default material
	m_scene = m_physics->createScene(sceneDesc);
	m_material = m_physics->createMaterial(0.5f, 0.5f, 0.6f);

	//character controller
	m_controller_mgr = PxCreateControllerManager(*m_scene);

	m_registery = PxSerialization::createSerializationRegistry(*m_physics);

	// read scene from collection file
	ParseFromCollectionFile();
	/*
	std::string file_path = "../Scenes/test.scene";
	std::ifstream input(file_path.c_str(), std::ios::in | std::ios::binary);
	if (!input)
	{
		std::cout << "can't open file " << std::endl;
		exit(0);
	}
	*/
	return true;
}

void PhysicsSceneManager::SetupPvdDebug()
{
	// check if PvdConnection manager is available on this platform
	if (m_physics->getPvdConnectionManager() == NULL)
		return;

	// setup connection parameters
	const char*     pvd_host_ip = "127.0.0.1";  // IP of the PC which is running PVD
	int             port        = 5425;         // TCP port to connect to, where PVD is listening
	unsigned int    timeout     = 100;          // timeout in milliseconds to wait for PVD to respond,
	// consoles and remote PCs need a higher timeout.
	PxVisualDebuggerConnectionFlags connectionFlags = PxVisualDebuggerExt::getAllConnectionFlags();

	// and now try to connect
	PxVisualDebuggerConnection* theConnection =
		PxVisualDebuggerExt::createConnection(m_physics->getPvdConnectionManager(),
		pvd_host_ip, port, timeout, connectionFlags);
}

bool PhysicsSceneManager::ParseFromCollectionFile()
{
	PxCollection* shared = NULL;
	PxCollection* collection = NULL;
	char *filename = "your scene file path";

	PxDefaultFileInputData inputStream(filename);

	collection = PxSerialization::createCollectionFromXml(inputStream, *m_cooking, *m_registery, shared);

	m_scene->addCollection(*collection);

	return true;
}

void PhysicsSceneManager::StepPhysics(bool)
{
	m_scene->simulate(1 / 60.f); //get the world state after 1/60.f second
	m_scene->fetchResults(true); //these two line must be used in pair
}

void PhysicsSceneManager::CleanPhysics()
{
	m_scene->release();
	m_dispatcher->release();
	PxProfileZoneManager* profileZoneManager = m_physics->getProfileZoneManager();
	if (m_pvdCon != NULL)
		m_pvdCon->release();

	m_physics->release();
	profileZoneManager->release();
	m_foundation->release();
	m_cooking->release();
}

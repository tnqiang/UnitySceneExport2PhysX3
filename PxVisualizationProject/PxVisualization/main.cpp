#include <iostream>
#include "physics_scene_manager.h"
#define RENDERFORTEST

PhysicsSceneManager *g_scene_mgr = new PhysicsSceneManager();

int main()
{
	extern void RenderLoop();
	RenderLoop();

	return 0;
}

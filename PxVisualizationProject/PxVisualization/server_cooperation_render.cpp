#define RENDERFORTEST

#ifdef RENDERFORTEST
#include <vector>
#include "PxPhysicsAPI.h"

#include "physics_scene_render.h"
#include "physics_scene_camera.h"
#include "physics_scene_manager.h"

using namespace physx;

extern PhysicsSceneManager *g_scene_mgr;

namespace
{
	PhysicsSceneRender::Camera* psr_camera;
		
	void KeyboardCallback(unsigned char key, int x, int y)
	{
        if (key == 27){
			exit(0);
        }else{
            psr_camera->HandleKey(key);
        }
	}

	void MotionCallback(int x, int y)
	{
		psr_camera->HandleMotion(x, y);
	}

	void PassiveMotionCallback(int x, int y)
	{
		psr_camera->HandlePassiveMotion(x, y);
	}

	void MouseCallback(int button, int state, int x, int y)
	{
		std::cout << x <<" " <<y << std::endl;
		psr_camera->HandleMouse(x, y);
	}

	void IdleCallback()
	{
		glutPostRedisplay();
	}


	void coordinate_3d()
	{
		GLfloat points[] = {
			0.0f, 0.0f, 0.0f,
			10.0f, 0.0f, 0.0f,
			0.0f, 10.0f, 0.0f,
			0.0f, 0.0f, 10.0f
		};

		GLubyte coordinate_3d[] = { 0, 1, 0, 2, 0, 3 };

		//glClearColor(0, 0, 0, 0);
		//glClear(GL_COLOR_BUFFER_BIT);

		glEnableClientState(GL_VERTEX_ARRAY);
		glVertexPointer(3, GL_FLOAT, 0, points);
		glColor3f(1.0f, 1.0f, 1.0f);

		glLoadIdentity();
		gluLookAt(6.0, 5.0, 4.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0);
		glColor3f(1.0f, 0.0f, 0.0f);
		glDrawElements(GL_LINES, 2, GL_UNSIGNED_BYTE, coordinate_3d);
		glColor3f(0.0f, 1.0f, 0.0f);
		glDrawElements(GL_LINES, 2, GL_UNSIGNED_BYTE, &coordinate_3d[2]);
		glColor3f(0.0f, 0.0f, 1.0f);
		glDrawElements(GL_LINES, 2, GL_UNSIGNED_BYTE, &coordinate_3d[4]);

		glFlush();
	}

	void RenderCallback()
	{
		g_scene_mgr->StepPhysics(true);

		PhysicsSceneRender::StartRender(psr_camera->GetEye(), psr_camera->GetDir());

		//analyze the scene, render every actor
		PxScene *scene;
		PxGetPhysics().getScenes(&scene, 1);
		PxU32 nbActors = scene->getNbActors(PxActorTypeSelectionFlag::eRIGID_DYNAMIC | PxActorTypeSelectionFlag::eRIGID_STATIC);
		//printf("actor nb: %d\n", nbActors);
		if (nbActors != 0)
		{
			std::vector<PxRigidActor*> actors(nbActors);
			scene->getActors(PxActorTypeSelectionFlag::eRIGID_DYNAMIC | PxActorTypeSelectionFlag::eRIGID_STATIC, (PxActor**)&actors[0], nbActors);
			PhysicsSceneRender::RenderActors(&actors[0], (PxU32)actors.size(), true);
		}
		//coordinate_3d();
		PhysicsSceneRender::FinishRender();

		//TODO: 在这里添加绘制坐标轴
	}


	void ExitCallback(void)
	{
		delete psr_camera;
		g_scene_mgr->CleanPhysics();
	}
}

void RenderLoop()
{
	psr_camera = new PhysicsSceneRender::Camera(PxVec3(50.f, 50.f, 50.f), PxVec3(-0.6f, -0.2f, -0.7f));

	PhysicsSceneRender::SetupDefaultWindow("Server Render For Debug");
	PhysicsSceneRender::SetupDefaultRenderState();
	glutIdleFunc(IdleCallback);
	glutDisplayFunc(RenderCallback);
	glutKeyboardFunc(KeyboardCallback);
	glutMouseFunc(MouseCallback);
	glutMotionFunc(MotionCallback);
	glutPassiveMotionFunc(PassiveMotionCallback);//no mouse button is clicked
	MotionCallback(0, 0);

	atexit(ExitCallback);

	g_scene_mgr->InitPhysics();
	glutMainLoop();
}

#endif

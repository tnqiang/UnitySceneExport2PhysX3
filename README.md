# UnitySceneExport2PhysX3

This is a unity editor tool to help game server developer to rebuild the physics of the client scene using [PhysX](https://www.geforce.com/hardware/technology/physx) 3.x or newer.

You can use a simple export button as shown below to export your scene to a xml(or binary) file.
![](./Image/editor_window.png)

Then you can rebuild your scene using C++ with a few line of codes like below:

```
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
```

## Shape Type Supported

- Sphere ![](./Image/support.png)
- Plane(No need to support)
- Capsule ![](./Image/support.png)
- Box ![](./Image/support.png)
- Convex Mesh ![](./Image/support.png)
- Triangle Mesh ![](./Image/support.png)
- Height Field(./Image/support.png)

## How to use

### Import the package
Import the ``UnityPhsyxExport.unitypackage`` package to your project and refer to the settings below.

1. Unity5.5 or higher
Check if there is a mcs.rsp in "Assets/" folder, if not exist, create it. Then add a line "-unsafe" and save the file, and reopen unity will solve the error. 

2. other Unity version
Check if there is a gmcs.rsp in "Assets/" folder, if not exist, create it. Then add a line "-unsafe" and save the file, and reopen unity will solve the error.

### Use
1. Open Editor Window: Window -> PhysXExport -> OpenWindow;
2. Export Selected Scene: Click one scene in the editor window

## Notes
1. The PhysX lib this Project use is PhysX 3.3
2. Attached with this tool is three project:
	- PxSerializationProject: C++ encapsulation of PhysX API exported to Unity, with Windows and MacOs supported.
	- UnityProject: C# encapsulation of PhysX API used to export scene and the editor window code.
	- PxVisualizationProject: A visualization tool to check the output xml scene file using GLUT and openGL, with windows and MacOS supported.

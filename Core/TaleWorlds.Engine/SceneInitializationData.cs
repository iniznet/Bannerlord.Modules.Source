using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	[EngineStruct("rglScene_initialization_data")]
	public struct SceneInitializationData
	{
		public SceneInitializationData(bool initializeWithDefaults)
		{
			if (initializeWithDefaults)
			{
				this.InitPhysicsWorld = true;
				this.LoadNavMesh = true;
				this.InitFloraNodes = true;
				this.UsePhysicsMaterials = true;
				this.EnableFloraPhysics = true;
				this.UseTerrainMeshBlending = true;
				this.DoNotUseLoadingScreen = false;
				this.CreateOros = false;
				this.ForTerrainShaderCompile = false;
				this.CamPosFromScene = MatrixFrame.Identity;
				return;
			}
			this.InitPhysicsWorld = false;
			this.LoadNavMesh = false;
			this.InitFloraNodes = false;
			this.UsePhysicsMaterials = false;
			this.EnableFloraPhysics = false;
			this.UseTerrainMeshBlending = false;
			this.DoNotUseLoadingScreen = false;
			this.CreateOros = false;
			this.ForTerrainShaderCompile = false;
			this.CamPosFromScene = MatrixFrame.Identity;
		}

		public MatrixFrame CamPosFromScene;

		public bool InitPhysicsWorld;

		public bool LoadNavMesh;

		public bool InitFloraNodes;

		public bool UsePhysicsMaterials;

		public bool EnableFloraPhysics;

		public bool UseTerrainMeshBlending;

		public bool DoNotUseLoadingScreen;

		public bool CreateOros;

		public bool ForTerrainShaderCompile;
	}
}

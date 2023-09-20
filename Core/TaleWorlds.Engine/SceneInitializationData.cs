using System;
using System.Runtime.InteropServices;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	[EngineStruct("rglScene_initialization_data", false)]
	public struct SceneInitializationData
	{
		public SceneInitializationData(bool initializeWithDefaults)
		{
			if (initializeWithDefaults)
			{
				this.CamPosFromScene = MatrixFrame.Identity;
				this.InitPhysicsWorld = true;
				this.LoadNavMesh = true;
				this.InitFloraNodes = true;
				this.UsePhysicsMaterials = true;
				this.EnableFloraPhysics = true;
				this.UseTerrainMeshBlending = true;
				this.DoNotUseLoadingScreen = false;
				this.CreateOros = false;
				this.ForTerrainShaderCompile = false;
				return;
			}
			this.CamPosFromScene = MatrixFrame.Identity;
			this.InitPhysicsWorld = false;
			this.LoadNavMesh = false;
			this.InitFloraNodes = false;
			this.UsePhysicsMaterials = false;
			this.EnableFloraPhysics = false;
			this.UseTerrainMeshBlending = false;
			this.DoNotUseLoadingScreen = false;
			this.CreateOros = false;
			this.ForTerrainShaderCompile = false;
		}

		public MatrixFrame CamPosFromScene;

		[MarshalAs(UnmanagedType.U1)]
		public bool InitPhysicsWorld;

		[MarshalAs(UnmanagedType.U1)]
		public bool LoadNavMesh;

		[MarshalAs(UnmanagedType.U1)]
		public bool InitFloraNodes;

		[MarshalAs(UnmanagedType.U1)]
		public bool UsePhysicsMaterials;

		[MarshalAs(UnmanagedType.U1)]
		public bool EnableFloraPhysics;

		[MarshalAs(UnmanagedType.U1)]
		public bool UseTerrainMeshBlending;

		[MarshalAs(UnmanagedType.U1)]
		public bool DoNotUseLoadingScreen;

		[MarshalAs(UnmanagedType.U1)]
		public bool CreateOros;

		[MarshalAs(UnmanagedType.U1)]
		public bool ForTerrainShaderCompile;
	}
}

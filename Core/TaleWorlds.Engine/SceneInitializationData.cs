using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000079 RID: 121
	[EngineStruct("rglScene_initialization_data")]
	public struct SceneInitializationData
	{
		// Token: 0x060008C6 RID: 2246 RVA: 0x00008E78 File Offset: 0x00007078
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

		// Token: 0x0400017C RID: 380
		public MatrixFrame CamPosFromScene;

		// Token: 0x0400017D RID: 381
		public bool InitPhysicsWorld;

		// Token: 0x0400017E RID: 382
		public bool LoadNavMesh;

		// Token: 0x0400017F RID: 383
		public bool InitFloraNodes;

		// Token: 0x04000180 RID: 384
		public bool UsePhysicsMaterials;

		// Token: 0x04000181 RID: 385
		public bool EnableFloraPhysics;

		// Token: 0x04000182 RID: 386
		public bool UseTerrainMeshBlending;

		// Token: 0x04000183 RID: 387
		public bool DoNotUseLoadingScreen;

		// Token: 0x04000184 RID: 388
		public bool CreateOros;

		// Token: 0x04000185 RID: 389
		public bool ForTerrainShaderCompile;
	}
}

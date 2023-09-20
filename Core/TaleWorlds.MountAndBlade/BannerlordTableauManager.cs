using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000182 RID: 386
	public static class BannerlordTableauManager
	{
		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x0600141C RID: 5148 RVA: 0x0004E592 File Offset: 0x0004C792
		public static Scene[] TableauCharacterScenes
		{
			get
			{
				return BannerlordTableauManager._tableauCharacterScenes;
			}
		}

		// Token: 0x0600141D RID: 5149 RVA: 0x0004E599 File Offset: 0x0004C799
		public static void RequestCharacterTableauRender(int characterCodeId, string path, GameEntity poseEntity, Camera cameraObject, int tableauType)
		{
			MBAPI.IMBBannerlordTableauManager.RequestCharacterTableauRender(characterCodeId, path, poseEntity.Pointer, cameraObject.Pointer, tableauType);
		}

		// Token: 0x0600141E RID: 5150 RVA: 0x0004E5B5 File Offset: 0x0004C7B5
		public static void ClearManager()
		{
			BannerlordTableauManager._tableauCharacterScenes = null;
			BannerlordTableauManager.RequestCallback = null;
			BannerlordTableauManager._isTableauRenderSystemInitialized = false;
		}

		// Token: 0x0600141F RID: 5151 RVA: 0x0004E5C9 File Offset: 0x0004C7C9
		public static void InitializeCharacterTableauRenderSystem()
		{
			if (!BannerlordTableauManager._isTableauRenderSystemInitialized)
			{
				MBAPI.IMBBannerlordTableauManager.InitializeCharacterTableauRenderSystem();
				BannerlordTableauManager._isTableauRenderSystemInitialized = true;
			}
		}

		// Token: 0x06001420 RID: 5152 RVA: 0x0004E5E2 File Offset: 0x0004C7E2
		public static int GetNumberOfPendingTableauRequests()
		{
			return MBAPI.IMBBannerlordTableauManager.GetNumberOfPendingTableauRequests();
		}

		// Token: 0x06001421 RID: 5153 RVA: 0x0004E5EE File Offset: 0x0004C7EE
		[MBCallback]
		internal static void RequestCharacterTableauSetup(int characterCodeId, Scene scene, GameEntity poseEntity)
		{
			BannerlordTableauManager.RequestCallback(characterCodeId, scene, poseEntity);
		}

		// Token: 0x06001422 RID: 5154 RVA: 0x0004E5FD File Offset: 0x0004C7FD
		[MBCallback]
		internal static void RegisterCharacterTableauScene(Scene scene, int type)
		{
			BannerlordTableauManager.TableauCharacterScenes[type] = scene;
		}

		// Token: 0x040006C2 RID: 1730
		private static Scene[] _tableauCharacterScenes = new Scene[5];

		// Token: 0x040006C3 RID: 1731
		private static bool _isTableauRenderSystemInitialized = false;

		// Token: 0x040006C4 RID: 1732
		public static BannerlordTableauManager.RequestCharacterTableauSetupDelegate RequestCallback;

		// Token: 0x020004FC RID: 1276
		// (Invoke) Token: 0x06003900 RID: 14592
		public delegate void RequestCharacterTableauSetupDelegate(int characterCodeId, Scene scene, GameEntity poseEntity);
	}
}

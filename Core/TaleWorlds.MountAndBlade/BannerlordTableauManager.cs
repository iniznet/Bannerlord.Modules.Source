using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public static class BannerlordTableauManager
	{
		public static Scene[] TableauCharacterScenes
		{
			get
			{
				return BannerlordTableauManager._tableauCharacterScenes;
			}
		}

		public static void RequestCharacterTableauRender(int characterCodeId, string path, GameEntity poseEntity, Camera cameraObject, int tableauType)
		{
			MBAPI.IMBBannerlordTableauManager.RequestCharacterTableauRender(characterCodeId, path, poseEntity.Pointer, cameraObject.Pointer, tableauType);
		}

		public static void ClearManager()
		{
			BannerlordTableauManager._tableauCharacterScenes = null;
			BannerlordTableauManager.RequestCallback = null;
			BannerlordTableauManager._isTableauRenderSystemInitialized = false;
		}

		public static void InitializeCharacterTableauRenderSystem()
		{
			if (!BannerlordTableauManager._isTableauRenderSystemInitialized)
			{
				MBAPI.IMBBannerlordTableauManager.InitializeCharacterTableauRenderSystem();
				BannerlordTableauManager._isTableauRenderSystemInitialized = true;
			}
		}

		public static int GetNumberOfPendingTableauRequests()
		{
			return MBAPI.IMBBannerlordTableauManager.GetNumberOfPendingTableauRequests();
		}

		[MBCallback]
		internal static void RequestCharacterTableauSetup(int characterCodeId, Scene scene, GameEntity poseEntity)
		{
			BannerlordTableauManager.RequestCallback(characterCodeId, scene, poseEntity);
		}

		[MBCallback]
		internal static void RegisterCharacterTableauScene(Scene scene, int type)
		{
			BannerlordTableauManager.TableauCharacterScenes[type] = scene;
		}

		private static Scene[] _tableauCharacterScenes = new Scene[5];

		private static bool _isTableauRenderSystemInitialized = false;

		public static BannerlordTableauManager.RequestCharacterTableauSetupDelegate RequestCallback;

		public delegate void RequestCharacterTableauSetupDelegate(int characterCodeId, Scene scene, GameEntity poseEntity);
	}
}

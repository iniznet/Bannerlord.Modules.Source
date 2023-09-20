using System;
using System.Collections.Generic;
using System.Threading;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.View.Screens.Scripts
{
	// Token: 0x02000036 RID: 54
	public class MultiThreadedStressTestsScreen : ScreenBase
	{
		// Token: 0x0600027D RID: 637 RVA: 0x00016ED0 File Offset: 0x000150D0
		protected override void OnActivate()
		{
			base.OnActivate();
			this._scene = Scene.CreateNewScene(true, true, 0, "mono_renderscene");
			this._scene.Read("mp_ruins_2");
			this._sceneView = SceneView.CreateSceneView();
			this._sceneView.SetScene(this._scene);
			this._sceneView.SetSceneUsesShadows(true);
			Camera camera = Camera.CreateCamera();
			camera.Frame = this._scene.ReadAndCalculateInitialCamera();
			this._sceneView.SetCamera(camera);
			this._workerThreads = new List<Thread>();
			Thread thread = new Thread(delegate
			{
				MultiThreadedStressTestsScreen.MultiThreadedTestFunctions.MeshMerger(0);
			});
			thread.Name = "StressTester|Mesh Merger Thread";
			this._workerThreads.Add(thread);
			Thread thread2 = new Thread(delegate
			{
				MultiThreadedStressTestsScreen.MultiThreadedTestFunctions.MeshMerger(1);
			});
			thread2.Name = "StressTester|Mesh Merger Thread";
			this._workerThreads.Add(thread2);
			Thread thread3 = new Thread(delegate
			{
				MultiThreadedStressTestsScreen.MultiThreadedTestFunctions.MeshMerger(2);
			});
			thread3.Name = "StressTester|Mesh Merger Thread";
			this._workerThreads.Add(thread3);
			for (int i = 0; i < this._workerThreads.Count; i++)
			{
				this._workerThreads[i].Start();
			}
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0001703C File Offset: 0x0001523C
		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			this._sceneView = null;
			this._scene = null;
		}

		// Token: 0x0600027F RID: 639 RVA: 0x00017054 File Offset: 0x00015254
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			bool flag = true;
			for (int i = 0; i < this._workerThreads.Count; i++)
			{
				if (this._workerThreads[i].IsAlive)
				{
					flag = false;
				}
			}
			if (flag)
			{
				ScreenManager.PopScreen();
			}
		}

		// Token: 0x04000194 RID: 404
		private List<Thread> _workerThreads;

		// Token: 0x04000195 RID: 405
		private Scene _scene;

		// Token: 0x04000196 RID: 406
		private SceneView _sceneView;

		// Token: 0x020000AD RID: 173
		public static class MultiThreadedTestFunctions
		{
			// Token: 0x0600053E RID: 1342 RVA: 0x00026C2C File Offset: 0x00024E2C
			public static void MeshMerger(InputLayout layout)
			{
				Mesh mesh = Mesh.GetRandomMeshWithVdecl(layout);
				mesh = mesh.CreateCopy();
				UIntPtr uintPtr = mesh.LockEditDataWrite();
				Mesh mesh2 = Mesh.GetRandomMeshWithVdecl(layout);
				mesh2 = mesh2.CreateCopy();
				Mesh randomMeshWithVdecl = Mesh.GetRandomMeshWithVdecl(layout);
				Mesh randomMeshWithVdecl2 = Mesh.GetRandomMeshWithVdecl(layout);
				mesh.AddMesh(randomMeshWithVdecl, MatrixFrame.Identity);
				mesh2.AddMesh(randomMeshWithVdecl2, MatrixFrame.Identity);
				mesh.AddMesh(mesh2, MatrixFrame.Identity);
				int num = mesh.AddFaceCorner(new Vec3(0f, 0f, 1f, -1f), new Vec3(0f, 0f, 1f, -1f), new Vec2(0f, 1f), 268435455U, uintPtr);
				int num2 = mesh.AddFaceCorner(new Vec3(0f, 1f, 0f, -1f), new Vec3(0f, 0f, 1f, -1f), new Vec2(1f, 0f), 268435455U, uintPtr);
				int num3 = mesh.AddFaceCorner(new Vec3(0f, 1f, 1f, -1f), new Vec3(0f, 0f, 1f, -1f), new Vec2(1f, 1f), 268435455U, uintPtr);
				mesh.AddFace(num, num2, num3, uintPtr);
				mesh.UnlockEditDataWrite(uintPtr);
			}

			// Token: 0x0600053F RID: 1343 RVA: 0x00026D94 File Offset: 0x00024F94
			public static void SceneHandler(SceneView view)
			{
				int i = 0;
				while (i < 500)
				{
					view.SetSceneUsesShadows(false);
					view.SetRenderWithPostfx(false);
					Thread.Sleep(5000);
					view.SetSceneUsesShadows(true);
					view.SetRenderWithPostfx(true);
					Thread.Sleep(5000);
					view.SetSceneUsesContour(true);
					Thread.Sleep(5000);
				}
			}
		}
	}
}

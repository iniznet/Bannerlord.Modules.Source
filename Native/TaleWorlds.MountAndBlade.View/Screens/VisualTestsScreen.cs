using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.View.Screens
{
	// Token: 0x02000035 RID: 53
	public class VisualTestsScreen : ScreenBase
	{
		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000269 RID: 617 RVA: 0x000164BE File Offset: 0x000146BE
		private int CamPointCount
		{
			get
			{
				return this.CamPoints.Count;
			}
		}

		// Token: 0x0600026A RID: 618 RVA: 0x000164CB File Offset: 0x000146CB
		public bool StartedRendering()
		{
			return this._sceneLayer.SceneView.ReadyToRender();
		}

		// Token: 0x0600026B RID: 619 RVA: 0x000164E0 File Offset: 0x000146E0
		public string GetSubTestName(VisualTestsScreen.CameraPointTestType type)
		{
			if (type == VisualTestsScreen.CameraPointTestType.Albedo)
			{
				return "_albedo";
			}
			if (type == VisualTestsScreen.CameraPointTestType.Normal)
			{
				return "_normal";
			}
			if (type == VisualTestsScreen.CameraPointTestType.Specular)
			{
				return "_specular";
			}
			if (type == VisualTestsScreen.CameraPointTestType.AO)
			{
				return "_ao";
			}
			if (type == VisualTestsScreen.CameraPointTestType.OnlyAmbient)
			{
				return "_onlyambient";
			}
			if (type == VisualTestsScreen.CameraPointTestType.OnlyDirect)
			{
				return "_onlydirect";
			}
			if (type == VisualTestsScreen.CameraPointTestType.Final)
			{
				return "_final";
			}
			return "";
		}

		// Token: 0x0600026C RID: 620 RVA: 0x00016537 File Offset: 0x00014737
		public Utilities.EngineRenderDisplayMode GetRenderMode(VisualTestsScreen.CameraPointTestType type)
		{
			if (type == VisualTestsScreen.CameraPointTestType.Albedo)
			{
				return 1;
			}
			if (type == VisualTestsScreen.CameraPointTestType.Normal)
			{
				return 2;
			}
			if (type == VisualTestsScreen.CameraPointTestType.Specular)
			{
				return 4;
			}
			if (type == VisualTestsScreen.CameraPointTestType.AO)
			{
				return 6;
			}
			if (type == VisualTestsScreen.CameraPointTestType.OnlyAmbient)
			{
				return 15;
			}
			if (type == VisualTestsScreen.CameraPointTestType.OnlyDirect)
			{
				return 21;
			}
			return 0;
		}

		// Token: 0x0600026D RID: 621 RVA: 0x00016560 File Offset: 0x00014760
		public VisualTestsScreen(bool isValidTest, NativeOptions.ConfigQuality preset, string sceneName, DateTime testTime, List<string> testTypesToCheck)
		{
			this.isValidTest_ = isValidTest;
			this.preset_ = preset;
			this.scene_name = sceneName;
			this.testTime = testTime;
			VisualTestsScreen.isSceneSuccess = true;
			this._failDirectory = string.Concat(new object[] { this._failDirectory, "/", sceneName, "_", preset });
			this.testTypesToCheck_ = testTypesToCheck;
		}

		// Token: 0x0600026E RID: 622 RVA: 0x00016633 File Offset: 0x00014833
		protected override void OnInitialize()
		{
			base.OnInitialize();
			this._sceneLayer = new SceneLayer("SceneLayer", true, true);
			base.AddLayer(this._sceneLayer);
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0001665C File Offset: 0x0001485C
		protected override void OnActivate()
		{
			base.OnActivate();
			if (!this.isValidTest_)
			{
				this.date = this.testTime.ToString("dd-MM-yyyy hh-mmtt");
				this._pathDirectory = this._pathDirectory + this.date + "/";
				Directory.CreateDirectory(this._pathDirectory);
				this._reportFile = this._pathDirectory + "report.txt";
			}
			this.CreateScene();
			this._scene.Tick(0f);
		}

		// Token: 0x06000270 RID: 624 RVA: 0x000166E1 File Offset: 0x000148E1
		protected override void OnDeactivate()
		{
			base.OnDeactivate();
		}

		// Token: 0x06000271 RID: 625 RVA: 0x000166EC File Offset: 0x000148EC
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			LoadingWindow.DisableGlobalLoadingWindow();
			MessageManager.EraseMessageLines();
			if (!this._sceneLayer.ReadyToRender())
			{
				return;
			}
			this.SetTestCamera();
			if (Utilities.GetNumberOfShaderCompilationsInProgress() > 0)
			{
				return;
			}
			float num = ((this._scene.GetName() == "visualtestmorph") ? 0.01f : 0f);
			this._scene.Tick(num);
			int num2 = 5;
			this.frameCounter++;
			if (this.frameCounter < num2)
			{
				return;
			}
			this.TakeScreenshotAndAnalyze();
			if (this.CurCameraIndex >= this.CamPointCount)
			{
				ScreenManager.PopScreen();
				return;
			}
			this.frameCounter = 0;
		}

		// Token: 0x06000272 RID: 626 RVA: 0x00016794 File Offset: 0x00014994
		private void CreateScene()
		{
			this._scene = Scene.CreateNewScene(true, true, 0, "mono_renderscene");
			this._scene.SetName("VisualTestScreen");
			this._agentRendererSceneController = MBAgentRendererSceneController.CreateNewAgentRendererSceneController(this._scene, 32);
			this._scene.Read(this.scene_name);
			this._scene.SetUseConstantTime(true);
			this._scene.SetOcclusionMode(true);
			this._scene.OptimizeScene(true, true);
			this._sceneLayer.SetScene(this._scene);
			this._sceneLayer.SceneView.SetSceneUsesShadows(true);
			this._sceneLayer.SceneView.SetForceShaderCompilation(true);
			this._sceneLayer.SceneView.SetClearGbuffer(true);
			this._camera = Camera.CreateCamera();
			this.GetCameraPoints();
			MessageManager.EraseMessageLines();
		}

		// Token: 0x06000273 RID: 627 RVA: 0x00016867 File Offset: 0x00014A67
		private bool ShouldCheckTestModeWithTag(string mode, GameEntity entity)
		{
			if (this.testTypesToCheck_.Count > 0)
			{
				return this.testTypesToCheck_.Contains(mode) && entity.HasTag(mode);
			}
			return entity.HasTag(mode);
		}

		// Token: 0x06000274 RID: 628 RVA: 0x00016896 File Offset: 0x00014A96
		private bool ShouldCheckTestMode(string mode)
		{
			return this.testTypesToCheck_.Contains(mode);
		}

		// Token: 0x06000275 RID: 629 RVA: 0x000168A4 File Offset: 0x00014AA4
		private void GetCameraPoints()
		{
			this.CamPoints = new List<VisualTestsScreen.CameraPoint>();
			foreach (GameEntity gameEntity in (from o in this._scene.FindEntitiesWithTag("test_camera")
				orderby o.Name
				select o).ToList<GameEntity>())
			{
				if (!gameEntity.HasTag("exclude_" + this.preset_))
				{
					VisualTestsScreen.CameraPoint cameraPoint = new VisualTestsScreen.CameraPoint();
					cameraPoint.CamFrame = gameEntity.GetFrame();
					cameraPoint.CameraName = gameEntity.Name;
					HashSet<VisualTestsScreen.CameraPointTestType> hashSet = new HashSet<VisualTestsScreen.CameraPointTestType>();
					if (this.ShouldCheckTestModeWithTag("gbuffer", gameEntity))
					{
						hashSet.Add(VisualTestsScreen.CameraPointTestType.Albedo);
						hashSet.Add(VisualTestsScreen.CameraPointTestType.Normal);
						hashSet.Add(VisualTestsScreen.CameraPointTestType.Specular);
						hashSet.Add(VisualTestsScreen.CameraPointTestType.AO);
					}
					if (this.ShouldCheckTestMode("albedo"))
					{
						hashSet.Add(VisualTestsScreen.CameraPointTestType.Albedo);
					}
					if (this.ShouldCheckTestMode("normal"))
					{
						hashSet.Add(VisualTestsScreen.CameraPointTestType.Normal);
					}
					if (this.ShouldCheckTestMode("specular"))
					{
						hashSet.Add(VisualTestsScreen.CameraPointTestType.Specular);
					}
					if (this.ShouldCheckTestMode("ao"))
					{
						hashSet.Add(VisualTestsScreen.CameraPointTestType.AO);
					}
					if (this.ShouldCheckTestModeWithTag("only_ambient", gameEntity))
					{
						hashSet.Add(VisualTestsScreen.CameraPointTestType.OnlyAmbient);
					}
					foreach (VisualTestsScreen.CameraPointTestType cameraPointTestType in hashSet)
					{
						cameraPoint.TestTypes.Add(cameraPointTestType);
					}
					cameraPoint.TestTypes.Add(VisualTestsScreen.CameraPointTestType.Final);
					this.CamPoints.Add(cameraPoint);
				}
			}
		}

		// Token: 0x06000276 RID: 630 RVA: 0x00016A88 File Offset: 0x00014C88
		private void SetTestCamera()
		{
			VisualTestsScreen.CameraPoint cameraPoint = this.CamPoints[this.CurCameraIndex];
			MatrixFrame camFrame = cameraPoint.CamFrame;
			this._camera.Frame = camFrame;
			float aspectRatio = Screen.AspectRatio;
			this._camera.SetFovVertical(1.0471976f, aspectRatio, 0.1f, 500f);
			this._sceneLayer.SetCamera(this._camera);
			VisualTestsScreen.CameraPointTestType cameraPointTestType = cameraPoint.TestTypes[this.TestSubIndex];
			Utilities.SetRenderMode(this.GetRenderMode(cameraPointTestType));
		}

		// Token: 0x06000277 RID: 631 RVA: 0x00016B08 File Offset: 0x00014D08
		protected override void OnFinalize()
		{
			MBDebug.Print("On finalized called for scene: " + this.scene_name, 0, 12, 17592186044416UL);
			base.OnFinalize();
			this._sceneLayer.ClearAll();
			MBAgentRendererSceneController.DestructAgentRendererSceneController(this._scene, this._agentRendererSceneController, false);
			this._agentRendererSceneController = null;
			this._scene = null;
		}

		// Token: 0x06000278 RID: 632 RVA: 0x00016B67 File Offset: 0x00014D67
		public void Reset()
		{
		}

		// Token: 0x06000279 RID: 633 RVA: 0x00016B6C File Offset: 0x00014D6C
		private void TakeScreenshotAndAnalyze()
		{
			VisualTestsScreen.CameraPoint cameraPoint = this.CamPoints[this.CurCameraIndex];
			VisualTestsScreen.CameraPointTestType cameraPointTestType = cameraPoint.TestTypes[this.TestSubIndex];
			this.GetRenderMode(cameraPointTestType);
			bool flag = true;
			string text;
			if (this.isValidTest_)
			{
				text = string.Concat(new string[]
				{
					this._validReadDirectory,
					this.scene_name,
					"_",
					cameraPoint.CameraName,
					"_",
					this.GetSubTestName(cameraPointTestType),
					"_preset_",
					NativeOptions.GetGFXPresetName(this.preset_),
					".bmp"
				});
			}
			else
			{
				text = string.Concat(new string[]
				{
					this._validReadDirectory,
					this.scene_name,
					"_",
					cameraPoint.CameraName,
					"_",
					this.GetSubTestName(cameraPointTestType),
					"_preset_",
					NativeOptions.GetGFXPresetName(this.preset_),
					".bmp"
				});
			}
			string text2 = string.Concat(new string[]
			{
				this.scene_name,
				"_",
				cameraPoint.CameraName,
				"_",
				this.GetSubTestName(cameraPointTestType),
				"_preset_",
				NativeOptions.GetGFXPresetName(this.preset_),
				".bmp"
			});
			string text3 = this._pathDirectory + text2;
			MBDebug.Print(text, 0, 12, 17592186044416UL);
			MBDebug.Print(text3, 0, 12, 17592186044416UL);
			if (this.isValidTest_)
			{
				Utilities.TakeScreenshot(text);
			}
			else
			{
				Utilities.TakeScreenshot(text3);
			}
			NativeOptions.GetGFXPresetName(this.preset_);
			if (!this.isValidTest_)
			{
				if (File.Exists(text))
				{
					if (!this.AnalyzeImageDifferences(text, text3))
					{
						flag = false;
					}
				}
				else
				{
					flag = false;
				}
			}
			if (!flag)
			{
				if (!Directory.Exists(this._failDirectory))
				{
					Directory.CreateDirectory(TestCommonBase.GetAttachmentsFolderPath());
				}
				if (!Directory.Exists(this._failDirectory))
				{
					Directory.CreateDirectory(this._failDirectory);
				}
				string text4 = this._failDirectory + "/" + cameraPoint.CameraName + this.GetSubTestName(cameraPointTestType);
				if (!Directory.Exists(text4))
				{
					Directory.CreateDirectory(text4);
				}
				string text5 = text4 + "/branch_result.bmp";
				string text6 = text4 + "/valid.bmp";
				if (File.Exists(text5))
				{
					File.Delete(text5);
				}
				if (File.Exists(text6))
				{
					File.Delete(text6);
				}
				File.Copy(text3, text5);
				if (File.Exists(text))
				{
					if (File.Exists(text6))
					{
						File.Delete(text6);
					}
					File.Copy(text, text6);
				}
				VisualTestsScreen.isSceneSuccess = false;
			}
			this.TestSubIndex++;
			if (cameraPoint.TestTypes.Count == this.TestSubIndex)
			{
				this.CurCameraIndex++;
				this.TestSubIndex = 0;
			}
		}

		// Token: 0x0600027A RID: 634 RVA: 0x00016E50 File Offset: 0x00015050
		private bool AnalyzeImageDifferences(string path1, string path2)
		{
			byte[] array = File.ReadAllBytes(path1);
			byte[] array2 = File.ReadAllBytes(path2);
			if (array.Length != array2.Length)
			{
				return false;
			}
			float num = 0f;
			for (int i = 0; i < array.Length; i++)
			{
				float num2 = (float)array[i];
				float num3 = (float)array2[i];
				float num4 = MathF.Max(MathF.Abs(num2 - num3), 0f);
				num += num4;
			}
			num /= (float)array.Length;
			return num < 0.5f;
		}

		// Token: 0x04000180 RID: 384
		private Scene _scene;

		// Token: 0x04000181 RID: 385
		private MBAgentRendererSceneController _agentRendererSceneController;

		// Token: 0x04000182 RID: 386
		private Camera _camera;

		// Token: 0x04000183 RID: 387
		private SceneLayer _sceneLayer;

		// Token: 0x04000184 RID: 388
		private List<VisualTestsScreen.CameraPoint> CamPoints;

		// Token: 0x04000185 RID: 389
		private DateTime testTime;

		// Token: 0x04000186 RID: 390
		private string _validWriteDirectory = Utilities.GetVisualTestsValidatePath();

		// Token: 0x04000187 RID: 391
		private string _validReadDirectory = Utilities.GetBasePath() + "ValidVisuals/";

		// Token: 0x04000188 RID: 392
		private string _pathDirectory = Utilities.GetVisualTestsTestFilesPath();

		// Token: 0x04000189 RID: 393
		private string _failDirectory = TestCommonBase.GetAttachmentsFolderPath();

		// Token: 0x0400018A RID: 394
		private string _reportFile = "report.txt";

		// Token: 0x0400018B RID: 395
		private int CurCameraIndex;

		// Token: 0x0400018C RID: 396
		private int TestSubIndex;

		// Token: 0x0400018D RID: 397
		private bool isValidTest_ = true;

		// Token: 0x0400018E RID: 398
		private NativeOptions.ConfigQuality preset_;

		// Token: 0x0400018F RID: 399
		public static bool isSceneSuccess = true;

		// Token: 0x04000190 RID: 400
		private string date;

		// Token: 0x04000191 RID: 401
		private string scene_name;

		// Token: 0x04000192 RID: 402
		private int frameCounter = -200;

		// Token: 0x04000193 RID: 403
		private List<string> testTypesToCheck_ = new List<string>();

		// Token: 0x020000AA RID: 170
		public enum CameraPointTestType
		{
			// Token: 0x04000330 RID: 816
			Final,
			// Token: 0x04000331 RID: 817
			Albedo,
			// Token: 0x04000332 RID: 818
			Normal,
			// Token: 0x04000333 RID: 819
			Specular,
			// Token: 0x04000334 RID: 820
			AO,
			// Token: 0x04000335 RID: 821
			OnlyAmbient,
			// Token: 0x04000336 RID: 822
			OnlyDirect
		}

		// Token: 0x020000AB RID: 171
		public class CameraPoint
		{
			// Token: 0x0600053A RID: 1338 RVA: 0x00026BE5 File Offset: 0x00024DE5
			public CameraPoint()
			{
				this.TestTypes = new List<VisualTestsScreen.CameraPointTestType>();
				this.CamFrame = MatrixFrame.Identity;
				this.CameraName = "";
			}

			// Token: 0x04000337 RID: 823
			public MatrixFrame CamFrame;

			// Token: 0x04000338 RID: 824
			public string CameraName;

			// Token: 0x04000339 RID: 825
			public List<VisualTestsScreen.CameraPointTestType> TestTypes;
		}
	}
}

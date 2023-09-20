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
	public class VisualTestsScreen : ScreenBase
	{
		private int CamPointCount
		{
			get
			{
				return this.CamPoints.Count;
			}
		}

		public bool StartedRendering()
		{
			return this._sceneLayer.SceneView.ReadyToRender();
		}

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

		protected override void OnInitialize()
		{
			base.OnInitialize();
			this._sceneLayer = new SceneLayer("SceneLayer", true, true);
			base.AddLayer(this._sceneLayer);
		}

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

		protected override void OnDeactivate()
		{
			base.OnDeactivate();
		}

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

		private bool ShouldCheckTestModeWithTag(string mode, GameEntity entity)
		{
			if (this.testTypesToCheck_.Count > 0)
			{
				return this.testTypesToCheck_.Contains(mode) && entity.HasTag(mode);
			}
			return entity.HasTag(mode);
		}

		private bool ShouldCheckTestMode(string mode)
		{
			return this.testTypesToCheck_.Contains(mode);
		}

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

		protected override void OnFinalize()
		{
			MBDebug.Print("On finalized called for scene: " + this.scene_name, 0, 12, 17592186044416UL);
			base.OnFinalize();
			this._sceneLayer.ClearAll();
			MBAgentRendererSceneController.DestructAgentRendererSceneController(this._scene, this._agentRendererSceneController, false);
			this._agentRendererSceneController = null;
			this._scene = null;
		}

		public void Reset()
		{
		}

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

		private Scene _scene;

		private MBAgentRendererSceneController _agentRendererSceneController;

		private Camera _camera;

		private SceneLayer _sceneLayer;

		private List<VisualTestsScreen.CameraPoint> CamPoints;

		private DateTime testTime;

		private string _validWriteDirectory = Utilities.GetVisualTestsValidatePath();

		private string _validReadDirectory = Utilities.GetBasePath() + "ValidVisuals/";

		private string _pathDirectory = Utilities.GetVisualTestsTestFilesPath();

		private string _failDirectory = TestCommonBase.GetAttachmentsFolderPath();

		private string _reportFile = "report.txt";

		private int CurCameraIndex;

		private int TestSubIndex;

		private bool isValidTest_ = true;

		private NativeOptions.ConfigQuality preset_;

		public static bool isSceneSuccess = true;

		private string date;

		private string scene_name;

		private int frameCounter = -200;

		private List<string> testTypesToCheck_ = new List<string>();

		public enum CameraPointTestType
		{
			Final,
			Albedo,
			Normal,
			Specular,
			AO,
			OnlyAmbient,
			OnlyDirect
		}

		public class CameraPoint
		{
			public CameraPoint()
			{
				this.TestTypes = new List<VisualTestsScreen.CameraPointTestType>();
				this.CamFrame = MatrixFrame.Identity;
				this.CameraName = "";
			}

			public MatrixFrame CamFrame;

			public string CameraName;

			public List<VisualTestsScreen.CameraPointTestType> TestTypes;
		}
	}
}

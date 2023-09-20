using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	public class ReplayCaptureLogic : MissionView
	{
		private void CheckFixedDeltaTimeMode()
		{
			if (this.RenderActive && this.SaveScreenshots)
			{
				base.Mission.FixedDeltaTime = 0.016666668f;
				base.Mission.FixedDeltaTimeMode = true;
				return;
			}
			base.Mission.FixedDeltaTime = 0f;
			base.Mission.FixedDeltaTimeMode = false;
		}

		private bool RenderActive
		{
			get
			{
				return this._renderActive;
			}
			set
			{
				this._renderActive = value;
				this.CheckFixedDeltaTimeMode();
			}
		}

		private Camera MissionCamera
		{
			get
			{
				if (base.MissionScreen == null || !(base.MissionScreen.CombatCamera != null))
				{
					return null;
				}
				return base.MissionScreen.CombatCamera;
			}
		}

		private float ReplayTime
		{
			get
			{
				return base.Mission.CurrentTime - this._replayTimeDiff;
			}
		}

		private bool SaveScreenshots
		{
			get
			{
				return this._saveScreenshots;
			}
			set
			{
				this._saveScreenshots = value;
				this.CheckFixedDeltaTimeMode();
			}
		}

		private KeyValuePair<float, MatrixFrame> PreviousKey
		{
			get
			{
				return this.GetPreviousKey();
			}
		}

		private KeyValuePair<float, MatrixFrame> NextKey
		{
			get
			{
				return this.GetNextKey();
			}
		}

		private KeyValuePair<float, MatrixFrame> GetPreviousKey()
		{
			KeyValuePair<float, MatrixFrame> keyValuePair = this._invalid;
			if (!this._cameraKeys.Any<KeyValuePair<float, SortedDictionary<int, MatrixFrame>>>())
			{
				return keyValuePair;
			}
			foreach (KeyValuePair<float, SortedDictionary<int, MatrixFrame>> keyValuePair2 in this._cameraKeys)
			{
				if (keyValuePair2.Key <= this.ReplayTime)
				{
					keyValuePair = new KeyValuePair<float, MatrixFrame>(keyValuePair2.Key, keyValuePair2.Value[keyValuePair2.Value.Count - 1]);
				}
			}
			return keyValuePair;
		}

		private KeyValuePair<float, MatrixFrame> GetNextKey()
		{
			KeyValuePair<float, MatrixFrame> keyValuePair = this._invalid;
			if (!this._cameraKeys.Any<KeyValuePair<float, SortedDictionary<int, MatrixFrame>>>())
			{
				return keyValuePair;
			}
			foreach (KeyValuePair<float, SortedDictionary<int, MatrixFrame>> keyValuePair2 in this._cameraKeys)
			{
				if (keyValuePair2.Key > this.ReplayTime)
				{
					keyValuePair = new KeyValuePair<float, MatrixFrame>(keyValuePair2.Key, keyValuePair2.Value[0]);
					break;
				}
			}
			return keyValuePair;
		}

		public ReplayCaptureLogic()
		{
			this._cameraKeys = new SortedDictionary<float, SortedDictionary<int, MatrixFrame>>();
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._replayLogic = base.Mission.GetMissionBehavior<ReplayMissionView>();
			this._replayLogic.OverrideInput(true);
			if (!MBCommon.IsPaused)
			{
				this._replayLogic.Pause();
			}
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (this._frameSkip && !MBCommon.IsPaused)
			{
				if (!this._isRendered)
				{
					this._isRendered = true;
					return;
				}
				this._replayLogic.Pause();
				this._frameSkip = false;
			}
			if (this.RenderActive)
			{
				this.SaveScreenshot();
				if (!base.Mission.Recorder.IsEndOfRecord())
				{
					KeyValuePair<float, MatrixFrame> previousKey = this.PreviousKey;
					KeyValuePair<float, MatrixFrame> nextKey = this.NextKey;
					this._replayLogic.Resume();
					if (nextKey.Key >= 0f)
					{
						for (int i = 0; i < this._cameraKeys.Count; i++)
						{
							if (previousKey.Key == this._cameraKeys.ElementAt(i).Key)
							{
								float num = nextKey.Key - previousKey.Key;
								float num2 = (this.ReplayTime - previousKey.Key) / num;
								int count = this._cameraKeys[previousKey.Key].Count;
								MatrixFrame matrixFrame;
								if (this._lastUsedIndex != i && count > 1)
								{
									matrixFrame = this._cameraKeys[previousKey.Key][count - 1];
								}
								else
								{
									MatrixFrame matrixFrame2 = default(MatrixFrame);
									matrixFrame2.origin = this._path.GetHermiteFrameForDt(num2, i).origin;
									matrixFrame = matrixFrame2;
									Vec3 vec = previousKey.Value.rotation.s * (1f - num2) + nextKey.Value.rotation.s * num2;
									Vec3 vec2 = previousKey.Value.rotation.u * (1f - num2) + nextKey.Value.rotation.u * num2;
									Vec3 vec3 = previousKey.Value.rotation.f * (1f - num2) + nextKey.Value.rotation.f * num2;
									matrixFrame.rotation.s = vec;
									matrixFrame.rotation.u = vec2;
									matrixFrame.rotation.f = vec3;
								}
								matrixFrame.rotation.s.Normalize();
								matrixFrame.rotation.u.Normalize();
								matrixFrame.rotation.f.Normalize();
								matrixFrame.rotation.Orthonormalize();
								base.MissionScreen.CustomCamera.Frame = matrixFrame;
								this._lastUsedIndex = i;
								return;
							}
						}
						return;
					}
					if (previousKey.Key >= 0f)
					{
						int count2 = this._cameraKeys[previousKey.Key].Count;
						if (count2 > 1)
						{
							MatrixFrame matrixFrame3 = this._cameraKeys[previousKey.Key][count2 - 1];
							matrixFrame3.rotation.s.Normalize();
							matrixFrame3.rotation.u.Normalize();
							matrixFrame3.rotation.f.Normalize();
							matrixFrame3.rotation.Orthonormalize();
							base.MissionScreen.CustomCamera.Frame = matrixFrame3;
							return;
						}
					}
				}
				else
				{
					MBDebug.Print("All images are saved.", 0, 6, 64UL);
					this.RenderActive = false;
					this._replayLogic.ResetReplay();
					this._replayTimeDiff = base.Mission.CurrentTime;
					base.MissionScreen.CustomCamera = null;
					this._replayLogic.Pause();
					this.SaveScreenshots = false;
					this._ssNum = 0;
				}
				return;
			}
		}

		private void InsertCamKey()
		{
			float replayTime = this.ReplayTime;
			MatrixFrame frame = this.MissionCamera.Frame;
			int num = 0;
			if (this._cameraKeys.ContainsKey(replayTime))
			{
				num = this._cameraKeys[replayTime].Count;
				this._cameraKeys[replayTime].Add(num, frame);
			}
			else
			{
				this._cameraKeys.Add(replayTime, new SortedDictionary<int, MatrixFrame> { { num, frame } });
			}
			MBDebug.Print(string.Concat(new object[] { "Keyframe to \"", replayTime, "\" has been inserted with the index: ", num, ".\n" }), 0, 4, 64UL);
		}

		private void MoveToNextFrame()
		{
			this._replayLogic.FastForward(0.016666668f);
			this._replayLogic.Resume();
			this._frameSkip = true;
		}

		private void GoToKey(float keyTime)
		{
			if (keyTime < 0f || !this._cameraKeys.ContainsKey(keyTime) || keyTime == this.ReplayTime)
			{
				return;
			}
			MatrixFrame matrixFrame;
			if (keyTime < this.ReplayTime)
			{
				matrixFrame = this._cameraKeys[keyTime][this._cameraKeys[keyTime].Count - 1];
				this._replayLogic.Rewind(this.ReplayTime - keyTime);
				this._replayTimeDiff = base.Mission.CurrentTime;
			}
			else
			{
				matrixFrame = this._cameraKeys[keyTime][0];
				this._replayLogic.FastForward(keyTime - this.ReplayTime);
			}
			this.MissionCamera.Frame = matrixFrame;
		}

		private void SetPath()
		{
			if (base.Mission.Scene.GetPathWithName("CameraPath") != null)
			{
				base.Mission.Scene.DeletePathWithName("CameraPath");
			}
			base.Mission.Scene.AddPath("CameraPath");
			foreach (KeyValuePair<float, SortedDictionary<int, MatrixFrame>> keyValuePair in this._cameraKeys)
			{
				base.Mission.Scene.AddPathPoint("CameraPath", keyValuePair.Value[0]);
			}
			this._path = base.Mission.Scene.GetPathWithName("CameraPath");
		}

		private void Render(bool saveScreenshots = false)
		{
			if (!this._cameraKeys.ContainsKey(0f))
			{
				this._cameraKeys.Add(0f, new SortedDictionary<int, MatrixFrame> { 
				{
					0,
					this.MissionCamera.Frame
				} });
			}
			else
			{
				this._cameraKeys[0f] = new SortedDictionary<int, MatrixFrame> { 
				{
					0,
					this.MissionCamera.Frame
				} };
			}
			this._replayLogic.ResetReplay();
			this._replayLogic.Pause();
			this._replayTimeDiff = base.Mission.CurrentTime;
			this.SetPath();
			this.SaveScreenshots = saveScreenshots;
			this.RenderActive = true;
			this._lastUsedIndex = 0;
			base.MissionScreen.CustomCamera = base.MissionScreen.CombatCamera;
		}

		private void SaveScreenshot()
		{
			if (!this.SaveScreenshots)
			{
				return;
			}
			if (string.IsNullOrEmpty(this._directoryPath.Path))
			{
				PlatformDirectoryPath platformDirectoryPath;
				platformDirectoryPath..ctor(0, "Captures");
				string text = "Cap_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
				this._directoryPath = platformDirectoryPath + text;
			}
			Utilities.TakeScreenshot(new PlatformFilePath(this._directoryPath, "time_" + string.Format("{0:000000}", this._ssNum) + ".bmp"));
			this._ssNum++;
		}

		private ReplayMissionView _replayLogic;

		private bool _renderActive;

		public const float CaptureFrameRate = 60f;

		private float _replayTimeDiff;

		private bool _frameSkip;

		private Path _path;

		private PlatformDirectoryPath _directoryPath;

		private bool _saveScreenshots;

		private readonly KeyValuePair<float, MatrixFrame> _invalid = new KeyValuePair<float, MatrixFrame>(-1f, default(MatrixFrame));

		private SortedDictionary<float, SortedDictionary<int, MatrixFrame>> _cameraKeys;

		private bool _isRendered;

		private int _lastUsedIndex;

		private int _ssNum;
	}
}

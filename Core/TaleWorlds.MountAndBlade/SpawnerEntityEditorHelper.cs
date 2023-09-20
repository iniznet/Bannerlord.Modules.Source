using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class SpawnerEntityEditorHelper
	{
		public bool IsValid { get; private set; }

		public GameEntity SpawnedGhostEntity { get; private set; }

		public SpawnerEntityEditorHelper(ScriptComponentBehavior spawner)
		{
			this.spawner_ = spawner;
			if (this.AddGhostEntity(this.spawner_.GameEntity, this.GetGhostName()) != null)
			{
				this.SyncMatrixFrames(true);
				this.IsValid = true;
				return;
			}
			Debug.FailedAssert("No prefab found. Spawner script will remove itself.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\SpawnerEntityEditorHelper.cs", ".ctor", 75);
			spawner.GameEntity.RemoveScriptComponent(this.spawner_.ScriptComponent.Pointer, 11);
		}

		public GameEntity GetGhostEntityOrChild(string name)
		{
			if (this.SpawnedGhostEntity.Name == name)
			{
				return this.SpawnedGhostEntity;
			}
			List<GameEntity> list = new List<GameEntity>();
			this.SpawnedGhostEntity.GetChildrenRecursive(ref list);
			GameEntity gameEntity = list.FirstOrDefault((GameEntity x) => x.Name == name);
			if (gameEntity != null)
			{
				return gameEntity;
			}
			return null;
		}

		public void Tick(float dt)
		{
			if (this.SpawnedGhostEntity.Parent != this.spawner_.GameEntity)
			{
				this.IsValid = false;
				this.spawner_.GameEntity.RemoveScriptComponent(this.spawner_.ScriptComponent.Pointer, 12);
			}
			if (this.IsValid)
			{
				if (this.LockGhostParent)
				{
					bool flag = this.SpawnedGhostEntity.GetFrame() != MatrixFrame.Identity;
					MatrixFrame identity = MatrixFrame.Identity;
					this.SpawnedGhostEntity.SetFrame(ref identity);
					if (flag)
					{
						this.SpawnedGhostEntity.UpdateTriadFrameForEditor();
					}
				}
				this.SyncMatrixFrames(false);
				if (this._ghostMovementMode)
				{
					this.UpdateGhostMovement(dt);
				}
			}
		}

		public void GivePermission(string childName, SpawnerEntityEditorHelper.Permission permission, Action<float> onChangeFunction)
		{
			this._stableChildrenPermissions.Add(Tuple.Create<string, SpawnerEntityEditorHelper.Permission, Action<float>>(childName, permission, onChangeFunction));
		}

		private void ApplyPermissions()
		{
			using (List<Tuple<string, SpawnerEntityEditorHelper.Permission, Action<float>>>.Enumerator enumerator = this._stableChildrenPermissions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Tuple<string, SpawnerEntityEditorHelper.Permission, Action<float>> item = enumerator.Current;
					KeyValuePair<string, MatrixFrame> keyValuePair = this.stableChildrenFrames.Find((KeyValuePair<string, MatrixFrame> x) => x.Key == item.Item1);
					MatrixFrame frame = this.GetGhostEntityOrChild(item.Item1).GetFrame();
					if (!frame.NearlyEquals(keyValuePair.Value, 1E-05f))
					{
						SpawnerEntityEditorHelper.PermissionType typeOfPermission = item.Item2.TypeOfPermission;
						if (typeOfPermission != SpawnerEntityEditorHelper.PermissionType.scale)
						{
							if (typeOfPermission == SpawnerEntityEditorHelper.PermissionType.rotation)
							{
								switch (item.Item2.PermittedAxis)
								{
								case SpawnerEntityEditorHelper.Axis.x:
									if (!frame.rotation.f.NearlyEquals(keyValuePair.Value.rotation.f, 1E-05f) && !frame.rotation.u.NearlyEquals(keyValuePair.Value.rotation.u, 1E-05f) && frame.rotation.s.NearlyEquals(keyValuePair.Value.rotation.s, 1E-05f))
									{
										this.ChangeStableChildMatrixFrame(item.Item1, frame);
										item.Item3(frame.rotation.GetEulerAngles().x);
									}
									break;
								case SpawnerEntityEditorHelper.Axis.y:
									if (!frame.rotation.s.NearlyEquals(keyValuePair.Value.rotation.s, 1E-05f) && !frame.rotation.u.NearlyEquals(keyValuePair.Value.rotation.u, 1E-05f) && frame.rotation.f.NearlyEquals(keyValuePair.Value.rotation.f, 1E-05f))
									{
										this.ChangeStableChildMatrixFrame(item.Item1, frame);
										item.Item3(frame.rotation.GetEulerAngles().y);
									}
									break;
								case SpawnerEntityEditorHelper.Axis.z:
									if (!frame.rotation.f.NearlyEquals(keyValuePair.Value.rotation.f, 1E-05f) && !frame.rotation.s.NearlyEquals(keyValuePair.Value.rotation.s, 1E-05f) && frame.rotation.u.NearlyEquals(keyValuePair.Value.rotation.u, 1E-05f))
									{
										this.ChangeStableChildMatrixFrame(item.Item1, frame);
										item.Item3(frame.rotation.GetEulerAngles().z);
									}
									break;
								}
							}
						}
						else if (frame.origin.NearlyEquals(keyValuePair.Value.origin, 0.0001f))
						{
							Vec3 vec = frame.rotation.f.NormalizedCopy();
							MatrixFrame matrixFrame = keyValuePair.Value;
							if (vec.NearlyEquals(matrixFrame.rotation.f.NormalizedCopy(), 0.0001f))
							{
								vec = frame.rotation.u.NormalizedCopy();
								matrixFrame = keyValuePair.Value;
								if (vec.NearlyEquals(matrixFrame.rotation.u.NormalizedCopy(), 0.0001f))
								{
									vec = frame.rotation.s.NormalizedCopy();
									matrixFrame = keyValuePair.Value;
									if (vec.NearlyEquals(matrixFrame.rotation.s.NormalizedCopy(), 0.0001f))
									{
										switch (item.Item2.PermittedAxis)
										{
										case SpawnerEntityEditorHelper.Axis.x:
											if (!frame.rotation.f.NearlyEquals(keyValuePair.Value.rotation.f, 1E-05f))
											{
												this.ChangeStableChildMatrixFrame(item.Item1, frame);
												item.Item3(frame.rotation.f.Length);
											}
											break;
										case SpawnerEntityEditorHelper.Axis.y:
											if (!frame.rotation.s.NearlyEquals(keyValuePair.Value.rotation.s, 1E-05f))
											{
												this.ChangeStableChildMatrixFrame(item.Item1, frame);
												item.Item3(frame.rotation.s.Length);
											}
											break;
										case SpawnerEntityEditorHelper.Axis.z:
											if (!frame.rotation.u.NearlyEquals(keyValuePair.Value.rotation.u, 1E-05f))
											{
												this.ChangeStableChildMatrixFrame(item.Item1, frame);
												item.Item3(frame.rotation.u.Length);
											}
											break;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private void ChangeStableChildMatrixFrame(string childName, MatrixFrame matrixFrame)
		{
			this.stableChildrenFrames.RemoveAll((KeyValuePair<string, MatrixFrame> x) => x.Key == childName);
			KeyValuePair<string, MatrixFrame> keyValuePair = new KeyValuePair<string, MatrixFrame>(childName, matrixFrame);
			this.stableChildrenFrames.Add(keyValuePair);
			if (SpawnerEntityEditorHelper.HasField(this.spawner_, childName, true))
			{
				SpawnerEntityEditorHelper.SetSpawnerMatrixFrame(this.spawner_, childName, matrixFrame);
			}
		}

		public void ChangeStableChildMatrixFrameAndApply(string childName, MatrixFrame matrixFrame, bool updateTriad = true)
		{
			this.ChangeStableChildMatrixFrame(childName, matrixFrame);
			this.GetGhostEntityOrChild(childName).SetFrame(ref matrixFrame);
			if (updateTriad)
			{
				this.SpawnedGhostEntity.UpdateTriadFrameForEditorForAllChildren();
			}
		}

		private GameEntity AddGhostEntity(GameEntity parent, List<string> possibleEntityNames)
		{
			this.spawner_.GameEntity.RemoveAllChildren();
			foreach (string text in possibleEntityNames)
			{
				if (GameEntity.PrefabExists(text))
				{
					this.SpawnedGhostEntity = GameEntity.Instantiate(parent.Scene, text, true);
					break;
				}
			}
			if (this.SpawnedGhostEntity == null)
			{
				return null;
			}
			this.SpawnedGhostEntity.SetMobility(GameEntity.Mobility.dynamic);
			this.SpawnedGhostEntity.EntityFlags |= EntityFlags.DontSaveToScene;
			parent.AddChild(this.SpawnedGhostEntity, false);
			MatrixFrame identity = MatrixFrame.Identity;
			this.SpawnedGhostEntity.SetFrame(ref identity);
			this.GetChildrenInitialFrames();
			this.SpawnedGhostEntity.UpdateTriadFrameForEditorForAllChildren();
			return this.SpawnedGhostEntity;
		}

		private void SyncMatrixFrames(bool first)
		{
			this.ApplyPermissions();
			List<GameEntity> list = new List<GameEntity>();
			this.SpawnedGhostEntity.GetChildrenRecursive(ref list);
			using (List<GameEntity>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GameEntity item = enumerator.Current;
					if (SpawnerEntityEditorHelper.HasField(this.spawner_, item.Name, false))
					{
						if (first)
						{
							MatrixFrame matrixFrame = (MatrixFrame)SpawnerEntityEditorHelper.GetFieldValue(this.spawner_, item.Name);
							if (!matrixFrame.IsZero)
							{
								item.SetFrame(ref matrixFrame);
							}
						}
						else
						{
							SpawnerEntityEditorHelper.SetSpawnerMatrixFrame(this.spawner_, item.Name, item.GetFrame());
						}
					}
					else
					{
						MatrixFrame value = this.stableChildrenFrames.Find((KeyValuePair<string, MatrixFrame> x) => x.Key == item.Name).Value;
						if (!value.NearlyEquals(item.GetFrame(), 1E-05f))
						{
							item.SetFrame(ref value);
							this.SpawnedGhostEntity.UpdateTriadFrameForEditorForAllChildren();
						}
					}
				}
			}
		}

		private void GetChildrenInitialFrames()
		{
			List<GameEntity> list = new List<GameEntity>();
			this.SpawnedGhostEntity.GetChildrenRecursive(ref list);
			foreach (GameEntity gameEntity in list)
			{
				if (!SpawnerEntityEditorHelper.HasField(this.spawner_, gameEntity.Name, false))
				{
					this.stableChildrenFrames.Add(new KeyValuePair<string, MatrixFrame>(gameEntity.Name, gameEntity.GetFrame()));
				}
			}
		}

		private List<string> GetGhostName()
		{
			string text = this.GetPrefabName();
			List<string> list = new List<string>();
			list.Add(text + "_ghost");
			text = text.Remove(text.Length - text.Split(new char[] { '_' }).Last<string>().Length - 1);
			list.Add(text + "_ghost");
			return list;
		}

		public string GetPrefabName()
		{
			return this.spawner_.GameEntity.Name.Remove(this.spawner_.GameEntity.Name.Length - this.spawner_.GameEntity.Name.Split(new char[] { '_' }).Last<string>().Length - 1);
		}

		public void SetupGhostMovement(string pathName)
		{
			this._ghostMovementMode = true;
			this._pathName = pathName;
			Path pathWithName = this.SpawnedGhostEntity.Scene.GetPathWithName(pathName);
			Vec3 scaleVector = this.SpawnedGhostEntity.GetFrame().rotation.GetScaleVector();
			this._tracker = new PathTracker(pathWithName, scaleVector);
			this._ghostObjectPosition = ((pathWithName != null) ? pathWithName.GetTotalLength() : 0f);
			this.SpawnedGhostEntity.UpdateTriadFrameForEditor();
			List<GameEntity> list = new List<GameEntity>();
			this.SpawnedGhostEntity.GetChildrenRecursive(ref list);
			this._wheels.Clear();
			this._wheels.AddRange(list.Where((GameEntity x) => x.HasTag("wheel")));
		}

		public void SetEnableAutoGhostMovement(bool enableAutoGhostMovement)
		{
			this._enableAutoGhostMovement = enableAutoGhostMovement;
			if (!this._enableAutoGhostMovement && this._tracker.IsValid)
			{
				this._ghostObjectPosition = this._tracker.GetPathLength();
			}
		}

		private void UpdateGhostMovement(float dt)
		{
			if (this._tracker.HasChanged)
			{
				this.SetupGhostMovement(this._pathName);
				this._tracker.Advance(this._tracker.GetPathLength());
			}
			if (this.spawner_.GameEntity.IsSelectedOnEditor() || this.SpawnedGhostEntity.IsSelectedOnEditor())
			{
				if (this._tracker.IsValid)
				{
					float num = 10f;
					if (Input.DebugInput.IsShiftDown())
					{
						num = 1f;
					}
					if (Input.DebugInput.IsKeyDown(InputKey.MouseScrollUp))
					{
						this._ghostObjectPosition += dt * num;
					}
					else if (Input.DebugInput.IsKeyDown(InputKey.MouseScrollDown))
					{
						this._ghostObjectPosition -= dt * num;
					}
					if (this._enableAutoGhostMovement)
					{
						this._ghostObjectPosition += dt * num;
						if (this._ghostObjectPosition >= this._tracker.GetPathLength())
						{
							this._ghostObjectPosition = 0f;
						}
					}
					this._ghostObjectPosition = MBMath.ClampFloat(this._ghostObjectPosition, 0f, this._tracker.GetPathLength());
				}
				else
				{
					this._ghostObjectPosition = 0f;
				}
			}
			if (this._tracker.IsValid)
			{
				MatrixFrame globalFrame = this.spawner_.GameEntity.GetGlobalFrame();
				this._tracker.Advance(0f);
				MatrixFrame matrixFrame;
				Vec3 vec;
				this._tracker.CurrentFrameAndColor(out matrixFrame, out vec);
				if (globalFrame != matrixFrame)
				{
					this.spawner_.GameEntity.SetGlobalFrame(matrixFrame);
					this.spawner_.GameEntity.UpdateTriadFrameForEditor();
				}
				this._tracker.Advance(this._ghostObjectPosition);
				this._tracker.CurrentFrameAndColor(out matrixFrame, out vec);
				if (this._wheels.Count == 2)
				{
					matrixFrame = this.LinearInterpolatedIK(ref this._tracker);
				}
				if (globalFrame != matrixFrame)
				{
					this.SpawnedGhostEntity.SetGlobalFrame(matrixFrame);
					this.SpawnedGhostEntity.UpdateTriadFrameForEditor();
				}
				this._tracker.Reset();
				return;
			}
			if (this.SpawnedGhostEntity.GetGlobalFrame() != this.spawner_.GameEntity.GetGlobalFrame())
			{
				GameEntity spawnedGhostEntity = this.SpawnedGhostEntity;
				MatrixFrame globalFrame2 = this.spawner_.GameEntity.GetGlobalFrame();
				spawnedGhostEntity.SetGlobalFrame(globalFrame2);
				this.SpawnedGhostEntity.UpdateTriadFrameForEditor();
			}
		}

		private MatrixFrame LinearInterpolatedIK(ref PathTracker pathTracker)
		{
			MatrixFrame matrixFrame;
			Vec3 vec;
			pathTracker.CurrentFrameAndColor(out matrixFrame, out vec);
			MatrixFrame matrixFrame2 = SiegeWeaponMovementComponent.FindGroundFrameForWheelsStatic(ref matrixFrame, 2.45f, 1.3f, this.SpawnedGhostEntity, this._wheels, this.SpawnedGhostEntity.Scene);
			return MatrixFrame.Lerp(matrixFrame, matrixFrame2, vec.x);
		}

		private static object GetFieldValue(object src, string propName)
		{
			return src.GetType().GetField(propName).GetValue(src);
		}

		private static bool HasField(object obj, string propertyName, bool findRestricted)
		{
			return obj.GetType().GetField(propertyName) != null && (findRestricted || obj.GetType().GetField(propertyName).GetCustomAttribute<RestrictedAccess>() == null);
		}

		private static bool SetSpawnerMatrixFrame(object target, string propertyName, MatrixFrame value)
		{
			value.Fill();
			FieldInfo field = target.GetType().GetField(propertyName);
			if (field != null)
			{
				field.SetValue(target, value);
				return true;
			}
			return false;
		}

		private List<Tuple<string, SpawnerEntityEditorHelper.Permission, Action<float>>> _stableChildrenPermissions = new List<Tuple<string, SpawnerEntityEditorHelper.Permission, Action<float>>>();

		private ScriptComponentBehavior spawner_;

		private List<KeyValuePair<string, MatrixFrame>> stableChildrenFrames = new List<KeyValuePair<string, MatrixFrame>>();

		public bool LockGhostParent = true;

		private bool _ghostMovementMode;

		private PathTracker _tracker;

		private float _ghostObjectPosition;

		private string _pathName;

		private bool _enableAutoGhostMovement;

		private readonly List<GameEntity> _wheels = new List<GameEntity>();

		public enum Axis
		{
			x,
			y,
			z
		}

		public enum PermissionType
		{
			scale,
			rotation
		}

		public struct Permission
		{
			public Permission(SpawnerEntityEditorHelper.PermissionType permission, SpawnerEntityEditorHelper.Axis axis)
			{
				this.TypeOfPermission = permission;
				this.PermittedAxis = axis;
			}

			public SpawnerEntityEditorHelper.PermissionType TypeOfPermission;

			public SpawnerEntityEditorHelper.Axis PermittedAxis;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200035E RID: 862
	public class SpawnerEntityEditorHelper
	{
		// Token: 0x1700087C RID: 2172
		// (get) Token: 0x06002F10 RID: 12048 RVA: 0x000BF642 File Offset: 0x000BD842
		// (set) Token: 0x06002F11 RID: 12049 RVA: 0x000BF64A File Offset: 0x000BD84A
		public bool IsValid { get; private set; }

		// Token: 0x1700087D RID: 2173
		// (get) Token: 0x06002F12 RID: 12050 RVA: 0x000BF653 File Offset: 0x000BD853
		// (set) Token: 0x06002F13 RID: 12051 RVA: 0x000BF65B File Offset: 0x000BD85B
		public GameEntity SpawnedGhostEntity { get; private set; }

		// Token: 0x06002F14 RID: 12052 RVA: 0x000BF664 File Offset: 0x000BD864
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

		// Token: 0x06002F15 RID: 12053 RVA: 0x000BF708 File Offset: 0x000BD908
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

		// Token: 0x06002F16 RID: 12054 RVA: 0x000BF774 File Offset: 0x000BD974
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

		// Token: 0x06002F17 RID: 12055 RVA: 0x000BF822 File Offset: 0x000BDA22
		public void GivePermission(string childName, SpawnerEntityEditorHelper.Permission permission, Action<float> onChangeFunction)
		{
			this._stableChildrenPermissions.Add(Tuple.Create<string, SpawnerEntityEditorHelper.Permission, Action<float>>(childName, permission, onChangeFunction));
		}

		// Token: 0x06002F18 RID: 12056 RVA: 0x000BF838 File Offset: 0x000BDA38
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

		// Token: 0x06002F19 RID: 12057 RVA: 0x000BFD90 File Offset: 0x000BDF90
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

		// Token: 0x06002F1A RID: 12058 RVA: 0x000BFE03 File Offset: 0x000BE003
		public void ChangeStableChildMatrixFrameAndApply(string childName, MatrixFrame matrixFrame, bool updateTriad = true)
		{
			this.ChangeStableChildMatrixFrame(childName, matrixFrame);
			this.GetGhostEntityOrChild(childName).SetFrame(ref matrixFrame);
			if (updateTriad)
			{
				this.SpawnedGhostEntity.UpdateTriadFrameForEditorForAllChildren();
			}
		}

		// Token: 0x06002F1B RID: 12059 RVA: 0x000BFE2C File Offset: 0x000BE02C
		private GameEntity AddGhostEntity(GameEntity parent, string entityName)
		{
			this.spawner_.GameEntity.RemoveAllChildren();
			this.SpawnedGhostEntity = GameEntity.Instantiate(parent.Scene, entityName, true);
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

		// Token: 0x06002F1C RID: 12060 RVA: 0x000BFEC8 File Offset: 0x000BE0C8
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

		// Token: 0x06002F1D RID: 12061 RVA: 0x000C0004 File Offset: 0x000BE204
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

		// Token: 0x06002F1E RID: 12062 RVA: 0x000C0090 File Offset: 0x000BE290
		private string GetGhostName()
		{
			return this.GetPrefabName() + "_ghost";
		}

		// Token: 0x06002F1F RID: 12063 RVA: 0x000C00A4 File Offset: 0x000BE2A4
		public string GetPrefabName()
		{
			return this.spawner_.GameEntity.Name.Remove(this.spawner_.GameEntity.Name.Length - this.spawner_.GameEntity.Name.Split(new char[] { '_' }).Last<string>().Length - 1);
		}

		// Token: 0x06002F20 RID: 12064 RVA: 0x000C0108 File Offset: 0x000BE308
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

		// Token: 0x06002F21 RID: 12065 RVA: 0x000C01D0 File Offset: 0x000BE3D0
		public void SetEnableAutoGhostMovement(bool enableAutoGhostMovement)
		{
			this._enableAutoGhostMovement = enableAutoGhostMovement;
			if (!this._enableAutoGhostMovement && this._tracker.IsValid)
			{
				this._ghostObjectPosition = this._tracker.GetPathLength();
			}
		}

		// Token: 0x06002F22 RID: 12066 RVA: 0x000C0200 File Offset: 0x000BE400
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

		// Token: 0x06002F23 RID: 12067 RVA: 0x000C044C File Offset: 0x000BE64C
		private MatrixFrame LinearInterpolatedIK(ref PathTracker pathTracker)
		{
			MatrixFrame matrixFrame;
			Vec3 vec;
			pathTracker.CurrentFrameAndColor(out matrixFrame, out vec);
			MatrixFrame matrixFrame2 = SiegeWeaponMovementComponent.FindGroundFrameForWheelsStatic(ref matrixFrame, 2.45f, 1.3f, this.SpawnedGhostEntity, this._wheels, this.SpawnedGhostEntity.Scene);
			return MatrixFrame.Lerp(matrixFrame, matrixFrame2, vec.x);
		}

		// Token: 0x06002F24 RID: 12068 RVA: 0x000C049A File Offset: 0x000BE69A
		private static object GetFieldValue(object src, string propName)
		{
			return src.GetType().GetField(propName).GetValue(src);
		}

		// Token: 0x06002F25 RID: 12069 RVA: 0x000C04AE File Offset: 0x000BE6AE
		private static bool HasField(object obj, string propertyName, bool findRestricted)
		{
			return obj.GetType().GetField(propertyName) != null && (findRestricted || obj.GetType().GetField(propertyName).GetCustomAttribute<RestrictedAccess>() == null);
		}

		// Token: 0x06002F26 RID: 12070 RVA: 0x000C04E0 File Offset: 0x000BE6E0
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

		// Token: 0x04001347 RID: 4935
		private List<Tuple<string, SpawnerEntityEditorHelper.Permission, Action<float>>> _stableChildrenPermissions = new List<Tuple<string, SpawnerEntityEditorHelper.Permission, Action<float>>>();

		// Token: 0x04001348 RID: 4936
		private ScriptComponentBehavior spawner_;

		// Token: 0x04001349 RID: 4937
		private List<KeyValuePair<string, MatrixFrame>> stableChildrenFrames = new List<KeyValuePair<string, MatrixFrame>>();

		// Token: 0x0400134C RID: 4940
		public bool LockGhostParent = true;

		// Token: 0x0400134D RID: 4941
		private bool _ghostMovementMode;

		// Token: 0x0400134E RID: 4942
		private PathTracker _tracker;

		// Token: 0x0400134F RID: 4943
		private float _ghostObjectPosition;

		// Token: 0x04001350 RID: 4944
		private string _pathName;

		// Token: 0x04001351 RID: 4945
		private bool _enableAutoGhostMovement;

		// Token: 0x04001352 RID: 4946
		private readonly List<GameEntity> _wheels = new List<GameEntity>();

		// Token: 0x02000670 RID: 1648
		public enum Axis
		{
			// Token: 0x040020ED RID: 8429
			x,
			// Token: 0x040020EE RID: 8430
			y,
			// Token: 0x040020EF RID: 8431
			z
		}

		// Token: 0x02000671 RID: 1649
		public enum PermissionType
		{
			// Token: 0x040020F1 RID: 8433
			scale,
			// Token: 0x040020F2 RID: 8434
			rotation
		}

		// Token: 0x02000672 RID: 1650
		public struct Permission
		{
			// Token: 0x06003E95 RID: 16021 RVA: 0x000F5B7B File Offset: 0x000F3D7B
			public Permission(SpawnerEntityEditorHelper.PermissionType permission, SpawnerEntityEditorHelper.Axis axis)
			{
				this.TypeOfPermission = permission;
				this.PermittedAxis = axis;
			}

			// Token: 0x040020F3 RID: 8435
			public SpawnerEntityEditorHelper.PermissionType TypeOfPermission;

			// Token: 0x040020F4 RID: 8436
			public SpawnerEntityEditorHelper.Axis PermittedAxis;
		}
	}
}

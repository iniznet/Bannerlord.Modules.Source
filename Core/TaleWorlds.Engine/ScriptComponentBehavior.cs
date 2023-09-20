using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000082 RID: 130
	public abstract class ScriptComponentBehavior : DotNetObject
	{
		// Token: 0x060009DA RID: 2522 RVA: 0x0000A93F File Offset: 0x00008B3F
		protected void InvalidateWeakPointersIfValid()
		{
			this._gameEntity.ManualInvalidate();
			this._scriptComponent.ManualInvalidate();
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x060009DB RID: 2523 RVA: 0x0000A957 File Offset: 0x00008B57
		// (set) Token: 0x060009DC RID: 2524 RVA: 0x0000A970 File Offset: 0x00008B70
		public GameEntity GameEntity
		{
			get
			{
				WeakNativeObjectReference gameEntity = this._gameEntity;
				return ((gameEntity != null) ? gameEntity.GetNativeObject() : null) as GameEntity;
			}
			private set
			{
				this._gameEntity = new WeakNativeObjectReference(value);
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060009DD RID: 2525 RVA: 0x0000A97E File Offset: 0x00008B7E
		// (set) Token: 0x060009DE RID: 2526 RVA: 0x0000A997 File Offset: 0x00008B97
		public ManagedScriptComponent ScriptComponent
		{
			get
			{
				WeakNativeObjectReference scriptComponent = this._scriptComponent;
				return ((scriptComponent != null) ? scriptComponent.GetNativeObject() : null) as ManagedScriptComponent;
			}
			private set
			{
				this._scriptComponent = new WeakNativeObjectReference(value);
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x060009DF RID: 2527 RVA: 0x0000A9A5 File Offset: 0x00008BA5
		// (set) Token: 0x060009E0 RID: 2528 RVA: 0x0000A9AD File Offset: 0x00008BAD
		private protected ManagedScriptHolder ManagedScriptHolder { protected get; private set; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x060009E1 RID: 2529 RVA: 0x0000A9B6 File Offset: 0x00008BB6
		// (set) Token: 0x060009E2 RID: 2530 RVA: 0x0000A9CF File Offset: 0x00008BCF
		public Scene Scene
		{
			get
			{
				WeakNativeObjectReference scene = this._scene;
				return ((scene != null) ? scene.GetNativeObject() : null) as Scene;
			}
			private set
			{
				this._scene = new WeakNativeObjectReference(value);
			}
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x0000A9DD File Offset: 0x00008BDD
		static ScriptComponentBehavior()
		{
			if (ScriptComponentBehavior.CachedFields == null)
			{
				ScriptComponentBehavior.CachedFields = new Dictionary<string, string[]>();
				ScriptComponentBehavior.CacheEditableFieldsForAllScriptComponents();
			}
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x0000AA11 File Offset: 0x00008C11
		internal void Construct(GameEntity myEntity, ManagedScriptComponent scriptComponent)
		{
			this.GameEntity = myEntity;
			this.Scene = myEntity.Scene;
			this.ScriptComponent = scriptComponent;
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x0000AA2D File Offset: 0x00008C2D
		internal void SetOwnerManagedScriptHolder(ManagedScriptHolder managedScriptHolder)
		{
			this.ManagedScriptHolder = managedScriptHolder;
		}

		// Token: 0x060009E7 RID: 2535 RVA: 0x0000AA38 File Offset: 0x00008C38
		private void SetScriptComponentToTickAux(ScriptComponentBehavior.TickRequirement value)
		{
			if (this._lastTickRequirement != value)
			{
				if (value.HasAnyFlag(ScriptComponentBehavior.TickRequirement.Tick) != this._lastTickRequirement.HasAnyFlag(ScriptComponentBehavior.TickRequirement.Tick))
				{
					if (this._lastTickRequirement.HasAnyFlag(ScriptComponentBehavior.TickRequirement.Tick))
					{
						this.ManagedScriptHolder.RemoveScriptComponentFromTickList(this);
					}
					else
					{
						this.ManagedScriptHolder.AddScriptComponentToTickList(this);
					}
				}
				if (value.HasAnyFlag(ScriptComponentBehavior.TickRequirement.TickOccasionally) != this._lastTickRequirement.HasAnyFlag(ScriptComponentBehavior.TickRequirement.TickOccasionally))
				{
					if (this._lastTickRequirement.HasAnyFlag(ScriptComponentBehavior.TickRequirement.TickOccasionally))
					{
						this.ManagedScriptHolder.RemoveScriptComponentFromTickOccasionallyList(this);
					}
					else
					{
						this.ManagedScriptHolder.AddScriptComponentToTickOccasionallyList(this);
					}
				}
				if (value.HasAnyFlag(ScriptComponentBehavior.TickRequirement.TickParallel) != this._lastTickRequirement.HasAnyFlag(ScriptComponentBehavior.TickRequirement.TickParallel))
				{
					if (this._lastTickRequirement.HasAnyFlag(ScriptComponentBehavior.TickRequirement.TickParallel))
					{
						this.ManagedScriptHolder.RemoveScriptComponentFromParallelTickList(this);
					}
					else
					{
						this.ManagedScriptHolder.AddScriptComponentToParallelTickList(this);
					}
				}
				if (value.HasAnyFlag(ScriptComponentBehavior.TickRequirement.TickParallel2) != this._lastTickRequirement.HasAnyFlag(ScriptComponentBehavior.TickRequirement.TickParallel2))
				{
					if (this._lastTickRequirement.HasAnyFlag(ScriptComponentBehavior.TickRequirement.TickParallel2))
					{
						this.ManagedScriptHolder.RemoveScriptComponentFromParallelTick2List(this);
					}
					else
					{
						this.ManagedScriptHolder.AddScriptComponentToParallelTick2List(this);
					}
				}
				this._lastTickRequirement = value;
			}
		}

		// Token: 0x060009E8 RID: 2536 RVA: 0x0000AB4C File Offset: 0x00008D4C
		public void SetScriptComponentToTick(ScriptComponentBehavior.TickRequirement value)
		{
			this.SetScriptComponentToTickAux(value);
		}

		// Token: 0x060009E9 RID: 2537 RVA: 0x0000AB58 File Offset: 0x00008D58
		public void SetScriptComponentToTickMT(ScriptComponentBehavior.TickRequirement value)
		{
			object addRemoveLockObject = ManagedScriptHolder.AddRemoveLockObject;
			lock (addRemoveLockObject)
			{
				this.SetScriptComponentToTickAux(value);
			}
		}

		// Token: 0x060009EA RID: 2538 RVA: 0x0000AB98 File Offset: 0x00008D98
		[EngineCallback]
		internal void AddScriptComponentToTick()
		{
			List<ScriptComponentBehavior> prefabScriptComponents = ScriptComponentBehavior._prefabScriptComponents;
			lock (prefabScriptComponents)
			{
				if (!ScriptComponentBehavior._prefabScriptComponents.Contains(this))
				{
					ScriptComponentBehavior._prefabScriptComponents.Add(this);
				}
			}
		}

		// Token: 0x060009EB RID: 2539 RVA: 0x0000ABEC File Offset: 0x00008DEC
		[EngineCallback]
		internal void RegisterAsPrefabScriptComponent()
		{
			List<ScriptComponentBehavior> prefabScriptComponents = ScriptComponentBehavior._prefabScriptComponents;
			lock (prefabScriptComponents)
			{
				if (!ScriptComponentBehavior._prefabScriptComponents.Contains(this))
				{
					ScriptComponentBehavior._prefabScriptComponents.Add(this);
				}
			}
		}

		// Token: 0x060009EC RID: 2540 RVA: 0x0000AC40 File Offset: 0x00008E40
		[EngineCallback]
		internal void DeregisterAsPrefabScriptComponent()
		{
			List<ScriptComponentBehavior> prefabScriptComponents = ScriptComponentBehavior._prefabScriptComponents;
			lock (prefabScriptComponents)
			{
				ScriptComponentBehavior._prefabScriptComponents.Remove(this);
			}
		}

		// Token: 0x060009ED RID: 2541 RVA: 0x0000AC88 File Offset: 0x00008E88
		[EngineCallback]
		internal void RegisterAsUndoStackScriptComponent()
		{
			if (!ScriptComponentBehavior._undoStackScriptComponents.Contains(this))
			{
				ScriptComponentBehavior._undoStackScriptComponents.Add(this);
			}
		}

		// Token: 0x060009EE RID: 2542 RVA: 0x0000ACA2 File Offset: 0x00008EA2
		[EngineCallback]
		internal void DeregisterAsUndoStackScriptComponent()
		{
			if (ScriptComponentBehavior._undoStackScriptComponents.Contains(this))
			{
				ScriptComponentBehavior._undoStackScriptComponents.Remove(this);
			}
		}

		// Token: 0x060009EF RID: 2543 RVA: 0x0000ACBD File Offset: 0x00008EBD
		[EngineCallback]
		protected internal virtual void SetScene(Scene scene)
		{
			this.Scene = scene;
		}

		// Token: 0x060009F0 RID: 2544 RVA: 0x0000ACC6 File Offset: 0x00008EC6
		[EngineCallback]
		protected internal virtual void OnInit()
		{
		}

		// Token: 0x060009F1 RID: 2545 RVA: 0x0000ACC8 File Offset: 0x00008EC8
		[EngineCallback]
		protected internal virtual void HandleOnRemoved(int removeReason)
		{
			this.OnRemoved(removeReason);
			this._scene = null;
			this._gameEntity = null;
		}

		// Token: 0x060009F2 RID: 2546 RVA: 0x0000ACDF File Offset: 0x00008EDF
		protected virtual void OnRemoved(int removeReason)
		{
		}

		// Token: 0x060009F3 RID: 2547 RVA: 0x0000ACE1 File Offset: 0x00008EE1
		public virtual ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return ScriptComponentBehavior.TickRequirement.None;
		}

		// Token: 0x060009F4 RID: 2548 RVA: 0x0000ACE4 File Offset: 0x00008EE4
		protected internal virtual void OnTick(float dt)
		{
			Debug.FailedAssert("This base function should never be called.", "C:\\Develop\\MB3\\Source\\Engine\\TaleWorlds.Engine\\ScriptComponentBehavior.cs", "OnTick", 256);
		}

		// Token: 0x060009F5 RID: 2549 RVA: 0x0000ACFF File Offset: 0x00008EFF
		protected internal virtual void OnTickParallel(float dt)
		{
			Debug.FailedAssert("This base function should never be called.", "C:\\Develop\\MB3\\Source\\Engine\\TaleWorlds.Engine\\ScriptComponentBehavior.cs", "OnTickParallel", 262);
		}

		// Token: 0x060009F6 RID: 2550 RVA: 0x0000AD1A File Offset: 0x00008F1A
		protected internal virtual void OnTickParallel2(float dt)
		{
		}

		// Token: 0x060009F7 RID: 2551 RVA: 0x0000AD1C File Offset: 0x00008F1C
		protected internal virtual void OnTickOccasionally(float currentFrameDeltaTime)
		{
			Debug.FailedAssert("This base function should never be called.", "C:\\Develop\\MB3\\Source\\Engine\\TaleWorlds.Engine\\ScriptComponentBehavior.cs", "OnTickOccasionally", 274);
		}

		// Token: 0x060009F8 RID: 2552 RVA: 0x0000AD37 File Offset: 0x00008F37
		[EngineCallback]
		protected internal virtual void OnPreInit()
		{
		}

		// Token: 0x060009F9 RID: 2553 RVA: 0x0000AD39 File Offset: 0x00008F39
		[EngineCallback]
		protected internal virtual void OnEditorInit()
		{
		}

		// Token: 0x060009FA RID: 2554 RVA: 0x0000AD3B File Offset: 0x00008F3B
		[EngineCallback]
		protected internal virtual void OnEditorTick(float dt)
		{
		}

		// Token: 0x060009FB RID: 2555 RVA: 0x0000AD3D File Offset: 0x00008F3D
		[EngineCallback]
		protected internal virtual void OnEditorValidate()
		{
		}

		// Token: 0x060009FC RID: 2556 RVA: 0x0000AD3F File Offset: 0x00008F3F
		[EngineCallback]
		protected internal virtual bool IsOnlyVisual()
		{
			return false;
		}

		// Token: 0x060009FD RID: 2557 RVA: 0x0000AD42 File Offset: 0x00008F42
		[EngineCallback]
		protected internal virtual bool MovesEntity()
		{
			return true;
		}

		// Token: 0x060009FE RID: 2558 RVA: 0x0000AD45 File Offset: 0x00008F45
		[EngineCallback]
		protected internal virtual bool DisablesOroCreation()
		{
			return true;
		}

		// Token: 0x060009FF RID: 2559 RVA: 0x0000AD48 File Offset: 0x00008F48
		[EngineCallback]
		protected internal virtual void OnEditorVariableChanged(string variableName)
		{
		}

		// Token: 0x06000A00 RID: 2560 RVA: 0x0000AD4A File Offset: 0x00008F4A
		[EngineCallback]
		protected internal virtual void OnSceneSave(string saveFolder)
		{
		}

		// Token: 0x06000A01 RID: 2561 RVA: 0x0000AD4C File Offset: 0x00008F4C
		[EngineCallback]
		protected internal virtual bool OnCheckForProblems()
		{
			return false;
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x0000AD4F File Offset: 0x00008F4F
		[EngineCallback]
		protected internal virtual void OnPhysicsCollision(ref PhysicsContact contact)
		{
		}

		// Token: 0x06000A03 RID: 2563 RVA: 0x0000AD51 File Offset: 0x00008F51
		[EngineCallback]
		protected internal virtual void OnEditModeVisibilityChanged(bool currentVisibility)
		{
		}

		// Token: 0x06000A04 RID: 2564 RVA: 0x0000AD54 File Offset: 0x00008F54
		private static void CacheEditableFieldsForAllScriptComponents()
		{
			foreach (KeyValuePair<string, Type> keyValuePair in Managed.ModuleTypes)
			{
				string key = keyValuePair.Key;
				ScriptComponentBehavior.CachedFields.Add(key, ScriptComponentBehavior.CollectEditableFields(key));
			}
		}

		// Token: 0x06000A05 RID: 2565 RVA: 0x0000ADB8 File Offset: 0x00008FB8
		private static string[] CollectEditableFields(string className)
		{
			List<string> list = new List<string>();
			Type type;
			if (Managed.ModuleTypes.TryGetValue(className, out type))
			{
				FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				for (int i = 0; i < fields.Length; i++)
				{
					FieldInfo fieldInfo = fields[i];
					object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(EditableScriptComponentVariable), true);
					bool flag = false;
					if (customAttributes.Length != 0)
					{
						flag = ((EditableScriptComponentVariable)customAttributes[0]).Visible;
					}
					else if (!fieldInfo.IsPrivate && !fieldInfo.IsFamily)
					{
						flag = true;
					}
					if (flag)
					{
						list.Add(fields[i].Name);
					}
				}
			}
			return list.ToArray();
		}

		// Token: 0x06000A06 RID: 2566 RVA: 0x0000AE54 File Offset: 0x00009054
		[EngineCallback]
		internal static string[] GetEditableFields(string className)
		{
			string[] array;
			ScriptComponentBehavior.CachedFields.TryGetValue(className, out array);
			return array;
		}

		// Token: 0x04000195 RID: 405
		private static List<ScriptComponentBehavior> _prefabScriptComponents = new List<ScriptComponentBehavior>();

		// Token: 0x04000196 RID: 406
		private static List<ScriptComponentBehavior> _undoStackScriptComponents = new List<ScriptComponentBehavior>();

		// Token: 0x04000197 RID: 407
		private WeakNativeObjectReference _gameEntity;

		// Token: 0x04000198 RID: 408
		private WeakNativeObjectReference _scriptComponent;

		// Token: 0x04000199 RID: 409
		private ScriptComponentBehavior.TickRequirement _lastTickRequirement;

		// Token: 0x0400019A RID: 410
		private static readonly Dictionary<string, string[]> CachedFields;

		// Token: 0x0400019C RID: 412
		private WeakNativeObjectReference _scene;

		// Token: 0x020000C3 RID: 195
		[Flags]
		public enum TickRequirement : uint
		{
			// Token: 0x04000402 RID: 1026
			None = 0U,
			// Token: 0x04000403 RID: 1027
			TickOccasionally = 1U,
			// Token: 0x04000404 RID: 1028
			Tick = 2U,
			// Token: 0x04000405 RID: 1029
			TickParallel = 4U,
			// Token: 0x04000406 RID: 1030
			TickParallel2 = 8U
		}
	}
}

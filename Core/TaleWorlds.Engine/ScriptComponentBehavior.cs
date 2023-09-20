using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	public abstract class ScriptComponentBehavior : DotNetObject
	{
		protected void InvalidateWeakPointersIfValid()
		{
			this._gameEntity.ManualInvalidate();
			this._scriptComponent.ManualInvalidate();
		}

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

		private protected ManagedScriptHolder ManagedScriptHolder { protected get; private set; }

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

		static ScriptComponentBehavior()
		{
			if (ScriptComponentBehavior.CachedFields == null)
			{
				ScriptComponentBehavior.CachedFields = new Dictionary<string, string[]>();
				ScriptComponentBehavior.CacheEditableFieldsForAllScriptComponents();
			}
		}

		internal void Construct(GameEntity myEntity, ManagedScriptComponent scriptComponent)
		{
			this.GameEntity = myEntity;
			this.Scene = myEntity.Scene;
			this.ScriptComponent = scriptComponent;
		}

		internal void SetOwnerManagedScriptHolder(ManagedScriptHolder managedScriptHolder)
		{
			this.ManagedScriptHolder = managedScriptHolder;
		}

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

		public void SetScriptComponentToTick(ScriptComponentBehavior.TickRequirement value)
		{
			this.SetScriptComponentToTickAux(value);
		}

		public void SetScriptComponentToTickMT(ScriptComponentBehavior.TickRequirement value)
		{
			object addRemoveLockObject = ManagedScriptHolder.AddRemoveLockObject;
			lock (addRemoveLockObject)
			{
				this.SetScriptComponentToTickAux(value);
			}
		}

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

		[EngineCallback]
		internal void DeregisterAsPrefabScriptComponent()
		{
			List<ScriptComponentBehavior> prefabScriptComponents = ScriptComponentBehavior._prefabScriptComponents;
			lock (prefabScriptComponents)
			{
				ScriptComponentBehavior._prefabScriptComponents.Remove(this);
			}
		}

		[EngineCallback]
		internal void RegisterAsUndoStackScriptComponent()
		{
			if (!ScriptComponentBehavior._undoStackScriptComponents.Contains(this))
			{
				ScriptComponentBehavior._undoStackScriptComponents.Add(this);
			}
		}

		[EngineCallback]
		internal void DeregisterAsUndoStackScriptComponent()
		{
			if (ScriptComponentBehavior._undoStackScriptComponents.Contains(this))
			{
				ScriptComponentBehavior._undoStackScriptComponents.Remove(this);
			}
		}

		[EngineCallback]
		protected internal virtual void SetScene(Scene scene)
		{
			this.Scene = scene;
		}

		[EngineCallback]
		protected internal virtual void OnInit()
		{
		}

		[EngineCallback]
		protected internal virtual void HandleOnRemoved(int removeReason)
		{
			this.OnRemoved(removeReason);
			this._scene = null;
			this._gameEntity = null;
		}

		protected virtual void OnRemoved(int removeReason)
		{
		}

		public virtual ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return ScriptComponentBehavior.TickRequirement.None;
		}

		protected internal virtual void OnTick(float dt)
		{
			Debug.FailedAssert("This base function should never be called.", "C:\\Develop\\MB3\\Source\\Engine\\TaleWorlds.Engine\\ScriptComponentBehavior.cs", "OnTick", 256);
		}

		protected internal virtual void OnTickParallel(float dt)
		{
			Debug.FailedAssert("This base function should never be called.", "C:\\Develop\\MB3\\Source\\Engine\\TaleWorlds.Engine\\ScriptComponentBehavior.cs", "OnTickParallel", 262);
		}

		protected internal virtual void OnTickParallel2(float dt)
		{
		}

		protected internal virtual void OnTickOccasionally(float currentFrameDeltaTime)
		{
			Debug.FailedAssert("This base function should never be called.", "C:\\Develop\\MB3\\Source\\Engine\\TaleWorlds.Engine\\ScriptComponentBehavior.cs", "OnTickOccasionally", 274);
		}

		[EngineCallback]
		protected internal virtual void OnPreInit()
		{
		}

		[EngineCallback]
		protected internal virtual void OnEditorInit()
		{
		}

		[EngineCallback]
		protected internal virtual void OnEditorTick(float dt)
		{
		}

		[EngineCallback]
		protected internal virtual void OnEditorValidate()
		{
		}

		[EngineCallback]
		protected internal virtual bool IsOnlyVisual()
		{
			return false;
		}

		[EngineCallback]
		protected internal virtual bool MovesEntity()
		{
			return true;
		}

		[EngineCallback]
		protected internal virtual bool DisablesOroCreation()
		{
			return true;
		}

		[EngineCallback]
		protected internal virtual void OnEditorVariableChanged(string variableName)
		{
		}

		[EngineCallback]
		protected internal virtual void OnSceneSave(string saveFolder)
		{
		}

		[EngineCallback]
		protected internal virtual bool OnCheckForProblems()
		{
			return false;
		}

		[EngineCallback]
		protected internal virtual void OnPhysicsCollision(ref PhysicsContact contact)
		{
		}

		[EngineCallback]
		protected internal virtual void OnEditModeVisibilityChanged(bool currentVisibility)
		{
		}

		private static void CacheEditableFieldsForAllScriptComponents()
		{
			foreach (KeyValuePair<string, Type> keyValuePair in Managed.ModuleTypes)
			{
				string key = keyValuePair.Key;
				ScriptComponentBehavior.CachedFields.Add(key, ScriptComponentBehavior.CollectEditableFields(key));
			}
		}

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

		[EngineCallback]
		internal static string[] GetEditableFields(string className)
		{
			string[] array;
			ScriptComponentBehavior.CachedFields.TryGetValue(className, out array);
			return array;
		}

		private static List<ScriptComponentBehavior> _prefabScriptComponents = new List<ScriptComponentBehavior>();

		private static List<ScriptComponentBehavior> _undoStackScriptComponents = new List<ScriptComponentBehavior>();

		private WeakNativeObjectReference _gameEntity;

		private WeakNativeObjectReference _scriptComponent;

		private ScriptComponentBehavior.TickRequirement _lastTickRequirement;

		private static readonly Dictionary<string, string[]> CachedFields;

		private WeakNativeObjectReference _scene;

		[Flags]
		public enum TickRequirement : uint
		{
			None = 0U,
			TickOccasionally = 1U,
			Tick = 2U,
			TickParallel = 4U,
			TickParallel2 = 8U
		}
	}
}

using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.ScreenSystem
{
	// Token: 0x02000006 RID: 6
	public abstract class ScreenBase
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000029 RID: 41 RVA: 0x00002198 File Offset: 0x00000398
		// (remove) Token: 0x0600002A RID: 42 RVA: 0x000021D0 File Offset: 0x000003D0
		public event ScreenBase.OnLayerAddedEvent OnAddLayer;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x0600002B RID: 43 RVA: 0x00002208 File Offset: 0x00000408
		// (remove) Token: 0x0600002C RID: 44 RVA: 0x00002240 File Offset: 0x00000440
		public event ScreenBase.OnLayerRemovedEvent OnRemoveLayer;

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002275 File Offset: 0x00000475
		public IInputContext DebugInput
		{
			get
			{
				return Input.DebugInput;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600002E RID: 46 RVA: 0x0000227C File Offset: 0x0000047C
		public MBReadOnlyList<ScreenLayer> Layers
		{
			get
			{
				return this._layers;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002284 File Offset: 0x00000484
		// (set) Token: 0x06000030 RID: 48 RVA: 0x0000228C File Offset: 0x0000048C
		public bool IsActive { get; private set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00002295 File Offset: 0x00000495
		// (set) Token: 0x06000032 RID: 50 RVA: 0x0000229D File Offset: 0x0000049D
		public bool IsPaused { get; private set; }

		// Token: 0x06000033 RID: 51 RVA: 0x000022A8 File Offset: 0x000004A8
		internal void HandleInitialize()
		{
			Debug.Print(this + "::HandleInitialize", 0, Debug.DebugColor.White, 17592186044416UL);
			if (!this._isInitialized)
			{
				this._isInitialized = true;
				this.OnInitialize();
				Debug.ReportMemoryBookmark("ScreenBase Initialized: " + base.GetType().Name);
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002300 File Offset: 0x00000500
		internal void HandleFinalize()
		{
			Debug.Print(this + "::HandleFinalize", 0, Debug.DebugColor.White, 17592186044416UL);
			if (this._isInitialized)
			{
				this._isInitialized = false;
				this.OnFinalize();
				for (int i = this._layers.Count - 1; i >= 0; i--)
				{
					this._layers[i].HandleFinalize();
				}
			}
			this.IsActive = false;
			this.OnAddLayer = null;
			this.OnRemoveLayer = null;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x0000237C File Offset: 0x0000057C
		internal void HandleActivate()
		{
			Debug.Print(this + "::HandleActivate", 0, Debug.DebugColor.White, 17592186044416UL);
			if (!this.IsActive)
			{
				this.IsActive = true;
				for (int i = this._layers.Count - 1; i >= 0; i--)
				{
					ScreenLayer screenLayer = this._layers[i];
					if (!screenLayer.IsActive)
					{
						screenLayer.HandleActivate();
					}
				}
				this.OnActivate();
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000023F0 File Offset: 0x000005F0
		internal void HandleDeactivate()
		{
			Debug.Print(this + "::HandleDeactivate", 0, Debug.DebugColor.White, 17592186044416UL);
			if (this.IsActive)
			{
				this.IsActive = false;
				for (int i = this._layers.Count - 1; i >= 0; i--)
				{
					ScreenLayer screenLayer = this._layers[i];
					if (screenLayer.IsActive)
					{
						screenLayer.HandleDeactivate();
					}
				}
				this.OnDeactivate();
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002464 File Offset: 0x00000664
		internal void HandleResume()
		{
			Debug.Print(this + "::HandleResume", 0, Debug.DebugColor.White, 17592186044416UL);
			if (this.IsPaused)
			{
				for (int i = this._layers.Count - 1; i >= 0; i--)
				{
					ScreenLayer screenLayer = this._layers[i];
					if (!screenLayer.IsActive)
					{
						screenLayer.HandleActivate();
					}
				}
				this.IsPaused = false;
				this.OnResume();
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000024D8 File Offset: 0x000006D8
		internal void HandlePause()
		{
			Debug.Print(this + "::HandlePause", 0, Debug.DebugColor.White, 17592186044416UL);
			if (!this.IsPaused)
			{
				for (int i = this._layers.Count - 1; i >= 0; i--)
				{
					ScreenLayer screenLayer = this._layers[i];
					if (screenLayer.IsActive)
					{
						screenLayer.HandleDeactivate();
					}
				}
				this.IsPaused = true;
				this.OnPause();
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x0000254C File Offset: 0x0000074C
		internal void FrameTick(float dt)
		{
			this._shouldTickLayersThisFrame = true;
			if (this.IsActive)
			{
				this.OnFrameTick(dt);
			}
			if (this.IsActive)
			{
				if (!this._shouldTickLayersThisFrame)
				{
					dt = 0f;
				}
				for (int i = 0; i < this._layers.Count; i++)
				{
					if (this._layers[i].IsActive)
					{
						this._layersCopy.Add(this._layers[i]);
					}
				}
				for (int j = 0; j < this._layersCopy.Count; j++)
				{
					if (!this._layersCopy[j].Finalized)
					{
						this._layersCopy[j].Tick(dt);
					}
				}
				ScreenManager.UpdateLateTickLayers(this._layersCopy);
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002610 File Offset: 0x00000810
		public void ActivateAllLayers()
		{
			foreach (ScreenLayer screenLayer in this._layers)
			{
				if (!screenLayer.IsActive)
				{
					screenLayer.HandleActivate();
				}
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x0000266C File Offset: 0x0000086C
		public void DeactivateAllLayers()
		{
			foreach (ScreenLayer screenLayer in this._layers)
			{
				if (screenLayer.IsActive)
				{
					screenLayer.HandleDeactivate();
				}
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000026C8 File Offset: 0x000008C8
		public void Deactivate()
		{
			if (this.IsActive)
			{
				this.HandleDeactivate();
				this.IsActive = false;
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000026DF File Offset: 0x000008DF
		public void Activate()
		{
			if (!this.IsActive)
			{
				this.HandleActivate();
				this.IsActive = true;
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x000026F8 File Offset: 0x000008F8
		public virtual void UpdateLayout()
		{
			for (int i = 0; i < this._layers.Count; i++)
			{
				if (!this._layers[i].Finalized)
				{
					this._layers[i].UpdateLayout();
				}
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x0000273F File Offset: 0x0000093F
		internal void IdleTick(float dt)
		{
			this.OnIdleTick(dt);
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00002748 File Offset: 0x00000948
		protected virtual void OnInitialize()
		{
		}

		// Token: 0x06000041 RID: 65 RVA: 0x0000274A File Offset: 0x0000094A
		protected virtual void OnFinalize()
		{
		}

		// Token: 0x06000042 RID: 66 RVA: 0x0000274C File Offset: 0x0000094C
		protected virtual void OnPause()
		{
		}

		// Token: 0x06000043 RID: 67 RVA: 0x0000274E File Offset: 0x0000094E
		protected virtual void OnResume()
		{
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00002750 File Offset: 0x00000950
		protected virtual void OnActivate()
		{
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002752 File Offset: 0x00000952
		protected virtual void OnDeactivate()
		{
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00002754 File Offset: 0x00000954
		protected virtual void OnFrameTick(float dt)
		{
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00002756 File Offset: 0x00000956
		protected virtual void OnIdleTick(float dt)
		{
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002758 File Offset: 0x00000958
		public virtual void OnFocusChangeOnGameWindow(bool focusGained)
		{
		}

		// Token: 0x06000049 RID: 73 RVA: 0x0000275A File Offset: 0x0000095A
		public void AddComponent(ScreenComponent component)
		{
			this._components.Add(component);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00002768 File Offset: 0x00000968
		public T FindComponent<T>() where T : ScreenComponent
		{
			foreach (ScreenComponent screenComponent in this._components)
			{
				if (screenComponent is T)
				{
					return (T)((object)screenComponent);
				}
			}
			return default(T);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000027D0 File Offset: 0x000009D0
		public void AddLayer(ScreenLayer layer)
		{
			if (this._layers.Contains(layer))
			{
				Debug.FailedAssert("Layer is already added to the screen!", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.ScreenSystem\\ScreenBase.cs", "AddLayer", 347);
				return;
			}
			this._layers.Add(layer);
			this._layers.Sort();
			if (this.IsActive)
			{
				layer.LastActiveState = true;
				layer.HandleActivate();
			}
			ScreenBase.OnLayerAddedEvent onAddLayer = this.OnAddLayer;
			if (onAddLayer == null)
			{
				return;
			}
			onAddLayer(layer);
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002844 File Offset: 0x00000A44
		public void RemoveLayer(ScreenLayer layer)
		{
			if (this.IsActive)
			{
				layer.LastActiveState = false;
				layer.HandleDeactivate();
			}
			layer.HandleFinalize();
			this._layers.Remove(layer);
			ScreenBase.OnLayerRemovedEvent onRemoveLayer = this.OnRemoveLayer;
			if (onRemoveLayer != null)
			{
				onRemoveLayer(layer);
			}
			ScreenManager.RefreshGlobalOrder();
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002890 File Offset: 0x00000A90
		public bool HasLayer(ScreenLayer layer)
		{
			return this._layers.Contains(layer);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x000028A0 File Offset: 0x00000AA0
		public T FindLayer<T>() where T : ScreenLayer
		{
			foreach (ScreenLayer screenLayer in this._layers)
			{
				if (screenLayer is T)
				{
					return (T)((object)screenLayer);
				}
			}
			return default(T);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00002908 File Offset: 0x00000B08
		public T FindLayer<T>(string name) where T : ScreenLayer
		{
			using (List<ScreenLayer>.Enumerator enumerator = this._layers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T t;
					if ((t = enumerator.Current as T) != null && t.Name == name)
					{
						return t;
					}
				}
			}
			return default(T);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00002988 File Offset: 0x00000B88
		public void SetLayerCategoriesState(string[] categoryIds, bool isActive)
		{
			foreach (ScreenLayer screenLayer in this._layers)
			{
				if (categoryIds.IndexOf(screenLayer._categoryId) >= 0)
				{
					if (isActive && !screenLayer.IsActive)
					{
						screenLayer.HandleActivate();
					}
					else if (!isActive && screenLayer.IsActive)
					{
						screenLayer.HandleDeactivate();
					}
				}
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00002A08 File Offset: 0x00000C08
		public void SetLayerCategoriesStateAndToggleOthers(string[] categoryIds, bool isActive)
		{
			foreach (ScreenLayer screenLayer in this._layers)
			{
				if (categoryIds.IndexOf(screenLayer._categoryId) >= 0)
				{
					if (isActive && !screenLayer.IsActive)
					{
						screenLayer.HandleActivate();
					}
					else if (!isActive && screenLayer.IsActive)
					{
						screenLayer.HandleDeactivate();
					}
				}
				else if (screenLayer.IsActive)
				{
					screenLayer.HandleDeactivate();
				}
				else
				{
					screenLayer.HandleActivate();
				}
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00002AA0 File Offset: 0x00000CA0
		public void SetLayerCategoriesStateAndDeactivateOthers(string[] categoryIds, bool isActive)
		{
			foreach (ScreenLayer screenLayer in this._layers)
			{
				if (categoryIds.IndexOf(screenLayer._categoryId) >= 0)
				{
					if (isActive && !screenLayer.IsActive)
					{
						screenLayer.HandleActivate();
					}
					else if (!isActive && screenLayer.IsActive)
					{
						screenLayer.HandleDeactivate();
					}
				}
				else if (screenLayer.IsActive)
				{
					screenLayer.HandleDeactivate();
				}
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00002B30 File Offset: 0x00000D30
		protected ScreenBase()
		{
			this.IsPaused = true;
			this.IsActive = false;
			this._components = new List<ScreenComponent>();
			this._layers = new MBList<ScreenLayer>();
			this._layersCopy = new List<ScreenLayer>();
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000054 RID: 84 RVA: 0x00002B6E File Offset: 0x00000D6E
		// (set) Token: 0x06000055 RID: 85 RVA: 0x00002B76 File Offset: 0x00000D76
		public virtual bool MouseVisible { get; set; }

		// Token: 0x06000056 RID: 86 RVA: 0x00002B80 File Offset: 0x00000D80
		internal void Update(IReadOnlyList<int> lastKeysPressed)
		{
			if (this.IsActive)
			{
				foreach (ScreenLayer screenLayer in this._layers)
				{
					if (screenLayer.IsActive)
					{
						screenLayer.Update(lastKeysPressed);
					}
				}
			}
		}

		// Token: 0x04000014 RID: 20
		private readonly List<ScreenComponent> _components;

		// Token: 0x04000015 RID: 21
		private readonly MBList<ScreenLayer> _layers;

		// Token: 0x04000016 RID: 22
		private readonly List<ScreenLayer> _layersCopy;

		// Token: 0x04000019 RID: 25
		protected bool _shouldTickLayersThisFrame = true;

		// Token: 0x0400001C RID: 28
		private bool _isInitialized;

		// Token: 0x0200000A RID: 10
		// (Invoke) Token: 0x060000DD RID: 221
		public delegate void OnLayerAddedEvent(ScreenLayer addedLayer);

		// Token: 0x0200000B RID: 11
		// (Invoke) Token: 0x060000E1 RID: 225
		public delegate void OnLayerRemovedEvent(ScreenLayer removedLayer);
	}
}

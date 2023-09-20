using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.ScreenSystem
{
	public abstract class ScreenBase
	{
		public event ScreenBase.OnLayerAddedEvent OnAddLayer;

		public event ScreenBase.OnLayerRemovedEvent OnRemoveLayer;

		public IInputContext DebugInput
		{
			get
			{
				return Input.DebugInput;
			}
		}

		public MBReadOnlyList<ScreenLayer> Layers
		{
			get
			{
				return this._layers;
			}
		}

		public bool IsActive { get; private set; }

		public bool IsPaused { get; private set; }

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

		public void Deactivate()
		{
			if (this.IsActive)
			{
				this.HandleDeactivate();
				this.IsActive = false;
			}
		}

		public void Activate()
		{
			if (!this.IsActive)
			{
				this.HandleActivate();
				this.IsActive = true;
			}
		}

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

		internal void IdleTick(float dt)
		{
			this.OnIdleTick(dt);
		}

		protected virtual void OnInitialize()
		{
		}

		protected virtual void OnFinalize()
		{
		}

		protected virtual void OnPause()
		{
		}

		protected virtual void OnResume()
		{
		}

		protected virtual void OnActivate()
		{
		}

		protected virtual void OnDeactivate()
		{
		}

		protected virtual void OnFrameTick(float dt)
		{
		}

		protected virtual void OnIdleTick(float dt)
		{
		}

		public virtual void OnFocusChangeOnGameWindow(bool focusGained)
		{
		}

		public void AddComponent(ScreenComponent component)
		{
			this._components.Add(component);
		}

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

		public void AddLayer(ScreenLayer layer)
		{
			if (layer == null || layer.Finalized)
			{
				Debug.FailedAssert("Trying to add a null or finalized layer", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.ScreenSystem\\ScreenBase.cs", "AddLayer", 334);
				return;
			}
			if (this._layers.Contains(layer))
			{
				Debug.FailedAssert("Layer is already added to the screen!", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.ScreenSystem\\ScreenBase.cs", "AddLayer", 353);
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

		public bool HasLayer(ScreenLayer layer)
		{
			return this._layers.Contains(layer);
		}

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

		protected ScreenBase()
		{
			this.IsPaused = true;
			this.IsActive = false;
			this._components = new List<ScreenComponent>();
			this._layers = new MBList<ScreenLayer>();
			this._layersCopy = new List<ScreenLayer>();
		}

		public virtual bool MouseVisible { get; set; }

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

		private readonly List<ScreenComponent> _components;

		private readonly MBList<ScreenLayer> _layers;

		private readonly List<ScreenLayer> _layersCopy;

		protected bool _shouldTickLayersThisFrame = true;

		private bool _isInitialized;

		public delegate void OnLayerAddedEvent(ScreenLayer addedLayer);

		public delegate void OnLayerRemovedEvent(ScreenLayer removedLayer);
	}
}

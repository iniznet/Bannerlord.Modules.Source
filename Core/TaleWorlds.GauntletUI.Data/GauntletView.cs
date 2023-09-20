using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.PrefabSystem;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI.Data
{
	public class GauntletView : WidgetComponent
	{
		internal bool AddedToChildren { get; private set; }

		public object Tag
		{
			get
			{
				return base.Target.Tag;
			}
			set
			{
				base.Target.Tag = value;
			}
		}

		public GauntletMovie GauntletMovie { get; private set; }

		public ItemTemplateUsageWithData ItemTemplateUsageWithData { get; internal set; }

		public BindingPath ViewModelPath
		{
			get
			{
				if (this.Parent == null)
				{
					return this._viewModelPath;
				}
				if (this._viewModelPath != null)
				{
					return this.Parent.ViewModelPath.Append(this._viewModelPath);
				}
				return this.Parent.ViewModelPath;
			}
		}

		public string ViewModelPathString
		{
			get
			{
				MBStringBuilder mbstringBuilder = default(MBStringBuilder);
				mbstringBuilder.Initialize(16, "ViewModelPathString");
				this.WriteViewModelPathToStringBuilder(ref mbstringBuilder);
				return mbstringBuilder.ToStringAndRelease();
			}
		}

		private void WriteViewModelPathToStringBuilder(ref MBStringBuilder sb)
		{
			if (this.Parent == null)
			{
				if (this._viewModelPath != null)
				{
					sb.Append<string>(this._viewModelPath.Path);
					return;
				}
			}
			else
			{
				this.Parent.WriteViewModelPathToStringBuilder(ref sb);
				if (this._viewModelPath != null)
				{
					sb.Append<string>("\\");
					sb.Append<string>(this._viewModelPath.Path);
				}
			}
		}

		internal void InitializeViewModelPath(BindingPath path)
		{
			this._viewModelPath = path;
		}

		public MBReadOnlyList<GauntletView> Children
		{
			get
			{
				return this._children;
			}
		}

		public IEnumerable<GauntletView> AllChildren
		{
			get
			{
				foreach (GauntletView gauntletView in this._children)
				{
					yield return gauntletView;
					foreach (GauntletView gauntletView2 in gauntletView.AllChildren)
					{
						yield return gauntletView2;
					}
					IEnumerator<GauntletView> enumerator2 = null;
					gauntletView = null;
				}
				List<GauntletView>.Enumerator enumerator = default(List<GauntletView>.Enumerator);
				yield break;
				yield break;
			}
		}

		public GauntletView Parent { get; private set; }

		internal GauntletView(GauntletMovie gauntletMovie, GauntletView parent, Widget target, int childCount = 64)
			: base(target)
		{
			this.GauntletMovie = gauntletMovie;
			this.Parent = parent;
			this._children = new MBList<GauntletView>(childCount);
			this._bindDataInfos = new Dictionary<string, ViewBindDataInfo>();
			this._bindCommandInfos = new Dictionary<string, ViewBindCommandInfo>();
			this._items = new List<GauntletView>(childCount);
		}

		public void AddChild(GauntletView child)
		{
			this._children.Add(child);
			child.AddedToChildren = true;
		}

		public void RemoveChild(GauntletView child)
		{
			base.Target.OnBeforeRemovedChild(child.Target);
			base.Target.RemoveChild(child.Target);
			this._children.Remove(child);
			child.ClearEventHandlersWithChildren();
		}

		public void SwapChildrenAtIndeces(GauntletView child1, GauntletView child2)
		{
			int num = this._children.IndexOf(child1);
			int num2 = this._children.IndexOf(child2);
			base.Target.SwapChildren(this._children[num].Target, this._children[num2].Target);
			GauntletView gauntletView = this._children[num];
			this._children[num] = this._children[num2];
			this._children[num2] = gauntletView;
		}

		public void RefreshBinding()
		{
			object viewModelAtPath = this.GauntletMovie.GetViewModelAtPath(this.ViewModelPath, this.ItemTemplateUsageWithData != null && this.ItemTemplateUsageWithData.ItemTemplateUsage != null);
			this.ClearEventHandlers();
			if (viewModelAtPath is IViewModel)
			{
				this._viewModel = viewModelAtPath as IViewModel;
				if (this._viewModel == null)
				{
					goto IL_1D0;
				}
				this._viewModel.PropertyChanged += this.OnViewModelPropertyChanged;
				this._viewModel.PropertyChangedWithValue += this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithBoolValue += this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithColorValue += this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithDoubleValue += this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithFloatValue += this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithIntValue += this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithUIntValue += this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithVec2Value += this.OnViewModelPropertyChangedWithValue;
				using (Dictionary<string, ViewBindDataInfo>.ValueCollection.Enumerator enumerator = this._bindDataInfos.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ViewBindDataInfo viewBindDataInfo = enumerator.Current;
						object propertyValue = this._viewModel.GetPropertyValue(viewBindDataInfo.Path.LastNode);
						this.SetData(viewBindDataInfo.Property, propertyValue);
					}
					goto IL_1D0;
				}
			}
			if (viewModelAtPath is IMBBindingList)
			{
				this._bindingList = viewModelAtPath as IMBBindingList;
				if (this._bindingList != null)
				{
					this._bindingList.ListChanged += this.OnViewModelBindingListChanged;
					for (int i = 0; i < this._bindingList.Count; i++)
					{
						this.AddItemToList(i);
					}
				}
			}
			IL_1D0:
			base.Target.EventFire += this.OnEventFired;
			base.Target.PropertyChanged += this.OnViewObjectPropertyChanged;
			base.Target.boolPropertyChanged += this.OnViewObjectPropertyChanged;
			base.Target.ColorPropertyChanged += this.OnViewObjectPropertyChanged;
			base.Target.doublePropertyChanged += this.OnViewObjectPropertyChanged;
			base.Target.floatPropertyChanged += this.OnViewObjectPropertyChanged;
			base.Target.intPropertyChanged += this.OnViewObjectPropertyChanged;
			base.Target.uintPropertyChanged += this.OnViewObjectPropertyChanged;
			base.Target.Vec2PropertyChanged += this.OnViewObjectPropertyChanged;
			base.Target.Vector2PropertyChanged += this.OnViewObjectPropertyChanged;
		}

		private void OnEventFired(Widget widget, string commandName, object[] args)
		{
			this.OnCommand(commandName, args);
		}

		public void RefreshBindingWithChildren()
		{
			this.RefreshBinding();
			for (int i = 0; i < this._children.Count; i++)
			{
				this._children[i].RefreshBindingWithChildren();
			}
		}

		private void ReleaseBinding()
		{
			if (this._viewModel != null)
			{
				this._viewModel.PropertyChanged -= this.OnViewModelPropertyChanged;
				this._viewModel.PropertyChangedWithValue -= this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithBoolValue -= this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithColorValue -= this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithDoubleValue -= this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithFloatValue -= this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithIntValue -= this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithUIntValue -= this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithVec2Value -= this.OnViewModelPropertyChangedWithValue;
				return;
			}
			if (this._bindingList != null)
			{
				this._bindingList.ListChanged -= this.OnViewModelBindingListChanged;
			}
		}

		public void ReleaseBindingWithChildren()
		{
			this.ReleaseBinding();
			for (int i = 0; i < this._children.Count; i++)
			{
				this._children[i].ReleaseBindingWithChildren();
			}
		}

		internal void ClearEventHandlersWithChildren()
		{
			this.ClearEventHandlers();
			for (int i = 0; i < this._children.Count; i++)
			{
				this._children[i].ClearEventHandlersWithChildren();
			}
		}

		private void ClearEventHandlers()
		{
			if (this._viewModel != null)
			{
				this._viewModel.PropertyChanged -= this.OnViewModelPropertyChanged;
				this._viewModel.PropertyChangedWithValue -= this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithBoolValue -= this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithColorValue -= this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithDoubleValue -= this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithFloatValue -= this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithIntValue -= this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithUIntValue -= this.OnViewModelPropertyChangedWithValue;
				this._viewModel.PropertyChangedWithVec2Value -= this.OnViewModelPropertyChangedWithValue;
				this._viewModel = null;
			}
			if (this._bindingList != null)
			{
				this.OnListReset();
				this._bindingList.ListChanged -= this.OnViewModelBindingListChanged;
				this._bindingList = null;
			}
			base.Target.EventFire -= this.OnEventFired;
			base.Target.PropertyChanged -= this.OnViewObjectPropertyChanged;
			base.Target.boolPropertyChanged -= this.OnViewObjectPropertyChanged;
			base.Target.ColorPropertyChanged -= this.OnViewObjectPropertyChanged;
			base.Target.doublePropertyChanged -= this.OnViewObjectPropertyChanged;
			base.Target.floatPropertyChanged -= this.OnViewObjectPropertyChanged;
			base.Target.intPropertyChanged -= this.OnViewObjectPropertyChanged;
			base.Target.uintPropertyChanged -= this.OnViewObjectPropertyChanged;
			base.Target.Vec2PropertyChanged -= this.OnViewObjectPropertyChanged;
			base.Target.Vector2PropertyChanged -= this.OnViewObjectPropertyChanged;
		}

		public void BindData(string property, BindingPath path)
		{
			ViewBindDataInfo viewBindDataInfo = new ViewBindDataInfo(this, property, path);
			if (!this._bindDataInfos.ContainsKey(path.Path))
			{
				this._bindDataInfos.Add(path.Path, viewBindDataInfo);
			}
			else
			{
				this._bindDataInfos[path.Path] = viewBindDataInfo;
			}
			if (this._viewModel != null)
			{
				object propertyValue = this._viewModel.GetPropertyValue(path.LastNode);
				this.SetData(viewBindDataInfo.Property, propertyValue);
			}
		}

		public void ClearBinding(string propertyName)
		{
			foreach (KeyValuePair<string, ViewBindDataInfo> keyValuePair in this._bindDataInfos.ToArray<KeyValuePair<string, ViewBindDataInfo>>())
			{
				if (keyValuePair.Value.Property == propertyName)
				{
					this._bindDataInfos.Remove(keyValuePair.Key);
				}
			}
		}

		internal void BindCommand(string command, BindingPath path, string parameterValue = null)
		{
			ViewBindCommandInfo viewBindCommandInfo = new ViewBindCommandInfo(this, command, path, parameterValue);
			if (!this._bindCommandInfos.ContainsKey(command))
			{
				this._bindCommandInfos.Add(command, viewBindCommandInfo);
				return;
			}
			this._bindCommandInfos[command] = viewBindCommandInfo;
		}

		private void OnViewModelBindingListChanged(object sender, ListChangedEventArgs e)
		{
			switch (e.ListChangedType)
			{
			case ListChangedType.Reset:
				this.OnListReset();
				return;
			case ListChangedType.Sorted:
				this.OnListSorted();
				return;
			case ListChangedType.ItemAdded:
				this.OnItemAddedToList(e.NewIndex);
				return;
			case ListChangedType.ItemBeforeDeleted:
				this.OnBeforeItemRemovedFromList(e.NewIndex);
				break;
			case ListChangedType.ItemDeleted:
				this.OnItemRemovedFromList(e.NewIndex);
				return;
			case ListChangedType.ItemChanged:
				break;
			default:
				return;
			}
		}

		private void OnViewModelPropertyChangedWithValue(object sender, PropertyChangedWithValueEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName, e.Value);
		}

		private void OnViewModelPropertyChangedWithValue(object sender, PropertyChangedWithBoolValueEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName, e.Value);
		}

		private void OnViewModelPropertyChangedWithValue(object sender, PropertyChangedWithFloatValueEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName, e.Value);
		}

		private void OnViewModelPropertyChangedWithValue(object sender, PropertyChangedWithColorValueEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName, e.Value);
		}

		private void OnViewModelPropertyChangedWithValue(object sender, PropertyChangedWithDoubleValueEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName, e.Value);
		}

		private void OnViewModelPropertyChangedWithValue(object sender, PropertyChangedWithIntValueEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName, e.Value);
		}

		private void OnViewModelPropertyChangedWithValue(object sender, PropertyChangedWithUIntValueEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName, e.Value);
		}

		private void OnViewModelPropertyChangedWithValue(object sender, PropertyChangedWithVec2ValueEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName, e.Value);
		}

		private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			object propertyValue = this._viewModel.GetPropertyValue(e.PropertyName);
			this.OnPropertyChanged(e.PropertyName, propertyValue);
		}

		private void OnPropertyChanged(string propertyName, object value)
		{
			if (value is ViewModel || value is IMBBindingList)
			{
				MBStringBuilder mbstringBuilder = default(MBStringBuilder);
				mbstringBuilder.Initialize(16, "OnPropertyChanged");
				this.WriteViewModelPathToStringBuilder(ref mbstringBuilder);
				mbstringBuilder.Append<string>("\\");
				mbstringBuilder.Append<string>(propertyName);
				string text = mbstringBuilder.ToStringAndRelease();
				using (List<GauntletView>.Enumerator enumerator = this.Children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						GauntletView gauntletView = enumerator.Current;
						if (BindingPath.IsRelatedWithPathAsString(text, gauntletView.ViewModelPathString))
						{
							gauntletView.RefreshBindingWithChildren();
						}
					}
					return;
				}
			}
			ViewBindDataInfo viewBindDataInfo;
			if (this._bindDataInfos.Count > 0 && this._bindDataInfos.TryGetValue(propertyName, out viewBindDataInfo))
			{
				this.SetData(viewBindDataInfo.Property, value);
			}
		}

		private void OnCommand(string command, object[] args)
		{
			ViewBindCommandInfo viewBindCommandInfo = null;
			if (this._bindCommandInfos.TryGetValue(command, out viewBindCommandInfo))
			{
				object[] array;
				if (viewBindCommandInfo.Parameter != null)
				{
					array = new object[args.Length + 1];
					array[args.Length] = viewBindCommandInfo.Parameter;
				}
				else
				{
					array = new object[args.Length];
				}
				for (int i = 0; i < args.Length; i++)
				{
					object obj = args[i];
					object obj2 = this.ConvertCommandParameter(obj);
					array[i] = obj2;
				}
				BindingPath parentPath = viewBindCommandInfo.Path.ParentPath;
				BindingPath bindingPath = this.ViewModelPath;
				if (parentPath != null)
				{
					bindingPath = bindingPath.Append(parentPath);
				}
				BindingPath bindingPath2 = bindingPath.Simplify();
				IViewModel viewModel = this.GauntletMovie.ViewModel;
				string lastNode = viewBindCommandInfo.Path.LastNode;
				ViewModel viewModel2 = viewModel.GetViewModelAtPath(bindingPath2, viewBindCommandInfo.Owner.ItemTemplateUsageWithData != null && viewBindCommandInfo.Owner.ItemTemplateUsageWithData.ItemTemplateUsage != null) as ViewModel;
				if (viewModel2 != null)
				{
					viewModel2.ExecuteCommand(lastNode, array);
				}
			}
		}

		private object ConvertCommandParameter(object parameter)
		{
			object obj = parameter;
			if (parameter is Widget)
			{
				Widget widget = (Widget)parameter;
				GauntletView gauntletView = this.GauntletMovie.FindViewOf(widget);
				if (gauntletView != null)
				{
					if (gauntletView._viewModel != null)
					{
						obj = gauntletView._viewModel;
					}
					else
					{
						obj = gauntletView._bindingList;
					}
				}
				else
				{
					obj = null;
				}
			}
			return obj;
		}

		private ViewBindDataInfo GetBindDataInfoOfProperty(string propertyName)
		{
			foreach (ViewBindDataInfo viewBindDataInfo in this._bindDataInfos.Values)
			{
				if (viewBindDataInfo.Property == propertyName)
				{
					return viewBindDataInfo;
				}
			}
			return null;
		}

		private void OnListSorted()
		{
			List<GauntletView> list = new List<GauntletView>(this._items.Capacity);
			for (int i = 0; i < this._bindingList.Count; i++)
			{
				object bindingObject = this._bindingList[i];
				GauntletView gauntletView3 = this._items.Find((GauntletView gauntletView) => gauntletView._viewModel == bindingObject);
				list.Add(gauntletView3);
			}
			this._items = list;
			for (int j = 0; j < this._items.Count; j++)
			{
				BindingPath bindingPath = new BindingPath(j);
				GauntletView gauntletView2 = this._items[j];
				gauntletView2.Target.SetSiblingIndex(j, false);
				gauntletView2.InitializeViewModelPath(bindingPath);
			}
		}

		private void OnListReset()
		{
			GauntletView[] array = this._items.ToArray();
			this._items.Clear();
			foreach (GauntletView gauntletView in array)
			{
				base.Target.OnBeforeRemovedChild(gauntletView.Target);
				this._children.Remove(gauntletView);
				base.Target.RemoveChild(gauntletView.Target);
				gauntletView.ClearEventHandlersWithChildren();
			}
		}

		private void OnItemAddedToList(int index)
		{
			this.AddItemToList(index).RefreshBindingWithChildren();
		}

		private GauntletView AddItemToList(int index)
		{
			for (int i = index; i < this._items.Count; i++)
			{
				this._items[i]._viewModelPath = new BindingPath(i + 1);
			}
			BindingPath bindingPath = new BindingPath(index);
			WidgetCreationData widgetCreationData = new WidgetCreationData(this.GauntletMovie.Context, this.GauntletMovie.WidgetFactory, base.Target);
			widgetCreationData.AddExtensionData(this.GauntletMovie);
			widgetCreationData.AddExtensionData(this);
			bool flag = false;
			GauntletView gauntletView;
			if (this._items.Count == 0 && this.ItemTemplateUsageWithData.FirstItemTemplate != null)
			{
				gauntletView = this.ItemTemplateUsageWithData.FirstItemTemplate.Instantiate(widgetCreationData, this.ItemTemplateUsageWithData.GivenParameters).GetGauntletView();
			}
			else if (this._items.Count == index && this._items.Count > 0 && this.ItemTemplateUsageWithData.LastItemTemplate != null)
			{
				BindingPath viewModelPath = this._items[this._items.Count - 1]._viewModelPath;
				GauntletView gauntletView2 = this.ItemTemplateUsageWithData.DefaultItemTemplate.Instantiate(widgetCreationData, this.ItemTemplateUsageWithData.GivenParameters).GetGauntletView();
				this._items.Insert(this._items.Count, gauntletView2);
				this.RemoveItemFromList(this._items.Count - 2);
				gauntletView2.InitializeViewModelPath(viewModelPath);
				gauntletView2.RefreshBindingWithChildren();
				flag = true;
				gauntletView = this.ItemTemplateUsageWithData.LastItemTemplate.Instantiate(widgetCreationData, this.ItemTemplateUsageWithData.GivenParameters).GetGauntletView();
			}
			else
			{
				gauntletView = this.ItemTemplateUsageWithData.DefaultItemTemplate.Instantiate(widgetCreationData, this.ItemTemplateUsageWithData.GivenParameters).GetGauntletView();
			}
			gauntletView.InitializeViewModelPath(bindingPath);
			this._items.Insert(index, gauntletView);
			for (int j = (flag ? (index - 1) : index); j < this._items.Count; j++)
			{
				this._items[j].Target.SetSiblingIndex(j, flag);
			}
			return gauntletView;
		}

		private void OnItemRemovedFromList(int index)
		{
			this.RemoveItemFromList(index);
		}

		private void RemoveItemFromList(int index)
		{
			GauntletView gauntletView = this._items[index];
			this._items.RemoveAt(index);
			this._children.Remove(gauntletView);
			base.Target.RemoveChild(gauntletView.Target);
			gauntletView.ClearEventHandlersWithChildren();
			for (int i = index; i < this._items.Count; i++)
			{
				this._items[i].Target.SetSiblingIndex(i, false);
			}
			BindingPath bindingPath = gauntletView._viewModelPath;
			for (int j = index; j < this._items.Count; j++)
			{
				GauntletView gauntletView2 = this._items[j];
				BindingPath viewModelPath = gauntletView2._viewModelPath;
				gauntletView2._viewModelPath = bindingPath;
				bindingPath = viewModelPath;
			}
		}

		private void OnBeforeItemRemovedFromList(int index)
		{
			this.PreviewRemoveItemFromList(index);
		}

		private void PreviewRemoveItemFromList(int index)
		{
			GauntletView gauntletView = this._items[index];
			base.Target.OnBeforeRemovedChild(gauntletView.Target);
		}

		private void SetData(string path, object value)
		{
			WidgetExtensions.SetWidgetAttribute(this.GauntletMovie.Context, base.Target, path, value);
		}

		private void OnViewPropertyChanged(string propertyName, object value)
		{
			if (this._viewModel != null)
			{
				ViewBindDataInfo bindDataInfoOfProperty = this.GetBindDataInfoOfProperty(propertyName);
				if (bindDataInfoOfProperty != null)
				{
					this._viewModel.SetPropertyValue(bindDataInfoOfProperty.Path.LastNode, value);
				}
			}
		}

		public string DisplayName
		{
			get
			{
				string text = "";
				if (base.Target != null)
				{
					text = text + base.Target.Id + "!" + base.Target.Tag.ToString();
				}
				if (this.ViewModelPath != null)
				{
					text = text + "@" + this.ViewModelPath.Path;
				}
				return text;
			}
		}

		private void OnViewObjectPropertyChanged(PropertyOwnerObject propertyOwnerObject, string propertyName, object value)
		{
			this.OnViewPropertyChanged(propertyName, value);
		}

		private void OnViewObjectPropertyChanged(PropertyOwnerObject propertyOwnerObject, string propertyName, bool value)
		{
			this.OnViewPropertyChanged(propertyName, value);
		}

		private void OnViewObjectPropertyChanged(PropertyOwnerObject propertyOwnerObject, string propertyName, float value)
		{
			this.OnViewPropertyChanged(propertyName, value);
		}

		private void OnViewObjectPropertyChanged(PropertyOwnerObject propertyOwnerObject, string propertyName, Color value)
		{
			this.OnViewPropertyChanged(propertyName, value);
		}

		private void OnViewObjectPropertyChanged(PropertyOwnerObject propertyOwnerObject, string propertyName, double value)
		{
			this.OnViewPropertyChanged(propertyName, value);
		}

		private void OnViewObjectPropertyChanged(PropertyOwnerObject propertyOwnerObject, string propertyName, int value)
		{
			this.OnViewPropertyChanged(propertyName, value);
		}

		private void OnViewObjectPropertyChanged(PropertyOwnerObject propertyOwnerObject, string propertyName, uint value)
		{
			this.OnViewPropertyChanged(propertyName, value);
		}

		private void OnViewObjectPropertyChanged(PropertyOwnerObject propertyOwnerObject, string propertyName, Vec2 value)
		{
			this.OnViewPropertyChanged(propertyName, value);
		}

		private void OnViewObjectPropertyChanged(PropertyOwnerObject propertyOwnerObject, string propertyName, Vector2 value)
		{
			this.OnViewPropertyChanged(propertyName, value);
		}

		private BindingPath _viewModelPath;

		private Dictionary<string, ViewBindDataInfo> _bindDataInfos;

		private Dictionary<string, ViewBindCommandInfo> _bindCommandInfos;

		private IViewModel _viewModel;

		private IMBBindingList _bindingList;

		private MBList<GauntletView> _children;

		private List<GauntletView> _items;
	}
}

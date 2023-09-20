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
	// Token: 0x02000003 RID: 3
	public class GauntletView : WidgetComponent
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600001A RID: 26 RVA: 0x0000244D File Offset: 0x0000064D
		// (set) Token: 0x0600001B RID: 27 RVA: 0x00002455 File Offset: 0x00000655
		internal bool AddedToChildren { get; private set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600001C RID: 28 RVA: 0x0000245E File Offset: 0x0000065E
		// (set) Token: 0x0600001D RID: 29 RVA: 0x0000246B File Offset: 0x0000066B
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

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600001E RID: 30 RVA: 0x00002479 File Offset: 0x00000679
		// (set) Token: 0x0600001F RID: 31 RVA: 0x00002481 File Offset: 0x00000681
		public GauntletMovie GauntletMovie { get; private set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000020 RID: 32 RVA: 0x0000248A File Offset: 0x0000068A
		// (set) Token: 0x06000021 RID: 33 RVA: 0x00002492 File Offset: 0x00000692
		public ItemTemplateUsageWithData ItemTemplateUsageWithData { get; internal set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000022 RID: 34 RVA: 0x0000249C File Offset: 0x0000069C
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

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000023 RID: 35 RVA: 0x000024E8 File Offset: 0x000006E8
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

		// Token: 0x06000024 RID: 36 RVA: 0x0000251C File Offset: 0x0000071C
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

		// Token: 0x06000025 RID: 37 RVA: 0x0000258A File Offset: 0x0000078A
		internal void InitializeViewModelPath(BindingPath path)
		{
			this._viewModelPath = path;
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000026 RID: 38 RVA: 0x00002593 File Offset: 0x00000793
		public MBReadOnlyList<GauntletView> Children
		{
			get
			{
				return this._children;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000027 RID: 39 RVA: 0x0000259B File Offset: 0x0000079B
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

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000028 RID: 40 RVA: 0x000025AB File Offset: 0x000007AB
		// (set) Token: 0x06000029 RID: 41 RVA: 0x000025B3 File Offset: 0x000007B3
		public GauntletView Parent { get; private set; }

		// Token: 0x0600002A RID: 42 RVA: 0x000025BC File Offset: 0x000007BC
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

		// Token: 0x0600002B RID: 43 RVA: 0x0000260E File Offset: 0x0000080E
		public void AddChild(GauntletView child)
		{
			this._children.Add(child);
			child.AddedToChildren = true;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002623 File Offset: 0x00000823
		public void RemoveChild(GauntletView child)
		{
			base.Target.OnBeforeRemovedChild(child.Target);
			base.Target.RemoveChild(child.Target);
			this._children.Remove(child);
			child.ClearEventHandlersWithChildren();
		}

		// Token: 0x0600002D RID: 45 RVA: 0x0000265C File Offset: 0x0000085C
		public void SwapChildrenAtIndeces(GauntletView child1, GauntletView child2)
		{
			int num = this._children.IndexOf(child1);
			int num2 = this._children.IndexOf(child2);
			base.Target.SwapChildren(this._children[num].Target, this._children[num2].Target);
			GauntletView gauntletView = this._children[num];
			this._children[num] = this._children[num2];
			this._children[num2] = gauntletView;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000026E4 File Offset: 0x000008E4
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

		// Token: 0x0600002F RID: 47 RVA: 0x000029B8 File Offset: 0x00000BB8
		private void OnEventFired(Widget widget, string commandName, object[] args)
		{
			this.OnCommand(commandName, args);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000029C4 File Offset: 0x00000BC4
		public void RefreshBindingWithChildren()
		{
			this.RefreshBinding();
			for (int i = 0; i < this._children.Count; i++)
			{
				this._children[i].RefreshBindingWithChildren();
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002A00 File Offset: 0x00000C00
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

		// Token: 0x06000032 RID: 50 RVA: 0x00002B08 File Offset: 0x00000D08
		public void ReleaseBindingWithChildren()
		{
			this.ReleaseBinding();
			for (int i = 0; i < this._children.Count; i++)
			{
				this._children[i].ReleaseBindingWithChildren();
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002B44 File Offset: 0x00000D44
		internal void ClearEventHandlersWithChildren()
		{
			this.ClearEventHandlers();
			for (int i = 0; i < this._children.Count; i++)
			{
				this._children[i].ClearEventHandlersWithChildren();
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002B80 File Offset: 0x00000D80
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

		// Token: 0x06000035 RID: 53 RVA: 0x00002D80 File Offset: 0x00000F80
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

		// Token: 0x06000036 RID: 54 RVA: 0x00002DF8 File Offset: 0x00000FF8
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

		// Token: 0x06000037 RID: 55 RVA: 0x00002E50 File Offset: 0x00001050
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

		// Token: 0x06000038 RID: 56 RVA: 0x00002E90 File Offset: 0x00001090
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

		// Token: 0x06000039 RID: 57 RVA: 0x00002EF7 File Offset: 0x000010F7
		private void OnViewModelPropertyChangedWithValue(object sender, PropertyChangedWithValueEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName, e.Value);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002F0B File Offset: 0x0000110B
		private void OnViewModelPropertyChangedWithValue(object sender, PropertyChangedWithBoolValueEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName, e.Value);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002F24 File Offset: 0x00001124
		private void OnViewModelPropertyChangedWithValue(object sender, PropertyChangedWithFloatValueEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName, e.Value);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002F3D File Offset: 0x0000113D
		private void OnViewModelPropertyChangedWithValue(object sender, PropertyChangedWithColorValueEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName, e.Value);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002F56 File Offset: 0x00001156
		private void OnViewModelPropertyChangedWithValue(object sender, PropertyChangedWithDoubleValueEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName, e.Value);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002F6F File Offset: 0x0000116F
		private void OnViewModelPropertyChangedWithValue(object sender, PropertyChangedWithIntValueEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName, e.Value);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002F88 File Offset: 0x00001188
		private void OnViewModelPropertyChangedWithValue(object sender, PropertyChangedWithUIntValueEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName, e.Value);
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00002FA1 File Offset: 0x000011A1
		private void OnViewModelPropertyChangedWithValue(object sender, PropertyChangedWithVec2ValueEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName, e.Value);
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00002FBC File Offset: 0x000011BC
		private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			object propertyValue = this._viewModel.GetPropertyValue(e.PropertyName);
			this.OnPropertyChanged(e.PropertyName, propertyValue);
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00002FE8 File Offset: 0x000011E8
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

		// Token: 0x06000043 RID: 67 RVA: 0x000030C0 File Offset: 0x000012C0
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

		// Token: 0x06000044 RID: 68 RVA: 0x000031B8 File Offset: 0x000013B8
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

		// Token: 0x06000045 RID: 69 RVA: 0x00003204 File Offset: 0x00001404
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

		// Token: 0x06000046 RID: 70 RVA: 0x0000326C File Offset: 0x0000146C
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

		// Token: 0x06000047 RID: 71 RVA: 0x00003324 File Offset: 0x00001524
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

		// Token: 0x06000048 RID: 72 RVA: 0x0000338F File Offset: 0x0000158F
		private void OnItemAddedToList(int index)
		{
			this.AddItemToList(index).RefreshBindingWithChildren();
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000033A0 File Offset: 0x000015A0
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

		// Token: 0x0600004A RID: 74 RVA: 0x000035AB File Offset: 0x000017AB
		private void OnItemRemovedFromList(int index)
		{
			this.RemoveItemFromList(index);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000035B4 File Offset: 0x000017B4
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

		// Token: 0x0600004C RID: 76 RVA: 0x00003666 File Offset: 0x00001866
		private void OnBeforeItemRemovedFromList(int index)
		{
			this.PreviewRemoveItemFromList(index);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003670 File Offset: 0x00001870
		private void PreviewRemoveItemFromList(int index)
		{
			GauntletView gauntletView = this._items[index];
			base.Target.OnBeforeRemovedChild(gauntletView.Target);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x0000369B File Offset: 0x0000189B
		private void SetData(string path, object value)
		{
			WidgetExtensions.SetWidgetAttribute(this.GauntletMovie.Context, base.Target, path, value);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000036B8 File Offset: 0x000018B8
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

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000050 RID: 80 RVA: 0x000036F0 File Offset: 0x000018F0
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

		// Token: 0x06000051 RID: 81 RVA: 0x00003758 File Offset: 0x00001958
		private void OnViewObjectPropertyChanged(PropertyOwnerObject propertyOwnerObject, string propertyName, object value)
		{
			this.OnViewPropertyChanged(propertyName, value);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003762 File Offset: 0x00001962
		private void OnViewObjectPropertyChanged(PropertyOwnerObject propertyOwnerObject, string propertyName, bool value)
		{
			this.OnViewPropertyChanged(propertyName, value);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003771 File Offset: 0x00001971
		private void OnViewObjectPropertyChanged(PropertyOwnerObject propertyOwnerObject, string propertyName, float value)
		{
			this.OnViewPropertyChanged(propertyName, value);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003780 File Offset: 0x00001980
		private void OnViewObjectPropertyChanged(PropertyOwnerObject propertyOwnerObject, string propertyName, Color value)
		{
			this.OnViewPropertyChanged(propertyName, value);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x0000378F File Offset: 0x0000198F
		private void OnViewObjectPropertyChanged(PropertyOwnerObject propertyOwnerObject, string propertyName, double value)
		{
			this.OnViewPropertyChanged(propertyName, value);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x0000379E File Offset: 0x0000199E
		private void OnViewObjectPropertyChanged(PropertyOwnerObject propertyOwnerObject, string propertyName, int value)
		{
			this.OnViewPropertyChanged(propertyName, value);
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000037AD File Offset: 0x000019AD
		private void OnViewObjectPropertyChanged(PropertyOwnerObject propertyOwnerObject, string propertyName, uint value)
		{
			this.OnViewPropertyChanged(propertyName, value);
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000037BC File Offset: 0x000019BC
		private void OnViewObjectPropertyChanged(PropertyOwnerObject propertyOwnerObject, string propertyName, Vec2 value)
		{
			this.OnViewPropertyChanged(propertyName, value);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000037CB File Offset: 0x000019CB
		private void OnViewObjectPropertyChanged(PropertyOwnerObject propertyOwnerObject, string propertyName, Vector2 value)
		{
			this.OnViewPropertyChanged(propertyName, value);
		}

		// Token: 0x0400000C RID: 12
		private BindingPath _viewModelPath;

		// Token: 0x0400000E RID: 14
		private Dictionary<string, ViewBindDataInfo> _bindDataInfos;

		// Token: 0x0400000F RID: 15
		private Dictionary<string, ViewBindCommandInfo> _bindCommandInfos;

		// Token: 0x04000010 RID: 16
		private IViewModel _viewModel;

		// Token: 0x04000011 RID: 17
		private IMBBindingList _bindingList;

		// Token: 0x04000012 RID: 18
		private MBList<GauntletView> _children;

		// Token: 0x04000013 RID: 19
		private List<GauntletView> _items;
	}
}

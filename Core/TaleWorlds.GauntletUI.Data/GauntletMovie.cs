using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.PrefabSystem;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI.Data
{
	// Token: 0x02000002 RID: 2
	public class GauntletMovie : IGauntletMovie
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002050 File Offset: 0x00000250
		public WidgetFactory WidgetFactory { get; private set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002059 File Offset: 0x00000259
		// (set) Token: 0x06000004 RID: 4 RVA: 0x00002061 File Offset: 0x00000261
		public BrushFactory BrushFactory { get; private set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000005 RID: 5 RVA: 0x0000206A File Offset: 0x0000026A
		// (set) Token: 0x06000006 RID: 6 RVA: 0x00002072 File Offset: 0x00000272
		public UIContext Context { get; private set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000007 RID: 7 RVA: 0x0000207B File Offset: 0x0000027B
		public IViewModel ViewModel
		{
			get
			{
				return this._viewModel;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000008 RID: 8 RVA: 0x00002083 File Offset: 0x00000283
		// (set) Token: 0x06000009 RID: 9 RVA: 0x0000208B File Offset: 0x0000028B
		public string MovieName { get; private set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000A RID: 10 RVA: 0x00002094 File Offset: 0x00000294
		// (set) Token: 0x0600000B RID: 11 RVA: 0x0000209C File Offset: 0x0000029C
		public GauntletView RootView { get; private set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000020A5 File Offset: 0x000002A5
		public Widget RootWidget
		{
			get
			{
				return this.RootView.Target;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600000D RID: 13 RVA: 0x000020B2 File Offset: 0x000002B2
		// (set) Token: 0x0600000E RID: 14 RVA: 0x000020BA File Offset: 0x000002BA
		public bool IsReleased { get; private set; }

		// Token: 0x0600000F RID: 15 RVA: 0x000020C4 File Offset: 0x000002C4
		private GauntletMovie(string movieName, UIContext context, WidgetFactory widgetFactory, IViewModel viewModel, bool hotReloadEnabled)
		{
			this.WidgetFactory = widgetFactory;
			this.BrushFactory = context.BrushFactory;
			this.Context = context;
			if (hotReloadEnabled)
			{
				this.WidgetFactory.PrefabChange += this.OnResourceChanged;
				this.BrushFactory.BrushChange += this.OnResourceChanged;
			}
			this._viewModel = viewModel;
			this.MovieName = movieName;
			this._movieRootNode = new Widget(this.Context);
			this.Context.Root.AddChild(this._movieRootNode);
			this._movieRootNode.WidthSizePolicy = SizePolicy.Fixed;
			this._movieRootNode.HeightSizePolicy = SizePolicy.Fixed;
			this._movieRootNode.ScaledSuggestedWidth = this.Context.TwoDimensionContext.Width;
			this._movieRootNode.ScaledSuggestedHeight = this.Context.TwoDimensionContext.Height;
			this._movieRootNode.DoNotAcceptEvents = true;
			this.IsReleased = false;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000021BA File Offset: 0x000003BA
		public void RefreshDataSource(IViewModel dataSourve)
		{
			this._viewModel = dataSourve;
			this.RootView.RefreshBindingWithChildren();
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000021CE File Offset: 0x000003CE
		private void OnResourceChanged()
		{
			this.RootView.ClearEventHandlersWithChildren();
			this.RootView = null;
			this._movieRootNode.RemoveAllChildren();
			this.Context.EventManager.OnMovieReleased(this.MovieName);
			this.LoadMovie();
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000220C File Offset: 0x0000040C
		private void LoadMovie()
		{
			this._moviePrefab = this.WidgetFactory.GetCustomType(this.MovieName);
			WidgetCreationData widgetCreationData = new WidgetCreationData(this.Context, this.WidgetFactory);
			widgetCreationData.AddExtensionData(this);
			WidgetInstantiationResult widgetInstantiationResult = this._moviePrefab.Instantiate(widgetCreationData);
			this.RootView = widgetInstantiationResult.GetGauntletView();
			Widget target = this.RootView.Target;
			this._movieRootNode.AddChild(target);
			this.RootView.RefreshBindingWithChildren();
			this.Context.EventManager.OnMovieLoaded(this.MovieName);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x0000229C File Offset: 0x0000049C
		public void Release()
		{
			this.IsReleased = true;
			this._movieRootNode.OnBeforeRemovedChild(this._movieRootNode);
			GauntletView rootView = this.RootView;
			if (rootView != null)
			{
				rootView.ReleaseBindingWithChildren();
			}
			this._moviePrefab.OnRelease();
			this.WidgetFactory.OnUnload(this.MovieName);
			this.WidgetFactory.PrefabChange -= this.OnResourceChanged;
			this.BrushFactory.BrushChange -= this.OnResourceChanged;
			this.Context.EventManager.OnMovieReleased(this.MovieName);
			this._movieRootNode.ParentWidget = null;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000233E File Offset: 0x0000053E
		internal void OnItemRemoved(string type)
		{
			this.WidgetFactory.OnUnload(type);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000234C File Offset: 0x0000054C
		public void Update()
		{
			this._movieRootNode.ScaledSuggestedWidth = this.Context.TwoDimensionContext.Width;
			this._movieRootNode.ScaledSuggestedHeight = this.Context.TwoDimensionContext.Height;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002384 File Offset: 0x00000584
		internal object GetViewModelAtPath(BindingPath path, bool isListExpected)
		{
			if (this._viewModel != null && path != null)
			{
				BindingPath bindingPath = path.Simplify();
				return this._viewModel.GetViewModelAtPath(bindingPath, isListExpected);
			}
			return null;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000023B8 File Offset: 0x000005B8
		public static IGauntletMovie Load(UIContext context, WidgetFactory widgetFactory, string movieName, IViewModel datasource, bool doNotUseGeneratedPrefabs, bool hotReloadEnabled)
		{
			IGauntletMovie gauntletMovie = null;
			if (!doNotUseGeneratedPrefabs)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				string text = "Default";
				if (datasource != null)
				{
					dictionary.Add("DataSource", datasource);
					text = datasource.GetType().FullName;
				}
				GeneratedPrefabInstantiationResult generatedPrefabInstantiationResult = widgetFactory.GeneratedPrefabContext.InstantiatePrefab(context, movieName, text, dictionary);
				if (generatedPrefabInstantiationResult != null)
				{
					gauntletMovie = generatedPrefabInstantiationResult.GetExtensionData("Movie") as IGauntletMovie;
					context.EventManager.OnMovieLoaded(movieName);
				}
			}
			if (gauntletMovie == null)
			{
				GauntletMovie gauntletMovie2 = new GauntletMovie(movieName, context, widgetFactory, datasource, hotReloadEnabled);
				gauntletMovie2.LoadMovie();
				gauntletMovie = gauntletMovie2;
			}
			return gauntletMovie;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002438 File Offset: 0x00000638
		public void RefreshBindingWithChildren()
		{
			this.RootView.RefreshBindingWithChildren();
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002445 File Offset: 0x00000645
		public GauntletView FindViewOf(Widget widget)
		{
			return widget.GetComponent<GauntletView>();
		}

		// Token: 0x04000004 RID: 4
		private WidgetPrefab _moviePrefab;

		// Token: 0x04000005 RID: 5
		private IViewModel _viewModel;

		// Token: 0x04000007 RID: 7
		private Widget _movieRootNode;
	}
}

using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.PrefabSystem;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI.Data
{
	public class GauntletMovie : IGauntletMovie
	{
		public WidgetFactory WidgetFactory { get; private set; }

		public BrushFactory BrushFactory { get; private set; }

		public UIContext Context { get; private set; }

		public IViewModel ViewModel
		{
			get
			{
				return this._viewModel;
			}
		}

		public string MovieName { get; private set; }

		public GauntletView RootView { get; private set; }

		public Widget RootWidget
		{
			get
			{
				return this.RootView.Target;
			}
		}

		public bool IsReleased { get; private set; }

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

		public void RefreshDataSource(IViewModel dataSourve)
		{
			this._viewModel = dataSourve;
			this.RootView.RefreshBindingWithChildren();
		}

		private void OnResourceChanged()
		{
			this.RootView.ClearEventHandlersWithChildren();
			this.RootView = null;
			this._movieRootNode.RemoveAllChildren();
			this.Context.EventManager.OnMovieReleased(this.MovieName);
			this.LoadMovie();
		}

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

		internal void OnItemRemoved(string type)
		{
			this.WidgetFactory.OnUnload(type);
		}

		public void Update()
		{
			this._movieRootNode.ScaledSuggestedWidth = this.Context.TwoDimensionContext.Width;
			this._movieRootNode.ScaledSuggestedHeight = this.Context.TwoDimensionContext.Height;
		}

		internal object GetViewModelAtPath(BindingPath path, bool isListExpected)
		{
			if (this._viewModel != null && path != null)
			{
				BindingPath bindingPath = path.Simplify();
				return this._viewModel.GetViewModelAtPath(bindingPath, isListExpected);
			}
			return null;
		}

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

		public void RefreshBindingWithChildren()
		{
			this.RootView.RefreshBindingWithChildren();
		}

		public GauntletView FindViewOf(Widget widget)
		{
			return widget.GetComponent<GauntletView>();
		}

		private WidgetPrefab _moviePrefab;

		private IViewModel _viewModel;

		private Widget _movieRootNode;
	}
}

using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.Data
{
	public class GeneratedGauntletMovie : IGauntletMovie
	{
		public UIContext Context { get; private set; }

		public Widget RootWidget { get; private set; }

		public string MovieName { get; private set; }

		public bool IsReleased { get; private set; }

		public GeneratedGauntletMovie(string movieName, Widget rootWidget)
		{
			this.MovieName = movieName;
			this.RootWidget = rootWidget;
			this.Context = rootWidget.Context;
			this._root = (IGeneratedGauntletMovieRoot)rootWidget;
			this._movieRootNode = new Widget(this.Context);
			this.Context.Root.AddChild(this._movieRootNode);
			this._movieRootNode.WidthSizePolicy = SizePolicy.Fixed;
			this._movieRootNode.HeightSizePolicy = SizePolicy.Fixed;
			this._movieRootNode.ScaledSuggestedWidth = this.Context.TwoDimensionContext.Width;
			this._movieRootNode.ScaledSuggestedHeight = this.Context.TwoDimensionContext.Height;
			this._movieRootNode.DoNotAcceptEvents = true;
			this._movieRootNode.AddChild(rootWidget);
		}

		public void Update()
		{
			this._movieRootNode.ScaledSuggestedWidth = this.Context.TwoDimensionContext.Width;
			this._movieRootNode.ScaledSuggestedHeight = this.Context.TwoDimensionContext.Height;
		}

		public void Release()
		{
			this.IsReleased = true;
			this._movieRootNode.OnBeforeRemovedChild(this._movieRootNode);
			this._root.DestroyDataSource();
			this._movieRootNode.ParentWidget = null;
			this.Context.EventManager.OnMovieReleased(this.MovieName);
		}

		public void RefreshBindingWithChildren()
		{
			this._root.RefreshBindingWithChildren();
		}

		private Widget _movieRootNode;

		private IGeneratedGauntletMovieRoot _root;
	}
}

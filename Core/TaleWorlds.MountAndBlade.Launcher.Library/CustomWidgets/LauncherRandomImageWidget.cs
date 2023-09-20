using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Launcher.Library.CustomWidgets
{
	public class LauncherRandomImageWidget : Widget
	{
		public LauncherRandomImageWidget(UIContext context)
			: base(context)
		{
			this._random = new Random();
		}

		private void ShuffleList<T>(List<T> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				T t = list[i];
				int num = this._random.Next(i, list.Count);
				list[i] = list[num];
				list[num] = t;
			}
		}

		private void CreateIndicesList()
		{
			this._imageIndices = new List<int>();
			for (int i = 0; i < this.ImageCount; i++)
			{
				this._imageIndices.Add(i);
			}
			this.ShuffleList<int>(this._imageIndices);
		}

		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			this.CreateIndicesList();
			int num = this._imageIndices[this._currentIndex];
			base.Sprite = base.Context.SpriteData.GetSprite("ConceptArts\\ConceptArt_" + num);
		}

		private void TriggerChanged()
		{
			this._currentIndex = (this._currentIndex + 1) % this._imageIndices.Count;
			int num = this._imageIndices[this._currentIndex];
			base.Sprite = base.Context.SpriteData.GetSprite("ConceptArts\\ConceptArt_" + num);
		}

		[DataSourceProperty]
		public int ImageCount
		{
			get
			{
				return this._imageCount;
			}
			set
			{
				if (value != this._imageCount)
				{
					this._imageCount = value;
					base.OnPropertyChanged(value, "ImageCount");
				}
			}
		}

		[DataSourceProperty]
		public bool ChangeTrigger
		{
			get
			{
				return this._changeTrigger;
			}
			set
			{
				if (value != this._changeTrigger)
				{
					this._changeTrigger = value;
					base.OnPropertyChanged(value, "ChangeTrigger");
					this.TriggerChanged();
				}
			}
		}

		private readonly Random _random;

		private List<int> _imageIndices;

		private int _currentIndex;

		private int _imageCount;

		private bool _changeTrigger;
	}
}

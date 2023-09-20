using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection
{
	public class TournamentRewardVM : ViewModel
	{
		public TournamentRewardVM(string text)
		{
			this.Text = text;
			this.GotImageIdentifier = false;
			this.ImageIdentifier = new ImageIdentifierVM(0);
		}

		public TournamentRewardVM(string text, ImageIdentifierVM imageIdentifierVM)
		{
			this.Text = text;
			this.GotImageIdentifier = true;
			this.ImageIdentifier = imageIdentifierVM;
		}

		[DataSourceProperty]
		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				if (value != this._text)
				{
					this._text = value;
					base.OnPropertyChangedWithValue<string>(value, "Text");
				}
			}
		}

		[DataSourceProperty]
		public bool GotImageIdentifier
		{
			get
			{
				return this._gotImageIdentifier;
			}
			set
			{
				if (value != this._gotImageIdentifier)
				{
					this._gotImageIdentifier = value;
					base.OnPropertyChangedWithValue(value, "GotImageIdentifier");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM ImageIdentifier
		{
			get
			{
				return this._imageIdentifier;
			}
			set
			{
				if (value != this._imageIdentifier)
				{
					this._imageIdentifier = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ImageIdentifier");
				}
			}
		}

		private string _text;

		private ImageIdentifierVM _imageIdentifier;

		private bool _gotImageIdentifier;
	}
}

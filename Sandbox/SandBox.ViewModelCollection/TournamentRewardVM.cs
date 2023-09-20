using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection
{
	// Token: 0x02000007 RID: 7
	public class TournamentRewardVM : ViewModel
	{
		// Token: 0x06000038 RID: 56 RVA: 0x00004EFB File Offset: 0x000030FB
		public TournamentRewardVM(string text)
		{
			this.Text = text;
			this.GotImageIdentifier = false;
			this.ImageIdentifier = new ImageIdentifierVM(0);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00004F1D File Offset: 0x0000311D
		public TournamentRewardVM(string text, ImageIdentifierVM imageIdentifierVM)
		{
			this.Text = text;
			this.GotImageIdentifier = true;
			this.ImageIdentifier = imageIdentifierVM;
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00004F3A File Offset: 0x0000313A
		// (set) Token: 0x0600003B RID: 59 RVA: 0x00004F42 File Offset: 0x00003142
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

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00004F65 File Offset: 0x00003165
		// (set) Token: 0x0600003D RID: 61 RVA: 0x00004F6D File Offset: 0x0000316D
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

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600003E RID: 62 RVA: 0x00004F8B File Offset: 0x0000318B
		// (set) Token: 0x0600003F RID: 63 RVA: 0x00004F93 File Offset: 0x00003193
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

		// Token: 0x0400000F RID: 15
		private string _text;

		// Token: 0x04000010 RID: 16
		private ImageIdentifierVM _imageIdentifier;

		// Token: 0x04000011 RID: 17
		private bool _gotImageIdentifier;
	}
}

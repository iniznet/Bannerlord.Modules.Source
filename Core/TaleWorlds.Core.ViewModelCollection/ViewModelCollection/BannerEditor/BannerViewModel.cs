using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.BannerEditor
{
	public class BannerViewModel : ViewModel
	{
		public Banner Banner { get; }

		public BannerViewModel(Banner banner)
		{
			this.Banner = banner;
		}

		public void SetCode(string code)
		{
			this.BannerCode = code;
		}

		public void SetIconMeshID(int meshID)
		{
			this.Banner.BannerDataList[1].MeshId = meshID;
		}

		public void SetPrimaryColorId(int colorID)
		{
			this.Banner.BannerDataList[0].ColorId = colorID;
		}

		public void SetSecondaryColorId(int colorID)
		{
			this.Banner.BannerDataList[0].ColorId2 = colorID;
		}

		public void SetSigilColorId(int colorID)
		{
			this.Banner.BannerDataList[1].ColorId = colorID;
		}

		public void SetIconSize(int newSize)
		{
			this.Banner.BannerDataList[1].Size = new Vec2((float)newSize, (float)newSize);
		}

		public int GetPrimaryColorId()
		{
			return this.Banner.BannerDataList[0].ColorId;
		}

		public uint GetPrimaryColor()
		{
			return BannerManager.Instance.ReadOnlyColorPalette.First((KeyValuePair<int, BannerColor> w) => w.Key == this.GetPrimaryColorId()).Value.Color;
		}

		public int GetSecondaryColorId()
		{
			return this.Banner.BannerDataList[0].ColorId2;
		}

		public int GetSigilColorId()
		{
			return this.Banner.BannerDataList[1].ColorId;
		}

		public uint GetSigilColor()
		{
			return BannerManager.Instance.ReadOnlyColorPalette.First((KeyValuePair<int, BannerColor> w) => w.Key == this.GetSigilColorId()).Value.Color;
		}

		[DataSourceProperty]
		public string BannerCode
		{
			get
			{
				return this._bannerCode;
			}
			set
			{
				if (value != this._bannerCode)
				{
					this._bannerCode = value;
					base.OnPropertyChangedWithValue<string>(value, "BannerCode");
					this.Banner.Deserialize(value);
				}
			}
		}

		private string _bannerCode = "";

		private const int _backgroundIndex = 0;

		private const int _bannerIconIndex = 1;
	}
}

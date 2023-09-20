using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.BannerEditor
{
	// Token: 0x02000027 RID: 39
	public class BannerViewModel : ViewModel
	{
		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060001B1 RID: 433 RVA: 0x0000566C File Offset: 0x0000386C
		public Banner Banner { get; }

		// Token: 0x060001B2 RID: 434 RVA: 0x00005674 File Offset: 0x00003874
		public BannerViewModel(Banner banner)
		{
			this.Banner = banner;
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000568E File Offset: 0x0000388E
		public void SetCode(string code)
		{
			this.BannerCode = code;
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x00005697 File Offset: 0x00003897
		public void SetIconMeshID(int meshID)
		{
			this.Banner.BannerDataList[1].MeshId = meshID;
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x000056B0 File Offset: 0x000038B0
		public void SetPrimaryColorId(int colorID)
		{
			this.Banner.BannerDataList[0].ColorId = colorID;
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x000056C9 File Offset: 0x000038C9
		public void SetSecondaryColorId(int colorID)
		{
			this.Banner.BannerDataList[0].ColorId2 = colorID;
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x000056E2 File Offset: 0x000038E2
		public void SetSigilColorId(int colorID)
		{
			this.Banner.BannerDataList[1].ColorId = colorID;
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x000056FB File Offset: 0x000038FB
		public void SetIconSize(int newSize)
		{
			this.Banner.BannerDataList[1].Size = new Vec2((float)newSize, (float)newSize);
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000571C File Offset: 0x0000391C
		public int GetPrimaryColorId()
		{
			return this.Banner.BannerDataList[0].ColorId;
		}

		// Token: 0x060001BA RID: 442 RVA: 0x00005734 File Offset: 0x00003934
		public uint GetPrimaryColor()
		{
			return BannerManager.Instance.ReadOnlyColorPalette.First((KeyValuePair<int, BannerColor> w) => w.Key == this.GetPrimaryColorId()).Value.Color;
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000576C File Offset: 0x0000396C
		public int GetSecondaryColorId()
		{
			return this.Banner.BannerDataList[0].ColorId2;
		}

		// Token: 0x060001BC RID: 444 RVA: 0x00005784 File Offset: 0x00003984
		public int GetSigilColorId()
		{
			return this.Banner.BannerDataList[1].ColorId;
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000579C File Offset: 0x0000399C
		public uint GetSigilColor()
		{
			return BannerManager.Instance.ReadOnlyColorPalette.First((KeyValuePair<int, BannerColor> w) => w.Key == this.GetSigilColorId()).Value.Color;
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060001BE RID: 446 RVA: 0x000057D4 File Offset: 0x000039D4
		// (set) Token: 0x060001BF RID: 447 RVA: 0x000057DC File Offset: 0x000039DC
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

		// Token: 0x040000B5 RID: 181
		private string _bannerCode = "";

		// Token: 0x040000B6 RID: 182
		private const int _backgroundIndex = 0;

		// Token: 0x040000B7 RID: 183
		private const int _bannerIconIndex = 1;
	}
}

using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
	public class Banner
	{
		public MBReadOnlyList<BannerData> BannerDataList
		{
			get
			{
				return this._bannerDataList;
			}
		}

		public IBannerVisual BannerVisual
		{
			get
			{
				IBannerVisual bannerVisual;
				if ((bannerVisual = this._bannerVisual) == null)
				{
					bannerVisual = (this._bannerVisual = Game.Current.CreateBannerVisual(this));
				}
				return bannerVisual;
			}
		}

		public Banner()
		{
			this._bannerDataList = new MBList<BannerData>();
		}

		public Banner(Banner banner)
		{
			this._bannerDataList = new MBList<BannerData>();
			foreach (BannerData bannerData in banner.BannerDataList)
			{
				this._bannerDataList.Add(new BannerData(bannerData));
			}
		}

		public Banner(string bannerKey)
		{
			this._bannerDataList = new MBList<BannerData>();
			this.Deserialize(bannerKey);
		}

		public Banner(string bannerKey, uint color1, uint color2)
		{
			this._bannerDataList = new MBList<BannerData>();
			this.Deserialize(bannerKey);
			this.ChangePrimaryColor(color1);
			this.ChangeIconColors(color2);
		}

		public void SetBannerVisual(IBannerVisual visual)
		{
			this._bannerVisual = visual;
		}

		public void ChangePrimaryColor(uint mainColor)
		{
			int colorId = BannerManager.GetColorId(mainColor);
			if (colorId < 0)
			{
				return;
			}
			this.BannerDataList[0].ColorId = colorId;
			this.BannerDataList[0].ColorId2 = colorId;
		}

		public void ChangeBackgroundColor(uint primaryColor, uint secondaryColor)
		{
			int colorId = BannerManager.GetColorId(primaryColor);
			int colorId2 = BannerManager.GetColorId(secondaryColor);
			if (colorId < 0)
			{
				return;
			}
			if (colorId2 < 0)
			{
				return;
			}
			this.BannerDataList[0].ColorId = colorId;
			this.BannerDataList[0].ColorId2 = colorId2;
		}

		public void ChangeIconColors(uint color)
		{
			int colorId = BannerManager.GetColorId(color);
			if (colorId < 0)
			{
				return;
			}
			for (int i = 1; i < this.BannerDataList.Count; i++)
			{
				this.BannerDataList[i].ColorId = colorId;
				this.BannerDataList[i].ColorId2 = colorId;
			}
		}

		public void RotateBackgroundToRight()
		{
			this.BannerDataList[0].RotationValue -= 0.00278f;
			this.BannerDataList[0].RotationValue = ((this.BannerDataList[0].RotationValue < 0f) ? (this.BannerDataList[0].RotationValue + 1f) : this.BannerDataList[0].RotationValue);
		}

		public void RotateBackgroundToLeft()
		{
			this.BannerDataList[0].RotationValue += 0.00278f;
			this.BannerDataList[0].RotationValue = ((this.BannerDataList[0].RotationValue > 0f) ? (this.BannerDataList[0].RotationValue - 1f) : this.BannerDataList[0].RotationValue);
		}

		public void SetBackgroundMeshId(int meshId)
		{
			this.BannerDataList[0].MeshId = meshId;
		}

		public string Serialize()
		{
			return Banner.GetBannerCodeFromBannerDataList(this._bannerDataList);
		}

		public void Deserialize(string message)
		{
			this._bannerVisual = null;
			this._bannerDataList.Clear();
			this._bannerDataList.AddRange(Banner.GetBannerDataFromBannerCode(message));
		}

		public void ClearAllIcons()
		{
			BannerData bannerData = this._bannerDataList[0];
			this._bannerDataList.Clear();
			this._bannerDataList.Add(bannerData);
		}

		public void AddIconData(BannerData iconData)
		{
			if (this._bannerDataList.Count < 33)
			{
				this._bannerDataList.Add(iconData);
			}
		}

		public static Banner CreateRandomClanBanner(int seed = -1)
		{
			return Banner.CreateRandomBannerInternal(seed, Banner.BannerIconOrientation.CentralPositionedOneIcon);
		}

		public static Banner CreateRandomBanner()
		{
			return Banner.CreateRandomBannerInternal(-1, Banner.BannerIconOrientation.None);
		}

		private static Banner CreateRandomBannerInternal(int seed = -1, Banner.BannerIconOrientation orientation = Banner.BannerIconOrientation.None)
		{
			Game game = Game.Current;
			MBFastRandom mbfastRandom = ((seed == -1) ? new MBFastRandom() : new MBFastRandom((uint)seed));
			Banner banner = new Banner();
			BannerData bannerData = new BannerData(BannerManager.Instance.GetRandomBackgroundId(mbfastRandom), mbfastRandom.Next(BannerManager.ColorPalette.Count), mbfastRandom.Next(BannerManager.ColorPalette.Count), new Vec2(1528f, 1528f), new Vec2(764f, 764f), false, false, 0f);
			banner.AddIconData(bannerData);
			switch ((orientation == Banner.BannerIconOrientation.None) ? mbfastRandom.Next(6) : ((int)orientation))
			{
			case 0:
				banner.CentralPositionedOneIcon(mbfastRandom);
				break;
			case 1:
				banner.CenteredTwoMirroredIcons(mbfastRandom);
				break;
			case 2:
				banner.DiagonalIcons(mbfastRandom);
				break;
			case 3:
				banner.HorizontalIcons(mbfastRandom);
				break;
			case 4:
				banner.VerticalIcons(mbfastRandom);
				break;
			case 5:
				banner.SquarePositionedFourIcons(mbfastRandom);
				break;
			}
			return banner;
		}

		public static Banner CreateOneColoredEmptyBanner(int colorIndex)
		{
			Banner banner = new Banner();
			BannerData bannerData = new BannerData(BannerManager.Instance.GetRandomBackgroundId(new MBFastRandom()), colorIndex, colorIndex, new Vec2(1528f, 1528f), new Vec2(764f, 764f), false, false, 0f);
			banner.AddIconData(bannerData);
			return banner;
		}

		public static Banner CreateOneColoredBannerWithOneIcon(uint backgroundColor, uint iconColor, int iconMeshId)
		{
			Banner banner = Banner.CreateOneColoredEmptyBanner(BannerManager.GetColorId(backgroundColor));
			if (iconMeshId == -1)
			{
				iconMeshId = BannerManager.Instance.GetRandomBannerIconId(new MBFastRandom());
			}
			banner.AddIconData(new BannerData(iconMeshId, BannerManager.GetColorId(iconColor), BannerManager.GetColorId(iconColor), new Vec2(512f, 512f), new Vec2(764f, 764f), false, false, 0f));
			return banner;
		}

		private void CentralPositionedOneIcon(MBFastRandom random)
		{
			int randomBannerIconId = BannerManager.Instance.GetRandomBannerIconId(random);
			int num = random.Next(BannerManager.ColorPalette.Count);
			bool flag = random.NextFloat() < 0.5f;
			int randomColorIdForStroke = this.GetRandomColorIdForStroke(flag, random);
			bool flag2 = random.Next(2) == 0;
			float num2 = random.NextFloat();
			float num3 = 0f;
			if (num2 > 0.9f)
			{
				num3 = 0.25f;
			}
			else if (num2 > 0.8f)
			{
				num3 = 0.5f;
			}
			else if (num2 > 0.7f)
			{
				num3 = 0.75f;
			}
			BannerData bannerData = new BannerData(randomBannerIconId, num, randomColorIdForStroke, new Vec2(512f, 512f), new Vec2(764f, 764f), flag, flag2, num3);
			this.AddIconData(bannerData);
		}

		private void DiagonalIcons(MBFastRandom random)
		{
			int num = ((random.NextFloat() < 0.5f) ? 2 : 3);
			bool flag = random.NextFloat() < 0.5f;
			int num2 = (512 - 20 * (num + 1)) / num;
			int num3 = BannerManager.Instance.GetRandomBannerIconId(random);
			int num4 = random.Next(BannerManager.ColorPalette.Count);
			bool flag2 = random.NextFloat() < 0.5f;
			int randomColorIdForStroke = this.GetRandomColorIdForStroke(flag2, random);
			int num5 = (512 - num * num2) / (num + 1);
			bool flag3 = random.NextFloat() < 0.3f;
			bool flag4 = flag3 || random.NextFloat() < 0.3f;
			for (int i = 0; i < num; i++)
			{
				num3 = (flag3 ? BannerManager.Instance.GetRandomBannerIconId(random) : num3);
				num4 = (flag4 ? random.Next(BannerManager.ColorPalette.Count) : num4);
				int num6 = i * (num2 + num5) + num5 + num2 / 2;
				int num7 = i * (num2 + num5) + num5 + num2 / 2;
				if (flag)
				{
					num7 = 512 - num7;
				}
				BannerData bannerData = new BannerData(num3, num4, randomColorIdForStroke, new Vec2((float)num2, (float)num2), new Vec2((float)(num6 + 508), (float)(num7 + 508)), flag2, false, 0f);
				this.AddIconData(bannerData);
			}
		}

		private void HorizontalIcons(MBFastRandom random)
		{
			int num = ((random.NextFloat() < 0.5f) ? 2 : 3);
			int num2 = (512 - 20 * (num + 1)) / num;
			int num3 = BannerManager.Instance.GetRandomBannerIconId(random);
			int num4 = random.Next(BannerManager.ColorPalette.Count);
			bool flag = random.NextFloat() < 0.5f;
			int randomColorIdForStroke = this.GetRandomColorIdForStroke(flag, random);
			int num5 = (512 - num * num2) / (num + 1);
			bool flag2 = random.NextFloat() < 0.3f;
			bool flag3 = flag2 || random.NextFloat() < 0.3f;
			for (int i = 0; i < num; i++)
			{
				num3 = (flag2 ? BannerManager.Instance.GetRandomBannerIconId(random) : num3);
				num4 = (flag3 ? random.Next(BannerManager.ColorPalette.Count) : num4);
				int num6 = i * (num2 + num5) + num5 + num2 / 2;
				BannerData bannerData = new BannerData(num3, num4, randomColorIdForStroke, new Vec2((float)num2, (float)num2), new Vec2((float)(num6 + 508), 764f), flag, false, 0f);
				this.AddIconData(bannerData);
			}
		}

		private void VerticalIcons(MBFastRandom random)
		{
			int num = ((random.NextFloat() < 0.5f) ? 2 : 3);
			int num2 = (512 - 20 * (num + 1)) / num;
			int num3 = BannerManager.Instance.GetRandomBannerIconId(random);
			int num4 = random.Next(BannerManager.ColorPalette.Count);
			bool flag = random.NextFloat() < 0.5f;
			int randomColorIdForStroke = this.GetRandomColorIdForStroke(flag, random);
			int num5 = (512 - num * num2) / (num + 1);
			bool flag2 = random.NextFloat() < 0.3f;
			bool flag3 = flag2 || random.NextFloat() < 0.3f;
			for (int i = 0; i < num; i++)
			{
				num3 = (flag2 ? BannerManager.Instance.GetRandomBannerIconId(random) : num3);
				num4 = (flag3 ? random.Next(BannerManager.ColorPalette.Count) : num4);
				int num6 = i * (num2 + num5) + num5 + num2 / 2;
				BannerData bannerData = new BannerData(num3, num4, randomColorIdForStroke, new Vec2((float)num2, (float)num2), new Vec2(764f, (float)(num6 + 508)), flag, false, 0f);
				this.AddIconData(bannerData);
			}
		}

		private void SquarePositionedFourIcons(MBFastRandom random)
		{
			bool flag = random.NextFloat() < 0.5f;
			bool flag2 = !flag && random.NextFloat() < 0.5f;
			bool flag3 = flag2 || random.NextFloat() < 0.5f;
			bool flag4 = random.NextFloat() < 0.5f;
			int num = BannerManager.Instance.GetRandomBannerIconId(random);
			int randomColorIdForStroke = this.GetRandomColorIdForStroke(flag4, random);
			int num2 = random.Next(BannerManager.ColorPalette.Count);
			BannerData bannerData = new BannerData(num, num2, randomColorIdForStroke, new Vec2(220f, 220f), new Vec2(654f, 654f), flag4, false, 0f);
			this.AddIconData(bannerData);
			num = (flag2 ? BannerManager.Instance.GetRandomBannerIconId(random) : num);
			num2 = (flag3 ? random.Next(BannerManager.ColorPalette.Count) : num2);
			bannerData = new BannerData(num, num2, randomColorIdForStroke, new Vec2(220f, 220f), new Vec2(874f, 654f), flag4, flag, 0f);
			this.AddIconData(bannerData);
			num = (flag2 ? BannerManager.Instance.GetRandomBannerIconId(random) : num);
			num2 = (flag3 ? random.Next(BannerManager.ColorPalette.Count) : num2);
			bannerData = new BannerData(num, num2, randomColorIdForStroke, new Vec2(220f, 220f), new Vec2(654f, 874f), flag4, flag, flag ? 0.5f : 0f);
			this.AddIconData(bannerData);
			num = (flag2 ? BannerManager.Instance.GetRandomBannerIconId(random) : num);
			num2 = (flag3 ? random.Next(BannerManager.ColorPalette.Count) : num2);
			bannerData = new BannerData(num, num2, randomColorIdForStroke, new Vec2(220f, 220f), new Vec2(874f, 874f), flag4, false, flag ? 0.5f : 0f);
			this.AddIconData(bannerData);
		}

		private void CenteredTwoMirroredIcons(MBFastRandom random)
		{
			bool flag = random.NextFloat() < 0.5f;
			bool flag2 = random.NextFloat() < 0.5f;
			int randomBannerIconId = BannerManager.Instance.GetRandomBannerIconId(random);
			int randomColorIdForStroke = this.GetRandomColorIdForStroke(flag2, random);
			int num = random.Next(BannerManager.ColorPalette.Count);
			BannerData bannerData = new BannerData(randomBannerIconId, num, randomColorIdForStroke, new Vec2(200f, 200f), new Vec2(664f, 764f), flag2, false, 0f);
			this.AddIconData(bannerData);
			num = (flag ? random.Next(BannerManager.ColorPalette.Count) : num);
			bannerData = new BannerData(randomBannerIconId, num, randomColorIdForStroke, new Vec2(200f, 200f), new Vec2(864f, 764f), flag2, true, 0f);
			this.AddIconData(bannerData);
		}

		private int GetRandomColorIdForStroke(bool hasStroke, MBFastRandom random)
		{
			if (!hasStroke)
			{
				return BannerManager.ColorPalette.Count - 1;
			}
			return random.Next(BannerManager.ColorPalette.Count);
		}

		public uint GetPrimaryColor()
		{
			if (this.BannerDataList.Count <= 0)
			{
				return uint.MaxValue;
			}
			return BannerManager.GetColor(this.BannerDataList[0].ColorId);
		}

		public uint GetSecondaryColor()
		{
			if (this.BannerDataList.Count <= 0)
			{
				return uint.MaxValue;
			}
			return BannerManager.GetColor(this.BannerDataList[0].ColorId2);
		}

		public uint GetFirstIconColor()
		{
			if (this.BannerDataList.Count <= 1)
			{
				return uint.MaxValue;
			}
			return BannerManager.GetColor(this.BannerDataList[1].ColorId);
		}

		public int GetVersionNo()
		{
			int num = 0;
			for (int i = 0; i < this._bannerDataList.Count; i++)
			{
				num += this._bannerDataList[i].LocalVersion;
			}
			return num;
		}

		public static string GetBannerCodeFromBannerDataList(MBList<BannerData> bannerDataList)
		{
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "GetBannerCodeFromBannerDataList");
			bool flag = true;
			foreach (BannerData bannerData in bannerDataList)
			{
				if (!flag)
				{
					mbstringBuilder.Append('.');
				}
				flag = false;
				mbstringBuilder.Append(bannerData.MeshId);
				mbstringBuilder.Append('.');
				mbstringBuilder.Append(bannerData.ColorId);
				mbstringBuilder.Append('.');
				mbstringBuilder.Append(bannerData.ColorId2);
				mbstringBuilder.Append('.');
				mbstringBuilder.Append((int)bannerData.Size.x);
				mbstringBuilder.Append('.');
				mbstringBuilder.Append((int)bannerData.Size.y);
				mbstringBuilder.Append('.');
				mbstringBuilder.Append((int)bannerData.Position.x);
				mbstringBuilder.Append('.');
				mbstringBuilder.Append((int)bannerData.Position.y);
				mbstringBuilder.Append('.');
				mbstringBuilder.Append(bannerData.DrawStroke ? 1 : 0);
				mbstringBuilder.Append('.');
				mbstringBuilder.Append(bannerData.Mirror ? 1 : 0);
				mbstringBuilder.Append('.');
				mbstringBuilder.Append((int)(bannerData.RotationValue / 0.00278f));
			}
			return mbstringBuilder.ToStringAndRelease();
		}

		public static List<BannerData> GetBannerDataFromBannerCode(string bannerCode)
		{
			List<BannerData> list = new List<BannerData>();
			string[] array = bannerCode.Split(new char[] { '.' });
			int num = 0;
			while (num + 10 <= array.Length)
			{
				BannerData bannerData = new BannerData(int.Parse(array[num]), int.Parse(array[num + 1]), int.Parse(array[num + 2]), new Vec2((float)int.Parse(array[num + 3]), (float)int.Parse(array[num + 4])), new Vec2((float)int.Parse(array[num + 5]), (float)int.Parse(array[num + 6])), int.Parse(array[num + 7]) == 1, int.Parse(array[num + 8]) == 1, (float)int.Parse(array[num + 9]) * 0.00278f);
				list.Add(bannerData);
				num += 10;
			}
			return list;
		}

		internal static void AutoGeneratedStaticCollectObjectsBanner(object o, List<object> collectedObjects)
		{
			((Banner)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this._bannerDataList);
		}

		internal static object AutoGeneratedGetMemberValue_bannerDataList(object o)
		{
			return ((Banner)o)._bannerDataList;
		}

		public const int MaxSize = 8000;

		public const int BannerFullSize = 1528;

		public const int BannerEditableAreaSize = 512;

		public const int MaxIconCount = 32;

		private const char Splitter = '.';

		public const int BackgroundDataIndex = 0;

		public const int BannerIconDataIndex = 1;

		[SaveableField(1)]
		private readonly MBList<BannerData> _bannerDataList;

		[CachedData]
		private IBannerVisual _bannerVisual;

		private enum BannerIconOrientation
		{
			None = -1,
			CentralPositionedOneIcon,
			CenteredTwoMirroredIcons,
			DiagonalIcons,
			HorizontalIcons,
			VerticalIcons,
			SquarePositionedFourIcons,
			NumberOfOrientation
		}
	}
}

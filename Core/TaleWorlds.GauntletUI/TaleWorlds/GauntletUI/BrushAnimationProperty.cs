using System;
using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x0200000A RID: 10
	public class BrushAnimationProperty
	{
		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000099 RID: 153 RVA: 0x000038AE File Offset: 0x00001AAE
		// (set) Token: 0x0600009A RID: 154 RVA: 0x000038B6 File Offset: 0x00001AB6
		public string LayerName { get; set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600009B RID: 155 RVA: 0x000038BF File Offset: 0x00001ABF
		public IEnumerable<BrushAnimationKeyFrame> KeyFrames
		{
			get
			{
				return this._keyFrames.AsReadOnly();
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600009C RID: 156 RVA: 0x000038CC File Offset: 0x00001ACC
		public int Count
		{
			get
			{
				return this._keyFrames.Count;
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000038D9 File Offset: 0x00001AD9
		public BrushAnimationProperty()
		{
			this._keyFrames = new List<BrushAnimationKeyFrame>();
		}

		// Token: 0x0600009E RID: 158 RVA: 0x000038EC File Offset: 0x00001AEC
		public BrushAnimationKeyFrame GetFrameAfter(float time)
		{
			for (int i = 0; i < this._keyFrames.Count; i++)
			{
				BrushAnimationKeyFrame brushAnimationKeyFrame = this._keyFrames[i];
				if (time < brushAnimationKeyFrame.Time)
				{
					return brushAnimationKeyFrame;
				}
			}
			return null;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00003928 File Offset: 0x00001B28
		public BrushAnimationKeyFrame GetFrameAt(int i)
		{
			if (i >= 0 && i < this._keyFrames.Count)
			{
				return this._keyFrames[i];
			}
			return null;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x0000394A File Offset: 0x00001B4A
		public BrushAnimationProperty Clone()
		{
			BrushAnimationProperty brushAnimationProperty = new BrushAnimationProperty();
			brushAnimationProperty.FillFrom(this);
			return brushAnimationProperty;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00003958 File Offset: 0x00001B58
		private void FillFrom(BrushAnimationProperty collection)
		{
			this.PropertyType = collection.PropertyType;
			this._keyFrames = new List<BrushAnimationKeyFrame>(collection._keyFrames.Count);
			for (int i = 0; i < collection._keyFrames.Count; i++)
			{
				BrushAnimationKeyFrame brushAnimationKeyFrame = collection._keyFrames[i].Clone();
				this._keyFrames.Add(brushAnimationKeyFrame);
			}
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000039BC File Offset: 0x00001BBC
		public void AddKeyFrame(BrushAnimationKeyFrame keyFrame)
		{
			this._keyFrames.Add(keyFrame);
			this._keyFrames = this._keyFrames.OrderBy((BrushAnimationKeyFrame k) => k.Time).ToList<BrushAnimationKeyFrame>();
			for (int i = 0; i < this._keyFrames.Count; i++)
			{
				this._keyFrames[i].InitializeIndex(i);
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00003A32 File Offset: 0x00001C32
		public void RemoveKeyFrame(BrushAnimationKeyFrame keyFrame)
		{
			this._keyFrames.Remove(keyFrame);
		}

		// Token: 0x04000036 RID: 54
		public BrushAnimationProperty.BrushAnimationPropertyType PropertyType;

		// Token: 0x04000037 RID: 55
		private List<BrushAnimationKeyFrame> _keyFrames;

		// Token: 0x02000072 RID: 114
		public enum BrushAnimationPropertyType
		{
			// Token: 0x040003CA RID: 970
			Name,
			// Token: 0x040003CB RID: 971
			ColorFactor,
			// Token: 0x040003CC RID: 972
			Color,
			// Token: 0x040003CD RID: 973
			AlphaFactor,
			// Token: 0x040003CE RID: 974
			HueFactor,
			// Token: 0x040003CF RID: 975
			SaturationFactor,
			// Token: 0x040003D0 RID: 976
			ValueFactor,
			// Token: 0x040003D1 RID: 977
			FontColor,
			// Token: 0x040003D2 RID: 978
			OverlayXOffset,
			// Token: 0x040003D3 RID: 979
			OverlayYOffset,
			// Token: 0x040003D4 RID: 980
			TextGlowColor,
			// Token: 0x040003D5 RID: 981
			TextOutlineColor,
			// Token: 0x040003D6 RID: 982
			TextOutlineAmount,
			// Token: 0x040003D7 RID: 983
			TextGlowRadius,
			// Token: 0x040003D8 RID: 984
			TextBlur,
			// Token: 0x040003D9 RID: 985
			TextShadowOffset,
			// Token: 0x040003DA RID: 986
			TextShadowAngle,
			// Token: 0x040003DB RID: 987
			TextColorFactor,
			// Token: 0x040003DC RID: 988
			TextAlphaFactor,
			// Token: 0x040003DD RID: 989
			TextHueFactor,
			// Token: 0x040003DE RID: 990
			TextSaturationFactor,
			// Token: 0x040003DF RID: 991
			TextValueFactor,
			// Token: 0x040003E0 RID: 992
			Sprite,
			// Token: 0x040003E1 RID: 993
			IsHidden,
			// Token: 0x040003E2 RID: 994
			XOffset,
			// Token: 0x040003E3 RID: 995
			YOffset,
			// Token: 0x040003E4 RID: 996
			OverridenWidth,
			// Token: 0x040003E5 RID: 997
			OverridenHeight,
			// Token: 0x040003E6 RID: 998
			WidthPolicy,
			// Token: 0x040003E7 RID: 999
			HeightPolicy,
			// Token: 0x040003E8 RID: 1000
			HorizontalFlip,
			// Token: 0x040003E9 RID: 1001
			VerticalFlip,
			// Token: 0x040003EA RID: 1002
			OverlayMethod,
			// Token: 0x040003EB RID: 1003
			OverlaySprite,
			// Token: 0x040003EC RID: 1004
			ExtendLeft,
			// Token: 0x040003ED RID: 1005
			ExtendRight,
			// Token: 0x040003EE RID: 1006
			ExtendTop,
			// Token: 0x040003EF RID: 1007
			ExtendBottom,
			// Token: 0x040003F0 RID: 1008
			UseRandomBaseOverlayXOffset,
			// Token: 0x040003F1 RID: 1009
			UseRandomBaseOverlayYOffset,
			// Token: 0x040003F2 RID: 1010
			Font,
			// Token: 0x040003F3 RID: 1011
			FontStyle,
			// Token: 0x040003F4 RID: 1012
			FontSize
		}
	}
}

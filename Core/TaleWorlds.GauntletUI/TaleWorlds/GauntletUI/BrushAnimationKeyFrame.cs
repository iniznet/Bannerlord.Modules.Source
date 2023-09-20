using System;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000009 RID: 9
	public class BrushAnimationKeyFrame
	{
		// Token: 0x17000031 RID: 49
		// (get) Token: 0x0600008B RID: 139 RVA: 0x0000377B File Offset: 0x0000197B
		// (set) Token: 0x0600008C RID: 140 RVA: 0x00003783 File Offset: 0x00001983
		public float Time { get; private set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600008D RID: 141 RVA: 0x0000378C File Offset: 0x0000198C
		// (set) Token: 0x0600008E RID: 142 RVA: 0x00003794 File Offset: 0x00001994
		public int Index { get; private set; }

		// Token: 0x06000090 RID: 144 RVA: 0x000037A5 File Offset: 0x000019A5
		public void InitializeAsFloat(float time, float value)
		{
			this.Time = time;
			this._valueType = BrushAnimationKeyFrame.ValueType.Float;
			this._valueAsFloat = value;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000037BC File Offset: 0x000019BC
		public void InitializeAsColor(float time, Color value)
		{
			this.Time = time;
			this._valueType = BrushAnimationKeyFrame.ValueType.Color;
			this._valueAsColor = value;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000037D3 File Offset: 0x000019D3
		public void InitializeAsSprite(float time, Sprite value)
		{
			this.Time = time;
			this._valueType = BrushAnimationKeyFrame.ValueType.Sprite;
			this._valueAsSprite = value;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000037EA File Offset: 0x000019EA
		public void InitializeIndex(int index)
		{
			this.Index = index;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000037F3 File Offset: 0x000019F3
		public float GetValueAsFloat()
		{
			return this._valueAsFloat;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x000037FB File Offset: 0x000019FB
		public Color GetValueAsColor()
		{
			return this._valueAsColor;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00003803 File Offset: 0x00001A03
		public Sprite GetValueAsSprite()
		{
			return this._valueAsSprite;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x0000380C File Offset: 0x00001A0C
		public object GetValueAsObject()
		{
			switch (this._valueType)
			{
			case BrushAnimationKeyFrame.ValueType.Float:
				return this._valueAsFloat;
			case BrushAnimationKeyFrame.ValueType.Color:
				return this._valueAsColor;
			case BrushAnimationKeyFrame.ValueType.Sprite:
				return this._valueAsSprite;
			default:
				return null;
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00003854 File Offset: 0x00001A54
		public BrushAnimationKeyFrame Clone()
		{
			return new BrushAnimationKeyFrame
			{
				_valueType = this._valueType,
				_valueAsFloat = this._valueAsFloat,
				_valueAsColor = this._valueAsColor,
				_valueAsSprite = this._valueAsSprite,
				Time = this.Time,
				Index = this.Index
			};
		}

		// Token: 0x0400002F RID: 47
		private BrushAnimationKeyFrame.ValueType _valueType;

		// Token: 0x04000030 RID: 48
		private float _valueAsFloat;

		// Token: 0x04000031 RID: 49
		private Color _valueAsColor;

		// Token: 0x04000032 RID: 50
		private Sprite _valueAsSprite;

		// Token: 0x02000071 RID: 113
		public enum ValueType
		{
			// Token: 0x040003C6 RID: 966
			Float,
			// Token: 0x040003C7 RID: 967
			Color,
			// Token: 0x040003C8 RID: 968
			Sprite
		}
	}
}

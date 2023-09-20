using System;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	public class BrushAnimationKeyFrame
	{
		public float Time { get; private set; }

		public int Index { get; private set; }

		public void InitializeAsFloat(float time, float value)
		{
			this.Time = time;
			this._valueType = BrushAnimationKeyFrame.ValueType.Float;
			this._valueAsFloat = value;
		}

		public void InitializeAsColor(float time, Color value)
		{
			this.Time = time;
			this._valueType = BrushAnimationKeyFrame.ValueType.Color;
			this._valueAsColor = value;
		}

		public void InitializeAsSprite(float time, Sprite value)
		{
			this.Time = time;
			this._valueType = BrushAnimationKeyFrame.ValueType.Sprite;
			this._valueAsSprite = value;
		}

		public void InitializeIndex(int index)
		{
			this.Index = index;
		}

		public float GetValueAsFloat()
		{
			return this._valueAsFloat;
		}

		public Color GetValueAsColor()
		{
			return this._valueAsColor;
		}

		public Sprite GetValueAsSprite()
		{
			return this._valueAsSprite;
		}

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

		private BrushAnimationKeyFrame.ValueType _valueType;

		private float _valueAsFloat;

		private Color _valueAsColor;

		private Sprite _valueAsSprite;

		public enum ValueType
		{
			Float,
			Color,
			Sprite
		}
	}
}

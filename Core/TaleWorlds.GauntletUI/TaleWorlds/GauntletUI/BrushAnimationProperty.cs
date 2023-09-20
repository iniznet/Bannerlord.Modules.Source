using System;
using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.GauntletUI
{
	public class BrushAnimationProperty
	{
		public string LayerName { get; set; }

		public IEnumerable<BrushAnimationKeyFrame> KeyFrames
		{
			get
			{
				return this._keyFrames.AsReadOnly();
			}
		}

		public int Count
		{
			get
			{
				return this._keyFrames.Count;
			}
		}

		public BrushAnimationProperty()
		{
			this._keyFrames = new List<BrushAnimationKeyFrame>();
		}

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

		public BrushAnimationKeyFrame GetFrameAt(int i)
		{
			if (i >= 0 && i < this._keyFrames.Count)
			{
				return this._keyFrames[i];
			}
			return null;
		}

		public BrushAnimationProperty Clone()
		{
			BrushAnimationProperty brushAnimationProperty = new BrushAnimationProperty();
			brushAnimationProperty.FillFrom(this);
			return brushAnimationProperty;
		}

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

		public void AddKeyFrame(BrushAnimationKeyFrame keyFrame)
		{
			this._keyFrames.Add(keyFrame);
			this._keyFrames = this._keyFrames.OrderBy((BrushAnimationKeyFrame k) => k.Time).ToList<BrushAnimationKeyFrame>();
			for (int i = 0; i < this._keyFrames.Count; i++)
			{
				this._keyFrames[i].InitializeIndex(i);
			}
		}

		public void RemoveKeyFrame(BrushAnimationKeyFrame keyFrame)
		{
			this._keyFrames.Remove(keyFrame);
		}

		public BrushAnimationProperty.BrushAnimationPropertyType PropertyType;

		private List<BrushAnimationKeyFrame> _keyFrames;

		public enum BrushAnimationPropertyType
		{
			Name,
			ColorFactor,
			Color,
			AlphaFactor,
			HueFactor,
			SaturationFactor,
			ValueFactor,
			FontColor,
			OverlayXOffset,
			OverlayYOffset,
			TextGlowColor,
			TextOutlineColor,
			TextOutlineAmount,
			TextGlowRadius,
			TextBlur,
			TextShadowOffset,
			TextShadowAngle,
			TextColorFactor,
			TextAlphaFactor,
			TextHueFactor,
			TextSaturationFactor,
			TextValueFactor,
			Sprite,
			IsHidden,
			XOffset,
			YOffset,
			OverridenWidth,
			OverridenHeight,
			WidthPolicy,
			HeightPolicy,
			HorizontalFlip,
			VerticalFlip,
			OverlayMethod,
			OverlaySprite,
			ExtendLeft,
			ExtendRight,
			ExtendTop,
			ExtendBottom,
			UseRandomBaseOverlayXOffset,
			UseRandomBaseOverlayYOffset,
			Font,
			FontStyle,
			FontSize
		}
	}
}

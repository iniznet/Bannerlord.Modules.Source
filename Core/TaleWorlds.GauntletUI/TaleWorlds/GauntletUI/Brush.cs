using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	public class Brush
	{
		public Brush ClonedFrom { get; private set; }

		[Editor(false)]
		public string Name { get; set; }

		[Editor(false)]
		public float TransitionDuration { get; set; }

		public Style DefaultStyle { get; private set; }

		public Font Font
		{
			get
			{
				return this.DefaultStyle.Font;
			}
			set
			{
				this.DefaultStyle.Font = value;
			}
		}

		public FontStyle FontStyle
		{
			get
			{
				return this.DefaultStyle.FontStyle;
			}
			set
			{
				this.DefaultStyle.FontStyle = value;
			}
		}

		public int FontSize
		{
			get
			{
				return this.DefaultStyle.FontSize;
			}
			set
			{
				this.DefaultStyle.FontSize = value;
			}
		}

		[Editor(false)]
		public TextHorizontalAlignment TextHorizontalAlignment { get; set; }

		[Editor(false)]
		public TextVerticalAlignment TextVerticalAlignment { get; set; }

		[Editor(false)]
		public float GlobalColorFactor { get; set; }

		[Editor(false)]
		public float GlobalAlphaFactor { get; set; }

		[Editor(false)]
		public Color GlobalColor { get; set; }

		public SoundProperties SoundProperties { get; set; }

		public Sprite Sprite
		{
			get
			{
				return this.DefaultStyleLayer.Sprite;
			}
			set
			{
				this.DefaultStyleLayer.Sprite = value;
			}
		}

		[Editor(false)]
		public bool VerticalFlip
		{
			get
			{
				return this.DefaultStyleLayer.VerticalFlip;
			}
			set
			{
				this.DefaultStyleLayer.VerticalFlip = value;
			}
		}

		[Editor(false)]
		public bool HorizontalFlip
		{
			get
			{
				return this.DefaultStyleLayer.HorizontalFlip;
			}
			set
			{
				this.DefaultStyleLayer.HorizontalFlip = value;
			}
		}

		public Color Color
		{
			get
			{
				return this.DefaultStyleLayer.Color;
			}
			set
			{
				this.DefaultStyleLayer.Color = value;
			}
		}

		public float ColorFactor
		{
			get
			{
				return this.DefaultStyleLayer.ColorFactor;
			}
			set
			{
				this.DefaultStyleLayer.ColorFactor = value;
			}
		}

		public float AlphaFactor
		{
			get
			{
				return this.DefaultStyleLayer.AlphaFactor;
			}
			set
			{
				this.DefaultStyleLayer.AlphaFactor = value;
			}
		}

		public float HueFactor
		{
			get
			{
				return this.DefaultStyleLayer.HueFactor;
			}
			set
			{
				this.DefaultStyleLayer.HueFactor = value;
			}
		}

		public float SaturationFactor
		{
			get
			{
				return this.DefaultStyleLayer.SaturationFactor;
			}
			set
			{
				this.DefaultStyleLayer.SaturationFactor = value;
			}
		}

		public float ValueFactor
		{
			get
			{
				return this.DefaultStyleLayer.ValueFactor;
			}
			set
			{
				this.DefaultStyleLayer.ValueFactor = value;
			}
		}

		public Color FontColor
		{
			get
			{
				return this.DefaultStyle.FontColor;
			}
			set
			{
				this.DefaultStyle.FontColor = value;
			}
		}

		public float TextColorFactor
		{
			get
			{
				return this.DefaultStyle.TextColorFactor;
			}
			set
			{
				this.DefaultStyle.TextColorFactor = value;
			}
		}

		public float TextAlphaFactor
		{
			get
			{
				return this.DefaultStyle.TextAlphaFactor;
			}
			set
			{
				this.DefaultStyle.TextAlphaFactor = value;
			}
		}

		public float TextHueFactor
		{
			get
			{
				return this.DefaultStyle.TextHueFactor;
			}
			set
			{
				this.DefaultStyle.TextHueFactor = value;
			}
		}

		public float TextSaturationFactor
		{
			get
			{
				return this.DefaultStyle.TextSaturationFactor;
			}
			set
			{
				this.DefaultStyle.TextSaturationFactor = value;
			}
		}

		public float TextValueFactor
		{
			get
			{
				return this.DefaultStyle.TextValueFactor;
			}
			set
			{
				this.DefaultStyle.TextValueFactor = value;
			}
		}

		[Editor(false)]
		public Dictionary<string, BrushLayer>.ValueCollection Layers
		{
			get
			{
				return this._layers.Values;
			}
		}

		public StyleLayer DefaultStyleLayer
		{
			get
			{
				return this.DefaultStyle.DefaultLayer;
			}
		}

		public BrushLayer DefaultLayer
		{
			get
			{
				return this._layers["Default"];
			}
		}

		public Brush()
		{
			this._styles = new Dictionary<string, Style>();
			this._layers = new Dictionary<string, BrushLayer>();
			this._brushAnimations = new Dictionary<string, BrushAnimation>();
			this.SoundProperties = new SoundProperties();
			this.TextHorizontalAlignment = TextHorizontalAlignment.Center;
			this.TextVerticalAlignment = TextVerticalAlignment.Center;
			BrushLayer brushLayer = new BrushLayer();
			brushLayer.Name = "Default";
			this._layers.Add(brushLayer.Name, brushLayer);
			this.DefaultStyle = new Style(new List<BrushLayer> { brushLayer });
			this.DefaultStyle.Name = "Default";
			this.DefaultStyle.SetAsDefaultStyle();
			this.AddStyle(this.DefaultStyle);
			this.ClonedFrom = null;
			this.TransitionDuration = 0.05f;
			this.GlobalColorFactor = 1f;
			this.GlobalAlphaFactor = 1f;
			this.GlobalColor = Color.White;
		}

		public Style GetStyle(string name)
		{
			Style style;
			this._styles.TryGetValue(name, out style);
			return style;
		}

		[Editor(false)]
		public Dictionary<string, Style>.ValueCollection Styles
		{
			get
			{
				return this._styles.Values;
			}
		}

		public Style GetStyleOrDefault(string name)
		{
			Style style;
			this._styles.TryGetValue(name, out style);
			return style ?? this.DefaultStyle;
		}

		public void AddStyle(Style style)
		{
			string name = style.Name;
			this._styles.Add(name, style);
		}

		public void RemoveStyle(string styleName)
		{
			this._styles.Remove(styleName);
		}

		public void AddLayer(BrushLayer layer)
		{
			this._layers.Add(layer.Name, layer);
			foreach (Style style in this.Styles)
			{
				style.AddLayer(new StyleLayer(layer));
			}
		}

		public void RemoveLayer(string layerName)
		{
			this._layers.Remove(layerName);
			foreach (Style style in this.Styles)
			{
				style.RemoveLayer(layerName);
			}
		}

		public BrushLayer GetLayer(string name)
		{
			BrushLayer brushLayer;
			if (this._layers.TryGetValue(name, out brushLayer))
			{
				return brushLayer;
			}
			return null;
		}

		public void FillFrom(Brush brush)
		{
			this.Name = brush.Name;
			this.TransitionDuration = brush.TransitionDuration;
			this.TextVerticalAlignment = brush.TextVerticalAlignment;
			this.TextHorizontalAlignment = brush.TextHorizontalAlignment;
			this.GlobalColorFactor = brush.GlobalColorFactor;
			this.GlobalAlphaFactor = brush.GlobalAlphaFactor;
			this.GlobalColor = brush.GlobalColor;
			this._layers = new Dictionary<string, BrushLayer>();
			foreach (BrushLayer brushLayer in brush._layers.Values)
			{
				BrushLayer brushLayer2 = new BrushLayer();
				brushLayer2.FillFrom(brushLayer);
				this._layers.Add(brushLayer2.Name, brushLayer2);
			}
			this._styles = new Dictionary<string, Style>();
			Style style = brush._styles["Default"];
			Style style2 = new Style(this._layers.Values);
			style2.SetAsDefaultStyle();
			style2.FillFrom(style);
			this._styles.Add(style2.Name, style2);
			this.DefaultStyle = style2;
			foreach (Style style3 in brush._styles.Values)
			{
				if (style3.Name != "Default")
				{
					Style style4 = new Style(this._layers.Values);
					style4.DefaultStyle = this.DefaultStyle;
					style4.FillFrom(style3);
					this._styles.Add(style4.Name, style4);
				}
			}
			this._brushAnimations = new Dictionary<string, BrushAnimation>();
			foreach (BrushAnimation brushAnimation in brush._brushAnimations.Values)
			{
				BrushAnimation brushAnimation2 = new BrushAnimation();
				brushAnimation2.FillFrom(brushAnimation);
				this._brushAnimations.Add(brushAnimation2.Name, brushAnimation2);
			}
			this.SoundProperties = new SoundProperties();
			this.SoundProperties.FillFrom(brush.SoundProperties);
		}

		public Brush Clone()
		{
			Brush brush = new Brush();
			brush.FillFrom(this);
			brush.Name = this.Name + "(Clone)";
			brush.ClonedFrom = this;
			return brush;
		}

		public void AddAnimation(BrushAnimation animation)
		{
			this._brushAnimations.Add(animation.Name, animation);
		}

		public BrushAnimation GetAnimation(string name)
		{
			BrushAnimation brushAnimation;
			if (name != null && this._brushAnimations.TryGetValue(name, out brushAnimation))
			{
				return brushAnimation;
			}
			return null;
		}

		public IEnumerable<BrushAnimation> GetAnimations()
		{
			return this._brushAnimations.Values;
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.Name))
			{
				return base.ToString();
			}
			return this.Name;
		}

		public bool IsCloneRelated(Brush brush)
		{
			return this.ClonedFrom == brush || brush.ClonedFrom == this || brush.ClonedFrom == this.ClonedFrom;
		}

		private const float DefaultTransitionDuration = 0.05f;

		private Dictionary<string, Style> _styles;

		private Dictionary<string, BrushLayer> _layers;

		private Dictionary<string, BrushAnimation> _brushAnimations;
	}
}

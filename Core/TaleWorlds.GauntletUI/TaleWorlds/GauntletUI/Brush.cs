using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000007 RID: 7
	public class Brush
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00002D41 File Offset: 0x00000F41
		// (set) Token: 0x06000033 RID: 51 RVA: 0x00002D49 File Offset: 0x00000F49
		public Brush ClonedFrom { get; private set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00002D52 File Offset: 0x00000F52
		// (set) Token: 0x06000035 RID: 53 RVA: 0x00002D5A File Offset: 0x00000F5A
		[Editor(false)]
		public string Name { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00002D63 File Offset: 0x00000F63
		// (set) Token: 0x06000037 RID: 55 RVA: 0x00002D6B File Offset: 0x00000F6B
		[Editor(false)]
		public float TransitionDuration { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00002D74 File Offset: 0x00000F74
		// (set) Token: 0x06000039 RID: 57 RVA: 0x00002D7C File Offset: 0x00000F7C
		public Style DefaultStyle { get; private set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00002D85 File Offset: 0x00000F85
		// (set) Token: 0x0600003B RID: 59 RVA: 0x00002D92 File Offset: 0x00000F92
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

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00002DA0 File Offset: 0x00000FA0
		// (set) Token: 0x0600003D RID: 61 RVA: 0x00002DAD File Offset: 0x00000FAD
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

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600003E RID: 62 RVA: 0x00002DBB File Offset: 0x00000FBB
		// (set) Token: 0x0600003F RID: 63 RVA: 0x00002DC8 File Offset: 0x00000FC8
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

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000040 RID: 64 RVA: 0x00002DD6 File Offset: 0x00000FD6
		// (set) Token: 0x06000041 RID: 65 RVA: 0x00002DDE File Offset: 0x00000FDE
		[Editor(false)]
		public TextHorizontalAlignment TextHorizontalAlignment { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000042 RID: 66 RVA: 0x00002DE7 File Offset: 0x00000FE7
		// (set) Token: 0x06000043 RID: 67 RVA: 0x00002DEF File Offset: 0x00000FEF
		[Editor(false)]
		public TextVerticalAlignment TextVerticalAlignment { get; set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00002DF8 File Offset: 0x00000FF8
		// (set) Token: 0x06000045 RID: 69 RVA: 0x00002E00 File Offset: 0x00001000
		[Editor(false)]
		public float GlobalColorFactor { get; set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000046 RID: 70 RVA: 0x00002E09 File Offset: 0x00001009
		// (set) Token: 0x06000047 RID: 71 RVA: 0x00002E11 File Offset: 0x00001011
		[Editor(false)]
		public float GlobalAlphaFactor { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000048 RID: 72 RVA: 0x00002E1A File Offset: 0x0000101A
		// (set) Token: 0x06000049 RID: 73 RVA: 0x00002E22 File Offset: 0x00001022
		[Editor(false)]
		public Color GlobalColor { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600004A RID: 74 RVA: 0x00002E2B File Offset: 0x0000102B
		// (set) Token: 0x0600004B RID: 75 RVA: 0x00002E33 File Offset: 0x00001033
		public SoundProperties SoundProperties { get; set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600004C RID: 76 RVA: 0x00002E3C File Offset: 0x0000103C
		// (set) Token: 0x0600004D RID: 77 RVA: 0x00002E49 File Offset: 0x00001049
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

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600004E RID: 78 RVA: 0x00002E57 File Offset: 0x00001057
		// (set) Token: 0x0600004F RID: 79 RVA: 0x00002E64 File Offset: 0x00001064
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

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000050 RID: 80 RVA: 0x00002E72 File Offset: 0x00001072
		// (set) Token: 0x06000051 RID: 81 RVA: 0x00002E7F File Offset: 0x0000107F
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

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000052 RID: 82 RVA: 0x00002E8D File Offset: 0x0000108D
		// (set) Token: 0x06000053 RID: 83 RVA: 0x00002E9A File Offset: 0x0000109A
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

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000054 RID: 84 RVA: 0x00002EA8 File Offset: 0x000010A8
		// (set) Token: 0x06000055 RID: 85 RVA: 0x00002EB5 File Offset: 0x000010B5
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

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000056 RID: 86 RVA: 0x00002EC3 File Offset: 0x000010C3
		// (set) Token: 0x06000057 RID: 87 RVA: 0x00002ED0 File Offset: 0x000010D0
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

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000058 RID: 88 RVA: 0x00002EDE File Offset: 0x000010DE
		// (set) Token: 0x06000059 RID: 89 RVA: 0x00002EEB File Offset: 0x000010EB
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

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00002EF9 File Offset: 0x000010F9
		// (set) Token: 0x0600005B RID: 91 RVA: 0x00002F06 File Offset: 0x00001106
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

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00002F14 File Offset: 0x00001114
		// (set) Token: 0x0600005D RID: 93 RVA: 0x00002F21 File Offset: 0x00001121
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

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00002F2F File Offset: 0x0000112F
		// (set) Token: 0x0600005F RID: 95 RVA: 0x00002F3C File Offset: 0x0000113C
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

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00002F4A File Offset: 0x0000114A
		// (set) Token: 0x06000061 RID: 97 RVA: 0x00002F57 File Offset: 0x00001157
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

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00002F65 File Offset: 0x00001165
		// (set) Token: 0x06000063 RID: 99 RVA: 0x00002F72 File Offset: 0x00001172
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

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00002F80 File Offset: 0x00001180
		// (set) Token: 0x06000065 RID: 101 RVA: 0x00002F8D File Offset: 0x0000118D
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

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00002F9B File Offset: 0x0000119B
		// (set) Token: 0x06000067 RID: 103 RVA: 0x00002FA8 File Offset: 0x000011A8
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

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00002FB6 File Offset: 0x000011B6
		// (set) Token: 0x06000069 RID: 105 RVA: 0x00002FC3 File Offset: 0x000011C3
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

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600006A RID: 106 RVA: 0x00002FD1 File Offset: 0x000011D1
		[Editor(false)]
		public Dictionary<string, BrushLayer>.ValueCollection Layers
		{
			get
			{
				return this._layers.Values;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600006B RID: 107 RVA: 0x00002FDE File Offset: 0x000011DE
		public StyleLayer DefaultStyleLayer
		{
			get
			{
				return this.DefaultStyle.DefaultLayer;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600006C RID: 108 RVA: 0x00002FEB File Offset: 0x000011EB
		public BrushLayer DefaultLayer
		{
			get
			{
				return this._layers["Default"];
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00003000 File Offset: 0x00001200
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

		// Token: 0x0600006E RID: 110 RVA: 0x000030E4 File Offset: 0x000012E4
		public Style GetStyle(string name)
		{
			Style style;
			this._styles.TryGetValue(name, out style);
			return style;
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600006F RID: 111 RVA: 0x00003101 File Offset: 0x00001301
		[Editor(false)]
		public Dictionary<string, Style>.ValueCollection Styles
		{
			get
			{
				return this._styles.Values;
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00003110 File Offset: 0x00001310
		public Style GetStyleOrDefault(string name)
		{
			Style style;
			this._styles.TryGetValue(name, out style);
			return style ?? this.DefaultStyle;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00003138 File Offset: 0x00001338
		public void AddStyle(Style style)
		{
			string name = style.Name;
			this._styles.Add(name, style);
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00003159 File Offset: 0x00001359
		public void RemoveStyle(string styleName)
		{
			this._styles.Remove(styleName);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00003168 File Offset: 0x00001368
		public void AddLayer(BrushLayer layer)
		{
			this._layers.Add(layer.Name, layer);
			foreach (Style style in this.Styles)
			{
				style.AddLayer(new StyleLayer(layer));
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000031D0 File Offset: 0x000013D0
		public void RemoveLayer(string layerName)
		{
			this._layers.Remove(layerName);
			foreach (Style style in this.Styles)
			{
				style.RemoveLayer(layerName);
			}
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00003230 File Offset: 0x00001430
		public BrushLayer GetLayer(string name)
		{
			BrushLayer brushLayer;
			if (this._layers.TryGetValue(name, out brushLayer))
			{
				return brushLayer;
			}
			return null;
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00003250 File Offset: 0x00001450
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

		// Token: 0x06000077 RID: 119 RVA: 0x00003494 File Offset: 0x00001694
		public Brush Clone()
		{
			Brush brush = new Brush();
			brush.FillFrom(this);
			brush.Name = this.Name + "(Clone)";
			brush.ClonedFrom = this;
			return brush;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000034BF File Offset: 0x000016BF
		public void AddAnimation(BrushAnimation animation)
		{
			this._brushAnimations.Add(animation.Name, animation);
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000034D4 File Offset: 0x000016D4
		public BrushAnimation GetAnimation(string name)
		{
			BrushAnimation brushAnimation;
			if (name != null && this._brushAnimations.TryGetValue(name, out brushAnimation))
			{
				return brushAnimation;
			}
			return null;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000034F7 File Offset: 0x000016F7
		public IEnumerable<BrushAnimation> GetAnimations()
		{
			return this._brushAnimations.Values;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00003504 File Offset: 0x00001704
		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.Name))
			{
				return base.ToString();
			}
			return this.Name;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00003520 File Offset: 0x00001720
		public bool IsCloneRelated(Brush brush)
		{
			return this.ClonedFrom == brush || brush.ClonedFrom == this || brush.ClonedFrom == this.ClonedFrom;
		}

		// Token: 0x0400001F RID: 31
		private const float DefaultTransitionDuration = 0.05f;

		// Token: 0x04000027 RID: 39
		private Dictionary<string, Style> _styles;

		// Token: 0x04000028 RID: 40
		private Dictionary<string, BrushLayer> _layers;

		// Token: 0x04000029 RID: 41
		private Dictionary<string, BrushAnimation> _brushAnimations;
	}
}

﻿using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	// Token: 0x0200001C RID: 28
	public class TooltipProperty : ViewModel, ISerializableObject
	{
		// Token: 0x1700006F RID: 111
		// (get) Token: 0x0600015A RID: 346 RVA: 0x00004C4A File Offset: 0x00002E4A
		// (set) Token: 0x0600015B RID: 347 RVA: 0x00004C52 File Offset: 0x00002E52
		public bool OnlyShowWhenExtended { get; set; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x0600015C RID: 348 RVA: 0x00004C5B File Offset: 0x00002E5B
		// (set) Token: 0x0600015D RID: 349 RVA: 0x00004C63 File Offset: 0x00002E63
		public bool OnlyShowWhenNotExtended { get; set; }

		// Token: 0x0600015E RID: 350 RVA: 0x00004C6C File Offset: 0x00002E6C
		public TooltipProperty()
		{
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00004C94 File Offset: 0x00002E94
		public void RefreshValue()
		{
			if (this.valueFunc != null)
			{
				string text = this.valueFunc();
				if (text != null)
				{
					this.ValueLabel = text;
				}
			}
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00004CBF File Offset: 0x00002EBF
		public void RefreshDefinition()
		{
			if (this.definitionFunc != null)
			{
				this.DefinitionLabel = this.definitionFunc();
			}
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00004CDC File Offset: 0x00002EDC
		public TooltipProperty(string definition, string value, int textHeight, bool onlyShowWhenExtended = false, TooltipProperty.TooltipPropertyFlags modifier = TooltipProperty.TooltipPropertyFlags.None)
		{
			this.TextHeight = textHeight;
			this.DefinitionLabel = definition;
			this.ValueLabel = value;
			this.OnlyShowWhenExtended = onlyShowWhenExtended;
			this.PropertyModifier = (int)modifier;
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00004D34 File Offset: 0x00002F34
		public TooltipProperty(string definition, Func<string> _valueFunc, int textHeight, bool onlyShowWhenExtended = false, TooltipProperty.TooltipPropertyFlags modifier = TooltipProperty.TooltipPropertyFlags.None)
		{
			this.valueFunc = _valueFunc;
			this.TextHeight = textHeight;
			this.DefinitionLabel = definition;
			this.OnlyShowWhenExtended = onlyShowWhenExtended;
			this.PropertyModifier = (int)modifier;
			this.RefreshValue();
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00004D94 File Offset: 0x00002F94
		public TooltipProperty(Func<string> _definitionFunc, Func<string> _valueFunc, int textHeight, bool onlyShowWhenExtended = false, TooltipProperty.TooltipPropertyFlags modifier = TooltipProperty.TooltipPropertyFlags.None)
		{
			this.valueFunc = _valueFunc;
			this.TextHeight = textHeight;
			this.definitionFunc = _definitionFunc;
			this.OnlyShowWhenExtended = onlyShowWhenExtended;
			this.PropertyModifier = (int)modifier;
			this.RefreshDefinition();
			this.RefreshValue();
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00004DF8 File Offset: 0x00002FF8
		public TooltipProperty(Func<string> _definitionFunc, Func<string> _valueFunc, object[] valueArgs, int textHeight, bool onlyShowWhenExtended = false, TooltipProperty.TooltipPropertyFlags modifier = TooltipProperty.TooltipPropertyFlags.None)
		{
			this.valueFunc = _valueFunc;
			this.TextHeight = textHeight;
			this.definitionFunc = _definitionFunc;
			this.OnlyShowWhenExtended = onlyShowWhenExtended;
			this.PropertyModifier = (int)modifier;
			this.RefreshDefinition();
			this.RefreshValue();
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00004E5C File Offset: 0x0000305C
		public TooltipProperty(string definition, string value, int textHeight, Color color, bool onlyShowWhenExtended = false, TooltipProperty.TooltipPropertyFlags modifier = TooltipProperty.TooltipPropertyFlags.None)
		{
			this.TextHeight = textHeight;
			this.TextColor = color;
			this.DefinitionLabel = definition;
			this.ValueLabel = value;
			this.OnlyShowWhenExtended = onlyShowWhenExtended;
			this.PropertyModifier = (int)modifier;
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00004EBC File Offset: 0x000030BC
		public TooltipProperty(string definition, Func<string> _valueFunc, int textHeight, Color color, bool onlyShowWhenExtended = false, TooltipProperty.TooltipPropertyFlags modifier = TooltipProperty.TooltipPropertyFlags.None)
		{
			this.valueFunc = _valueFunc;
			this.TextHeight = textHeight;
			this.TextColor = color;
			this.DefinitionLabel = definition;
			this.OnlyShowWhenExtended = onlyShowWhenExtended;
			this.PropertyModifier = (int)modifier;
			this.RefreshValue();
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00004F24 File Offset: 0x00003124
		public TooltipProperty(Func<string> _definitionFunc, Func<string> _valueFunc, int textHeight, Color color, bool onlyShowWhenExtended = false, TooltipProperty.TooltipPropertyFlags modifier = TooltipProperty.TooltipPropertyFlags.None)
		{
			this.valueFunc = _valueFunc;
			this.definitionFunc = _definitionFunc;
			this.TextHeight = textHeight;
			this.TextColor = color;
			this.OnlyShowWhenExtended = onlyShowWhenExtended;
			this.PropertyModifier = (int)modifier;
			this.RefreshDefinition();
			this.RefreshValue();
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00004F90 File Offset: 0x00003190
		public TooltipProperty(TooltipProperty property)
		{
			this.TextHeight = property.TextHeight;
			this.TextColor = property.TextColor;
			this.DefinitionLabel = property.DefinitionLabel;
			this.ValueLabel = property.ValueLabel;
			this.OnlyShowWhenExtended = property.OnlyShowWhenExtended;
			this.PropertyModifier = property.PropertyModifier;
		}

		// Token: 0x06000169 RID: 361 RVA: 0x0000500A File Offset: 0x0000320A
		public void DeserializeFrom(IReader reader)
		{
			this.TextHeight = reader.ReadInt();
			this.TextColor = reader.ReadColor();
			this.DefinitionLabel = reader.ReadString();
			this.ValueLabel = reader.ReadString();
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000503C File Offset: 0x0000323C
		public void SerializeTo(IWriter writer)
		{
			writer.WriteInt(this.TextHeight);
			writer.WriteColor(this.TextColor);
			writer.WriteString(this.DefinitionLabel);
			writer.WriteString(this.ValueLabel);
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x0600016B RID: 363 RVA: 0x0000506E File Offset: 0x0000326E
		// (set) Token: 0x0600016C RID: 364 RVA: 0x00005076 File Offset: 0x00003276
		public int TextHeight
		{
			get
			{
				return this._textHeight;
			}
			set
			{
				if (value != this._textHeight)
				{
					this._textHeight = value;
					base.OnPropertyChangedWithValue(value, "TextHeight");
				}
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600016D RID: 365 RVA: 0x00005094 File Offset: 0x00003294
		// (set) Token: 0x0600016E RID: 366 RVA: 0x0000509C File Offset: 0x0000329C
		public Color TextColor
		{
			get
			{
				return this._textColor;
			}
			set
			{
				if (value != this._textColor)
				{
					this._textColor = value;
					base.OnPropertyChangedWithValue(value, "TextColor");
				}
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x0600016F RID: 367 RVA: 0x000050BF File Offset: 0x000032BF
		// (set) Token: 0x06000170 RID: 368 RVA: 0x000050C7 File Offset: 0x000032C7
		public string DefinitionLabel
		{
			get
			{
				return this._definitionLabel;
			}
			set
			{
				if (value != this._definitionLabel)
				{
					this._definitionLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "DefinitionLabel");
				}
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000171 RID: 369 RVA: 0x000050EA File Offset: 0x000032EA
		// (set) Token: 0x06000172 RID: 370 RVA: 0x000050F2 File Offset: 0x000032F2
		public string ValueLabel
		{
			get
			{
				return this._valueLabel;
			}
			set
			{
				if (value != this._valueLabel)
				{
					this._valueLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "ValueLabel");
				}
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000173 RID: 371 RVA: 0x00005115 File Offset: 0x00003315
		// (set) Token: 0x06000174 RID: 372 RVA: 0x0000511D File Offset: 0x0000331D
		public int PropertyModifier
		{
			get
			{
				return this._propertyModifier;
			}
			set
			{
				if (value != this._propertyModifier)
				{
					this._propertyModifier = value;
					base.OnPropertyChangedWithValue(value, "PropertyModifier");
				}
			}
		}

		// Token: 0x0400008C RID: 140
		private Func<string> valueFunc;

		// Token: 0x0400008D RID: 141
		private Func<string> definitionFunc;

		// Token: 0x0400008E RID: 142
		private string _definitionLabel;

		// Token: 0x0400008F RID: 143
		private string _valueLabel;

		// Token: 0x04000090 RID: 144
		private Color _textColor = new Color(0f, 0f, 0f, 0f);

		// Token: 0x04000091 RID: 145
		private int _textHeight;

		// Token: 0x04000092 RID: 146
		private int _propertyModifier;

		// Token: 0x0200002C RID: 44
		[Flags]
		public enum TooltipPropertyFlags
		{
			// Token: 0x040000C9 RID: 201
			None = 0,
			// Token: 0x040000CA RID: 202
			MultiLine = 1,
			// Token: 0x040000CB RID: 203
			BattleMode = 2,
			// Token: 0x040000CC RID: 204
			BattleModeOver = 4,
			// Token: 0x040000CD RID: 205
			WarFirstEnemy = 8,
			// Token: 0x040000CE RID: 206
			WarFirstAlly = 16,
			// Token: 0x040000CF RID: 207
			WarFirstNeutral = 32,
			// Token: 0x040000D0 RID: 208
			WarSecondEnemy = 64,
			// Token: 0x040000D1 RID: 209
			WarSecondAlly = 128,
			// Token: 0x040000D2 RID: 210
			WarSecondNeutral = 256,
			// Token: 0x040000D3 RID: 211
			RundownSeperator = 512,
			// Token: 0x040000D4 RID: 212
			DefaultSeperator = 1024,
			// Token: 0x040000D5 RID: 213
			Cost = 2048,
			// Token: 0x040000D6 RID: 214
			Title = 4096,
			// Token: 0x040000D7 RID: 215
			RundownResult = 8192
		}
	}
}

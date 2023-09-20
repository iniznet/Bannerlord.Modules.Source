using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	public class TooltipProperty : ViewModel, ISerializableObject
	{
		public bool OnlyShowWhenExtended { get; set; }

		public bool OnlyShowWhenNotExtended { get; set; }

		public TooltipProperty()
		{
		}

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

		public void RefreshDefinition()
		{
			if (this.definitionFunc != null)
			{
				this.DefinitionLabel = this.definitionFunc();
			}
		}

		public TooltipProperty(string definition, string value, int textHeight, bool onlyShowWhenExtended = false, TooltipProperty.TooltipPropertyFlags modifier = TooltipProperty.TooltipPropertyFlags.None)
		{
			this.TextHeight = textHeight;
			this.DefinitionLabel = definition;
			this.ValueLabel = value;
			this.OnlyShowWhenExtended = onlyShowWhenExtended;
			this.PropertyModifier = (int)modifier;
		}

		public TooltipProperty(string definition, Func<string> _valueFunc, int textHeight, bool onlyShowWhenExtended = false, TooltipProperty.TooltipPropertyFlags modifier = TooltipProperty.TooltipPropertyFlags.None)
		{
			this.valueFunc = _valueFunc;
			this.TextHeight = textHeight;
			this.DefinitionLabel = definition;
			this.OnlyShowWhenExtended = onlyShowWhenExtended;
			this.PropertyModifier = (int)modifier;
			this.RefreshValue();
		}

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

		public TooltipProperty(string definition, string value, int textHeight, Color color, bool onlyShowWhenExtended = false, TooltipProperty.TooltipPropertyFlags modifier = TooltipProperty.TooltipPropertyFlags.None)
		{
			this.TextHeight = textHeight;
			this.TextColor = color;
			this.DefinitionLabel = definition;
			this.ValueLabel = value;
			this.OnlyShowWhenExtended = onlyShowWhenExtended;
			this.PropertyModifier = (int)modifier;
		}

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

		public TooltipProperty(TooltipProperty property)
		{
			this.TextHeight = property.TextHeight;
			this.TextColor = property.TextColor;
			this.DefinitionLabel = property.DefinitionLabel;
			this.ValueLabel = property.ValueLabel;
			this.OnlyShowWhenExtended = property.OnlyShowWhenExtended;
			this.PropertyModifier = property.PropertyModifier;
		}

		public void DeserializeFrom(IReader reader)
		{
			this.TextHeight = reader.ReadInt();
			this.TextColor = reader.ReadColor();
			this.DefinitionLabel = reader.ReadString();
			this.ValueLabel = reader.ReadString();
		}

		public void SerializeTo(IWriter writer)
		{
			writer.WriteInt(this.TextHeight);
			writer.WriteColor(this.TextColor);
			writer.WriteString(this.DefinitionLabel);
			writer.WriteString(this.ValueLabel);
		}

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

		private Func<string> valueFunc;

		private Func<string> definitionFunc;

		private string _definitionLabel;

		private string _valueLabel;

		private Color _textColor = new Color(0f, 0f, 0f, 0f);

		private int _textHeight;

		private int _propertyModifier;

		[Flags]
		public enum TooltipPropertyFlags
		{
			None = 0,
			MultiLine = 1,
			BattleMode = 2,
			BattleModeOver = 4,
			WarFirstEnemy = 8,
			WarFirstAlly = 16,
			WarFirstNeutral = 32,
			WarSecondEnemy = 64,
			WarSecondAlly = 128,
			WarSecondNeutral = 256,
			RundownSeperator = 512,
			DefaultSeperator = 1024,
			Cost = 2048,
			Title = 4096,
			RundownResult = 8192
		}
	}
}

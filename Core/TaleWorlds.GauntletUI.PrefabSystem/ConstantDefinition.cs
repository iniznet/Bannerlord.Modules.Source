using System;
using System.Collections.Generic;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	public class ConstantDefinition
	{
		public string Name { get; private set; }

		public string Value { get; set; }

		public string SpriteName { get; set; }

		public string BrushName { get; set; }

		public string LayerName { get; set; }

		public string Additive { get; set; }

		public string Prefix { get; set; }

		public string Suffix { get; set; }

		public float MultiplyResult { get; set; }

		public string OnTrueValue { get; set; }

		public string OnFalseValue { get; set; }

		public ConstantDefinitionType Type { get; set; }

		public ConstantDefinition(string name)
		{
			this.Name = name;
			this.MultiplyResult = 1f;
		}

		public string GetValue(BrushFactory brushFactory, SpriteData spriteData, Dictionary<string, ConstantDefinition> constants, Dictionary<string, WidgetAttributeTemplate> parameters, Dictionary<string, string> defaultParameters)
		{
			string text = "";
			if (this.Type == ConstantDefinitionType.Constant)
			{
				string text2 = ConstantDefinition.GetActualValueOf(this.Value, brushFactory, spriteData, constants, parameters, defaultParameters);
				if (!string.IsNullOrEmpty(this.Additive))
				{
					int num = Convert.ToInt32(ConstantDefinition.GetActualValueOf(this.Additive, brushFactory, spriteData, constants, parameters, defaultParameters));
					text2 = (Convert.ToInt32(decimal.Parse(text2)) + num).ToString();
				}
				text = text2;
			}
			else if (this.Type == ConstantDefinitionType.BooleanCheck)
			{
				if (ConstantDefinition.GetActualValueOf(this.Value, brushFactory, spriteData, constants, parameters, defaultParameters) == "true")
				{
					text = ConstantDefinition.GetActualValueOf(this.OnTrueValue, brushFactory, spriteData, constants, parameters, defaultParameters);
				}
				else
				{
					text = ConstantDefinition.GetActualValueOf(this.OnFalseValue, brushFactory, spriteData, constants, parameters, defaultParameters);
				}
			}
			else if (this.Type == ConstantDefinitionType.BrushLayerWidth || this.Type == ConstantDefinitionType.BrushLayerHeight)
			{
				string actualValueOf = ConstantDefinition.GetActualValueOf(this.BrushName, brushFactory, spriteData, constants, parameters, defaultParameters);
				Brush brush = brushFactory.GetBrush(actualValueOf);
				if (brush != null)
				{
					BrushLayer layer = brush.GetLayer(this.LayerName);
					if (layer != null)
					{
						Sprite sprite = layer.Sprite;
						if (sprite != null)
						{
							int num2 = 0;
							if (!string.IsNullOrEmpty(this.Additive))
							{
								num2 = Convert.ToInt32(ConstantDefinition.GetActualValueOf(this.Additive, brushFactory, spriteData, constants, parameters, defaultParameters));
							}
							if (this.Type == ConstantDefinitionType.BrushLayerWidth)
							{
								float extendLeft = layer.ExtendLeft;
								float extendRight = layer.ExtendRight;
								text = ((int)((float)sprite.Width - extendLeft - extendRight + (float)num2)).ToString();
							}
							else if (this.Type == ConstantDefinitionType.BrushLayerHeight)
							{
								float extendTop = layer.ExtendTop;
								float extendBottom = layer.ExtendBottom;
								text = ((int)((float)sprite.Height - extendTop - extendBottom + (float)num2)).ToString();
							}
						}
					}
				}
			}
			else if (this.Type == ConstantDefinitionType.SpriteWidth || this.Type == ConstantDefinitionType.SpriteHeight)
			{
				Sprite sprite2 = spriteData.GetSprite(this.SpriteName);
				if (sprite2 != null)
				{
					int num3 = 0;
					if (!string.IsNullOrEmpty(this.Additive))
					{
						num3 = Convert.ToInt32(ConstantDefinition.GetActualValueOf(this.Additive, brushFactory, spriteData, constants, parameters, defaultParameters));
					}
					if (this.Type == ConstantDefinitionType.SpriteWidth)
					{
						text = (sprite2.Width + num3).ToString();
					}
					else if (this.Type == ConstantDefinitionType.SpriteHeight)
					{
						text = (sprite2.Height + num3).ToString();
					}
				}
			}
			if (this.MultiplyResult != 1f)
			{
				text = ((float)Convert.ToInt32(text) * this.MultiplyResult).ToString();
			}
			if (!string.IsNullOrEmpty(this.Prefix))
			{
				text = this.Prefix + text;
			}
			if (!string.IsNullOrEmpty(this.Suffix))
			{
				text += this.Suffix;
			}
			return text;
		}

		public static string GetActualValueOf(string value, BrushFactory brushFactory, SpriteData spriteData, Dictionary<string, ConstantDefinition> constants, Dictionary<string, WidgetAttributeTemplate> parameters, Dictionary<string, string> defaultParameters)
		{
			string text = "";
			if (value.StartsWith("!"))
			{
				string text2 = value.Substring("!".Length);
				if (constants.ContainsKey(text2))
				{
					text = constants[text2].GetValue(brushFactory, spriteData, constants, parameters, defaultParameters);
				}
			}
			else if (value.StartsWith("*"))
			{
				string text3 = value.Substring("*".Length);
				if (parameters.ContainsKey(text3))
				{
					text = parameters[text3].Value;
				}
				else if (defaultParameters != null && defaultParameters.ContainsKey(text3))
				{
					text = defaultParameters[text3];
				}
			}
			else
			{
				text = value;
			}
			return text;
		}
	}
}

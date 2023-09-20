using System;
using System.Collections.Generic;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	public class VisualStateTemplate
	{
		public string State { get; set; }

		public VisualStateTemplate()
		{
			this._attributes = new Dictionary<string, string>();
		}

		public void SetAttribute(string name, string value)
		{
			if (this._attributes.ContainsKey(name))
			{
				this._attributes[name] = value;
				return;
			}
			this._attributes.Add(name, value);
		}

		public Dictionary<string, string> GetAttributes()
		{
			return this._attributes;
		}

		public void ClearAttribute(string name)
		{
			if (this._attributes.ContainsKey(name))
			{
				this._attributes.Remove(name);
			}
		}

		public VisualState CreateVisualState(BrushFactory brushFactory, SpriteData spriteData, Dictionary<string, VisualDefinitionTemplate> visualDefinitionTemplates, Dictionary<string, ConstantDefinition> constants, Dictionary<string, WidgetAttributeTemplate> parameters, Dictionary<string, string> defaultParameters)
		{
			VisualState visualState = new VisualState(this.State);
			foreach (KeyValuePair<string, string> keyValuePair in this._attributes)
			{
				string key = keyValuePair.Key;
				string actualValueOf = ConstantDefinition.GetActualValueOf(keyValuePair.Value, brushFactory, spriteData, constants, parameters, defaultParameters);
				WidgetExtensions.SetWidgetAttributeFromString(visualState, key, actualValueOf, brushFactory, spriteData, visualDefinitionTemplates, constants, parameters, null, defaultParameters);
			}
			return visualState;
		}

		private Dictionary<string, string> _attributes;
	}
}

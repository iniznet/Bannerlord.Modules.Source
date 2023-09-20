using System;

namespace TaleWorlds.GauntletUI.CodeGenerator
{
	public class ParameterGenerationContext
	{
		public string Name { get; private set; }

		public string Value { get; private set; }

		public ParameterGenerationContext(string name, string value)
		{
			this.Name = name;
			this.Value = value;
		}
	}
}

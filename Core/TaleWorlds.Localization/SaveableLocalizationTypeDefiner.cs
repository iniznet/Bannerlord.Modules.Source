using System;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Localization
{
	public class SaveableLocalizationTypeDefiner : SaveableTypeDefiner
	{
		public SaveableLocalizationTypeDefiner()
			: base(20000)
		{
		}

		protected override void DefineClassTypes()
		{
			base.AddClassDefinition(typeof(TextObject), 1, null);
		}

		protected override void DefineContainerDefinitions()
		{
			base.ConstructContainerDefinition(typeof(Dictionary<string, TextObject>));
		}
	}
}

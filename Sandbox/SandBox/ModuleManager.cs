using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine;

namespace SandBox
{
	public class ModuleManager : IModuleManager
	{
		public string[] ModuleNames
		{
			get
			{
				return Utilities.GetModulesNames();
			}
		}
	}
}

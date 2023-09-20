using System;
using System.Collections.Generic;
using TaleWorlds.Engine.Options;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Options
{
	public class OptionGroup
	{
		public OptionGroup(TextObject groupName, IEnumerable<IOptionData> options)
		{
			this.GroupName = groupName;
			this.Options = options;
		}

		public readonly TextObject GroupName;

		public readonly IEnumerable<IOptionData> Options;
	}
}

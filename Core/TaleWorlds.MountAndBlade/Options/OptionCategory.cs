using System;
using System.Collections.Generic;
using TaleWorlds.Engine.Options;

namespace TaleWorlds.MountAndBlade.Options
{
	public class OptionCategory
	{
		public OptionCategory(IEnumerable<IOptionData> baseOptions, IEnumerable<OptionGroup> groups)
		{
			this.BaseOptions = baseOptions;
			this.Groups = groups;
		}

		public readonly IEnumerable<IOptionData> BaseOptions;

		public readonly IEnumerable<OptionGroup> Groups;
	}
}

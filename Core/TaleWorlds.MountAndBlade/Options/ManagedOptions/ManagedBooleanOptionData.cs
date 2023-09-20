using System;
using TaleWorlds.Engine.Options;

namespace TaleWorlds.MountAndBlade.Options.ManagedOptions
{
	public class ManagedBooleanOptionData : ManagedOptionData, IBooleanOptionData, IOptionData
	{
		public ManagedBooleanOptionData(ManagedOptions.ManagedOptionsType type)
			: base(type)
		{
		}
	}
}

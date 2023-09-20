using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Load
{
	internal class ElementLoadData : VariableLoadData
	{
		public ContainerLoadData ContainerLoadData { get; private set; }

		internal ElementLoadData(ContainerLoadData containerLoadData, IReader reader)
			: base(containerLoadData.Context, reader)
		{
			this.ContainerLoadData = containerLoadData;
		}
	}
}

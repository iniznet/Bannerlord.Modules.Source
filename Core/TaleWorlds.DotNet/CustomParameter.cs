using System;

namespace TaleWorlds.DotNet
{
	internal class CustomParameter<T> : DotNetObject
	{
		public T Target { get; set; }

		public CustomParameter(T target)
		{
			this.Target = target;
		}
	}
}

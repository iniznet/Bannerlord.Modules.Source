using System;

namespace TaleWorlds.Diamond
{
	public sealed class InnerProcessConnectionInformation : IConnectionInformation
	{
		string IConnectionInformation.GetAddress()
		{
			return "InnerProcess";
		}
	}
}

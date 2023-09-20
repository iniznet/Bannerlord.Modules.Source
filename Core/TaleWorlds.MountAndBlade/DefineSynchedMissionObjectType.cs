using System;

namespace TaleWorlds.MountAndBlade
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	internal sealed class DefineSynchedMissionObjectType : Attribute
	{
		public DefineSynchedMissionObjectType(Type type)
		{
			this.Type = type;
		}

		public readonly Type Type;
	}
}

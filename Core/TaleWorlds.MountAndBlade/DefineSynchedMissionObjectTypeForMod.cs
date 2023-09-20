using System;

namespace TaleWorlds.MountAndBlade
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public sealed class DefineSynchedMissionObjectTypeForMod : Attribute
	{
		public DefineSynchedMissionObjectTypeForMod(Type type)
		{
			this.Type = type;
		}

		public readonly Type Type;
	}
}

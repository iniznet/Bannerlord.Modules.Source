using System;

namespace TaleWorlds.MountAndBlade
{
	[AttributeUsage(AttributeTargets.Field)]
	public class MultiplayerOptionsProperty : Attribute
	{
		public bool HasBounds
		{
			get
			{
				return this.BoundsMax > this.BoundsMin;
			}
		}

		public MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType optionValueType, MultiplayerOptionsProperty.ReplicationOccurrence replicationOccurrence, string description = null, int boundsMin = 0, int boundsMax = 0, string[] validGameModes = null, bool hasMultipleSelections = false, Type enumType = null)
		{
			this.OptionValueType = optionValueType;
			this.Replication = replicationOccurrence;
			this.Description = description;
			this.BoundsMin = boundsMin;
			this.BoundsMax = boundsMax;
			this.ValidGameModes = validGameModes;
			this.HasMultipleSelections = hasMultipleSelections;
			this.EnumType = enumType;
		}

		public readonly MultiplayerOptions.OptionValueType OptionValueType;

		public readonly MultiplayerOptionsProperty.ReplicationOccurrence Replication;

		public readonly string Description;

		public readonly int BoundsMin;

		public readonly int BoundsMax;

		public readonly string[] ValidGameModes;

		public readonly bool HasMultipleSelections;

		public readonly Type EnumType;

		public enum ReplicationOccurrence
		{
			Never,
			AtMapLoad,
			Immediately
		}
	}
}

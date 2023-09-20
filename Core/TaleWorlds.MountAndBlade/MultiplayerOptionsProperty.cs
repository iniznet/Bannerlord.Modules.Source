using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200032C RID: 812
	[AttributeUsage(AttributeTargets.Field)]
	public class MultiplayerOptionsProperty : Attribute
	{
		// Token: 0x170007E7 RID: 2023
		// (get) Token: 0x06002BF1 RID: 11249 RVA: 0x000AA886 File Offset: 0x000A8A86
		public bool HasBounds
		{
			get
			{
				return this.BoundsMax > this.BoundsMin;
			}
		}

		// Token: 0x06002BF2 RID: 11250 RVA: 0x000AA898 File Offset: 0x000A8A98
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

		// Token: 0x0400109F RID: 4255
		public readonly MultiplayerOptions.OptionValueType OptionValueType;

		// Token: 0x040010A0 RID: 4256
		public readonly MultiplayerOptionsProperty.ReplicationOccurrence Replication;

		// Token: 0x040010A1 RID: 4257
		public readonly string Description;

		// Token: 0x040010A2 RID: 4258
		public readonly int BoundsMin;

		// Token: 0x040010A3 RID: 4259
		public readonly int BoundsMax;

		// Token: 0x040010A4 RID: 4260
		public readonly string[] ValidGameModes;

		// Token: 0x040010A5 RID: 4261
		public readonly bool HasMultipleSelections;

		// Token: 0x040010A6 RID: 4262
		public readonly Type EnumType;

		// Token: 0x02000645 RID: 1605
		public enum ReplicationOccurrence
		{
			// Token: 0x04002051 RID: 8273
			Never,
			// Token: 0x04002052 RID: 8274
			AtMapLoad,
			// Token: 0x04002053 RID: 8275
			Immediately
		}
	}
}

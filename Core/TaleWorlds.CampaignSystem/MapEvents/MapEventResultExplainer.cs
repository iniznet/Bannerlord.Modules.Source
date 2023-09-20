using System;

namespace TaleWorlds.CampaignSystem.MapEvents
{
	public class MapEventResultExplainer
	{
		public ExplainedNumber InfluenceExplainedNumber = new ExplainedNumber(0f, true, null);

		public ExplainedNumber RenownExplainedNumber = new ExplainedNumber(0f, true, null);

		public ExplainedNumber GoldExplainedNumber = new ExplainedNumber(0f, true, null);

		public ExplainedNumber MoraleExplainedNumber = new ExplainedNumber(0f, true, null);

		public ExplainedNumber PlunderedGoldExplainedNumber = new ExplainedNumber(0f, true, null);
	}
}

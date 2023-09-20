using System;

namespace TaleWorlds.CampaignSystem.Encyclopedia
{
	public struct EncyclopediaListItem
	{
		public EncyclopediaListItem(object obj, string name, string description, string id, string typeName, bool playerCanSeeValues, Action onShowTooltip = null)
		{
			this.Object = obj;
			this.Name = name;
			this.Description = description;
			this.Id = id;
			this.TypeName = typeName;
			this.PlayerCanSeeValues = playerCanSeeValues;
			this.OnShowTooltip = onShowTooltip;
		}

		public readonly object Object;

		public readonly string Name;

		public readonly string Description;

		public readonly string Id;

		public readonly string TypeName;

		public readonly bool PlayerCanSeeValues;

		public readonly Action OnShowTooltip;
	}
}

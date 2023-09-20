using System;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages
{
	public struct EncyclopediaPageArgs
	{
		public EncyclopediaPageArgs(object o)
		{
			this.Obj = o;
		}

		public readonly object Obj;
	}
}

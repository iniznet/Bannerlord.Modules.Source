using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Issues.IssueQuestTasks
{
	// Token: 0x02000328 RID: 808
	public class RaidVillageQuestTask : QuestTaskBase
	{
		// Token: 0x06002DD6 RID: 11734 RVA: 0x000BF756 File Offset: 0x000BD956
		public RaidVillageQuestTask(Village village, Action onSucceededAction, Action onFailedAction, Action onCanceledAction, DialogFlow dialogFlow = null)
			: base(dialogFlow, onSucceededAction, onFailedAction, onCanceledAction)
		{
			this._targetVillage = village;
		}

		// Token: 0x06002DD7 RID: 11735 RVA: 0x000BF76B File Offset: 0x000BD96B
		public void OnVillageLooted(Village village)
		{
			if (this._targetVillage == village)
			{
				base.Finish((this._targetVillage.Owner.MapEvent.AttackerSide.LeaderParty == MobileParty.MainParty.Party) ? QuestTaskBase.FinishStates.Success : QuestTaskBase.FinishStates.Fail);
			}
		}

		// Token: 0x06002DD8 RID: 11736 RVA: 0x000BF7A6 File Offset: 0x000BD9A6
		public void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification)
		{
			if (!FactionManager.IsAtWarAgainstFaction(newKingdom, this._targetVillage.Settlement.MapFaction))
			{
				base.Finish(QuestTaskBase.FinishStates.Cancel);
			}
		}

		// Token: 0x06002DD9 RID: 11737 RVA: 0x000BF7C7 File Offset: 0x000BD9C7
		public override void SetReferences()
		{
			CampaignEvents.VillageLooted.AddNonSerializedListener(this, new Action<Village>(this.OnVillageLooted));
			CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
		}

		// Token: 0x04000DD2 RID: 3538
		[SaveableField(50)]
		private readonly Village _targetVillage;
	}
}

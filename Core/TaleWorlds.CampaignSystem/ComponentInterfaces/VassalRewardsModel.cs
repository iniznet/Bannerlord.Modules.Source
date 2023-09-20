﻿using System;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class VassalRewardsModel : GameModel
	{
		public abstract float InfluenceReward { get; }

		public abstract int RelationRewardWithLeader { get; }

		public abstract TroopRoster GetTroopRewardsForJoiningKingdom(Kingdom kingdom);

		public abstract ItemRoster GetEquipmentRewardsForJoiningKingdom(Kingdom kingdom);
	}
}

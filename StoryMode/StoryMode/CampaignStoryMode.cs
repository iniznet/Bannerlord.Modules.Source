﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace StoryMode
{
	// Token: 0x02000004 RID: 4
	public class CampaignStoryMode : Campaign
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002058 File Offset: 0x00000258
		// (set) Token: 0x06000004 RID: 4 RVA: 0x00002060 File Offset: 0x00000260
		[SaveableProperty(9999)]
		public StoryModeManager StoryMode { get; private set; }

		// Token: 0x06000005 RID: 5 RVA: 0x00002069 File Offset: 0x00000269
		public CampaignStoryMode(CampaignGameMode gameMode)
			: base(gameMode)
		{
			this.StoryMode = new StoryModeManager();
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000207D File Offset: 0x0000027D
		protected override void BeforeRegisterTypes(MBObjectManager objectManager)
		{
			base.BeforeRegisterTypes(objectManager);
			objectManager.RegisterType<TrainingField>("TrainingField", "TrainingFields", 1U, true, false);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000209C File Offset: 0x0000029C
		protected override void DoLoadingForGameType(GameTypeLoadingStates gameTypeLoadingState, out GameTypeLoadingStates nextState)
		{
			base.DoLoadingForGameType(gameTypeLoadingState, ref nextState);
			if (gameTypeLoadingState == null && nextState != gameTypeLoadingState)
			{
				this.StoryMode.InitializeStoryModeObjects();
			}
			if (gameTypeLoadingState == 2 && nextState != gameTypeLoadingState)
			{
				Settlement settlement = Settlement.Find("tutorial_training_field");
				if (settlement != null)
				{
					base.MapSceneWrapper.AddNewEntityToMapScene(settlement.StringId, settlement.Position2D);
				}
			}
			if (gameTypeLoadingState == 3)
			{
				this.OnSessionLaunched();
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000020FC File Offset: 0x000002FC
		private void OnSessionLaunched()
		{
			this.StoryMode.MainStoryLine.OnSessionLaunched();
		}

		// Token: 0x06000009 RID: 9 RVA: 0x0000210E File Offset: 0x0000030E
		internal static void AutoGeneratedStaticCollectObjectsCampaignStoryMode(object o, List<object> collectedObjects)
		{
			((CampaignStoryMode)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000211C File Offset: 0x0000031C
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.StoryMode);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002131 File Offset: 0x00000331
		internal static object AutoGeneratedGetMemberValueStoryMode(object o)
		{
			return ((CampaignStoryMode)o).StoryMode;
		}
	}
}

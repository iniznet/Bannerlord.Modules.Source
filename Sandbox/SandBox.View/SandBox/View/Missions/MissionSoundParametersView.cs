using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions
{
	// Token: 0x0200001E RID: 30
	[DefaultView]
	public class MissionSoundParametersView : MissionView
	{
		// Token: 0x060000B0 RID: 176 RVA: 0x00009A3B File Offset: 0x00007C3B
		public override void EarlyStart()
		{
			base.EarlyStart();
			this.InitializeGlobalParameters();
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00009A49 File Offset: 0x00007C49
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			SoundManager.SetGlobalParameter("MissionCulture", 0f);
			SoundManager.SetGlobalParameter("MissionProsperity", 0f);
			SoundManager.SetGlobalParameter("MissionCombatMode", 0f);
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00009A7E File Offset: 0x00007C7E
		public override void OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
		{
			this.InitializeCombatModeParameter();
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00009A86 File Offset: 0x00007C86
		private void InitializeGlobalParameters()
		{
			this.InitializeCultureParameter();
			this.InitializeProsperityParameter();
			this.InitializeCombatModeParameter();
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00009A9C File Offset: 0x00007C9C
		private void InitializeCultureParameter()
		{
			MissionSoundParametersView.SoundParameterMissionCulture soundParameterMissionCulture = MissionSoundParametersView.SoundParameterMissionCulture.None;
			if (Campaign.Current != null)
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				if (currentSettlement != null)
				{
					if (currentSettlement.IsHideout)
					{
						soundParameterMissionCulture = MissionSoundParametersView.SoundParameterMissionCulture.Bandit;
					}
					else
					{
						string stringId = currentSettlement.Culture.StringId;
						if (!(stringId == "empire"))
						{
							if (!(stringId == "sturgia"))
							{
								if (!(stringId == "aserai"))
								{
									if (!(stringId == "vlandia"))
									{
										if (!(stringId == "battania"))
										{
											if (stringId == "khuzait")
											{
												soundParameterMissionCulture = MissionSoundParametersView.SoundParameterMissionCulture.Khuzait;
											}
										}
										else
										{
											soundParameterMissionCulture = MissionSoundParametersView.SoundParameterMissionCulture.Battania;
										}
									}
									else
									{
										soundParameterMissionCulture = MissionSoundParametersView.SoundParameterMissionCulture.Vlandia;
									}
								}
								else
								{
									soundParameterMissionCulture = MissionSoundParametersView.SoundParameterMissionCulture.Aserai;
								}
							}
							else
							{
								soundParameterMissionCulture = MissionSoundParametersView.SoundParameterMissionCulture.Sturgia;
							}
						}
						else
						{
							soundParameterMissionCulture = MissionSoundParametersView.SoundParameterMissionCulture.Empire;
						}
					}
				}
			}
			SoundManager.SetGlobalParameter("MissionCulture", (float)soundParameterMissionCulture);
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00009B48 File Offset: 0x00007D48
		private void InitializeProsperityParameter()
		{
			MissionSoundParametersView.SoundParameterMissionProsperityLevel soundParameterMissionProsperityLevel = MissionSoundParametersView.SoundParameterMissionProsperityLevel.None;
			if (Campaign.Current != null && Settlement.CurrentSettlement != null)
			{
				switch (Settlement.CurrentSettlement.SettlementComponent.GetProsperityLevel())
				{
				case 0:
					soundParameterMissionProsperityLevel = MissionSoundParametersView.SoundParameterMissionProsperityLevel.None;
					break;
				case 1:
					soundParameterMissionProsperityLevel = MissionSoundParametersView.SoundParameterMissionProsperityLevel.Mid;
					break;
				case 2:
					soundParameterMissionProsperityLevel = MissionSoundParametersView.SoundParameterMissionProsperityLevel.High;
					break;
				}
			}
			SoundManager.SetGlobalParameter("MissionProsperity", (float)soundParameterMissionProsperityLevel);
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00009BA0 File Offset: 0x00007DA0
		private void InitializeCombatModeParameter()
		{
			bool flag = base.Mission.Mode == 2 || base.Mission.Mode == 3 || base.Mission.Mode == 7;
			SoundManager.SetGlobalParameter("MissionCombatMode", (float)(flag ? 1 : 0));
		}

		// Token: 0x04000077 RID: 119
		private const string CultureParameterId = "MissionCulture";

		// Token: 0x04000078 RID: 120
		private const string ProsperityParameterId = "MissionProsperity";

		// Token: 0x04000079 RID: 121
		private const string CombatParameterId = "MissionCombatMode";

		// Token: 0x02000069 RID: 105
		public enum SoundParameterMissionCulture : short
		{
			// Token: 0x0400025B RID: 603
			None,
			// Token: 0x0400025C RID: 604
			Aserai,
			// Token: 0x0400025D RID: 605
			Battania,
			// Token: 0x0400025E RID: 606
			Empire,
			// Token: 0x0400025F RID: 607
			Khuzait,
			// Token: 0x04000260 RID: 608
			Sturgia,
			// Token: 0x04000261 RID: 609
			Vlandia,
			// Token: 0x04000262 RID: 610
			Bandit
		}

		// Token: 0x0200006A RID: 106
		private enum SoundParameterMissionProsperityLevel : short
		{
			// Token: 0x04000264 RID: 612
			None,
			// Token: 0x04000265 RID: 613
			Low = 0,
			// Token: 0x04000266 RID: 614
			Mid,
			// Token: 0x04000267 RID: 615
			High
		}
	}
}

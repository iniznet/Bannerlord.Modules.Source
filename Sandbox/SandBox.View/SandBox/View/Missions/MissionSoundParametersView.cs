using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions
{
	[DefaultView]
	public class MissionSoundParametersView : MissionView
	{
		public override void EarlyStart()
		{
			base.EarlyStart();
			this.InitializeGlobalParameters();
		}

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			SoundManager.SetGlobalParameter("MissionCulture", 0f);
			SoundManager.SetGlobalParameter("MissionProsperity", 0f);
			SoundManager.SetGlobalParameter("MissionCombatMode", 0f);
		}

		public override void OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
		{
			this.InitializeCombatModeParameter();
		}

		private void InitializeGlobalParameters()
		{
			this.InitializeCultureParameter();
			this.InitializeProsperityParameter();
			this.InitializeCombatModeParameter();
		}

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

		private void InitializeCombatModeParameter()
		{
			bool flag = base.Mission.Mode == 2 || base.Mission.Mode == 3 || base.Mission.Mode == 7;
			SoundManager.SetGlobalParameter("MissionCombatMode", (float)(flag ? 1 : 0));
		}

		private const string CultureParameterId = "MissionCulture";

		private const string ProsperityParameterId = "MissionProsperity";

		private const string CombatParameterId = "MissionCombatMode";

		public enum SoundParameterMissionCulture : short
		{
			None,
			Aserai,
			Battania,
			Empire,
			Khuzait,
			Sturgia,
			Vlandia,
			Bandit
		}

		private enum SoundParameterMissionProsperityLevel : short
		{
			None,
			Low = 0,
			Mid,
			High
		}
	}
}

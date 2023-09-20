using System;
using psai.net;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.View
{
	// Token: 0x02000006 RID: 6
	public class CampaignMusicHandler : IMusicHandler
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000018 RID: 24 RVA: 0x00003116 File Offset: 0x00001316
		bool IMusicHandler.IsPausable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00003119 File Offset: 0x00001319
		private CampaignMusicHandler()
		{
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00003124 File Offset: 0x00001324
		public static void Create()
		{
			CampaignMusicHandler campaignMusicHandler = new CampaignMusicHandler();
			MBMusicManager.Current.OnCampaignMusicHandlerInit(campaignMusicHandler);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00003142 File Offset: 0x00001342
		void IMusicHandler.OnUpdated(float dt)
		{
			this.CheckMusicMode();
			this.TickCampaignMusic(dt);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00003151 File Offset: 0x00001351
		private void CheckMusicMode()
		{
			if (MBMusicManager.Current.CurrentMode == null)
			{
				MBMusicManager.Current.ActivateCampaignMode();
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x0000316C File Offset: 0x0000136C
		private void TickCampaignMusic(float dt)
		{
			bool flag = PsaiCore.Instance.GetPsaiInfo().psaiState == 2;
			if (this._restTimer <= 0f)
			{
				this._restTimer += dt;
				if (this._restTimer > 0f)
				{
					MBMusicManager.Current.StartThemeWithConstantIntensity(MBMusicManager.Current.GetCampaignMusicTheme(this.GetNearbyCulture(), this.GetMoodOfMainParty() < MusicParameters.CampaignDarkModeThreshold, this.IsPlayerInAnArmy()), false);
					Debug.Print("Campaign music play started.", 0, 9, 64UL);
					return;
				}
			}
			else if (!flag)
			{
				MBMusicManager.Current.ForceStopThemeWithFadeOut();
				this._restTimer = -(30f + MBRandom.RandomFloat * 90f);
				Debug.Print("Campaign music rest started.", 0, 9, 64UL);
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x0000322C File Offset: 0x0000142C
		private CultureCode GetNearbyCulture()
		{
			CultureObject cultureObject = null;
			float num = float.MaxValue;
			foreach (Settlement settlement in Campaign.Current.Settlements)
			{
				if (settlement.IsTown || settlement.IsVillage)
				{
					float num2 = settlement.Position2D.DistanceSquared(PartyBase.MainParty.Position2D);
					if (settlement.IsVillage)
					{
						num2 *= 1.05f;
					}
					if (num > num2)
					{
						cultureObject = settlement.Culture;
						num = num2;
					}
				}
			}
			return cultureObject.GetCultureCode();
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000032D8 File Offset: 0x000014D8
		private bool IsPlayerInAnArmy()
		{
			return MobileParty.MainParty.Army != null;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000032E7 File Offset: 0x000014E7
		private float GetMoodOfMainParty()
		{
			return MathF.Clamp(MobileParty.MainParty.Morale / 100f, 0f, 1f);
		}

		// Token: 0x0400000D RID: 13
		private const float MinRestDurationInSeconds = 30f;

		// Token: 0x0400000E RID: 14
		private const float MaxRestDurationInSeconds = 120f;

		// Token: 0x0400000F RID: 15
		private float _restTimer;
	}
}

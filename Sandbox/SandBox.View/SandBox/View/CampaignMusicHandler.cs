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
	public class CampaignMusicHandler : IMusicHandler
	{
		bool IMusicHandler.IsPausable
		{
			get
			{
				return false;
			}
		}

		private CampaignMusicHandler()
		{
		}

		public static void Create()
		{
			CampaignMusicHandler campaignMusicHandler = new CampaignMusicHandler();
			MBMusicManager.Current.OnCampaignMusicHandlerInit(campaignMusicHandler);
		}

		void IMusicHandler.OnUpdated(float dt)
		{
			this.CheckMusicMode();
			this.TickCampaignMusic(dt);
		}

		private void CheckMusicMode()
		{
			if (MBMusicManager.Current.CurrentMode == null)
			{
				MBMusicManager.Current.ActivateCampaignMode();
			}
		}

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

		private bool IsPlayerInAnArmy()
		{
			return MobileParty.MainParty.Army != null;
		}

		private float GetMoodOfMainParty()
		{
			return MathF.Clamp(MobileParty.MainParty.Morale / 100f, 0f, 1f);
		}

		private const float MinRestDurationInSeconds = 30f;

		private const float MaxRestDurationInSeconds = 120f;

		private float _restTimer;
	}
}

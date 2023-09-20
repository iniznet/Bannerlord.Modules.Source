using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components;

namespace SandBox.View.Missions.Sound.Components
{
	// Token: 0x02000029 RID: 41
	public class MusicMissionAlleyFightComponent : MusicMissionActionComponent
	{
		// Token: 0x06000155 RID: 341 RVA: 0x00010DE1 File Offset: 0x0000EFE1
		public MusicMissionAlleyFightComponent()
		{
			this.TrackUpdateInterval = 1f;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00010DF4 File Offset: 0x0000EFF4
		public override void PreInitialize()
		{
			base.PreInitialize();
			base.ActionTracks.Add(Tuple.Create<MBMusicManagerOld.MusicMood, string>(12, "small"));
			this.CurrentMood = base.SelectNewActionTrack();
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00010E1F File Offset: 0x0000F01F
		protected override float CalculateIntensityFromDamageFromPlayer()
		{
			return MathF.Pow(base.CalculateIntensityFromDamageFromPlayer(), 0.75f);
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00010E31 File Offset: 0x0000F031
		protected override float CalculateIntensityFromDamageToPlayer()
		{
			return MathF.Pow(base.CalculateIntensityFromDamageToPlayer(), 0.75f);
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00010E43 File Offset: 0x0000F043
		protected override float CalculateIntensityFromEnemiesInDuelRange()
		{
			return MathF.Pow(base.CalculateIntensityFromEnemiesInDuelRange(), 0.55f);
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00010E55 File Offset: 0x0000F055
		protected override float CalculateIntensityFromEnemiesAround()
		{
			return MathF.Pow(base.CalculateIntensityFromEnemiesAround(), 1.5f);
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00010E67 File Offset: 0x0000F067
		public override bool IsActive()
		{
			return Mission.Current != null && Mission.Current.Mode == 2;
		}
	}
}

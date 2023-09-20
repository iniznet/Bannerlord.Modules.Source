using System;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Sound
{
	// Token: 0x02000059 RID: 89
	public class MusicSilencedMissionView : MissionView, IMusicHandler
	{
		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060003DD RID: 989 RVA: 0x0002057B File Offset: 0x0001E77B
		bool IMusicHandler.IsPausable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060003DE RID: 990 RVA: 0x0002057E File Offset: 0x0001E77E
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			MBMusicManager.Current.DeactivateCurrentMode();
			MBMusicManager.Current.OnSilencedMusicHandlerInit(this);
		}

		// Token: 0x060003DF RID: 991 RVA: 0x0002059B File Offset: 0x0001E79B
		public override void OnMissionScreenFinalize()
		{
			MBMusicManager.Current.OnSilencedMusicHandlerFinalize();
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x000205A7 File Offset: 0x0001E7A7
		void IMusicHandler.OnUpdated(float dt)
		{
		}
	}
}

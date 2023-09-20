using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer
{
	// Token: 0x0200008B RID: 139
	public class MissionMultiplayerPreloadView : MissionView
	{
		// Token: 0x060004E5 RID: 1253 RVA: 0x00025C38 File Offset: 0x00023E38
		public override void OnPreMissionTick(float dt)
		{
			if (!this._preloadDone)
			{
				MissionMultiplayerGameModeBaseClient missionBehavior = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
				IEnumerable<MultiplayerClassDivisions.MPHeroClass> mpheroClasses = MultiplayerClassDivisions.GetMPHeroClasses(MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptionsExtensions.GetStrValue(14, 0)));
				IEnumerable<MultiplayerClassDivisions.MPHeroClass> mpheroClasses2 = MultiplayerClassDivisions.GetMPHeroClasses(MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptionsExtensions.GetStrValue(15, 0)));
				List<BasicCharacterObject> list = new List<BasicCharacterObject>();
				foreach (MultiplayerClassDivisions.MPHeroClass mpheroClass in mpheroClasses)
				{
					list.Add(mpheroClass.HeroCharacter);
					if (missionBehavior.GameType == 5)
					{
						list.Add(mpheroClass.TroopCharacter);
					}
				}
				foreach (MultiplayerClassDivisions.MPHeroClass mpheroClass2 in mpheroClasses2)
				{
					list.Add(mpheroClass2.HeroCharacter);
					if (missionBehavior.GameType == 5)
					{
						list.Add(mpheroClass2.TroopCharacter);
					}
				}
				this._helperInstance.PreloadCharacters(list);
				MissionMultiplayerSiegeClient missionBehavior2 = Mission.Current.GetMissionBehavior<MissionMultiplayerSiegeClient>();
				if (missionBehavior2 != null)
				{
					this._helperInstance.PreloadItems(missionBehavior2.GetSiegeMissiles());
				}
				this._preloadDone = true;
			}
		}

		// Token: 0x060004E6 RID: 1254 RVA: 0x00025D78 File Offset: 0x00023F78
		public override void OnSceneRenderingStarted()
		{
			this._helperInstance.WaitForMeshesToBeLoaded();
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x00025D85 File Offset: 0x00023F85
		public override void OnMissionStateDeactivated()
		{
			base.OnMissionStateDeactivated();
			this._helperInstance.Clear();
		}

		// Token: 0x04000301 RID: 769
		private PreloadHelper _helperInstance = new PreloadHelper();

		// Token: 0x04000302 RID: 770
		private bool _preloadDone;
	}
}

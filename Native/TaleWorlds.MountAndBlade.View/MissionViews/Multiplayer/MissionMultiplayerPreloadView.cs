using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer
{
	public class MissionMultiplayerPreloadView : MissionView
	{
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

		public override void OnSceneRenderingStarted()
		{
			this._helperInstance.WaitForMeshesToBeLoaded();
		}

		public override void OnMissionStateDeactivated()
		{
			base.OnMissionStateDeactivated();
			this._helperInstance.Clear();
		}

		private PreloadHelper _helperInstance = new PreloadHelper();

		private bool _preloadDone;
	}
}

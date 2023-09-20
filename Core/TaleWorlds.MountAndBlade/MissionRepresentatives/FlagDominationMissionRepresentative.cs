using System;
using System.Collections.Generic;
using NetworkMessages.FromServer;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.MissionRepresentatives
{
	// Token: 0x020003E4 RID: 996
	public class FlagDominationMissionRepresentative : MissionRepresentativeBase
	{
		// Token: 0x17000935 RID: 2357
		// (get) Token: 0x0600347D RID: 13437 RVA: 0x000D95E5 File Offset: 0x000D77E5
		private bool Forfeited
		{
			get
			{
				return base.Gold < 0;
			}
		}

		// Token: 0x0600347E RID: 13438 RVA: 0x000D95F0 File Offset: 0x000D77F0
		public int GetGoldAmountForVisual()
		{
			if (base.Gold < 0)
			{
				return 80;
			}
			return base.Gold;
		}

		// Token: 0x0600347F RID: 13439 RVA: 0x000D9604 File Offset: 0x000D7804
		public void UpdateSelectedClassServer(Agent agent)
		{
			this._survivedLastRound = agent != null;
		}

		// Token: 0x06003480 RID: 13440 RVA: 0x000D9610 File Offset: 0x000D7810
		public bool CheckIfSurvivedLastRoundAndReset()
		{
			bool survivedLastRound = this._survivedLastRound;
			this._survivedLastRound = false;
			return survivedLastRound;
		}

		// Token: 0x06003481 RID: 13441 RVA: 0x000D9620 File Offset: 0x000D7820
		public int GetGoldGainsFromKillData(MPPerkObject.MPPerkHandler killerPerkHandler, MPPerkObject.MPPerkHandler assistingHitterPerkHandler, MultiplayerClassDivisions.MPHeroClass victimClass, bool isAssist, bool isFriendly)
		{
			if (isFriendly || this.Forfeited)
			{
				return 0;
			}
			int num;
			if (isAssist)
			{
				num = ((killerPerkHandler != null) ? killerPerkHandler.GetRewardedGoldOnAssist() : 0) + ((assistingHitterPerkHandler != null) ? assistingHitterPerkHandler.GetGoldOnAssist() : 0);
			}
			else
			{
				int num2 = ((base.ControlledAgent != null) ? MultiplayerClassDivisions.GetMPHeroClassForCharacter(base.ControlledAgent.Character).TroopBattleCost : 0);
				num = ((killerPerkHandler != null) ? killerPerkHandler.GetGoldOnKill((float)num2, (float)victimClass.TroopBattleCost) : 0);
			}
			if (num > 0)
			{
				GameNetwork.BeginModuleEventAsServer(base.Peer);
				GameNetwork.WriteMessage(new GoldGain(new List<KeyValuePair<ushort, int>>
				{
					new KeyValuePair<ushort, int>(2048, num)
				}));
				GameNetwork.EndModuleEventAsServer();
			}
			return num;
		}

		// Token: 0x06003482 RID: 13442 RVA: 0x000D96CC File Offset: 0x000D78CC
		public int GetGoldGainFromKillDataAndUpdateFlags(MultiplayerClassDivisions.MPHeroClass victimClass, bool isAssist)
		{
			int num = 0;
			int num2 = 50;
			List<KeyValuePair<ushort, int>> list = new List<KeyValuePair<ushort, int>>();
			if (base.ControlledAgent != null)
			{
				num2 += victimClass.TroopBattleCost - MultiplayerClassDivisions.GetMPHeroClassForCharacter(base.ControlledAgent.Character).TroopBattleCost / 2;
			}
			if (isAssist)
			{
				int num3 = MathF.Max(5, num2 / 10);
				num += num3;
				list.Add(new KeyValuePair<ushort, int>(256, num3));
			}
			else if (base.ControlledAgent != null)
			{
				int num4 = MathF.Max(10, num2 / 5);
				num += num4;
				list.Add(new KeyValuePair<ushort, int>(128, num4));
			}
			if (list.Count > 0 && !base.Peer.Communicator.IsServerPeer && base.Peer.Communicator.IsConnectionActive)
			{
				GameNetwork.BeginModuleEventAsServer(base.Peer);
				GameNetwork.WriteMessage(new GoldGain(list));
				GameNetwork.EndModuleEventAsServer();
			}
			return num;
		}

		// Token: 0x06003483 RID: 13443 RVA: 0x000D97A8 File Offset: 0x000D79A8
		public int GetGoldGainsFromAllyDeathReward(int baseAmount)
		{
			if (this.Forfeited)
			{
				return 0;
			}
			if (baseAmount > 0 && !base.Peer.Communicator.IsServerPeer && base.Peer.Communicator.IsConnectionActive)
			{
				GameNetwork.BeginModuleEventAsServer(base.Peer);
				GameNetwork.WriteMessage(new GoldGain(new List<KeyValuePair<ushort, int>>
				{
					new KeyValuePair<ushort, int>(2048, baseAmount)
				}));
				GameNetwork.EndModuleEventAsServer();
			}
			return baseAmount;
		}

		// Token: 0x0400165B RID: 5723
		private bool _survivedLastRound;
	}
}

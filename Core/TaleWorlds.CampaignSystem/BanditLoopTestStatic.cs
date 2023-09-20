using System;
using Helpers;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x020000A3 RID: 163
	public static class BanditLoopTestStatic
	{
		// Token: 0x06001183 RID: 4483 RVA: 0x00050418 File Offset: 0x0004E618
		public static void Init()
		{
			BanditLoopTestStatic.IsBanditLoopTest = true;
			BanditLoopTestStatic.BanditLoopSettlement = MBObjectManager.Instance.GetObject<Settlement>("town_ES7");
			BanditLoopTestStatic.BanditLoopPlayerParty = MobileParty.MainParty;
			BanditLoopTestStatic.BanditLoopPlayerParty.MemberRoster.RemoveIf((TroopRosterElement item) => item.Character != CharacterObject.PlayerCharacter);
			BanditLoopTestStatic.BanditLoopPlayerParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_recruit"), 16, false, 0, 0, true, -1);
			BanditLoopTestStatic.BanditLoopPlayerParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_archer"), 8, false, 0, 0, true, -1);
			BanditLoopTestStatic.BanditLoopPlayerParty.Position2D = new Vec2(559f, 432.4f);
			int num = 0;
			Clan clan = Campaign.Current.CampaignObjectManager.Find<Clan>("looters");
			foreach (MobileParty mobileParty in MobileParty.All)
			{
				if (mobileParty.IsBandit && mobileParty.MapFaction == clan)
				{
					MobileParty mobileParty2 = mobileParty;
					if (num == 0)
					{
						Vec2 vec = new Vec2(BanditLoopTestStatic.BanditLoopPlayerParty.Position2D.x + 0.75f, BanditLoopTestStatic.BanditLoopPlayerParty.Position2D.y - 0.75f);
						BanditLoopTestStatic.BanditLoopBandit1Party = mobileParty2;
						BanditLoopTestStatic.BanditLoopBandit1Party.MemberRoster.Clear();
						BanditLoopTestStatic.BanditLoopBandit1Party.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("looter"), 14, false, 0, 0, true, -1);
						BanditLoopTestStatic.BanditLoopBandit1Party.Position2D = vec;
						num++;
					}
					else if (num == 1)
					{
						Vec2 vec2 = new Vec2(BanditLoopTestStatic.BanditLoopPlayerParty.Position2D.x - 0.75f, BanditLoopTestStatic.BanditLoopPlayerParty.Position2D.y - 0.75f);
						BanditLoopTestStatic.BanditLoopBandit2Party = mobileParty2;
						BanditLoopTestStatic.BanditLoopBandit2Party.MemberRoster.Clear();
						BanditLoopTestStatic.BanditLoopBandit2Party.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("looter"), 18, false, 0, 0, true, -1);
						BanditLoopTestStatic.BanditLoopBandit2Party.Position2D = vec2;
						break;
					}
				}
			}
			Settlement @object = MBObjectManager.Instance.GetObject<Settlement>("village_V7_2");
			foreach (MobileParty mobileParty3 in MobileParty.All)
			{
				if (mobileParty3.IsBandit && mobileParty3 != BanditLoopTestStatic.BanditLoopBandit1Party && mobileParty3 != BanditLoopTestStatic.BanditLoopBandit2Party && mobileParty3.Position2D.DistanceSquared(BanditLoopTestStatic.BanditLoopPlayerParty.Position2D) < 25f)
				{
					mobileParty3.Position2D = MobilePartyHelper.FindReachablePointAroundPosition(@object.Position2D, 37.5f, 0f);
				}
			}
		}

		// Token: 0x04000611 RID: 1553
		public static bool IsBanditLoopTest;

		// Token: 0x04000612 RID: 1554
		public static MobileParty BanditLoopPlayerParty;

		// Token: 0x04000613 RID: 1555
		public static MobileParty BanditLoopBandit1Party;

		// Token: 0x04000614 RID: 1556
		public static MobileParty BanditLoopBandit2Party;

		// Token: 0x04000615 RID: 1557
		public static Settlement BanditLoopSettlement;
	}
}

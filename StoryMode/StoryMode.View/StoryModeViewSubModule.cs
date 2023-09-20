using System;
using System.Collections.Generic;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace StoryMode.View
{
	// Token: 0x02000003 RID: 3
	public class StoryModeViewSubModule : MBSubModuleBase
	{
		// Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			TextObject coreContentDisabledReason = new TextObject("{=V8BXjyYq}Disabled during installation.", null);
			Module.CurrentModule.AddInitialStateOption(new InitialStateOption("StoryModeNewGame", new TextObject("{=sf_menu_storymode_new_game}New Campaign", null), 2, delegate
			{
				MBGameManager.StartNewGame(new StoryModeGameManager());
			}, () => new ValueTuple<bool, TextObject>(Module.CurrentModule.IsOnlyCoreContentEnabled, coreContentDisabledReason)));
			Module.CurrentModule.ImguiProfilerTick += this.OnImguiProfilerTick;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020E3 File Offset: 0x000002E3
		protected virtual void FillDataForCampaign()
		{
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020E5 File Offset: 0x000002E5
		protected override void OnSubModuleUnloaded()
		{
			Module.CurrentModule.ImguiProfilerTick -= this.OnImguiProfilerTick;
			base.OnSubModuleUnloaded();
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002104 File Offset: 0x00000304
		private void OnImguiProfilerTick()
		{
			if (Campaign.Current == null)
			{
				return;
			}
			List<MobileParty> all = MobileParty.All;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (MobileParty mobileParty in all)
			{
				if (mobileParty.IsVisible)
				{
					num++;
				}
				if (((PartyVisual)mobileParty.Party.Visuals).HumanAgentVisuals != null)
				{
					num2++;
				}
				if (((PartyVisual)mobileParty.Party.Visuals).MountAgentVisuals != null)
				{
					num2++;
				}
				if (((PartyVisual)mobileParty.Party.Visuals).CaravanMountAgentVisuals != null)
				{
					num2++;
				}
				num3++;
			}
			Imgui.BeginMainThreadScope();
			Imgui.Begin("Bannerlord Campaign Statistics");
			Imgui.Columns(2, "", true);
			Imgui.Text("Name");
			Imgui.NextColumn();
			Imgui.Text("Count");
			Imgui.NextColumn();
			Imgui.Separator();
			Imgui.Text("Total Mobile Party");
			Imgui.NextColumn();
			Imgui.Text(num3.ToString());
			Imgui.NextColumn();
			Imgui.Text("Visible Mobile Party");
			Imgui.NextColumn();
			Imgui.Text(num.ToString());
			Imgui.NextColumn();
			Imgui.Text("Total Agent Visuals");
			Imgui.NextColumn();
			Imgui.Text(num2.ToString());
			Imgui.NextColumn();
			Imgui.End();
			Imgui.EndMainThreadScope();
		}
	}
}

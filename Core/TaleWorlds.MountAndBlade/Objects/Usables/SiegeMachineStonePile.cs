using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade.Objects.Usables
{
	// Token: 0x020003A2 RID: 930
	public class SiegeMachineStonePile : UsableMachine, ISpawnable
	{
		// Token: 0x060032B9 RID: 12985 RVA: 0x000D1D4B File Offset: 0x000CFF4B
		protected internal override void OnInit()
		{
			base.OnInit();
		}

		// Token: 0x060032BA RID: 12986 RVA: 0x000D1D53 File Offset: 0x000CFF53
		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			if (usableGameObject.GameEntity.HasTag(this.AmmoPickUpTag))
			{
				TextObject textObject = new TextObject("{=jfcceEoE}{PILE_TYPE} Pile", null);
				textObject.SetTextVariable("PILE_TYPE", new TextObject("{=1CPdu9K0}Stone", null));
				return textObject;
			}
			return TextObject.Empty;
		}

		// Token: 0x060032BB RID: 12987 RVA: 0x000D1D90 File Offset: 0x000CFF90
		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			if (gameEntity.HasTag(this.AmmoPickUpTag))
			{
				TextObject textObject = new TextObject("{=bNYm3K6b}{KEY} Pick Up", null);
				textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
				return textObject.ToString();
			}
			return string.Empty;
		}

		// Token: 0x060032BC RID: 12988 RVA: 0x000D1DDE File Offset: 0x000CFFDE
		public void SetSpawnedFromSpawner()
		{
			this._spawnedFromSpawner = true;
		}

		// Token: 0x060032BD RID: 12989 RVA: 0x000D1DE7 File Offset: 0x000CFFE7
		public override OrderType GetOrder(BattleSideEnum side)
		{
			return OrderType.None;
		}

		// Token: 0x04001561 RID: 5473
		private bool _spawnedFromSpawner;
	}
}

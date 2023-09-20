using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade.Objects.Usables
{
	public class SiegeMachineStonePile : UsableMachine, ISpawnable
	{
		protected internal override void OnInit()
		{
			base.OnInit();
		}

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

		public void SetSpawnedFromSpawner()
		{
			this._spawnedFromSpawner = true;
		}

		public override OrderType GetOrder(BattleSideEnum side)
		{
			return OrderType.None;
		}

		private bool _spawnedFromSpawner;
	}
}

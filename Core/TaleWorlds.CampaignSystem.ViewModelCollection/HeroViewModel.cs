using System;
using Helpers;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	// Token: 0x02000017 RID: 23
	public class HeroViewModel : CharacterViewModel
	{
		// Token: 0x06000158 RID: 344 RVA: 0x0000A6FF File Offset: 0x000088FF
		public HeroViewModel(CharacterViewModel.StanceTypes stance = CharacterViewModel.StanceTypes.None)
			: base(stance)
		{
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0000A708 File Offset: 0x00008908
		public override void SetEquipment(Equipment equipment)
		{
			this._equipment = ((equipment != null) ? equipment.Clone(false) : null);
			Equipment equipment2 = this._equipment;
			base.HasMount = ((equipment2 != null) ? equipment2[10].Item : null) != null;
			Equipment equipment3 = this._equipment;
			base.EquipmentCode = ((equipment3 != null) ? equipment3.CalculateEquipmentCode() : null);
			if (this._hero != null)
			{
				base.MountCreationKey = TaleWorlds.Core.MountCreationKey.GetRandomMountKeyString(equipment[10].Item, this._hero.CharacterObject.GetMountKeySeed());
			}
		}

		// Token: 0x0600015A RID: 346 RVA: 0x0000A798 File Offset: 0x00008998
		public void FillFrom(Hero hero, int seed = -1, bool useCivilian = false, bool useCharacteristicIdleAction = false)
		{
			TextObject textObject;
			base.IsHidden = CampaignUIHelper.IsHeroInformationHidden(hero, out textObject);
			if (FaceGen.GetMaturityTypeWithAge(hero.Age) > BodyMeshMaturityType.Child && !base.IsHidden)
			{
				this._hero = hero;
				base.FillFrom(hero.CharacterObject, seed);
				base.MountCreationKey = TaleWorlds.Core.MountCreationKey.GetRandomMountKeyString(hero.CharacterObject.Equipment[10].Item, hero.CharacterObject.GetMountKeySeed());
				this.IsDead = hero.IsDead;
				if (hero.IsNoncombatant || useCivilian)
				{
					Equipment civilianEquipment = hero.CivilianEquipment;
					this._equipment = ((civilianEquipment != null) ? civilianEquipment.Clone(false) : null);
				}
				else
				{
					Equipment battleEquipment = hero.BattleEquipment;
					this._equipment = ((battleEquipment != null) ? battleEquipment.Clone(false) : null);
				}
				if (useCharacteristicIdleAction)
				{
					ConversationAnimData conversationAnimData;
					if (Campaign.Current.ConversationManager.ConversationAnimationManager.ConversationAnims.TryGetValue(CharacterHelper.GetNonconversationPose(hero.CharacterObject), out conversationAnimData))
					{
						base.IdleAction = conversationAnimData.IdleAnimLoop;
					}
					base.IdleFaceAnim = CharacterHelper.GetNonconversationFacialIdle(hero.CharacterObject) ?? "";
				}
				Equipment equipment = this._equipment;
				base.EquipmentCode = ((equipment != null) ? equipment.CalculateEquipmentCode() : null);
				Equipment equipment2 = this._equipment;
				base.HasMount = ((equipment2 != null) ? equipment2[10].Item : null) != null;
				if (((hero != null) ? hero.ClanBanner : null) != null)
				{
					base.BannerCodeText = BannerCode.CreateFrom(hero.ClanBanner).Code;
				}
				IFaction mapFaction = hero.MapFaction;
				base.ArmorColor1 = ((mapFaction != null) ? mapFaction.Color : 0U);
				IFaction mapFaction2 = hero.MapFaction;
				base.ArmorColor2 = ((mapFaction2 != null) ? mapFaction2.Color2 : 0U);
			}
		}

		// Token: 0x0600015B RID: 347 RVA: 0x0000A940 File Offset: 0x00008B40
		public override void OnFinalize()
		{
			base.OnFinalize();
			this._hero = null;
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600015C RID: 348 RVA: 0x0000A94F File Offset: 0x00008B4F
		// (set) Token: 0x0600015D RID: 349 RVA: 0x0000A957 File Offset: 0x00008B57
		[DataSourceProperty]
		public bool IsDead
		{
			get
			{
				return this._isDead;
			}
			set
			{
				if (value != this._isDead)
				{
					this._isDead = value;
					base.OnPropertyChangedWithValue(value, "IsDead");
				}
			}
		}

		// Token: 0x040000A6 RID: 166
		private Hero _hero;

		// Token: 0x040000A7 RID: 167
		private bool _isDead;
	}
}

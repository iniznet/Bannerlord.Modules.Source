using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CharacterCreationContent
{
	// Token: 0x020001E6 RID: 486
	public class SandboxCharacterCreationContent : CharacterCreationContentBase
	{
		// Token: 0x1700074E RID: 1870
		// (get) Token: 0x06001C42 RID: 7234 RVA: 0x0007E7B1 File Offset: 0x0007C9B1
		public override TextObject ReviewPageDescription
		{
			get
			{
				return new TextObject("{=W6pKpEoT}You prepare to set off for a grand adventure in Calradia! Here is your character. Continue if you are ready, or go back to make changes.", null);
			}
		}

		// Token: 0x1700074F RID: 1871
		// (get) Token: 0x06001C43 RID: 7235 RVA: 0x0007E7BE File Offset: 0x0007C9BE
		public override IEnumerable<Type> CharacterCreationStages
		{
			get
			{
				yield return typeof(CharacterCreationCultureStage);
				yield return typeof(CharacterCreationFaceGeneratorStage);
				yield return typeof(CharacterCreationGenericStage);
				yield return typeof(CharacterCreationBannerEditorStage);
				yield return typeof(CharacterCreationClanNamingStage);
				yield return typeof(CharacterCreationReviewStage);
				yield return typeof(CharacterCreationOptionsStage);
				yield break;
			}
		}

		// Token: 0x06001C44 RID: 7236 RVA: 0x0007E7C8 File Offset: 0x0007C9C8
		protected override void OnCultureSelected()
		{
			base.SelectedTitleType = 1;
			base.SelectedParentType = 0;
			TextObject textObject = FactionHelper.GenerateClanNameforPlayer();
			Clan.PlayerClan.ChangeClanName(textObject, textObject);
		}

		// Token: 0x06001C45 RID: 7237 RVA: 0x0007E7F5 File Offset: 0x0007C9F5
		public override int GetSelectedParentType()
		{
			return base.SelectedParentType;
		}

		// Token: 0x06001C46 RID: 7238 RVA: 0x0007E800 File Offset: 0x0007CA00
		public override void OnCharacterCreationFinalized()
		{
			CultureObject culture = CharacterObject.PlayerCharacter.Culture;
			Vec2 vec;
			if (this._startingPoints.TryGetValue(culture.StringId, out vec))
			{
				MobileParty.MainParty.Position2D = vec;
			}
			else
			{
				MobileParty.MainParty.Position2D = Campaign.Current.DefaultStartingPosition;
				Debug.FailedAssert("Selected culture is not in the dictionary!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\CharacterCreationContent\\SandboxCharacterCreationContent.cs", "OnCharacterCreationFinalized", 108);
			}
			MapState mapState;
			if ((mapState = GameStateManager.Current.ActiveState as MapState) != null)
			{
				mapState.Handler.ResetCamera(true, true);
				mapState.Handler.TeleportCameraToMainParty();
			}
			this.SetHeroAge((float)this._startingAge);
		}

		// Token: 0x06001C47 RID: 7239 RVA: 0x0007E89C File Offset: 0x0007CA9C
		protected override void OnInitialized(CharacterCreation characterCreation)
		{
			this.AddParentsMenu(characterCreation);
			this.AddChildhoodMenu(characterCreation);
			this.AddEducationMenu(characterCreation);
			this.AddYouthMenu(characterCreation);
			this.AddAdulthoodMenu(characterCreation);
			this.AddAgeSelectionMenu(characterCreation);
		}

		// Token: 0x06001C48 RID: 7240 RVA: 0x0007E8C8 File Offset: 0x0007CAC8
		protected override void OnApplyCulture()
		{
		}

		// Token: 0x06001C49 RID: 7241 RVA: 0x0007E8CC File Offset: 0x0007CACC
		protected void AddParentsMenu(CharacterCreation characterCreation)
		{
			CharacterCreationMenu characterCreationMenu = new CharacterCreationMenu(new TextObject("{=b4lDDcli}Family", null), new TextObject("{=XgFU1pCx}You were born into a family of...", null), new CharacterCreationOnInit(this.ParentsOnInit), CharacterCreationMenu.MenuTypes.MultipleChoice);
			CharacterCreationCategory characterCreationCategory = characterCreationMenu.AddMenuCategory(new CharacterCreationOnCondition(this.EmpireParentsOnCondition));
			MBList<SkillObject> mblist = new MBList<SkillObject>
			{
				DefaultSkills.Riding,
				DefaultSkills.Polearm
			};
			CharacterAttribute characterAttribute = DefaultCharacterAttributes.Vigor;
			characterCreationCategory.AddCategoryOption(new TextObject("{=InN5ZZt3}A landlord's retainers", null), mblist, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.EmpireLandlordsRetainerOnConsequence), new CharacterCreationApplyFinalEffects(this.EmpireLandlordsRetainerOnApply), new TextObject("{=ivKl4mV2}Your father was a trusted lieutenant of the local landowning aristocrat. He rode with the lord's cavalry, fighting as an armored lancer.", null), null, 0, 0, 0, 0, 0);
			mblist = new MBList<SkillObject>
			{
				DefaultSkills.Trade,
				DefaultSkills.Charm
			};
			characterAttribute = DefaultCharacterAttributes.Social;
			characterCreationCategory.AddCategoryOption(new TextObject("{=651FhzdR}Urban merchants", null), mblist, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.EmpireMerchantOnConsequence), new CharacterCreationApplyFinalEffects(this.EmpireMerchantOnApply), new TextObject("{=FQntPChs}Your family were merchants in one of the main cities of the Empire. They sometimes organized caravans to nearby towns, and discussed issues in the town council.", null), null, 0, 0, 0, 0, 0);
			mblist = new MBList<SkillObject>
			{
				DefaultSkills.Athletics,
				DefaultSkills.Polearm
			};
			characterAttribute = DefaultCharacterAttributes.Endurance;
			characterCreationCategory.AddCategoryOption(new TextObject("{=sb4gg8Ak}Freeholders", null), mblist, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.EmpireFreeholderOnConsequence), new CharacterCreationApplyFinalEffects(this.EmpireFreeholderOnApply), new TextObject("{=09z8Q08f}Your family were small farmers with just enough land to feed themselves and make a small profit. People like them were the pillars of the imperial rural economy, as well as the backbone of the levy.", null), null, 0, 0, 0, 0, 0);
			mblist = new MBList<SkillObject>
			{
				DefaultSkills.Crafting,
				DefaultSkills.Crossbow
			};
			characterAttribute = DefaultCharacterAttributes.Intelligence;
			characterCreationCategory.AddCategoryOption(new TextObject("{=v48N6h1t}Urban artisans", null), mblist, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.EmpireArtisanOnConsequence), new CharacterCreationApplyFinalEffects(this.EmpireArtisanOnApply), new TextObject("{=ZKynvffv}Your family owned their own workshop in a city, making goods from raw materials brought in from the countryside. Your father played an active if minor role in the town council, and also served in the militia.", null), null, 0, 0, 0, 0, 0);
			mblist = new MBList<SkillObject>
			{
				DefaultSkills.Scouting,
				DefaultSkills.Bow
			};
			characterAttribute = DefaultCharacterAttributes.Control;
			characterCreationCategory.AddCategoryOption(new TextObject("{=7eWmU2mF}Foresters", null), mblist, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.EmpireWoodsmanOnConsequence), new CharacterCreationApplyFinalEffects(this.EmpireWoodsmanOnApply), new TextObject("{=yRFSzSDZ}Your family lived in a village, but did not own their own land. Instead, your father supplemented paid jobs with long trips in the woods, hunting and trapping, always keeping a wary eye for the lord's game wardens.", null), null, 0, 0, 0, 0, 0);
			mblist = new MBList<SkillObject>
			{
				DefaultSkills.Roguery,
				DefaultSkills.Throwing
			};
			characterAttribute = DefaultCharacterAttributes.Cunning;
			characterCreationCategory.AddCategoryOption(new TextObject("{=aEke8dSb}Urban vagabonds", null), mblist, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.EmpireVagabondOnConsequence), new CharacterCreationApplyFinalEffects(this.EmpireVagabondOnApply), new TextObject("{=Jvf6K7TZ}Your family numbered among the many poor migrants living in the slums that grow up outside the walls of imperial cities, making whatever money they could from a variety of odd jobs. Sometimes they did service for one of the Empire's many criminal gangs, and you had an early look at the dark side of life.", null), null, 0, 0, 0, 0, 0);
			CharacterCreationCategory characterCreationCategory2 = characterCreationMenu.AddMenuCategory(new CharacterCreationOnCondition(this.VlandianParentsOnCondition));
			mblist = new MBList<SkillObject>
			{
				DefaultSkills.Riding,
				DefaultSkills.Polearm
			};
			characterAttribute = DefaultCharacterAttributes.Social;
			characterCreationCategory2.AddCategoryOption(new TextObject("{=2TptWc4m}A baron's retainers", null), mblist, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.VlandiaBaronsRetainerOnConsequence), new CharacterCreationApplyFinalEffects(this.VlandiaBaronsRetainerOnApply), new TextObject("{=0Suu1Q9q}Your father was a bailiff for a local feudal magnate. He looked after his liege's estates, resolved disputes in the village, and helped train the village levy. He rode with the lord's cavalry, fighting as an armored knight.", null), null, 0, 0, 0, 0, 0);
			mblist = new MBList<SkillObject>
			{
				DefaultSkills.Trade,
				DefaultSkills.Charm
			};
			characterAttribute = DefaultCharacterAttributes.Intelligence;
			characterCreationCategory2.AddCategoryOption(new TextObject("{=651FhzdR}Urban merchants", null), mblist, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.VlandiaMerchantOnConsequence), new CharacterCreationApplyFinalEffects(this.VlandiaMerchantOnApply), new TextObject("{=qNZFkxJb}Your family were merchants in one of the main cities of the kingdom. They organized caravans to nearby towns and were active in the local merchant's guild.", null), null, 0, 0, 0, 0, 0);
			mblist = new MBList<SkillObject>
			{
				DefaultSkills.Polearm,
				DefaultSkills.Crossbow
			};
			characterAttribute = DefaultCharacterAttributes.Endurance;
			characterCreationCategory2.AddCategoryOption(new TextObject("{=RDfXuVxT}Yeomen", null), mblist, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.VlandiaYeomanOnConsequence), new CharacterCreationApplyFinalEffects(this.VlandiaYeomanOnApply), new TextObject("{=BLZ4mdhb}Your family were small farmers with just enough land to feed themselves and make a small profit. People like them were the pillars of the kingdom's economy, as well as the backbone of the levy.", null), null, 0, 0, 0, 0, 0);
			mblist = new MBList<SkillObject>
			{
				DefaultSkills.Crafting,
				DefaultSkills.TwoHanded
			};
			characterAttribute = DefaultCharacterAttributes.Vigor;
			characterCreationCategory2.AddCategoryOption(new TextObject("{=p2KIhGbE}Urban blacksmith", null), mblist, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.VlandiaBlacksmithOnConsequence), new CharacterCreationApplyFinalEffects(this.VlandiaBlacksmithOnApply), new TextObject("{=btsMpRcA}Your family owned a smithy in a city. Your father played an active if minor role in the town council, and also served in the militia.", null), null, 0, 0, 0, 0, 0);
			mblist = new MBList<SkillObject>
			{
				DefaultSkills.Scouting,
				DefaultSkills.Crossbow
			};
			characterAttribute = DefaultCharacterAttributes.Control;
			characterCreationCategory2.AddCategoryOption(new TextObject("{=YcnK0Thk}Hunters", null), mblist, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.VlandiaHunterOnConsequence), new CharacterCreationApplyFinalEffects(this.VlandiaHunterOnApply), new TextObject("{=yRFSzSDZ}Your family lived in a village, but did not own their own land. Instead, your father supplemented paid jobs with long trips in the woods, hunting and trapping, always keeping a wary eye for the lord's game wardens.", null), null, 0, 0, 0, 0, 0);
			mblist = new MBList<SkillObject>
			{
				DefaultSkills.Roguery,
				DefaultSkills.Crossbow
			};
			characterAttribute = DefaultCharacterAttributes.Cunning;
			characterCreationCategory2.AddCategoryOption(new TextObject("{=ipQP6aVi}Mercenaries", null), mblist, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.VlandiaMercenaryOnConsequence), new CharacterCreationApplyFinalEffects(this.VlandiaMercenaryOnApply), new TextObject("{=yYhX6JQC}Your father joined one of Vlandia's many mercenary companies, composed of men who got such a taste for war in their lord's service that they never took well to peace. Their crossbowmen were much valued across Calradia. Your mother was a camp follower, taking you along in the wake of bloody campaigns.", null), null, 0, 0, 0, 0, 0);
			CharacterCreationCategory characterCreationCategory3 = characterCreationMenu.AddMenuCategory(new CharacterCreationOnCondition(this.SturgianParentsOnCondition));
			characterCreationCategory3.AddCategoryOption(new TextObject("{=mc78FEbA}A boyar's companions", null), new MBList<SkillObject>
			{
				DefaultSkills.Riding,
				DefaultSkills.TwoHanded
			}, DefaultCharacterAttributes.Social, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.SturgiaBoyarsCompanionOnConsequence), new CharacterCreationApplyFinalEffects(this.SturgiaBoyarsCompanionOnApply), new TextObject("{=hob3WVkU}Your father was a member of a boyar's druzhina, the 'companions' that make up his retinue. He sat at his lord's table in the great hall, oversaw the boyar's estates, and stood by his side in the center of the shield wall in battle.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory3.AddCategoryOption(new TextObject("{=HqzVBfpl}Urban traders", null), new MBList<SkillObject>
			{
				DefaultSkills.Trade,
				DefaultSkills.Tactics
			}, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.SturgiaTraderOnConsequence), new CharacterCreationApplyFinalEffects(this.SturgiaTraderOnApply), new TextObject("{=bjVMtW3W}Your family were merchants who lived in one of Sturgia's great river ports, organizing the shipment of the north's bounty of furs, honey and other goods to faraway lands.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory3.AddCategoryOption(new TextObject("{=zrpqSWSh}Free farmers", null), new MBList<SkillObject>
			{
				DefaultSkills.Athletics,
				DefaultSkills.Polearm
			}, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.SturgiaFreemanOnConsequence), new CharacterCreationApplyFinalEffects(this.SturgiaFreemanOnApply), new TextObject("{=Mcd3ZyKq}Your family had just enough land to feed themselves and make a small profit. People like them were the pillars of the kingdom's economy, as well as the backbone of the levy.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory3.AddCategoryOption(new TextObject("{=v48N6h1t}Urban artisans", null), new MBList<SkillObject>
			{
				DefaultSkills.Crafting,
				DefaultSkills.OneHanded
			}, DefaultCharacterAttributes.Intelligence, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.SturgiaArtisanOnConsequence), new CharacterCreationApplyFinalEffects(this.SturgiaArtisanOnApply), new TextObject("{=ueCm5y1C}Your family owned their own workshop in a city, making goods from raw materials brought in from the countryside. Your father played an active if minor role in the town council, and also served in the militia.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory3.AddCategoryOption(new TextObject("{=YcnK0Thk}Hunters", null), new MBList<SkillObject>
			{
				DefaultSkills.Scouting,
				DefaultSkills.Bow
			}, DefaultCharacterAttributes.Vigor, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.SturgiaHunterOnConsequence), new CharacterCreationApplyFinalEffects(this.SturgiaHunterOnApply), new TextObject("{=WyZ2UtFF}Your family had no taste for the authority of the boyars. They made their living deep in the woods, slashing and burning fields which they tended for a year or two before moving on. They hunted and trapped fox, hare, ermine, and other fur-bearing animals.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory3.AddCategoryOption(new TextObject("{=TPoK3GSj}Vagabonds", null), new MBList<SkillObject>
			{
				DefaultSkills.Roguery,
				DefaultSkills.Throwing
			}, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.SturgiaVagabondOnConsequence), new CharacterCreationApplyFinalEffects(this.SturgiaVagabondOnApply), new TextObject("{=2SDWhGmQ}Your family numbered among the poor migrants living in the slums that grow up outside the walls of the river cities, making whatever money they could from a variety of odd jobs. Sometimes they did services for one of the region's many criminal gangs.", null), null, 0, 0, 0, 0, 0);
			CharacterCreationCategory characterCreationCategory4 = characterCreationMenu.AddMenuCategory(new CharacterCreationOnCondition(this.AseraiParentsOnCondition));
			characterCreationCategory4.AddCategoryOption(new TextObject("{=Sw8OxnNr}Kinsfolk of an emir", null), new MBList<SkillObject>
			{
				DefaultSkills.Riding,
				DefaultSkills.Throwing
			}, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.AseraiTribesmanOnConsequence), new CharacterCreationApplyFinalEffects(this.AseraiTribesmanOnApply), new TextObject("{=MFrIHJZM}Your family was from a smaller offshoot of an emir's tribe. Your father's land gave him enough income to afford a horse but he was not quite wealthy enough to buy the armor needed to join the heavier cavalry. He fought as one of the light horsemen for which the desert is famous.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory4.AddCategoryOption(new TextObject("{=ngFVgwDD}Warrior-slaves", null), new MBList<SkillObject>
			{
				DefaultSkills.Riding,
				DefaultSkills.Polearm
			}, DefaultCharacterAttributes.Vigor, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.AseraiWariorSlaveOnConsequence), new CharacterCreationApplyFinalEffects(this.AseraiWariorSlaveOnApply), new TextObject("{=GsPC2MgU}Your father was part of one of the slave-bodyguards maintained by the Aserai emirs. He fought by his master's side with tribe's armored cavalry, and was freed - perhaps for an act of valor, or perhaps he paid for his freedom with his share of the spoils of battle. He then married your mother.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory4.AddCategoryOption(new TextObject("{=651FhzdR}Urban merchants", null), new MBList<SkillObject>
			{
				DefaultSkills.Trade,
				DefaultSkills.Charm
			}, DefaultCharacterAttributes.Social, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.AseraiMerchantOnConsequence), new CharacterCreationApplyFinalEffects(this.AseraiMerchantOnApply), new TextObject("{=1zXrlaav}Your family were respected traders in an oasis town. They ran caravans across the desert, and were experts in the finer points of negotiating passage through the desert tribes' territories.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory4.AddCategoryOption(new TextObject("{=g31pXuqi}Oasis farmers", null), new MBList<SkillObject>
			{
				DefaultSkills.Athletics,
				DefaultSkills.OneHanded
			}, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.AseraiOasisFarmerOnConsequence), new CharacterCreationApplyFinalEffects(this.AseraiOasisFarmerOnApply), new TextObject("{=5P0KqBAw}Your family tilled the soil in one of the oases of the Nahasa and tended the palm orchards that produced the desert's famous dates. Your father was a member of the main foot levy of his tribe, fighting with his kinsmen under the emir's banner.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory4.AddCategoryOption(new TextObject("{=EEedqolz}Bedouin", null), new MBList<SkillObject>
			{
				DefaultSkills.Scouting,
				DefaultSkills.Bow
			}, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.AseraiBedouinOnConsequence), new CharacterCreationApplyFinalEffects(this.AseraiBedouinOnApply), new TextObject("{=PKhcPbBX}Your family were part of a nomadic clan, crisscrossing the wastes between wadi beds and wells to feed their herds of goats and camels on the scraggly scrubs of the Nahasa.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory4.AddCategoryOption(new TextObject("{=tRIrbTvv}Urban back-alley thugs", null), new MBList<SkillObject>
			{
				DefaultSkills.Roguery,
				DefaultSkills.Polearm
			}, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.AseraiBackAlleyThugOnConsequence), new CharacterCreationApplyFinalEffects(this.AseraiBackAlleyThugOnApply), new TextObject("{=6bUSbsKC}Your father worked for a fitiwi, one of the strongmen who keep order in the poorer quarters of the oasis towns. He resolved disputes over land, dice and insults, imposing his authority with the fitiwi's traditional staff.", null), null, 0, 0, 0, 0, 0);
			CharacterCreationCategory characterCreationCategory5 = characterCreationMenu.AddMenuCategory(new CharacterCreationOnCondition(this.BattanianParentsOnCondition));
			characterCreationCategory5.AddCategoryOption(new TextObject("{=GeNKQlHR}Members of the chieftain's hearthguard", null), new MBList<SkillObject>
			{
				DefaultSkills.TwoHanded,
				DefaultSkills.Bow
			}, DefaultCharacterAttributes.Vigor, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.BattaniaChieftainsHearthguardOnConsequence), new CharacterCreationApplyFinalEffects(this.BattaniaChieftainsHearthguardOnApply), new TextObject("{=LpH8SYFL}Your family were the trusted kinfolk of a Battanian chieftain, and sat at his table in his great hall. Your father assisted his chief in running the affairs of the clan and trained with the traditional weapons of the Battanian elite, the two-handed sword or falx and the bow.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory5.AddCategoryOption(new TextObject("{=AeBzTj6w}Healers", null), new MBList<SkillObject>
			{
				DefaultSkills.Medicine,
				DefaultSkills.Charm
			}, DefaultCharacterAttributes.Intelligence, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.BattaniaHealerOnConsequence), new CharacterCreationApplyFinalEffects(this.BattaniaHealerOnApply), new TextObject("{=j6py5Rv5}Your parents were healers who gathered herbs and treated the sick. As a living reservoir of Battanian tradition, they were also asked to adjudicate many disputes between the clans.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory5.AddCategoryOption(new TextObject("{=tGEStbxb}Tribespeople", null), new MBList<SkillObject>
			{
				DefaultSkills.Athletics,
				DefaultSkills.Throwing
			}, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.BattaniaTribesmanOnConsequence), new CharacterCreationApplyFinalEffects(this.BattaniaTribesmanOnApply), new TextObject("{=WchH8bS2}Your family were middle-ranking members of a Battanian clan, who tilled their own land. Your father fought with the kern, the main body of his people's warriors, joining in the screaming charges for which the Battanians were famous.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory5.AddCategoryOption(new TextObject("{=BCU6RezA}Smiths", null), new MBList<SkillObject>
			{
				DefaultSkills.Crafting,
				DefaultSkills.TwoHanded
			}, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.BattaniaSmithOnConsequence), new CharacterCreationApplyFinalEffects(this.BattaniaSmithOnApply), new TextObject("{=kg9YtrOg}Your family were smiths, a revered profession among the Battanians. They crafted everything from fine filigree jewelry in geometric designs to the well-balanced longswords favored by the Battanian aristocracy.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory5.AddCategoryOption(new TextObject("{=7eWmU2mF}Foresters", null), new MBList<SkillObject>
			{
				DefaultSkills.Scouting,
				DefaultSkills.Tactics
			}, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.BattaniaWoodsmanOnConsequence), new CharacterCreationApplyFinalEffects(this.BattaniaWoodsmanOnApply), new TextObject("{=7jBroUUQ}Your family had little land of their own, so they earned their living from the woods, hunting and trapping. They taught you from an early age that skills like finding game trails and killing an animal with one shot could make the difference between eating and starvation.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory5.AddCategoryOption(new TextObject("{=SpJqhEEh}Bards", null), new MBList<SkillObject>
			{
				DefaultSkills.Roguery,
				DefaultSkills.Charm
			}, DefaultCharacterAttributes.Social, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.BattaniaBardOnConsequence), new CharacterCreationApplyFinalEffects(this.BattaniaBardOnApply), new TextObject("{=aVzcyhhy}Your father was a bard, drifting from chieftain's hall to chieftain's hall making his living singing the praises of one Battanian aristocrat and mocking his enemies, then going to his enemy's hall and doing the reverse. You learned from him that a clever tongue could spare you  from a life toiling in the fields, if you kept your wits about you.", null), null, 0, 0, 0, 0, 0);
			CharacterCreationCategory characterCreationCategory6 = characterCreationMenu.AddMenuCategory(new CharacterCreationOnCondition(this.KhuzaitParentsOnCondition));
			characterCreationCategory6.AddCategoryOption(new TextObject("{=FVaRDe2a}A noyan's kinsfolk", null), new MBList<SkillObject>
			{
				DefaultSkills.Riding,
				DefaultSkills.Polearm
			}, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.KhuzaitNoyansKinsmanOnConsequence), new CharacterCreationApplyFinalEffects(this.KhuzaitNoyansKinsmanOnApply), new TextObject("{=jAs3kDXh}Your family were the trusted kinsfolk of a Khuzait noyan, and shared his meals in the chieftain's yurt. Your father assisted his chief in running the affairs of the clan and fought in the core of armored lancers in the center of the Khuzait battle line.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory6.AddCategoryOption(new TextObject("{=TkgLEDRM}Merchants", null), new MBList<SkillObject>
			{
				DefaultSkills.Trade,
				DefaultSkills.Charm
			}, DefaultCharacterAttributes.Social, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.KhuzaitMerchantOnConsequence), new CharacterCreationApplyFinalEffects(this.KhuzaitMerchantOnApply), new TextObject("{=qPg3IDiq}Your family came from one of the merchant clans that dominated the cities in eastern Calradia before the Khuzait conquest. They adjusted quickly to their new masters, keeping the caravan routes running and ensuring that the tariff revenues that once went into imperial coffers now flowed to the khanate.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory6.AddCategoryOption(new TextObject("{=tGEStbxb}Tribespeople", null), new MBList<SkillObject>
			{
				DefaultSkills.Bow,
				DefaultSkills.Riding
			}, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.KhuzaitTribesmanOnConsequence), new CharacterCreationApplyFinalEffects(this.KhuzaitTribesmanOnApply), new TextObject("{=URgZ4ai4}Your family were middle-ranking members of one of the Khuzait clans. He had some  herds of his own, but was not rich. When the Khuzait horde was summoned to battle, he fought with the horse archers, shooting and wheeling and wearing down the enemy before the lancers delivered the final punch.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory6.AddCategoryOption(new TextObject("{=gQ2tAvCz}Farmers", null), new MBList<SkillObject>
			{
				DefaultSkills.Polearm,
				DefaultSkills.Throwing
			}, DefaultCharacterAttributes.Vigor, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.KhuzaitFarmerOnConsequence), new CharacterCreationApplyFinalEffects(this.KhuzaitFarmerOnApply), new TextObject("{=5QSGoRFj}Your family tilled one of the small patches of arable land in the steppes for generations. When the Khuzaits came, they ceased paying taxes to the emperor and providing conscripts for his army, and served the khan instead.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory6.AddCategoryOption(new TextObject("{=vfhVveLW}Shamans", null), new MBList<SkillObject>
			{
				DefaultSkills.Medicine,
				DefaultSkills.Charm
			}, DefaultCharacterAttributes.Intelligence, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.KhuzaitShamanOnConsequence), new CharacterCreationApplyFinalEffects(this.KhuzaitShamanOnApply), new TextObject("{=WOKNhaG2}Your family were guardians of the sacred traditions of the Khuzaits, channelling the spirits of the wilderness and of the ancestors. They tended the sick and dispensed wisdom, resolving disputes and providing practical advice.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory6.AddCategoryOption(new TextObject("{=Xqba1Obq}Nomads", null), new MBList<SkillObject>
			{
				DefaultSkills.Scouting,
				DefaultSkills.Riding
			}, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.KhuzaitNomadOnConsequence), new CharacterCreationApplyFinalEffects(this.KhuzaitNomadOnApply), new TextObject("{=9aoQYpZs}Your family's clan never pledged its loyalty to the khan and never settled down, preferring to live out in the deep steppe away from his authority. They remain some of the finest trackers and scouts in the grasslands, as the ability to spot an enemy coming and move quickly is often all that protects their herds from their neighbors' predations.", null), null, 0, 0, 0, 0, 0);
			characterCreation.AddNewMenu(characterCreationMenu);
		}

		// Token: 0x06001C4A RID: 7242 RVA: 0x0007F8F4 File Offset: 0x0007DAF4
		protected void AddChildhoodMenu(CharacterCreation characterCreation)
		{
			CharacterCreationMenu characterCreationMenu = new CharacterCreationMenu(new TextObject("{=8Yiwt1z6}Early Childhood", null), new TextObject("{=character_creation_content_16}As a child you were noted for...", null), new CharacterCreationOnInit(this.ChildhoodOnInit), CharacterCreationMenu.MenuTypes.MultipleChoice);
			CharacterCreationCategory characterCreationCategory = characterCreationMenu.AddMenuCategory(null);
			characterCreationCategory.AddCategoryOption(new TextObject("{=kmM68Qx4}your leadership skills.", null), new MBList<SkillObject>
			{
				DefaultSkills.Leadership,
				DefaultSkills.Tactics
			}, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(SandboxCharacterCreationContent.ChildhoodYourLeadershipSkillsOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.ChildhoodGoodLeadingOnApply), new TextObject("{=FfNwXtii}If the wolf pup gang of your early childhood had an alpha, it was definitely you. All the other kids followed your lead as you decided what to play and where to play, and led them in games and mischief.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=5HXS8HEY}your brawn.", null), new MBList<SkillObject>
			{
				DefaultSkills.TwoHanded,
				DefaultSkills.Throwing
			}, DefaultCharacterAttributes.Vigor, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(SandboxCharacterCreationContent.ChildhoodYourBrawnOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.ChildhoodGoodAthleticsOnApply), new TextObject("{=YKzuGc54}You were big, and other children looked to have you around in any scrap with children from a neighboring village. You pushed a plough and threw an axe like an adult.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=QrYjPUEf}your attention to detail.", null), new MBList<SkillObject>
			{
				DefaultSkills.Athletics,
				DefaultSkills.Bow
			}, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(SandboxCharacterCreationContent.ChildhoodAttentionToDetailOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.ChildhoodGoodMemoryOnApply), new TextObject("{=JUSHAPnu}You were quick on your feet and attentive to what was going on around you. Usually you could run away from trouble, though you could give a good account of yourself in a fight with other children if cornered.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=Y3UcaX74}your aptitude for numbers.", null), new MBList<SkillObject>
			{
				DefaultSkills.Engineering,
				DefaultSkills.Trade
			}, DefaultCharacterAttributes.Intelligence, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(SandboxCharacterCreationContent.ChildhoodAptitudeForNumbersOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.ChildhoodGoodMathOnApply), new TextObject("{=DFidSjIf}Most children around you had only the most rudimentary education, but you lingered after class to study letters and mathematics. You were fascinated by the marketplace - weights and measures, tallies and accounts, the chatter about profits and losses.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=GEYzLuwb}your way with people.", null), new MBList<SkillObject>
			{
				DefaultSkills.Charm,
				DefaultSkills.Leadership
			}, DefaultCharacterAttributes.Social, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(SandboxCharacterCreationContent.ChildhoodWayWithPeopleOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.ChildhoodGoodMannersOnApply), new TextObject("{=w2TEQq26}You were always attentive to other people, good at guessing their motivations. You studied how individuals were swayed, and tried out what you learned from adults on your friends.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=MEgLE2kj}your skill with horses.", null), new MBList<SkillObject>
			{
				DefaultSkills.Riding,
				DefaultSkills.Medicine
			}, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(SandboxCharacterCreationContent.ChildhoodSkillsWithHorsesOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.ChildhoodAffinityWithAnimalsOnApply), new TextObject("{=ngazFofr}You were always drawn to animals, and spent as much time as possible hanging out in the village stables. You could calm horses, and were sometimes called upon to break in new colts. You learned the basics of veterinary arts, much of which is applicable to humans as well.", null), null, 0, 0, 0, 0, 0);
			characterCreation.AddNewMenu(characterCreationMenu);
		}

		// Token: 0x06001C4B RID: 7243 RVA: 0x0007FBC8 File Offset: 0x0007DDC8
		protected void AddEducationMenu(CharacterCreation characterCreation)
		{
			CharacterCreationMenu characterCreationMenu = new CharacterCreationMenu(new TextObject("{=rcoueCmk}Adolescence", null), this._educationIntroductoryText, new CharacterCreationOnInit(this.EducationOnInit), CharacterCreationMenu.MenuTypes.MultipleChoice);
			CharacterCreationCategory characterCreationCategory = characterCreationMenu.AddMenuCategory(null);
			characterCreationCategory.AddCategoryOption(new TextObject("{=RKVNvimC}herded the sheep.", null), new MBList<SkillObject>
			{
				DefaultSkills.Athletics,
				DefaultSkills.Throwing
			}, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.RuralAdolescenceOnCondition), new CharacterCreationOnSelect(this.RuralAdolescenceHerderOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.RuralAdolescenceHerderOnApply), new TextObject("{=KfaqPpbK}You went with other fleet-footed youths to take the villages' sheep, goats or cattle to graze in pastures near the village. You were in charge of chasing down stray beasts, and always kept a big stone on hand to be hurled at lurking predators if necessary.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=bTKiN0hr}worked in the village smithy.", null), new MBList<SkillObject>
			{
				DefaultSkills.TwoHanded,
				DefaultSkills.Crafting
			}, DefaultCharacterAttributes.Vigor, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.RuralAdolescenceOnCondition), new CharacterCreationOnSelect(this.RuralAdolescenceSmithyOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.RuralAdolescenceSmithyOnApply), new TextObject("{=y6j1bJTH}You were apprenticed to the local smith. You learned how to heat and forge metal, hammering for hours at a time until your muscles ached.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=tI8ZLtoA}repaired projects.", null), new MBList<SkillObject>
			{
				DefaultSkills.Crafting,
				DefaultSkills.Engineering
			}, DefaultCharacterAttributes.Intelligence, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.RuralAdolescenceOnCondition), new CharacterCreationOnSelect(this.RuralAdolescenceRepairmanOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.RuralAdolescenceRepairmanOnApply), new TextObject("{=6LFj919J}You helped dig wells, rethatch houses, and fix broken plows. You learned about the basics of construction, as well as what it takes to keep a farming community prosperous.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=TRwgSLD2}gathered herbs in the wild.", null), new MBList<SkillObject>
			{
				DefaultSkills.Medicine,
				DefaultSkills.Scouting
			}, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.RuralAdolescenceOnCondition), new CharacterCreationOnSelect(this.RuralAdolescenceGathererOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.RuralAdolescenceGathererOnApply), new TextObject("{=9ks4u5cH}You were sent by the village healer up into the hills to look for useful medicinal plants. You learned which herbs healed wounds or brought down a fever, and how to find them.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=T7m7ReTq}hunted small game.", null), new MBList<SkillObject>
			{
				DefaultSkills.Bow,
				DefaultSkills.Tactics
			}, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.RuralAdolescenceOnCondition), new CharacterCreationOnSelect(this.RuralAdolescenceHunterOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.RuralAdolescenceHunterOnApply), new TextObject("{=RuvSk3QT}You accompanied a local hunter as he went into the wilderness, helping him set up traps and catch small animals.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=qAbMagWq}sold product at the market.", null), new MBList<SkillObject>
			{
				DefaultSkills.Trade,
				DefaultSkills.Charm
			}, DefaultCharacterAttributes.Social, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.RuralAdolescenceOnCondition), new CharacterCreationOnSelect(this.RuralAdolescenceHelperOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.RuralAdolescenceHelperOnApply), new TextObject("{=DIgsfYfz}You took your family's goods to the nearest town to sell your produce and buy supplies. It was hard work, but you enjoyed the hubbub of the marketplace.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=nOfSqRnI}at the town watch's training ground.", null), new MBList<SkillObject>
			{
				DefaultSkills.Crossbow,
				DefaultSkills.Tactics
			}, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.UrbanAdolescenceOnCondition), new CharacterCreationOnSelect(this.UrbanAdolescenceWatcherOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.UrbanAdolescenceWatcherOnApply), new TextObject("{=qnqdEJOv}You watched the town's watch practice shooting and perfect their plans to defend the walls in case of a siege.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=8a6dnLd2}with the alley gangs.", null), new MBList<SkillObject>
			{
				DefaultSkills.Roguery,
				DefaultSkills.OneHanded
			}, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.UrbanAdolescenceOnCondition), new CharacterCreationOnSelect(this.UrbanAdolescenceGangerOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.UrbanAdolescenceGangerOnApply), new TextObject("{=1SUTcF0J}The gang leaders who kept watch over the slums of Calradian cities were always in need of poor youth to run messages and back them up in turf wars, while thrill-seeking merchants' sons and daughters sometimes slummed it in their company as well.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=7Hv984Sf}at docks and building sites.", null), new MBList<SkillObject>
			{
				DefaultSkills.Athletics,
				DefaultSkills.Crafting
			}, DefaultCharacterAttributes.Vigor, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.UrbanAdolescenceOnCondition), new CharacterCreationOnSelect(this.UrbanAdolescenceDockerOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.UrbanAdolescenceDockerOnApply), new TextObject("{=bhdkegZ4}All towns had their share of projects that were constantly in need of both skilled and unskilled labor. You learned how hoists and scaffolds were constructed, how planks and stones were hewn and fitted, and other skills.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=kbcwb5TH}in the markets and caravanserais.", null), new MBList<SkillObject>
			{
				DefaultSkills.Trade,
				DefaultSkills.Charm
			}, DefaultCharacterAttributes.Social, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.UrbanPoorAdolescenceOnCondition), new CharacterCreationOnSelect(this.UrbanAdolescenceMarketerOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.UrbanAdolescenceMarketerOnApply), new TextObject("{=lLJh7WAT}You worked in the marketplace, selling trinkets and drinks to busy shoppers.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=kbcwb5TH}in the markets and caravanserais.", null), new MBList<SkillObject>
			{
				DefaultSkills.Trade,
				DefaultSkills.Charm
			}, DefaultCharacterAttributes.Social, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.UrbanRichAdolescenceOnCondition), new CharacterCreationOnSelect(this.UrbanAdolescenceMarketerOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.UrbanAdolescenceMarketerOnApply), new TextObject("{=rmMcwSn8}You helped your family handle their business affairs, going down to the marketplace to make purchases and oversee the arrival of caravans.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=mfRbx5KE}reading and studying.", null), new MBList<SkillObject>
			{
				DefaultSkills.Engineering,
				DefaultSkills.Leadership
			}, DefaultCharacterAttributes.Intelligence, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.UrbanPoorAdolescenceOnCondition), new CharacterCreationOnSelect(this.UrbanAdolescenceTutorOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.UrbanAdolescenceDockerOnApply), new TextObject("{=elQnygal}Your family scraped up the money for a rudimentary schooling and you took full advantage, reading voraciously on history, mathematics, and philosophy and discussing what you read with your tutor and classmates.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=etG87fB7}with your tutor.", null), new MBList<SkillObject>
			{
				DefaultSkills.Engineering,
				DefaultSkills.Leadership
			}, DefaultCharacterAttributes.Intelligence, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.UrbanRichAdolescenceOnCondition), new CharacterCreationOnSelect(this.UrbanAdolescenceTutorOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.UrbanAdolescenceDockerOnApply), new TextObject("{=hXl25avg}Your family arranged for a private tutor and you took full advantage, reading voraciously on history, mathematics, and philosophy and discussing what you read with your tutor and classmates.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=FKpLEamz}caring for horses.", null), new MBList<SkillObject>
			{
				DefaultSkills.Riding,
				DefaultSkills.Steward
			}, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.UrbanRichAdolescenceOnCondition), new CharacterCreationOnSelect(this.UrbanAdolescenceHorserOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.UrbanAdolescenceDockerOnApply), new TextObject("{=Ghz90npw}Your family owned a few horses at the town stables and you took charge of their care. Many evenings you would take them out beyond the walls and gallup through the fields, racing other youth.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=vH7GtuuK}working at the stables.", null), new MBList<SkillObject>
			{
				DefaultSkills.Riding,
				DefaultSkills.Steward
			}, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.UrbanPoorAdolescenceOnCondition), new CharacterCreationOnSelect(this.UrbanAdolescenceHorserOnConsequence), new CharacterCreationApplyFinalEffects(SandboxCharacterCreationContent.UrbanAdolescenceDockerOnApply), new TextObject("{=csUq1RCC}You were employed as a hired hand at the town's stables. The overseers recognized that you had a knack for horses, and you were allowed to exercise them and sometimes even break in new steeds.", null), null, 0, 0, 0, 0, 0);
			characterCreation.AddNewMenu(characterCreationMenu);
		}

		// Token: 0x06001C4C RID: 7244 RVA: 0x00080310 File Offset: 0x0007E510
		protected void AddYouthMenu(CharacterCreation characterCreation)
		{
			CharacterCreationMenu characterCreationMenu = new CharacterCreationMenu(new TextObject("{=ok8lSW6M}Youth", null), this._youthIntroductoryText, new CharacterCreationOnInit(this.YouthOnInit), CharacterCreationMenu.MenuTypes.MultipleChoice);
			CharacterCreationCategory characterCreationCategory = characterCreationMenu.AddMenuCategory(null);
			characterCreationCategory.AddCategoryOption(new TextObject("{=CITG915d}joined a commander's staff.", null), new MBList<SkillObject>
			{
				DefaultSkills.Steward,
				DefaultSkills.Tactics
			}, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthCommanderOnCondition), new CharacterCreationOnSelect(this.YouthCommanderOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthCommanderOnApply), new TextObject("{=Ay0G3f7I}Your family arranged for you to be part of the staff of an imperial strategos. You were not given major responsibilities - mostly carrying messages and tending to his horse -- but it did give you a chance to see how campaigns were planned and men were deployed in battle.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=bhE2i6OU}served as a baron's groom.", null), new MBList<SkillObject>
			{
				DefaultSkills.Steward,
				DefaultSkills.Tactics
			}, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthGroomOnCondition), new CharacterCreationOnSelect(this.YouthGroomOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthGroomOnApply), new TextObject("{=iZKtGI6Y}Your family arranged for you to accompany a minor baron of the Vlandian kingdom. You were not given major responsibilities - mostly carrying messages and tending to his horse -- but it did give you a chance to see how campaigns were planned and men were deployed in battle.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=F2bgujPo}were a chieftain's servant.", null), new MBList<SkillObject>
			{
				DefaultSkills.Steward,
				DefaultSkills.Tactics
			}, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthChieftainOnCondition), new CharacterCreationOnSelect(this.YouthChieftainOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthChieftainOnApply), new TextObject("{=7AYJ3SjK}Your family arranged for you to accompany a chieftain of your people. You were not given major responsibilities - mostly carrying messages and tending to his horse -- but it did give you a chance to see how campaigns were planned and men were deployed in battle.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=h2KnarLL}trained with the cavalry.", null), new MBList<SkillObject>
			{
				DefaultSkills.Riding,
				DefaultSkills.Polearm
			}, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthCavalryOnCondition), new CharacterCreationOnSelect(this.YouthCavalryOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthCavalryOnApply), new TextObject("{=7cHsIMLP}You could never have bought the equipment on your own, but you were a good enough rider so that the local lord lent you a horse and equipment. You joined the armored cavalry, training with the lance.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=zsC2t5Hb}trained with the hearth guard.", null), new MBList<SkillObject>
			{
				DefaultSkills.Riding,
				DefaultSkills.Polearm
			}, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthHearthGuardOnCondition), new CharacterCreationOnSelect(this.YouthHearthGuardOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthHearthGuardOnApply), new TextObject("{=RmbWW6Bm}You were a big and imposing enough youth that the chief's guard allowed you to train alongside them, in preparation to join them some day.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=aTncHUfL}stood guard with the garrisons.", null), new MBList<SkillObject>
			{
				DefaultSkills.Crossbow,
				DefaultSkills.Engineering
			}, DefaultCharacterAttributes.Intelligence, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthGarrisonOnCondition), new CharacterCreationOnSelect(this.YouthGarrisonOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthGarrisonOnApply), new TextObject("{=63TAYbkx}Urban troops spend much of their time guarding the town walls. Most of their training was in missile weapons, especially useful during sieges.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=aTncHUfL}stood guard with the garrisons.", null), new MBList<SkillObject>
			{
				DefaultSkills.Bow,
				DefaultSkills.Engineering
			}, DefaultCharacterAttributes.Intelligence, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthOtherGarrisonOnCondition), new CharacterCreationOnSelect(this.YouthOtherGarrisonOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthOtherGarrisonOnApply), new TextObject("{=1EkEElZd}Urban troops spend much of their time guarding the town walls. Most of their training was in missile weapons.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=VlXOgIX6}rode with the scouts.", null), new MBList<SkillObject>
			{
				DefaultSkills.Riding,
				DefaultSkills.Bow
			}, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthOutridersOnCondition), new CharacterCreationOnSelect(this.YouthOutridersOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthOutridersOnApply), new TextObject("{=888lmJqs}All of Calradia's kingdoms recognize the value of good light cavalry and horse archers, and are sure to recruit nomads and borderers with the skills to fulfill those duties. You were a good enough rider that your neighbors pitched in to buy you a small pony and a good bow so that you could fulfill their levy obligations.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=VlXOgIX6}rode with the scouts.", null), new MBList<SkillObject>
			{
				DefaultSkills.Riding,
				DefaultSkills.Bow
			}, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthOtherOutridersOnCondition), new CharacterCreationOnSelect(this.YouthOtherOutridersOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthOtherOutridersOnApply), new TextObject("{=sYuN6hPD}All of Calradia's kingdoms recognize the value of good light cavalry, and are sure to recruit nomads and borderers with the skills to fulfill those duties. You were a good enough rider that your neighbors pitched in to buy you a small pony and a sheaf of javelins so that you could fulfill their levy obligations.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=a8arFSra}trained with the infantry.", null), new MBList<SkillObject>
			{
				DefaultSkills.Polearm,
				DefaultSkills.OneHanded
			}, DefaultCharacterAttributes.Vigor, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.YouthInfantryOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthInfantryOnApply), new TextObject("{=afH90aNs}Levy armed with spear and shield, drawn from smallholding farmers, have always been the backbone of most armies of Calradia.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=oMbOIPc9}joined the skirmishers.", null), new MBList<SkillObject>
			{
				DefaultSkills.Throwing,
				DefaultSkills.OneHanded
			}, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthSkirmisherOnCondition), new CharacterCreationOnSelect(this.YouthSkirmisherOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthSkirmisherOnApply), new TextObject("{=bXAg5w19}Younger recruits, or those of a slighter build, or those too poor to buy shield and armor tend to join the skirmishers. Fighting with bow and javelin, they try to stay out of reach of the main enemy forces.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=cDWbwBwI}joined the kern.", null), new MBList<SkillObject>
			{
				DefaultSkills.Throwing,
				DefaultSkills.OneHanded
			}, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthKernOnCondition), new CharacterCreationOnSelect(this.YouthKernOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthKernOnApply), new TextObject("{=tTb28jyU}Many Battanians fight as kern, versatile troops who could both harass the enemy line with their javelins or join in the final screaming charge once it weakened.", null), null, 0, 0, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=GFUggps8}marched with the camp followers.", null), new MBList<SkillObject>
			{
				DefaultSkills.Roguery,
				DefaultSkills.Throwing
			}, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthCamperOnCondition), new CharacterCreationOnSelect(this.YouthCamperOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthCamperOnApply), new TextObject("{=64rWqBLN}You avoided service with one of the main forces of your realm's armies, but followed instead in the train - the troops' wives, lovers and servants, and those who make their living by caring for, entertaining, or cheating the soldiery.", null), null, 0, 0, 0, 0, 0);
			characterCreation.AddNewMenu(characterCreationMenu);
		}

		// Token: 0x06001C4D RID: 7245 RVA: 0x0008095C File Offset: 0x0007EB5C
		protected void AddAdulthoodMenu(CharacterCreation characterCreation)
		{
			MBTextManager.SetTextVariable("EXP_VALUE", this.SkillLevelToAdd);
			CharacterCreationMenu characterCreationMenu = new CharacterCreationMenu(new TextObject("{=MafIe9yI}Young Adulthood", null), new TextObject("{=4WYY0X59}Before you set out for a life of adventure, your biggest achievement was...", null), new CharacterCreationOnInit(this.AccomplishmentOnInit), CharacterCreationMenu.MenuTypes.MultipleChoice);
			CharacterCreationCategory characterCreationCategory = characterCreationMenu.AddMenuCategory(null);
			characterCreationCategory.AddCategoryOption(new TextObject("{=8bwpVpgy}you defeated an enemy in battle.", null), new MBList<SkillObject>
			{
				DefaultSkills.OneHanded,
				DefaultSkills.TwoHanded
			}, DefaultCharacterAttributes.Vigor, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.AccomplishmentDefeatedEnemyOnConsequence), new CharacterCreationApplyFinalEffects(this.AccomplishmentDefeatedEnemyOnApply), new TextObject("{=1IEroJKs}Not everyone who musters for the levy marches to war, and not everyone who goes on campaign sees action. You did both, and you also took down an enemy warrior in direct one-to-one combat, in the full view of your comrades.", null), new MBList<TraitObject> { DefaultTraits.Valor }, 1, 20, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=mP3uFbcq}you led a successful manhunt.", null), new MBList<SkillObject>
			{
				DefaultSkills.Tactics,
				DefaultSkills.Leadership
			}, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.AccomplishmentPosseOnConditions), new CharacterCreationOnSelect(this.AccomplishmentExpeditionOnConsequence), new CharacterCreationApplyFinalEffects(this.AccomplishmentExpeditionOnApply), new TextObject("{=4f5xwzX0}When your community needed to organize a posse to pursue horse thieves, you were the obvious choice. You hunted down the raiders, surrounded them and forced their surrender, and took back your stolen property.", null), new MBList<TraitObject> { DefaultTraits.Calculating }, 1, 10, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=wfbtS71d}you led a caravan.", null), new MBList<SkillObject>
			{
				DefaultSkills.Tactics,
				DefaultSkills.Leadership
			}, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.AccomplishmentMerchantOnCondition), new CharacterCreationOnSelect(this.AccomplishmentMerchantOnConsequence), new CharacterCreationApplyFinalEffects(this.AccomplishmentExpeditionOnApply), new TextObject("{=joRHKCkm}Your family needed someone trustworthy to take a caravan to a neighboring town. You organized supplies, ensured a constant watch to keep away bandits, and brought it safely to its destination.", null), new MBList<TraitObject> { DefaultTraits.Calculating }, 1, 10, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=x1HTX5hq}you saved your village from a flood.", null), new MBList<SkillObject>
			{
				DefaultSkills.Tactics,
				DefaultSkills.Leadership
			}, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.AccomplishmentSavedVillageOnCondition), new CharacterCreationOnSelect(this.AccomplishmentSavedVillageOnConsequence), new CharacterCreationApplyFinalEffects(this.AccomplishmentExpeditionOnApply), new TextObject("{=bWlmGDf3}When a sudden storm caused the local stream to rise suddenly, your neighbors needed quick-thinking leadership. You provided it, directing them to build levees to save their homes.", null), new MBList<TraitObject> { DefaultTraits.Calculating }, 1, 10, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=s8PNllPN}you saved your city quarter from a fire.", null), new MBList<SkillObject>
			{
				DefaultSkills.Tactics,
				DefaultSkills.Leadership
			}, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.AccomplishmentSavedStreetOnCondition), new CharacterCreationOnSelect(this.AccomplishmentSavedStreetOnConsequence), new CharacterCreationApplyFinalEffects(this.AccomplishmentExpeditionOnApply), new TextObject("{=ZAGR6PYc}When a sudden blaze broke out in a back alley, your neighbors needed quick-thinking leadership and you provided it. You organized a bucket line to the nearest well, putting the fire out before any homes were lost.", null), new MBList<TraitObject> { DefaultTraits.Calculating }, 1, 10, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=xORjDTal}you invested some money in a workshop.", null), new MBList<SkillObject>
			{
				DefaultSkills.Trade,
				DefaultSkills.Crafting
			}, DefaultCharacterAttributes.Intelligence, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.AccomplishmentUrbanOnCondition), new CharacterCreationOnSelect(this.AccomplishmentWorkshopOnConsequence), new CharacterCreationApplyFinalEffects(this.AccomplishmentWorkshopOnApply), new TextObject("{=PyVqDLBu}Your parents didn't give you much money, but they did leave just enough for you to secure a loan against a larger amount to build a small workshop. You paid back what you borrowed, and sold your enterprise for a profit.", null), new MBList<TraitObject> { DefaultTraits.Calculating }, 1, 10, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=xKXcqRJI}you invested some money in land.", null), new MBList<SkillObject>
			{
				DefaultSkills.Trade,
				DefaultSkills.Crafting
			}, DefaultCharacterAttributes.Intelligence, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.AccomplishmentRuralOnCondition), new CharacterCreationOnSelect(this.AccomplishmentWorkshopOnConsequence), new CharacterCreationApplyFinalEffects(this.AccomplishmentWorkshopOnApply), new TextObject("{=cbF9jdQo}Your parents didn't give you much money, but they did leave just enough for you to purchase a plot of unused land at the edge of the village. You cleared away rocks and dug an irrigation ditch, raised a few seasons of crops, than sold it for a considerable profit.", null), new MBList<TraitObject> { DefaultTraits.Calculating }, 1, 10, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=TbNRtUjb}you hunted a dangerous animal.", null), new MBList<SkillObject>
			{
				DefaultSkills.Polearm,
				DefaultSkills.Crossbow
			}, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.AccomplishmentRuralOnCondition), new CharacterCreationOnSelect(this.AccomplishmentSiegeHunterOnConsequence), new CharacterCreationApplyFinalEffects(this.AccomplishmentSiegeHunterOnApply), new TextObject("{=I3PcdaaL}Wolves, bears are a constant menace to the flocks of northern Calradia, while hyenas and leopards trouble the south. You went with a group of your fellow villagers and fired the missile that brought down the beast.", null), new MBList<TraitObject> { DefaultTraits.Valor }, 1, 5, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=WbHfGCbd}you survived a siege.", null), new MBList<SkillObject>
			{
				DefaultSkills.Bow,
				DefaultSkills.Crossbow
			}, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.AccomplishmentUrbanOnCondition), new CharacterCreationOnSelect(this.AccomplishmentSiegeHunterOnConsequence), new CharacterCreationApplyFinalEffects(this.AccomplishmentSiegeHunterOnApply), new TextObject("{=FhZPjhli}Your hometown was briefly placed under siege, and you were called to defend the walls. Everyone did their part to repulse the enemy assault, and everyone is justly proud of what they endured.", null), null, 0, 5, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=kNXet6Um}you had a famous escapade in town.", null), new MBList<SkillObject>
			{
				DefaultSkills.Athletics,
				DefaultSkills.Roguery
			}, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.AccomplishmentRuralOnCondition), new CharacterCreationOnSelect(this.AccomplishmentEscapadeOnConsequence), new CharacterCreationApplyFinalEffects(this.AccomplishmentEscapadeOnApply), new TextObject("{=DjeAJtix}Maybe it was a love affair, or maybe you cheated at dice, or maybe you just chose your words poorly when drinking with a dangerous crowd. Anyway, on one of your trips into town you got into the kind of trouble from which only a quick tongue or quick feet get you out alive.", null), new MBList<TraitObject> { DefaultTraits.Valor }, 1, 5, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=qlOuiKXj}you had a famous escapade.", null), new MBList<SkillObject>
			{
				DefaultSkills.Athletics,
				DefaultSkills.Roguery
			}, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.AccomplishmentUrbanOnCondition), new CharacterCreationOnSelect(this.AccomplishmentEscapadeOnConsequence), new CharacterCreationApplyFinalEffects(this.AccomplishmentEscapadeOnApply), new TextObject("{=lD5Ob3R4}Maybe it was a love affair, or maybe you cheated at dice, or maybe you just chose your words poorly when drinking with a dangerous crowd. Anyway, you got into the kind of trouble from which only a quick tongue or quick feet get you out alive.", null), new MBList<TraitObject> { DefaultTraits.Valor }, 1, 5, 0, 0, 0);
			characterCreationCategory.AddCategoryOption(new TextObject("{=Yqm0Dics}you treated people well.", null), new MBList<SkillObject>
			{
				DefaultSkills.Charm,
				DefaultSkills.Steward
			}, DefaultCharacterAttributes.Social, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.AccomplishmentTreaterOnConsequence), new CharacterCreationApplyFinalEffects(this.AccomplishmentTreaterOnApply), new TextObject("{=dDmcqTzb}Yours wasn't the kind of reputation that local legends are made of, but it was the kind that wins you respect among those around you. You were consistently fair and honest in your business dealings and helpful to those in trouble. In doing so, you got a sense of what made people tick.", null), new MBList<TraitObject>
			{
				DefaultTraits.Mercy,
				DefaultTraits.Generosity,
				DefaultTraits.Honor
			}, 1, 5, 0, 0, 0);
			characterCreation.AddNewMenu(characterCreationMenu);
		}

		// Token: 0x06001C4E RID: 7246 RVA: 0x00080FFC File Offset: 0x0007F1FC
		protected void AddAgeSelectionMenu(CharacterCreation characterCreation)
		{
			MBTextManager.SetTextVariable("EXP_VALUE", this.SkillLevelToAdd);
			CharacterCreationMenu characterCreationMenu = new CharacterCreationMenu(new TextObject("{=HDFEAYDk}Starting Age", null), new TextObject("{=VlOGrGSn}Your character started off on the adventuring path at the age of...", null), new CharacterCreationOnInit(this.StartingAgeOnInit), CharacterCreationMenu.MenuTypes.MultipleChoice);
			CharacterCreationCategory characterCreationCategory = characterCreationMenu.AddMenuCategory(null);
			characterCreationCategory.AddCategoryOption(new TextObject("{=!}20", null), new MBList<SkillObject>(), null, 0, 0, 0, null, new CharacterCreationOnSelect(this.StartingAgeYoungOnConsequence), new CharacterCreationApplyFinalEffects(this.StartingAgeYoungOnApply), new TextObject("{=2k7adlh7}While lacking experience a bit, you are full with youthful energy, you are fully eager, for the long years of adventuring ahead.", null), null, 0, 0, 0, 2, 1);
			characterCreationCategory.AddCategoryOption(new TextObject("{=!}30", null), new MBList<SkillObject>(), null, 0, 0, 0, null, new CharacterCreationOnSelect(this.StartingAgeAdultOnConsequence), new CharacterCreationApplyFinalEffects(this.StartingAgeAdultOnApply), new TextObject("{=NUlVFRtK}You are at your prime, You still have some youthful energy but also have a substantial amount of experience under your belt. ", null), null, 0, 0, 0, 4, 2);
			characterCreationCategory.AddCategoryOption(new TextObject("{=!}40", null), new MBList<SkillObject>(), null, 0, 0, 0, null, new CharacterCreationOnSelect(this.StartingAgeMiddleAgedOnConsequence), new CharacterCreationApplyFinalEffects(this.StartingAgeMiddleAgedOnApply), new TextObject("{=5MxTYApM}This is the right age for starting off, you have years of experience, and you are old enough for people to respect you and gather under your banner.", null), null, 0, 0, 0, 6, 3);
			characterCreationCategory.AddCategoryOption(new TextObject("{=!}50", null), new MBList<SkillObject>(), null, 0, 0, 0, null, new CharacterCreationOnSelect(this.StartingAgeElderlyOnConsequence), new CharacterCreationApplyFinalEffects(this.StartingAgeElderlyOnApply), new TextObject("{=ePD5Afvy}While you are past your prime, there is still enough time to go on that last big adventure for you. And you have all the experience you need to overcome anything!", null), null, 0, 0, 0, 8, 4);
			characterCreation.AddNewMenu(characterCreationMenu);
		}

		// Token: 0x06001C4F RID: 7247 RVA: 0x00081160 File Offset: 0x0007F360
		protected void ParentsOnInit(CharacterCreation characterCreation)
		{
			characterCreation.IsPlayerAlone = false;
			characterCreation.HasSecondaryCharacter = false;
			SandboxCharacterCreationContent.ClearMountEntity(characterCreation);
			characterCreation.ClearFaceGenPrefab();
			if (base.PlayerBodyProperties != CharacterObject.PlayerCharacter.GetBodyProperties(CharacterObject.PlayerCharacter.Equipment, -1))
			{
				base.PlayerBodyProperties = CharacterObject.PlayerCharacter.GetBodyProperties(CharacterObject.PlayerCharacter.Equipment, -1);
				BodyProperties playerBodyProperties = base.PlayerBodyProperties;
				BodyProperties playerBodyProperties2 = base.PlayerBodyProperties;
				FaceGen.GenerateParentKey(base.PlayerBodyProperties, CharacterObject.PlayerCharacter.Race, ref playerBodyProperties, ref playerBodyProperties2);
				playerBodyProperties = new BodyProperties(new DynamicBodyProperties(33f, 0.3f, 0.2f), playerBodyProperties.StaticProperties);
				playerBodyProperties2 = new BodyProperties(new DynamicBodyProperties(33f, 0.5f, 0.5f), playerBodyProperties2.StaticProperties);
				base.MotherFacegenCharacter = new FaceGenChar(playerBodyProperties, CharacterObject.PlayerCharacter.Race, new Equipment(), true, "anim_mother_1");
				base.FatherFacegenCharacter = new FaceGenChar(playerBodyProperties2, CharacterObject.PlayerCharacter.Race, new Equipment(), false, "anim_father_1");
			}
			characterCreation.ChangeFaceGenChars(new List<FaceGenChar> { base.MotherFacegenCharacter, base.FatherFacegenCharacter });
			this.ChangeParentsOutfit(characterCreation, "", "", true, true);
			this.ChangeParentsAnimation(characterCreation);
		}

		// Token: 0x06001C50 RID: 7248 RVA: 0x000812B4 File Offset: 0x0007F4B4
		protected void ChangeParentsOutfit(CharacterCreation characterCreation, string fatherItemId = "", string motherItemId = "", bool isLeftHandItemForFather = true, bool isLeftHandItemForMother = true)
		{
			characterCreation.ClearFaceGenPrefab();
			List<Equipment> list = new List<Equipment>();
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(string.Concat(new object[]
			{
				"mother_char_creation_",
				base.SelectedParentType,
				"_",
				base.GetSelectedCulture().StringId
			}));
			Equipment equipment = ((@object != null) ? @object.DefaultEquipment : null) ?? MBEquipmentRoster.EmptyEquipment;
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(string.Concat(new object[]
			{
				"father_char_creation_",
				base.SelectedParentType,
				"_",
				base.GetSelectedCulture().StringId
			}));
			Equipment equipment2 = ((object2 != null) ? object2.DefaultEquipment : null) ?? MBEquipmentRoster.EmptyEquipment;
			if (motherItemId != "")
			{
				ItemObject object3 = Game.Current.ObjectManager.GetObject<ItemObject>(motherItemId);
				if (object3 != null)
				{
					equipment.AddEquipmentToSlotWithoutAgent(isLeftHandItemForMother ? EquipmentIndex.WeaponItemBeginSlot : EquipmentIndex.Weapon1, new EquipmentElement(object3, null, null, false));
				}
				else
				{
					Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(characterCreation.FaceGenChars[0].Race);
					characterCreation.ChangeCharacterPrefab(motherItemId, isLeftHandItemForMother ? baseMonsterFromRace.MainHandItemBoneIndex : baseMonsterFromRace.OffHandItemBoneIndex);
				}
			}
			if (fatherItemId != "")
			{
				ItemObject object4 = Game.Current.ObjectManager.GetObject<ItemObject>(fatherItemId);
				if (object4 != null)
				{
					equipment2.AddEquipmentToSlotWithoutAgent(isLeftHandItemForFather ? EquipmentIndex.WeaponItemBeginSlot : EquipmentIndex.Weapon1, new EquipmentElement(object4, null, null, false));
				}
			}
			list.Add(equipment);
			list.Add(equipment2);
			characterCreation.ChangeCharactersEquipment(list);
		}

		// Token: 0x06001C51 RID: 7249 RVA: 0x00081444 File Offset: 0x0007F644
		protected void ChangeParentsAnimation(CharacterCreation characterCreation)
		{
			List<string> list = new List<string>
			{
				"anim_mother_" + base.SelectedParentType,
				"anim_father_" + base.SelectedParentType
			};
			characterCreation.ChangeCharsAnimation(list);
		}

		// Token: 0x06001C52 RID: 7250 RVA: 0x00081494 File Offset: 0x0007F694
		protected void SetParentAndOccupationType(CharacterCreation characterCreation, int parentType, SandboxCharacterCreationContent.OccupationTypes occupationType, string fatherItemId = "", string motherItemId = "", bool isLeftHandItemForFather = true, bool isLeftHandItemForMother = true)
		{
			base.SelectedParentType = parentType;
			this._familyOccupationType = occupationType;
			characterCreation.ChangeFaceGenChars(new List<FaceGenChar> { base.MotherFacegenCharacter, base.FatherFacegenCharacter });
			this.ChangeParentsAnimation(characterCreation);
			this.ChangeParentsOutfit(characterCreation, fatherItemId, motherItemId, isLeftHandItemForFather, isLeftHandItemForMother);
		}

		// Token: 0x06001C53 RID: 7251 RVA: 0x000814E8 File Offset: 0x0007F6E8
		protected void EmpireLandlordsRetainerOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 1, SandboxCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
		}

		// Token: 0x06001C54 RID: 7252 RVA: 0x000814FF File Offset: 0x0007F6FF
		protected void EmpireMerchantOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 2, SandboxCharacterCreationContent.OccupationTypes.Merchant, "", "", true, true);
		}

		// Token: 0x06001C55 RID: 7253 RVA: 0x00081516 File Offset: 0x0007F716
		protected void EmpireFreeholderOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 3, SandboxCharacterCreationContent.OccupationTypes.Farmer, "", "", true, true);
		}

		// Token: 0x06001C56 RID: 7254 RVA: 0x0008152D File Offset: 0x0007F72D
		protected void EmpireArtisanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 4, SandboxCharacterCreationContent.OccupationTypes.Artisan, "", "", true, true);
		}

		// Token: 0x06001C57 RID: 7255 RVA: 0x00081544 File Offset: 0x0007F744
		protected void EmpireWoodsmanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 5, SandboxCharacterCreationContent.OccupationTypes.Hunter, "", "", true, true);
		}

		// Token: 0x06001C58 RID: 7256 RVA: 0x0008155B File Offset: 0x0007F75B
		protected void EmpireVagabondOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 6, SandboxCharacterCreationContent.OccupationTypes.Vagabond, "", "", true, true);
		}

		// Token: 0x06001C59 RID: 7257 RVA: 0x00081572 File Offset: 0x0007F772
		protected void EmpireLandlordsRetainerOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C5A RID: 7258 RVA: 0x0008157A File Offset: 0x0007F77A
		protected void EmpireMerchantOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C5B RID: 7259 RVA: 0x00081582 File Offset: 0x0007F782
		protected void EmpireFreeholderOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C5C RID: 7260 RVA: 0x0008158A File Offset: 0x0007F78A
		protected void EmpireArtisanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C5D RID: 7261 RVA: 0x00081592 File Offset: 0x0007F792
		protected void EmpireWoodsmanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C5E RID: 7262 RVA: 0x0008159A File Offset: 0x0007F79A
		protected void EmpireVagabondOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C5F RID: 7263 RVA: 0x000815A2 File Offset: 0x0007F7A2
		protected void VlandiaBaronsRetainerOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 1, SandboxCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
		}

		// Token: 0x06001C60 RID: 7264 RVA: 0x000815B9 File Offset: 0x0007F7B9
		protected void VlandiaMerchantOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 2, SandboxCharacterCreationContent.OccupationTypes.Merchant, "", "", true, true);
		}

		// Token: 0x06001C61 RID: 7265 RVA: 0x000815D0 File Offset: 0x0007F7D0
		protected void VlandiaYeomanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 3, SandboxCharacterCreationContent.OccupationTypes.Farmer, "", "", true, true);
		}

		// Token: 0x06001C62 RID: 7266 RVA: 0x000815E7 File Offset: 0x0007F7E7
		protected void VlandiaBlacksmithOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 4, SandboxCharacterCreationContent.OccupationTypes.Artisan, "", "", true, true);
		}

		// Token: 0x06001C63 RID: 7267 RVA: 0x000815FE File Offset: 0x0007F7FE
		protected void VlandiaHunterOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 5, SandboxCharacterCreationContent.OccupationTypes.Hunter, "", "", true, true);
		}

		// Token: 0x06001C64 RID: 7268 RVA: 0x00081615 File Offset: 0x0007F815
		protected void VlandiaMercenaryOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 6, SandboxCharacterCreationContent.OccupationTypes.Mercenary, "", "", true, true);
		}

		// Token: 0x06001C65 RID: 7269 RVA: 0x0008162C File Offset: 0x0007F82C
		protected void VlandiaBaronsRetainerOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C66 RID: 7270 RVA: 0x00081634 File Offset: 0x0007F834
		protected void VlandiaMerchantOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C67 RID: 7271 RVA: 0x0008163C File Offset: 0x0007F83C
		protected void VlandiaYeomanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C68 RID: 7272 RVA: 0x00081644 File Offset: 0x0007F844
		protected void VlandiaBlacksmithOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C69 RID: 7273 RVA: 0x0008164C File Offset: 0x0007F84C
		protected void VlandiaHunterOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C6A RID: 7274 RVA: 0x00081654 File Offset: 0x0007F854
		protected void VlandiaMercenaryOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C6B RID: 7275 RVA: 0x0008165C File Offset: 0x0007F85C
		protected void SturgiaBoyarsCompanionOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 1, SandboxCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
		}

		// Token: 0x06001C6C RID: 7276 RVA: 0x00081673 File Offset: 0x0007F873
		protected void SturgiaTraderOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 2, SandboxCharacterCreationContent.OccupationTypes.Merchant, "", "", true, true);
		}

		// Token: 0x06001C6D RID: 7277 RVA: 0x0008168A File Offset: 0x0007F88A
		protected void SturgiaFreemanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 3, SandboxCharacterCreationContent.OccupationTypes.Farmer, "", "", true, true);
		}

		// Token: 0x06001C6E RID: 7278 RVA: 0x000816A1 File Offset: 0x0007F8A1
		protected void SturgiaArtisanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 4, SandboxCharacterCreationContent.OccupationTypes.Artisan, "", "", true, true);
		}

		// Token: 0x06001C6F RID: 7279 RVA: 0x000816B8 File Offset: 0x0007F8B8
		protected void SturgiaHunterOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 5, SandboxCharacterCreationContent.OccupationTypes.Hunter, "", "", true, true);
		}

		// Token: 0x06001C70 RID: 7280 RVA: 0x000816CF File Offset: 0x0007F8CF
		protected void SturgiaVagabondOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 6, SandboxCharacterCreationContent.OccupationTypes.Vagabond, "", "", true, true);
		}

		// Token: 0x06001C71 RID: 7281 RVA: 0x000816E6 File Offset: 0x0007F8E6
		protected void SturgiaBoyarsCompanionOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C72 RID: 7282 RVA: 0x000816EE File Offset: 0x0007F8EE
		protected void SturgiaTraderOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C73 RID: 7283 RVA: 0x000816F6 File Offset: 0x0007F8F6
		protected void SturgiaFreemanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C74 RID: 7284 RVA: 0x000816FE File Offset: 0x0007F8FE
		protected void SturgiaArtisanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C75 RID: 7285 RVA: 0x00081706 File Offset: 0x0007F906
		protected void SturgiaHunterOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C76 RID: 7286 RVA: 0x0008170E File Offset: 0x0007F90E
		protected void SturgiaVagabondOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C77 RID: 7287 RVA: 0x00081716 File Offset: 0x0007F916
		protected void AseraiTribesmanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 1, SandboxCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
		}

		// Token: 0x06001C78 RID: 7288 RVA: 0x0008172D File Offset: 0x0007F92D
		protected void AseraiWariorSlaveOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 2, SandboxCharacterCreationContent.OccupationTypes.Mercenary, "", "", true, true);
		}

		// Token: 0x06001C79 RID: 7289 RVA: 0x00081744 File Offset: 0x0007F944
		protected void AseraiMerchantOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 3, SandboxCharacterCreationContent.OccupationTypes.Merchant, "", "", true, true);
		}

		// Token: 0x06001C7A RID: 7290 RVA: 0x0008175B File Offset: 0x0007F95B
		protected void AseraiOasisFarmerOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 4, SandboxCharacterCreationContent.OccupationTypes.Farmer, "", "", true, true);
		}

		// Token: 0x06001C7B RID: 7291 RVA: 0x00081772 File Offset: 0x0007F972
		protected void AseraiBedouinOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 5, SandboxCharacterCreationContent.OccupationTypes.Herder, "", "", true, true);
		}

		// Token: 0x06001C7C RID: 7292 RVA: 0x00081789 File Offset: 0x0007F989
		protected void AseraiBackAlleyThugOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 6, SandboxCharacterCreationContent.OccupationTypes.Artisan, "", "", true, true);
		}

		// Token: 0x06001C7D RID: 7293 RVA: 0x000817A0 File Offset: 0x0007F9A0
		protected void AseraiTribesmanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C7E RID: 7294 RVA: 0x000817A8 File Offset: 0x0007F9A8
		protected void AseraiWariorSlaveOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C7F RID: 7295 RVA: 0x000817B0 File Offset: 0x0007F9B0
		protected void AseraiMerchantOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C80 RID: 7296 RVA: 0x000817B8 File Offset: 0x0007F9B8
		protected void AseraiOasisFarmerOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C81 RID: 7297 RVA: 0x000817C0 File Offset: 0x0007F9C0
		protected void AseraiBedouinOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C82 RID: 7298 RVA: 0x000817C8 File Offset: 0x0007F9C8
		protected void AseraiBackAlleyThugOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C83 RID: 7299 RVA: 0x000817D0 File Offset: 0x0007F9D0
		protected void BattaniaChieftainsHearthguardOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 1, SandboxCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
		}

		// Token: 0x06001C84 RID: 7300 RVA: 0x000817E7 File Offset: 0x0007F9E7
		protected void BattaniaHealerOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 2, SandboxCharacterCreationContent.OccupationTypes.Healer, "", "", true, true);
		}

		// Token: 0x06001C85 RID: 7301 RVA: 0x000817FF File Offset: 0x0007F9FF
		protected void BattaniaTribesmanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 3, SandboxCharacterCreationContent.OccupationTypes.Farmer, "", "", true, true);
		}

		// Token: 0x06001C86 RID: 7302 RVA: 0x00081816 File Offset: 0x0007FA16
		protected void BattaniaSmithOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 4, SandboxCharacterCreationContent.OccupationTypes.Artisan, "", "", true, true);
		}

		// Token: 0x06001C87 RID: 7303 RVA: 0x0008182D File Offset: 0x0007FA2D
		protected void BattaniaWoodsmanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 5, SandboxCharacterCreationContent.OccupationTypes.Hunter, "", "", true, true);
		}

		// Token: 0x06001C88 RID: 7304 RVA: 0x00081844 File Offset: 0x0007FA44
		protected void BattaniaBardOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 6, SandboxCharacterCreationContent.OccupationTypes.Bard, "", "", true, true);
		}

		// Token: 0x06001C89 RID: 7305 RVA: 0x0008185B File Offset: 0x0007FA5B
		protected void BattaniaChieftainsHearthguardOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C8A RID: 7306 RVA: 0x00081863 File Offset: 0x0007FA63
		protected void BattaniaHealerOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C8B RID: 7307 RVA: 0x0008186B File Offset: 0x0007FA6B
		protected void BattaniaTribesmanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C8C RID: 7308 RVA: 0x00081873 File Offset: 0x0007FA73
		protected void BattaniaSmithOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C8D RID: 7309 RVA: 0x0008187B File Offset: 0x0007FA7B
		protected void BattaniaWoodsmanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C8E RID: 7310 RVA: 0x00081883 File Offset: 0x0007FA83
		protected void BattaniaBardOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C8F RID: 7311 RVA: 0x0008188B File Offset: 0x0007FA8B
		protected void KhuzaitNoyansKinsmanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 1, SandboxCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
		}

		// Token: 0x06001C90 RID: 7312 RVA: 0x000818A2 File Offset: 0x0007FAA2
		protected void KhuzaitMerchantOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 2, SandboxCharacterCreationContent.OccupationTypes.Merchant, "", "", true, true);
		}

		// Token: 0x06001C91 RID: 7313 RVA: 0x000818B9 File Offset: 0x0007FAB9
		protected void KhuzaitTribesmanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 3, SandboxCharacterCreationContent.OccupationTypes.Herder, "", "", true, true);
		}

		// Token: 0x06001C92 RID: 7314 RVA: 0x000818D0 File Offset: 0x0007FAD0
		protected void KhuzaitFarmerOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 4, SandboxCharacterCreationContent.OccupationTypes.Farmer, "", "", true, true);
		}

		// Token: 0x06001C93 RID: 7315 RVA: 0x000818E7 File Offset: 0x0007FAE7
		protected void KhuzaitShamanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 5, SandboxCharacterCreationContent.OccupationTypes.Healer, "", "", true, true);
		}

		// Token: 0x06001C94 RID: 7316 RVA: 0x000818FF File Offset: 0x0007FAFF
		protected void KhuzaitNomadOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 6, SandboxCharacterCreationContent.OccupationTypes.Herder, "", "", true, true);
		}

		// Token: 0x06001C95 RID: 7317 RVA: 0x00081916 File Offset: 0x0007FB16
		protected void KhuzaitNoyansKinsmanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C96 RID: 7318 RVA: 0x0008191E File Offset: 0x0007FB1E
		protected void KhuzaitMerchantOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C97 RID: 7319 RVA: 0x00081926 File Offset: 0x0007FB26
		protected void KhuzaitTribesmanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C98 RID: 7320 RVA: 0x0008192E File Offset: 0x0007FB2E
		protected void KhuzaitFarmerOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C99 RID: 7321 RVA: 0x00081936 File Offset: 0x0007FB36
		protected void KhuzaitShamanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C9A RID: 7322 RVA: 0x0008193E File Offset: 0x0007FB3E
		protected void KhuzaitNomadOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		// Token: 0x06001C9B RID: 7323 RVA: 0x00081946 File Offset: 0x0007FB46
		protected bool EmpireParentsOnCondition()
		{
			return base.GetSelectedCulture().StringId == "empire";
		}

		// Token: 0x06001C9C RID: 7324 RVA: 0x0008195D File Offset: 0x0007FB5D
		protected bool VlandianParentsOnCondition()
		{
			return base.GetSelectedCulture().StringId == "vlandia";
		}

		// Token: 0x06001C9D RID: 7325 RVA: 0x00081974 File Offset: 0x0007FB74
		protected bool SturgianParentsOnCondition()
		{
			return base.GetSelectedCulture().StringId == "sturgia";
		}

		// Token: 0x06001C9E RID: 7326 RVA: 0x0008198B File Offset: 0x0007FB8B
		protected bool AseraiParentsOnCondition()
		{
			return base.GetSelectedCulture().StringId == "aserai";
		}

		// Token: 0x06001C9F RID: 7327 RVA: 0x000819A2 File Offset: 0x0007FBA2
		protected bool BattanianParentsOnCondition()
		{
			return base.GetSelectedCulture().StringId == "battania";
		}

		// Token: 0x06001CA0 RID: 7328 RVA: 0x000819B9 File Offset: 0x0007FBB9
		protected bool KhuzaitParentsOnCondition()
		{
			return base.GetSelectedCulture().StringId == "khuzait";
		}

		// Token: 0x06001CA1 RID: 7329 RVA: 0x000819D0 File Offset: 0x0007FBD0
		protected void FinalizeParents()
		{
			CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>("main_hero_mother");
			CharacterObject object2 = Game.Current.ObjectManager.GetObject<CharacterObject>("main_hero_father");
			@object.HeroObject.ModifyPlayersFamilyAppearance(base.MotherFacegenCharacter.BodyProperties.StaticProperties);
			object2.HeroObject.ModifyPlayersFamilyAppearance(base.FatherFacegenCharacter.BodyProperties.StaticProperties);
			@object.HeroObject.Weight = base.MotherFacegenCharacter.BodyProperties.Weight;
			@object.HeroObject.Build = base.MotherFacegenCharacter.BodyProperties.Build;
			object2.HeroObject.Weight = base.FatherFacegenCharacter.BodyProperties.Weight;
			object2.HeroObject.Build = base.FatherFacegenCharacter.BodyProperties.Build;
			EquipmentHelper.AssignHeroEquipmentFromEquipment(@object.HeroObject, base.MotherFacegenCharacter.Equipment);
			EquipmentHelper.AssignHeroEquipmentFromEquipment(object2.HeroObject, base.FatherFacegenCharacter.Equipment);
			@object.Culture = Hero.MainHero.Culture;
			object2.Culture = Hero.MainHero.Culture;
			StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, null, false);
			TextObject textObject = GameTexts.FindText("str_player_father_name", Hero.MainHero.Culture.StringId);
			object2.HeroObject.SetName(textObject, textObject);
			TextObject textObject2 = new TextObject("{=XmvaRfLM}{PLAYER_FATHER.NAME} was the father of {PLAYER.LINK}. He was slain when raiders attacked the inn at which his family was staying.", null);
			StringHelpers.SetCharacterProperties("PLAYER_FATHER", object2, textObject2, false);
			object2.HeroObject.EncyclopediaText = textObject2;
			TextObject textObject3 = GameTexts.FindText("str_player_mother_name", Hero.MainHero.Culture.StringId);
			@object.HeroObject.SetName(textObject3, textObject3);
			TextObject textObject4 = new TextObject("{=hrhvEWP8}{PLAYER_MOTHER.NAME} was the mother of {PLAYER.LINK}. She was slain when raiders attacked the inn at which her family was staying.", null);
			StringHelpers.SetCharacterProperties("PLAYER_MOTHER", @object, textObject4, false);
			@object.HeroObject.EncyclopediaText = textObject4;
			@object.HeroObject.UpdateHomeSettlement();
			object2.HeroObject.UpdateHomeSettlement();
			@object.HeroObject.SetHasMet();
			object2.HeroObject.SetHasMet();
		}

		// Token: 0x06001CA2 RID: 7330 RVA: 0x00081BEC File Offset: 0x0007FDEC
		protected static List<FaceGenChar> ChangePlayerFaceWithAge(float age, string actionName = "act_childhood_schooled")
		{
			List<FaceGenChar> list = new List<FaceGenChar>();
			BodyProperties bodyProperties = CharacterObject.PlayerCharacter.GetBodyProperties(CharacterObject.PlayerCharacter.Equipment, -1);
			bodyProperties = FaceGen.GetBodyPropertiesWithAge(ref bodyProperties, age);
			list.Add(new FaceGenChar(bodyProperties, CharacterObject.PlayerCharacter.Race, new Equipment(), CharacterObject.PlayerCharacter.IsFemale, actionName));
			return list;
		}

		// Token: 0x06001CA3 RID: 7331 RVA: 0x00081C44 File Offset: 0x0007FE44
		protected Equipment ChangePlayerOutfit(CharacterCreation characterCreation, string outfit)
		{
			List<Equipment> list = new List<Equipment>();
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(outfit);
			Equipment equipment = ((@object != null) ? @object.DefaultEquipment : null);
			if (equipment == null)
			{
				Debug.FailedAssert("item shouldn't be null!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\CharacterCreationContent\\SandboxCharacterCreationContent.cs", "ChangePlayerOutfit", 1048);
				equipment = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>("player_char_creation_default").DefaultEquipment;
			}
			list.Add(equipment);
			characterCreation.ChangeCharactersEquipment(list);
			return equipment;
		}

		// Token: 0x06001CA4 RID: 7332 RVA: 0x00081CBC File Offset: 0x0007FEBC
		protected static void ChangePlayerMount(CharacterCreation characterCreation, Hero hero)
		{
			if (hero.CharacterObject.HasMount())
			{
				FaceGenMount faceGenMount = new FaceGenMount(MountCreationKey.GetRandomMountKey(hero.CharacterObject.Equipment[EquipmentIndex.ArmorItemEndSlot].Item, hero.CharacterObject.GetMountKeySeed()), hero.CharacterObject.Equipment[EquipmentIndex.ArmorItemEndSlot].Item, hero.CharacterObject.Equipment[EquipmentIndex.HorseHarness].Item, "act_horse_stand_1");
				characterCreation.SetFaceGenMount(faceGenMount);
			}
		}

		// Token: 0x06001CA5 RID: 7333 RVA: 0x00081D46 File Offset: 0x0007FF46
		protected static void ClearMountEntity(CharacterCreation characterCreation)
		{
			characterCreation.ClearFaceGenMounts();
		}

		// Token: 0x06001CA6 RID: 7334 RVA: 0x00081D50 File Offset: 0x0007FF50
		protected void ChildhoodOnInit(CharacterCreation characterCreation)
		{
			characterCreation.IsPlayerAlone = true;
			characterCreation.HasSecondaryCharacter = false;
			characterCreation.ClearFaceGenPrefab();
			characterCreation.ChangeFaceGenChars(SandboxCharacterCreationContent.ChangePlayerFaceWithAge((float)this.ChildhoodAge, "act_childhood_schooled"));
			string text = string.Concat(new object[]
			{
				"player_char_creation_childhood_age_",
				base.GetSelectedCulture().StringId,
				"_",
				base.SelectedParentType
			});
			text += (Hero.MainHero.IsFemale ? "_f" : "_m");
			this.ChangePlayerOutfit(characterCreation, text);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_schooled" });
			SandboxCharacterCreationContent.ClearMountEntity(characterCreation);
		}

		// Token: 0x06001CA7 RID: 7335 RVA: 0x00081E04 File Offset: 0x00080004
		protected static void ChildhoodYourLeadershipSkillsOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_leader" });
		}

		// Token: 0x06001CA8 RID: 7336 RVA: 0x00081E1C File Offset: 0x0008001C
		protected static void ChildhoodYourBrawnOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_athlete" });
		}

		// Token: 0x06001CA9 RID: 7337 RVA: 0x00081E34 File Offset: 0x00080034
		protected static void ChildhoodAttentionToDetailOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_memory" });
		}

		// Token: 0x06001CAA RID: 7338 RVA: 0x00081E4C File Offset: 0x0008004C
		protected static void ChildhoodAptitudeForNumbersOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_numbers" });
		}

		// Token: 0x06001CAB RID: 7339 RVA: 0x00081E64 File Offset: 0x00080064
		protected static void ChildhoodWayWithPeopleOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_manners" });
		}

		// Token: 0x06001CAC RID: 7340 RVA: 0x00081E7C File Offset: 0x0008007C
		protected static void ChildhoodSkillsWithHorsesOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_animals" });
		}

		// Token: 0x06001CAD RID: 7341 RVA: 0x00081E94 File Offset: 0x00080094
		protected static void ChildhoodGoodLeadingOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CAE RID: 7342 RVA: 0x00081E96 File Offset: 0x00080096
		protected static void ChildhoodGoodAthleticsOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CAF RID: 7343 RVA: 0x00081E98 File Offset: 0x00080098
		protected static void ChildhoodGoodMemoryOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CB0 RID: 7344 RVA: 0x00081E9A File Offset: 0x0008009A
		protected static void ChildhoodGoodMathOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CB1 RID: 7345 RVA: 0x00081E9C File Offset: 0x0008009C
		protected static void ChildhoodGoodMannersOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CB2 RID: 7346 RVA: 0x00081E9E File Offset: 0x0008009E
		protected static void ChildhoodAffinityWithAnimalsOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CB3 RID: 7347 RVA: 0x00081EA0 File Offset: 0x000800A0
		protected void EducationOnInit(CharacterCreation characterCreation)
		{
			characterCreation.IsPlayerAlone = true;
			characterCreation.HasSecondaryCharacter = false;
			characterCreation.ClearFaceGenPrefab();
			TextObject textObject = new TextObject("{=WYvnWcXQ}Like all village children you helped out in the fields. You also...", null);
			TextObject textObject2 = new TextObject("{=DsCkf6Pb}Growing up, you spent most of your time...", null);
			this._educationIntroductoryText.SetTextVariable("EDUCATION_INTRO", this.RuralType() ? textObject : textObject2);
			characterCreation.ChangeFaceGenChars(SandboxCharacterCreationContent.ChangePlayerFaceWithAge((float)this.EducationAge, "act_childhood_schooled"));
			string text = string.Concat(new object[]
			{
				"player_char_creation_education_age_",
				base.GetSelectedCulture().StringId,
				"_",
				base.SelectedParentType
			});
			text += (Hero.MainHero.IsFemale ? "_f" : "_m");
			this.ChangePlayerOutfit(characterCreation, text);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_schooled" });
			SandboxCharacterCreationContent.ClearMountEntity(characterCreation);
		}

		// Token: 0x06001CB4 RID: 7348 RVA: 0x00081F8C File Offset: 0x0008018C
		protected bool RuralType()
		{
			return this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Retainer || this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Farmer || this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Hunter || this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Bard || this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Herder || this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Vagabond || this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Healer || this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Artisan;
		}

		// Token: 0x06001CB5 RID: 7349 RVA: 0x00081FE4 File Offset: 0x000801E4
		protected bool RichParents()
		{
			return this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Retainer || this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Merchant;
		}

		// Token: 0x06001CB6 RID: 7350 RVA: 0x00081FFA File Offset: 0x000801FA
		protected bool RuralAdolescenceOnCondition()
		{
			return this.RuralType();
		}

		// Token: 0x06001CB7 RID: 7351 RVA: 0x00082002 File Offset: 0x00080202
		protected bool UrbanAdolescenceOnCondition()
		{
			return !this.RuralType();
		}

		// Token: 0x06001CB8 RID: 7352 RVA: 0x0008200D File Offset: 0x0008020D
		protected bool UrbanRichAdolescenceOnCondition()
		{
			return !this.RuralType() && this.RichParents();
		}

		// Token: 0x06001CB9 RID: 7353 RVA: 0x0008201F File Offset: 0x0008021F
		protected bool UrbanPoorAdolescenceOnCondition()
		{
			return !this.RuralType() && !this.RichParents();
		}

		// Token: 0x06001CBA RID: 7354 RVA: 0x00082034 File Offset: 0x00080234
		protected void RefreshPropsAndClothing(CharacterCreation characterCreation, bool isChildhoodStage, string itemId, bool isLeftHand, string secondItemId = "")
		{
			characterCreation.ClearFaceGenPrefab();
			characterCreation.ClearCharactersEquipment();
			string text = (isChildhoodStage ? string.Concat(new object[]
			{
				"player_char_creation_childhood_age_",
				base.GetSelectedCulture().StringId,
				"_",
				base.SelectedParentType
			}) : string.Concat(new object[]
			{
				"player_char_creation_education_age_",
				base.GetSelectedCulture().StringId,
				"_",
				base.SelectedParentType
			}));
			text += (Hero.MainHero.IsFemale ? "_f" : "_m");
			Equipment equipment = this.ChangePlayerOutfit(characterCreation, text).Clone(false);
			if (Game.Current.ObjectManager.GetObject<ItemObject>(itemId) != null)
			{
				ItemObject itemObject = Game.Current.ObjectManager.GetObject<ItemObject>(itemId);
				equipment.AddEquipmentToSlotWithoutAgent(isLeftHand ? EquipmentIndex.WeaponItemBeginSlot : EquipmentIndex.Weapon1, new EquipmentElement(itemObject, null, null, false));
				if (secondItemId != "")
				{
					itemObject = Game.Current.ObjectManager.GetObject<ItemObject>(secondItemId);
					equipment.AddEquipmentToSlotWithoutAgent(isLeftHand ? EquipmentIndex.Weapon1 : EquipmentIndex.WeaponItemBeginSlot, new EquipmentElement(itemObject, null, null, false));
				}
			}
			else
			{
				Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(characterCreation.FaceGenChars[0].Race);
				characterCreation.ChangeCharacterPrefab(itemId, isLeftHand ? baseMonsterFromRace.MainHandItemBoneIndex : baseMonsterFromRace.OffHandItemBoneIndex);
			}
			characterCreation.ChangeCharactersEquipment(new List<Equipment> { equipment });
		}

		// Token: 0x06001CBB RID: 7355 RVA: 0x000821A9 File Offset: 0x000803A9
		protected void RuralAdolescenceHerderOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_streets" });
			this.RefreshPropsAndClothing(characterCreation, false, "carry_bostaff_rogue1", true, "");
		}

		// Token: 0x06001CBC RID: 7356 RVA: 0x000821D4 File Offset: 0x000803D4
		protected void RuralAdolescenceSmithyOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_militia" });
			this.RefreshPropsAndClothing(characterCreation, false, "peasant_hammer_1_t1", true, "");
		}

		// Token: 0x06001CBD RID: 7357 RVA: 0x000821FF File Offset: 0x000803FF
		protected void RuralAdolescenceRepairmanOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_grit" });
			this.RefreshPropsAndClothing(characterCreation, false, "carry_hammer", true, "");
		}

		// Token: 0x06001CBE RID: 7358 RVA: 0x0008222A File Offset: 0x0008042A
		protected void RuralAdolescenceGathererOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_peddlers" });
			this.RefreshPropsAndClothing(characterCreation, false, "_to_carry_bd_basket_a", true, "");
		}

		// Token: 0x06001CBF RID: 7359 RVA: 0x00082255 File Offset: 0x00080455
		protected void RuralAdolescenceHunterOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_sharp" });
			this.RefreshPropsAndClothing(characterCreation, false, "composite_bow", true, "");
		}

		// Token: 0x06001CC0 RID: 7360 RVA: 0x00082280 File Offset: 0x00080480
		protected void RuralAdolescenceHelperOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_peddlers_2" });
			this.RefreshPropsAndClothing(characterCreation, false, "_to_carry_bd_fabric_c", true, "");
		}

		// Token: 0x06001CC1 RID: 7361 RVA: 0x000822AB File Offset: 0x000804AB
		protected void UrbanAdolescenceWatcherOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_fox" });
			this.RefreshPropsAndClothing(characterCreation, false, "", true, "");
		}

		// Token: 0x06001CC2 RID: 7362 RVA: 0x000822D6 File Offset: 0x000804D6
		protected void UrbanAdolescenceMarketerOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_manners" });
			this.RefreshPropsAndClothing(characterCreation, false, "", true, "");
		}

		// Token: 0x06001CC3 RID: 7363 RVA: 0x00082301 File Offset: 0x00080501
		protected void UrbanAdolescenceGangerOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_athlete" });
			this.RefreshPropsAndClothing(characterCreation, false, "", true, "");
		}

		// Token: 0x06001CC4 RID: 7364 RVA: 0x0008232C File Offset: 0x0008052C
		protected void UrbanAdolescenceDockerOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_peddlers" });
			this.RefreshPropsAndClothing(characterCreation, false, "_to_carry_bd_basket_a", true, "");
		}

		// Token: 0x06001CC5 RID: 7365 RVA: 0x00082357 File Offset: 0x00080557
		protected void UrbanAdolescenceHorserOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_peddlers_2" });
			this.RefreshPropsAndClothing(characterCreation, false, "_to_carry_bd_fabric_c", true, "");
		}

		// Token: 0x06001CC6 RID: 7366 RVA: 0x00082382 File Offset: 0x00080582
		protected void UrbanAdolescenceTutorOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_book" });
			this.RefreshPropsAndClothing(characterCreation, false, "character_creation_notebook", false, "");
		}

		// Token: 0x06001CC7 RID: 7367 RVA: 0x000823AD File Offset: 0x000805AD
		protected static void RuralAdolescenceHerderOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CC8 RID: 7368 RVA: 0x000823AF File Offset: 0x000805AF
		protected static void RuralAdolescenceSmithyOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CC9 RID: 7369 RVA: 0x000823B1 File Offset: 0x000805B1
		protected static void RuralAdolescenceRepairmanOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CCA RID: 7370 RVA: 0x000823B3 File Offset: 0x000805B3
		protected static void RuralAdolescenceGathererOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CCB RID: 7371 RVA: 0x000823B5 File Offset: 0x000805B5
		protected static void RuralAdolescenceHunterOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CCC RID: 7372 RVA: 0x000823B7 File Offset: 0x000805B7
		protected static void RuralAdolescenceHelperOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CCD RID: 7373 RVA: 0x000823B9 File Offset: 0x000805B9
		protected static void UrbanAdolescenceWatcherOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CCE RID: 7374 RVA: 0x000823BB File Offset: 0x000805BB
		protected static void UrbanAdolescenceMarketerOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CCF RID: 7375 RVA: 0x000823BD File Offset: 0x000805BD
		protected static void UrbanAdolescenceGangerOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CD0 RID: 7376 RVA: 0x000823BF File Offset: 0x000805BF
		protected static void UrbanAdolescenceDockerOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CD1 RID: 7377 RVA: 0x000823C4 File Offset: 0x000805C4
		protected void YouthOnInit(CharacterCreation characterCreation)
		{
			characterCreation.IsPlayerAlone = true;
			characterCreation.HasSecondaryCharacter = false;
			characterCreation.ClearFaceGenPrefab();
			TextObject textObject = new TextObject("{=F7OO5SAa}As a youngster growing up in Calradia, war was never too far away. You...", null);
			TextObject textObject2 = new TextObject("{=5kbeAC7k}In wartorn Calradia, especially in frontier or tribal areas, some women as well as men learn to fight from an early age. You...", null);
			this._youthIntroductoryText.SetTextVariable("YOUTH_INTRO", CharacterObject.PlayerCharacter.IsFemale ? textObject2 : textObject);
			characterCreation.ChangeFaceGenChars(SandboxCharacterCreationContent.ChangePlayerFaceWithAge((float)this.YouthAge, "act_childhood_schooled"));
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_schooled" });
			if (base.SelectedTitleType < 1 || base.SelectedTitleType > 10)
			{
				base.SelectedTitleType = 1;
			}
			this.RefreshPlayerAppearance(characterCreation);
		}

		// Token: 0x06001CD2 RID: 7378 RVA: 0x0008246C File Offset: 0x0008066C
		protected void RefreshPlayerAppearance(CharacterCreation characterCreation)
		{
			string text = string.Concat(new object[]
			{
				"player_char_creation_",
				base.GetSelectedCulture().StringId,
				"_",
				base.SelectedTitleType
			});
			text += (Hero.MainHero.IsFemale ? "_f" : "_m");
			this.ChangePlayerOutfit(characterCreation, text);
			this.ApplyEquipments(characterCreation);
		}

		// Token: 0x06001CD3 RID: 7379 RVA: 0x000824E0 File Offset: 0x000806E0
		protected bool YouthCommanderOnCondition()
		{
			return base.GetSelectedCulture().StringId == "empire" && this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Retainer;
		}

		// Token: 0x06001CD4 RID: 7380 RVA: 0x00082504 File Offset: 0x00080704
		protected void YouthCommanderOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CD5 RID: 7381 RVA: 0x00082506 File Offset: 0x00080706
		protected bool YouthGroomOnCondition()
		{
			return base.GetSelectedCulture().StringId == "vlandia" && this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Retainer;
		}

		// Token: 0x06001CD6 RID: 7382 RVA: 0x0008252A File Offset: 0x0008072A
		protected void YouthCommanderOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 10;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_decisive" });
		}

		// Token: 0x06001CD7 RID: 7383 RVA: 0x00082551 File Offset: 0x00080751
		protected void YouthGroomOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 10;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_sharp" });
		}

		// Token: 0x06001CD8 RID: 7384 RVA: 0x00082578 File Offset: 0x00080778
		protected void YouthChieftainOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 10;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_ready" });
		}

		// Token: 0x06001CD9 RID: 7385 RVA: 0x0008259F File Offset: 0x0008079F
		protected void YouthCavalryOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 9;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_apprentice" });
		}

		// Token: 0x06001CDA RID: 7386 RVA: 0x000825C6 File Offset: 0x000807C6
		protected void YouthHearthGuardOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 9;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_athlete" });
		}

		// Token: 0x06001CDB RID: 7387 RVA: 0x000825ED File Offset: 0x000807ED
		protected void YouthOutridersOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 2;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_gracious" });
		}

		// Token: 0x06001CDC RID: 7388 RVA: 0x00082613 File Offset: 0x00080813
		protected void YouthOtherOutridersOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 2;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_gracious" });
		}

		// Token: 0x06001CDD RID: 7389 RVA: 0x00082639 File Offset: 0x00080839
		protected void YouthInfantryOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 3;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_fierce" });
		}

		// Token: 0x06001CDE RID: 7390 RVA: 0x0008265F File Offset: 0x0008085F
		protected void YouthSkirmisherOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 4;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_fox" });
		}

		// Token: 0x06001CDF RID: 7391 RVA: 0x00082685 File Offset: 0x00080885
		protected void YouthGarrisonOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 1;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_vibrant" });
		}

		// Token: 0x06001CE0 RID: 7392 RVA: 0x000826AB File Offset: 0x000808AB
		protected void YouthOtherGarrisonOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 1;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_sharp" });
		}

		// Token: 0x06001CE1 RID: 7393 RVA: 0x000826D1 File Offset: 0x000808D1
		protected void YouthKernOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 8;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_apprentice" });
		}

		// Token: 0x06001CE2 RID: 7394 RVA: 0x000826F7 File Offset: 0x000808F7
		protected void YouthCamperOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 5;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_militia" });
		}

		// Token: 0x06001CE3 RID: 7395 RVA: 0x0008271D File Offset: 0x0008091D
		protected void YouthGroomOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CE4 RID: 7396 RVA: 0x0008271F File Offset: 0x0008091F
		protected bool YouthChieftainOnCondition()
		{
			return (base.GetSelectedCulture().StringId == "battania" || base.GetSelectedCulture().StringId == "khuzait") && this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Retainer;
		}

		// Token: 0x06001CE5 RID: 7397 RVA: 0x0008275A File Offset: 0x0008095A
		protected void YouthChieftainOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CE6 RID: 7398 RVA: 0x0008275C File Offset: 0x0008095C
		protected bool YouthCavalryOnCondition()
		{
			return base.GetSelectedCulture().StringId == "empire" || base.GetSelectedCulture().StringId == "khuzait" || base.GetSelectedCulture().StringId == "aserai" || base.GetSelectedCulture().StringId == "vlandia";
		}

		// Token: 0x06001CE7 RID: 7399 RVA: 0x000827C5 File Offset: 0x000809C5
		protected void YouthCavalryOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CE8 RID: 7400 RVA: 0x000827C7 File Offset: 0x000809C7
		protected bool YouthHearthGuardOnCondition()
		{
			return base.GetSelectedCulture().StringId == "sturgia" || base.GetSelectedCulture().StringId == "battania";
		}

		// Token: 0x06001CE9 RID: 7401 RVA: 0x000827F7 File Offset: 0x000809F7
		protected void YouthHearthGuardOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CEA RID: 7402 RVA: 0x000827F9 File Offset: 0x000809F9
		protected bool YouthOutridersOnCondition()
		{
			return base.GetSelectedCulture().StringId == "empire" || base.GetSelectedCulture().StringId == "khuzait";
		}

		// Token: 0x06001CEB RID: 7403 RVA: 0x00082829 File Offset: 0x00080A29
		protected void YouthOutridersOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CEC RID: 7404 RVA: 0x0008282B File Offset: 0x00080A2B
		protected bool YouthOtherOutridersOnCondition()
		{
			return base.GetSelectedCulture().StringId != "empire" && base.GetSelectedCulture().StringId != "khuzait";
		}

		// Token: 0x06001CED RID: 7405 RVA: 0x0008285B File Offset: 0x00080A5B
		protected void YouthOtherOutridersOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CEE RID: 7406 RVA: 0x0008285D File Offset: 0x00080A5D
		protected void YouthInfantryOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CEF RID: 7407 RVA: 0x0008285F File Offset: 0x00080A5F
		protected void YouthSkirmisherOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CF0 RID: 7408 RVA: 0x00082861 File Offset: 0x00080A61
		protected bool YouthGarrisonOnCondition()
		{
			return base.GetSelectedCulture().StringId == "empire" || base.GetSelectedCulture().StringId == "vlandia";
		}

		// Token: 0x06001CF1 RID: 7409 RVA: 0x00082891 File Offset: 0x00080A91
		protected void YouthGarrisonOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CF2 RID: 7410 RVA: 0x00082893 File Offset: 0x00080A93
		protected bool YouthOtherGarrisonOnCondition()
		{
			return base.GetSelectedCulture().StringId != "empire" && base.GetSelectedCulture().StringId != "vlandia";
		}

		// Token: 0x06001CF3 RID: 7411 RVA: 0x000828C3 File Offset: 0x00080AC3
		protected void YouthOtherGarrisonOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CF4 RID: 7412 RVA: 0x000828C5 File Offset: 0x00080AC5
		protected bool YouthSkirmisherOnCondition()
		{
			return base.GetSelectedCulture().StringId != "battania";
		}

		// Token: 0x06001CF5 RID: 7413 RVA: 0x000828DC File Offset: 0x00080ADC
		protected bool YouthKernOnCondition()
		{
			return base.GetSelectedCulture().StringId == "battania";
		}

		// Token: 0x06001CF6 RID: 7414 RVA: 0x000828F3 File Offset: 0x00080AF3
		protected void YouthKernOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CF7 RID: 7415 RVA: 0x000828F5 File Offset: 0x00080AF5
		protected bool YouthCamperOnCondition()
		{
			return this._familyOccupationType != SandboxCharacterCreationContent.OccupationTypes.Retainer;
		}

		// Token: 0x06001CF8 RID: 7416 RVA: 0x00082903 File Offset: 0x00080B03
		protected void YouthCamperOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CF9 RID: 7417 RVA: 0x00082908 File Offset: 0x00080B08
		protected void AccomplishmentOnInit(CharacterCreation characterCreation)
		{
			characterCreation.IsPlayerAlone = true;
			characterCreation.HasSecondaryCharacter = false;
			characterCreation.ClearFaceGenPrefab();
			characterCreation.ChangeFaceGenChars(SandboxCharacterCreationContent.ChangePlayerFaceWithAge((float)this.AccomplishmentAge, "act_childhood_schooled"));
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_schooled" });
			this.RefreshPlayerAppearance(characterCreation);
		}

		// Token: 0x06001CFA RID: 7418 RVA: 0x0008295D File Offset: 0x00080B5D
		protected void AccomplishmentDefeatedEnemyOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CFB RID: 7419 RVA: 0x0008295F File Offset: 0x00080B5F
		protected void AccomplishmentExpeditionOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001CFC RID: 7420 RVA: 0x00082961 File Offset: 0x00080B61
		protected bool AccomplishmentRuralOnCondition()
		{
			return this.RuralType();
		}

		// Token: 0x06001CFD RID: 7421 RVA: 0x00082969 File Offset: 0x00080B69
		protected bool AccomplishmentMerchantOnCondition()
		{
			return this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Merchant;
		}

		// Token: 0x06001CFE RID: 7422 RVA: 0x00082974 File Offset: 0x00080B74
		protected bool AccomplishmentPosseOnConditions()
		{
			return this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Retainer || this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Herder || this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Mercenary;
		}

		// Token: 0x06001CFF RID: 7423 RVA: 0x00082993 File Offset: 0x00080B93
		protected bool AccomplishmentSavedVillageOnCondition()
		{
			return this.RuralType() && this._familyOccupationType != SandboxCharacterCreationContent.OccupationTypes.Retainer && this._familyOccupationType != SandboxCharacterCreationContent.OccupationTypes.Herder;
		}

		// Token: 0x06001D00 RID: 7424 RVA: 0x000829B4 File Offset: 0x00080BB4
		protected bool AccomplishmentSavedStreetOnCondition()
		{
			return !this.RuralType() && this._familyOccupationType != SandboxCharacterCreationContent.OccupationTypes.Merchant && this._familyOccupationType != SandboxCharacterCreationContent.OccupationTypes.Mercenary;
		}

		// Token: 0x06001D01 RID: 7425 RVA: 0x000829D5 File Offset: 0x00080BD5
		protected bool AccomplishmentUrbanOnCondition()
		{
			return !this.RuralType();
		}

		// Token: 0x06001D02 RID: 7426 RVA: 0x000829E0 File Offset: 0x00080BE0
		protected void AccomplishmentWorkshopOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001D03 RID: 7427 RVA: 0x000829E2 File Offset: 0x00080BE2
		protected void AccomplishmentSiegeHunterOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001D04 RID: 7428 RVA: 0x000829E4 File Offset: 0x00080BE4
		protected void AccomplishmentEscapadeOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001D05 RID: 7429 RVA: 0x000829E6 File Offset: 0x00080BE6
		protected void AccomplishmentTreaterOnApply(CharacterCreation characterCreation)
		{
		}

		// Token: 0x06001D06 RID: 7430 RVA: 0x000829E8 File Offset: 0x00080BE8
		protected void AccomplishmentDefeatedEnemyOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_athlete" });
		}

		// Token: 0x06001D07 RID: 7431 RVA: 0x00082A00 File Offset: 0x00080C00
		protected void AccomplishmentExpeditionOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_gracious" });
		}

		// Token: 0x06001D08 RID: 7432 RVA: 0x00082A18 File Offset: 0x00080C18
		protected void AccomplishmentMerchantOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_ready" });
		}

		// Token: 0x06001D09 RID: 7433 RVA: 0x00082A30 File Offset: 0x00080C30
		protected void AccomplishmentSavedVillageOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_vibrant" });
		}

		// Token: 0x06001D0A RID: 7434 RVA: 0x00082A48 File Offset: 0x00080C48
		protected void AccomplishmentSavedStreetOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_vibrant" });
		}

		// Token: 0x06001D0B RID: 7435 RVA: 0x00082A60 File Offset: 0x00080C60
		protected void AccomplishmentWorkshopOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_decisive" });
		}

		// Token: 0x06001D0C RID: 7436 RVA: 0x00082A78 File Offset: 0x00080C78
		protected void AccomplishmentSiegeHunterOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_tough" });
		}

		// Token: 0x06001D0D RID: 7437 RVA: 0x00082A90 File Offset: 0x00080C90
		protected void AccomplishmentEscapadeOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_clever" });
		}

		// Token: 0x06001D0E RID: 7438 RVA: 0x00082AA8 File Offset: 0x00080CA8
		protected void AccomplishmentTreaterOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_manners" });
		}

		// Token: 0x06001D0F RID: 7439 RVA: 0x00082AC0 File Offset: 0x00080CC0
		protected void StartingAgeOnInit(CharacterCreation characterCreation)
		{
			characterCreation.IsPlayerAlone = true;
			characterCreation.HasSecondaryCharacter = false;
			characterCreation.ClearFaceGenPrefab();
			characterCreation.ChangeFaceGenChars(SandboxCharacterCreationContent.ChangePlayerFaceWithAge((float)this._startingAge, "act_childhood_schooled"));
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_schooled" });
			this.RefreshPlayerAppearance(characterCreation);
		}

		// Token: 0x06001D10 RID: 7440 RVA: 0x00082B18 File Offset: 0x00080D18
		protected void StartingAgeYoungOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ClearFaceGenPrefab();
			characterCreation.ChangeFaceGenChars(SandboxCharacterCreationContent.ChangePlayerFaceWithAge(20f, "act_childhood_schooled"));
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_focus" });
			this.RefreshPlayerAppearance(characterCreation);
			this._startingAge = SandboxCharacterCreationContent.SandboxAgeOptions.YoungAdult;
			this.SetHeroAge(20f);
		}

		// Token: 0x06001D11 RID: 7441 RVA: 0x00082B70 File Offset: 0x00080D70
		protected void StartingAgeAdultOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ClearFaceGenPrefab();
			characterCreation.ChangeFaceGenChars(SandboxCharacterCreationContent.ChangePlayerFaceWithAge(30f, "act_childhood_schooled"));
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_ready" });
			this.RefreshPlayerAppearance(characterCreation);
			this._startingAge = SandboxCharacterCreationContent.SandboxAgeOptions.Adult;
			this.SetHeroAge(30f);
		}

		// Token: 0x06001D12 RID: 7442 RVA: 0x00082BC8 File Offset: 0x00080DC8
		protected void StartingAgeMiddleAgedOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ClearFaceGenPrefab();
			characterCreation.ChangeFaceGenChars(SandboxCharacterCreationContent.ChangePlayerFaceWithAge(40f, "act_childhood_schooled"));
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_sharp" });
			this.RefreshPlayerAppearance(characterCreation);
			this._startingAge = SandboxCharacterCreationContent.SandboxAgeOptions.MiddleAged;
			this.SetHeroAge(40f);
		}

		// Token: 0x06001D13 RID: 7443 RVA: 0x00082C20 File Offset: 0x00080E20
		protected void StartingAgeElderlyOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ClearFaceGenPrefab();
			characterCreation.ChangeFaceGenChars(SandboxCharacterCreationContent.ChangePlayerFaceWithAge(50f, "act_childhood_schooled"));
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_tough" });
			this.RefreshPlayerAppearance(characterCreation);
			this._startingAge = SandboxCharacterCreationContent.SandboxAgeOptions.Elder;
			this.SetHeroAge(50f);
		}

		// Token: 0x06001D14 RID: 7444 RVA: 0x00082C78 File Offset: 0x00080E78
		protected void StartingAgeYoungOnApply(CharacterCreation characterCreation)
		{
			this._startingAge = SandboxCharacterCreationContent.SandboxAgeOptions.YoungAdult;
		}

		// Token: 0x06001D15 RID: 7445 RVA: 0x00082C82 File Offset: 0x00080E82
		protected void StartingAgeAdultOnApply(CharacterCreation characterCreation)
		{
			this._startingAge = SandboxCharacterCreationContent.SandboxAgeOptions.Adult;
		}

		// Token: 0x06001D16 RID: 7446 RVA: 0x00082C8C File Offset: 0x00080E8C
		protected void StartingAgeMiddleAgedOnApply(CharacterCreation characterCreation)
		{
			this._startingAge = SandboxCharacterCreationContent.SandboxAgeOptions.MiddleAged;
		}

		// Token: 0x06001D17 RID: 7447 RVA: 0x00082C96 File Offset: 0x00080E96
		protected void StartingAgeElderlyOnApply(CharacterCreation characterCreation)
		{
			this._startingAge = SandboxCharacterCreationContent.SandboxAgeOptions.Elder;
		}

		// Token: 0x06001D18 RID: 7448 RVA: 0x00082CA0 File Offset: 0x00080EA0
		protected void ApplyEquipments(CharacterCreation characterCreation)
		{
			SandboxCharacterCreationContent.ClearMountEntity(characterCreation);
			string text = string.Concat(new object[]
			{
				"player_char_creation_",
				base.GetSelectedCulture().StringId,
				"_",
				base.SelectedTitleType
			});
			text += (Hero.MainHero.IsFemale ? "_f" : "_m");
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(text);
			base.PlayerStartEquipment = ((@object != null) ? @object.DefaultEquipment : null) ?? MBEquipmentRoster.EmptyEquipment;
			base.PlayerCivilianEquipment = ((@object != null) ? @object.GetCivilianEquipments().FirstOrDefault<Equipment>() : null) ?? MBEquipmentRoster.EmptyEquipment;
			if (base.PlayerStartEquipment != null && base.PlayerCivilianEquipment != null)
			{
				CharacterObject.PlayerCharacter.Equipment.FillFrom(base.PlayerStartEquipment, true);
				CharacterObject.PlayerCharacter.FirstCivilianEquipment.FillFrom(base.PlayerCivilianEquipment, true);
			}
			SandboxCharacterCreationContent.ChangePlayerMount(characterCreation, Hero.MainHero);
		}

		// Token: 0x06001D19 RID: 7449 RVA: 0x00082D9D File Offset: 0x00080F9D
		protected void SetHeroAge(float age)
		{
			Hero.MainHero.SetBirthDay(CampaignTime.YearsFromNow(-age));
		}

		// Token: 0x040008F0 RID: 2288
		protected const int FocusToAddYouthStart = 2;

		// Token: 0x040008F1 RID: 2289
		protected const int FocusToAddAdultStart = 4;

		// Token: 0x040008F2 RID: 2290
		protected const int FocusToAddMiddleAgedStart = 6;

		// Token: 0x040008F3 RID: 2291
		protected const int FocusToAddElderlyStart = 8;

		// Token: 0x040008F4 RID: 2292
		protected const int AttributeToAddYouthStart = 1;

		// Token: 0x040008F5 RID: 2293
		protected const int AttributeToAddAdultStart = 2;

		// Token: 0x040008F6 RID: 2294
		protected const int AttributeToAddMiddleAgedStart = 3;

		// Token: 0x040008F7 RID: 2295
		protected const int AttributeToAddElderlyStart = 4;

		// Token: 0x040008F8 RID: 2296
		protected readonly Dictionary<string, Vec2> _startingPoints = new Dictionary<string, Vec2>
		{
			{
				"empire",
				new Vec2(657.95f, 279.08f)
			},
			{
				"sturgia",
				new Vec2(356.75f, 551.52f)
			},
			{
				"aserai",
				new Vec2(300.78f, 259.99f)
			},
			{
				"battania",
				new Vec2(293.64f, 446.39f)
			},
			{
				"khuzait",
				new Vec2(680.73f, 480.8f)
			},
			{
				"vlandia",
				new Vec2(207.04f, 389.04f)
			}
		};

		// Token: 0x040008F9 RID: 2297
		protected SandboxCharacterCreationContent.SandboxAgeOptions _startingAge = SandboxCharacterCreationContent.SandboxAgeOptions.YoungAdult;

		// Token: 0x040008FA RID: 2298
		protected SandboxCharacterCreationContent.OccupationTypes _familyOccupationType;

		// Token: 0x040008FB RID: 2299
		protected TextObject _educationIntroductoryText = new TextObject("{=!}{EDUCATION_INTRO}", null);

		// Token: 0x040008FC RID: 2300
		protected TextObject _youthIntroductoryText = new TextObject("{=!}{YOUTH_INTRO}", null);

		// Token: 0x02000561 RID: 1377
		protected enum SandboxAgeOptions
		{
			// Token: 0x040016BA RID: 5818
			YoungAdult = 20,
			// Token: 0x040016BB RID: 5819
			Adult = 30,
			// Token: 0x040016BC RID: 5820
			MiddleAged = 40,
			// Token: 0x040016BD RID: 5821
			Elder = 50
		}

		// Token: 0x02000562 RID: 1378
		protected enum OccupationTypes
		{
			// Token: 0x040016BF RID: 5823
			Artisan,
			// Token: 0x040016C0 RID: 5824
			Bard,
			// Token: 0x040016C1 RID: 5825
			Retainer,
			// Token: 0x040016C2 RID: 5826
			Merchant,
			// Token: 0x040016C3 RID: 5827
			Farmer,
			// Token: 0x040016C4 RID: 5828
			Hunter,
			// Token: 0x040016C5 RID: 5829
			Vagabond,
			// Token: 0x040016C6 RID: 5830
			Mercenary,
			// Token: 0x040016C7 RID: 5831
			Herder,
			// Token: 0x040016C8 RID: 5832
			Healer,
			// Token: 0x040016C9 RID: 5833
			NumberOfTypes
		}
	}
}

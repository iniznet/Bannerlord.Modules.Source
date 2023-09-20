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
	public class SandboxCharacterCreationContent : CharacterCreationContentBase
	{
		public override TextObject ReviewPageDescription
		{
			get
			{
				return new TextObject("{=W6pKpEoT}You prepare to set off for a grand adventure in Calradia! Here is your character. Continue if you are ready, or go back to make changes.", null);
			}
		}

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

		protected override void OnCultureSelected()
		{
			base.SelectedTitleType = 1;
			base.SelectedParentType = 0;
			TextObject textObject = FactionHelper.GenerateClanNameforPlayer();
			Clan.PlayerClan.ChangeClanName(textObject, textObject);
		}

		public override int GetSelectedParentType()
		{
			return base.SelectedParentType;
		}

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

		protected override void OnInitialized(CharacterCreation characterCreation)
		{
			this.AddParentsMenu(characterCreation);
			this.AddChildhoodMenu(characterCreation);
			this.AddEducationMenu(characterCreation);
			this.AddYouthMenu(characterCreation);
			this.AddAdulthoodMenu(characterCreation);
			this.AddAgeSelectionMenu(characterCreation);
		}

		protected override void OnApplyCulture()
		{
		}

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

		protected void ChangeParentsAnimation(CharacterCreation characterCreation)
		{
			List<string> list = new List<string>
			{
				"anim_mother_" + base.SelectedParentType,
				"anim_father_" + base.SelectedParentType
			};
			characterCreation.ChangeCharsAnimation(list);
		}

		protected void SetParentAndOccupationType(CharacterCreation characterCreation, int parentType, SandboxCharacterCreationContent.OccupationTypes occupationType, string fatherItemId = "", string motherItemId = "", bool isLeftHandItemForFather = true, bool isLeftHandItemForMother = true)
		{
			base.SelectedParentType = parentType;
			this._familyOccupationType = occupationType;
			characterCreation.ChangeFaceGenChars(new List<FaceGenChar> { base.MotherFacegenCharacter, base.FatherFacegenCharacter });
			this.ChangeParentsAnimation(characterCreation);
			this.ChangeParentsOutfit(characterCreation, fatherItemId, motherItemId, isLeftHandItemForFather, isLeftHandItemForMother);
		}

		protected void EmpireLandlordsRetainerOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 1, SandboxCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
		}

		protected void EmpireMerchantOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 2, SandboxCharacterCreationContent.OccupationTypes.Merchant, "", "", true, true);
		}

		protected void EmpireFreeholderOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 3, SandboxCharacterCreationContent.OccupationTypes.Farmer, "", "", true, true);
		}

		protected void EmpireArtisanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 4, SandboxCharacterCreationContent.OccupationTypes.Artisan, "", "", true, true);
		}

		protected void EmpireWoodsmanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 5, SandboxCharacterCreationContent.OccupationTypes.Hunter, "", "", true, true);
		}

		protected void EmpireVagabondOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 6, SandboxCharacterCreationContent.OccupationTypes.Vagabond, "", "", true, true);
		}

		protected void EmpireLandlordsRetainerOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void EmpireMerchantOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void EmpireFreeholderOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void EmpireArtisanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void EmpireWoodsmanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void EmpireVagabondOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void VlandiaBaronsRetainerOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 1, SandboxCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
		}

		protected void VlandiaMerchantOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 2, SandboxCharacterCreationContent.OccupationTypes.Merchant, "", "", true, true);
		}

		protected void VlandiaYeomanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 3, SandboxCharacterCreationContent.OccupationTypes.Farmer, "", "", true, true);
		}

		protected void VlandiaBlacksmithOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 4, SandboxCharacterCreationContent.OccupationTypes.Artisan, "", "", true, true);
		}

		protected void VlandiaHunterOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 5, SandboxCharacterCreationContent.OccupationTypes.Hunter, "", "", true, true);
		}

		protected void VlandiaMercenaryOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 6, SandboxCharacterCreationContent.OccupationTypes.Mercenary, "", "", true, true);
		}

		protected void VlandiaBaronsRetainerOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void VlandiaMerchantOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void VlandiaYeomanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void VlandiaBlacksmithOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void VlandiaHunterOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void VlandiaMercenaryOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void SturgiaBoyarsCompanionOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 1, SandboxCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
		}

		protected void SturgiaTraderOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 2, SandboxCharacterCreationContent.OccupationTypes.Merchant, "", "", true, true);
		}

		protected void SturgiaFreemanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 3, SandboxCharacterCreationContent.OccupationTypes.Farmer, "", "", true, true);
		}

		protected void SturgiaArtisanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 4, SandboxCharacterCreationContent.OccupationTypes.Artisan, "", "", true, true);
		}

		protected void SturgiaHunterOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 5, SandboxCharacterCreationContent.OccupationTypes.Hunter, "", "", true, true);
		}

		protected void SturgiaVagabondOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 6, SandboxCharacterCreationContent.OccupationTypes.Vagabond, "", "", true, true);
		}

		protected void SturgiaBoyarsCompanionOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void SturgiaTraderOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void SturgiaFreemanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void SturgiaArtisanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void SturgiaHunterOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void SturgiaVagabondOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void AseraiTribesmanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 1, SandboxCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
		}

		protected void AseraiWariorSlaveOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 2, SandboxCharacterCreationContent.OccupationTypes.Mercenary, "", "", true, true);
		}

		protected void AseraiMerchantOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 3, SandboxCharacterCreationContent.OccupationTypes.Merchant, "", "", true, true);
		}

		protected void AseraiOasisFarmerOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 4, SandboxCharacterCreationContent.OccupationTypes.Farmer, "", "", true, true);
		}

		protected void AseraiBedouinOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 5, SandboxCharacterCreationContent.OccupationTypes.Herder, "", "", true, true);
		}

		protected void AseraiBackAlleyThugOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 6, SandboxCharacterCreationContent.OccupationTypes.Artisan, "", "", true, true);
		}

		protected void AseraiTribesmanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void AseraiWariorSlaveOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void AseraiMerchantOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void AseraiOasisFarmerOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void AseraiBedouinOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void AseraiBackAlleyThugOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void BattaniaChieftainsHearthguardOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 1, SandboxCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
		}

		protected void BattaniaHealerOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 2, SandboxCharacterCreationContent.OccupationTypes.Healer, "", "", true, true);
		}

		protected void BattaniaTribesmanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 3, SandboxCharacterCreationContent.OccupationTypes.Farmer, "", "", true, true);
		}

		protected void BattaniaSmithOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 4, SandboxCharacterCreationContent.OccupationTypes.Artisan, "", "", true, true);
		}

		protected void BattaniaWoodsmanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 5, SandboxCharacterCreationContent.OccupationTypes.Hunter, "", "", true, true);
		}

		protected void BattaniaBardOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 6, SandboxCharacterCreationContent.OccupationTypes.Bard, "", "", true, true);
		}

		protected void BattaniaChieftainsHearthguardOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void BattaniaHealerOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void BattaniaTribesmanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void BattaniaSmithOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void BattaniaWoodsmanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void BattaniaBardOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void KhuzaitNoyansKinsmanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 1, SandboxCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
		}

		protected void KhuzaitMerchantOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 2, SandboxCharacterCreationContent.OccupationTypes.Merchant, "", "", true, true);
		}

		protected void KhuzaitTribesmanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 3, SandboxCharacterCreationContent.OccupationTypes.Herder, "", "", true, true);
		}

		protected void KhuzaitFarmerOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 4, SandboxCharacterCreationContent.OccupationTypes.Farmer, "", "", true, true);
		}

		protected void KhuzaitShamanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 5, SandboxCharacterCreationContent.OccupationTypes.Healer, "", "", true, true);
		}

		protected void KhuzaitNomadOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 6, SandboxCharacterCreationContent.OccupationTypes.Herder, "", "", true, true);
		}

		protected void KhuzaitNoyansKinsmanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void KhuzaitMerchantOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void KhuzaitTribesmanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void KhuzaitFarmerOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void KhuzaitShamanOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected void KhuzaitNomadOnApply(CharacterCreation characterCreation)
		{
			this.FinalizeParents();
		}

		protected bool EmpireParentsOnCondition()
		{
			return base.GetSelectedCulture().StringId == "empire";
		}

		protected bool VlandianParentsOnCondition()
		{
			return base.GetSelectedCulture().StringId == "vlandia";
		}

		protected bool SturgianParentsOnCondition()
		{
			return base.GetSelectedCulture().StringId == "sturgia";
		}

		protected bool AseraiParentsOnCondition()
		{
			return base.GetSelectedCulture().StringId == "aserai";
		}

		protected bool BattanianParentsOnCondition()
		{
			return base.GetSelectedCulture().StringId == "battania";
		}

		protected bool KhuzaitParentsOnCondition()
		{
			return base.GetSelectedCulture().StringId == "khuzait";
		}

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

		protected static List<FaceGenChar> ChangePlayerFaceWithAge(float age, string actionName = "act_childhood_schooled")
		{
			List<FaceGenChar> list = new List<FaceGenChar>();
			BodyProperties bodyProperties = CharacterObject.PlayerCharacter.GetBodyProperties(CharacterObject.PlayerCharacter.Equipment, -1);
			bodyProperties = FaceGen.GetBodyPropertiesWithAge(ref bodyProperties, age);
			list.Add(new FaceGenChar(bodyProperties, CharacterObject.PlayerCharacter.Race, new Equipment(), CharacterObject.PlayerCharacter.IsFemale, actionName));
			return list;
		}

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

		protected static void ChangePlayerMount(CharacterCreation characterCreation, Hero hero)
		{
			if (hero.CharacterObject.HasMount())
			{
				FaceGenMount faceGenMount = new FaceGenMount(MountCreationKey.GetRandomMountKey(hero.CharacterObject.Equipment[EquipmentIndex.ArmorItemEndSlot].Item, hero.CharacterObject.GetMountKeySeed()), hero.CharacterObject.Equipment[EquipmentIndex.ArmorItemEndSlot].Item, hero.CharacterObject.Equipment[EquipmentIndex.HorseHarness].Item, "act_horse_stand_1");
				characterCreation.SetFaceGenMount(faceGenMount);
			}
		}

		protected static void ClearMountEntity(CharacterCreation characterCreation)
		{
			characterCreation.ClearFaceGenMounts();
		}

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

		protected static void ChildhoodYourLeadershipSkillsOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_leader" });
		}

		protected static void ChildhoodYourBrawnOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_athlete" });
		}

		protected static void ChildhoodAttentionToDetailOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_memory" });
		}

		protected static void ChildhoodAptitudeForNumbersOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_numbers" });
		}

		protected static void ChildhoodWayWithPeopleOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_manners" });
		}

		protected static void ChildhoodSkillsWithHorsesOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_animals" });
		}

		protected static void ChildhoodGoodLeadingOnApply(CharacterCreation characterCreation)
		{
		}

		protected static void ChildhoodGoodAthleticsOnApply(CharacterCreation characterCreation)
		{
		}

		protected static void ChildhoodGoodMemoryOnApply(CharacterCreation characterCreation)
		{
		}

		protected static void ChildhoodGoodMathOnApply(CharacterCreation characterCreation)
		{
		}

		protected static void ChildhoodGoodMannersOnApply(CharacterCreation characterCreation)
		{
		}

		protected static void ChildhoodAffinityWithAnimalsOnApply(CharacterCreation characterCreation)
		{
		}

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

		protected bool RuralType()
		{
			return this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Retainer || this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Farmer || this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Hunter || this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Bard || this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Herder || this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Vagabond || this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Healer || this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Artisan;
		}

		protected bool RichParents()
		{
			return this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Retainer || this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Merchant;
		}

		protected bool RuralAdolescenceOnCondition()
		{
			return this.RuralType();
		}

		protected bool UrbanAdolescenceOnCondition()
		{
			return !this.RuralType();
		}

		protected bool UrbanRichAdolescenceOnCondition()
		{
			return !this.RuralType() && this.RichParents();
		}

		protected bool UrbanPoorAdolescenceOnCondition()
		{
			return !this.RuralType() && !this.RichParents();
		}

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

		protected void RuralAdolescenceHerderOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_streets" });
			this.RefreshPropsAndClothing(characterCreation, false, "carry_bostaff_rogue1", true, "");
		}

		protected void RuralAdolescenceSmithyOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_militia" });
			this.RefreshPropsAndClothing(characterCreation, false, "peasant_hammer_1_t1", true, "");
		}

		protected void RuralAdolescenceRepairmanOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_grit" });
			this.RefreshPropsAndClothing(characterCreation, false, "carry_hammer", true, "");
		}

		protected void RuralAdolescenceGathererOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_peddlers" });
			this.RefreshPropsAndClothing(characterCreation, false, "_to_carry_bd_basket_a", true, "");
		}

		protected void RuralAdolescenceHunterOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_sharp" });
			this.RefreshPropsAndClothing(characterCreation, false, "composite_bow", true, "");
		}

		protected void RuralAdolescenceHelperOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_peddlers_2" });
			this.RefreshPropsAndClothing(characterCreation, false, "_to_carry_bd_fabric_c", true, "");
		}

		protected void UrbanAdolescenceWatcherOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_fox" });
			this.RefreshPropsAndClothing(characterCreation, false, "", true, "");
		}

		protected void UrbanAdolescenceMarketerOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_manners" });
			this.RefreshPropsAndClothing(characterCreation, false, "", true, "");
		}

		protected void UrbanAdolescenceGangerOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_athlete" });
			this.RefreshPropsAndClothing(characterCreation, false, "", true, "");
		}

		protected void UrbanAdolescenceDockerOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_peddlers" });
			this.RefreshPropsAndClothing(characterCreation, false, "_to_carry_bd_basket_a", true, "");
		}

		protected void UrbanAdolescenceHorserOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_peddlers_2" });
			this.RefreshPropsAndClothing(characterCreation, false, "_to_carry_bd_fabric_c", true, "");
		}

		protected void UrbanAdolescenceTutorOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_book" });
			this.RefreshPropsAndClothing(characterCreation, false, "character_creation_notebook", false, "");
		}

		protected static void RuralAdolescenceHerderOnApply(CharacterCreation characterCreation)
		{
		}

		protected static void RuralAdolescenceSmithyOnApply(CharacterCreation characterCreation)
		{
		}

		protected static void RuralAdolescenceRepairmanOnApply(CharacterCreation characterCreation)
		{
		}

		protected static void RuralAdolescenceGathererOnApply(CharacterCreation characterCreation)
		{
		}

		protected static void RuralAdolescenceHunterOnApply(CharacterCreation characterCreation)
		{
		}

		protected static void RuralAdolescenceHelperOnApply(CharacterCreation characterCreation)
		{
		}

		protected static void UrbanAdolescenceWatcherOnApply(CharacterCreation characterCreation)
		{
		}

		protected static void UrbanAdolescenceMarketerOnApply(CharacterCreation characterCreation)
		{
		}

		protected static void UrbanAdolescenceGangerOnApply(CharacterCreation characterCreation)
		{
		}

		protected static void UrbanAdolescenceDockerOnApply(CharacterCreation characterCreation)
		{
		}

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

		protected bool YouthCommanderOnCondition()
		{
			return base.GetSelectedCulture().StringId == "empire" && this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Retainer;
		}

		protected void YouthCommanderOnApply(CharacterCreation characterCreation)
		{
		}

		protected bool YouthGroomOnCondition()
		{
			return base.GetSelectedCulture().StringId == "vlandia" && this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Retainer;
		}

		protected void YouthCommanderOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 10;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_decisive" });
		}

		protected void YouthGroomOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 10;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_sharp" });
		}

		protected void YouthChieftainOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 10;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_ready" });
		}

		protected void YouthCavalryOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 9;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_apprentice" });
		}

		protected void YouthHearthGuardOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 9;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_athlete" });
		}

		protected void YouthOutridersOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 2;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_gracious" });
		}

		protected void YouthOtherOutridersOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 2;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_gracious" });
		}

		protected void YouthInfantryOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 3;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_fierce" });
		}

		protected void YouthSkirmisherOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 4;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_fox" });
		}

		protected void YouthGarrisonOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 1;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_vibrant" });
		}

		protected void YouthOtherGarrisonOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 1;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_sharp" });
		}

		protected void YouthKernOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 8;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_apprentice" });
		}

		protected void YouthCamperOnConsequence(CharacterCreation characterCreation)
		{
			base.SelectedTitleType = 5;
			this.RefreshPlayerAppearance(characterCreation);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_militia" });
		}

		protected void YouthGroomOnApply(CharacterCreation characterCreation)
		{
		}

		protected bool YouthChieftainOnCondition()
		{
			return (base.GetSelectedCulture().StringId == "battania" || base.GetSelectedCulture().StringId == "khuzait") && this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Retainer;
		}

		protected void YouthChieftainOnApply(CharacterCreation characterCreation)
		{
		}

		protected bool YouthCavalryOnCondition()
		{
			return base.GetSelectedCulture().StringId == "empire" || base.GetSelectedCulture().StringId == "khuzait" || base.GetSelectedCulture().StringId == "aserai" || base.GetSelectedCulture().StringId == "vlandia";
		}

		protected void YouthCavalryOnApply(CharacterCreation characterCreation)
		{
		}

		protected bool YouthHearthGuardOnCondition()
		{
			return base.GetSelectedCulture().StringId == "sturgia" || base.GetSelectedCulture().StringId == "battania";
		}

		protected void YouthHearthGuardOnApply(CharacterCreation characterCreation)
		{
		}

		protected bool YouthOutridersOnCondition()
		{
			return base.GetSelectedCulture().StringId == "empire" || base.GetSelectedCulture().StringId == "khuzait";
		}

		protected void YouthOutridersOnApply(CharacterCreation characterCreation)
		{
		}

		protected bool YouthOtherOutridersOnCondition()
		{
			return base.GetSelectedCulture().StringId != "empire" && base.GetSelectedCulture().StringId != "khuzait";
		}

		protected void YouthOtherOutridersOnApply(CharacterCreation characterCreation)
		{
		}

		protected void YouthInfantryOnApply(CharacterCreation characterCreation)
		{
		}

		protected void YouthSkirmisherOnApply(CharacterCreation characterCreation)
		{
		}

		protected bool YouthGarrisonOnCondition()
		{
			return base.GetSelectedCulture().StringId == "empire" || base.GetSelectedCulture().StringId == "vlandia";
		}

		protected void YouthGarrisonOnApply(CharacterCreation characterCreation)
		{
		}

		protected bool YouthOtherGarrisonOnCondition()
		{
			return base.GetSelectedCulture().StringId != "empire" && base.GetSelectedCulture().StringId != "vlandia";
		}

		protected void YouthOtherGarrisonOnApply(CharacterCreation characterCreation)
		{
		}

		protected bool YouthSkirmisherOnCondition()
		{
			return base.GetSelectedCulture().StringId != "battania";
		}

		protected bool YouthKernOnCondition()
		{
			return base.GetSelectedCulture().StringId == "battania";
		}

		protected void YouthKernOnApply(CharacterCreation characterCreation)
		{
		}

		protected bool YouthCamperOnCondition()
		{
			return this._familyOccupationType != SandboxCharacterCreationContent.OccupationTypes.Retainer;
		}

		protected void YouthCamperOnApply(CharacterCreation characterCreation)
		{
		}

		protected void AccomplishmentOnInit(CharacterCreation characterCreation)
		{
			characterCreation.IsPlayerAlone = true;
			characterCreation.HasSecondaryCharacter = false;
			characterCreation.ClearFaceGenPrefab();
			characterCreation.ChangeFaceGenChars(SandboxCharacterCreationContent.ChangePlayerFaceWithAge((float)this.AccomplishmentAge, "act_childhood_schooled"));
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_schooled" });
			this.RefreshPlayerAppearance(characterCreation);
		}

		protected void AccomplishmentDefeatedEnemyOnApply(CharacterCreation characterCreation)
		{
		}

		protected void AccomplishmentExpeditionOnApply(CharacterCreation characterCreation)
		{
		}

		protected bool AccomplishmentRuralOnCondition()
		{
			return this.RuralType();
		}

		protected bool AccomplishmentMerchantOnCondition()
		{
			return this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Merchant;
		}

		protected bool AccomplishmentPosseOnConditions()
		{
			return this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Retainer || this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Herder || this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Mercenary;
		}

		protected bool AccomplishmentSavedVillageOnCondition()
		{
			return this.RuralType() && this._familyOccupationType != SandboxCharacterCreationContent.OccupationTypes.Retainer && this._familyOccupationType != SandboxCharacterCreationContent.OccupationTypes.Herder;
		}

		protected bool AccomplishmentSavedStreetOnCondition()
		{
			return !this.RuralType() && this._familyOccupationType != SandboxCharacterCreationContent.OccupationTypes.Merchant && this._familyOccupationType != SandboxCharacterCreationContent.OccupationTypes.Mercenary;
		}

		protected bool AccomplishmentUrbanOnCondition()
		{
			return !this.RuralType();
		}

		protected void AccomplishmentWorkshopOnApply(CharacterCreation characterCreation)
		{
		}

		protected void AccomplishmentSiegeHunterOnApply(CharacterCreation characterCreation)
		{
		}

		protected void AccomplishmentEscapadeOnApply(CharacterCreation characterCreation)
		{
		}

		protected void AccomplishmentTreaterOnApply(CharacterCreation characterCreation)
		{
		}

		protected void AccomplishmentDefeatedEnemyOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_athlete" });
		}

		protected void AccomplishmentExpeditionOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_gracious" });
		}

		protected void AccomplishmentMerchantOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_ready" });
		}

		protected void AccomplishmentSavedVillageOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_vibrant" });
		}

		protected void AccomplishmentSavedStreetOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_vibrant" });
		}

		protected void AccomplishmentWorkshopOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_decisive" });
		}

		protected void AccomplishmentSiegeHunterOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_tough" });
		}

		protected void AccomplishmentEscapadeOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_clever" });
		}

		protected void AccomplishmentTreaterOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_manners" });
		}

		protected void StartingAgeOnInit(CharacterCreation characterCreation)
		{
			characterCreation.IsPlayerAlone = true;
			characterCreation.HasSecondaryCharacter = false;
			characterCreation.ClearFaceGenPrefab();
			characterCreation.ChangeFaceGenChars(SandboxCharacterCreationContent.ChangePlayerFaceWithAge((float)this._startingAge, "act_childhood_schooled"));
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_schooled" });
			this.RefreshPlayerAppearance(characterCreation);
		}

		protected void StartingAgeYoungOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ClearFaceGenPrefab();
			characterCreation.ChangeFaceGenChars(SandboxCharacterCreationContent.ChangePlayerFaceWithAge(20f, "act_childhood_schooled"));
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_focus" });
			this.RefreshPlayerAppearance(characterCreation);
			this._startingAge = SandboxCharacterCreationContent.SandboxAgeOptions.YoungAdult;
			this.SetHeroAge(20f);
		}

		protected void StartingAgeAdultOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ClearFaceGenPrefab();
			characterCreation.ChangeFaceGenChars(SandboxCharacterCreationContent.ChangePlayerFaceWithAge(30f, "act_childhood_schooled"));
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_ready" });
			this.RefreshPlayerAppearance(characterCreation);
			this._startingAge = SandboxCharacterCreationContent.SandboxAgeOptions.Adult;
			this.SetHeroAge(30f);
		}

		protected void StartingAgeMiddleAgedOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ClearFaceGenPrefab();
			characterCreation.ChangeFaceGenChars(SandboxCharacterCreationContent.ChangePlayerFaceWithAge(40f, "act_childhood_schooled"));
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_sharp" });
			this.RefreshPlayerAppearance(characterCreation);
			this._startingAge = SandboxCharacterCreationContent.SandboxAgeOptions.MiddleAged;
			this.SetHeroAge(40f);
		}

		protected void StartingAgeElderlyOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ClearFaceGenPrefab();
			characterCreation.ChangeFaceGenChars(SandboxCharacterCreationContent.ChangePlayerFaceWithAge(50f, "act_childhood_schooled"));
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_tough" });
			this.RefreshPlayerAppearance(characterCreation);
			this._startingAge = SandboxCharacterCreationContent.SandboxAgeOptions.Elder;
			this.SetHeroAge(50f);
		}

		protected void StartingAgeYoungOnApply(CharacterCreation characterCreation)
		{
			this._startingAge = SandboxCharacterCreationContent.SandboxAgeOptions.YoungAdult;
		}

		protected void StartingAgeAdultOnApply(CharacterCreation characterCreation)
		{
			this._startingAge = SandboxCharacterCreationContent.SandboxAgeOptions.Adult;
		}

		protected void StartingAgeMiddleAgedOnApply(CharacterCreation characterCreation)
		{
			this._startingAge = SandboxCharacterCreationContent.SandboxAgeOptions.MiddleAged;
		}

		protected void StartingAgeElderlyOnApply(CharacterCreation characterCreation)
		{
			this._startingAge = SandboxCharacterCreationContent.SandboxAgeOptions.Elder;
		}

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

		protected void SetHeroAge(float age)
		{
			Hero.MainHero.SetBirthDay(CampaignTime.YearsFromNow(-age));
		}

		protected const int FocusToAddYouthStart = 2;

		protected const int FocusToAddAdultStart = 4;

		protected const int FocusToAddMiddleAgedStart = 6;

		protected const int FocusToAddElderlyStart = 8;

		protected const int AttributeToAddYouthStart = 1;

		protected const int AttributeToAddAdultStart = 2;

		protected const int AttributeToAddMiddleAgedStart = 3;

		protected const int AttributeToAddElderlyStart = 4;

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

		protected SandboxCharacterCreationContent.SandboxAgeOptions _startingAge = SandboxCharacterCreationContent.SandboxAgeOptions.YoungAdult;

		protected SandboxCharacterCreationContent.OccupationTypes _familyOccupationType;

		protected TextObject _educationIntroductoryText = new TextObject("{=!}{EDUCATION_INTRO}", null);

		protected TextObject _youthIntroductoryText = new TextObject("{=!}{YOUTH_INTRO}", null);

		protected enum SandboxAgeOptions
		{
			YoungAdult = 20,
			Adult = 30,
			MiddleAged = 40,
			Elder = 50
		}

		protected enum OccupationTypes
		{
			Artisan,
			Bard,
			Retainer,
			Merchant,
			Farmer,
			Hunter,
			Vagabond,
			Mercenary,
			Herder,
			Healer,
			NumberOfTypes
		}
	}
}

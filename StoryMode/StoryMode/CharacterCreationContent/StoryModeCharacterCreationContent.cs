using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace StoryMode.CharacterCreationContent
{
	public class StoryModeCharacterCreationContent : CharacterCreationContentBase
	{
		public override TextObject ReviewPageDescription
		{
			get
			{
				return new TextObject("{=wbhKgpmr}You prepare to set off with your brother on a mission of vengeance and rescue. Here is your character. Continue if you are ready, or go back to make changes.", null);
			}
		}

		public override IEnumerable<Type> CharacterCreationStages
		{
			get
			{
				yield return typeof(CharacterCreationCultureStage);
				yield return typeof(CharacterCreationFaceGeneratorStage);
				yield return typeof(CharacterCreationGenericStage);
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
		}

		protected override void OnInitialized(CharacterCreation characterCreation)
		{
			this.AddParentsMenu(characterCreation);
			this.AddChildhoodMenu(characterCreation);
			this.AddEducationMenu(characterCreation);
			this.AddYouthMenu(characterCreation);
			this.AddAdulthoodMenu(characterCreation);
			this.AddEscapeMenu(characterCreation);
		}

		protected override void OnApplyCulture()
		{
			StoryModeHeroes.LittleBrother.Culture = base.GetSelectedCulture();
			StoryModeHeroes.LittleSister.Culture = base.GetSelectedCulture();
		}

		protected void AddParentsMenu(CharacterCreation characterCreation)
		{
			CharacterCreationMenu characterCreationMenu = new CharacterCreationMenu(new TextObject("{=b4lDDcli}Family", null), new TextObject("{=XgFU1pCx}You were born into a family of...", null), new CharacterCreationOnInit(this.ParentsOnInit), 0);
			CharacterCreationCategory characterCreationCategory = characterCreationMenu.AddMenuCategory(new CharacterCreationOnCondition(this.EmpireParentsOnCondition));
			MBList<SkillObject> mblist = new MBList<SkillObject>();
			mblist.Add(DefaultSkills.Riding);
			mblist.Add(DefaultSkills.Polearm);
			MBList<SkillObject> mblist2 = mblist;
			CharacterAttribute characterAttribute = DefaultCharacterAttributes.Vigor;
			characterCreationCategory.AddCategoryOption(new TextObject("{=InN5ZZt3}A landlord's retainers", null), mblist2, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.EmpireLandlordsRetainerOnConsequence), new CharacterCreationApplyFinalEffects(this.EmpireLandlordsRetainerOnApply), new TextObject("{=ivKl4mV2}Your father was a trusted lieutenant of the local landowning aristocrat. He rode with the lord's cavalry, fighting as an armored lancer.", null), null, 0, 0, 0, 0, 0);
			MBList<SkillObject> mblist3 = new MBList<SkillObject>();
			mblist3.Add(DefaultSkills.Trade);
			mblist3.Add(DefaultSkills.Charm);
			mblist2 = mblist3;
			characterAttribute = DefaultCharacterAttributes.Social;
			characterCreationCategory.AddCategoryOption(new TextObject("{=651FhzdR}Urban merchants", null), mblist2, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.EmpireMerchantOnConsequence), new CharacterCreationApplyFinalEffects(this.EmpireMerchantOnApply), new TextObject("{=FQntPChs}Your family were merchants in one of the main cities of the Empire. They sometimes organized caravans to nearby towns, and discussed issues in the town council.", null), null, 0, 0, 0, 0, 0);
			MBList<SkillObject> mblist4 = new MBList<SkillObject>();
			mblist4.Add(DefaultSkills.Athletics);
			mblist4.Add(DefaultSkills.Polearm);
			mblist2 = mblist4;
			characterAttribute = DefaultCharacterAttributes.Endurance;
			characterCreationCategory.AddCategoryOption(new TextObject("{=sb4gg8Ak}Freeholders", null), mblist2, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.EmpireFreeholderOnConsequence), new CharacterCreationApplyFinalEffects(this.EmpireFreeholderOnApply), new TextObject("{=09z8Q08f}Your family were small farmers with just enough land to feed themselves and make a small profit. People like them were the pillars of the imperial rural economy, as well as the backbone of the levy.", null), null, 0, 0, 0, 0, 0);
			MBList<SkillObject> mblist5 = new MBList<SkillObject>();
			mblist5.Add(DefaultSkills.Crafting);
			mblist5.Add(DefaultSkills.Crossbow);
			mblist2 = mblist5;
			characterAttribute = DefaultCharacterAttributes.Intelligence;
			characterCreationCategory.AddCategoryOption(new TextObject("{=v48N6h1t}Urban artisans", null), mblist2, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.EmpireArtisanOnConsequence), new CharacterCreationApplyFinalEffects(this.EmpireArtisanOnApply), new TextObject("{=ZKynvffv}Your family owned their own workshop in a city, making goods from raw materials brought in from the countryside. Your father played an active if minor role in the town council, and also served in the militia.", null), null, 0, 0, 0, 0, 0);
			MBList<SkillObject> mblist6 = new MBList<SkillObject>();
			mblist6.Add(DefaultSkills.Scouting);
			mblist6.Add(DefaultSkills.Bow);
			mblist2 = mblist6;
			characterAttribute = DefaultCharacterAttributes.Control;
			characterCreationCategory.AddCategoryOption(new TextObject("{=7eWmU2mF}Foresters", null), mblist2, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.EmpireWoodsmanOnConsequence), new CharacterCreationApplyFinalEffects(this.EmpireWoodsmanOnApply), new TextObject("{=yRFSzSDZ}Your family lived in a village, but did not own their own land. Instead, your father supplemented paid jobs with long trips in the woods, hunting and trapping, always keeping a wary eye for the lord's game wardens.", null), null, 0, 0, 0, 0, 0);
			MBList<SkillObject> mblist7 = new MBList<SkillObject>();
			mblist7.Add(DefaultSkills.Roguery);
			mblist7.Add(DefaultSkills.Throwing);
			mblist2 = mblist7;
			characterAttribute = DefaultCharacterAttributes.Cunning;
			characterCreationCategory.AddCategoryOption(new TextObject("{=aEke8dSb}Urban vagabonds", null), mblist2, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.EmpireVagabondOnConsequence), new CharacterCreationApplyFinalEffects(this.EmpireVagabondOnApply), new TextObject("{=Jvf6K7TZ}Your family numbered among the many poor migrants living in the slums that grow up outside the walls of imperial cities, making whatever money they could from a variety of odd jobs. Sometimes they did service for one of the Empire's many criminal gangs, and you had an early look at the dark side of life.", null), null, 0, 0, 0, 0, 0);
			CharacterCreationCategory characterCreationCategory2 = characterCreationMenu.AddMenuCategory(new CharacterCreationOnCondition(this.VlandianParentsOnCondition));
			MBList<SkillObject> mblist8 = new MBList<SkillObject>();
			mblist8.Add(DefaultSkills.Riding);
			mblist8.Add(DefaultSkills.Polearm);
			mblist2 = mblist8;
			characterAttribute = DefaultCharacterAttributes.Social;
			characterCreationCategory2.AddCategoryOption(new TextObject("{=2TptWc4m}A baron's retainers", null), mblist2, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.VlandiaBaronsRetainerOnConsequence), new CharacterCreationApplyFinalEffects(this.VlandiaBaronsRetainerOnApply), new TextObject("{=0Suu1Q9q}Your father was a bailiff for a local feudal magnate. He looked after his liege's estates, resolved disputes in the village, and helped train the village levy. He rode with the lord's cavalry, fighting as an armored knight.", null), null, 0, 0, 0, 0, 0);
			MBList<SkillObject> mblist9 = new MBList<SkillObject>();
			mblist9.Add(DefaultSkills.Trade);
			mblist9.Add(DefaultSkills.Charm);
			mblist2 = mblist9;
			characterAttribute = DefaultCharacterAttributes.Intelligence;
			characterCreationCategory2.AddCategoryOption(new TextObject("{=651FhzdR}Urban merchants", null), mblist2, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.VlandiaMerchantOnConsequence), new CharacterCreationApplyFinalEffects(this.VlandiaMerchantOnApply), new TextObject("{=qNZFkxJb}Your family were merchants in one of the main cities of the kingdom. They organized caravans to nearby towns and were active in the local merchant's guild.", null), null, 0, 0, 0, 0, 0);
			MBList<SkillObject> mblist10 = new MBList<SkillObject>();
			mblist10.Add(DefaultSkills.Polearm);
			mblist10.Add(DefaultSkills.Crossbow);
			mblist2 = mblist10;
			characterAttribute = DefaultCharacterAttributes.Endurance;
			characterCreationCategory2.AddCategoryOption(new TextObject("{=RDfXuVxT}Yeomen", null), mblist2, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.VlandiaYeomanOnConsequence), new CharacterCreationApplyFinalEffects(this.VlandiaYeomanOnApply), new TextObject("{=BLZ4mdhb}Your family were small farmers with just enough land to feed themselves and make a small profit. People like them were the pillars of the kingdom's economy, as well as the backbone of the levy.", null), null, 0, 0, 0, 0, 0);
			MBList<SkillObject> mblist11 = new MBList<SkillObject>();
			mblist11.Add(DefaultSkills.Crafting);
			mblist11.Add(DefaultSkills.TwoHanded);
			mblist2 = mblist11;
			characterAttribute = DefaultCharacterAttributes.Vigor;
			characterCreationCategory2.AddCategoryOption(new TextObject("{=p2KIhGbE}Urban blacksmith", null), mblist2, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.VlandiaBlacksmithOnConsequence), new CharacterCreationApplyFinalEffects(this.VlandiaBlacksmithOnApply), new TextObject("{=btsMpRcA}Your family owned a smithy in a city. Your father played an active if minor role in the town council, and also served in the militia.", null), null, 0, 0, 0, 0, 0);
			MBList<SkillObject> mblist12 = new MBList<SkillObject>();
			mblist12.Add(DefaultSkills.Scouting);
			mblist12.Add(DefaultSkills.Crossbow);
			mblist2 = mblist12;
			characterAttribute = DefaultCharacterAttributes.Control;
			characterCreationCategory2.AddCategoryOption(new TextObject("{=YcnK0Thk}Hunters", null), mblist2, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.VlandiaHunterOnConsequence), new CharacterCreationApplyFinalEffects(this.VlandiaHunterOnApply), new TextObject("{=yRFSzSDZ}Your family lived in a village, but did not own their own land. Instead, your father supplemented paid jobs with long trips in the woods, hunting and trapping, always keeping a wary eye for the lord's game wardens.", null), null, 0, 0, 0, 0, 0);
			MBList<SkillObject> mblist13 = new MBList<SkillObject>();
			mblist13.Add(DefaultSkills.Roguery);
			mblist13.Add(DefaultSkills.Crossbow);
			mblist2 = mblist13;
			characterAttribute = DefaultCharacterAttributes.Cunning;
			characterCreationCategory2.AddCategoryOption(new TextObject("{=ipQP6aVi}Mercenaries", null), mblist2, characterAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.VlandiaMercenaryOnConsequence), new CharacterCreationApplyFinalEffects(this.VlandiaMercenaryOnApply), new TextObject("{=yYhX6JQC}Your father joined one of Vlandia's many mercenary companies, composed of men who got such a taste for war in their lord's service that they never took well to peace. Their crossbowmen were much valued across Calradia. Your mother was a camp follower, taking you along in the wake of bloody campaigns.", null), null, 0, 0, 0, 0, 0);
			CharacterCreationCategory characterCreationCategory3 = characterCreationMenu.AddMenuCategory(new CharacterCreationOnCondition(this.SturgianParentsOnCondition));
			TextObject textObject = new TextObject("{=mc78FEbA}A boyar's companions", null);
			MBList<SkillObject> mblist14 = new MBList<SkillObject>();
			mblist14.Add(DefaultSkills.Riding);
			mblist14.Add(DefaultSkills.TwoHanded);
			characterCreationCategory3.AddCategoryOption(textObject, mblist14, DefaultCharacterAttributes.Social, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.SturgiaBoyarsCompanionOnConsequence), new CharacterCreationApplyFinalEffects(this.SturgiaBoyarsCompanionOnApply), new TextObject("{=hob3WVkU}Your father was a member of a boyar's druzhina, the 'companions' that make up his retinue. He sat at his lord's table in the great hall, oversaw the boyar's estates, and stood by his side in the center of the shield wall in battle.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject2 = new TextObject("{=HqzVBfpl}Urban traders", null);
			MBList<SkillObject> mblist15 = new MBList<SkillObject>();
			mblist15.Add(DefaultSkills.Trade);
			mblist15.Add(DefaultSkills.Tactics);
			characterCreationCategory3.AddCategoryOption(textObject2, mblist15, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.SturgiaTraderOnConsequence), new CharacterCreationApplyFinalEffects(this.SturgiaTraderOnApply), new TextObject("{=bjVMtW3W}Your family were merchants who lived in one of Sturgia's great river ports, organizing the shipment of the north's bounty of furs, honey and other goods to faraway lands.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject3 = new TextObject("{=zrpqSWSh}Free farmers", null);
			MBList<SkillObject> mblist16 = new MBList<SkillObject>();
			mblist16.Add(DefaultSkills.Athletics);
			mblist16.Add(DefaultSkills.Polearm);
			characterCreationCategory3.AddCategoryOption(textObject3, mblist16, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.SturgiaFreemanOnConsequence), new CharacterCreationApplyFinalEffects(this.SturgiaFreemanOnApply), new TextObject("{=Mcd3ZyKq}Your family had just enough land to feed themselves and make a small profit. People like them were the pillars of the kingdom's economy, as well as the backbone of the levy.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject4 = new TextObject("{=v48N6h1t}Urban artisans", null);
			MBList<SkillObject> mblist17 = new MBList<SkillObject>();
			mblist17.Add(DefaultSkills.Crafting);
			mblist17.Add(DefaultSkills.OneHanded);
			characterCreationCategory3.AddCategoryOption(textObject4, mblist17, DefaultCharacterAttributes.Intelligence, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.SturgiaArtisanOnConsequence), new CharacterCreationApplyFinalEffects(this.SturgiaArtisanOnApply), new TextObject("{=ueCm5y1C}Your family owned their own workshop in a city, making goods from raw materials brought in from the countryside. Your father played an active if minor role in the town council, and also served in the militia.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject5 = new TextObject("{=YcnK0Thk}Hunters", null);
			MBList<SkillObject> mblist18 = new MBList<SkillObject>();
			mblist18.Add(DefaultSkills.Scouting);
			mblist18.Add(DefaultSkills.Bow);
			characterCreationCategory3.AddCategoryOption(textObject5, mblist18, DefaultCharacterAttributes.Vigor, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.SturgiaHunterOnConsequence), new CharacterCreationApplyFinalEffects(this.SturgiaHunterOnApply), new TextObject("{=WyZ2UtFF}Your family had no taste for the authority of the boyars. They made their living deep in the woods, slashing and burning fields which they tended for a year or two before moving on. They hunted and trapped fox, hare, ermine, and other fur-bearing animals.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject6 = new TextObject("{=TPoK3GSj}Vagabonds", null);
			MBList<SkillObject> mblist19 = new MBList<SkillObject>();
			mblist19.Add(DefaultSkills.Roguery);
			mblist19.Add(DefaultSkills.Throwing);
			characterCreationCategory3.AddCategoryOption(textObject6, mblist19, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.SturgiaVagabondOnConsequence), new CharacterCreationApplyFinalEffects(this.SturgiaVagabondOnApply), new TextObject("{=2SDWhGmQ}Your family numbered among the poor migrants living in the slums that grow up outside the walls of the river cities, making whatever money they could from a variety of odd jobs. Sometimes they did services for one of the region's many criminal gangs.", null), null, 0, 0, 0, 0, 0);
			CharacterCreationCategory characterCreationCategory4 = characterCreationMenu.AddMenuCategory(new CharacterCreationOnCondition(this.AseraiParentsOnCondition));
			TextObject textObject7 = new TextObject("{=Sw8OxnNr}Kinsfolk of an emir", null);
			MBList<SkillObject> mblist20 = new MBList<SkillObject>();
			mblist20.Add(DefaultSkills.Riding);
			mblist20.Add(DefaultSkills.Throwing);
			characterCreationCategory4.AddCategoryOption(textObject7, mblist20, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.AseraiTribesmanOnConsequence), new CharacterCreationApplyFinalEffects(this.AseraiTribesmanOnApply), new TextObject("{=MFrIHJZM}Your family was from a smaller offshoot of an emir's tribe. Your father's land gave him enough income to afford a horse but he was not quite wealthy enough to buy the armor needed to join the heavier cavalry. He fought as one of the light horsemen for which the desert is famous.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject8 = new TextObject("{=ngFVgwDD}Warrior-slaves", null);
			MBList<SkillObject> mblist21 = new MBList<SkillObject>();
			mblist21.Add(DefaultSkills.Riding);
			mblist21.Add(DefaultSkills.Polearm);
			characterCreationCategory4.AddCategoryOption(textObject8, mblist21, DefaultCharacterAttributes.Vigor, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.AseraiWariorSlaveOnConsequence), new CharacterCreationApplyFinalEffects(this.AseraiWariorSlaveOnApply), new TextObject("{=GsPC2MgU}Your father was part of one of the slave-bodyguards maintained by the Aserai emirs. He fought by his master's side with tribe's armored cavalry, and was freed - perhaps for an act of valor, or perhaps he paid for his freedom with his share of the spoils of battle. He then married your mother.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject9 = new TextObject("{=651FhzdR}Urban merchants", null);
			MBList<SkillObject> mblist22 = new MBList<SkillObject>();
			mblist22.Add(DefaultSkills.Trade);
			mblist22.Add(DefaultSkills.Charm);
			characterCreationCategory4.AddCategoryOption(textObject9, mblist22, DefaultCharacterAttributes.Social, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.AseraiMerchantOnConsequence), new CharacterCreationApplyFinalEffects(this.AseraiMerchantOnApply), new TextObject("{=1zXrlaav}Your family were respected traders in an oasis town. They ran caravans across the desert, and were experts in the finer points of negotiating passage through the desert tribes' territories.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject10 = new TextObject("{=g31pXuqi}Oasis farmers", null);
			MBList<SkillObject> mblist23 = new MBList<SkillObject>();
			mblist23.Add(DefaultSkills.Athletics);
			mblist23.Add(DefaultSkills.OneHanded);
			characterCreationCategory4.AddCategoryOption(textObject10, mblist23, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.AseraiOasisFarmerOnConsequence), new CharacterCreationApplyFinalEffects(this.AseraiOasisFarmerOnApply), new TextObject("{=5P0KqBAw}Your family tilled the soil in one of the oases of the Nahasa and tended the palm orchards that produced the desert's famous dates. Your father was a member of the main foot levy of his tribe, fighting with his kinsmen under the emir's banner.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject11 = new TextObject("{=EEedqolz}Bedouin", null);
			MBList<SkillObject> mblist24 = new MBList<SkillObject>();
			mblist24.Add(DefaultSkills.Scouting);
			mblist24.Add(DefaultSkills.Bow);
			characterCreationCategory4.AddCategoryOption(textObject11, mblist24, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.AseraiBedouinOnConsequence), new CharacterCreationApplyFinalEffects(this.AseraiBedouinOnApply), new TextObject("{=PKhcPbBX}Your family were part of a nomadic clan, crisscrossing the wastes between wadi beds and wells to feed their herds of goats and camels on the scraggly scrubs of the Nahasa.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject12 = new TextObject("{=tRIrbTvv}Urban back-alley thugs", null);
			MBList<SkillObject> mblist25 = new MBList<SkillObject>();
			mblist25.Add(DefaultSkills.Roguery);
			mblist25.Add(DefaultSkills.Polearm);
			characterCreationCategory4.AddCategoryOption(textObject12, mblist25, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.AseraiBackAlleyThugOnConsequence), new CharacterCreationApplyFinalEffects(this.AseraiBackAlleyThugOnApply), new TextObject("{=6bUSbsKC}Your father worked for a fitiwi, one of the strongmen who keep order in the poorer quarters of the oasis towns. He resolved disputes over land, dice and insults, imposing his authority with the fitiwi's traditional staff.", null), null, 0, 0, 0, 0, 0);
			CharacterCreationCategory characterCreationCategory5 = characterCreationMenu.AddMenuCategory(new CharacterCreationOnCondition(this.BattanianParentsOnCondition));
			TextObject textObject13 = new TextObject("{=GeNKQlHR}Members of the chieftain's hearthguard", null);
			MBList<SkillObject> mblist26 = new MBList<SkillObject>();
			mblist26.Add(DefaultSkills.TwoHanded);
			mblist26.Add(DefaultSkills.Bow);
			characterCreationCategory5.AddCategoryOption(textObject13, mblist26, DefaultCharacterAttributes.Vigor, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.BattaniaChieftainsHearthguardOnConsequence), new CharacterCreationApplyFinalEffects(this.BattaniaChieftainsHearthguardOnApply), new TextObject("{=LpH8SYFL}Your family were the trusted kinfolk of a Battanian chieftain, and sat at his table in his great hall. Your father assisted his chief in running the affairs of the clan and trained with the traditional weapons of the Battanian elite, the two-handed sword or falx and the bow.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject14 = new TextObject("{=AeBzTj6w}Healers", null);
			MBList<SkillObject> mblist27 = new MBList<SkillObject>();
			mblist27.Add(DefaultSkills.Medicine);
			mblist27.Add(DefaultSkills.Charm);
			characterCreationCategory5.AddCategoryOption(textObject14, mblist27, DefaultCharacterAttributes.Intelligence, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.BattaniaHealerOnConsequence), new CharacterCreationApplyFinalEffects(this.BattaniaHealerOnApply), new TextObject("{=j6py5Rv5}Your parents were healers who gathered herbs and treated the sick. As a living reservoir of Battanian tradition, they were also asked to adjudicate many disputes between the clans.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject15 = new TextObject("{=tGEStbxb}Tribespeople", null);
			MBList<SkillObject> mblist28 = new MBList<SkillObject>();
			mblist28.Add(DefaultSkills.Athletics);
			mblist28.Add(DefaultSkills.Throwing);
			characterCreationCategory5.AddCategoryOption(textObject15, mblist28, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.BattaniaTribesmanOnConsequence), new CharacterCreationApplyFinalEffects(this.BattaniaTribesmanOnApply), new TextObject("{=WchH8bS2}Your family were middle-ranking members of a Battanian clan, who tilled their own land. Your father fought with the kern, the main body of his people's warriors, joining in the screaming charges for which the Battanians were famous.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject16 = new TextObject("{=BCU6RezA}Smiths", null);
			MBList<SkillObject> mblist29 = new MBList<SkillObject>();
			mblist29.Add(DefaultSkills.Crafting);
			mblist29.Add(DefaultSkills.TwoHanded);
			characterCreationCategory5.AddCategoryOption(textObject16, mblist29, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.BattaniaSmithOnConsequence), new CharacterCreationApplyFinalEffects(this.BattaniaSmithOnApply), new TextObject("{=kg9YtrOg}Your family were smiths, a revered profession among the Battanians. They crafted everything from fine filigree jewelry in geometric designs to the well-balanced longswords favored by the Battanian aristocracy.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject17 = new TextObject("{=7eWmU2mF}Foresters", null);
			MBList<SkillObject> mblist30 = new MBList<SkillObject>();
			mblist30.Add(DefaultSkills.Scouting);
			mblist30.Add(DefaultSkills.Tactics);
			characterCreationCategory5.AddCategoryOption(textObject17, mblist30, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.BattaniaWoodsmanOnConsequence), new CharacterCreationApplyFinalEffects(this.BattaniaWoodsmanOnApply), new TextObject("{=7jBroUUQ}Your family had little land of their own, so they earned their living from the woods, hunting and trapping. They taught you from an early age that skills like finding game trails and killing an animal with one shot could make the difference between eating and starvation.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject18 = new TextObject("{=SpJqhEEh}Bards", null);
			MBList<SkillObject> mblist31 = new MBList<SkillObject>();
			mblist31.Add(DefaultSkills.Roguery);
			mblist31.Add(DefaultSkills.Charm);
			characterCreationCategory5.AddCategoryOption(textObject18, mblist31, DefaultCharacterAttributes.Social, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.BattaniaBardOnConsequence), new CharacterCreationApplyFinalEffects(this.BattaniaBardOnApply), new TextObject("{=aVzcyhhy}Your father was a bard, drifting from chieftain's hall to chieftain's hall making his living singing the praises of one Battanian aristocrat and mocking his enemies, then going to his enemy's hall and doing the reverse. You learned from him that a clever tongue could spare you  from a life toiling in the fields, if you kept your wits about you.", null), null, 0, 0, 0, 0, 0);
			CharacterCreationCategory characterCreationCategory6 = characterCreationMenu.AddMenuCategory(new CharacterCreationOnCondition(this.KhuzaitParentsOnCondition));
			TextObject textObject19 = new TextObject("{=FVaRDe2a}A noyan's kinsfolk", null);
			MBList<SkillObject> mblist32 = new MBList<SkillObject>();
			mblist32.Add(DefaultSkills.Riding);
			mblist32.Add(DefaultSkills.Polearm);
			characterCreationCategory6.AddCategoryOption(textObject19, mblist32, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.KhuzaitNoyansKinsmanOnConsequence), new CharacterCreationApplyFinalEffects(this.KhuzaitNoyansKinsmanOnApply), new TextObject("{=jAs3kDXh}Your family were the trusted kinsfolk of a Khuzait noyan, and shared his meals in the chieftain's yurt. Your father assisted his chief in running the affairs of the clan and fought in the core of armored lancers in the center of the Khuzait battle line.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject20 = new TextObject("{=TkgLEDRM}Merchants", null);
			MBList<SkillObject> mblist33 = new MBList<SkillObject>();
			mblist33.Add(DefaultSkills.Trade);
			mblist33.Add(DefaultSkills.Charm);
			characterCreationCategory6.AddCategoryOption(textObject20, mblist33, DefaultCharacterAttributes.Social, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.KhuzaitMerchantOnConsequence), new CharacterCreationApplyFinalEffects(this.KhuzaitMerchantOnApply), new TextObject("{=qPg3IDiq}Your family came from one of the merchant clans that dominated the cities in eastern Calradia before the Khuzait conquest. They adjusted quickly to their new masters, keeping the caravan routes running and ensuring that the tariff revenues that once went into imperial coffers now flowed to the khanate.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject21 = new TextObject("{=tGEStbxb}Tribespeople", null);
			MBList<SkillObject> mblist34 = new MBList<SkillObject>();
			mblist34.Add(DefaultSkills.Bow);
			mblist34.Add(DefaultSkills.Riding);
			characterCreationCategory6.AddCategoryOption(textObject21, mblist34, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.KhuzaitTribesmanOnConsequence), new CharacterCreationApplyFinalEffects(this.KhuzaitTribesmanOnApply), new TextObject("{=URgZ4ai4}Your family were middle-ranking members of one of the Khuzait clans. He had some  herds of his own, but was not rich. When the Khuzait horde was summoned to battle, he fought with the horse archers, shooting and wheeling and wearing down the enemy before the lancers delivered the final punch.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject22 = new TextObject("{=gQ2tAvCz}Farmers", null);
			MBList<SkillObject> mblist35 = new MBList<SkillObject>();
			mblist35.Add(DefaultSkills.Polearm);
			mblist35.Add(DefaultSkills.Throwing);
			characterCreationCategory6.AddCategoryOption(textObject22, mblist35, DefaultCharacterAttributes.Vigor, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.KhuzaitFarmerOnConsequence), new CharacterCreationApplyFinalEffects(this.KhuzaitFarmerOnApply), new TextObject("{=5QSGoRFj}Your family tilled one of the small patches of arable land in the steppes for generations. When the Khuzaits came, they ceased paying taxes to the emperor and providing conscripts for his army, and served the khan instead.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject23 = new TextObject("{=vfhVveLW}Shamans", null);
			MBList<SkillObject> mblist36 = new MBList<SkillObject>();
			mblist36.Add(DefaultSkills.Medicine);
			mblist36.Add(DefaultSkills.Charm);
			characterCreationCategory6.AddCategoryOption(textObject23, mblist36, DefaultCharacterAttributes.Intelligence, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.KhuzaitShamanOnConsequence), new CharacterCreationApplyFinalEffects(this.KhuzaitShamanOnApply), new TextObject("{=WOKNhaG2}Your family were guardians of the sacred traditions of the Khuzaits, channelling the spirits of the wilderness and of the ancestors. They tended the sick and dispensed wisdom, resolving disputes and providing practical advice.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject24 = new TextObject("{=Xqba1Obq}Nomads", null);
			MBList<SkillObject> mblist37 = new MBList<SkillObject>();
			mblist37.Add(DefaultSkills.Scouting);
			mblist37.Add(DefaultSkills.Riding);
			characterCreationCategory6.AddCategoryOption(textObject24, mblist37, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.KhuzaitNomadOnConsequence), new CharacterCreationApplyFinalEffects(this.KhuzaitNomadOnApply), new TextObject("{=9aoQYpZs}Your family's clan never pledged its loyalty to the khan and never settled down, preferring to live out in the deep steppe away from his authority. They remain some of the finest trackers and scouts in the grasslands, as the ability to spot an enemy coming and move quickly is often all that protects their herds from their neighbors' predations.", null), null, 0, 0, 0, 0, 0);
			characterCreation.AddNewMenu(characterCreationMenu);
		}

		protected void AddChildhoodMenu(CharacterCreation characterCreation)
		{
			CharacterCreationMenu characterCreationMenu = new CharacterCreationMenu(new TextObject("{=8Yiwt1z6}Early Childhood", null), new TextObject("{=character_creation_content_16}As a child you were noted for...", null), new CharacterCreationOnInit(this.ChildhoodOnInit), 0);
			CharacterCreationCategory characterCreationCategory = characterCreationMenu.AddMenuCategory(null);
			TextObject textObject = new TextObject("{=kmM68Qx4}your leadership skills.", null);
			MBList<SkillObject> mblist = new MBList<SkillObject>();
			mblist.Add(DefaultSkills.Leadership);
			mblist.Add(DefaultSkills.Tactics);
			characterCreationCategory.AddCategoryOption(textObject, mblist, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.ChildhoodYourLeadershipSkillsOnConsequence), new CharacterCreationApplyFinalEffects(this.ChildhoodGoodLeadingOnApply), new TextObject("{=FfNwXtii}If the wolf pup gang of your early childhood had an alpha, it was definitely you. All the other kids followed your lead as you decided what to play and where to play, and led them in games and mischief.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject2 = new TextObject("{=5HXS8HEY}your brawn.", null);
			MBList<SkillObject> mblist2 = new MBList<SkillObject>();
			mblist2.Add(DefaultSkills.TwoHanded);
			mblist2.Add(DefaultSkills.Throwing);
			characterCreationCategory.AddCategoryOption(textObject2, mblist2, DefaultCharacterAttributes.Vigor, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.ChildhoodYourBrawnOnConsequence), new CharacterCreationApplyFinalEffects(this.ChildhoodGoodAthleticsOnApply), new TextObject("{=YKzuGc54}You were big, and other children looked to have you around in any scrap with children from a neighboring village. You pushed a plough and threw an axe like an adult.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject3 = new TextObject("{=QrYjPUEf}your attention to detail.", null);
			MBList<SkillObject> mblist3 = new MBList<SkillObject>();
			mblist3.Add(DefaultSkills.Athletics);
			mblist3.Add(DefaultSkills.Bow);
			characterCreationCategory.AddCategoryOption(textObject3, mblist3, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.ChildhoodAttentionToDetailOnConsequence), new CharacterCreationApplyFinalEffects(this.ChildhoodGoodMemoryOnApply), new TextObject("{=JUSHAPnu}You were quick on your feet and attentive to what was going on around you. Usually you could run away from trouble, though you could give a good account of yourself in a fight with other children if cornered.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject4 = new TextObject("{=Y3UcaX74}your aptitude for numbers.", null);
			MBList<SkillObject> mblist4 = new MBList<SkillObject>();
			mblist4.Add(DefaultSkills.Engineering);
			mblist4.Add(DefaultSkills.Trade);
			characterCreationCategory.AddCategoryOption(textObject4, mblist4, DefaultCharacterAttributes.Intelligence, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.ChildhoodAptitudeForNumbersOnConsequence), new CharacterCreationApplyFinalEffects(this.ChildhoodGoodMathOnApply), new TextObject("{=DFidSjIf}Most children around you had only the most rudimentary education, but you lingered after class to study letters and mathematics. You were fascinated by the marketplace - weights and measures, tallies and accounts, the chatter about profits and losses.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject5 = new TextObject("{=GEYzLuwb}your way with people.", null);
			MBList<SkillObject> mblist5 = new MBList<SkillObject>();
			mblist5.Add(DefaultSkills.Charm);
			mblist5.Add(DefaultSkills.Leadership);
			characterCreationCategory.AddCategoryOption(textObject5, mblist5, DefaultCharacterAttributes.Social, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.ChildhoodWayWithPeopleOnConsequence), new CharacterCreationApplyFinalEffects(this.ChildhoodGoodMannersOnApply), new TextObject("{=w2TEQq26}You were always attentive to other people, good at guessing their motivations. You studied how individuals were swayed, and tried out what you learned from adults on your friends.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject6 = new TextObject("{=MEgLE2kj}your skill with horses.", null);
			MBList<SkillObject> mblist6 = new MBList<SkillObject>();
			mblist6.Add(DefaultSkills.Riding);
			mblist6.Add(DefaultSkills.Medicine);
			characterCreationCategory.AddCategoryOption(textObject6, mblist6, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.ChildhoodSkillsWithHorsesOnConsequence), new CharacterCreationApplyFinalEffects(this.ChildhoodAffinityWithAnimalsOnApply), new TextObject("{=ngazFofr}You were always drawn to animals, and spent as much time as possible hanging out in the village stables. You could calm horses, and were sometimes called upon to break in new colts. You learned the basics of veterinary arts, much of which is applicable to humans as well.", null), null, 0, 0, 0, 0, 0);
			characterCreation.AddNewMenu(characterCreationMenu);
		}

		protected void AddEducationMenu(CharacterCreation characterCreation)
		{
			CharacterCreationMenu characterCreationMenu = new CharacterCreationMenu(new TextObject("{=rcoueCmk}Adolescence", null), this._educationIntroductoryText, new CharacterCreationOnInit(this.EducationOnInit), 0);
			CharacterCreationCategory characterCreationCategory = characterCreationMenu.AddMenuCategory(null);
			TextObject textObject = new TextObject("{=RKVNvimC}herded the sheep.", null);
			MBList<SkillObject> mblist = new MBList<SkillObject>();
			mblist.Add(DefaultSkills.Athletics);
			mblist.Add(DefaultSkills.Throwing);
			characterCreationCategory.AddCategoryOption(textObject, mblist, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.RuralAdolescenceOnCondition), new CharacterCreationOnSelect(this.RuralAdolescenceHerderOnConsequence), new CharacterCreationApplyFinalEffects(this.RuralAdolescenceHerderOnApply), new TextObject("{=KfaqPpbK}You went with other fleet-footed youths to take the villages' sheep, goats or cattle to graze in pastures near the village. You were in charge of chasing down stray beasts, and always kept a big stone on hand to be hurled at lurking predators if necessary.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject2 = new TextObject("{=bTKiN0hr}worked in the village smithy.", null);
			MBList<SkillObject> mblist2 = new MBList<SkillObject>();
			mblist2.Add(DefaultSkills.TwoHanded);
			mblist2.Add(DefaultSkills.Crafting);
			characterCreationCategory.AddCategoryOption(textObject2, mblist2, DefaultCharacterAttributes.Vigor, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.RuralAdolescenceOnCondition), new CharacterCreationOnSelect(this.RuralAdolescenceSmithyOnConsequence), new CharacterCreationApplyFinalEffects(this.RuralAdolescenceSmithyOnApply), new TextObject("{=y6j1bJTH}You were apprenticed to the local smith. You learned how to heat and forge metal, hammering for hours at a time until your muscles ached.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject3 = new TextObject("{=tI8ZLtoA}repaired projects.", null);
			MBList<SkillObject> mblist3 = new MBList<SkillObject>();
			mblist3.Add(DefaultSkills.Crafting);
			mblist3.Add(DefaultSkills.Engineering);
			characterCreationCategory.AddCategoryOption(textObject3, mblist3, DefaultCharacterAttributes.Intelligence, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.RuralAdolescenceOnCondition), new CharacterCreationOnSelect(this.RuralAdolescenceRepairmanOnConsequence), new CharacterCreationApplyFinalEffects(this.RuralAdolescenceRepairmanOnApply), new TextObject("{=6LFj919J}You helped dig wells, rethatch houses, and fix broken plows. You learned about the basics of construction, as well as what it takes to keep a farming community prosperous.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject4 = new TextObject("{=TRwgSLD2}gathered herbs in the wild.", null);
			MBList<SkillObject> mblist4 = new MBList<SkillObject>();
			mblist4.Add(DefaultSkills.Medicine);
			mblist4.Add(DefaultSkills.Scouting);
			characterCreationCategory.AddCategoryOption(textObject4, mblist4, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.RuralAdolescenceOnCondition), new CharacterCreationOnSelect(this.RuralAdolescenceGathererOnConsequence), new CharacterCreationApplyFinalEffects(this.RuralAdolescenceGathererOnApply), new TextObject("{=9ks4u5cH}You were sent by the village healer up into the hills to look for useful medicinal plants. You learned which herbs healed wounds or brought down a fever, and how to find them.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject5 = new TextObject("{=T7m7ReTq}hunted small game.", null);
			MBList<SkillObject> mblist5 = new MBList<SkillObject>();
			mblist5.Add(DefaultSkills.Bow);
			mblist5.Add(DefaultSkills.Tactics);
			characterCreationCategory.AddCategoryOption(textObject5, mblist5, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.RuralAdolescenceOnCondition), new CharacterCreationOnSelect(this.RuralAdolescenceHunterOnConsequence), new CharacterCreationApplyFinalEffects(this.RuralAdolescenceHunterOnApply), new TextObject("{=RuvSk3QT}You accompanied a local hunter as he went into the wilderness, helping him set up traps and catch small animals.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject6 = new TextObject("{=qAbMagWq}sold product at the market.", null);
			MBList<SkillObject> mblist6 = new MBList<SkillObject>();
			mblist6.Add(DefaultSkills.Trade);
			mblist6.Add(DefaultSkills.Charm);
			characterCreationCategory.AddCategoryOption(textObject6, mblist6, DefaultCharacterAttributes.Social, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.RuralAdolescenceOnCondition), new CharacterCreationOnSelect(this.RuralAdolescenceHelperOnConsequence), new CharacterCreationApplyFinalEffects(this.RuralAdolescenceHelperOnApply), new TextObject("{=DIgsfYfz}You took your family's goods to the nearest town to sell your produce and buy supplies. It was hard work, but you enjoyed the hubbub of the marketplace.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject7 = new TextObject("{=nOfSqRnI}at the town watch's training ground.", null);
			MBList<SkillObject> mblist7 = new MBList<SkillObject>();
			mblist7.Add(DefaultSkills.Crossbow);
			mblist7.Add(DefaultSkills.Tactics);
			characterCreationCategory.AddCategoryOption(textObject7, mblist7, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.UrbanAdolescenceOnCondition), new CharacterCreationOnSelect(this.UrbanAdolescenceWatcherOnConsequence), new CharacterCreationApplyFinalEffects(this.UrbanAdolescenceWatcherOnApply), new TextObject("{=qnqdEJOv}You watched the town's watch practice shooting and perfect their plans to defend the walls in case of a siege.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject8 = new TextObject("{=8a6dnLd2}with the alley gangs.", null);
			MBList<SkillObject> mblist8 = new MBList<SkillObject>();
			mblist8.Add(DefaultSkills.Roguery);
			mblist8.Add(DefaultSkills.OneHanded);
			characterCreationCategory.AddCategoryOption(textObject8, mblist8, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.UrbanAdolescenceOnCondition), new CharacterCreationOnSelect(this.UrbanAdolescenceGangerOnConsequence), new CharacterCreationApplyFinalEffects(this.UrbanAdolescenceGangerOnApply), new TextObject("{=1SUTcF0J}The gang leaders who kept watch over the slums of Calradian cities were always in need of poor youth to run messages and back them up in turf wars, while thrill-seeking merchants' sons and daughters sometimes slummed it in their company as well.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject9 = new TextObject("{=7Hv984Sf}at docks and building sites.", null);
			MBList<SkillObject> mblist9 = new MBList<SkillObject>();
			mblist9.Add(DefaultSkills.Athletics);
			mblist9.Add(DefaultSkills.Crafting);
			characterCreationCategory.AddCategoryOption(textObject9, mblist9, DefaultCharacterAttributes.Vigor, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.UrbanAdolescenceOnCondition), new CharacterCreationOnSelect(this.UrbanAdolescenceDockerOnConsequence), new CharacterCreationApplyFinalEffects(this.UrbanAdolescenceDockerOnApply), new TextObject("{=bhdkegZ4}All towns had their share of projects that were constantly in need of both skilled and unskilled labor. You learned how hoists and scaffolds were constructed, how planks and stones were hewn and fitted, and other skills.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject10 = new TextObject("{=kbcwb5TH}in the markets and caravanserais.", null);
			MBList<SkillObject> mblist10 = new MBList<SkillObject>();
			mblist10.Add(DefaultSkills.Trade);
			mblist10.Add(DefaultSkills.Charm);
			characterCreationCategory.AddCategoryOption(textObject10, mblist10, DefaultCharacterAttributes.Social, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.UrbanPoorAdolescenceOnCondition), new CharacterCreationOnSelect(this.UrbanAdolescenceMarketerOnConsequence), new CharacterCreationApplyFinalEffects(this.UrbanAdolescenceMarketerOnApply), new TextObject("{=lLJh7WAT}You worked in the marketplace, selling trinkets and drinks to busy shoppers.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject11 = new TextObject("{=kbcwb5TH}in the markets and caravanserais.", null);
			MBList<SkillObject> mblist11 = new MBList<SkillObject>();
			mblist11.Add(DefaultSkills.Trade);
			mblist11.Add(DefaultSkills.Charm);
			characterCreationCategory.AddCategoryOption(textObject11, mblist11, DefaultCharacterAttributes.Social, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.UrbanRichAdolescenceOnCondition), new CharacterCreationOnSelect(this.UrbanAdolescenceMarketerOnConsequence), new CharacterCreationApplyFinalEffects(this.UrbanAdolescenceMarketerOnApply), new TextObject("{=rmMcwSn8}You helped your family handle their business affairs, going down to the marketplace to make purchases and oversee the arrival of caravans.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject12 = new TextObject("{=mfRbx5KE}reading and studying.", null);
			MBList<SkillObject> mblist12 = new MBList<SkillObject>();
			mblist12.Add(DefaultSkills.Engineering);
			mblist12.Add(DefaultSkills.Leadership);
			characterCreationCategory.AddCategoryOption(textObject12, mblist12, DefaultCharacterAttributes.Intelligence, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.UrbanPoorAdolescenceOnCondition), new CharacterCreationOnSelect(this.UrbanAdolescenceTutorOnConsequence), new CharacterCreationApplyFinalEffects(this.UrbanAdolescenceDockerOnApply), new TextObject("{=elQnygal}Your family scraped up the money for a rudimentary schooling and you took full advantage, reading voraciously on history, mathematics, and philosophy and discussing what you read with your tutor and classmates.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject13 = new TextObject("{=etG87fB7}with your tutor.", null);
			MBList<SkillObject> mblist13 = new MBList<SkillObject>();
			mblist13.Add(DefaultSkills.Engineering);
			mblist13.Add(DefaultSkills.Leadership);
			characterCreationCategory.AddCategoryOption(textObject13, mblist13, DefaultCharacterAttributes.Intelligence, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.UrbanRichAdolescenceOnCondition), new CharacterCreationOnSelect(this.UrbanAdolescenceTutorOnConsequence), new CharacterCreationApplyFinalEffects(this.UrbanAdolescenceDockerOnApply), new TextObject("{=hXl25avg}Your family arranged for a private tutor and you took full advantage, reading voraciously on history, mathematics, and philosophy and discussing what you read with your tutor and classmates.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject14 = new TextObject("{=FKpLEamz}caring for horses.", null);
			MBList<SkillObject> mblist14 = new MBList<SkillObject>();
			mblist14.Add(DefaultSkills.Riding);
			mblist14.Add(DefaultSkills.Steward);
			characterCreationCategory.AddCategoryOption(textObject14, mblist14, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.UrbanRichAdolescenceOnCondition), new CharacterCreationOnSelect(this.UrbanAdolescenceHorserOnConsequence), new CharacterCreationApplyFinalEffects(this.UrbanAdolescenceDockerOnApply), new TextObject("{=Ghz90npw}Your family owned a few horses at the town stables and you took charge of their care. Many evenings you would take them out beyond the walls and gallup through the fields, racing other youth.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject15 = new TextObject("{=vH7GtuuK}working at the stables.", null);
			MBList<SkillObject> mblist15 = new MBList<SkillObject>();
			mblist15.Add(DefaultSkills.Riding);
			mblist15.Add(DefaultSkills.Steward);
			characterCreationCategory.AddCategoryOption(textObject15, mblist15, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.UrbanPoorAdolescenceOnCondition), new CharacterCreationOnSelect(this.UrbanAdolescenceHorserOnConsequence), new CharacterCreationApplyFinalEffects(this.UrbanAdolescenceDockerOnApply), new TextObject("{=csUq1RCC}You were employed as a hired hand at the town's stables. The overseers recognized that you had a knack for horses, and you were allowed to exercise them and sometimes even break in new steeds.", null), null, 0, 0, 0, 0, 0);
			characterCreation.AddNewMenu(characterCreationMenu);
		}

		protected void AddYouthMenu(CharacterCreation characterCreation)
		{
			CharacterCreationMenu characterCreationMenu = new CharacterCreationMenu(new TextObject("{=ok8lSW6M}Youth", null), this._youthIntroductoryText, new CharacterCreationOnInit(this.YouthOnInit), 0);
			CharacterCreationCategory characterCreationCategory = characterCreationMenu.AddMenuCategory(null);
			TextObject textObject = new TextObject("{=CITG915d}joined a commander's staff.", null);
			MBList<SkillObject> mblist = new MBList<SkillObject>();
			mblist.Add(DefaultSkills.Steward);
			mblist.Add(DefaultSkills.Tactics);
			characterCreationCategory.AddCategoryOption(textObject, mblist, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthCommanderOnCondition), new CharacterCreationOnSelect(this.YouthCommanderOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthCommanderOnApply), new TextObject("{=Ay0G3f7I}Your family arranged for you to be part of the staff of an imperial strategos. You were not given major responsibilities - mostly carrying messages and tending to his horse -- but it did give you a chance to see how campaigns were planned and men were deployed in battle.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject2 = new TextObject("{=bhE2i6OU}served as a baron's groom.", null);
			MBList<SkillObject> mblist2 = new MBList<SkillObject>();
			mblist2.Add(DefaultSkills.Steward);
			mblist2.Add(DefaultSkills.Tactics);
			characterCreationCategory.AddCategoryOption(textObject2, mblist2, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthGroomOnCondition), new CharacterCreationOnSelect(this.YouthGroomOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthGroomOnApply), new TextObject("{=iZKtGI6Y}Your family arranged for you to accompany a minor baron of the Vlandian kingdom. You were not given major responsibilities - mostly carrying messages and tending to his horse -- but it did give you a chance to see how campaigns were planned and men were deployed in battle.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject3 = new TextObject("{=F2bgujPo}were a chieftain's servant.", null);
			MBList<SkillObject> mblist3 = new MBList<SkillObject>();
			mblist3.Add(DefaultSkills.Steward);
			mblist3.Add(DefaultSkills.Tactics);
			characterCreationCategory.AddCategoryOption(textObject3, mblist3, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthChieftainOnCondition), new CharacterCreationOnSelect(this.YouthChieftainOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthChieftainOnApply), new TextObject("{=7AYJ3SjK}Your family arranged for you to accompany a chieftain of your people. You were not given major responsibilities - mostly carrying messages and tending to his horse -- but it did give you a chance to see how campaigns were planned and men were deployed in battle.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject4 = new TextObject("{=h2KnarLL}trained with the cavalry.", null);
			MBList<SkillObject> mblist4 = new MBList<SkillObject>();
			mblist4.Add(DefaultSkills.Riding);
			mblist4.Add(DefaultSkills.Polearm);
			characterCreationCategory.AddCategoryOption(textObject4, mblist4, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthCavalryOnCondition), new CharacterCreationOnSelect(this.YouthCavalryOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthCavalryOnApply), new TextObject("{=7cHsIMLP}You could never have bought the equipment on your own, but you were a good enough rider so that the local lord lent you a horse and equipment. You joined the armored cavalry, training with the lance.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject5 = new TextObject("{=zsC2t5Hb}trained with the hearth guard.", null);
			MBList<SkillObject> mblist5 = new MBList<SkillObject>();
			mblist5.Add(DefaultSkills.Riding);
			mblist5.Add(DefaultSkills.Polearm);
			characterCreationCategory.AddCategoryOption(textObject5, mblist5, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthHearthGuardOnCondition), new CharacterCreationOnSelect(this.YouthHearthGuardOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthHearthGuardOnApply), new TextObject("{=RmbWW6Bm}You were a big and imposing enough youth that the chief's guard allowed you to train alongside them, in preparation to join them some day.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject6 = new TextObject("{=aTncHUfL}stood guard with the garrisons.", null);
			MBList<SkillObject> mblist6 = new MBList<SkillObject>();
			mblist6.Add(DefaultSkills.Crossbow);
			mblist6.Add(DefaultSkills.Engineering);
			characterCreationCategory.AddCategoryOption(textObject6, mblist6, DefaultCharacterAttributes.Intelligence, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthGarrisonOnCondition), new CharacterCreationOnSelect(this.YouthGarrisonOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthGarrisonOnApply), new TextObject("{=63TAYbkx}Urban troops spend much of their time guarding the town walls. Most of their training was in missile weapons, especially useful during sieges.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject7 = new TextObject("{=aTncHUfL}stood guard with the garrisons.", null);
			MBList<SkillObject> mblist7 = new MBList<SkillObject>();
			mblist7.Add(DefaultSkills.Bow);
			mblist7.Add(DefaultSkills.Engineering);
			characterCreationCategory.AddCategoryOption(textObject7, mblist7, DefaultCharacterAttributes.Intelligence, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthOtherGarrisonOnCondition), new CharacterCreationOnSelect(this.YouthOtherGarrisonOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthOtherGarrisonOnApply), new TextObject("{=1EkEElZd}Urban troops spend much of their time guarding the town walls. Most of their training was in missile weapons.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject8 = new TextObject("{=VlXOgIX6}rode with the scouts.", null);
			MBList<SkillObject> mblist8 = new MBList<SkillObject>();
			mblist8.Add(DefaultSkills.Riding);
			mblist8.Add(DefaultSkills.Bow);
			characterCreationCategory.AddCategoryOption(textObject8, mblist8, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthOutridersOnCondition), new CharacterCreationOnSelect(this.YouthOutridersOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthOutridersOnApply), new TextObject("{=888lmJqs}All of Calradia's kingdoms recognize the value of good light cavalry and horse archers, and are sure to recruit nomads and borderers with the skills to fulfill those duties. You were a good enough rider that your neighbors pitched in to buy you a small pony and a good bow so that you could fulfill their levy obligations.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject9 = new TextObject("{=VlXOgIX6}rode with the scouts.", null);
			MBList<SkillObject> mblist9 = new MBList<SkillObject>();
			mblist9.Add(DefaultSkills.Riding);
			mblist9.Add(DefaultSkills.Bow);
			characterCreationCategory.AddCategoryOption(textObject9, mblist9, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthOtherOutridersOnCondition), new CharacterCreationOnSelect(this.YouthOtherOutridersOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthOtherOutridersOnApply), new TextObject("{=sYuN6hPD}All of Calradia's kingdoms recognize the value of good light cavalry, and are sure to recruit nomads and borderers with the skills to fulfill those duties. You were a good enough rider that your neighbors pitched in to buy you a small pony and a sheaf of javelins so that you could fulfill their levy obligations.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject10 = new TextObject("{=a8arFSra}trained with the infantry.", null);
			MBList<SkillObject> mblist10 = new MBList<SkillObject>();
			mblist10.Add(DefaultSkills.Polearm);
			mblist10.Add(DefaultSkills.OneHanded);
			characterCreationCategory.AddCategoryOption(textObject10, mblist10, DefaultCharacterAttributes.Vigor, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.YouthInfantryOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthInfantryOnApply), new TextObject("{=afH90aNs}Levy armed with spear and shield, drawn from smallholding farmers, have always been the backbone of most armies of Calradia.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject11 = new TextObject("{=oMbOIPc9}joined the skirmishers.", null);
			MBList<SkillObject> mblist11 = new MBList<SkillObject>();
			mblist11.Add(DefaultSkills.Throwing);
			mblist11.Add(DefaultSkills.OneHanded);
			characterCreationCategory.AddCategoryOption(textObject11, mblist11, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthSkirmisherOnCondition), new CharacterCreationOnSelect(this.YouthSkirmisherOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthSkirmisherOnApply), new TextObject("{=bXAg5w19}Younger recruits, or those of a slighter build, or those too poor to buy shield and armor tend to join the skirmishers. Fighting with bow and javelin, they try to stay out of reach of the main enemy forces.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject12 = new TextObject("{=cDWbwBwI}joined the kern.", null);
			MBList<SkillObject> mblist12 = new MBList<SkillObject>();
			mblist12.Add(DefaultSkills.Throwing);
			mblist12.Add(DefaultSkills.OneHanded);
			characterCreationCategory.AddCategoryOption(textObject12, mblist12, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthKernOnCondition), new CharacterCreationOnSelect(this.YouthKernOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthKernOnApply), new TextObject("{=tTb28jyU}Many Battanians fight as kern, versatile troops who could both harass the enemy line with their javelins or join in the final screaming charge once it weakened.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject13 = new TextObject("{=GFUggps8}marched with the camp followers.", null);
			MBList<SkillObject> mblist13 = new MBList<SkillObject>();
			mblist13.Add(DefaultSkills.Roguery);
			mblist13.Add(DefaultSkills.Throwing);
			characterCreationCategory.AddCategoryOption(textObject13, mblist13, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.YouthCamperOnCondition), new CharacterCreationOnSelect(this.YouthCamperOnConsequence), new CharacterCreationApplyFinalEffects(this.YouthCamperOnApply), new TextObject("{=64rWqBLN}You avoided service with one of the main forces of your realm's armies, but followed instead in the train - the troops' wives, lovers and servants, and those who make their living by caring for, entertaining, or cheating the soldiery.", null), null, 0, 0, 0, 0, 0);
			characterCreation.AddNewMenu(characterCreationMenu);
		}

		protected void AddAdulthoodMenu(CharacterCreation characterCreation)
		{
			MBTextManager.SetTextVariable("EXP_VALUE", this.SkillLevelToAdd);
			CharacterCreationMenu characterCreationMenu = new CharacterCreationMenu(new TextObject("{=MafIe9yI}Young Adulthood", null), new TextObject("{=4WYY0X59}Before you set out for a life of adventure, your biggest achievement was...", null), new CharacterCreationOnInit(this.AccomplishmentOnInit), 0);
			CharacterCreationCategory characterCreationCategory = characterCreationMenu.AddMenuCategory(null);
			TextObject textObject = new TextObject("{=8bwpVpgy}you defeated an enemy in battle.", null);
			MBList<SkillObject> mblist = new MBList<SkillObject>();
			mblist.Add(DefaultSkills.OneHanded);
			mblist.Add(DefaultSkills.TwoHanded);
			CharacterAttribute vigor = DefaultCharacterAttributes.Vigor;
			int focusToAdd = this.FocusToAdd;
			int skillLevelToAdd = this.SkillLevelToAdd;
			int attributeLevelToAdd = this.AttributeLevelToAdd;
			CharacterCreationOnCondition characterCreationOnCondition = null;
			CharacterCreationOnSelect characterCreationOnSelect = new CharacterCreationOnSelect(this.AccomplishmentDefeatedEnemyOnConsequence);
			CharacterCreationApplyFinalEffects characterCreationApplyFinalEffects = new CharacterCreationApplyFinalEffects(this.AccomplishmentDefeatedEnemyOnApply);
			TextObject textObject2 = new TextObject("{=1IEroJKs}Not everyone who musters for the levy marches to war, and not everyone who goes on campaign sees action. You did both, and you also took down an enemy warrior in direct one-to-one combat, in the full view of your comrades.", null);
			MBList<TraitObject> mblist2 = new MBList<TraitObject>();
			mblist2.Add(DefaultTraits.Valor);
			characterCreationCategory.AddCategoryOption(textObject, mblist, vigor, focusToAdd, skillLevelToAdd, attributeLevelToAdd, characterCreationOnCondition, characterCreationOnSelect, characterCreationApplyFinalEffects, textObject2, mblist2, 1, 20, 0, 0, 0);
			TextObject textObject3 = new TextObject("{=mP3uFbcq}you led a successful manhunt.", null);
			MBList<SkillObject> mblist3 = new MBList<SkillObject>();
			mblist3.Add(DefaultSkills.Tactics);
			mblist3.Add(DefaultSkills.Leadership);
			CharacterAttribute cunning = DefaultCharacterAttributes.Cunning;
			int focusToAdd2 = this.FocusToAdd;
			int skillLevelToAdd2 = this.SkillLevelToAdd;
			int attributeLevelToAdd2 = this.AttributeLevelToAdd;
			CharacterCreationOnCondition characterCreationOnCondition2 = new CharacterCreationOnCondition(this.AccomplishmentPosseOnConditions);
			CharacterCreationOnSelect characterCreationOnSelect2 = new CharacterCreationOnSelect(this.AccomplishmentExpeditionOnConsequence);
			CharacterCreationApplyFinalEffects characterCreationApplyFinalEffects2 = new CharacterCreationApplyFinalEffects(this.AccomplishmentExpeditionOnApply);
			TextObject textObject4 = new TextObject("{=4f5xwzX0}When your community needed to organize a posse to pursue horse thieves, you were the obvious choice. You hunted down the raiders, surrounded them and forced their surrender, and took back your stolen property.", null);
			MBList<TraitObject> mblist4 = new MBList<TraitObject>();
			mblist4.Add(DefaultTraits.Calculating);
			characterCreationCategory.AddCategoryOption(textObject3, mblist3, cunning, focusToAdd2, skillLevelToAdd2, attributeLevelToAdd2, characterCreationOnCondition2, characterCreationOnSelect2, characterCreationApplyFinalEffects2, textObject4, mblist4, 1, 10, 0, 0, 0);
			TextObject textObject5 = new TextObject("{=wfbtS71d}you led a caravan.", null);
			MBList<SkillObject> mblist5 = new MBList<SkillObject>();
			mblist5.Add(DefaultSkills.Tactics);
			mblist5.Add(DefaultSkills.Leadership);
			CharacterAttribute cunning2 = DefaultCharacterAttributes.Cunning;
			int focusToAdd3 = this.FocusToAdd;
			int skillLevelToAdd3 = this.SkillLevelToAdd;
			int attributeLevelToAdd3 = this.AttributeLevelToAdd;
			CharacterCreationOnCondition characterCreationOnCondition3 = new CharacterCreationOnCondition(this.AccomplishmentMerchantOnCondition);
			CharacterCreationOnSelect characterCreationOnSelect3 = new CharacterCreationOnSelect(this.AccomplishmentMerchantOnConsequence);
			CharacterCreationApplyFinalEffects characterCreationApplyFinalEffects3 = new CharacterCreationApplyFinalEffects(this.AccomplishmentExpeditionOnApply);
			TextObject textObject6 = new TextObject("{=joRHKCkm}Your family needed someone trustworthy to take a caravan to a neighboring town. You organized supplies, ensured a constant watch to keep away bandits, and brought it safely to its destination.", null);
			MBList<TraitObject> mblist6 = new MBList<TraitObject>();
			mblist6.Add(DefaultTraits.Calculating);
			characterCreationCategory.AddCategoryOption(textObject5, mblist5, cunning2, focusToAdd3, skillLevelToAdd3, attributeLevelToAdd3, characterCreationOnCondition3, characterCreationOnSelect3, characterCreationApplyFinalEffects3, textObject6, mblist6, 1, 10, 0, 0, 0);
			TextObject textObject7 = new TextObject("{=x1HTX5hq}you saved your village from a flood.", null);
			MBList<SkillObject> mblist7 = new MBList<SkillObject>();
			mblist7.Add(DefaultSkills.Tactics);
			mblist7.Add(DefaultSkills.Leadership);
			CharacterAttribute cunning3 = DefaultCharacterAttributes.Cunning;
			int focusToAdd4 = this.FocusToAdd;
			int skillLevelToAdd4 = this.SkillLevelToAdd;
			int attributeLevelToAdd4 = this.AttributeLevelToAdd;
			CharacterCreationOnCondition characterCreationOnCondition4 = new CharacterCreationOnCondition(this.AccomplishmentSavedVillageOnCondition);
			CharacterCreationOnSelect characterCreationOnSelect4 = new CharacterCreationOnSelect(this.AccomplishmentSavedVillageOnConsequence);
			CharacterCreationApplyFinalEffects characterCreationApplyFinalEffects4 = new CharacterCreationApplyFinalEffects(this.AccomplishmentExpeditionOnApply);
			TextObject textObject8 = new TextObject("{=bWlmGDf3}When a sudden storm caused the local stream to rise suddenly, your neighbors needed quick-thinking leadership. You provided it, directing them to build levees to save their homes.", null);
			MBList<TraitObject> mblist8 = new MBList<TraitObject>();
			mblist8.Add(DefaultTraits.Calculating);
			characterCreationCategory.AddCategoryOption(textObject7, mblist7, cunning3, focusToAdd4, skillLevelToAdd4, attributeLevelToAdd4, characterCreationOnCondition4, characterCreationOnSelect4, characterCreationApplyFinalEffects4, textObject8, mblist8, 1, 10, 0, 0, 0);
			TextObject textObject9 = new TextObject("{=s8PNllPN}you saved your city quarter from a fire.", null);
			MBList<SkillObject> mblist9 = new MBList<SkillObject>();
			mblist9.Add(DefaultSkills.Tactics);
			mblist9.Add(DefaultSkills.Leadership);
			CharacterAttribute cunning4 = DefaultCharacterAttributes.Cunning;
			int focusToAdd5 = this.FocusToAdd;
			int skillLevelToAdd5 = this.SkillLevelToAdd;
			int attributeLevelToAdd5 = this.AttributeLevelToAdd;
			CharacterCreationOnCondition characterCreationOnCondition5 = new CharacterCreationOnCondition(this.AccomplishmentSavedStreetOnCondition);
			CharacterCreationOnSelect characterCreationOnSelect5 = new CharacterCreationOnSelect(this.AccomplishmentSavedStreetOnConsequence);
			CharacterCreationApplyFinalEffects characterCreationApplyFinalEffects5 = new CharacterCreationApplyFinalEffects(this.AccomplishmentExpeditionOnApply);
			TextObject textObject10 = new TextObject("{=ZAGR6PYc}When a sudden blaze broke out in a back alley, your neighbors needed quick-thinking leadership and you provided it. You organized a bucket line to the nearest well, putting the fire out before any homes were lost.", null);
			MBList<TraitObject> mblist10 = new MBList<TraitObject>();
			mblist10.Add(DefaultTraits.Calculating);
			characterCreationCategory.AddCategoryOption(textObject9, mblist9, cunning4, focusToAdd5, skillLevelToAdd5, attributeLevelToAdd5, characterCreationOnCondition5, characterCreationOnSelect5, characterCreationApplyFinalEffects5, textObject10, mblist10, 1, 10, 0, 0, 0);
			TextObject textObject11 = new TextObject("{=xORjDTal}you invested some money in a workshop.", null);
			MBList<SkillObject> mblist11 = new MBList<SkillObject>();
			mblist11.Add(DefaultSkills.Trade);
			mblist11.Add(DefaultSkills.Crafting);
			CharacterAttribute intelligence = DefaultCharacterAttributes.Intelligence;
			int focusToAdd6 = this.FocusToAdd;
			int skillLevelToAdd6 = this.SkillLevelToAdd;
			int attributeLevelToAdd6 = this.AttributeLevelToAdd;
			CharacterCreationOnCondition characterCreationOnCondition6 = new CharacterCreationOnCondition(this.AccomplishmentUrbanOnCondition);
			CharacterCreationOnSelect characterCreationOnSelect6 = new CharacterCreationOnSelect(this.AccomplishmentWorkshopOnConsequence);
			CharacterCreationApplyFinalEffects characterCreationApplyFinalEffects6 = new CharacterCreationApplyFinalEffects(this.AccomplishmentWorkshopOnApply);
			TextObject textObject12 = new TextObject("{=PyVqDLBu}Your parents didn't give you much money, but they did leave just enough for you to secure a loan against a larger amount to build a small workshop. You paid back what you borrowed, and sold your enterprise for a profit.", null);
			MBList<TraitObject> mblist12 = new MBList<TraitObject>();
			mblist12.Add(DefaultTraits.Calculating);
			characterCreationCategory.AddCategoryOption(textObject11, mblist11, intelligence, focusToAdd6, skillLevelToAdd6, attributeLevelToAdd6, characterCreationOnCondition6, characterCreationOnSelect6, characterCreationApplyFinalEffects6, textObject12, mblist12, 1, 10, 0, 0, 0);
			TextObject textObject13 = new TextObject("{=xKXcqRJI}you invested some money in land.", null);
			MBList<SkillObject> mblist13 = new MBList<SkillObject>();
			mblist13.Add(DefaultSkills.Trade);
			mblist13.Add(DefaultSkills.Crafting);
			CharacterAttribute intelligence2 = DefaultCharacterAttributes.Intelligence;
			int focusToAdd7 = this.FocusToAdd;
			int skillLevelToAdd7 = this.SkillLevelToAdd;
			int attributeLevelToAdd7 = this.AttributeLevelToAdd;
			CharacterCreationOnCondition characterCreationOnCondition7 = new CharacterCreationOnCondition(this.AccomplishmentRuralOnCondition);
			CharacterCreationOnSelect characterCreationOnSelect7 = new CharacterCreationOnSelect(this.AccomplishmentWorkshopOnConsequence);
			CharacterCreationApplyFinalEffects characterCreationApplyFinalEffects7 = new CharacterCreationApplyFinalEffects(this.AccomplishmentWorkshopOnApply);
			TextObject textObject14 = new TextObject("{=cbF9jdQo}Your parents didn't give you much money, but they did leave just enough for you to purchase a plot of unused land at the edge of the village. You cleared away rocks and dug an irrigation ditch, raised a few seasons of crops, than sold it for a considerable profit.", null);
			MBList<TraitObject> mblist14 = new MBList<TraitObject>();
			mblist14.Add(DefaultTraits.Calculating);
			characterCreationCategory.AddCategoryOption(textObject13, mblist13, intelligence2, focusToAdd7, skillLevelToAdd7, attributeLevelToAdd7, characterCreationOnCondition7, characterCreationOnSelect7, characterCreationApplyFinalEffects7, textObject14, mblist14, 1, 10, 0, 0, 0);
			TextObject textObject15 = new TextObject("{=TbNRtUjb}you hunted a dangerous animal.", null);
			MBList<SkillObject> mblist15 = new MBList<SkillObject>();
			mblist15.Add(DefaultSkills.Polearm);
			mblist15.Add(DefaultSkills.Crossbow);
			CharacterAttribute control = DefaultCharacterAttributes.Control;
			int focusToAdd8 = this.FocusToAdd;
			int skillLevelToAdd8 = this.SkillLevelToAdd;
			int attributeLevelToAdd8 = this.AttributeLevelToAdd;
			CharacterCreationOnCondition characterCreationOnCondition8 = new CharacterCreationOnCondition(this.AccomplishmentRuralOnCondition);
			CharacterCreationOnSelect characterCreationOnSelect8 = new CharacterCreationOnSelect(this.AccomplishmentSiegeHunterOnConsequence);
			CharacterCreationApplyFinalEffects characterCreationApplyFinalEffects8 = new CharacterCreationApplyFinalEffects(this.AccomplishmentSiegeHunterOnApply);
			TextObject textObject16 = new TextObject("{=I3PcdaaL}Wolves, bears are a constant menace to the flocks of northern Calradia, while hyenas and leopards trouble the south. You went with a group of your fellow villagers and fired the missile that brought down the beast.", null);
			MBList<TraitObject> mblist16 = new MBList<TraitObject>();
			mblist16.Add(DefaultTraits.Valor);
			characterCreationCategory.AddCategoryOption(textObject15, mblist15, control, focusToAdd8, skillLevelToAdd8, attributeLevelToAdd8, characterCreationOnCondition8, characterCreationOnSelect8, characterCreationApplyFinalEffects8, textObject16, mblist16, 1, 5, 0, 0, 0);
			TextObject textObject17 = new TextObject("{=WbHfGCbd}you survived a siege.", null);
			MBList<SkillObject> mblist17 = new MBList<SkillObject>();
			mblist17.Add(DefaultSkills.Bow);
			mblist17.Add(DefaultSkills.Crossbow);
			characterCreationCategory.AddCategoryOption(textObject17, mblist17, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, new CharacterCreationOnCondition(this.AccomplishmentUrbanOnCondition), new CharacterCreationOnSelect(this.AccomplishmentSiegeHunterOnConsequence), new CharacterCreationApplyFinalEffects(this.AccomplishmentSiegeHunterOnApply), new TextObject("{=FhZPjhli}Your hometown was briefly placed under siege, and you were called to defend the walls. Everyone did their part to repulse the enemy assault, and everyone is justly proud of what they endured.", null), null, 0, 5, 0, 0, 0);
			TextObject textObject18 = new TextObject("{=kNXet6Um}you had a famous escapade in town.", null);
			MBList<SkillObject> mblist18 = new MBList<SkillObject>();
			mblist18.Add(DefaultSkills.Athletics);
			mblist18.Add(DefaultSkills.Roguery);
			CharacterAttribute endurance = DefaultCharacterAttributes.Endurance;
			int focusToAdd9 = this.FocusToAdd;
			int skillLevelToAdd9 = this.SkillLevelToAdd;
			int attributeLevelToAdd9 = this.AttributeLevelToAdd;
			CharacterCreationOnCondition characterCreationOnCondition9 = new CharacterCreationOnCondition(this.AccomplishmentRuralOnCondition);
			CharacterCreationOnSelect characterCreationOnSelect9 = new CharacterCreationOnSelect(this.AccomplishmentEscapadeOnConsequence);
			CharacterCreationApplyFinalEffects characterCreationApplyFinalEffects9 = new CharacterCreationApplyFinalEffects(this.AccomplishmentEscapadeOnApply);
			TextObject textObject19 = new TextObject("{=DjeAJtix}Maybe it was a love affair, or maybe you cheated at dice, or maybe you just chose your words poorly when drinking with a dangerous crowd. Anyway, on one of your trips into town you got into the kind of trouble from which only a quick tongue or quick feet get you out alive.", null);
			MBList<TraitObject> mblist19 = new MBList<TraitObject>();
			mblist19.Add(DefaultTraits.Valor);
			characterCreationCategory.AddCategoryOption(textObject18, mblist18, endurance, focusToAdd9, skillLevelToAdd9, attributeLevelToAdd9, characterCreationOnCondition9, characterCreationOnSelect9, characterCreationApplyFinalEffects9, textObject19, mblist19, 1, 5, 0, 0, 0);
			TextObject textObject20 = new TextObject("{=qlOuiKXj}you had a famous escapade.", null);
			MBList<SkillObject> mblist20 = new MBList<SkillObject>();
			mblist20.Add(DefaultSkills.Athletics);
			mblist20.Add(DefaultSkills.Roguery);
			CharacterAttribute endurance2 = DefaultCharacterAttributes.Endurance;
			int focusToAdd10 = this.FocusToAdd;
			int skillLevelToAdd10 = this.SkillLevelToAdd;
			int attributeLevelToAdd10 = this.AttributeLevelToAdd;
			CharacterCreationOnCondition characterCreationOnCondition10 = new CharacterCreationOnCondition(this.AccomplishmentUrbanOnCondition);
			CharacterCreationOnSelect characterCreationOnSelect10 = new CharacterCreationOnSelect(this.AccomplishmentEscapadeOnConsequence);
			CharacterCreationApplyFinalEffects characterCreationApplyFinalEffects10 = new CharacterCreationApplyFinalEffects(this.AccomplishmentEscapadeOnApply);
			TextObject textObject21 = new TextObject("{=lD5Ob3R4}Maybe it was a love affair, or maybe you cheated at dice, or maybe you just chose your words poorly when drinking with a dangerous crowd. Anyway, you got into the kind of trouble from which only a quick tongue or quick feet get you out alive.", null);
			MBList<TraitObject> mblist21 = new MBList<TraitObject>();
			mblist21.Add(DefaultTraits.Valor);
			characterCreationCategory.AddCategoryOption(textObject20, mblist20, endurance2, focusToAdd10, skillLevelToAdd10, attributeLevelToAdd10, characterCreationOnCondition10, characterCreationOnSelect10, characterCreationApplyFinalEffects10, textObject21, mblist21, 1, 5, 0, 0, 0);
			TextObject textObject22 = new TextObject("{=Yqm0Dics}you treated people well.", null);
			MBList<SkillObject> mblist22 = new MBList<SkillObject>();
			mblist22.Add(DefaultSkills.Charm);
			mblist22.Add(DefaultSkills.Steward);
			CharacterAttribute social = DefaultCharacterAttributes.Social;
			int focusToAdd11 = this.FocusToAdd;
			int skillLevelToAdd11 = this.SkillLevelToAdd;
			int attributeLevelToAdd11 = this.AttributeLevelToAdd;
			CharacterCreationOnCondition characterCreationOnCondition11 = null;
			CharacterCreationOnSelect characterCreationOnSelect11 = new CharacterCreationOnSelect(this.AccomplishmentTreaterOnConsequence);
			CharacterCreationApplyFinalEffects characterCreationApplyFinalEffects11 = new CharacterCreationApplyFinalEffects(this.AccomplishmentTreaterOnApply);
			TextObject textObject23 = new TextObject("{=dDmcqTzb}Yours wasn't the kind of reputation that local legends are made of, but it was the kind that wins you respect among those around you. You were consistently fair and honest in your business dealings and helpful to those in trouble. In doing so, you got a sense of what made people tick.", null);
			MBList<TraitObject> mblist23 = new MBList<TraitObject>();
			mblist23.Add(DefaultTraits.Mercy);
			mblist23.Add(DefaultTraits.Generosity);
			mblist23.Add(DefaultTraits.Honor);
			characterCreationCategory.AddCategoryOption(textObject22, mblist22, social, focusToAdd11, skillLevelToAdd11, attributeLevelToAdd11, characterCreationOnCondition11, characterCreationOnSelect11, characterCreationApplyFinalEffects11, textObject23, mblist23, 1, 5, 0, 0, 0);
			characterCreation.AddNewMenu(characterCreationMenu);
		}

		protected void AddEscapeMenu(CharacterCreation characterCreation)
		{
			MBTextManager.SetTextVariable("EXP_VALUE", this.SkillLevelToAdd);
			CharacterCreationMenu characterCreationMenu = new CharacterCreationMenu(new TextObject("{=peNBA0WW}Story Background", null), new TextObject("{=jg3T5AyE}Like many families in Calradia, your life was upended by war. Your home was ravaged by the passage of army after army. Eventually, you sold your property and set off with your father, mother, brother, and your two younger siblings to a new town you'd heard was safer. But you did not make it. Along the way, the inn at which you were staying was attacked by raiders. Your parents were slain and your two youngest siblings seized, but you and your brother survived because...", null), new CharacterCreationOnInit(this.EscapeOnInit), 0);
			CharacterCreationCategory characterCreationCategory = characterCreationMenu.AddMenuCategory(null);
			TextObject textObject = new TextObject("{=6vCHovVH}you subdued a raider.", null);
			MBList<SkillObject> mblist = new MBList<SkillObject>();
			mblist.Add(DefaultSkills.OneHanded);
			mblist.Add(DefaultSkills.Athletics);
			characterCreationCategory.AddCategoryOption(textObject, mblist, DefaultCharacterAttributes.Vigor, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.EscapeSubdueRaiderOnConsequence), new CharacterCreationApplyFinalEffects(this.EscapeSubdueRaiderOnApply), new TextObject("{=CvBoRaFv}You were able to grab a knife in the confusion of the attack. You stabbed a raider blocking your way.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject2 = new TextObject("{=2XhW49TX}you drove them off with arrows.", null);
			MBList<SkillObject> mblist2 = new MBList<SkillObject>();
			mblist2.Add(DefaultSkills.Bow);
			mblist2.Add(DefaultSkills.Tactics);
			characterCreationCategory.AddCategoryOption(textObject2, mblist2, DefaultCharacterAttributes.Control, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.EscapeDrawArrowsOnConsequence), new CharacterCreationApplyFinalEffects(this.EscapeDrawArrowsOnApply), new TextObject("{=ccf67J3J}You grabbed a bow and sent a few arrows the raiders' way. They took cover, giving you the opportunity to flee with your brother.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject3 = new TextObject("{=gOI8lKcl}you rode off on a fast horse.", null);
			MBList<SkillObject> mblist3 = new MBList<SkillObject>();
			mblist3.Add(DefaultSkills.Riding);
			mblist3.Add(DefaultSkills.Scouting);
			characterCreationCategory.AddCategoryOption(textObject3, mblist3, DefaultCharacterAttributes.Endurance, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.EscapeFastHorseOnConsequence), new CharacterCreationApplyFinalEffects(this.EscapeFastHorseOnApply), new TextObject("{=cepWNzEA}Jumping on the two remaining horses in the inn's burning stable, you and your brother broke out of the encircling raiders and rode off.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject4 = new TextObject("{=EdUppdLZ}you tricked the raiders.", null);
			MBList<SkillObject> mblist4 = new MBList<SkillObject>();
			mblist4.Add(DefaultSkills.Roguery);
			mblist4.Add(DefaultSkills.Tactics);
			characterCreationCategory.AddCategoryOption(textObject4, mblist4, DefaultCharacterAttributes.Cunning, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.EscapeRoadOffWithBrotherOnConsequence), new CharacterCreationApplyFinalEffects(this.EscapeRoadOffWithBrotherOnApply), new TextObject("{=ZqOvtLBM}In the confusion of the attack you shouted that someone had found treasure in the back room. You then made your way out of the undefended entrance with your brother.", null), null, 0, 0, 0, 0, 0);
			TextObject textObject5 = new TextObject("{=qhAhPWdp}you organized the travelers to break out.", null);
			MBList<SkillObject> mblist5 = new MBList<SkillObject>();
			mblist5.Add(DefaultSkills.Leadership);
			mblist5.Add(DefaultSkills.Charm);
			characterCreationCategory.AddCategoryOption(textObject5, mblist5, DefaultCharacterAttributes.Social, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, new CharacterCreationOnSelect(this.EscapeOrganizeTravellersOnConsequence), new CharacterCreationApplyFinalEffects(this.EscapeOrganizeTravellersOnApply), new TextObject("{=Lmfi0cYk}You encouraged the few travellers in the inn to break out in a coordinated fashion. Raiders killed or captured most but you and your brother were able to escape.", null), null, 0, 0, 0, 0, 0);
			characterCreation.AddNewMenu(characterCreationMenu);
		}

		protected void ParentsOnInit(CharacterCreation characterCreation)
		{
			characterCreation.IsPlayerAlone = false;
			characterCreation.HasSecondaryCharacter = false;
			this.ClearMountEntity(characterCreation);
			characterCreation.ClearFaceGenPrefab();
			if (base.PlayerBodyProperties != CharacterObject.PlayerCharacter.GetBodyProperties(CharacterObject.PlayerCharacter.Equipment, -1))
			{
				base.PlayerBodyProperties = CharacterObject.PlayerCharacter.GetBodyProperties(CharacterObject.PlayerCharacter.Equipment, -1);
				BodyProperties playerBodyProperties = base.PlayerBodyProperties;
				BodyProperties playerBodyProperties2 = base.PlayerBodyProperties;
				FaceGen.GenerateParentKey(base.PlayerBodyProperties, CharacterObject.PlayerCharacter.Race, ref playerBodyProperties, ref playerBodyProperties2);
				playerBodyProperties..ctor(new DynamicBodyProperties(33f, 0.3f, 0.2f), playerBodyProperties.StaticProperties);
				playerBodyProperties2..ctor(new DynamicBodyProperties(33f, 0.5f, 0.5f), playerBodyProperties2.StaticProperties);
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
			Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(characterCreation.FaceGenChars[0].Race);
			if (motherItemId != "")
			{
				ItemObject object3 = Game.Current.ObjectManager.GetObject<ItemObject>(motherItemId);
				if (object3 != null)
				{
					equipment.AddEquipmentToSlotWithoutAgent(isLeftHandItemForMother ? 0 : 1, new EquipmentElement(object3, null, null, false));
				}
				else
				{
					characterCreation.ChangeCharacterPrefab(motherItemId, isLeftHandItemForMother ? baseMonsterFromRace.MainHandItemBoneIndex : baseMonsterFromRace.OffHandItemBoneIndex);
				}
			}
			if (fatherItemId != "")
			{
				ItemObject object4 = Game.Current.ObjectManager.GetObject<ItemObject>(fatherItemId);
				if (object4 != null)
				{
					equipment2.AddEquipmentToSlotWithoutAgent(isLeftHandItemForFather ? 0 : 1, new EquipmentElement(object4, null, null, false));
				}
			}
			list.Add(equipment);
			list.Add(equipment2);
			characterCreation.ChangeCharactersEquipment(list);
		}

		protected void ChangeParentsAnimation(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string>
			{
				"anim_mother_" + base.SelectedParentType,
				"anim_father_" + base.SelectedParentType
			});
		}

		protected void SetParentAndOccupationType(CharacterCreation characterCreation, int parentType, StoryModeCharacterCreationContent.OccupationTypes occupationType, string fatherItemId = "", string motherItemId = "", bool isLeftHandItemForFather = true, bool isLeftHandItemForMother = true)
		{
			base.SelectedParentType = parentType;
			this._familyOccupationType = occupationType;
			characterCreation.ChangeFaceGenChars(new List<FaceGenChar> { base.MotherFacegenCharacter, base.FatherFacegenCharacter });
			this.ChangeParentsAnimation(characterCreation);
			this.ChangeParentsOutfit(characterCreation, fatherItemId, motherItemId, isLeftHandItemForFather, isLeftHandItemForMother);
		}

		protected void EmpireLandlordsRetainerOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 1, StoryModeCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
		}

		protected void EmpireMerchantOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 2, StoryModeCharacterCreationContent.OccupationTypes.Merchant, "", "", true, true);
		}

		protected void EmpireFreeholderOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 3, StoryModeCharacterCreationContent.OccupationTypes.Farmer, "", "", true, true);
		}

		protected void EmpireArtisanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 4, StoryModeCharacterCreationContent.OccupationTypes.Artisan, "", "", true, true);
		}

		protected void EmpireWoodsmanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 5, StoryModeCharacterCreationContent.OccupationTypes.Hunter, "", "", true, true);
		}

		protected void EmpireVagabondOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 6, StoryModeCharacterCreationContent.OccupationTypes.Vagabond, "", "", true, true);
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
			this.SetParentAndOccupationType(characterCreation, 1, StoryModeCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
		}

		protected void VlandiaMerchantOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 2, StoryModeCharacterCreationContent.OccupationTypes.Merchant, "", "", true, true);
		}

		protected void VlandiaYeomanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 3, StoryModeCharacterCreationContent.OccupationTypes.Farmer, "", "", true, true);
		}

		protected void VlandiaBlacksmithOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 4, StoryModeCharacterCreationContent.OccupationTypes.Artisan, "", "", true, true);
		}

		protected void VlandiaHunterOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 5, StoryModeCharacterCreationContent.OccupationTypes.Hunter, "", "", true, true);
		}

		protected void VlandiaMercenaryOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 6, StoryModeCharacterCreationContent.OccupationTypes.Mercenary, "", "", true, true);
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
			this.SetParentAndOccupationType(characterCreation, 1, StoryModeCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
		}

		protected void SturgiaTraderOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 2, StoryModeCharacterCreationContent.OccupationTypes.Merchant, "", "", true, true);
		}

		protected void SturgiaFreemanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 3, StoryModeCharacterCreationContent.OccupationTypes.Farmer, "", "", true, true);
		}

		protected void SturgiaArtisanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 4, StoryModeCharacterCreationContent.OccupationTypes.Artisan, "", "", true, true);
		}

		protected void SturgiaHunterOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 5, StoryModeCharacterCreationContent.OccupationTypes.Hunter, "", "", true, true);
		}

		protected void SturgiaVagabondOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 6, StoryModeCharacterCreationContent.OccupationTypes.Vagabond, "", "", true, true);
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
			this.SetParentAndOccupationType(characterCreation, 1, StoryModeCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
		}

		protected void AseraiWariorSlaveOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 2, StoryModeCharacterCreationContent.OccupationTypes.Mercenary, "", "", true, true);
		}

		protected void AseraiMerchantOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 3, StoryModeCharacterCreationContent.OccupationTypes.Merchant, "", "", true, true);
		}

		protected void AseraiOasisFarmerOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 4, StoryModeCharacterCreationContent.OccupationTypes.Farmer, "", "", true, true);
		}

		protected void AseraiBedouinOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 5, StoryModeCharacterCreationContent.OccupationTypes.Herder, "", "", true, true);
		}

		protected void AseraiBackAlleyThugOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 6, StoryModeCharacterCreationContent.OccupationTypes.Artisan, "", "", true, true);
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
			this.SetParentAndOccupationType(characterCreation, 1, StoryModeCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
		}

		protected void BattaniaHealerOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 2, StoryModeCharacterCreationContent.OccupationTypes.Healer, "", "", true, true);
		}

		protected void BattaniaTribesmanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 3, StoryModeCharacterCreationContent.OccupationTypes.Farmer, "", "", true, true);
		}

		protected void BattaniaSmithOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 4, StoryModeCharacterCreationContent.OccupationTypes.Artisan, "", "", true, true);
		}

		protected void BattaniaWoodsmanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 5, StoryModeCharacterCreationContent.OccupationTypes.Hunter, "", "", true, true);
		}

		protected void BattaniaBardOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 6, StoryModeCharacterCreationContent.OccupationTypes.Bard, "", "", true, true);
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
			this.SetParentAndOccupationType(characterCreation, 1, StoryModeCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
		}

		protected void KhuzaitMerchantOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 2, StoryModeCharacterCreationContent.OccupationTypes.Merchant, "", "", true, true);
		}

		protected void KhuzaitTribesmanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 3, StoryModeCharacterCreationContent.OccupationTypes.Herder, "", "", true, true);
		}

		protected void KhuzaitFarmerOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 4, StoryModeCharacterCreationContent.OccupationTypes.Farmer, "", "", true, true);
		}

		protected void KhuzaitShamanOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 5, StoryModeCharacterCreationContent.OccupationTypes.Healer, "", "", true, true);
		}

		protected void KhuzaitNomadOnConsequence(CharacterCreation characterCreation)
		{
			this.SetParentAndOccupationType(characterCreation, 6, StoryModeCharacterCreationContent.OccupationTypes.Herder, "", "", true, true);
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
			CharacterObject characterObject = StoryModeHeroes.ElderBrother.CharacterObject;
			@object.HeroObject.ModifyPlayersFamilyAppearance(base.MotherFacegenCharacter.BodyProperties.StaticProperties);
			object2.HeroObject.ModifyPlayersFamilyAppearance(base.FatherFacegenCharacter.BodyProperties.StaticProperties);
			@object.HeroObject.Weight = base.MotherFacegenCharacter.BodyProperties.Weight;
			@object.HeroObject.Build = base.MotherFacegenCharacter.BodyProperties.Build;
			object2.HeroObject.Weight = base.FatherFacegenCharacter.BodyProperties.Weight;
			object2.HeroObject.Build = base.FatherFacegenCharacter.BodyProperties.Build;
			EquipmentHelper.AssignHeroEquipmentFromEquipment(@object.HeroObject, base.MotherFacegenCharacter.Equipment);
			EquipmentHelper.AssignHeroEquipmentFromEquipment(object2.HeroObject, base.FatherFacegenCharacter.Equipment);
			EquipmentHelper.AssignHeroEquipmentFromEquipment(characterObject.HeroObject, characterObject.Equipment);
			@object.Culture = Hero.MainHero.Culture;
			object2.Culture = Hero.MainHero.Culture;
			characterObject.Culture = Hero.MainHero.Culture;
			StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, null, false);
			TextObject textObject = GameTexts.FindText("str_player_little_brother_name", Hero.MainHero.Culture.StringId);
			StoryModeHeroes.LittleBrother.SetName(textObject, textObject);
			StoryModeHeroes.LittleBrother.SetHasMet();
			TextObject textObject2 = new TextObject("{=h68qCoz3}{PLAYER_LITTLE_BROTHER.NAME} is the little brother of {PLAYER.LINK}. He has been abducted by bandits, who intend to sell him into slavery.", null);
			StringHelpers.SetCharacterProperties("PLAYER_LITTLE_BROTHER", StoryModeHeroes.LittleBrother.CharacterObject, textObject2, false);
			StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject2, false);
			StoryModeHeroes.LittleBrother.EncyclopediaText = textObject2;
			TextObject textObject3 = GameTexts.FindText("str_player_little_sister_name", Hero.MainHero.Culture.StringId);
			StoryModeHeroes.LittleSister.SetName(textObject3, textObject3);
			StoryModeHeroes.LittleSister.SetHasMet();
			TextObject textObject4 = new TextObject("{=epB0x816}{PLAYER_LITTLE_SISTER.NAME} is the little sister of {PLAYER.LINK}. She has been abducted by bandits, who intend to sell her into slavery.", null);
			StringHelpers.SetCharacterProperties("PLAYER_LITTLE_SISTER", StoryModeHeroes.LittleSister.CharacterObject, textObject4, false);
			StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject4, false);
			StoryModeHeroes.LittleSister.EncyclopediaText = textObject4;
			TextObject textObject5 = GameTexts.FindText("str_player_father_name", Hero.MainHero.Culture.StringId);
			object2.HeroObject.SetName(textObject5, textObject5);
			TextObject textObject6 = new TextObject("{=XmvaRfLM}{PLAYER_FATHER.NAME} was the father of {PLAYER.LINK}. He was slain when raiders attacked the inn at which his family was staying.", null);
			StringHelpers.SetCharacterProperties("PLAYER_FATHER", object2, textObject6, false);
			StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject6, false);
			object2.HeroObject.EncyclopediaText = textObject6;
			TextObject textObject7 = GameTexts.FindText("str_player_mother_name", Hero.MainHero.Culture.StringId);
			@object.HeroObject.SetName(textObject7, textObject7);
			TextObject textObject8 = new TextObject("{=hrhvEWP8}{PLAYER_MOTHER.NAME} was the mother of {PLAYER.LINK}. She was slain when raiders attacked the inn at which her family was staying.", null);
			StringHelpers.SetCharacterProperties("PLAYER_MOTHER", @object, textObject8, false);
			StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject8, false);
			@object.HeroObject.EncyclopediaText = textObject8;
			TextObject textObject9 = GameTexts.FindText("str_player_brother_name", Hero.MainHero.Culture.StringId);
			characterObject.HeroObject.SetName(textObject9, textObject9);
			TextObject textObject10 = new TextObject("{=bsWSecYa}{PLAYER_BROTHER.NAME} is the elder brother of {PLAYER.LINK}. He has gone in search of the family's two youngest siblings, {PLAYER_LITTLE_BROTHER.NAME} and {PLAYER_LITTLE_SISTER.NAME}.", null);
			StringHelpers.SetCharacterProperties("PLAYER_BROTHER", characterObject, textObject10, false);
			StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject10, false);
			StringHelpers.SetCharacterProperties("PLAYER_LITTLE_BROTHER", StoryModeHeroes.LittleBrother.CharacterObject, textObject10, false);
			StringHelpers.SetCharacterProperties("PLAYER_LITTLE_SISTER", StoryModeHeroes.LittleSister.CharacterObject, textObject10, false);
			characterObject.HeroObject.EncyclopediaText = textObject10;
			@object.HeroObject.UpdateHomeSettlement();
			object2.HeroObject.UpdateHomeSettlement();
			characterObject.HeroObject.UpdateHomeSettlement();
			@object.HeroObject.SetHasMet();
			object2.HeroObject.SetHasMet();
			characterObject.HeroObject.SetHasMet();
		}

		protected List<FaceGenChar> ChangePlayerFaceWithAge(float age, string actionName = "act_childhood_schooled")
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
				Debug.FailedAssert("item shouldn't be null!", "C:\\Develop\\MB3\\Source\\Bannerlord\\StoryMode\\CharacterCreationContent\\StoryModeCharacterCreationContent.cs", "ChangePlayerOutfit", 1036);
				equipment = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>("player_char_creation_default").DefaultEquipment;
			}
			list.Add(equipment);
			characterCreation.ChangeCharactersEquipment(list);
			return equipment;
		}

		protected void ChangePlayerMount(CharacterCreation characterCreation, Hero hero)
		{
			if (hero.CharacterObject.HasMount())
			{
				FaceGenMount faceGenMount = new FaceGenMount(MountCreationKey.GetRandomMountKey(hero.CharacterObject.Equipment[10].Item, hero.CharacterObject.GetMountKeySeed()), hero.CharacterObject.Equipment[10].Item, hero.CharacterObject.Equipment[11].Item, "act_horse_stand_1");
				characterCreation.SetFaceGenMount(faceGenMount);
			}
		}

		protected void ClearMountEntity(CharacterCreation characterCreation)
		{
			characterCreation.ClearFaceGenMounts();
		}

		protected void ClearCharacters(CharacterCreation characterCreation)
		{
			characterCreation.ClearFaceGenChars();
		}

		protected void ChildhoodOnInit(CharacterCreation characterCreation)
		{
			characterCreation.IsPlayerAlone = true;
			characterCreation.HasSecondaryCharacter = false;
			characterCreation.ClearFaceGenPrefab();
			characterCreation.ChangeFaceGenChars(this.ChangePlayerFaceWithAge((float)this.ChildhoodAge, "act_childhood_schooled"));
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
			this.ClearMountEntity(characterCreation);
		}

		protected void ChildhoodYourLeadershipSkillsOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_leader" });
		}

		protected void ChildhoodYourBrawnOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_athlete" });
		}

		protected void ChildhoodAttentionToDetailOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_memory" });
		}

		protected void ChildhoodAptitudeForNumbersOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_numbers" });
		}

		protected void ChildhoodWayWithPeopleOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_manners" });
		}

		protected void ChildhoodSkillsWithHorsesOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_animals" });
		}

		protected void ChildhoodGoodLeadingOnApply(CharacterCreation characterCreation)
		{
		}

		protected void ChildhoodGoodAthleticsOnApply(CharacterCreation characterCreation)
		{
		}

		protected void ChildhoodGoodMemoryOnApply(CharacterCreation characterCreation)
		{
		}

		protected void ChildhoodGoodMathOnApply(CharacterCreation characterCreation)
		{
		}

		protected void ChildhoodGoodMannersOnApply(CharacterCreation characterCreation)
		{
		}

		protected void ChildhoodAffinityWithAnimalsOnApply(CharacterCreation characterCreation)
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
			characterCreation.ChangeFaceGenChars(this.ChangePlayerFaceWithAge((float)this.EducationAge, "act_childhood_schooled"));
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
			this.ClearMountEntity(characterCreation);
		}

		protected bool RuralType()
		{
			return this._familyOccupationType == StoryModeCharacterCreationContent.OccupationTypes.Retainer || this._familyOccupationType == StoryModeCharacterCreationContent.OccupationTypes.Farmer || this._familyOccupationType == StoryModeCharacterCreationContent.OccupationTypes.Hunter || this._familyOccupationType == StoryModeCharacterCreationContent.OccupationTypes.Bard || this._familyOccupationType == StoryModeCharacterCreationContent.OccupationTypes.Herder || this._familyOccupationType == StoryModeCharacterCreationContent.OccupationTypes.Vagabond || this._familyOccupationType == StoryModeCharacterCreationContent.OccupationTypes.Healer || this._familyOccupationType == StoryModeCharacterCreationContent.OccupationTypes.Artisan;
		}

		protected bool RichParents()
		{
			return this._familyOccupationType == StoryModeCharacterCreationContent.OccupationTypes.Retainer || this._familyOccupationType == StoryModeCharacterCreationContent.OccupationTypes.Merchant;
		}

		protected bool RuralChildhoodOnCondition()
		{
			return this.RuralType();
		}

		protected bool UrbanChildhoodOnCondition()
		{
			return !this.RuralType();
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
				equipment.AddEquipmentToSlotWithoutAgent(isLeftHand ? 0 : 1, new EquipmentElement(itemObject, null, null, false));
				if (secondItemId != "")
				{
					itemObject = Game.Current.ObjectManager.GetObject<ItemObject>(secondItemId);
					equipment.AddEquipmentToSlotWithoutAgent(isLeftHand ? 1 : 0, new EquipmentElement(itemObject, null, null, false));
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
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_manners" });
			this.RefreshPropsAndClothing(characterCreation, false, "", true, "");
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

		protected void UrbanAdolescencePreacherOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_leader" });
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

		protected void RuralAdolescenceHerderOnApply(CharacterCreation characterCreation)
		{
		}

		protected void RuralAdolescenceSmithyOnApply(CharacterCreation characterCreation)
		{
		}

		protected void RuralAdolescenceRepairmanOnApply(CharacterCreation characterCreation)
		{
		}

		protected void RuralAdolescenceGathererOnApply(CharacterCreation characterCreation)
		{
		}

		protected void RuralAdolescenceHunterOnApply(CharacterCreation characterCreation)
		{
		}

		protected void RuralAdolescenceHelperOnApply(CharacterCreation characterCreation)
		{
		}

		protected void UrbanAdolescenceWatcherOnApply(CharacterCreation characterCreation)
		{
		}

		protected void UrbanAdolescenceMarketerOnApply(CharacterCreation characterCreation)
		{
		}

		protected void UrbanAdolescencePreacherOnApply(CharacterCreation characterCreation)
		{
		}

		protected void UrbanAdolescenceGangerOnApply(CharacterCreation characterCreation)
		{
		}

		protected void UrbanAdolescenceDockerOnApply(CharacterCreation characterCreation)
		{
		}

		protected void UrbanAdolescenceTutorOnApply(CharacterCreation characterCreation)
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
			characterCreation.ChangeFaceGenChars(this.ChangePlayerFaceWithAge((float)this.YouthAge, "act_childhood_schooled"));
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
			return base.GetSelectedCulture().StringId == "empire" && this._familyOccupationType == StoryModeCharacterCreationContent.OccupationTypes.Retainer;
		}

		protected void YouthCommanderOnApply(CharacterCreation characterCreation)
		{
		}

		protected bool YouthGroomOnCondition()
		{
			return base.GetSelectedCulture().StringId == "vlandia" && this._familyOccupationType == StoryModeCharacterCreationContent.OccupationTypes.Retainer;
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
			return (base.GetSelectedCulture().StringId == "battania" || base.GetSelectedCulture().StringId == "khuzait") && this._familyOccupationType == StoryModeCharacterCreationContent.OccupationTypes.Retainer;
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
			return this._familyOccupationType != StoryModeCharacterCreationContent.OccupationTypes.Retainer;
		}

		protected void YouthCamperOnApply(CharacterCreation characterCreation)
		{
		}

		protected void AccomplishmentOnInit(CharacterCreation characterCreation)
		{
			characterCreation.IsPlayerAlone = true;
			characterCreation.HasSecondaryCharacter = false;
			characterCreation.ClearFaceGenPrefab();
			characterCreation.ChangeFaceGenChars(this.ChangePlayerFaceWithAge((float)this.AccomplishmentAge, "act_childhood_schooled"));
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
			return this._familyOccupationType == StoryModeCharacterCreationContent.OccupationTypes.Merchant;
		}

		protected bool AccomplishmentPosseOnConditions()
		{
			return this._familyOccupationType == StoryModeCharacterCreationContent.OccupationTypes.Retainer || this._familyOccupationType == StoryModeCharacterCreationContent.OccupationTypes.Herder || this._familyOccupationType == StoryModeCharacterCreationContent.OccupationTypes.Mercenary;
		}

		protected bool AccomplishmentSavedVillageOnCondition()
		{
			return this.RuralType() && this._familyOccupationType != StoryModeCharacterCreationContent.OccupationTypes.Retainer && this._familyOccupationType != StoryModeCharacterCreationContent.OccupationTypes.Herder;
		}

		protected bool AccomplishmentSavedStreetOnCondition()
		{
			return !this.RuralType() && this._familyOccupationType != StoryModeCharacterCreationContent.OccupationTypes.Merchant && this._familyOccupationType != StoryModeCharacterCreationContent.OccupationTypes.Mercenary;
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

		protected void EscapeOnInit(CharacterCreation characterCreation)
		{
			characterCreation.IsPlayerAlone = false;
			characterCreation.HasSecondaryCharacter = true;
			characterCreation.ClearFaceGenPrefab();
			this.ClearCharacters(characterCreation);
			this.ClearMountEntity(characterCreation);
			Hero elderBrother = StoryModeHeroes.ElderBrother;
			List<FaceGenChar> list = new List<FaceGenChar>();
			BodyProperties bodyProperties = CharacterObject.PlayerCharacter.GetBodyProperties(CharacterObject.PlayerCharacter.Equipment, -1);
			bodyProperties = FaceGen.GetBodyPropertiesWithAge(ref bodyProperties, 23f);
			this.CreateSibling(StoryModeHeroes.LittleBrother);
			this.CreateSibling(StoryModeHeroes.LittleSister);
			BodyProperties randomBodyProperties = BodyProperties.GetRandomBodyProperties(elderBrother.CharacterObject.Race, elderBrother.IsFemale, base.MotherFacegenCharacter.BodyProperties, base.FatherFacegenCharacter.BodyProperties, 1, Hero.MainHero.Mother.CharacterObject.GetDefaultFaceSeed(1), Hero.MainHero.Father.HairTags, Hero.MainHero.Father.BeardTags, Hero.MainHero.Father.TattooTags);
			randomBodyProperties..ctor(new DynamicBodyProperties(elderBrother.Age, 0.5f, 0.5f), randomBodyProperties.StaticProperties);
			elderBrother.ModifyPlayersFamilyAppearance(randomBodyProperties.StaticProperties);
			elderBrother.Weight = randomBodyProperties.Weight;
			elderBrother.Build = randomBodyProperties.Build;
			list.Add(new FaceGenChar(bodyProperties, CharacterObject.PlayerCharacter.Race, new Equipment(), CharacterObject.PlayerCharacter.IsFemale, "act_childhood_schooled"));
			list.Add(new FaceGenChar(elderBrother.BodyProperties, CharacterObject.PlayerCharacter.Race, new Equipment(), elderBrother.CharacterObject.IsFemale, "act_childhood_schooled"));
			characterCreation.ChangeFaceGenChars(list);
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_schooled", "act_childhood_schooled" });
			this.ChangeStoryStageEquipments(characterCreation);
			List<FaceGenMount> list2 = new List<FaceGenMount>();
			if (CharacterObject.PlayerCharacter.HasMount())
			{
				ItemObject item = CharacterObject.PlayerCharacter.Equipment[10].Item;
				list2.Add(new FaceGenMount(MountCreationKey.GetRandomMountKey(item, CharacterObject.PlayerCharacter.GetMountKeySeed()), CharacterObject.PlayerCharacter.Equipment[10].Item, CharacterObject.PlayerCharacter.Equipment[11].Item, "act_inventory_idle_start"));
			}
			if (elderBrother.CharacterObject.HasMount())
			{
				ItemObject item2 = elderBrother.CharacterObject.Equipment[10].Item;
				list2.Add(new FaceGenMount(MountCreationKey.GetRandomMountKey(item2, elderBrother.CharacterObject.GetMountKeySeed()), elderBrother.CharacterObject.Equipment[10].Item, elderBrother.CharacterObject.Equipment[11].Item, "act_inventory_idle_start"));
			}
		}

		protected void ChangeStoryStageEquipments(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharactersEquipment(new List<Equipment>
			{
				CharacterObject.PlayerCharacter.Equipment,
				StoryModeHeroes.ElderBrother.CharacterObject.Equipment
			});
		}

		protected void EscapeSubdueRaiderOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_fierce", "act_childhood_athlete" });
		}

		protected void EscapeDrawArrowsOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_gracious", "act_childhood_sharp" });
		}

		protected void EscapeFastHorseOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_tough", "act_childhood_decisive" });
		}

		protected void EscapeRoadOffWithBrotherOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_ready", "act_childhood_tough" });
		}

		protected void EscapeOrganizeTravellersOnConsequence(CharacterCreation characterCreation)
		{
			characterCreation.ChangeCharsAnimation(new List<string> { "act_childhood_manners", "act_childhood_gracious" });
		}

		protected void EscapeSubdueRaiderOnApply(CharacterCreation characterCreation)
		{
		}

		protected void EscapeDrawArrowsOnApply(CharacterCreation characterCreation)
		{
		}

		protected void EscapeFastHorseOnApply(CharacterCreation characterCreation)
		{
		}

		protected void EscapeRoadOffWithBrotherOnApply(CharacterCreation characterCreation)
		{
		}

		protected void EscapeOrganizeTravellersOnApply(CharacterCreation characterCreation)
		{
		}

		protected void ApplyEquipments(CharacterCreation characterCreation)
		{
			this.ClearMountEntity(characterCreation);
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
			base.PlayerCivilianEquipment = ((@object != null) ? MBEquipmentRosterExtensions.GetCivilianEquipments(@object).FirstOrDefault<Equipment>() : null) ?? MBEquipmentRoster.EmptyEquipment;
			if (base.PlayerStartEquipment != null && base.PlayerCivilianEquipment != null)
			{
				CharacterObject.PlayerCharacter.Equipment.FillFrom(base.PlayerStartEquipment, true);
				CharacterObject.PlayerCharacter.FirstCivilianEquipment.FillFrom(base.PlayerCivilianEquipment, true);
			}
			this.ChangePlayerMount(characterCreation, Hero.MainHero);
			this.ApplyBrotherClothingAndEquipment();
		}

		protected void ApplyBrotherClothingAndEquipment()
		{
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>("brother_char_creation_" + base.GetSelectedCulture().StringId);
			Hero elderBrother = StoryModeHeroes.ElderBrother;
			elderBrother.CharacterObject.Equipment.FillFrom(((@object != null) ? @object.DefaultEquipment : null) ?? MBEquipmentRoster.EmptyEquipment, true);
			elderBrother.CharacterObject.FirstCivilianEquipment.FillFrom(((@object != null) ? MBEquipmentRosterExtensions.GetCivilianEquipments(@object).FirstOrDefault<Equipment>() : null) ?? MBEquipmentRoster.EmptyEquipment, true);
		}

		protected void CreateSibling(Hero hero)
		{
			BodyProperties randomBodyProperties = BodyProperties.GetRandomBodyProperties(hero.CharacterObject.Race, hero.IsFemale, base.MotherFacegenCharacter.BodyProperties, base.FatherFacegenCharacter.BodyProperties, 1, Hero.MainHero.Mother.CharacterObject.GetDefaultFaceSeed(1), hero.IsFemale ? Hero.MainHero.Mother.HairTags : Hero.MainHero.Father.HairTags, hero.IsFemale ? Hero.MainHero.Mother.BeardTags : Hero.MainHero.Father.BeardTags, hero.IsFemale ? Hero.MainHero.Mother.TattooTags : Hero.MainHero.Father.TattooTags);
			randomBodyProperties..ctor(new DynamicBodyProperties(hero.Age, 0.5f, 0.5f), randomBodyProperties.StaticProperties);
			hero.ModifyPlayersFamilyAppearance(randomBodyProperties.StaticProperties);
			hero.Weight = randomBodyProperties.Weight;
			hero.Build = randomBodyProperties.Build;
		}

		protected StoryModeCharacterCreationContent.OccupationTypes _familyOccupationType;

		protected TextObject _educationIntroductoryText = new TextObject("{=!}{EDUCATION_INTRO}", null);

		protected TextObject _youthIntroductoryText = new TextObject("{=!}{YOUTH_INTRO}", null);

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

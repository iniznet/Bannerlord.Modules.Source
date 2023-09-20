using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CharacterCreationContent
{
	public class CharacterCreation
	{
		public bool IsPlayerAlone { get; set; }

		public bool HasSecondaryCharacter { get; set; }

		public string PrefabId { get; private set; }

		public sbyte PrefabBoneUsage { get; private set; }

		public MBReadOnlyList<FaceGenChar> FaceGenChars
		{
			get
			{
				return this._faceGenChars;
			}
		}

		public FaceGenMount FaceGenMount { get; private set; }

		public bool CharsEquipmentNeedsRefresh { get; private set; }

		public bool CharsNeedsRefresh { get; set; }

		public bool MountsNeedsRefresh { get; set; }

		public string Name { get; set; }

		public int CharacterCreationMenuCount
		{
			get
			{
				return this.CharacterCreationMenus.Count;
			}
		}

		public void ChangeFaceGenChars(List<FaceGenChar> newChars)
		{
			this._faceGenChars.Clear();
			foreach (FaceGenChar faceGenChar in newChars)
			{
				this._faceGenChars.Add(faceGenChar);
			}
			this.CharsNeedsRefresh = true;
		}

		public void SetFaceGenMount(FaceGenMount newMount)
		{
			this.FaceGenMount = null;
			if (newMount != null)
			{
				this.FaceGenMount = newMount;
			}
			this.MountsNeedsRefresh = true;
		}

		public void ClearFaceGenMounts()
		{
			this.FaceGenMount = null;
			this.MountsNeedsRefresh = true;
		}

		public void ClearFaceGenChars()
		{
			this._faceGenChars.Clear();
			this.CharsNeedsRefresh = true;
		}

		public void ClearFaceGenPrefab()
		{
			this.PrefabId = "";
			this.PrefabBoneUsage = 0;
		}

		public void ChangeCharactersEquipment(List<Equipment> equipmentList)
		{
			for (int i = 0; i < equipmentList.Count; i++)
			{
				this._faceGenChars[i].Equipment.FillFrom(equipmentList[i], true);
			}
			this.CharsEquipmentNeedsRefresh = true;
		}

		public void ClearCharactersEquipment()
		{
			for (int i = 0; i < this._faceGenChars.Count; i++)
			{
				this._faceGenChars[i].Equipment.FillFrom(new Equipment(), true);
			}
			this.CharsEquipmentNeedsRefresh = true;
		}

		public void ChangeCharacterPrefab(string id, sbyte boneUsage)
		{
			this.PrefabId = id;
			this.PrefabBoneUsage = boneUsage;
		}

		public void ChangeCharsAnimation(List<string> actionList)
		{
			for (int i = 0; i < actionList.Count; i++)
			{
				this._faceGenChars[i].ActionName = actionList[i];
			}
			this.CharsEquipmentNeedsRefresh = true;
		}

		public void ChangeMountsAnimation(string action)
		{
			this.FaceGenMount.ActionName = action;
			this.MountsNeedsRefresh = true;
		}

		public CharacterCreation()
		{
			this._faceGenChars = new MBList<FaceGenChar>();
			this.CharacterCreationMenus = new List<CharacterCreationMenu>();
			this.CharsEquipmentNeedsRefresh = false;
		}

		public void AddNewMenu(CharacterCreationMenu menu)
		{
			this.CharacterCreationMenus.Add(menu);
		}

		public CharacterCreationMenu GetCurrentMenu(int index)
		{
			if (index >= 0 && index < this.CharacterCreationMenus.Count)
			{
				return this.CharacterCreationMenus[index];
			}
			return null;
		}

		public IEnumerable<CharacterCreationOption> GetCurrentMenuOptions(int index)
		{
			List<CharacterCreationOption> list = new List<CharacterCreationOption>();
			CharacterCreationMenu currentMenu = this.GetCurrentMenu(index);
			if (currentMenu != null)
			{
				foreach (CharacterCreationCategory characterCreationCategory in currentMenu.CharacterCreationCategories)
				{
					CharacterCreationOnCondition categoryCondition = characterCreationCategory.CategoryCondition;
					if (categoryCondition == null || categoryCondition())
					{
						foreach (CharacterCreationOption characterCreationOption in characterCreationCategory.CharacterCreationOptions)
						{
							if (characterCreationOption.OnCondition == null || characterCreationOption.OnCondition())
							{
								list.Add(characterCreationOption);
							}
						}
					}
				}
			}
			return list;
		}

		public void ResetMenuOptions()
		{
			for (int i = 0; i < this.CharacterCreationMenus.Count; i++)
			{
				this.CharacterCreationMenus[i].SelectedOptions.Clear();
			}
		}

		public void OnInit(int stage)
		{
			if (this.CharacterCreationMenus[stage].OnInit != null)
			{
				this.CharacterCreationMenus[stage].OnInit(this);
			}
		}

		public TextObject GetCurrentMenuText(int stage)
		{
			StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, null, false);
			return this.CharacterCreationMenus[stage].Text;
		}

		public TextObject GetCurrentMenuTitle(int stage)
		{
			return this.CharacterCreationMenus[stage].Title;
		}

		public void RunConsequence(CharacterCreationOption option, int stage, bool fromInit)
		{
			if (this.CharacterCreationMenus[stage].MenuType == CharacterCreationMenu.MenuTypes.MultipleChoice)
			{
				this.CharacterCreationMenus[stage].SelectedOptions.Clear();
			}
			if (!fromInit && this.CharacterCreationMenus[stage].SelectedOptions.Contains(option.Id))
			{
				this.CharacterCreationMenus[stage].SelectedOptions.Remove(option.Id);
				return;
			}
			this.CharacterCreationMenus[stage].SelectedOptions.Add(option.Id);
			if (option.OnSelect != null)
			{
				option.OnSelect(this);
			}
		}

		public IEnumerable<int> GetSelectedOptions(int stage)
		{
			return this.CharacterCreationMenus[stage].SelectedOptions;
		}

		public void ApplyFinalEffects()
		{
			Clan.PlayerClan.Renown = 0f;
			CharacterCreationContentBase.Instance.ApplyCulture(this);
			foreach (CharacterCreationMenu characterCreationMenu in this.CharacterCreationMenus)
			{
				characterCreationMenu.ApplyFinalEffect(this);
			}
			Campaign.Current.PlayerTraitDeveloper.UpdateTraitXPAccordingToTraitLevels();
		}

		private readonly MBList<FaceGenChar> _faceGenChars;

		public readonly List<CharacterCreationMenu> CharacterCreationMenus;
	}
}

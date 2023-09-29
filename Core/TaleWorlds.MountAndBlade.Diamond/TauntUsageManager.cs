using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Diamond
{
	public static class TauntUsageManager
	{
		static TauntUsageManager()
		{
			TauntUsageManager.Read();
		}

		public static void Read()
		{
			foreach (object obj in TauntUsageManager.LoadXmlFile(ModuleHelper.GetModuleFullPath("Multiplayer") + "ModuleData/taunt_usage_sets.xml").DocumentElement.SelectNodes("taunt_usage_set"))
			{
				XmlNode xmlNode = (XmlNode)obj;
				string innerText = xmlNode.Attributes["id"].InnerText;
				TauntUsageManager._tauntUsageSets.Add(new TauntUsageManager.TauntUsageSet());
				TauntUsageManager._tauntUsageSetIndexMap[innerText] = TauntUsageManager._tauntUsageSets.Count - 1;
				foreach (object obj2 in xmlNode.SelectNodes("taunt_usage"))
				{
					XmlNode xmlNode2 = (XmlNode)obj2;
					TauntUsageManager.TauntUsage.TauntUsageFlag tauntUsageFlag = TauntUsageManager.TauntUsage.TauntUsageFlag.None;
					XmlAttribute xmlAttribute = xmlNode2.Attributes["requires_bow"];
					if (bool.Parse(((xmlAttribute != null) ? xmlAttribute.Value : null) ?? "False"))
					{
						tauntUsageFlag |= TauntUsageManager.TauntUsage.TauntUsageFlag.RequiresBow;
					}
					XmlAttribute xmlAttribute2 = xmlNode2.Attributes["requires_on_foot"];
					if (bool.Parse(((xmlAttribute2 != null) ? xmlAttribute2.Value : null) ?? "False"))
					{
						tauntUsageFlag |= TauntUsageManager.TauntUsage.TauntUsageFlag.RequiresOnFoot;
					}
					XmlAttribute xmlAttribute3 = xmlNode2.Attributes["requires_shield"];
					if (bool.Parse(((xmlAttribute3 != null) ? xmlAttribute3.Value : null) ?? "False"))
					{
						tauntUsageFlag |= TauntUsageManager.TauntUsage.TauntUsageFlag.RequiresShield;
					}
					XmlAttribute xmlAttribute4 = xmlNode2.Attributes["is_left_stance"];
					if (bool.Parse(((xmlAttribute4 != null) ? xmlAttribute4.Value : null) ?? "False"))
					{
						tauntUsageFlag |= TauntUsageManager.TauntUsage.TauntUsageFlag.IsLeftStance;
					}
					XmlAttribute xmlAttribute5 = xmlNode2.Attributes["unsuitable_for_two_handed"];
					if (bool.Parse(((xmlAttribute5 != null) ? xmlAttribute5.Value : null) ?? "False"))
					{
						tauntUsageFlag |= TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForTwoHanded;
					}
					XmlAttribute xmlAttribute6 = xmlNode2.Attributes["unsuitable_for_one_handed"];
					if (bool.Parse(((xmlAttribute6 != null) ? xmlAttribute6.Value : null) ?? "False"))
					{
						tauntUsageFlag |= TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForOneHanded;
					}
					XmlAttribute xmlAttribute7 = xmlNode2.Attributes["unsuitable_for_shield"];
					if (bool.Parse(((xmlAttribute7 != null) ? xmlAttribute7.Value : null) ?? "False"))
					{
						tauntUsageFlag |= TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForShield;
					}
					XmlAttribute xmlAttribute8 = xmlNode2.Attributes["unsuitable_for_bow"];
					if (bool.Parse(((xmlAttribute8 != null) ? xmlAttribute8.Value : null) ?? "False"))
					{
						tauntUsageFlag |= TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForBow;
					}
					XmlAttribute xmlAttribute9 = xmlNode2.Attributes["unsuitable_for_crossbow"];
					if (bool.Parse(((xmlAttribute9 != null) ? xmlAttribute9.Value : null) ?? "False"))
					{
						tauntUsageFlag |= TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForCrossbow;
					}
					XmlAttribute xmlAttribute10 = xmlNode2.Attributes["unsuitable_for_empty"];
					if (bool.Parse(((xmlAttribute10 != null) ? xmlAttribute10.Value : null) ?? "False"))
					{
						tauntUsageFlag |= TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForEmpty;
					}
					string value = xmlNode2.Attributes["action"].Value;
					TauntUsageManager._tauntUsageSets.Last<TauntUsageManager.TauntUsageSet>().AddUsage(new TauntUsageManager.TauntUsage(tauntUsageFlag, value));
				}
			}
		}

		public static TauntUsageManager.TauntUsageSet GetUsageSet(string id)
		{
			int num;
			if (TauntUsageManager._tauntUsageSetIndexMap.TryGetValue(id, out num) && num >= 0 && num < TauntUsageManager._tauntUsageSets.Count)
			{
				return TauntUsageManager._tauntUsageSets[num];
			}
			return null;
		}

		public static string GetAction(int index, bool isLeftStance, bool onFoot, WeaponComponentData mainHandWeapon, WeaponComponentData offhandWeapon)
		{
			string text = null;
			foreach (TauntUsageManager.TauntUsage tauntUsage in TauntUsageManager._tauntUsageSets[index].GetUsages())
			{
				if (tauntUsage.IsSuitable(isLeftStance, onFoot, mainHandWeapon, offhandWeapon))
				{
					text = tauntUsage.GetAction();
				}
			}
			return text;
		}

		private static TextObject GetHintTextFromReasons(List<TextObject> reasons)
		{
			TextObject textObject = TextObject.Empty;
			for (int i = 0; i < reasons.Count; i++)
			{
				if (i >= 1)
				{
					GameTexts.SetVariable("STR1", textObject.ToString());
					GameTexts.SetVariable("STR2", reasons[i]);
					textObject = GameTexts.FindText("str_string_newline_string", null);
				}
				else
				{
					textObject = reasons[i];
				}
			}
			return textObject;
		}

		public static string GetActionDisabledReasonText(TauntUsageManager.TauntUsage.TauntUsageFlag disabledReasonFlag)
		{
			List<TextObject> list = new List<TextObject>();
			if (disabledReasonFlag.HasAllFlags(TauntUsageManager.TauntUsage.TauntUsageFlag.RequiresBow))
			{
				list.Add(new TextObject("{=*}Requires Bow.", null));
			}
			if (disabledReasonFlag.HasAllFlags(TauntUsageManager.TauntUsage.TauntUsageFlag.RequiresShield))
			{
				list.Add(new TextObject("{=*}Requires Shield.", null));
			}
			if (disabledReasonFlag.HasAllFlags(TauntUsageManager.TauntUsage.TauntUsageFlag.RequiresOnFoot))
			{
				list.Add(new TextObject("{=*}Can't be used while mounted.", null));
			}
			if (disabledReasonFlag.HasAllFlags(TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForTwoHanded))
			{
				list.Add(new TextObject("{=*}Can't be used with Two Handed weapons.", null));
			}
			if (disabledReasonFlag.HasAllFlags(TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForOneHanded))
			{
				list.Add(new TextObject("{=*}Can't be used with One Handed weapons.", null));
			}
			if (disabledReasonFlag.HasAllFlags(TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForShield))
			{
				list.Add(new TextObject("{=*}Can't be used with Shields.", null));
			}
			if (disabledReasonFlag.HasAllFlags(TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForBow))
			{
				list.Add(new TextObject("{=*}Can't be used with Bows.", null));
			}
			if (disabledReasonFlag.HasAllFlags(TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForCrossbow))
			{
				list.Add(new TextObject("{=*}Can't be used with Crossbows.", null));
			}
			if (disabledReasonFlag.HasAllFlags(TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForEmpty))
			{
				list.Add(new TextObject("{=*}Can't be used without a weapon.", null));
			}
			if (list.Count > 0)
			{
				return TauntUsageManager.GetHintTextFromReasons(list).ToString();
			}
			if (disabledReasonFlag.HasAllFlags(TauntUsageManager.TauntUsage.TauntUsageFlag.IsLeftStance))
			{
				return string.Empty;
			}
			return null;
		}

		public static TauntUsageManager.TauntUsage.TauntUsageFlag GetIsActionNotSuitableReason(int index, bool isLeftStance, bool onFoot, WeaponComponentData mainHandWeapon, WeaponComponentData offhandWeapon)
		{
			MBReadOnlyList<TauntUsageManager.TauntUsage> usages = TauntUsageManager._tauntUsageSets[index].GetUsages();
			if (usages.Count == 0)
			{
				Debug.FailedAssert("Taunt usages are empty", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Diamond\\TauntUsageManager.cs", "GetIsActionNotSuitableReason", 214);
				return TauntUsageManager.TauntUsage.TauntUsageFlag.None;
			}
			TauntUsageManager.TauntUsage.TauntUsageFlag[] array = new TauntUsageManager.TauntUsage.TauntUsageFlag[usages.Count];
			for (int i = 0; i < usages.Count; i++)
			{
				TauntUsageManager.TauntUsage.TauntUsageFlag isNotSuitableReason = usages[i].GetIsNotSuitableReason(isLeftStance, onFoot, mainHandWeapon, offhandWeapon);
				if (isNotSuitableReason == TauntUsageManager.TauntUsage.TauntUsageFlag.None)
				{
					return TauntUsageManager.TauntUsage.TauntUsageFlag.None;
				}
				array[i] = isNotSuitableReason;
			}
			Array.Sort<TauntUsageManager.TauntUsage.TauntUsageFlag>(array, new TauntUsageManager.TauntUsageFlagComparer());
			return array[0];
		}

		public static int GetTauntItemCount()
		{
			return TauntUsageManager._tauntUsageSets.Count;
		}

		public static int GetIndexOfAction(string id)
		{
			int num;
			if (TauntUsageManager._tauntUsageSetIndexMap.TryGetValue(id, out num))
			{
				return num;
			}
			return -1;
		}

		public static string GetDefaultAction(int index)
		{
			TauntUsageManager.TauntUsage tauntUsage = TauntUsageManager._tauntUsageSets[index].GetUsages().Last<TauntUsageManager.TauntUsage>();
			if (tauntUsage == null)
			{
				return null;
			}
			return tauntUsage.GetAction();
		}

		private static XmlDocument LoadXmlFile(string path)
		{
			string text = new StreamReader(path).ReadToEnd();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(text);
			return xmlDocument;
		}

		private static List<TauntUsageManager.TauntUsageSet> _tauntUsageSets = new List<TauntUsageManager.TauntUsageSet>();

		private static Dictionary<string, int> _tauntUsageSetIndexMap = new Dictionary<string, int>();

		private class TauntUsageFlagComparer : IComparer<TauntUsageManager.TauntUsage.TauntUsageFlag>
		{
			public int Compare(TauntUsageManager.TauntUsage.TauntUsageFlag x, TauntUsageManager.TauntUsage.TauntUsageFlag y)
			{
				int num = (int)x;
				return num.CompareTo((int)y);
			}
		}

		public class TauntUsageSet
		{
			public TauntUsageSet()
			{
				this._tauntUsages = new MBList<TauntUsageManager.TauntUsage>();
			}

			public void AddUsage(TauntUsageManager.TauntUsage usage)
			{
				this._tauntUsages.Add(usage);
			}

			public MBReadOnlyList<TauntUsageManager.TauntUsage> GetUsages()
			{
				return this._tauntUsages;
			}

			private MBList<TauntUsageManager.TauntUsage> _tauntUsages;
		}

		public class TauntUsage
		{
			public TauntUsageManager.TauntUsage.TauntUsageFlag UsageFlag { get; }

			public TauntUsage(TauntUsageManager.TauntUsage.TauntUsageFlag usageFlag, string actionName)
			{
				this.UsageFlag = usageFlag;
				this._actionName = actionName;
			}

			public bool IsSuitable(bool isLeftStance, bool isOnFoot, WeaponComponentData mainHandWeapon, WeaponComponentData offhandWeapon)
			{
				return this.GetIsNotSuitableReason(isLeftStance, isOnFoot, mainHandWeapon, offhandWeapon) == TauntUsageManager.TauntUsage.TauntUsageFlag.None;
			}

			public TauntUsageManager.TauntUsage.TauntUsageFlag GetIsNotSuitableReason(bool isLeftStance, bool isOnFoot, WeaponComponentData mainHandWeapon, WeaponComponentData offhandWeapon)
			{
				TauntUsageManager.TauntUsage.TauntUsageFlag tauntUsageFlag = TauntUsageManager.TauntUsage.TauntUsageFlag.None;
				if (this.UsageFlag.HasAllFlags(TauntUsageManager.TauntUsage.TauntUsageFlag.RequiresBow) && (mainHandWeapon == null || !mainHandWeapon.IsBow))
				{
					tauntUsageFlag |= TauntUsageManager.TauntUsage.TauntUsageFlag.RequiresBow;
				}
				if (this.UsageFlag.HasAllFlags(TauntUsageManager.TauntUsage.TauntUsageFlag.RequiresShield) && (offhandWeapon == null || !offhandWeapon.IsShield))
				{
					tauntUsageFlag |= TauntUsageManager.TauntUsage.TauntUsageFlag.RequiresShield;
				}
				if (this.UsageFlag.HasAllFlags(TauntUsageManager.TauntUsage.TauntUsageFlag.RequiresOnFoot) && !isOnFoot)
				{
					tauntUsageFlag |= TauntUsageManager.TauntUsage.TauntUsageFlag.RequiresOnFoot;
				}
				if (this.UsageFlag.HasAllFlags(TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForTwoHanded) && mainHandWeapon != null && mainHandWeapon.IsTwoHanded)
				{
					tauntUsageFlag |= TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForTwoHanded;
				}
				if (this.UsageFlag.HasAllFlags(TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForOneHanded) && mainHandWeapon != null && mainHandWeapon.IsOneHanded)
				{
					tauntUsageFlag |= TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForOneHanded;
				}
				if (this.UsageFlag.HasAllFlags(TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForShield) && offhandWeapon != null && offhandWeapon.IsShield)
				{
					tauntUsageFlag |= TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForShield;
				}
				if (this.UsageFlag.HasAllFlags(TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForBow) && mainHandWeapon != null && mainHandWeapon.IsBow)
				{
					tauntUsageFlag |= TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForBow;
				}
				if (this.UsageFlag.HasAllFlags(TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForCrossbow) && mainHandWeapon != null && mainHandWeapon.IsCrossBow)
				{
					tauntUsageFlag |= TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForCrossbow;
				}
				if (this.UsageFlag.HasAllFlags(TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForEmpty) && mainHandWeapon == null && offhandWeapon == null)
				{
					tauntUsageFlag |= TauntUsageManager.TauntUsage.TauntUsageFlag.UnsuitableForEmpty;
				}
				if (this.UsageFlag.HasAllFlags(TauntUsageManager.TauntUsage.TauntUsageFlag.IsLeftStance) != isLeftStance)
				{
					tauntUsageFlag |= TauntUsageManager.TauntUsage.TauntUsageFlag.IsLeftStance;
				}
				return tauntUsageFlag;
			}

			public string GetAction()
			{
				return this._actionName;
			}

			private string _actionName;

			[Flags]
			public enum TauntUsageFlag
			{
				None = 0,
				RequiresBow = 1,
				RequiresShield = 2,
				IsLeftStance = 4,
				RequiresOnFoot = 8,
				UnsuitableForTwoHanded = 16,
				UnsuitableForOneHanded = 32,
				UnsuitableForShield = 64,
				UnsuitableForBow = 128,
				UnsuitableForCrossbow = 256,
				UnsuitableForEmpty = 512
			}
		}
	}
}

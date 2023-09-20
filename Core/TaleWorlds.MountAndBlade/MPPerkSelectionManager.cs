using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200031B RID: 795
	public class MPPerkSelectionManager
	{
		// Token: 0x170007A9 RID: 1961
		// (get) Token: 0x06002AF6 RID: 10998 RVA: 0x000A831E File Offset: 0x000A651E
		public static MPPerkSelectionManager Instance
		{
			get
			{
				MPPerkSelectionManager mpperkSelectionManager;
				if ((mpperkSelectionManager = MPPerkSelectionManager._instance) == null)
				{
					mpperkSelectionManager = (MPPerkSelectionManager._instance = new MPPerkSelectionManager());
				}
				return mpperkSelectionManager;
			}
		}

		// Token: 0x06002AF7 RID: 10999 RVA: 0x000A8334 File Offset: 0x000A6534
		public static void FreeInstance()
		{
			if (MPPerkSelectionManager._instance != null)
			{
				Dictionary<MultiplayerClassDivisions.MPHeroClass, List<MPPerkSelectionManager.MPPerkSelection>> selections = MPPerkSelectionManager._instance._selections;
				if (selections != null)
				{
					selections.Clear();
				}
				Dictionary<MultiplayerClassDivisions.MPHeroClass, List<MPPerkSelectionManager.MPPerkSelection>> pendingChanges = MPPerkSelectionManager._instance._pendingChanges;
				if (pendingChanges != null)
				{
					pendingChanges.Clear();
				}
				MPPerkSelectionManager._instance = null;
			}
		}

		// Token: 0x06002AF8 RID: 11000 RVA: 0x000A8370 File Offset: 0x000A6570
		public void InitializeForUser(string username, PlayerId playerId)
		{
			if (this._playerIdOfSelectionsOwner != playerId)
			{
				Dictionary<MultiplayerClassDivisions.MPHeroClass, List<MPPerkSelectionManager.MPPerkSelection>> selections = this._selections;
				if (selections != null)
				{
					selections.Clear();
				}
				this._playerIdOfSelectionsOwner = playerId;
				this._xmlPath = new PlatformFilePath(EngineFilePaths.ConfigsPath, "MPDefaultPerks_" + playerId + ".xml");
				try
				{
					PlatformFilePath platformFilePath = new PlatformFilePath(EngineFilePaths.ConfigsPath, "MPDefaultPerks_" + username + ".xml");
					if (FileHelper.FileExists(platformFilePath))
					{
						FileHelper.CopyFile(platformFilePath, this._xmlPath);
						FileHelper.DeleteFile(platformFilePath);
					}
				}
				catch (Exception)
				{
				}
				Dictionary<MultiplayerClassDivisions.MPHeroClass, List<MPPerkSelectionManager.MPPerkSelection>> dictionary = this.LoadSelectionsForUserFromXML();
				this._selections = dictionary ?? new Dictionary<MultiplayerClassDivisions.MPHeroClass, List<MPPerkSelectionManager.MPPerkSelection>>();
			}
		}

		// Token: 0x06002AF9 RID: 11001 RVA: 0x000A8430 File Offset: 0x000A6630
		public void ResetPendingChanges()
		{
			Dictionary<MultiplayerClassDivisions.MPHeroClass, List<MPPerkSelectionManager.MPPerkSelection>> pendingChanges = this._pendingChanges;
			if (pendingChanges != null)
			{
				pendingChanges.Clear();
			}
			Action onAfterResetPendingChanges = this.OnAfterResetPendingChanges;
			if (onAfterResetPendingChanges == null)
			{
				return;
			}
			onAfterResetPendingChanges();
		}

		// Token: 0x06002AFA RID: 11002 RVA: 0x000A8454 File Offset: 0x000A6654
		public void TryToApplyAndSavePendingChanges()
		{
			if (this._pendingChanges != null)
			{
				foreach (KeyValuePair<MultiplayerClassDivisions.MPHeroClass, List<MPPerkSelectionManager.MPPerkSelection>> keyValuePair in this._pendingChanges)
				{
					if (this._selections.ContainsKey(keyValuePair.Key))
					{
						this._selections.Remove(keyValuePair.Key);
					}
					this._selections.Add(keyValuePair.Key, keyValuePair.Value);
				}
				this._pendingChanges.Clear();
				List<KeyValuePair<MultiplayerClassDivisions.MPHeroClass, List<MPPerkSelectionManager.MPPerkSelection>>> selections = new List<KeyValuePair<MultiplayerClassDivisions.MPHeroClass, List<MPPerkSelectionManager.MPPerkSelection>>>();
				foreach (KeyValuePair<MultiplayerClassDivisions.MPHeroClass, List<MPPerkSelectionManager.MPPerkSelection>> keyValuePair2 in this._selections)
				{
					selections.Add(new KeyValuePair<MultiplayerClassDivisions.MPHeroClass, List<MPPerkSelectionManager.MPPerkSelection>>(keyValuePair2.Key, keyValuePair2.Value));
				}
				((ITask)AsyncTask.CreateWithDelegate(new ManagedDelegate
				{
					Instance = delegate
					{
						MPPerkSelectionManager instance = MPPerkSelectionManager.Instance;
						lock (instance)
						{
							this.SaveAsXML(selections);
						}
					}
				}, true)).Invoke();
			}
		}

		// Token: 0x06002AFB RID: 11003 RVA: 0x000A858C File Offset: 0x000A678C
		public List<MPPerkSelectionManager.MPPerkSelection> GetSelectionsForHeroClass(MultiplayerClassDivisions.MPHeroClass currentHeroClass)
		{
			List<MPPerkSelectionManager.MPPerkSelection> list = new List<MPPerkSelectionManager.MPPerkSelection>();
			if ((this._pendingChanges == null || !this._pendingChanges.TryGetValue(currentHeroClass, out list)) && this._selections != null)
			{
				this._selections.TryGetValue(currentHeroClass, out list);
			}
			return list;
		}

		// Token: 0x06002AFC RID: 11004 RVA: 0x000A85D4 File Offset: 0x000A67D4
		public void SetSelectionsForHeroClassTemporarily(MultiplayerClassDivisions.MPHeroClass currentHeroClass, List<MPPerkSelectionManager.MPPerkSelection> perkChoices)
		{
			if (this._pendingChanges == null)
			{
				this._pendingChanges = new Dictionary<MultiplayerClassDivisions.MPHeroClass, List<MPPerkSelectionManager.MPPerkSelection>>();
			}
			List<MPPerkSelectionManager.MPPerkSelection> list;
			if (!this._pendingChanges.TryGetValue(currentHeroClass, out list))
			{
				list = new List<MPPerkSelectionManager.MPPerkSelection>();
				this._pendingChanges.Add(currentHeroClass, list);
			}
			else
			{
				list.Clear();
			}
			int count = perkChoices.Count;
			for (int i = 0; i < count; i++)
			{
				list.Add(perkChoices[i]);
			}
		}

		// Token: 0x06002AFD RID: 11005 RVA: 0x000A8640 File Offset: 0x000A6840
		private Dictionary<MultiplayerClassDivisions.MPHeroClass, List<MPPerkSelectionManager.MPPerkSelection>> LoadSelectionsForUserFromXML()
		{
			Dictionary<MultiplayerClassDivisions.MPHeroClass, List<MPPerkSelectionManager.MPPerkSelection>> dictionary = null;
			MPPerkSelectionManager instance = MPPerkSelectionManager.Instance;
			lock (instance)
			{
				bool flag2 = FileHelper.FileExists(this._xmlPath);
				if (flag2)
				{
					dictionary = new Dictionary<MultiplayerClassDivisions.MPHeroClass, List<MPPerkSelectionManager.MPPerkSelection>>();
					try
					{
						MBReadOnlyList<MultiplayerClassDivisions.MPHeroClass> mpheroClasses = MultiplayerClassDivisions.GetMPHeroClasses();
						int count = mpheroClasses.Count;
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.Load(this._xmlPath);
						foreach (object obj in xmlDocument.DocumentElement.ChildNodes)
						{
							XmlNode xmlNode = (XmlNode)obj;
							XmlNode xmlNode2 = xmlNode.Attributes["id"];
							MultiplayerClassDivisions.MPHeroClass mpheroClass = null;
							string value = xmlNode2.Value;
							for (int i = 0; i < count; i++)
							{
								if (mpheroClasses[i].StringId == value)
								{
									mpheroClass = mpheroClasses[i];
									break;
								}
							}
							if (mpheroClass != null)
							{
								List<MPPerkSelectionManager.MPPerkSelection> list = new List<MPPerkSelectionManager.MPPerkSelection>(2);
								foreach (object obj2 in xmlNode.ChildNodes)
								{
									XmlNode xmlNode3 = (XmlNode)obj2;
									XmlAttribute xmlAttribute = xmlNode3.Attributes["index"];
									XmlAttribute xmlAttribute2 = xmlNode3.Attributes["listIndex"];
									if (xmlAttribute != null && xmlAttribute2 != null)
									{
										int num = Convert.ToInt32(xmlAttribute.Value);
										int num2 = Convert.ToInt32(xmlAttribute2.Value);
										list.Add(new MPPerkSelectionManager.MPPerkSelection(num, num2));
									}
									else
									{
										flag2 = false;
									}
								}
								dictionary.Add(mpheroClass, list);
							}
							else
							{
								flag2 = false;
							}
						}
					}
					catch
					{
						flag2 = false;
					}
				}
				if (!flag2)
				{
					dictionary = null;
				}
			}
			return dictionary;
		}

		// Token: 0x06002AFE RID: 11006 RVA: 0x000A8860 File Offset: 0x000A6A60
		private bool SaveAsXML(List<KeyValuePair<MultiplayerClassDivisions.MPHeroClass, List<MPPerkSelectionManager.MPPerkSelection>>> selections)
		{
			bool flag = true;
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.InsertBefore(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null), xmlDocument.DocumentElement);
				XmlElement xmlElement = xmlDocument.CreateElement("HeroClasses");
				xmlDocument.AppendChild(xmlElement);
				foreach (KeyValuePair<MultiplayerClassDivisions.MPHeroClass, List<MPPerkSelectionManager.MPPerkSelection>> keyValuePair in selections)
				{
					MultiplayerClassDivisions.MPHeroClass key = keyValuePair.Key;
					List<MPPerkSelectionManager.MPPerkSelection> value = keyValuePair.Value;
					XmlElement xmlElement2 = xmlDocument.CreateElement("HeroClass");
					xmlElement2.SetAttribute("id", key.StringId);
					xmlElement.AppendChild(xmlElement2);
					foreach (MPPerkSelectionManager.MPPerkSelection mpperkSelection in value)
					{
						XmlElement xmlElement3 = xmlDocument.CreateElement("PerkSelection");
						xmlElement3.SetAttribute("index", mpperkSelection.Index.ToString());
						xmlElement3.SetAttribute("listIndex", mpperkSelection.ListIndex.ToString());
						xmlElement2.AppendChild(xmlElement3);
					}
				}
				xmlDocument.Save(this._xmlPath);
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x04001057 RID: 4183
		private static MPPerkSelectionManager _instance;

		// Token: 0x04001058 RID: 4184
		public Action OnAfterResetPendingChanges;

		// Token: 0x04001059 RID: 4185
		private Dictionary<MultiplayerClassDivisions.MPHeroClass, List<MPPerkSelectionManager.MPPerkSelection>> _selections;

		// Token: 0x0400105A RID: 4186
		private Dictionary<MultiplayerClassDivisions.MPHeroClass, List<MPPerkSelectionManager.MPPerkSelection>> _pendingChanges;

		// Token: 0x0400105B RID: 4187
		private PlatformFilePath _xmlPath;

		// Token: 0x0400105C RID: 4188
		private PlayerId _playerIdOfSelectionsOwner;

		// Token: 0x0200063A RID: 1594
		public struct MPPerkSelection
		{
			// Token: 0x06003E12 RID: 15890 RVA: 0x000F4970 File Offset: 0x000F2B70
			public MPPerkSelection(int index, int listIndex)
			{
				this.Index = index;
				this.ListIndex = listIndex;
			}

			// Token: 0x0400202E RID: 8238
			public readonly int Index;

			// Token: 0x0400202F RID: 8239
			public readonly int ListIndex;
		}
	}
}

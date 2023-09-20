using System;
using System.Reflection;
using System.Xml;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameMenus
{
	// Token: 0x020000E9 RID: 233
	public class WaitMenuOption
	{
		// Token: 0x170005A5 RID: 1445
		// (get) Token: 0x06001413 RID: 5139 RVA: 0x000587C5 File Offset: 0x000569C5
		// (set) Token: 0x06001414 RID: 5140 RVA: 0x000587CD File Offset: 0x000569CD
		public int Priority { get; private set; }

		// Token: 0x06001415 RID: 5141 RVA: 0x000587D6 File Offset: 0x000569D6
		internal WaitMenuOption()
		{
			this.Priority = 100;
			this._text = TextObject.Empty;
			this._tooltip = "";
		}

		// Token: 0x06001416 RID: 5142 RVA: 0x000587FC File Offset: 0x000569FC
		internal WaitMenuOption(string idString, TextObject text, WaitMenuOption.OnConditionDelegate condition, WaitMenuOption.OnConsequenceDelegate consequence, int priority = 100, string tooltip = "")
		{
			this._idstring = idString;
			this._text = text;
			this.OnCondition = condition;
			this.OnConsequence = consequence;
			this.Priority = priority;
			this._tooltip = tooltip;
		}

		// Token: 0x06001417 RID: 5143 RVA: 0x00058834 File Offset: 0x00056A34
		public bool GetConditionsHold(Game game, MapState mapState)
		{
			if (this.OnCondition != null)
			{
				MenuCallbackArgs menuCallbackArgs = new MenuCallbackArgs(mapState, this.Text);
				return this.OnCondition(menuCallbackArgs);
			}
			return true;
		}

		// Token: 0x170005A6 RID: 1446
		// (get) Token: 0x06001418 RID: 5144 RVA: 0x00058864 File Offset: 0x00056A64
		public TextObject Text
		{
			get
			{
				return this._text;
			}
		}

		// Token: 0x170005A7 RID: 1447
		// (get) Token: 0x06001419 RID: 5145 RVA: 0x0005886C File Offset: 0x00056A6C
		public string IdString
		{
			get
			{
				return this._idstring;
			}
		}

		// Token: 0x170005A8 RID: 1448
		// (get) Token: 0x0600141A RID: 5146 RVA: 0x00058874 File Offset: 0x00056A74
		public string Tooltip
		{
			get
			{
				return this._tooltip;
			}
		}

		// Token: 0x170005A9 RID: 1449
		// (get) Token: 0x0600141B RID: 5147 RVA: 0x0005887C File Offset: 0x00056A7C
		public bool IsLeave
		{
			get
			{
				return this._isLeave;
			}
		}

		// Token: 0x0600141C RID: 5148 RVA: 0x00058884 File Offset: 0x00056A84
		public void RunConsequence(Game game, MapState mapState)
		{
			if (this.OnConsequence != null)
			{
				MenuCallbackArgs menuCallbackArgs = new MenuCallbackArgs(mapState, this.Text);
				this.OnConsequence(menuCallbackArgs);
			}
		}

		// Token: 0x0600141D RID: 5149 RVA: 0x000588B4 File Offset: 0x00056AB4
		public void Deserialize(XmlNode node, Type typeOfWaitMenusCallbacks)
		{
			if (node.Attributes == null)
			{
				throw new TWXmlLoadException("node.Attributes != null");
			}
			this._idstring = node.Attributes["id"].Value;
			XmlNode xmlNode = node.Attributes["text"];
			if (xmlNode != null)
			{
				this._text = new TextObject(xmlNode.InnerText, null);
			}
			if (node.Attributes["is_leave"] != null)
			{
				this._isLeave = true;
			}
			XmlNode xmlNode2 = node.Attributes["on_condition"];
			if (xmlNode2 != null)
			{
				string innerText = xmlNode2.InnerText;
				this._methodOnCondition = typeOfWaitMenusCallbacks.GetMethod(innerText);
				if (this._methodOnCondition == null)
				{
					throw new MBNotFoundException("Can not find WaitMenuOption condition:" + innerText);
				}
				this.OnCondition = (WaitMenuOption.OnConditionDelegate)Delegate.CreateDelegate(typeof(WaitMenuOption.OnConditionDelegate), null, this._methodOnCondition);
			}
			XmlNode xmlNode3 = node.Attributes["on_consequence"];
			if (xmlNode3 != null)
			{
				string innerText2 = xmlNode3.InnerText;
				this._methodOnConsequence = typeOfWaitMenusCallbacks.GetMethod(innerText2);
				if (this._methodOnConsequence == null)
				{
					throw new MBNotFoundException("Can not find WaitMenuOption consequence:" + innerText2);
				}
				this.OnConsequence = (WaitMenuOption.OnConsequenceDelegate)Delegate.CreateDelegate(typeof(WaitMenuOption.OnConsequenceDelegate), null, this._methodOnConsequence);
			}
		}

		// Token: 0x0400070D RID: 1805
		private string _idstring;

		// Token: 0x0400070E RID: 1806
		private TextObject _text;

		// Token: 0x0400070F RID: 1807
		private string _tooltip;

		// Token: 0x04000710 RID: 1808
		private MethodInfo _methodOnCondition;

		// Token: 0x04000711 RID: 1809
		public WaitMenuOption.OnConditionDelegate OnCondition;

		// Token: 0x04000712 RID: 1810
		private MethodInfo _methodOnConsequence;

		// Token: 0x04000713 RID: 1811
		public WaitMenuOption.OnConsequenceDelegate OnConsequence;

		// Token: 0x04000714 RID: 1812
		private bool _isLeave;

		// Token: 0x020004F6 RID: 1270
		// (Invoke) Token: 0x060041EC RID: 16876
		public delegate bool OnConditionDelegate(MenuCallbackArgs args);

		// Token: 0x020004F7 RID: 1271
		// (Invoke) Token: 0x060041F0 RID: 16880
		public delegate void OnConsequenceDelegate(MenuCallbackArgs args);
	}
}

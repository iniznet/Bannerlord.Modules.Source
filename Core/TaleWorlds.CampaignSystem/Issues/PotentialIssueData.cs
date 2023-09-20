using System;

namespace TaleWorlds.CampaignSystem.Issues
{
	// Token: 0x02000323 RID: 803
	public struct PotentialIssueData
	{
		// Token: 0x17000AE3 RID: 2787
		// (get) Token: 0x06002DAA RID: 11690 RVA: 0x000BF0C4 File Offset: 0x000BD2C4
		public PotentialIssueData.StartIssueDelegate OnStartIssue { get; }

		// Token: 0x17000AE4 RID: 2788
		// (get) Token: 0x06002DAB RID: 11691 RVA: 0x000BF0CC File Offset: 0x000BD2CC
		public string IssueId { get; }

		// Token: 0x17000AE5 RID: 2789
		// (get) Token: 0x06002DAC RID: 11692 RVA: 0x000BF0D4 File Offset: 0x000BD2D4
		public Type IssueType { get; }

		// Token: 0x17000AE6 RID: 2790
		// (get) Token: 0x06002DAD RID: 11693 RVA: 0x000BF0DC File Offset: 0x000BD2DC
		public IssueBase.IssueFrequency Frequency { get; }

		// Token: 0x17000AE7 RID: 2791
		// (get) Token: 0x06002DAE RID: 11694 RVA: 0x000BF0E4 File Offset: 0x000BD2E4
		public object RelatedObject { get; }

		// Token: 0x17000AE8 RID: 2792
		// (get) Token: 0x06002DAF RID: 11695 RVA: 0x000BF0EC File Offset: 0x000BD2EC
		public bool IsValid
		{
			get
			{
				return this.OnStartIssue != null;
			}
		}

		// Token: 0x06002DB0 RID: 11696 RVA: 0x000BF0F7 File Offset: 0x000BD2F7
		public PotentialIssueData(PotentialIssueData.StartIssueDelegate onStartIssue, Type issueType, IssueBase.IssueFrequency frequency, object relatedObject = null)
		{
			this.OnStartIssue = onStartIssue;
			this.IssueId = issueType.Name;
			this.IssueType = issueType;
			this.Frequency = frequency;
			this.RelatedObject = relatedObject;
		}

		// Token: 0x06002DB1 RID: 11697 RVA: 0x000BF122 File Offset: 0x000BD322
		public PotentialIssueData(Type issueType, IssueBase.IssueFrequency frequency)
		{
			this.OnStartIssue = null;
			this.IssueId = issueType.Name;
			this.IssueType = issueType;
			this.Frequency = frequency;
			this.RelatedObject = null;
		}

		// Token: 0x02000676 RID: 1654
		// (Invoke) Token: 0x060051E0 RID: 20960
		public delegate IssueBase StartIssueDelegate(in PotentialIssueData pid, Hero issueOwner);
	}
}

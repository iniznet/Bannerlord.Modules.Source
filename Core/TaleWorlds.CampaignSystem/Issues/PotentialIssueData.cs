using System;

namespace TaleWorlds.CampaignSystem.Issues
{
	public struct PotentialIssueData
	{
		public PotentialIssueData.StartIssueDelegate OnStartIssue { get; }

		public string IssueId { get; }

		public Type IssueType { get; }

		public IssueBase.IssueFrequency Frequency { get; }

		public object RelatedObject { get; }

		public bool IsValid
		{
			get
			{
				return this.OnStartIssue != null;
			}
		}

		public PotentialIssueData(PotentialIssueData.StartIssueDelegate onStartIssue, Type issueType, IssueBase.IssueFrequency frequency, object relatedObject = null)
		{
			this.OnStartIssue = onStartIssue;
			this.IssueId = issueType.Name;
			this.IssueType = issueType;
			this.Frequency = frequency;
			this.RelatedObject = relatedObject;
		}

		public PotentialIssueData(Type issueType, IssueBase.IssueFrequency frequency)
		{
			this.OnStartIssue = null;
			this.IssueId = issueType.Name;
			this.IssueType = issueType;
			this.Frequency = frequency;
			this.RelatedObject = null;
		}

		public delegate IssueBase StartIssueDelegate(in PotentialIssueData pid, Hero issueOwner);
	}
}

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x0200007D RID: 125
	public struct ExplainedNumber
	{
		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x06000F56 RID: 3926 RVA: 0x00047E4E File Offset: 0x0004604E
		public float ResultNumber
		{
			get
			{
				return MathF.Clamp(this.BaseNumber + this.BaseNumber * this._sumOfFactors, this.LimitMinValue, this.LimitMaxValue);
			}
		}

		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x06000F57 RID: 3927 RVA: 0x00047E75 File Offset: 0x00046075
		// (set) Token: 0x06000F58 RID: 3928 RVA: 0x00047E7D File Offset: 0x0004607D
		public float BaseNumber { get; private set; }

		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x06000F59 RID: 3929 RVA: 0x00047E86 File Offset: 0x00046086
		public bool IncludeDescriptions
		{
			get
			{
				return this._explainer != null;
			}
		}

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x06000F5A RID: 3930 RVA: 0x00047E91 File Offset: 0x00046091
		public float LimitMinValue
		{
			get
			{
				if (this._limitMinValue == null)
				{
					return float.MinValue;
				}
				return this._limitMinValue.Value;
			}
		}

		// Token: 0x1700040B RID: 1035
		// (get) Token: 0x06000F5B RID: 3931 RVA: 0x00047EB2 File Offset: 0x000460B2
		public float LimitMaxValue
		{
			get
			{
				if (this._limitMaxValue == null)
				{
					return float.MaxValue;
				}
				return this._limitMaxValue.Value;
			}
		}

		// Token: 0x06000F5C RID: 3932 RVA: 0x00047ED4 File Offset: 0x000460D4
		public ExplainedNumber(float baseNumber = 0f, bool includeDescriptions = false, TextObject baseText = null)
		{
			this.BaseNumber = baseNumber;
			this._explainer = (includeDescriptions ? new ExplainedNumber.StatExplainer() : null);
			this._sumOfFactors = 0f;
			this._limitMinValue = new float?(float.MinValue);
			this._limitMaxValue = new float?(float.MaxValue);
			if (this._explainer != null && !this.BaseNumber.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				this._explainer.AddLine((baseText ?? ExplainedNumber.BaseText).ToString(), this.BaseNumber, ExplainedNumber.StatExplainer.OperationType.Base);
			}
		}

		// Token: 0x06000F5D RID: 3933 RVA: 0x00047F64 File Offset: 0x00046164
		public string GetExplanations()
		{
			if (this._explainer == null)
			{
				return "";
			}
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "GetExplanations");
			foreach (ValueTuple<string, float> valueTuple in this._explainer.GetLines(this.BaseNumber, this.ResultNumber))
			{
				string text = string.Format("{0} : {1}{2:0.##}\n", valueTuple.Item1, (valueTuple.Item2 > 0.001f) ? "+" : "", valueTuple.Item2);
				mbstringBuilder.Append<string>(text);
			}
			return mbstringBuilder.ToStringAndRelease();
		}

		// Token: 0x06000F5E RID: 3934 RVA: 0x0004802C File Offset: 0x0004622C
		[return: TupleElementNames(new string[] { "name", "number" })]
		public List<ValueTuple<string, float>> GetLines()
		{
			if (this._explainer == null)
			{
				return new List<ValueTuple<string, float>>();
			}
			return this._explainer.GetLines(this.BaseNumber, this.ResultNumber);
		}

		// Token: 0x06000F5F RID: 3935 RVA: 0x00048054 File Offset: 0x00046254
		public void AddFromExplainedNumber(ExplainedNumber explainedNumber, TextObject baseText)
		{
			if (explainedNumber._explainer != null && this._explainer != null)
			{
				if (explainedNumber._explainer.BaseLine != null && explainedNumber._explainer.BaseLine != null && !explainedNumber.BaseNumber.ApproximatelyEqualsTo(0f, 1E-05f))
				{
					float num = explainedNumber._explainer.BaseLine.Value.Number + explainedNumber._explainer.BaseLine.Value.Number * explainedNumber._sumOfFactors;
					this._explainer.AddLine(((baseText != null) ? baseText.ToString() : null) ?? ExplainedNumber.BaseText.ToString(), num, ExplainedNumber.StatExplainer.OperationType.Add);
				}
				foreach (ExplainedNumber.StatExplainer.ExplanationLine explanationLine in explainedNumber._explainer.Lines)
				{
					if (explanationLine.OperationType == ExplainedNumber.StatExplainer.OperationType.Add)
					{
						float num2 = explanationLine.Number + explanationLine.Number * explainedNumber._sumOfFactors;
						this._explainer.AddLine(explanationLine.Name, num2, explanationLine.OperationType);
					}
				}
			}
			this.BaseNumber += explainedNumber.ResultNumber;
		}

		// Token: 0x06000F60 RID: 3936 RVA: 0x000481AC File Offset: 0x000463AC
		public void Add(float value, TextObject description = null, TextObject variable = null)
		{
			if (value.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				return;
			}
			this.BaseNumber += value;
			if (description != null && this._explainer != null && !value.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				if (variable != null)
				{
					description.SetTextVariable("A0", variable);
				}
				this._explainer.AddLine(description.ToString(), value, ExplainedNumber.StatExplainer.OperationType.Add);
			}
		}

		// Token: 0x06000F61 RID: 3937 RVA: 0x0004821C File Offset: 0x0004641C
		public void AddFactor(float value, TextObject description = null)
		{
			if (value.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				return;
			}
			this._sumOfFactors += value;
			if (description != null && this._explainer != null && !value.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				this._explainer.AddLine(description.ToString(), MathF.Round(value, 3) * 100f, ExplainedNumber.StatExplainer.OperationType.Multiply);
			}
		}

		// Token: 0x06000F62 RID: 3938 RVA: 0x00048286 File Offset: 0x00046486
		public void LimitMin(float minValue)
		{
			this._limitMinValue = new float?(minValue);
			if (this._explainer != null)
			{
				this._explainer.AddLine(ExplainedNumber.LimitMinText.ToString(), minValue, ExplainedNumber.StatExplainer.OperationType.LimitMin);
			}
		}

		// Token: 0x06000F63 RID: 3939 RVA: 0x000482B3 File Offset: 0x000464B3
		public void LimitMax(float maxValue)
		{
			this._limitMaxValue = new float?(maxValue);
			if (this._explainer != null)
			{
				this._explainer.AddLine(ExplainedNumber.LimitMaxText.ToString(), maxValue, ExplainedNumber.StatExplainer.OperationType.LimitMax);
			}
		}

		// Token: 0x06000F64 RID: 3940 RVA: 0x000482E0 File Offset: 0x000464E0
		public void Clamp(float minValue, float maxValue)
		{
			this.LimitMin(minValue);
			this.LimitMax(maxValue);
		}

		// Token: 0x04000538 RID: 1336
		private static readonly TextObject LimitMinText = new TextObject("{=GNalaRaN}Minimum", null);

		// Token: 0x04000539 RID: 1337
		private static readonly TextObject LimitMaxText = new TextObject("{=cfjTtxWv}Maximum", null);

		// Token: 0x0400053A RID: 1338
		private static readonly TextObject BaseText = new TextObject("{=basevalue}Base", null);

		// Token: 0x0400053C RID: 1340
		private float? _limitMinValue;

		// Token: 0x0400053D RID: 1341
		private float? _limitMaxValue;

		// Token: 0x0400053E RID: 1342
		private ExplainedNumber.StatExplainer _explainer;

		// Token: 0x0400053F RID: 1343
		private float _sumOfFactors;

		// Token: 0x020004C5 RID: 1221
		private class StatExplainer
		{
			// Token: 0x17000D84 RID: 3460
			// (get) Token: 0x0600414C RID: 16716 RVA: 0x001333AD File Offset: 0x001315AD
			// (set) Token: 0x0600414D RID: 16717 RVA: 0x001333B5 File Offset: 0x001315B5
			public List<ExplainedNumber.StatExplainer.ExplanationLine> Lines { get; private set; } = new List<ExplainedNumber.StatExplainer.ExplanationLine>();

			// Token: 0x17000D85 RID: 3461
			// (get) Token: 0x0600414E RID: 16718 RVA: 0x001333BE File Offset: 0x001315BE
			// (set) Token: 0x0600414F RID: 16719 RVA: 0x001333C6 File Offset: 0x001315C6
			public ExplainedNumber.StatExplainer.ExplanationLine? BaseLine { get; private set; }

			// Token: 0x17000D86 RID: 3462
			// (get) Token: 0x06004150 RID: 16720 RVA: 0x001333CF File Offset: 0x001315CF
			// (set) Token: 0x06004151 RID: 16721 RVA: 0x001333D7 File Offset: 0x001315D7
			public ExplainedNumber.StatExplainer.ExplanationLine? LimitMinLine { get; private set; }

			// Token: 0x17000D87 RID: 3463
			// (get) Token: 0x06004152 RID: 16722 RVA: 0x001333E0 File Offset: 0x001315E0
			// (set) Token: 0x06004153 RID: 16723 RVA: 0x001333E8 File Offset: 0x001315E8
			public ExplainedNumber.StatExplainer.ExplanationLine? LimitMaxLine { get; private set; }

			// Token: 0x06004154 RID: 16724 RVA: 0x001333F4 File Offset: 0x001315F4
			[return: TupleElementNames(new string[] { "name", "number" })]
			public List<ValueTuple<string, float>> GetLines(float baseNumber, float resultNumber)
			{
				List<ValueTuple<string, float>> list = new List<ValueTuple<string, float>>();
				if (this.BaseLine != null)
				{
					list.Add(new ValueTuple<string, float>(this.BaseLine.Value.Name, this.BaseLine.Value.Number));
				}
				foreach (ExplainedNumber.StatExplainer.ExplanationLine explanationLine in this.Lines)
				{
					float num = explanationLine.Number;
					if (explanationLine.OperationType == ExplainedNumber.StatExplainer.OperationType.Multiply)
					{
						num = baseNumber * num * 0.01f;
					}
					list.Add(new ValueTuple<string, float>(explanationLine.Name, num));
				}
				if (this.LimitMinLine != null && this.LimitMinLine.Value.Number > resultNumber)
				{
					list.Add(new ValueTuple<string, float>(this.LimitMinLine.Value.Name, this.LimitMinLine.Value.Number));
				}
				if (this.LimitMaxLine != null && this.LimitMaxLine.Value.Number < resultNumber)
				{
					list.Add(new ValueTuple<string, float>(this.LimitMaxLine.Value.Name, this.LimitMaxLine.Value.Number));
				}
				return list;
			}

			// Token: 0x06004155 RID: 16725 RVA: 0x00133568 File Offset: 0x00131768
			public void AddLine(string name, float number, ExplainedNumber.StatExplainer.OperationType opType)
			{
				ExplainedNumber.StatExplainer.ExplanationLine explanationLine = new ExplainedNumber.StatExplainer.ExplanationLine(name, number, opType);
				if (opType == ExplainedNumber.StatExplainer.OperationType.Add || opType == ExplainedNumber.StatExplainer.OperationType.Multiply)
				{
					int num = -1;
					for (int i = 0; i < this.Lines.Count; i++)
					{
						if (this.Lines[i].Name.Equals(name) && this.Lines[i].OperationType == opType)
						{
							num = i;
							break;
						}
					}
					if (num < 0)
					{
						this.Lines.Add(explanationLine);
						return;
					}
					explanationLine = new ExplainedNumber.StatExplainer.ExplanationLine(name, number + this.Lines[num].Number, opType);
					this.Lines[num] = explanationLine;
					return;
				}
				else
				{
					if (opType == ExplainedNumber.StatExplainer.OperationType.Base)
					{
						this.BaseLine = new ExplainedNumber.StatExplainer.ExplanationLine?(explanationLine);
						return;
					}
					if (opType == ExplainedNumber.StatExplainer.OperationType.LimitMin)
					{
						this.LimitMinLine = new ExplainedNumber.StatExplainer.ExplanationLine?(explanationLine);
						return;
					}
					if (opType == ExplainedNumber.StatExplainer.OperationType.LimitMax)
					{
						this.LimitMaxLine = new ExplainedNumber.StatExplainer.ExplanationLine?(explanationLine);
					}
					return;
				}
			}

			// Token: 0x02000776 RID: 1910
			public enum OperationType
			{
				// Token: 0x04001E97 RID: 7831
				Base,
				// Token: 0x04001E98 RID: 7832
				Add,
				// Token: 0x04001E99 RID: 7833
				Multiply,
				// Token: 0x04001E9A RID: 7834
				LimitMin,
				// Token: 0x04001E9B RID: 7835
				LimitMax
			}

			// Token: 0x02000777 RID: 1911
			public readonly struct ExplanationLine
			{
				// Token: 0x060056BD RID: 22205 RVA: 0x0016EB03 File Offset: 0x0016CD03
				public ExplanationLine(string name, float number, ExplainedNumber.StatExplainer.OperationType operationType)
				{
					this.Name = name;
					this.Number = number;
					this.OperationType = operationType;
				}

				// Token: 0x04001E9C RID: 7836
				public readonly float Number;

				// Token: 0x04001E9D RID: 7837
				public readonly string Name;

				// Token: 0x04001E9E RID: 7838
				public readonly ExplainedNumber.StatExplainer.OperationType OperationType;
			}
		}
	}
}

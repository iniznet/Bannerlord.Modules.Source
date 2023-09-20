using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network
{
	// Token: 0x020003B3 RID: 947
	public static class DebugNetworkEventStatistics
	{
		// Token: 0x14000098 RID: 152
		// (add) Token: 0x06003305 RID: 13061 RVA: 0x000D341C File Offset: 0x000D161C
		// (remove) Token: 0x06003306 RID: 13062 RVA: 0x000D3450 File Offset: 0x000D1650
		public static event Action<IEnumerable<DebugNetworkEventStatistics.TotalEventData>> OnEventDataUpdated;

		// Token: 0x14000099 RID: 153
		// (add) Token: 0x06003307 RID: 13063 RVA: 0x000D3484 File Offset: 0x000D1684
		// (remove) Token: 0x06003308 RID: 13064 RVA: 0x000D34B8 File Offset: 0x000D16B8
		public static event Action<DebugNetworkEventStatistics.PerSecondEventData> OnPerSecondEventDataUpdated;

		// Token: 0x1400009A RID: 154
		// (add) Token: 0x06003309 RID: 13065 RVA: 0x000D34EC File Offset: 0x000D16EC
		// (remove) Token: 0x0600330A RID: 13066 RVA: 0x000D3520 File Offset: 0x000D1720
		public static event Action<IEnumerable<float>> OnFPSEventUpdated;

		// Token: 0x1400009B RID: 155
		// (add) Token: 0x0600330B RID: 13067 RVA: 0x000D3554 File Offset: 0x000D1754
		// (remove) Token: 0x0600330C RID: 13068 RVA: 0x000D3588 File Offset: 0x000D1788
		public static event Action OnOpenExternalMonitor;

		// Token: 0x1700090F RID: 2319
		// (get) Token: 0x0600330D RID: 13069 RVA: 0x000D35BB File Offset: 0x000D17BB
		// (set) Token: 0x0600330E RID: 13070 RVA: 0x000D35C2 File Offset: 0x000D17C2
		public static int SamplesPerSecond
		{
			get
			{
				return DebugNetworkEventStatistics._samplesPerSecond;
			}
			set
			{
				DebugNetworkEventStatistics._samplesPerSecond = value;
				DebugNetworkEventStatistics.MaxGraphPointCount = value * 5;
			}
		} = 10;

		// Token: 0x17000910 RID: 2320
		// (get) Token: 0x0600330F RID: 13071 RVA: 0x000D35D2 File Offset: 0x000D17D2
		// (set) Token: 0x06003310 RID: 13072 RVA: 0x000D35D9 File Offset: 0x000D17D9
		public static bool IsActive { get; private set; }

		// Token: 0x06003312 RID: 13074 RVA: 0x000D36AC File Offset: 0x000D18AC
		internal static void StartEvent(string eventName, int eventType)
		{
			if (!DebugNetworkEventStatistics.IsActive)
			{
				return;
			}
			DebugNetworkEventStatistics._curEventType = eventType;
			if (!DebugNetworkEventStatistics._statistics.ContainsKey(DebugNetworkEventStatistics._curEventType))
			{
				DebugNetworkEventStatistics._statistics.Add(DebugNetworkEventStatistics._curEventType, new DebugNetworkEventStatistics.PerEventData
				{
					Name = eventName
				});
			}
			DebugNetworkEventStatistics._statistics[DebugNetworkEventStatistics._curEventType].Count++;
			DebugNetworkEventStatistics._totalData.TotalCount++;
		}

		// Token: 0x06003313 RID: 13075 RVA: 0x000D3724 File Offset: 0x000D1924
		internal static void EndEvent()
		{
			if (!DebugNetworkEventStatistics.IsActive)
			{
				return;
			}
			DebugNetworkEventStatistics.PerEventData perEventData = DebugNetworkEventStatistics._statistics[DebugNetworkEventStatistics._curEventType];
			perEventData.DataSize = perEventData.TotalDataSize / perEventData.Count;
			DebugNetworkEventStatistics._curEventType = -1;
		}

		// Token: 0x06003314 RID: 13076 RVA: 0x000D3762 File Offset: 0x000D1962
		internal static void AddDataToStatistic(int bitCount)
		{
			if (!DebugNetworkEventStatistics.IsActive)
			{
				return;
			}
			DebugNetworkEventStatistics._statistics[DebugNetworkEventStatistics._curEventType].TotalDataSize += bitCount;
			DebugNetworkEventStatistics._totalData.TotalDataSize += bitCount;
		}

		// Token: 0x06003315 RID: 13077 RVA: 0x000D379A File Offset: 0x000D199A
		public static void OpenExternalMonitor()
		{
			if (DebugNetworkEventStatistics.OnOpenExternalMonitor != null)
			{
				DebugNetworkEventStatistics.OnOpenExternalMonitor();
			}
		}

		// Token: 0x06003316 RID: 13078 RVA: 0x000D37AD File Offset: 0x000D19AD
		public static void ControlActivate()
		{
			DebugNetworkEventStatistics.IsActive = true;
		}

		// Token: 0x06003317 RID: 13079 RVA: 0x000D37B5 File Offset: 0x000D19B5
		public static void ControlDeactivate()
		{
			DebugNetworkEventStatistics.IsActive = false;
		}

		// Token: 0x06003318 RID: 13080 RVA: 0x000D37BD File Offset: 0x000D19BD
		public static void ControlJustDump()
		{
			DebugNetworkEventStatistics.DumpData();
		}

		// Token: 0x06003319 RID: 13081 RVA: 0x000D37C4 File Offset: 0x000D19C4
		public static void ControlDumpAll()
		{
			DebugNetworkEventStatistics.DumpData();
			DebugNetworkEventStatistics.DumpReplicationData();
		}

		// Token: 0x0600331A RID: 13082 RVA: 0x000D37D0 File Offset: 0x000D19D0
		public static void ControlClear()
		{
			DebugNetworkEventStatistics.Clear();
		}

		// Token: 0x0600331B RID: 13083 RVA: 0x000D37D7 File Offset: 0x000D19D7
		public static void ClearNetGraphs()
		{
			DebugNetworkEventStatistics._eventSamples.Clear();
			DebugNetworkEventStatistics._lossSamples.Clear();
			DebugNetworkEventStatistics._prevEventData = new DebugNetworkEventStatistics.TotalEventData();
			DebugNetworkEventStatistics._currEventData = new DebugNetworkEventStatistics.TotalEventData();
			DebugNetworkEventStatistics._collectSampleCheck = 0f;
		}

		// Token: 0x0600331C RID: 13084 RVA: 0x000D380B File Offset: 0x000D1A0B
		public static void ClearFpsGraph()
		{
			DebugNetworkEventStatistics._fpsSamplesUntilNextSampling.Clear();
			DebugNetworkEventStatistics._fpsSamples.Clear();
			DebugNetworkEventStatistics._collectFpsSampleCheck = 0f;
		}

		// Token: 0x0600331D RID: 13085 RVA: 0x000D382B File Offset: 0x000D1A2B
		public static void ControlClearAll()
		{
			DebugNetworkEventStatistics.Clear();
			DebugNetworkEventStatistics.ClearFpsGraph();
			DebugNetworkEventStatistics.ClearNetGraphs();
			DebugNetworkEventStatistics.ClearReplicationData();
		}

		// Token: 0x0600331E RID: 13086 RVA: 0x000D3841 File Offset: 0x000D1A41
		public static void ControlDumpReplicationData()
		{
			DebugNetworkEventStatistics.DumpReplicationData();
			DebugNetworkEventStatistics.ClearReplicationData();
		}

		// Token: 0x0600331F RID: 13087 RVA: 0x000D3850 File Offset: 0x000D1A50
		public static void EndTick(float dt)
		{
			if (DebugNetworkEventStatistics._useImgui && Input.DebugInput.IsHotKeyPressed("DebugNetworkEventStatisticsHotkeyToggleActive"))
			{
				DebugNetworkEventStatistics.ToggleActive();
				if (DebugNetworkEventStatistics.IsActive)
				{
					Imgui.NewFrame();
				}
			}
			if (!DebugNetworkEventStatistics.IsActive)
			{
				return;
			}
			DebugNetworkEventStatistics._totalData.TotalTime += dt;
			DebugNetworkEventStatistics._totalData.TotalFrameCount++;
			if (DebugNetworkEventStatistics._useImgui)
			{
				Imgui.BeginMainThreadScope();
				Imgui.Begin("Network panel");
				if (Imgui.Button("Disable Network Panel"))
				{
					DebugNetworkEventStatistics.ToggleActive();
				}
				Imgui.Separator();
				if (Imgui.Button("Show Upload Data (screen)"))
				{
					DebugNetworkEventStatistics._showUploadDataText = !DebugNetworkEventStatistics._showUploadDataText;
				}
				Imgui.Separator();
				if (Imgui.Button("Clear Data"))
				{
					DebugNetworkEventStatistics.Clear();
				}
				if (Imgui.Button("Dump Data (console)"))
				{
					DebugNetworkEventStatistics.DumpData();
				}
				Imgui.Separator();
				if (Imgui.Button("Clear Replication Data"))
				{
					DebugNetworkEventStatistics.ClearReplicationData();
				}
				if (Imgui.Button("Dump Replication Data (console)"))
				{
					DebugNetworkEventStatistics.DumpReplicationData();
				}
				if (Imgui.Button("Dump & Clear Replication Data (console)"))
				{
					DebugNetworkEventStatistics.DumpReplicationData();
					DebugNetworkEventStatistics.ClearReplicationData();
				}
				if (DebugNetworkEventStatistics._showUploadDataText)
				{
					Imgui.Separator();
					DebugNetworkEventStatistics.ShowUploadData();
				}
				Imgui.End();
			}
			if (!DebugNetworkEventStatistics.IsActive)
			{
				return;
			}
			DebugNetworkEventStatistics.CollectFpsSample(dt);
			DebugNetworkEventStatistics._collectSampleCheck += dt;
			if (DebugNetworkEventStatistics._collectSampleCheck >= 1f / (float)DebugNetworkEventStatistics.SamplesPerSecond)
			{
				DebugNetworkEventStatistics._currEventData = DebugNetworkEventStatistics.GetCurrentEventData();
				if (DebugNetworkEventStatistics._currEventData.HasData && DebugNetworkEventStatistics._prevEventData.HasData && DebugNetworkEventStatistics._currEventData != DebugNetworkEventStatistics._prevEventData)
				{
					DebugNetworkEventStatistics._lossSamples.Enqueue(GameNetwork.GetAveragePacketLossRatio());
					DebugNetworkEventStatistics._eventSamples.Enqueue(DebugNetworkEventStatistics._currEventData - DebugNetworkEventStatistics._prevEventData);
					DebugNetworkEventStatistics._prevEventData = DebugNetworkEventStatistics._currEventData;
					if (DebugNetworkEventStatistics._eventSamples.Count > DebugNetworkEventStatistics.MaxGraphPointCount)
					{
						DebugNetworkEventStatistics._eventSamples.Dequeue();
						DebugNetworkEventStatistics._lossSamples.Dequeue();
					}
					if (DebugNetworkEventStatistics._eventSamples.Count >= DebugNetworkEventStatistics.SamplesPerSecond)
					{
						List<DebugNetworkEventStatistics.TotalEventData> range = DebugNetworkEventStatistics._eventSamples.ToList<DebugNetworkEventStatistics.TotalEventData>().GetRange(DebugNetworkEventStatistics._eventSamples.Count - DebugNetworkEventStatistics.SamplesPerSecond, DebugNetworkEventStatistics.SamplesPerSecond);
						DebugNetworkEventStatistics.UploadPerSecondEventData = new DebugNetworkEventStatistics.PerSecondEventData(range.Sum((DebugNetworkEventStatistics.TotalEventData x) => x.TotalUpload), range.Sum((DebugNetworkEventStatistics.TotalEventData x) => x.TotalConstantsUpload), range.Sum((DebugNetworkEventStatistics.TotalEventData x) => x.TotalReliableUpload), range.Sum((DebugNetworkEventStatistics.TotalEventData x) => x.TotalReplicationUpload), range.Sum((DebugNetworkEventStatistics.TotalEventData x) => x.TotalUnreliableUpload), range.Sum((DebugNetworkEventStatistics.TotalEventData x) => x.TotalOtherUpload));
						if (DebugNetworkEventStatistics.OnPerSecondEventDataUpdated != null)
						{
							DebugNetworkEventStatistics.OnPerSecondEventDataUpdated(DebugNetworkEventStatistics.UploadPerSecondEventData);
						}
					}
					if (DebugNetworkEventStatistics.OnEventDataUpdated != null)
					{
						DebugNetworkEventStatistics.OnEventDataUpdated(DebugNetworkEventStatistics._eventSamples.ToList<DebugNetworkEventStatistics.TotalEventData>());
					}
					DebugNetworkEventStatistics._collectSampleCheck -= 1f / (float)DebugNetworkEventStatistics.SamplesPerSecond;
				}
			}
			if (DebugNetworkEventStatistics._useImgui)
			{
				Imgui.Begin("Network Graph panel");
				float[] array = DebugNetworkEventStatistics._eventSamples.Select((DebugNetworkEventStatistics.TotalEventData x) => (float)x.TotalUpload / 8192f).ToArray<float>();
				float num = ((array.Length != 0) ? array.Max() : 0f);
				DebugNetworkEventStatistics._targetMaxGraphHeight = (DebugNetworkEventStatistics._useAbsoluteMaximum ? MathF.Max(num, DebugNetworkEventStatistics._targetMaxGraphHeight) : num);
				float num2 = MBMath.ClampFloat(3f * dt, 0f, 1f);
				DebugNetworkEventStatistics._curMaxGraphHeight = MBMath.Lerp(DebugNetworkEventStatistics._curMaxGraphHeight, DebugNetworkEventStatistics._targetMaxGraphHeight, num2, 1E-05f);
				if (DebugNetworkEventStatistics.UploadPerSecondEventData != null)
				{
					Imgui.Text(string.Concat(new object[]
					{
						"Taking ",
						DebugNetworkEventStatistics.SamplesPerSecond,
						" samples per second. Total KiB per second:",
						(float)DebugNetworkEventStatistics.UploadPerSecondEventData.TotalUploadPerSecond / 8192f
					}));
				}
				float[] array2 = DebugNetworkEventStatistics._eventSamples.Select((DebugNetworkEventStatistics.TotalEventData x) => (float)x.TotalConstantsUpload / 8192f).ToArray<float>();
				Imgui.PlotLines("", array2, DebugNetworkEventStatistics._eventSamples.Count, 0, "Constants upload (in KiB)", 0f, DebugNetworkEventStatistics._curMaxGraphHeight, 400f, 45f, 4);
				Imgui.SameLine(0f, 0f);
				Imgui.Text("Y-range: " + DebugNetworkEventStatistics._curMaxGraphHeight);
				float[] array3 = DebugNetworkEventStatistics._eventSamples.Select((DebugNetworkEventStatistics.TotalEventData x) => (float)x.TotalReliableUpload / 8192f).ToArray<float>();
				Imgui.PlotLines("", array3, DebugNetworkEventStatistics._eventSamples.Count, 0, "Reliable upload (in KiB)", 0f, DebugNetworkEventStatistics._curMaxGraphHeight, 400f, 45f, 4);
				Imgui.SameLine(0f, 0f);
				Imgui.Text("Y-range: " + DebugNetworkEventStatistics._curMaxGraphHeight);
				float[] array4 = DebugNetworkEventStatistics._eventSamples.Select((DebugNetworkEventStatistics.TotalEventData x) => (float)x.TotalReplicationUpload / 8192f).ToArray<float>();
				Imgui.PlotLines("", array4, DebugNetworkEventStatistics._eventSamples.Count, 0, "Replication upload (in KiB)", 0f, DebugNetworkEventStatistics._curMaxGraphHeight, 400f, 45f, 4);
				Imgui.SameLine(0f, 0f);
				Imgui.Text("Y-range: " + DebugNetworkEventStatistics._curMaxGraphHeight);
				float[] array5 = DebugNetworkEventStatistics._eventSamples.Select((DebugNetworkEventStatistics.TotalEventData x) => (float)x.TotalUnreliableUpload / 8192f).ToArray<float>();
				Imgui.PlotLines("", array5, DebugNetworkEventStatistics._eventSamples.Count, 0, "Unreliable upload (in KiB)", 0f, DebugNetworkEventStatistics._curMaxGraphHeight, 400f, 45f, 4);
				Imgui.SameLine(0f, 0f);
				Imgui.Text("Y-range: " + DebugNetworkEventStatistics._curMaxGraphHeight);
				float[] array6 = DebugNetworkEventStatistics._eventSamples.Select((DebugNetworkEventStatistics.TotalEventData x) => (float)x.TotalOtherUpload / 8192f).ToArray<float>();
				Imgui.PlotLines("", array6, DebugNetworkEventStatistics._eventSamples.Count, 0, "Other upload (in KiB)", 0f, DebugNetworkEventStatistics._curMaxGraphHeight, 400f, 45f, 4);
				Imgui.SameLine(0f, 0f);
				Imgui.Text("Y-range: " + DebugNetworkEventStatistics._curMaxGraphHeight);
				Imgui.Separator();
				float[] array7 = DebugNetworkEventStatistics._eventSamples.Select((DebugNetworkEventStatistics.TotalEventData x) => (float)x.TotalUpload / (float)x.TotalPackets / 8f).ToArray<float>();
				Imgui.PlotLines("", array7, DebugNetworkEventStatistics._eventSamples.Count, 0, "Data per package (in B)", 0f, 1400f, 400f, 45f, 4);
				Imgui.SameLine(0f, 0f);
				Imgui.Text("Y-range: " + 1400);
				Imgui.Separator();
				float num3 = ((DebugNetworkEventStatistics._lossSamples.Count > 0) ? DebugNetworkEventStatistics._lossSamples.Max() : 0f);
				DebugNetworkEventStatistics._targetMaxLossGraphHeight = (DebugNetworkEventStatistics._useAbsoluteMaximum ? MathF.Max(num3, DebugNetworkEventStatistics._targetMaxLossGraphHeight) : num3);
				float num4 = MBMath.ClampFloat(3f * dt, 0f, 1f);
				DebugNetworkEventStatistics._currMaxLossGraphHeight = MBMath.Lerp(DebugNetworkEventStatistics._currMaxLossGraphHeight, DebugNetworkEventStatistics._targetMaxLossGraphHeight, num4, 1E-05f);
				Imgui.PlotLines("", DebugNetworkEventStatistics._lossSamples.ToArray(), DebugNetworkEventStatistics._lossSamples.Count, 0, "Averaged loss ratio", 0f, DebugNetworkEventStatistics._currMaxLossGraphHeight, 400f, 45f, 4);
				Imgui.SameLine(0f, 0f);
				Imgui.Text("Y-range: " + DebugNetworkEventStatistics._currMaxLossGraphHeight);
				Imgui.Checkbox("Use absolute Maximum", ref DebugNetworkEventStatistics._useAbsoluteMaximum);
				Imgui.End();
			}
			Imgui.EndMainThreadScope();
		}

		// Token: 0x06003320 RID: 13088 RVA: 0x000D40BC File Offset: 0x000D22BC
		private static void CollectFpsSample(float dt)
		{
			if (DebugNetworkEventStatistics.TrackFps)
			{
				float fps = Utilities.GetFps();
				if (!float.IsInfinity(fps) && !float.IsNegativeInfinity(fps) && !float.IsNaN(fps))
				{
					DebugNetworkEventStatistics._fpsSamplesUntilNextSampling.Add(fps);
				}
				DebugNetworkEventStatistics._collectFpsSampleCheck += dt;
				if (DebugNetworkEventStatistics._collectFpsSampleCheck >= 1f / (float)DebugNetworkEventStatistics.SamplesPerSecond)
				{
					if (DebugNetworkEventStatistics._fpsSamplesUntilNextSampling.Count > 0)
					{
						DebugNetworkEventStatistics._fpsSamples.Enqueue(DebugNetworkEventStatistics._fpsSamplesUntilNextSampling.Min());
						DebugNetworkEventStatistics._fpsSamplesUntilNextSampling.Clear();
						if (DebugNetworkEventStatistics._fpsSamples.Count > DebugNetworkEventStatistics.MaxGraphPointCount)
						{
							DebugNetworkEventStatistics._fpsSamples.Dequeue();
						}
						Action<IEnumerable<float>> onFPSEventUpdated = DebugNetworkEventStatistics.OnFPSEventUpdated;
						if (onFPSEventUpdated != null)
						{
							onFPSEventUpdated(DebugNetworkEventStatistics._fpsSamples.ToList<float>());
						}
					}
					DebugNetworkEventStatistics._collectFpsSampleCheck -= 1f / (float)DebugNetworkEventStatistics.SamplesPerSecond;
				}
			}
		}

		// Token: 0x06003321 RID: 13089 RVA: 0x000D4193 File Offset: 0x000D2393
		private static void ToggleActive()
		{
			DebugNetworkEventStatistics.IsActive = !DebugNetworkEventStatistics.IsActive;
		}

		// Token: 0x06003322 RID: 13090 RVA: 0x000D41A2 File Offset: 0x000D23A2
		private static void Clear()
		{
			DebugNetworkEventStatistics._totalData = new DebugNetworkEventStatistics.TotalData();
			DebugNetworkEventStatistics._statistics = new Dictionary<int, DebugNetworkEventStatistics.PerEventData>();
			GameNetwork.ResetDebugUploads();
			DebugNetworkEventStatistics._curEventType = -1;
		}

		// Token: 0x06003323 RID: 13091 RVA: 0x000D41C4 File Offset: 0x000D23C4
		private static void DumpData()
		{
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "DumpData");
			mbstringBuilder.AppendLine();
			mbstringBuilder.AppendLine<string>("///GENERAL DATA///");
			mbstringBuilder.AppendLine<string>("Total elapsed time: " + DebugNetworkEventStatistics._totalData.TotalTime + " seconds.");
			mbstringBuilder.AppendLine<string>("Total frame count: " + DebugNetworkEventStatistics._totalData.TotalFrameCount);
			mbstringBuilder.AppendLine<string>("Total avg packet count: " + (int)(DebugNetworkEventStatistics._totalData.TotalTime / 60f));
			mbstringBuilder.AppendLine<string>("Total event data size: " + DebugNetworkEventStatistics._totalData.TotalDataSize + " bits.");
			mbstringBuilder.AppendLine<string>("Total event count: " + DebugNetworkEventStatistics._totalData.TotalCount);
			mbstringBuilder.AppendLine();
			mbstringBuilder.AppendLine<string>("///ALL DATA///");
			List<DebugNetworkEventStatistics.PerEventData> list = new List<DebugNetworkEventStatistics.PerEventData>();
			list.AddRange(DebugNetworkEventStatistics._statistics.Values);
			list.Sort();
			foreach (DebugNetworkEventStatistics.PerEventData perEventData in list)
			{
				mbstringBuilder.AppendLine<string>("Event name: " + perEventData.Name);
				mbstringBuilder.AppendLine<string>("\tEvent size (for one event): " + perEventData.DataSize + " bits.");
				mbstringBuilder.AppendLine<string>("\tTotal count: " + perEventData.Count);
				mbstringBuilder.AppendLine<string>(string.Concat(new object[]
				{
					"\tTotal size: ",
					perEventData.TotalDataSize,
					"bits | ~",
					perEventData.TotalDataSize / 8 + ((perEventData.TotalDataSize % 8 == 0) ? 0 : 1),
					" bytes."
				}));
				mbstringBuilder.AppendLine<string>("\tTotal count per frame: " + (float)perEventData.Count / (float)DebugNetworkEventStatistics._totalData.TotalFrameCount);
				mbstringBuilder.AppendLine<string>("\tTotal size per frame: " + (float)perEventData.TotalDataSize / (float)DebugNetworkEventStatistics._totalData.TotalFrameCount + " bits per frame.");
				mbstringBuilder.AppendLine();
			}
			DebugNetworkEventStatistics.GetFormattedDebugUploadDataOutput(ref mbstringBuilder);
			mbstringBuilder.AppendLine<string>("NetworkEventStaticticsLogLength: " + mbstringBuilder.Length + "\n");
			MBDebug.Print(mbstringBuilder.ToStringAndRelease(), 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x06003324 RID: 13092 RVA: 0x000D4480 File Offset: 0x000D2680
		private static void GetFormattedDebugUploadDataOutput(ref MBStringBuilder outStr)
		{
			GameNetwork.DebugNetworkPacketStatisticsStruct debugNetworkPacketStatisticsStruct = default(GameNetwork.DebugNetworkPacketStatisticsStruct);
			GameNetwork.DebugNetworkPositionCompressionStatisticsStruct debugNetworkPositionCompressionStatisticsStruct = default(GameNetwork.DebugNetworkPositionCompressionStatisticsStruct);
			GameNetwork.GetDebugUploadsInBits(ref debugNetworkPacketStatisticsStruct, ref debugNetworkPositionCompressionStatisticsStruct);
			outStr.AppendLine<string>("REAL NETWORK UPLOAD PERCENTS");
			if (debugNetworkPacketStatisticsStruct.TotalUpload == 0)
			{
				outStr.AppendLine<string>("Total Upload is ZERO");
				return;
			}
			int num = debugNetworkPacketStatisticsStruct.TotalUpload - (debugNetworkPacketStatisticsStruct.TotalConstantsUpload + debugNetworkPacketStatisticsStruct.TotalReliableEventUpload + debugNetworkPacketStatisticsStruct.TotalReplicationUpload + debugNetworkPacketStatisticsStruct.TotalUnreliableEventUpload);
			if (num == debugNetworkPacketStatisticsStruct.TotalUpload)
			{
				outStr.AppendLine<string>("USE_DEBUG_NETWORK_PACKET_PERCENTS not defined!");
			}
			else
			{
				outStr.AppendLine<string>("\tAverage Ping: " + debugNetworkPacketStatisticsStruct.average_ping_time);
				outStr.AppendLine<string>("\tTime out period: " + debugNetworkPacketStatisticsStruct.time_out_period);
				outStr.AppendLine<string>("\tLost Percent: " + debugNetworkPacketStatisticsStruct.lost_percent);
				outStr.AppendLine<string>("\tlost_count: " + debugNetworkPacketStatisticsStruct.lost_count);
				outStr.AppendLine<string>("\ttotal_count_on_lost_check: " + debugNetworkPacketStatisticsStruct.total_count_on_lost_check);
				outStr.AppendLine<string>("\tround_trip_time: " + debugNetworkPacketStatisticsStruct.round_trip_time);
				float num2 = (float)debugNetworkPacketStatisticsStruct.TotalUpload;
				float num3 = 1f / (float)debugNetworkPacketStatisticsStruct.TotalPackets;
				outStr.AppendLine<string>(string.Concat(new object[]
				{
					"\tConstants Upload: percent: ",
					(float)debugNetworkPacketStatisticsStruct.TotalConstantsUpload / num2 * 100f,
					"; size in bits: ",
					(float)debugNetworkPacketStatisticsStruct.TotalConstantsUpload * num3,
					";"
				}));
				outStr.AppendLine<string>(string.Concat(new object[]
				{
					"\tReliable Upload: percent: ",
					(float)debugNetworkPacketStatisticsStruct.TotalReliableEventUpload / num2 * 100f,
					"; size in bits: ",
					(float)debugNetworkPacketStatisticsStruct.TotalReliableEventUpload * num3,
					";"
				}));
				outStr.AppendLine<string>(string.Concat(new object[]
				{
					"\tReplication Upload: percent: ",
					(float)debugNetworkPacketStatisticsStruct.TotalReplicationUpload / num2 * 100f,
					"; size in bits: ",
					(float)debugNetworkPacketStatisticsStruct.TotalReplicationUpload * num3,
					";"
				}));
				outStr.AppendLine<string>(string.Concat(new object[]
				{
					"\tUnreliable Upload: percent: ",
					(float)debugNetworkPacketStatisticsStruct.TotalUnreliableEventUpload / num2 * 100f,
					"; size in bits: ",
					(float)debugNetworkPacketStatisticsStruct.TotalUnreliableEventUpload * num3,
					";"
				}));
				outStr.AppendLine<string>(string.Concat(new object[]
				{
					"\tOthers (headers, ack etc.) Upload: percent: ",
					(float)num / num2 * 100f,
					"; size in bits: ",
					(float)num * num3,
					";"
				}));
				int num4 = debugNetworkPositionCompressionStatisticsStruct.totalPositionCoarseBitCountX + debugNetworkPositionCompressionStatisticsStruct.totalPositionCoarseBitCountY + debugNetworkPositionCompressionStatisticsStruct.totalPositionCoarseBitCountZ;
				float num5 = 1f / (float)debugNetworkPacketStatisticsStruct.debug_total_cell_priority_checks;
				outStr.AppendLine<string>(string.Concat(new object[]
				{
					"\n\tTotal PPS: ",
					(float)debugNetworkPacketStatisticsStruct.TotalPackets / DebugNetworkEventStatistics._totalData.TotalTime,
					"; bps: ",
					(float)debugNetworkPacketStatisticsStruct.TotalUpload / DebugNetworkEventStatistics._totalData.TotalTime,
					";"
				}));
			}
			outStr.AppendLine<string>(string.Concat(new object[]
			{
				"\n\tTotal packets: ",
				debugNetworkPacketStatisticsStruct.TotalPackets,
				"; bits per packet: ",
				(float)debugNetworkPacketStatisticsStruct.TotalUpload / (float)debugNetworkPacketStatisticsStruct.TotalPackets,
				";"
			}));
			outStr.AppendLine<string>("Total Upload: " + debugNetworkPacketStatisticsStruct.TotalUpload + " in bits");
		}

		// Token: 0x06003325 RID: 13093 RVA: 0x000D484C File Offset: 0x000D2A4C
		private static void ShowUploadData()
		{
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "ShowUploadData");
			DebugNetworkEventStatistics.GetFormattedDebugUploadDataOutput(ref mbstringBuilder);
			string[] array = mbstringBuilder.ToStringAndRelease().Split(new char[] { '\n' });
			for (int i = 0; i < array.Length; i++)
			{
				Imgui.Text(array[i]);
			}
		}

		// Token: 0x06003326 RID: 13094 RVA: 0x000D48A4 File Offset: 0x000D2AA4
		private static DebugNetworkEventStatistics.TotalEventData GetCurrentEventData()
		{
			GameNetwork.DebugNetworkPacketStatisticsStruct debugNetworkPacketStatisticsStruct = default(GameNetwork.DebugNetworkPacketStatisticsStruct);
			GameNetwork.DebugNetworkPositionCompressionStatisticsStruct debugNetworkPositionCompressionStatisticsStruct = default(GameNetwork.DebugNetworkPositionCompressionStatisticsStruct);
			GameNetwork.GetDebugUploadsInBits(ref debugNetworkPacketStatisticsStruct, ref debugNetworkPositionCompressionStatisticsStruct);
			return new DebugNetworkEventStatistics.TotalEventData(debugNetworkPacketStatisticsStruct.TotalPackets, debugNetworkPacketStatisticsStruct.TotalUpload, debugNetworkPacketStatisticsStruct.TotalConstantsUpload, debugNetworkPacketStatisticsStruct.TotalReliableEventUpload, debugNetworkPacketStatisticsStruct.TotalReplicationUpload, debugNetworkPacketStatisticsStruct.TotalUnreliableEventUpload);
		}

		// Token: 0x06003327 RID: 13095 RVA: 0x000D48F3 File Offset: 0x000D2AF3
		private static void DumpReplicationData()
		{
			GameNetwork.PrintReplicationTableStatistics();
		}

		// Token: 0x06003328 RID: 13096 RVA: 0x000D48FA File Offset: 0x000D2AFA
		private static void ClearReplicationData()
		{
			GameNetwork.ClearReplicationTableStatistics();
		}

		// Token: 0x040015BE RID: 5566
		private static DebugNetworkEventStatistics.TotalData _totalData = new DebugNetworkEventStatistics.TotalData();

		// Token: 0x040015BF RID: 5567
		private static int _curEventType = -1;

		// Token: 0x040015C0 RID: 5568
		private static Dictionary<int, DebugNetworkEventStatistics.PerEventData> _statistics = new Dictionary<int, DebugNetworkEventStatistics.PerEventData>();

		// Token: 0x040015C1 RID: 5569
		private static int _samplesPerSecond;

		// Token: 0x040015C2 RID: 5570
		public static int MaxGraphPointCount;

		// Token: 0x040015C3 RID: 5571
		private static bool _showUploadDataText = false;

		// Token: 0x040015C4 RID: 5572
		private static bool _useAbsoluteMaximum = false;

		// Token: 0x040015C5 RID: 5573
		private static float _collectSampleCheck = 0f;

		// Token: 0x040015C6 RID: 5574
		private static float _collectFpsSampleCheck = 0f;

		// Token: 0x040015C7 RID: 5575
		private static float _curMaxGraphHeight = 0f;

		// Token: 0x040015C8 RID: 5576
		private static float _targetMaxGraphHeight = 0f;

		// Token: 0x040015C9 RID: 5577
		private static float _currMaxLossGraphHeight = 0f;

		// Token: 0x040015CA RID: 5578
		private static float _targetMaxLossGraphHeight = 0f;

		// Token: 0x040015CB RID: 5579
		private static DebugNetworkEventStatistics.PerSecondEventData UploadPerSecondEventData;

		// Token: 0x040015CC RID: 5580
		private static readonly Queue<DebugNetworkEventStatistics.TotalEventData> _eventSamples = new Queue<DebugNetworkEventStatistics.TotalEventData>();

		// Token: 0x040015CD RID: 5581
		private static readonly Queue<float> _lossSamples = new Queue<float>();

		// Token: 0x040015CE RID: 5582
		private static DebugNetworkEventStatistics.TotalEventData _prevEventData = new DebugNetworkEventStatistics.TotalEventData();

		// Token: 0x040015CF RID: 5583
		private static DebugNetworkEventStatistics.TotalEventData _currEventData = new DebugNetworkEventStatistics.TotalEventData();

		// Token: 0x040015D0 RID: 5584
		private static readonly List<float> _fpsSamplesUntilNextSampling = new List<float>();

		// Token: 0x040015D1 RID: 5585
		private static readonly Queue<float> _fpsSamples = new Queue<float>();

		// Token: 0x040015D2 RID: 5586
		private static bool _useImgui = !GameNetwork.IsDedicatedServer;

		// Token: 0x040015D3 RID: 5587
		public static bool TrackFps = false;

		// Token: 0x020006B8 RID: 1720
		public class TotalEventData
		{
			// Token: 0x06003FB3 RID: 16307 RVA: 0x000F8C88 File Offset: 0x000F6E88
			protected bool Equals(DebugNetworkEventStatistics.TotalEventData other)
			{
				return this.TotalPackets == other.TotalPackets && this.TotalUpload == other.TotalUpload && this.TotalConstantsUpload == other.TotalConstantsUpload && this.TotalReliableUpload == other.TotalReliableUpload && this.TotalReplicationUpload == other.TotalReplicationUpload && this.TotalUnreliableUpload == other.TotalUnreliableUpload && this.TotalOtherUpload == other.TotalOtherUpload;
			}

			// Token: 0x06003FB4 RID: 16308 RVA: 0x000F8CF9 File Offset: 0x000F6EF9
			public override bool Equals(object obj)
			{
				return obj != null && (this == obj || (obj.GetType() == base.GetType() && this.Equals((DebugNetworkEventStatistics.TotalEventData)obj)));
			}

			// Token: 0x06003FB5 RID: 16309 RVA: 0x000F8D28 File Offset: 0x000F6F28
			public override int GetHashCode()
			{
				return (((((((((((this.TotalPackets * 397) ^ this.TotalUpload) * 397) ^ this.TotalConstantsUpload) * 397) ^ this.TotalReliableUpload) * 397) ^ this.TotalReplicationUpload) * 397) ^ this.TotalUnreliableUpload) * 397) ^ this.TotalOtherUpload;
			}

			// Token: 0x06003FB6 RID: 16310 RVA: 0x000F8D89 File Offset: 0x000F6F89
			public TotalEventData()
			{
			}

			// Token: 0x06003FB7 RID: 16311 RVA: 0x000F8D94 File Offset: 0x000F6F94
			public TotalEventData(int totalPackets, int totalUpload, int totalConstants, int totalReliable, int totalReplication, int totalUnreliable)
			{
				this.TotalPackets = totalPackets;
				this.TotalUpload = totalUpload;
				this.TotalConstantsUpload = totalConstants;
				this.TotalReliableUpload = totalReliable;
				this.TotalReplicationUpload = totalReplication;
				this.TotalUnreliableUpload = totalUnreliable;
				this.TotalOtherUpload = totalUpload - (totalConstants + totalReliable + totalReplication + totalUnreliable);
			}

			// Token: 0x17000A12 RID: 2578
			// (get) Token: 0x06003FB8 RID: 16312 RVA: 0x000F8DE6 File Offset: 0x000F6FE6
			public bool HasData
			{
				get
				{
					return this.TotalUpload > 0;
				}
			}

			// Token: 0x06003FB9 RID: 16313 RVA: 0x000F8DF4 File Offset: 0x000F6FF4
			public static DebugNetworkEventStatistics.TotalEventData operator -(DebugNetworkEventStatistics.TotalEventData d1, DebugNetworkEventStatistics.TotalEventData d2)
			{
				return new DebugNetworkEventStatistics.TotalEventData(d1.TotalPackets - d2.TotalPackets, d1.TotalUpload - d2.TotalUpload, d1.TotalConstantsUpload - d2.TotalConstantsUpload, d1.TotalReliableUpload - d2.TotalReliableUpload, d1.TotalReplicationUpload - d2.TotalReplicationUpload, d1.TotalUnreliableUpload - d2.TotalUnreliableUpload);
			}

			// Token: 0x06003FBA RID: 16314 RVA: 0x000F8E54 File Offset: 0x000F7054
			public static bool operator ==(DebugNetworkEventStatistics.TotalEventData d1, DebugNetworkEventStatistics.TotalEventData d2)
			{
				return d1.TotalPackets == d2.TotalPackets && d1.TotalUpload == d2.TotalUpload && d1.TotalConstantsUpload == d2.TotalConstantsUpload && d1.TotalReliableUpload == d2.TotalReliableUpload && d1.TotalReplicationUpload == d2.TotalReplicationUpload && d1.TotalUnreliableUpload == d2.TotalUnreliableUpload;
			}

			// Token: 0x06003FBB RID: 16315 RVA: 0x000F8EB7 File Offset: 0x000F70B7
			public static bool operator !=(DebugNetworkEventStatistics.TotalEventData d1, DebugNetworkEventStatistics.TotalEventData d2)
			{
				return !(d1 == d2);
			}

			// Token: 0x0400226D RID: 8813
			public readonly int TotalPackets;

			// Token: 0x0400226E RID: 8814
			public readonly int TotalUpload;

			// Token: 0x0400226F RID: 8815
			public readonly int TotalConstantsUpload;

			// Token: 0x04002270 RID: 8816
			public readonly int TotalReliableUpload;

			// Token: 0x04002271 RID: 8817
			public readonly int TotalReplicationUpload;

			// Token: 0x04002272 RID: 8818
			public readonly int TotalUnreliableUpload;

			// Token: 0x04002273 RID: 8819
			public readonly int TotalOtherUpload;
		}

		// Token: 0x020006B9 RID: 1721
		private class PerEventData : IComparable<DebugNetworkEventStatistics.PerEventData>
		{
			// Token: 0x06003FBC RID: 16316 RVA: 0x000F8EC3 File Offset: 0x000F70C3
			public int CompareTo(DebugNetworkEventStatistics.PerEventData other)
			{
				return other.TotalDataSize - this.TotalDataSize;
			}

			// Token: 0x04002274 RID: 8820
			public string Name;

			// Token: 0x04002275 RID: 8821
			public int DataSize;

			// Token: 0x04002276 RID: 8822
			public int TotalDataSize;

			// Token: 0x04002277 RID: 8823
			public int Count;
		}

		// Token: 0x020006BA RID: 1722
		public class PerSecondEventData
		{
			// Token: 0x06003FBE RID: 16318 RVA: 0x000F8EDA File Offset: 0x000F70DA
			public PerSecondEventData(int totalUploadPerSecond, int constantsUploadPerSecond, int reliableUploadPerSecond, int replicationUploadPerSecond, int unreliableUploadPerSecond, int otherUploadPerSecond)
			{
				this.TotalUploadPerSecond = totalUploadPerSecond;
				this.ConstantsUploadPerSecond = constantsUploadPerSecond;
				this.ReliableUploadPerSecond = reliableUploadPerSecond;
				this.ReplicationUploadPerSecond = replicationUploadPerSecond;
				this.UnreliableUploadPerSecond = unreliableUploadPerSecond;
				this.OtherUploadPerSecond = otherUploadPerSecond;
			}

			// Token: 0x04002278 RID: 8824
			public readonly int TotalUploadPerSecond;

			// Token: 0x04002279 RID: 8825
			public readonly int ConstantsUploadPerSecond;

			// Token: 0x0400227A RID: 8826
			public readonly int ReliableUploadPerSecond;

			// Token: 0x0400227B RID: 8827
			public readonly int ReplicationUploadPerSecond;

			// Token: 0x0400227C RID: 8828
			public readonly int UnreliableUploadPerSecond;

			// Token: 0x0400227D RID: 8829
			public readonly int OtherUploadPerSecond;
		}

		// Token: 0x020006BB RID: 1723
		private class TotalData
		{
			// Token: 0x0400227E RID: 8830
			public float TotalTime;

			// Token: 0x0400227F RID: 8831
			public int TotalFrameCount;

			// Token: 0x04002280 RID: 8832
			public int TotalCount;

			// Token: 0x04002281 RID: 8833
			public int TotalDataSize;
		}
	}
}

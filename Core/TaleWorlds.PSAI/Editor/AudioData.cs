using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using psai.net;

namespace psai.Editor
{
	[Serializable]
	public class AudioData : ICloneable
	{
		[XmlElement("Path")]
		public string FilePathRelativeToProjectDir
		{
			get
			{
				return this._filePathRelativeToProjectDir;
			}
			set
			{
				string text = value.Replace(Path.DirectorySeparatorChar, '/');
				this._filePathRelativeToProjectDir = text;
			}
		}

		[XmlIgnore]
		public string FilePathRelativeToProjectDirForCurrentSystem
		{
			get
			{
				return this._filePathRelativeToProjectDir.Replace('/', Path.DirectorySeparatorChar);
			}
		}

		public float Bpm { get; set; }

		public float PreBeats { get; set; }

		public float PostBeats { get; set; }

		public bool CalculatePostAndPrebeatLengthBasedOnBeats { get; set; }

		public int PreBeatLengthInSamples
		{
			get
			{
				if (this.CalculatePostAndPrebeatLengthBasedOnBeats)
				{
					return this.GetPrebeatLengthInSamplesBasedOnBeats();
				}
				return this._prebeatLengthInSamplesEnteredManually;
			}
			set
			{
				this._prebeatLengthInSamplesEnteredManually = value;
			}
		}

		public int PostBeatLengthInSamples
		{
			get
			{
				if (this.CalculatePostAndPrebeatLengthBasedOnBeats)
				{
					return this.GetPostbeatLengthInSamplesBasedOnBeats();
				}
				return this._postbeatLengthInSamplesEnteredManually;
			}
			set
			{
				this._postbeatLengthInSamplesEnteredManually = value;
			}
		}

		public int TotalLengthInSamples { get; set; }

		public int SampleRate { get; set; }

		[XmlIgnore]
		public int BitsPerSample { get; set; }

		[XmlIgnore]
		public int ChannelCount { get; set; }

		[XmlIgnore]
		public long ByteIndexOfWaveformDataWithinAudioFile { get; set; }

		[XmlIgnore]
		public int LengthOfWaveformDataInBytes { get; set; }

		public AudioData()
		{
			this.FilePathRelativeToProjectDir = "";
			this.BitsPerSample = 0;
			this.PostBeatLengthInSamples = 0;
			this.PreBeatLengthInSamples = 0;
			this.SampleRate = 0;
			this.LengthOfWaveformDataInBytes = 0;
			this.Bpm = 100f;
			this.PreBeats = 1f;
			this.PostBeats = 1f;
			this.CalculatePostAndPrebeatLengthBasedOnBeats = false;
		}

		public AudioData CreatePsaiDotNetVersion()
		{
			AudioData audioData = new AudioData();
			audioData.filePathRelativeToProjectDir = this.FilePathRelativeToProjectDir;
			if (this.CalculatePostAndPrebeatLengthBasedOnBeats)
			{
				audioData.sampleCountPreBeat = this.GetPrebeatLengthInSamplesBasedOnBeats();
				audioData.sampleCountPostBeat = this.GetPostbeatLengthInSamplesBasedOnBeats();
			}
			else
			{
				audioData.sampleCountPreBeat = this.PreBeatLengthInSamples;
				audioData.sampleCountPostBeat = this.PostBeatLengthInSamples;
			}
			audioData.sampleCountTotal = this.TotalLengthInSamples;
			audioData.sampleRateHz = this.SampleRate;
			audioData.bpm = this.Bpm;
			return audioData;
		}

		public int GetMillisecondsFromSampleCount(int sampleCount)
		{
			return (int)((long)sampleCount * 1000L / (long)this.SampleRate);
		}

		public int GetSampleCountFromMilliseconds(int durationMs)
		{
			return this.SampleRate * durationMs / 1000;
		}

		public int GetLengthInSamplesBasedOnBeats(float bpm, float beats)
		{
			int num = (int)(60000f / bpm);
			return this.GetSampleCountFromMilliseconds((int)((float)num * beats));
		}

		public int GetPostbeatLengthInSamplesBasedOnBeats()
		{
			return this.GetLengthInSamplesBasedOnBeats(this.Bpm, this.PostBeats);
		}

		public int GetPrebeatLengthInSamplesBasedOnBeats()
		{
			return this.GetLengthInSamplesBasedOnBeats(this.Bpm, this.PreBeats);
		}

		public static int CalculateTotalLengthInSamples(int lengthOfWaveformDataInBytes, int bitsPerSample, int channelCount)
		{
			if (lengthOfWaveformDataInBytes > 0 && bitsPerSample > 0 && channelCount > 0)
			{
				return lengthOfWaveformDataInBytes / (bitsPerSample / 8) / channelCount;
			}
			return 0;
		}

		public bool DoUpdateMembersBasedOnWaveHeader(string fullPathToAudioFile, out string errorMessage)
		{
			bool flag = false;
			if (fullPathToAudioFile != null && fullPathToAudioFile.Length > 0)
			{
				string text = fullPathToAudioFile.Replace('/', Path.DirectorySeparatorChar);
				text = text.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
				if (File.Exists(text))
				{
					Stream stream = null;
					int num = 0;
					while (stream == null && num < 100)
					{
						try
						{
							stream = File.Open(text, FileMode.Open, FileAccess.Read, FileShare.Read);
						}
						catch (IOException ex)
						{
							errorMessage = ex.ToString() + "   numberOfTries=" + num;
							Thread.Sleep(50);
						}
						num++;
					}
					if (stream != null)
					{
						int num2;
						int num3;
						int num4;
						int num5;
						long num6;
						if (AudioData.ReadWaveHeader(stream, out num2, out num3, out num4, out num5, out num6) == PsaiResult.OK)
						{
							this.ChannelCount = num2;
							this.SampleRate = num3;
							this.LengthOfWaveformDataInBytes = num5;
							this.BitsPerSample = num4;
							this.ByteIndexOfWaveformDataWithinAudioFile = num6;
							this.TotalLengthInSamples = AudioData.CalculateTotalLengthInSamples(num5, num4, num2);
							errorMessage = "";
							flag = true;
						}
						else
						{
							errorMessage = "ERROR: file '" + text + "' contains an unsupported format. Please make sure your audio files are standard RIFF WAV files with up to 16 bits / 44.1kHz.";
						}
						stream.Close();
						return flag;
					}
					errorMessage = "ERROR: audio file '" + text + "' could not be opened. ";
					return false;
				}
			}
			errorMessage = "ERROR: audio file '" + fullPathToAudioFile + "' could not be found. Please make sure that all audio files reside within a subfolder of your project directory";
			return false;
		}

		public static bool SeekChunkInWaveHeader(ref BinaryReader reader, string chunk)
		{
			if (chunk.Length != 4)
			{
				return false;
			}
			Queue<byte> queue = new Queue<byte>(4);
			try
			{
				while (reader.BaseStream.CanRead)
				{
					byte b;
					do
					{
						b = reader.ReadByte();
						queue.Enqueue(b);
						if (queue.Count > 4)
						{
							queue.Dequeue();
						}
					}
					while ((char)b != chunk[3]);
					if (Encoding.ASCII.GetString(queue.ToArray()).Equals(chunk))
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return false;
			}
			return false;
		}

		public static PsaiResult ReadWaveHeader(Stream stream, out int outChannelCount, out int outSampleRate, out int outBitsPerSample, out int outLengthOfWaveformDatablockInBytes, out long outBytePositionOfWaveformData)
		{
			outChannelCount = 0;
			outBitsPerSample = 0;
			outSampleRate = 0;
			outLengthOfWaveformDatablockInBytes = 0;
			outBytePositionOfWaveformData = 0L;
			BinaryReader binaryReader = new BinaryReader(stream);
			if (new string(binaryReader.ReadChars(4)) != "RIFF")
			{
				binaryReader.Close();
				return PsaiResult.format_error;
			}
			binaryReader.ReadInt32();
			if (new string(binaryReader.ReadChars(4)) != "WAVE")
			{
				binaryReader.Close();
				return PsaiResult.format_error;
			}
			try
			{
				if (!AudioData.SeekChunkInWaveHeader(ref binaryReader, "fmt "))
				{
					Console.WriteLine(".wave file corrupt! format-chunk not found.");
					binaryReader.Close();
					return PsaiResult.format_error;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				binaryReader.Close();
				return PsaiResult.format_error;
			}
			long position = binaryReader.BaseStream.Position;
			int num = binaryReader.ReadInt32();
			binaryReader.ReadInt16();
			int num2 = (int)binaryReader.ReadInt16();
			int num3 = binaryReader.ReadInt32();
			binaryReader.ReadInt32();
			binaryReader.ReadInt16();
			int num4 = (int)binaryReader.ReadInt16();
			if (num4 > 16)
			{
				Console.WriteLine("OpenAL does not support playback of 24 bits. Please convert to 16 bits.");
				return PsaiResult.output_format_error;
			}
			long num5 = position + (long)num + 4L;
			binaryReader.BaseStream.Seek(num5, SeekOrigin.Begin);
			if (!AudioData.SeekChunkInWaveHeader(ref binaryReader, "data"))
			{
				binaryReader.BaseStream.Seek(0L, SeekOrigin.Begin);
				if (!AudioData.SeekChunkInWaveHeader(ref binaryReader, "data"))
				{
					Console.WriteLine("wave file corrupt! no 'data' chunk found!");
					binaryReader.Close();
					return PsaiResult.format_error;
				}
			}
			int num6 = stream.ReadByte() + stream.ReadByte() * 256 + stream.ReadByte() * 65536 + stream.ReadByte() * 16777216;
			outLengthOfWaveformDatablockInBytes = num6;
			outBytePositionOfWaveformData = stream.Position;
			outChannelCount = num2;
			outBitsPerSample = num4;
			outSampleRate = num3;
			binaryReader.Close();
			return PsaiResult.OK;
		}

		public static byte[] LoadWaveformDataToByteArray(string fullFilePath, long byteIndexOfWaveformDataWithinAudioFile, int lengthOfWaveformDataInBytes)
		{
			Stream stream = null;
			int num = 0;
			while (stream == null && num < 100)
			{
				string text = fullFilePath.Replace('/', Path.DirectorySeparatorChar);
				text = text.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
				try
				{
					stream = File.Open(text, FileMode.Open, FileAccess.Read, FileShare.Read);
				}
				catch (IOException ex)
				{
					Console.WriteLine(ex.ToString() + "   numberOfTries=" + num);
					Thread.Sleep(50);
				}
				num++;
			}
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			byte[] array = null;
			using (BinaryReader binaryReader = new BinaryReader(stream))
			{
				try
				{
					binaryReader.BaseStream.Position = byteIndexOfWaveformDataWithinAudioFile;
					array = binaryReader.ReadBytes(lengthOfWaveformDataInBytes);
				}
				catch (Exception ex2)
				{
					Console.WriteLine("Exception reading Audio Data! e=" + ex2.ToString() + "  " + ex2.Message);
				}
			}
			stream.Close();
			return array;
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}

		private string _filePathRelativeToProjectDir = "";

		public int _prebeatLengthInSamplesEnteredManually;

		public int _postbeatLengthInSamplesEnteredManually;
	}
}

using System;
using System.IO;
using System.Xml.Serialization;

namespace Sdl.Core.FineGrainedAlignment.Core
{
	[XmlType("AlignedSpanPair")]
	public class LiftAlignedSpanPair
	{
		public enum LiftAlignedSpanPairProvenance
		{
			[XmlEnum("1")]
			NodeAlignment = 1,
			[XmlEnum("2")]
			Deduction,
			[XmlEnum("3")]
			PlaceableAlignment,
			[XmlEnum("4")]
			PhraseExtractor,
			[XmlEnum("5")]
			Structure,
			[XmlEnum("6")]
			TrainedAligner,
			[XmlEnum("7")]
			Other
		}

		public LiftSpan SourceSpan = new LiftSpan();

		public LiftSpan TargetSpan = new LiftSpan();

		[XmlAttribute]
		public float Confidence;

		[XmlAttribute]
		public byte Provenance;

		public short SourceEndIndex => SourceSpan.EndIndex;

		public short TargetEndIndex => TargetSpan.EndIndex;

		public short SourceStartIndex => SourceSpan.StartIndex;

		public short TargetStartIndex => TargetSpan.StartIndex;

		public short TargetLength => TargetSpan.Length;

		public short SourceLength => SourceSpan.Length;

		public LiftAlignedSpanPair()
		{
		}

		public LiftAlignedSpanPair(LiftSpan sourceSpan, LiftSpan targetSpan)
			: this(sourceSpan.StartIndex, sourceSpan.Length, targetSpan.StartIndex, targetSpan.Length)
		{
		}

		public override string ToString()
		{
			return $"{SourceSpan}={TargetSpan}";
		}

		public LiftAlignedSpanPair(short sourceStartIndex, short sourceLength, short targetStartIndex, short targetLength)
		{
			SourceSpan.StartIndex = sourceStartIndex;
			SourceSpan.Length = sourceLength;
			TargetSpan.StartIndex = targetStartIndex;
			TargetSpan.Length = targetLength;
			Provenance = 7;
		}

		public bool Contradicts(LiftAlignedSpanPair other, bool repetitionIsContradiction = true)
		{
			return DoContradicts(other, repetitionIsContradiction);
		}

		private bool DoContradicts(LiftAlignedSpanPair other, bool repetitionIsContradiction)
		{
			if (SourceSpan.SpanEquals(other.SourceSpan) || TargetSpan.SpanEquals(other.TargetSpan))
			{
				if (repetitionIsContradiction)
				{
					return true;
				}
				if (SourceSpan.SpanEquals(other.SourceSpan) && TargetSpan.SpanEquals(other.TargetSpan))
				{
					return false;
				}
				return true;
			}
			if (SourceSpan.Overlaps(other.SourceSpan))
			{
				if (SourceSpan.Covers(other.SourceSpan, 0))
				{
					return !TargetSpan.Covers(other.TargetSpan, 0);
				}
				if (other.SourceSpan.Covers(SourceSpan, 0))
				{
					return !other.TargetSpan.Covers(TargetSpan, 0);
				}
				return true;
			}
			return TargetSpan.Overlaps(other.TargetSpan);
		}

		public void Validate()
		{
			SourceSpan.Validate();
			TargetSpan.Validate();
			if (Provenance == 0)
			{
				throw new Exception("Invalid Provenance value: 0");
			}
			if (Confidence < 0f || Confidence > 1f)
			{
				throw new Exception("Invalid Confidence value: " + Confidence.ToString());
			}
		}

		internal static LiftAlignedSpanPair Load(BinaryReader reader, bool compactSzn)
		{
			LiftAlignedSpanPair liftAlignedSpanPair = new LiftAlignedSpanPair();
			liftAlignedSpanPair.SourceSpan = LiftSpan.Load(reader, compactSzn);
			liftAlignedSpanPair.TargetSpan = LiftSpan.Load(reader, compactSzn);
			liftAlignedSpanPair.Confidence = 0f;
			if (reader.ReadBoolean())
			{
				liftAlignedSpanPair.Confidence = reader.ReadSingle();
			}
			liftAlignedSpanPair.Provenance = reader.ReadByte();
			return liftAlignedSpanPair;
		}

		internal void Save(BinaryWriter writer, bool compactSzn)
		{
			SourceSpan.Save(writer, compactSzn);
			TargetSpan.Save(writer, compactSzn);
			writer.Write(Confidence > 0f);
			if (Confidence > 0f)
			{
				writer.Write(Confidence);
			}
			writer.Write(Provenance);
		}

		public LiftSpan Span(bool searchTargetText)
		{
			if (!searchTargetText)
			{
				return SourceSpan;
			}
			return TargetSpan;
		}

		public LiftSpan TranslationSpan(bool searchTargetText)
		{
			if (searchTargetText)
			{
				return SourceSpan;
			}
			return TargetSpan;
		}

		public short StartIndex(bool searchTargetText)
		{
			if (!searchTargetText)
			{
				return SourceSpan.StartIndex;
			}
			return TargetSpan.StartIndex;
		}

		public short TranslationStartIndex(bool searchTargetText)
		{
			if (searchTargetText)
			{
				return SourceSpan.StartIndex;
			}
			return TargetSpan.StartIndex;
		}

		public short Length(bool searchTargetText)
		{
			if (!searchTargetText)
			{
				return SourceSpan.Length;
			}
			return TargetSpan.Length;
		}

		public short TranslationLength(bool searchTargetText)
		{
			if (searchTargetText)
			{
				return SourceSpan.Length;
			}
			return TargetSpan.Length;
		}

		public short EndIndex(bool searchTargetText)
		{
			if (!searchTargetText)
			{
				return SourceEndIndex;
			}
			return TargetEndIndex;
		}

		public short TranslationEndIndex(bool searchTargetText)
		{
			if (searchTargetText)
			{
				return SourceEndIndex;
			}
			return TargetEndIndex;
		}
	}
}

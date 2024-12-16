using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Common;
using System;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel
{
	[DataContract]
	public class DependencyFile : MetadataContainer, ICloneable<DependencyFile>, IEquatable<DependencyFile>
	{
		[DataMember(Name = "id")]
		public string Id
		{
			get;
			set;
		}

		[DataMember(Name = "usage")]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public DependencyFileUsage Usage
		{
			get;
			set;
		}

		[DataMember(Name = "location")]
		public string Location
		{
			get;
			set;
		}

		[DataMember(Name = "fileName")]
		public string FileName
		{
			get;
			set;
		}

		[DataMember(Name = "provider")]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
		public string Provider
		{
			get;
			set;
		}

		[DataMember(Name = "embeddedContent")]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
		public string EmbeddedContent
		{
			get;
			set;
		}

		public DependencyFile Clone()
		{
			DependencyFile dependencyFile = (DependencyFile)MemberwiseClone();
			dependencyFile.ReplaceMetadataWith(base.Metadata);
			return dependencyFile;
		}

		public bool Equals(DependencyFile other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (string.Equals(Id, other.Id) && Usage == other.Usage && string.Equals(Location, other.Location))
			{
				return string.Equals(FileName, other.FileName);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((DependencyFile)obj);
		}

		public override int GetHashCode()
		{
			int num = (Id != null) ? Id.GetHashCode() : 0;
			num = ((num * 397) ^ (int)Usage);
			num = ((num * 397) ^ ((Location != null) ? Location.GetHashCode() : 0));
			return (num * 397) ^ ((FileName != null) ? FileName.GetHashCode() : 0);
		}
	}
}

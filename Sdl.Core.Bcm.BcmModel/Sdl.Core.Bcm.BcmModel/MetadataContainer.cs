using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmModel.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sdl.Core.Bcm.BcmModel
{
	[DataContract]
	public abstract class MetadataContainer : ExtensionDataContainer, IEquatable<MetadataContainer>
	{
		[JsonProperty(PropertyName = "metadata", NullValueHandling = NullValueHandling.Ignore, Order = 2147483646)]
		private DictionaryEx<string, string> _metadata = new DictionaryEx<string, string>();

		public bool HasMetaData => _metadata.Count > 0;

		public int MetaDataCount => _metadata.Count;

		public IEnumerable<KeyValuePair<string, string>> Metadata => _metadata;

		public void AddMetadataFrom(IEnumerable<KeyValuePair<string, string>> from)
		{
			if (from == null)
			{
				throw new ArgumentNullException("from");
			}
			foreach (KeyValuePair<string, string> item in from)
			{
				_metadata[item.Key] = item.Value;
			}
		}

		public void ReplaceMetadataWith(IEnumerable<KeyValuePair<string, string>> from)
		{
			if (from == null)
			{
				throw new ArgumentNullException("from");
			}
			_metadata = new DictionaryEx<string, string>(from);
		}

		public bool MetaDataContainsKey(string key)
		{
			return _metadata.ContainsKey(key);
		}

		public string GetMetadata(string key)
		{
			if (!_metadata.ContainsKey(key))
			{
				return null;
			}
			return _metadata[key];
		}

		public void SetMetadata(string key, string value)
		{
			_metadata[key] = value;
		}

		public void RemoveMetadata(string key)
		{
			_metadata.Remove(key);
		}

		public void ClearMetadata()
		{
			_metadata.Clear();
		}

		protected void CopyPropertiesTo(MetadataContainer target)
		{
			target.AddMetadataFrom(Metadata);
		}

		public bool Equals(MetadataContainer other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (MetaDataCount == 0 && other.MetaDataCount == 0)
			{
				return true;
			}
			if (MetaDataCount != other.MetaDataCount)
			{
				return false;
			}
			return _metadata.Equals(other._metadata);
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
			return Equals((MetadataContainer)obj);
		}

		public override int GetHashCode()
		{
			if (_metadata == null)
			{
				return 0;
			}
			return _metadata.GetHashCode();
		}

		public bool ShouldSerialize_metadata()
		{
			if (_metadata != null)
			{
				return _metadata.Count != 0;
			}
			return false;
		}
	}
}

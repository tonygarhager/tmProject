using System;
using System.IO;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class Filter
	{
		private string _name;

		private FilterExpression _filterExpression;

		private int _penalty;

		[DataMember]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value.Trim();
				if (_name.IndexOf(' ') >= 0)
				{
					throw new ArgumentException("Filter name cannot contain blanks");
				}
				if (_name.Length == 0)
				{
					throw new ArgumentException("Filter name cannot be empty");
				}
			}
		}

		[DataMember]
		public FilterExpression FilterExpression
		{
			get
			{
				return _filterExpression;
			}
			set
			{
				_filterExpression = (value ?? throw new ArgumentNullException());
			}
		}

		[DataMember]
		public int Penalty
		{
			get
			{
				return _penalty;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				_penalty = value;
			}
		}

		public Filter(FilterExpression filterExpression, string name, int penalty)
		{
			Name = name;
			FilterExpression = filterExpression;
			Penalty = penalty;
		}

		public void Save(string fileName)
		{
			using (FileStream outputStream = File.Create(fileName))
			{
				Save(outputStream);
			}
		}

		public void Save(Stream outputStream)
		{
			new DataContractSerializer(GetType()).WriteObject(outputStream, this);
		}

		public static Filter Load(string fileName)
		{
			using (FileStream stream = File.OpenRead(fileName))
			{
				return Load(stream);
			}
		}

		public static Filter Load(Stream stream)
		{
			return (Filter)new DataContractSerializer(typeof(Filter)).ReadObject(stream);
		}
	}
}

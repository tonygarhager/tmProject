using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory.EditScripts
{
	[DataContract]
	public class EditScript
	{
		[DataMember]
		public List<EditAction> Actions
		{
			get;
			set;
		}

		[DataMember]
		public Continuation Continuation
		{
			get;
			set;
		}

		[DataMember]
		public FilterExpression Filter
		{
			get;
			set;
		}

		public void Add(EditAction action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (Actions == null)
			{
				Actions = new List<EditAction>();
			}
			Actions.Add(action);
		}

		public void Clear()
		{
			Actions?.Clear();
		}

		public bool Validate(IFieldDefinitions fields, bool throwIfInvalid)
		{
			if (Filter != null && !Filter.Validate(fields, throwIfInvalid))
			{
				return false;
			}
			if (Actions == null)
			{
				return true;
			}
			foreach (EditAction action in Actions)
			{
				if (!action.Validate(fields, throwIfInvalid))
				{
					return false;
				}
			}
			return true;
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

		public static EditScript Load(string fileName)
		{
			using (FileStream stream = File.OpenRead(fileName))
			{
				return Load(stream);
			}
		}

		public static EditScript Load(Stream stream)
		{
			return (EditScript)new DataContractSerializer(typeof(EditScript)).ReadObject(stream);
		}
	}
}

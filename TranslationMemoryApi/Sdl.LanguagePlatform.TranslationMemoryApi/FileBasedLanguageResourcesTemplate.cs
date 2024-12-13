using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// A language resources template implementation (<see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ILanguageResourcesTemplate" />) that can be loaded from and saved to a file.
	/// </summary>
	public class FileBasedLanguageResourcesTemplate : ILanguageResourcesTemplate
	{
		private const string ElementLanguageResourceGroup = "LanguageResourceGroup";

		private const string ElementLanguageResource = "LanguageResource";

		private const string AttributeUniqueId = "UniqueId";

		private const string AttributeName = "Name";

		private const string AttributeDescription = "Description";

		private const string AttributeType = "Type";

		private const string AttributeLcid = "Lcid";

		private const string AttributeCultureName = "CultureName";

		private const string AttributeRecognizers = "Recognizers";

		private const string AttributeWordCountFlags = "WordCountFlags";

		private const BuiltinRecognizers DefaultRecognizers = BuiltinRecognizers.RecognizeAll;

		private const WordCountFlags DefaultWordCountFlags = Sdl.LanguagePlatform.TranslationMemory.WordCountFlags.BreakOnTag;

		private const TokenizerFlags DefaultTokenizerFlags = Sdl.LanguagePlatform.Core.Tokenization.TokenizerFlags.DefaultFlags;

		private bool _isLoaded;

		private Guid _id;

		private string _filePath;

		private string _name;

		private string _description;

		private LanguageResourceBundleCollection _languageResourceBundles;

		/// <summary>
		/// Gets or sets the recognizers which are enabled for this template. 
		/// </summary>
		public BuiltinRecognizers? Recognizers
		{
			get;
			set;
		} = BuiltinRecognizers.RecognizeAll;


		/// <summary>
		/// Gets or sets the flags affecting word count behaviour for this language resource template.
		/// </summary>
		public WordCountFlags? WordCountFlags
		{
			get;
			set;
		} = Sdl.LanguagePlatform.TranslationMemory.WordCountFlags.BreakOnTag;


		/// <summary>
		/// Gets or sets the flags affecting tokenizer behaviour for this language resource template.
		/// </summary>
		public TokenizerFlags? TokenizerFlags
		{
			get;
			set;
		} = Sdl.LanguagePlatform.Core.Tokenization.TokenizerFlags.DefaultFlags;


		/// <summary>
		/// Gets the location where this language resources template is located.
		/// </summary>
		public string FilePath => _filePath;

		/// <summary>
		/// Gets the collection of language resource bundles in this template.
		/// </summary>
		public LanguageResourceBundleCollection LanguageResourceBundles
		{
			get
			{
				EnsureLoaded();
				return _languageResourceBundles;
			}
		}

		/// <summary>
		/// gets the unique id of this template.
		/// </summary>
		public Guid Id
		{
			get
			{
				EnsureLoaded();
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of this template. Not that this is not necessarily the same as the file name.
		/// </summary>
		public string Name
		{
			get
			{
				EnsureLoaded();
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Gets or sets the description of this template.
		/// </summary>
		public string Description
		{
			get
			{
				EnsureLoaded();
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		/// <summary>
		/// Creates a new file-based language resources template.
		/// </summary>
		public FileBasedLanguageResourcesTemplate()
			: this(Guid.NewGuid())
		{
		}

		/// <summary>
		/// Creates a new file-based language resources template with the provided ID. This is useful during copying.
		/// </summary>
		public FileBasedLanguageResourcesTemplate(Guid id)
		{
			_languageResourceBundles = new LanguageResourceBundleCollection(new EntityCollection<LanguageResourceEntity>());
			_id = id;
		}

		/// <summary>
		/// Loads a file-based language resources template from the specified file.
		/// </summary>
		/// <param name="filePath">The location where the template should be loaded from.</param>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="filePath" /> is null or empty.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">Thrown if <paramref name="filePath" /> does not exist.</exception>
		public FileBasedLanguageResourcesTemplate(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentNullException("filePath");
			}
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException(Path.GetFullPath(filePath));
			}
			_languageResourceBundles = new LanguageResourceBundleCollection(new EntityCollection<LanguageResourceEntity>());
			_filePath = filePath;
			_id = Guid.NewGuid();
		}

		/// <summary>
		/// Reloads the template from the file.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">Thrown when trying to call this method on an unsaved template.</exception>
		public void Refresh()
		{
			if (string.IsNullOrEmpty(FilePath) || !File.Exists(FilePath))
			{
				throw new InvalidOperationException("This template has not been saved yet.");
			}
			Load();
		}

		/// <summary>
		/// Saves the template to the location specified in <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedLanguageResourcesTemplate.FilePath" />. If this is the first time 
		/// the template is saved, use <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedLanguageResourcesTemplate.SaveAs(System.String)" /> instead.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">Thrown if this template has not been saved before. Use SaveAs and specify a file path instead.</exception>
		public void Save()
		{
			if (string.IsNullOrEmpty(FilePath))
			{
				throw new InvalidOperationException("This template has not been saved before. Use SaveAs and specify a file path instead.");
			}
			using (FileStream stream = File.Create(FilePath))
			{
				Save(stream);
			}
		}

		/// <summary>
		/// Saves the template to the specified location. This also updates the current <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedLanguageResourcesTemplate.FilePath" />.
		/// Subsequent calls to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedLanguageResourcesTemplate.Save" /> will save the template to this new location.
		/// </summary>
		/// <param name="filePath">The location to save the template to.</param>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="filePath" /> is null or empty.</exception>
		public void SaveAs(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentNullException("filePath");
			}
			_filePath = filePath;
			Save();
		}

		private void ReadXml(XmlReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			EntityCollection<LanguageResourceEntity> entityCollection = new EntityCollection<LanguageResourceEntity>();
			reader.ReadToFollowing("LanguageResourceGroup");
			if (reader.NodeType == XmlNodeType.None)
			{
				throw new InvalidOperationException("This file is in an incorrect format. 'LanguageResourceGroup' root element could not be found");
			}
			if (!string.IsNullOrEmpty(reader.GetAttribute("UniqueId")))
			{
				_id = new Guid(reader.GetAttribute("UniqueId"));
			}
			Name = reader.GetAttribute("Name");
			Description = reader.GetAttribute("Description");
			string attribute = reader.GetAttribute("Recognizers");
			if (Enum.TryParse(attribute, out BuiltinRecognizers result))
			{
				Recognizers = result;
			}
			string attribute2 = reader.GetAttribute("Recognizers");
			if (Enum.TryParse(attribute2, out WordCountFlags result2))
			{
				WordCountFlags = result2;
			}
			while (reader.NodeType != XmlNodeType.EndElement || !(reader.Name == "LanguageResourceGroup"))
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "LanguageResource")
					{
						entityCollection.Add(ReadLanguageResourceEntity(reader));
					}
					else if (!reader.Read())
					{
						break;
					}
				}
			}
			_languageResourceBundles = new LanguageResourceBundleCollection(entityCollection);
		}

		private void WriteXml(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			EnsureLoaded();
			writer.WriteStartElement("LanguageResourceGroup");
			writer.WriteAttributeString("UniqueId", Id.ToString());
			if (Name != null)
			{
				writer.WriteAttributeString("Name", Name);
			}
			if (Description != null)
			{
				writer.WriteAttributeString("Description", Description);
			}
			if (Recognizers != BuiltinRecognizers.RecognizeAll)
			{
				writer.WriteAttributeString("Recognizers", Recognizers.ToString());
			}
			if (WordCountFlags != Sdl.LanguagePlatform.TranslationMemory.WordCountFlags.BreakOnTag)
			{
				writer.WriteAttributeString("WordCountFlags", WordCountFlags.ToString());
			}
			_languageResourceBundles.SaveToEntities();
			foreach (LanguageResourceEntity entity in _languageResourceBundles.Entities)
			{
				WriteLanguageResourceEntity(writer, entity);
			}
			writer.WriteEndElement();
		}

		private static LanguageResourceEntity ReadLanguageResourceEntity(XmlReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			LanguageResourceEntity languageResourceEntity = new LanguageResourceEntity();
			if (!string.IsNullOrEmpty(reader.GetAttribute("Type")))
			{
				string attribute = reader.GetAttribute("Type");
				languageResourceEntity.Type = (LanguageResourceType)Enum.Parse(typeof(LanguageResourceType), attribute);
			}
			string attribute2 = reader.GetAttribute("CultureName");
			if (!string.IsNullOrEmpty(attribute2))
			{
				languageResourceEntity.CultureName = attribute2;
			}
			else if (!string.IsNullOrEmpty(reader.GetAttribute("Lcid")))
			{
				languageResourceEntity.CultureName = new CultureInfo(Convert.ToInt32(reader.GetAttribute("Lcid"), CultureInfo.InvariantCulture)).Name;
			}
			languageResourceEntity.Data = Convert.FromBase64String(reader.ReadElementString());
			return languageResourceEntity;
		}

		private static void WriteLanguageResourceEntity(XmlWriter writer, LanguageResourceEntity languageResourceDefinition)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteStartElement("LanguageResource");
			writer.WriteAttributeString("Type", languageResourceDefinition.Type.ToString());
			writer.WriteAttributeString("CultureName", languageResourceDefinition.CultureName);
			writer.WriteString(Convert.ToBase64String(languageResourceDefinition.Data));
			writer.WriteEndElement();
		}

		/// <summary>
		/// Saves this template to the specified stream.
		/// </summary>
		/// <param name="stream">The stream to which the template should be saved.</param>
		public void Save(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanWrite)
			{
				throw new ArgumentException("The specified stream is not writeable.");
			}
			EnsureLoaded();
			using (XmlWriter xmlWriter = XmlWriter.Create(stream))
			{
				xmlWriter.WriteStartDocument();
				WriteXml(xmlWriter);
				xmlWriter.WriteEndDocument();
			}
		}

		private void EnsureLoaded()
		{
			if (!_isLoaded)
			{
				Load();
				_isLoaded = true;
			}
		}

		private void Load()
		{
			if (!string.IsNullOrEmpty(FilePath))
			{
				using (Stream input = File.OpenRead(_filePath))
				{
					using (XmlReader reader = XmlReader.Create(input))
					{
						ReadXml(reader);
					}
				}
			}
		}
	}
}

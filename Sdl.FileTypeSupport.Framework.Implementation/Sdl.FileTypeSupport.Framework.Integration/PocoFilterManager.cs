using Sdl.Core.PluginFramework;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class PocoFilterManager : FilterManager
	{
		protected class PocoFileTypeDefinitionComparer : IComparer<IFileTypeDefinition>
		{
			public int Compare(IFileTypeDefinition firstFileTypeDefinition, IFileTypeDefinition secondFileTypeDefinition)
			{
				string id = firstFileTypeDefinition.FileTypeInformation.FileTypeDefinitionId.Id;
				string id2 = secondFileTypeDefinition.FileTypeInformation.FileTypeDefinitionId.Id;
				int num = _orderedFileTypeDefinitionIds.IndexOf(id);
				int num2 = _orderedFileTypeDefinitionIds.IndexOf(id2);
				if (num == num2)
				{
					return 0;
				}
				if (num <= num2)
				{
					return -1;
				}
				return 1;
			}
		}

		private const string WordprocessingMLv2 = "WordprocessingML v. 2";

		private const string PDFv3 = "PDF v 3.0.0.0";

		private const string Xliff2 = "XLIFF 2.0 v 1.0.0.0";

		private const string Json = "JSON v 1.0.0.0";

		private const string SpreadsheetML = "SpreadsheetML v. 1";

		private const string PresentationML = "PresentationML v. 1";

		private const string Rtfv2 = "RTF v 2.0.0.0";

		private const string Docv2 = "DOC v 2.0.0.0";

		private const string Xlsv3 = "XLS v 3.0.0.0";

		private const string Pptv2 = "PPT v 2.0.0.0";

		private const string Visio = "Visio v 1.0.0.0";

		private const string Photoshop = "Photoshop v 1.0.0.0";

		private const string Markdown = "Markdown v 1.0.0.0";

		private const string Xml2Xhtml = "XHTML 1.1 v 2.0.0.0";

		private const string Xml2Resx = "XML: RESX v 2.0.0.0";

		private const string Xml2Dita = "XML: DITA 1.3 v 2.0.0.0";

		private const string Xml2DocBook = "XML: DocBook 4.5 v 2.0.0.0";

		private const string Xml2Authorit = "XML: Author-IT 1.2 v 2.0.0.0";

		private const string Xml2MadCap = "XML: MadCap 1.2 v 2.0.0.0";

		private const string Xml2Its = "XML: ITS 1.0 v 2.0.0.0";

		private const string Xml2Any = "XML: Any v 2.0.0.0";

		private const string Subtitles = "Subtitles v 1.0.0.0";

		private const string Yaml = "YAML v 1.0.0.0";

		private const string Email = "EMAIL v 1.0.0.0";

		protected static List<string> _orderedFileTypeDefinitionIds = new List<string>
		{
			"SDL XLIFF 1.0 v 1.0.0.0",
			"TTX 2.0 v 2.0.0.0",
			"ITD v 1.0.0.0",
			"WordprocessingML v. 2",
			"DOC v 2.0.0.0",
			"PresentationML v. 1",
			"PPT v 2.0.0.0",
			"SpreadsheetML v. 1",
			"XLS v 3.0.0.0",
			"Bilingual Excel v 1.0.0.0",
			"EMAIL v 1.0.0.0",
			"Bilingual Workbench 1.0.0.0",
			"RTF v 2.0.0.0",
			"Visio v 1.0.0.0",
			"XHTML 1.1 v 2.0.0.0",
			"XHTML 1.1 v 1.2.0.0",
			"Html 5 2.0.0.0",
			"Html 4 2.0.0.0",
			"FrameMaker v 10.0.0",
			"FrameMaker 8.0 v 2.0.0.0",
			"Inx 1.0.0.0",
			"IDML v 1.0.0.0",
			"ICML Filter 1.0.0.0",
			"Photoshop v 1.0.0.0",
			"Odt 1.0.0.0",
			"Odp 1.0.0.0",
			"Ods 1.0.0.0",
			"QuarkXPress v 2.0.0.0",
			"XLIFF 1.1-1.2 v 2.0.0.0",
			"MemoQ v 1.0.0.0",
			"XLIFF 2.0 v 1.0.0.0",
			"WsXliff 1.0.0.0",
			"PDF v 3.0.0.0",
			"CSV v 2.0.0.0",
			"Tab Delimited v 2.0.0.0",
			"Java Resources v 2.0.0.0",
			"PO files v 1.0.0.0",
			"Subtitles v 1.0.0.0",
			"SubRip v 1.0.0.0",
			"JSON v 1.0.0.0",
			"YAML v 1.0.0.0",
			"Markdown v 1.0.0.0",
			"XML: RESX v 2.0.0.0",
			"XML: RESX v 1.2.0.0",
			"XML: DITA 1.3 v 2.0.0.0",
			"XML: DITA 1.2 v 1.2.0.0",
			"XML: DocBook 4.5 v 2.0.0.0",
			"DocBook 4.5 v 1.2.0.0",
			"XML: Author-IT 1.2 v 2.0.0.0",
			"XML: Author-it 1.2 v 1.0.0.0",
			"XML: MadCap 1.2 v 2.0.0.0",
			"XML: MadCap 1.2 v 1.0.0.0",
			"XML: ITS 1.0 v 2.0.0.0",
			"XML: ITS 1.0 v 1.2.0.0",
			"XML: Any v 2.0.0.0",
			"XML: Any v 1.2.0.0",
			"Plain Text v 1.0.0.0"
		};

		protected static List<string> _autoLoadedFileTypeDefinitionIds = new List<string>
		{
			"WordprocessingML v. 2",
			"PDF v 3.0.0.0",
			"SpreadsheetML v. 1",
			"PresentationML v. 1",
			"JSON v 1.0.0.0",
			"Markdown v 1.0.0.0",
			"RTF v 2.0.0.0",
			"DOC v 2.0.0.0",
			"XLS v 3.0.0.0",
			"PPT v 2.0.0.0",
			"Visio v 1.0.0.0",
			"Photoshop v 1.0.0.0",
			"XHTML 1.1 v 2.0.0.0",
			"XML: RESX v 2.0.0.0",
			"XML: DITA 1.3 v 2.0.0.0",
			"XML: DocBook 4.5 v 2.0.0.0",
			"XML: Author-IT 1.2 v 2.0.0.0",
			"XML: MadCap 1.2 v 2.0.0.0",
			"XML: ITS 1.0 v 2.0.0.0",
			"XML: Any v 2.0.0.0"
		};

		private readonly Dictionary<string, IFileTypeComponentBuilder> _standardFileTypeDefinitionCache = new Dictionary<string, IFileTypeComponentBuilder>();

		private readonly Dictionary<string, IFileTypeComponentBuilder> _templateFileTypeDefinitionCache = new Dictionary<string, IFileTypeComponentBuilder>();

		public override List<string> AutoLoadedFileTypes => _autoLoadedFileTypeDefinitionIds;

		public bool FileTypeAutoLoad
		{
			get;
			private set;
		}

		public PocoFilterManager()
			: this(autoLoadFileTypes: false)
		{
		}

		public PocoFilterManager(bool autoLoadFileTypes)
		{
			SetFileTypeDefinitionFactory(new PocoFileTypeDefinitionFactory());
			FileTypeAutoLoad = autoLoadFileTypes;
			if (autoLoadFileTypes)
			{
				LoadAllDefaultFileTypeDefinitions();
			}
		}

		public void AddFileTypeDefinition(IFileTypeComponentBuilder componentBuilder)
		{
			componentBuilder.FileTypeManager = this;
			FilterDefinition filterDefinition = new FilterDefinition();
			filterDefinition.ComponentBuilder = componentBuilder;
			AddFileTypeDefinition(filterDefinition);
		}

		public override IFileTypeDefinition CreateFileTypeDefinition(IFileTypeComponentBuilder componentBuilder, FileTypeProfile profileOverride)
		{
			if (componentBuilder == null)
			{
				throw new ArgumentNullException("componentBuilder is null");
			}
			componentBuilder.FileTypeManager = this;
			FilterDefinition filterDefinition = new FilterDefinition();
			filterDefinition.ComponentBuilder = componentBuilder;
			if (profileOverride != null)
			{
				FileTypeProfile.ApplyProfile(filterDefinition, profileOverride);
			}
			return filterDefinition;
		}

		public override IFileTypeDefinition CreateFileTypeDefinition(string fileTypeDefinitionId)
		{
			return CreateFileTypeDefinition(fileTypeDefinitionId, null);
		}

		public override IFileTypeDefinition CreateFileTypeDefinition(string fileTypeDefinitionId, FileTypeProfile profileOverride)
		{
			if (!_standardFileTypeDefinitionCache.Any() && !_templateFileTypeDefinitionCache.Any())
			{
				LoadFileTypeCache();
			}
			if (_standardFileTypeDefinitionCache.TryGetValue(fileTypeDefinitionId, out IFileTypeComponentBuilder value))
			{
				return CreateFileTypeDefinition(value, profileOverride);
			}
			if (_templateFileTypeDefinitionCache.TryGetValue(fileTypeDefinitionId, out IFileTypeComponentBuilder value2))
			{
				return CreateFileTypeDefinition(value2, profileOverride);
			}
			return null;
		}

		private void LoadFileTypeCache()
		{
			_standardFileTypeDefinitionCache.Clear();
			_templateFileTypeDefinitionCache.Clear();
			IExtensionPoint extensionPoint = PluginManager.DefaultPluginRegistry.GetExtensionPoint<FileTypeComponentBuilderAttribute>();
			IExtensionPoint extensionPoint2 = PluginManager.DefaultPluginRegistry.GetExtensionPoint<FileTypeComponentBuilderExtensionAttribute>();
			foreach (IExtension extension2 in extensionPoint.Extensions)
			{
				FileTypeComponentBuilderAttribute fileTypeComponentBuilderAttribute = extension2.ExtensionAttribute as FileTypeComponentBuilderAttribute;
				if (fileTypeComponentBuilderAttribute != null)
				{
					IFileTypeComponentBuilder fileTypeComponentBuilder = (IFileTypeComponentBuilder)extension2.CreateInstance();
					fileTypeComponentBuilder.FileTypeManager = this;
					IFileTypeInformation fileTypeInformation = fileTypeComponentBuilder.BuildFileTypeInformation(string.Empty);
					string fileTypeDefinitionId = fileTypeInformation.FileTypeDefinitionId.Id;
					IExtension extension = extensionPoint2.Extensions.FirstOrDefault((IExtension p) => (p.ExtensionAttribute as FileTypeComponentBuilderExtensionAttribute).OriginalFileType == fileTypeDefinitionId);
					if (extension != null)
					{
						IFileTypeComponentBuilderAdapter fileTypeComponentBuilderAdapter = (IFileTypeComponentBuilderAdapter)extension.CreateInstance();
						if (fileTypeComponentBuilderAdapter != null)
						{
							fileTypeComponentBuilderAdapter.FileTypeManager = this;
							fileTypeComponentBuilderAdapter.Original = fileTypeComponentBuilder;
							fileTypeComponentBuilder = fileTypeComponentBuilderAdapter;
						}
					}
					if (fileTypeComponentBuilderAttribute.IsTemplate)
					{
						_templateFileTypeDefinitionCache.Add(fileTypeDefinitionId, fileTypeComponentBuilder);
					}
					else
					{
						_standardFileTypeDefinitionCache.Add(fileTypeDefinitionId, fileTypeComponentBuilder);
					}
				}
			}
		}

		protected void LoadAllDefaultFileTypeDefinitions()
		{
			List<IFileTypeDefinition> list = new List<IFileTypeDefinition>();
			List<IFileTypeDefinition> list2 = new List<IFileTypeDefinition>();
			List<IFileTypeDefinition> list3 = new List<IFileTypeDefinition>();
			LoadFileTypeCache();
			foreach (IFileTypeComponentBuilder value in _standardFileTypeDefinitionCache.Values)
			{
				IFileTypeDefinition fileTypeDefinition = CreateFileTypeDefinition(value, null);
				string id = fileTypeDefinition.FileTypeInformation.FileTypeDefinitionId.Id;
				if (_orderedFileTypeDefinitionIds.Contains(id))
				{
					list.Add(fileTypeDefinition);
				}
				else if (value is IFileTypeComponentBuilderAdapter)
				{
					list3.Add(fileTypeDefinition);
				}
				else
				{
					list2.Add(fileTypeDefinition);
				}
			}
			foreach (IFileTypeComponentBuilder value2 in _templateFileTypeDefinitionCache.Values)
			{
				if (value2 is IFileTypeComponentBuilderAdapter)
				{
					IFileTypeDefinition item = CreateFileTypeDefinition(value2, null);
					list3.Add(item);
				}
			}
			list.Sort(new PocoFileTypeDefinitionComparer());
			for (int i = 0; i < list.Count; i++)
			{
				AddFileTypeDefinition(list[i]);
			}
			foreach (IFileTypeDefinition item2 in list2)
			{
				InsertFileTypeDefinition(1, item2);
			}
			foreach (IFileTypeDefinition item3 in list3)
			{
				InsertFileTypeDefinition(1, item3);
			}
		}
	}
}

using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmConverters.ToBilingualApi.Extensions;
using Sdl.Core.Bcm.BcmConverters.ToBilingualApi.MarkupDataConverters;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.Core.Bcm.BcmConverters.ToBilingualApi
{
	internal static class BcmReaderUtil
	{
		internal static void SetSnifferInfo(File currentFile, IFileProperties properties)
		{
			string metadata = currentFile.GetMetadata("frameworkOriginalSniffInfo");
			if (metadata != null)
			{
				currentFile.RemoveMetadata("frameworkOriginalSniffInfo");
				string value = JsonConvert.DeserializeObject<string>(metadata);
				Sdl.Core.Bcm.BcmConverters.Common.SniffInfo sniffInfo = JsonConvert.DeserializeObject<Sdl.Core.Bcm.BcmConverters.Common.SniffInfo>(value);
				Sdl.FileTypeSupport.Framework.NativeApi.SniffInfo fileSnifferInfo = sniffInfo.Convert();
				properties.FileConversionProperties.FileSnifferInfo = fileSnifferInfo;
			}
		}

		internal static void AddFileLevelComments(File file, IFileProperties properties)
		{
			if (file.CommentDefinitionIds != null)
			{
				FileSkeleton skeleton = file.Skeleton;
				PropertiesFactory propertiesFactory = new PropertiesFactory();
				properties.Comments = propertiesFactory.CreateCommentProperties();
				foreach (int commentDefinitionId in file.CommentDefinitionIds)
				{
					CommentDefinition commentDefinition = skeleton.CommentDefinitions.First((CommentDefinition x) => x.Id == commentDefinitionId);
					IComment comment = propertiesFactory.CreateComment(commentDefinition.Text, commentDefinition.Author, (Severity)commentDefinition.CommentSeverity);
					comment.Date = commentDefinition.Date;
					properties.Comments.Add(comment);
				}
			}
		}

		internal static FileProperties CreateFileProperties(File currentFile, IDocumentProperties docProperties)
		{
			return new FileProperties
			{
				FileConversionProperties = new PersistentFileConversionProperties
				{
					FileTypeDefinitionId = new FileTypeDefinitionId(currentFile.FileTypeDefinitionId),
					OriginalEncoding = new Codepage(currentFile.OriginalEncoding),
					SourceLanguage = docProperties?.SourceLanguage,
					TargetLanguage = docProperties?.TargetLanguage
				}
			};
		}

		internal static void SetProperties(File currentFile, IBilingualContentHandler output, IDocumentProperties documentProperties, IFileProperties properties, string targetEncoding = null)
		{
			SetSnifferInfo(currentFile, properties);
			foreach (KeyValuePair<string, string> metadatum in currentFile.Metadata)
			{
				properties.FileConversionProperties.SetMetaData(metadatum.Key, metadatum.Value);
			}
			SetQuickInsertIds(properties, currentFile);
			if (output != null)
			{
				DependencyFileConverter dependencyFileConverter = new DependencyFileConverter();
				foreach (DependencyFile item in currentFile.DependencyFiles.Where((DependencyFile dependencyFile) => dependencyFile.Id != "fullContextsTable"))
				{
					properties.FileConversionProperties.DependencyFiles.Add(dependencyFileConverter.Convert(item));
				}
				AddFileLevelComments(currentFile, properties);
				string text = targetEncoding ?? currentFile.PreferredTargetEncoding;
				if (!string.IsNullOrEmpty(text))
				{
					properties.FileConversionProperties.PreferredTargetEncoding = new Codepage(Encoding.GetEncoding(text));
				}
				output.SetFileProperties(properties);
			}
		}

		private static void SetQuickInsertIds(IFileProperties properties, File currentFile)
		{
			if (currentFile?.Skeleton.QuickInsertIds != null && currentFile.Skeleton.QuickInsertIds.Count != 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string quickInsertId in currentFile.Skeleton.QuickInsertIds)
				{
					stringBuilder.Append(quickInsertId + ";");
				}
				string value = stringBuilder.ToString().TrimEnd(';');
				properties.FileConversionProperties.SetMetaData("SDL:QuickInsertsList", value);
			}
		}
	}
}

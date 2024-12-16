using Sdl.Core.Bcm.BcmConverters.FromBilingualApi.Extensions;
using Sdl.Core.Bcm.BcmConverters.FromBilingualApi.Helpers;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Integration.QuickInserts;
using Sdl.FileTypeSupport.Framework.IntegrationApi.QuickInserts;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters.FromBilingualApi
{
	internal class FilePropertiesBuilder
	{
		private readonly IFileProperties _fileInfo;

		private readonly FileSkeleton _currentFileSkeleton;

		private readonly ContextHierarchy _contextHierarchy;

		private const string ProposedOutputFileNameMetadataKey = "SDL:ProposedOutputFileName";

		internal Action<DependencyFile> DependencyFileHandler
		{
			get;
			set;
		}

		internal EventHandler DependencyFilesAdded
		{
			get;
			set;
		}

		public FilePropertiesBuilder(IFileProperties fileInfo, FileSkeleton currentFileSkeleton, ContextHierarchy contextHierarchy)
		{
			_fileInfo = fileInfo;
			_currentFileSkeleton = currentFileSkeleton;
			_contextHierarchy = contextHierarchy;
		}

		internal void AddFileProperties(Sdl.Core.Bcm.BcmModel.File currentFile)
		{
			string metaData = _fileInfo.FileConversionProperties.GetMetaData("SDL:ProposedOutputFileName");
			currentFile.OriginalFileName = (string.IsNullOrEmpty(metaData) ? Path.GetFileName(_fileInfo.FileConversionProperties.OriginalFilePath) : Path.GetFileName(metaData));
			currentFile.FileTypeDefinitionId = _fileInfo.FileConversionProperties.FileTypeDefinitionId.Id;
			AddEncoding(currentFile);
			AddQuickInsertIds(currentFile);
			currentFile.CopyMetadataFrom(_fileInfo.FileConversionProperties);
			AddFileLevelComments(currentFile, _fileInfo.Comments);
			AddDependencyFiles(currentFile);
		}

		private void AddQuickInsertIds(Sdl.Core.Bcm.BcmModel.File currentFile)
		{
			List<string> list = new List<string>();
			string metaData = _fileInfo.FileConversionProperties.GetMetaData("SDL:QuickInsertsList");
			if (metaData != null)
			{
				QuickInsertDefinitionsManager quickInsertDefinitionsManager = new QuickInsertDefinitionsManager();
				List<QuickInsertIds> list2 = quickInsertDefinitionsManager.ParseQuickInsertIds(metaData);
				foreach (QuickInsertIds item in list2)
				{
					list.Add(item.ToString());
				}
				_fileInfo.FileConversionProperties.RemoveMetaData("SDL:QuickInsertsList");
			}
			currentFile.Skeleton.QuickInsertIds = list;
		}

		private void AddEncoding(Sdl.Core.Bcm.BcmModel.File currentFile)
		{
			if (_fileInfo.FileConversionProperties.OriginalEncoding.IsValid)
			{
				currentFile.OriginalEncoding = _fileInfo.FileConversionProperties.OriginalEncoding.Name;
			}
			if (_fileInfo.FileConversionProperties.PreferredTargetEncoding.IsValid)
			{
				currentFile.PreferredTargetEncoding = _fileInfo.FileConversionProperties.PreferredTargetEncoding.Name;
			}
		}

		private void AddFileLevelComments(Sdl.Core.Bcm.BcmModel.File currentFile, ICommentProperties comments)
		{
			if (comments?.Comments != null)
			{
				if (currentFile.CommentDefinitionIds == null)
				{
					currentFile.CommentDefinitionIds = new List<int>();
				}
				foreach (CommentDefinition item in comments.Comments.Select(MarkupDataConverter.Convert))
				{
					int id = _currentFileSkeleton.CommentDefinitions.GetOrAdd(item).Id;
					currentFile.CommentDefinitionIds.Add(id);
				}
			}
		}

		private void AddDependencyFiles(Sdl.Core.Bcm.BcmModel.File currentFile)
		{
			currentFile.DependencyFiles = new List<DependencyFile>();
			AddFullContextsDependencyFile(currentFile);
			foreach (IDependencyFileProperties dependencyFile2 in _fileInfo.FileConversionProperties.DependencyFiles)
			{
				DependencyFile dependencyFile = new DependencyFile
				{
					Id = dependencyFile2.Id,
					Usage = Convert(dependencyFile2.ExpectedUsage),
					Location = dependencyFile2.OriginalFilePath,
					FileName = ((!string.IsNullOrEmpty(dependencyFile2.OriginalFilePath) && System.IO.File.Exists(dependencyFile2.OriginalFilePath)) ? dependencyFile2.OriginalFilePath : dependencyFile2.CurrentFilePath)
				};
				DependencyFileHandler(dependencyFile);
				currentFile.DependencyFiles.Add(dependencyFile);
			}
			OnDependencyFilesAdded();
		}

		private void AddFullContextsDependencyFile(Sdl.Core.Bcm.BcmModel.File currentFile)
		{
			if (_contextHierarchy.HasFullContexts)
			{
				string fileName = _contextHierarchy.SerializeFullContextsToFile();
				string metaData = _fileInfo.FileConversionProperties.GetMetaData("SDL:ContextFileLocation");
				DependencyFile dependencyFile = new DependencyFile
				{
					Id = "fullContextsTable",
					Usage = Sdl.Core.Bcm.BcmModel.DependencyFileUsage.Generation,
					Location = metaData,
					FileName = fileName
				};
				DependencyFileHandler(dependencyFile);
				currentFile.DependencyFiles.Add(dependencyFile);
			}
		}

		private void OnDependencyFilesAdded()
		{
			DependencyFilesAdded?.Invoke(this, EventArgs.Empty);
		}

		private static Sdl.Core.Bcm.BcmModel.DependencyFileUsage Convert(Sdl.FileTypeSupport.Framework.NativeApi.DependencyFileUsage dependencyFileUsage)
		{
			switch (dependencyFileUsage)
			{
			case Sdl.FileTypeSupport.Framework.NativeApi.DependencyFileUsage.None:
				return Sdl.Core.Bcm.BcmModel.DependencyFileUsage.None;
			case Sdl.FileTypeSupport.Framework.NativeApi.DependencyFileUsage.Extraction:
				return Sdl.Core.Bcm.BcmModel.DependencyFileUsage.Extraction;
			case Sdl.FileTypeSupport.Framework.NativeApi.DependencyFileUsage.Generation:
				return Sdl.Core.Bcm.BcmModel.DependencyFileUsage.Generation;
			case Sdl.FileTypeSupport.Framework.NativeApi.DependencyFileUsage.Final:
				return Sdl.Core.Bcm.BcmModel.DependencyFileUsage.Final;
			default:
				return Sdl.Core.Bcm.BcmModel.DependencyFileUsage.None;
			}
		}
	}
}

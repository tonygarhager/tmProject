//using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Client;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;
using System.IO;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public abstract class ImportExportOperationBase : ScheduledOperation
	{
		protected const string STREAM_HANDLER_TYPE = "Sdl.LanguagePlatform.ServerBasedTranslationMemory.Services.StreamHandler, Sdl.LanguagePlatform.ServerBasedTranslationMemory.Services";

		public const int DefaultTranslationUnitChunkSize = 25;

		private ServerBasedTranslationMemoryLanguageDirection _LanguageDirection;

		private TranslationProviderServer _Server;

		private ImportExportEntity _Entity;

		internal ImportExportEntity Entity
		{
			get
			{
				return _Entity;
			}
			set
			{
				_Entity = value;
			}
		}

		internal TranslationMemoryAdministrationClient Service => _Server.Service;

		public ServerBasedTranslationMemoryLanguageDirection LanguageDirection
		{
			get
			{
				return _LanguageDirection;
			}
			internal set
			{
				_LanguageDirection = value;
			}
		}

		public Guid Id => _Entity.UniqueId.Value;

		public int ChunkSize
		{
			get
			{
				return _Entity.ChunkSize.Value;
			}
			set
			{
				_Entity.ChunkSize = value;
			}
		}

		public bool ContinueOnError
		{
			get
			{
				return _Entity.ContinueOnError.Value;
			}
			set
			{
				_Entity.ContinueOnError = value;
			}
		}

		protected bool IsScheduled => base.ScheduledOperationEntity != null;

		internal ImportExportOperationBase()
		{
		}

		internal ImportExportOperationBase(TranslationProviderServer server, ImportExportEntity entity)
			: base(entity.ScheduledOperation)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			_Server = server;
			_Entity = entity;
		}

		internal ImportExportOperationBase(ServerBasedTranslationMemoryLanguageDirection languageDirection)
		{
			if (languageDirection == null)
			{
				throw new ArgumentNullException("languageDirection");
			}
			_LanguageDirection = languageDirection;
			_Server = languageDirection.TranslationProviderServer;
		}

		internal static bool CopyStream(Stream from, Stream to, Func<long, bool> totalBytesCopiedHandler)
		{
			byte[] buffer = new byte[32768];
			int num = 0;
			int num2 = 0;
			while ((num = from.Read(buffer, 0, 32768)) > 0)
			{
				to.Write(buffer, 0, num);
				num2 += num;
				if (totalBytesCopiedHandler != null && !totalBytesCopiedHandler(num2))
				{
					return false;
				}
			}
			return true;
		}
	}
}

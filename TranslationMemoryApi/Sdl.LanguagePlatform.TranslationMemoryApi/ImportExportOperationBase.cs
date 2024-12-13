using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Client;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;
using System.IO;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Import Export scheduled operation base class
	/// </summary>
	public abstract class ImportExportOperationBase : ScheduledOperation
	{
		/// <summary>
		/// Constant for stream handler type
		/// </summary>
		protected const string STREAM_HANDLER_TYPE = "Sdl.LanguagePlatform.ServerBasedTranslationMemory.Services.StreamHandler, Sdl.LanguagePlatform.ServerBasedTranslationMemory.Services";

		/// <summary>
		/// Default TranslationUnit ChunkSize
		/// </summary>
		public const int DefaultTranslationUnitChunkSize = 25;

		private ServerBasedTranslationMemoryLanguageDirection _LanguageDirection;

		private TranslationProviderServer _Server;

		private ImportExportEntity _Entity;

		/// <summary>
		/// Gets or sets the entity.
		/// </summary>
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

		/// <summary>
		/// Gets the service.
		/// </summary>
		internal TranslationMemoryAdministrationClient Service => _Server.Service;

		/// <summary>
		/// Gets the language direction.
		/// </summary>
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

		/// <summary>
		/// Gets the unique Id for the import.
		/// </summary>
		/// <value>The id.</value>
		public Guid Id => _Entity.UniqueId.Value;

		/// <summary>
		/// Gets or sets the chunk size (number of translation units) for the import.
		/// </summary>
		/// <value>The size of the chunk.</value>
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

		/// <summary>
		/// Gets or sets a value indicating whether to continue on error.
		/// </summary>
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

		/// <summary>
		/// Gets a value indicating whether this instance is scheduled.
		/// </summary>
		protected bool IsScheduled => base.ScheduledOperationEntity != null;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ImportExportOperationBase" /> class.
		/// </summary>
		internal ImportExportOperationBase()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ImportExportOperationBase" /> class.
		/// </summary>
		/// <param name="server">The server.</param>
		/// <param name="entity">The entity.</param>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ImportExportOperationBase" /> class.
		/// </summary>
		/// <param name="languageDirection">The language direction.</param>
		internal ImportExportOperationBase(ServerBasedTranslationMemoryLanguageDirection languageDirection)
		{
			if (languageDirection == null)
			{
				throw new ArgumentNullException("languageDirection");
			}
			_LanguageDirection = languageDirection;
			_Server = languageDirection.TranslationProviderServer;
		}

		/// <summary>
		/// Copies a stream.
		/// </summary>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		/// <param name="totalBytesCopiedHandler">The total bytes copied handler.</param>
		/// <returns></returns>
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

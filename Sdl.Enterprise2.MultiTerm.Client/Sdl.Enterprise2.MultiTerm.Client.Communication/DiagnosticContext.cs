using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Sdl.Enterprise2.MultiTerm.Client.Communication
{
	public class DiagnosticContext : IExtension<OperationContext>
	{
		private readonly Dictionary<string, string> diagnosticData = new Dictionary<string, string>();

		private readonly Guid actionId;

		public static DiagnosticContext Current
		{
			get
			{
				if (OperationContext.Current == null)
				{
					return null;
				}
				return OperationContext.Current.Extensions.Find<DiagnosticContext>();
			}
		}

		public Dictionary<string, string> Data => diagnosticData;

		public Guid ActionId => actionId;

		public DiagnosticContext(Guid id)
		{
			actionId = id;
		}

		public void Attach(OperationContext owner)
		{
		}

		public void Detach(OperationContext owner)
		{
		}
	}
}

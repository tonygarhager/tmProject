using log4net;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Sdl.Enterprise2.MultiTerm.Client.Communication
{
	public sealed class DiagnosticBehavior : Attribute, IServiceBehavior, IDispatchMessageInspector
	{
		private class CorrelationState
		{
			public Stopwatch Sw
			{
				get;
				set;
			}

			public Guid ActionId
			{
				get;
				set;
			}
		}

		private readonly ILog diagnosticLog;

		public DiagnosticBehavior(string loggerToMatch)
		{
			if (string.IsNullOrEmpty(loggerToMatch))
			{
				throw new ArgumentNullException("loggerToMatch");
			}
			diagnosticLog = LogManager.GetLogger(loggerToMatch);
		}

		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
		}

		public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			if (diagnosticLog.IsDebugEnabled)
			{
				foreach (EndpointDispatcher item in serviceHostBase.ChannelDispatchers.OfType<ChannelDispatcher>().SelectMany((ChannelDispatcher channelDispatcher) => channelDispatcher.Endpoints))
				{
					item.DispatchRuntime.MessageInspectors.Add(this);
				}
			}
		}

		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			int num = request.Headers.FindHeader("ActionId", "http://sdl.com/identity/2011");
			DiagnosticContext diagnosticContext = (num >= 0) ? new DiagnosticContext(request.Headers.GetHeader<Guid>(num)) : new DiagnosticContext(Guid.NewGuid());
			OperationContext.Current.Extensions.Add(diagnosticContext);
			CorrelationState correlationState = new CorrelationState
			{
				ActionId = diagnosticContext.ActionId,
				Sw = new Stopwatch()
			};
			correlationState.Sw.Start();
			diagnosticLog.DebugFormat("Request|{0}|{1}|-1|-1", request.Headers.Action, correlationState.ActionId);
			return correlationState;
		}

		public void BeforeSendReply(ref Message reply, object correlationState)
		{
			CorrelationState correlationState2 = correlationState as CorrelationState;
			if (correlationState2 != null)
			{
				correlationState2.Sw.Stop();
				StringBuilder stringBuilder = new StringBuilder();
				if (DiagnosticContext.Current != null)
				{
					foreach (string key in DiagnosticContext.Current.Data.Keys)
					{
						DiagnosticContext.Current.Data.TryGetValue(key, out string value);
						stringBuilder.Append(key);
						stringBuilder.Append("=");
						stringBuilder.Append(value);
						stringBuilder.Append("|");
					}
				}
				diagnosticLog.DebugFormat("Response|{0}|{1}|{2}|{3}|{4}", reply.Headers.Action, correlationState2.ActionId, correlationState2.Sw.ElapsedMilliseconds, reply.IsFault, stringBuilder);
			}
		}
	}
}

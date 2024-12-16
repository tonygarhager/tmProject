#define TRACE
using Sdl.Enterprise2.MultiTerm.Client.Logging;
using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Sdl.Enterprise2.MultiTerm.Client.Communication
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class CompressionEndpointBehavior : Attribute, IEndpointBehavior, IDispatchMessageInspector, IClientMessageInspector
	{
		private readonly WSCompressionMode compressionMode;

		private readonly bool compressMessages;

		public WSCompressionMode CompressionMode => compressionMode;

		public bool CompressMessages => compressMessages;

		public CompressionEndpointBehavior()
		{
			compressMessages = false;
		}

		public CompressionEndpointBehavior(WSCompressionMode compressionMode)
			: this()
		{
			this.compressionMode = compressionMode;
		}

		public CompressionEndpointBehavior(WSCompressionMode compressionMode, bool compressMessages)
		{
			this.compressionMode = compressionMode;
			this.compressMessages = compressMessages;
		}

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			clientRuntime.MessageInspectors.Add(this);
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
		}

		public void Validate(ServiceEndpoint endpoint)
		{
		}

		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			if (request != null)
			{
				try
				{
					EnterpriseTraceSource.TraceSource.TraceEvent(TraceEventType.Start, 0, "Server: decompress request");
					request = WSCompression.Decompress(request);
				}
				finally
				{
					EnterpriseTraceSource.TraceSource.TraceEvent(TraceEventType.Stop, 0, "Server: decompress request");
				}
			}
			return null;
		}

		public void BeforeSendReply(ref Message reply, object correlationState)
		{
			if (reply != null && CompressMessages)
			{
				try
				{
					EnterpriseTraceSource.TraceSource.TraceEvent(TraceEventType.Start, 0, "Server: compress reply");
					reply = WSCompression.Compress(compressionMode, reply, "reply", out bool compressed, out int orginalBodySize, out int compressedBodySize);
					ServiceCallData serviceCallData = new ServiceCallData();
					serviceCallData.Values["ReplyCompressed"] = compressed.ToString();
					serviceCallData.Values["ReplyOriginalSize"] = orginalBodySize.ToString();
					serviceCallData.Values["ReplyCompressedSize"] = compressedBodySize.ToString();
					EnterpriseTraceSource.TraceSource.TraceData(TraceEventType.Information, 0, serviceCallData.ToTraceData());
				}
				finally
				{
					EnterpriseTraceSource.TraceSource.TraceEvent(TraceEventType.Stop, 0, "Server: compress reply");
				}
			}
		}

		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
			if (reply != null)
			{
				try
				{
					EnterpriseTraceSource.TraceSource.TraceEvent(TraceEventType.Start, 0, "Client: decompress reply");
					reply = WSCompression.Decompress(reply);
				}
				finally
				{
					EnterpriseTraceSource.TraceSource.TraceEvent(TraceEventType.Stop, 0, "Client: decompress reply");
				}
			}
		}

		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			if (request != null && CompressMessages)
			{
				try
				{
					EnterpriseTraceSource.TraceSource.TraceEvent(TraceEventType.Start, 0, "Client: compress request");
					request = WSCompression.Compress(compressionMode, request, "request", out bool compressed, out int orginalBodySize, out int compressedBodySize);
					ServiceCallData serviceCallData = new ServiceCallData();
					serviceCallData.Values["RequestCompressed"] = compressed.ToString();
					serviceCallData.Values["RequestOriginalSize"] = orginalBodySize.ToString();
					serviceCallData.Values["RequestCompressedSize"] = compressedBodySize.ToString();
					EnterpriseTraceSource.TraceSource.TraceData(TraceEventType.Information, 0, serviceCallData.ToTraceData());
				}
				finally
				{
					EnterpriseTraceSource.TraceSource.TraceEvent(TraceEventType.Stop, 0, "Client: compress request");
				}
			}
			return null;
		}
	}
}

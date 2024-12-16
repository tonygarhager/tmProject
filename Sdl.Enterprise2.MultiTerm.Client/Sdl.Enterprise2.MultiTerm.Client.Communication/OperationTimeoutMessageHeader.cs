using System;
using System.ServiceModel.Channels;
using System.Xml;

namespace Sdl.Enterprise2.MultiTerm.Client.Communication
{
	internal class OperationTimeoutMessageHeader : MessageHeader
	{
		private TimeSpan _operationTimeout;

		public override string Name => "OperationTimeout";

		public override string Namespace => "http://sdl.com/soap/timeout/2010";

		public override bool MustUnderstand => true;

		public OperationTimeoutMessageHeader(TimeSpan operationTimeout)
		{
			_operationTimeout = operationTimeout;
		}

		protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
		{
			writer.WriteAttributeString("value", _operationTimeout.ToString());
		}
	}
}

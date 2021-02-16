using System;
using System.Buffers;
using System.Buffers.Binary;
using Bedrock.Framework.Protocols;

namespace Protocols
{
    public class LengthPrefixedProtocol : IMessageReader<Message>, IMessageWriter<Message>
    {
        public bool TryParseMessage(in ReadOnlySequence<byte> input, ref SequencePosition consumed, ref SequencePosition examined, out Message message)
        {
            var reader = new SequenceReader<byte>(input);
            if (!reader.TryReadBigEndian(out int length) || input.Length < length)
            {
                message = default;
                return false;
            }

            var payload = input.Slice(reader.Position, length);
            message = new Message(payload);

            consumed = payload.End;
            examined = consumed;
            return true;
        }

        public void WriteMessage(Message message, IBufferWriter<byte> output)
        {
            var lengthBuffer = output.GetSpan(4);
            BinaryPrimitives.WriteInt32BigEndian(lengthBuffer, (int)message.Payload.Length);
            output.Advance(4);
            output.Write(message.Payload);
        }
    }

    public class Message:IDisposable
    {
        // Flag: Has Dispose already been called?
        private bool disposed = false;
        private byte[] payload;
        public Message(byte[] payload)
        {
            this.payload = payload;
        }

        public Message(ReadOnlySequence<byte> payload)
            :this(payload.ToArray())
        {
        }

        public byte[] Payload { get;}

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }



        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                this.payload=null;
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }
    }
}

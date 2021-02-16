using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication
{
    internal class Session:IDisposable
    {
        private  ConnectionContext context;

        // Flag: Has Dispose already been called?
        private bool  disposed = false;

        /// <summary>
        /// The writer
        /// </summary>
        private  ProtocolWriter writer;
        private IMessageWriter<Message> protocol;
        private bool isConnected;
        private long timeStamps;

        internal bool IsConnected { get => isConnected; }

        public Session(ConnectionContext context, ProtocolWriter writer, IMessageWriter<Message> protocol)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.isConnected = true;
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.timeStamps = Environment.TickCount64;
        }

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Sends the heartbeat.
        /// </summary>
        /// <param name="state">The state.</param>
        internal async Task SendHeartbeatAsync(object state)
        {
            if (this.disposed)
            {
                return;
            }

            if (this.timeStamps >Environment.TickCount64)
            {
                this.timeStamps = Environment.TickCount64;
                return;
            }
            long span = Environment.TickCount64 - this.timeStamps;
            if(span<30000)
            {
                return;
            }
            this.timeStamps = Environment.TickCount64;

            try
            {
              await  this.writer.WriteAsync(this.protocol, Heartbeat.Request);
               
            }
            catch (Exception ex)
            {
                this.context.Abort();
                this.isConnected = false;
            }
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                this.writer = null;
                this.context = null;
                this.protocol = null;
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }
    }
}

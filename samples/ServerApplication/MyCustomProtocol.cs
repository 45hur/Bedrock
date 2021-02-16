// ***********************************************************************
// Assembly         : ServerApplication
// Author           : stephen.wang
// Created          : 04-10-2020
//
// Last Modified By : stephen.wang
// Last Modified On : 04-14-2020
// ***********************************************************************
// <copyright file="MyCustomProtocol.cs" company="ServerApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Threading.Tasks;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.Extensions.Logging;
using Protocols;

namespace ServerApplication
{
    /// <summary>
    /// Class MyCustomProtocol.
    /// Implements the <see cref="Microsoft.AspNetCore.Connections.ConnectionHandler" />
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Connections.ConnectionHandler" />
    /// <seealso cref="System.IDisposable" />
    public class MyCustomProtocol : ConnectionHandler,IDisposable
    {
        // Flag: Has Dispose already been called?
        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Finalizes an instance of the <see cref="MyCustomProtocol"/> class.
        /// </summary>
        ~MyCustomProtocol()
        {
            Dispose(false);
        }


        /// <summary>
        /// The logger
        /// </summary>
        private ILogger _logger;
        /// <summary>
        /// The protocol
        /// </summary>
        private LengthPrefixedProtocol protocol;
        private ConnectionContext context;

        /// <summary>
        /// The reader
        /// </summary>
        private ProtocolReader reader;
        /// <summary>
        /// The writer
        /// </summary>
        private ProtocolWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyCustomProtocol"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public MyCustomProtocol(ILogger<MyCustomProtocol> logger)
        {
            this.disposed = false;
            _logger = logger;// Use a length prefixed protocol
            this.protocol = new LengthPrefixedProtocol();
        }

        /// <summary>
        /// on connected as an asynchronous operation.
        /// </summary>
        /// <param name="connection">The new <see cref="T:Microsoft.AspNetCore.Connections.ConnectionContext" /></param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the connection lifetime. When the task completes, the connection is complete.</returns>
        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            this.context = connection;
            this.reader = connection.CreateReader();
            this.writer = connection.CreateWriter();

            SessionManager.Singleton.AcceptSession(connection, writer, this.protocol);

            //IConnectionHeartbeatFeature heartbeatFeature = connection.Features.Get<IConnectionHeartbeatFeature>();
            //heartbeatFeature.OnHeartbeat(this.SendHeartbeat, null);

            while (true)
            {
                try
                {
                    var result = await reader.ReadAsync(protocol);
                    var message = result.Message;

                    _logger.LogInformation("Received a message of {Length} bytes", message.Payload.Length);

                    if (result.IsCompleted)
                    {
                        break;
                    }



                    await writer.WriteAsync(protocol, message);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.Message);
                    connection.Abort();
                    break;
                }
                finally
                {
                    reader.Advance();
                }
            }

           
        }



        // Public implementation of Dispose pattern callable by consumers.
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
                this._logger = null;
                this.protocol = null;
                this.writer = null;
                this.reader = null;
            }
      
            // Free any unmanaged objects here.
            //
            disposed = true;
        }

      
    }
}
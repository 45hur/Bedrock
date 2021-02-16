using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Protocols;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServerApplication
{
    internal class SessionManager
    {
        private static readonly SessionManager singleton = new SessionManager();
        private List<Session> onlines;
        private System.Collections.Concurrent.ConcurrentQueue<Session> queue;
        private bool stopped;
        private Thread thread;

        internal static SessionManager Singleton => singleton;

        internal SessionManager()
        {
            this.onlines = new List<Session>();
            this.queue = new System.Collections.Concurrent.ConcurrentQueue<Session>();
            this.stopped = false;
        }

        internal void AcceptSession(ConnectionContext context, ProtocolWriter writer, IMessageWriter<Message> protocol)
        {
            Session session = new Session(context, writer, protocol);
            this.queue.Enqueue(session);
        }

        internal void Start(int capacity)
        {
            this.stopped = false;
            if(this.thread!=null)
            {
                return;
            }
            this.onlines.Capacity = capacity;
            this.thread = new Thread(new ThreadStart(this.WorkingAsync));
            this.thread.IsBackground = true;
            this.thread.Start();

        }

        private void WorkingAsync()
        {
            while(true)
            {
                if(this.stopped)
                {
                    break;
                }
                //
                this.MoveQuueToOnlines();
                this.SendHeatbeatAsync();
                Thread.Sleep(1000);
            }
        }

        private void SendHeatbeatAsync()
        {
            Session[] all = onlines.ToArray();
            if(all==null||all.Length<1)
            {
                return;
            }
            Task[] tasks = new Task[all.Length];
            int index = 0;
            foreach (var session in all)
            {
                var task= this.SendHeatbeatAsync(session);
                tasks[index++] = task;
            }

            Task.WaitAll(tasks);
        }

        private async Task SendHeatbeatAsync(Session session)
        {
            await session.SendHeartbeatAsync(Heartbeat.Request);
            if(session.IsConnected)
            {
                return;
            }

            this.onlines.Remove(session);
           
        }

        private void MoveQuueToOnlines()
        {
            Session session = null;
            while (this.queue.TryDequeue(out session))
            {
                this.onlines.Add(session);
                session = null;
            }

        }

        internal void Stop()
        {
            this.stopped = true;
        }

    }
}

using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace ReadOnlySequence.Explore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            byte[] array = new byte[128];
            byte pos = 0;
            for(int i=3;i<array.Length;i++)
            {
                array[i] = pos++;
            }

            ReadOnlySequence<byte> input = new ReadOnlySequence<byte>(array);
            SequenceReader<byte> reader = new SequenceReader<byte>(input);
            int frontFour = 0;
            reader.TryReadBigEndian(out frontFour);

            var payLoad = input.Slice(reader.Position, 4);


           // return await SyncAsync();

        }

        private static async Task SyncAsync()
        {
            Console.WriteLine("111 balabala. My Thread ID is :" + Thread.CurrentThread.ManagedThreadId);
            var task =  await TimeConsumingMethod();
            Console.WriteLine(task);
            Console.WriteLine("222 balabala. My Thread ID is :" + Thread.CurrentThread.ManagedThreadId);
        }


        //这个函数就是一个耗时函数，可能是IO操作，也可能是cpu密集型工作。
        private static Task<string> TimeConsumingMethod()
        {
            var task = Task.Run(() => {
                Console.WriteLine("Helo I am TimeConsumingMethod. My Thread ID is :" + Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(5000);
                Console.WriteLine("Helo I am TimeConsumingMethod after Sleep(5000). My Thread ID is :" + Thread.CurrentThread.ManagedThreadId);
                return "Hello I am TimeConsumingMethod";
            });

            return task;
        }
    }
}

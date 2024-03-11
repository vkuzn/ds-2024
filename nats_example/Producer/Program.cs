using NATS.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Producer started");

            CancellationTokenSource cts = new CancellationTokenSource();

            Task.Factory.StartNew(() => ProduceAsync(cts.Token), cts.Token);

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            cts.Cancel();

            Console.WriteLine("done");
        }

        static async Task ProduceAsync(CancellationToken ct)
        {
            ConnectionFactory cf = new ConnectionFactory();

            using (IConnection c = cf.CreateConnection())
            {
                ulong count = 0;
                while (!ct.IsCancellationRequested)
                {
                    string m = $"#{count}";
                    Console.WriteLine("Produced: {0}", m);
                    byte[] data = Encoding.UTF8.GetBytes(m);
                    c.Publish("valuator.processing.rank", data);
                    await Task.Delay(1000);
                    ++count;
                }
                c.Drain();

                c.Close();
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace HomeWorkParallel
{
    class Program
    {
        static int countPools = 4;
        static void Main(string[] args)
        {
            int n = 10000000;
            int[] mas = new int[n];
            mas = Initialize(n);

            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            Stopwatch sw3 = new Stopwatch();

            sw1.Start();
            int sum1 = SimpleSum(mas);
            sw1.Stop();

            sw2.Start();
            int sum2 = SimpleSum(mas);
            sw2.Stop();

            sw3.Start();
            int sum3 = SimpleSum(mas);
            sw3.Stop();

            Console.WriteLine("Последовательный подсчет суммы занял " + sw1.Elapsed.TotalSeconds + " секунд, результат " + sum1);
            Console.WriteLine("Параллельный (thread) подсчет суммы занял " + sw2.Elapsed.TotalSeconds + " секунд, результат " + sum2);
            Console.WriteLine("Параллельный (linq) подсчет суммы занял " + sw3.Elapsed.TotalSeconds + " секунд, результат " + sum3);
        }

        static int[] Initialize(int n)
        {
            int[] mas = new int[n];
            Random r = new Random();
            for (int i = 0; i < n; i++)
            {
                mas[i] = r.Next(0, 100);
            }
            return mas;
        }

        static int SimpleSum(int[] mas)
        {
            int sum = 0;
            for (int i = 0; i < mas.Length; i++)
                sum += mas[i];
            return sum;
        }
        static void SimpleSumThread(ref int sum, int[] mas)
        {
            sum = 0;
            for (int i = 0; i < mas.Length; i++)
                sum += mas[i];
        }

        static int ThreadSum(int[] mas)
        {
            int sum = 0; 
            int range = mas.Length / countPools + 1;
            Task[] tasks = new Task[countPools];
            for (int iThread = 0; iThread < countPools; iThread++)
            {
                var localThread = iThread;
                tasks[localThread] = Task.Run(() =>
                {
                    for (int j = localThread * range; j < (localThread + 1) * range; j++)
                    {
                        if (j < mas.Length)
                            Interlocked.Add(ref sum, mas[j]);
                        else break;
                    }
                });
            }
            Task.WaitAll(tasks);
            return sum;
        }

        static int ParallelSum(int[] mas)
        {
            int sum = 0;
            Parallel.ForEach<int>(mas, (int i) => sum += i);
            return sum;
        }
    }
}

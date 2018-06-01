using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TaskDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            //使用ThreadPool和task完成相同的事情
            ThreadPool.QueueUserWorkItem((obj) => WriteOperationOne());//线程池调用阻塞定时方法
            Task.Run(() => WriteOperationOne());//Task 调用阻塞定时方法 ==》等价于new Task(() => WriteOperationOne()).Start();

            Task.Run(() => WriteOperationTwo());//Task 调用非阻塞定时方法
            //Task的协作式取消
            Task.Run(() => WriteOperationThree(cts.Token));//利用lambda闭包传递cts
            Console.WriteLine("2S后取消Operation3");
            cts.Token.Register(() => Console.WriteLine("Operation3 取消完成"));
            cts.CancelAfter(2000);



            //异步操作中，防止主线程提前退出，而程序终止
            //Task是基于ThreadPool的，ThreadPool创建的线程均为后台线程。
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        //不推荐写法
        static void WriteOperationOne()
        {
            while (true)
            {
                Console.WriteLine("Operation One");
                Thread.Sleep(1000);//造成线程阻塞，浪费性能
            }
        }

        //定时任务的推荐写法
        static async void WriteOperationTwo()
        {
            while(true)
            {
                Console.WriteLine("Operation Two");
                await Task.Delay(1000);//效果与WriteOperationOne相同，但是不造成线程阻塞，性能良好
            }
        }

        //定时任务的协作取消
        static async void WriteOperationThree(CancellationToken token)
        {
            while(true)
            {
                if (token.IsCancellationRequested)
                    break;
                Console.WriteLine("Operation Three");
                await Task.Delay(1000);
            }
        }
    }
}

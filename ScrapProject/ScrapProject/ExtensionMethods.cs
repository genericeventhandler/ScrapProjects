using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrapProject
{
    public static class ExtensionMethods
    {
        public static Task<T> ExecuteAsync<T>(this Func<T> methodToExecute)
        {
            var t = new Task<T>(methodToExecute);
            t.Start();
            return t;
        }

        public static void WaitAndDispose(this Task task)
        {
            if (task != null)
            {
                task.Wait();
                task.Dispose();
            }
        }

        public static Task ExecuteAsync(this Action methodToExecute)
        {
            var t = new Task(methodToExecute);
            t.Start();
            return t;
        }

        public static Task<TResult> ExecuteAsync<T, TResult>(this Func<T, TResult> methodToExecute, T parameter)
        {
            var t = new Task<TResult>(() => methodToExecute(parameter));
            t.Start();
            return t;
        }

        public static void TestAysnc()
        {
            var t1 = new Func<string>(() =>
            {
                var x = "aaa";
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine("Update from within method aysnc " + x);
                System.Threading.Thread.Sleep(2000);
                return x;
            }).ExecuteAsync();

            var t2 = new Func<string>(() =>
            {
                var x = "bbb";
                System.Threading.Thread.Sleep(500);
                Console.WriteLine("Update from within method aysnc " + x);
                System.Threading.Thread.Sleep(3000);
                return x;
            }).ExecuteAsync();

            var t3 = new Func<string>(() =>
            {
                var x = "ccc";
                System.Threading.Thread.Sleep(1500);
                Console.WriteLine("Update from within method aysnc " + x);
                System.Threading.Thread.Sleep(4000);
                return x;
            }).ExecuteAsync();

            var t4 = new Func<string>(TryIt).ExecuteAsync();
            var t5 = new Func<string, string>(TryIt2).ExecuteAsync("oooo.oooo");

            t1.WaitAndDispose();
            t2.WaitAndDispose();
            t3.WaitAndDispose();
            t4.WaitAndDispose();
            t5.WaitAndDispose();

            Console.WriteLine("All finished");
            Console.WriteLine();
            for (int loop = 4; loop >= 0; loop--)
            {
                System.Threading.Thread.Sleep(1000);
                Console.Write("." + loop);
            }
        }

        public static string TryIt()
        {
            var x = "ddd";
            System.Threading.Thread.Sleep(1700);
            Console.WriteLine("Update from within method aysnc " + x);
            System.Threading.Thread.Sleep(5000);
            return x;
        }

        public static string TryIt2(string x)
        {
            System.Threading.Thread.Sleep(100);
            Console.WriteLine("Tryit2");
            return x + " !!!";
        }
    }
}

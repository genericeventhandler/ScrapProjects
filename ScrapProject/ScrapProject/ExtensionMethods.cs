// <copyright file="ExtensionMethods.cs" company="GenericEventHandler">
//     Copyright (c) GenericEventHandler all rights reserved. Licensed under the Mit license.
// </copyright>

using System;
using System.Threading.Tasks;

namespace ScrapProject
{
    /// <summary>Extension methods and test methods for Executing methods async</summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Executes a parameterless method async. be sure to call WaitAndDispose on the method.
        /// </summary>
        /// <param name="methodToExecute">the method to execute</param>
        /// <typeparam name="T">the return type of the method</typeparam>
        /// <returns>a task object</returns>
        public static Task<T> ExecuteAsync<T>(this Func<T> methodToExecute)
        {
            var t = new Task<T>(methodToExecute);
            t.Start();
            return t;
        }

        /// <summary>Executes a void Action</summary>
        /// <param name="methodToExecute">the action to execute</param>
        /// <returns>the task that is executing</returns>
        public static Task ExecuteAsync(this Action methodToExecute)
        {
            var t = new Task(methodToExecute);
            t.Start();
            return t;
        }

        /// <summary>Executes a method with a parameter passed</summary>
        /// <typeparam name="T">The type of parameter passed to the method</typeparam>
        /// <typeparam name="TResult">the result of the method</typeparam>
        /// <param name="methodToExecute">the method to execute</param>
        /// <param name="parameter">the parameter to pass</param>
        /// <returns>A task that is running</returns>
        public static Task<TResult> ExecuteAsync<T, TResult>(this Func<T, TResult> methodToExecute, T parameter)
        {
            var t = new Task<TResult>(() => methodToExecute(parameter));
            t.Start();
            return t;
        }

        /// <summary>Gets the result from the task and disposes the task</summary>
        /// <typeparam name="T">the type of return parameter from the task</typeparam>
        /// <param name="task">the task that is executing</param>
        /// <returns>T the return type of the task.</returns>
        public static T ResultAndDispose<T>(this Task<T> task)
        {
            var result = task.Result;
            task.Dispose();
            return result;
        }

        /// <summary>Test the Task execution</summary>
        public static void TestAysnc()
        {
            var t1 = new Func<string>(() =>
            {
                Console.WriteLine("Running task 1");
                var x = "aaa";
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine("Update from within method aysnc " + x);
                System.Threading.Thread.Sleep(2000);
                return x;
            }).ExecuteAsync();

            var t2 = new Func<string>(() =>
            {
                Console.WriteLine("Running task 2");
                var x = "bbb";
                System.Threading.Thread.Sleep(500);
                Console.WriteLine("Update from within method aysnc " + x);
                System.Threading.Thread.Sleep(3000);
                return x;
            }).ExecuteAsync();

            var t3 = new Func<string>(() =>
            {
                Console.WriteLine("Running task 3");
                var x = "ccc";
                System.Threading.Thread.Sleep(1500);
                Console.WriteLine("Update from within method aysnc " + x);
                System.Threading.Thread.Sleep(4000);
                return x;
            }).ExecuteAsync();

            var t4 = new Func<string>(TryIt).ExecuteAsync();
            var t5 = new Func<string, string>(TryIt2).ExecuteAsync("oooo.oooo");
            var t6 = new Func<MyObject, string>(TryIt3).ExecuteAsync(new MyObject { FirstName = "Donald", Name = "Duck" });

            t1.WaitAndDispose();
            t2.WaitAndDispose();
            t3.WaitAndDispose();
            t4.WaitAndDispose();
            t5.WaitAndDispose();
            t6.WaitAndDispose();

            Console.WriteLine("All finished");
            Console.WriteLine();
            for (int loop = 4; loop >= 0; loop--)
            {
                System.Threading.Thread.Sleep(1000);
                Console.Write("." + loop);
            }
        }

        /// <summary>A method that writes to the console and fakes some action</summary>
        /// <returns>the string ddd</returns>
        public static string TryIt()
        {
            Console.WriteLine("Running task 4");
            var x = "ddd";
            System.Threading.Thread.Sleep(1700);
            Console.WriteLine("Update from within method aysnc " + x);
            System.Threading.Thread.Sleep(5000);
            return x;
        }

        /// <summary>Waits for the task to complete and then disposes it.</summary>
        /// <param name="task">the task that is executing</param>
        public static void WaitAndDispose(this Task task)
        {
            if (task != null)
            {
                task.Wait();
                task.Dispose();
            }
        }

        private static string TryIt2(string x)
        {
            Console.WriteLine("Running task 5");
            System.Threading.Thread.Sleep(100);
            Console.WriteLine("Tryit2");
            return x + " !!!";
        }

        private static string TryIt3(MyObject ob)
        {
            Console.WriteLine("Running task 6");
            Console.WriteLine(ob);
            return ob.ToString();
        }

        private class MyObject
        {
            /// <summary>Gets or sets the first name</summary>
            internal string FirstName { get; set; }

            /// <summary>Gets or sets the name</summary>
            internal string Name { get; set; }

            /// <summary>Concatinates name and firstname</summary>
            /// <returns>the string representation of the class.</returns>
            public override string ToString()
            {
                return Name + " " + FirstName;
            }
        }
    }
}
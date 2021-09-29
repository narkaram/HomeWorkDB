using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HomeWork_Delegates
{
    class Program
    {
        public event EventHandler<FileArgs> FileFound;
        static void Main(string[] args)
        {
            //////1
            Test1[] mas = { new Test1(1), new Test1(2), new Test1(15), new Test1(3), new Test1(4), new Test1(5) };
            var b = ExtensionClass.GetMax<Test1>(mas, toFloat);
            Console.WriteLine("Максимальное число - " + b.a.ToString());
            Console.WriteLine("");
            //////2
            FileWatcher fileWatcher = new(@"D:\Рабочий стол\ДЗ"); 
            fileWatcher.FileFound += onFileFound;
            fileWatcher.Search("");
        }

        public static void onFileFound(object sender, FileArgs eventArgs)
        {
            Console.WriteLine(eventArgs.NameFile);
            if (Path.GetFileName(eventArgs.NameFile) == "exit.txt")
                eventArgs.CancelRequested = true;
        }

        static public float toFloat(Test1 a)
        {
            return (float)a.a;
        }

    }

    #region первое задание
    class Test1
    {
        public int a { get; set; }
        public Test1()
        {
            a = 15;
        }
        public Test1(int _a)
        {
            a = _a;
        }

    }
    public static class ExtensionClass //where T : class
    {
        public static T GetMax<T>(this IEnumerable<T> e, Func<T, float> getParameter)
        {
            float max = -10000;
            var list = e.ToList();
            T res = default(T);
            for(int i=0; i< list.Count; i++)
            {
                var value = getParameter(list[i]);
                if(value > max)
                {
                    max = value;
                    res = list[i];
                }
                    
            }
            //var digits = e.Select(getParameter);
            //var max = digits.ToList().FindIndex(item => item == digits.Max());
            return res;
        }
    }
    #endregion

}

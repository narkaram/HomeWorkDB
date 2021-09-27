using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace HomeWork_9._0
{
    class Program
    {
        static void Main(string[] args)
        {
            string exit = "";
            while (exit != "exit")
            {
                Console.WriteLine("Введите выражение");
                var expr = Console.ReadLine();
                Console.WriteLine(Calculation(expr));
            }
        }
        

        static string Calculation(string expression)
        {
            while(expression.Contains("("))
            {
                int end = expression.IndexOf(')');
                int start = -1;
                for (int i = end; i >= 0; i--)
                {
                    start = expression.IndexOf('(', i);
                    if (start < end && start > -1)
                        break;
                }
                if (start == -1)
                {
                    Error err = new("Ошибка", "Неверная расстановка скобок");//NEW FEATURE
                    PrintError(err);
                }
                string exp = expression.Substring(start, end - start + 1);
                expression = expression.Replace(exp, Calculation(exp.Replace(")", "").Replace("(", "")));
            }
            Regex reg = new Regex(@"[\+\-\*\/]");
            string oper = reg.Match(expression).ToString();
            double par1 = Convert.ToDouble(expression.Split(oper)[0]);
            double par2 = Convert.ToDouble(expression.Split(oper)[1]);
            try
            {
                //NEW FEATURE
                static double? calc (string oper, double par1, double par2) =>
                    oper switch
                    {
                        "+" => par1 + par2,
                        "-" => par1 - par2,
                        "*" => par1 * par2,
                        "/" => par1 / par2,
                        _ => null
                    };
                var result = calc(oper, par1, par2);
                if (result is not null) //NEW FEATURE
                    return result.ToString();
                else
                    PrintError(new Error("Ошибка", "Неизвестный оператор"));
                return "";
            }
            catch (Exception e)
            {
                if(e is System.DivideByZeroException)
                    PrintError(new Error(null, "На 0 делить нельзя")); 
                else PrintError(new Error("Ошибка", null));
                return "";
            }
            
        }

        static void PrintError(Error err)
        {
            if (err is { Note: not null, Type: not null }) //NEW FEATURE
            {
                Console.WriteLine(err.Type + ", " + err.Note);
            }
            else Console.WriteLine(isnull(err.Type) + isnull(err.Note));
        }

        static string isnull(string str)
        {
            if (str is null)
                return "";
            return str;
        }

        //NEW FEATURE
        [ModuleInitializer]
        public static void InitializerMethod1()
        {
            Console.WriteLine(value: "Данная программа позволяет вычислять простые выражения, состоящие из операторов +,-,* и /. Допускается использование круглых скобок. Для выхода из программы введите команду \"exit\"");
        }

        public class Error
        {
            public string Type { get; set; }
            public string Note { get; set; }

            public Error(string type, string note)
            {
                Type = type;
                Note = note;
            }
        }

    }
}

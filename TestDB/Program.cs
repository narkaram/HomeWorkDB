using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDB
{
    class Program
    {
        static void Main(string[] args)
        {

            WorkWithDB.CreateUsersTable();
            WorkWithDB.CreateCategoriesTable();
            WorkWithDB.CreateAnnouncementTable();

            WorkWithDB.InsertData();

            var users = WorkWithDB.GetData("select * from users");
            WriteText("Users", users.Item1, users.Item2);

            var categories = WorkWithDB.GetData("select * from categories");
            WriteText("Categories", categories.Item1, categories.Item2);

            var announcement = WorkWithDB.GetData("select * from announcement");
            WriteText("Announcement", announcement.Item1, announcement.Item2);

            if (Console.ReadLine().ToLower() == "insert")
            {
                Console.WriteLine("В какую таблицу писать?");
                string table = Console.ReadLine().ToLower();
                switch (table)
                {
                    case "users":
                        Console.WriteLine("Введите логин пользователя");
                        string username = Console.ReadLine();
                        Console.WriteLine("Введите имя пользователя");
                        string first_name = Console.ReadLine();
                        Console.WriteLine("Введите фамилию пользователя");
                        string last_name = Console.ReadLine();
                        Console.WriteLine("Введите электронный адрес пользователя");
                        string email = Console.ReadLine();
                        Console.WriteLine("Введите телефон пользователя");
                        string phone = Console.ReadLine();
                        WorkWithDB.GetData("INSERT INTO USERS(username, first_name, last_name, email, phone) VALUES('" + username + "', '" + first_name + "', '" + last_name + "', '" + email + "', '" + phone + "')");

                        Console.WriteLine("Пользователь успешно добавлен!");
                        var users1 = WorkWithDB.GetData("select * from users");
                        WriteText("Users", users1.Item1, users1.Item2);

                        break;
                    case "categories":
                        Console.WriteLine("Введите название категории");
                        string name_cat = Console.ReadLine();
                        Console.WriteLine("Введенная категория является верхнеуровневой? (y/n)");
                        string sub_cat = Console.ReadLine().ToLower();
                        bool err = false;
                        if (sub_cat == "y")
                            WorkWithDB.GetData("INSERT INTO CATEGORIES(NAME) VALUES('" + name_cat + "')");
                        else
                        {
                            Console.WriteLine("Введите название категории, к которой относится добавляемая категория");
                            string sub_cat_name = Console.ReadLine().ToLower();
                            var data_cat = WorkWithDB.GetData("select id from CATEGORIES where name like '" + sub_cat_name + "')");
                            try
                            {
                                string id = data_cat.Item2[0][0];
                                WorkWithDB.GetData("INSERT INTO CATEGORIES(NAME, Main_category) VALUES('" + name_cat + "', " + id + ")");
                            }
                            catch
                            {
                                err = true;
                                Console.WriteLine("Не удалось найти заданную категорию.");
                            }
                        }
                        if (!err)
                        {
                            Console.WriteLine("Категория успешно добавлена!");
                            var categories1 = WorkWithDB.GetData("select * from CATEGORIES");
                            WriteText("Users", categories1.Item1, categories1.Item2);
                        }
                        break;
                    case "announcement":
                        bool errAnn = false;
                        Console.WriteLine("Введите заголовок объявления");
                        string caption = Console.ReadLine();
                        Console.WriteLine("Введите название категории пользователя");
                        string category_id = Console.ReadLine().ToLower();
                        var data = WorkWithDB.GetData("select id from CATEGORIES where name like '" + category_id + "')");
                        try
                        {
                            category_id = data.Item2[0][0];
                        }
                        catch
                        {
                            errAnn = true;
                            Console.WriteLine("Не удалось найти заданную категорию");
                        }
                        if (!errAnn)
                        {
                            Console.WriteLine("Введите логин пользователя");
                            string user_id = Console.ReadLine();
                            var data2 = WorkWithDB.GetData("select id from users where name like '" + user_id + "')");
                            try
                            {
                                user_id = data2.Item2[0][0];
                            }
                            catch
                            {
                                errAnn = true;
                                Console.WriteLine("Не удалось найти указанного пользователя");
                            }
                            if (!errAnn)
                            {
                                Console.WriteLine("Введите адрес");
                                string address = Console.ReadLine();
                                Console.WriteLine("Введите цену товара");
                                string price = Console.ReadLine();
                                Console.WriteLine("Введите описание объявления");
                                string description = Console.ReadLine();
                                WorkWithDB.GetData(@"INSERT (caption, category_id, address, price, create_date, user_id, description)
                                                     VALUES('" + caption + "', " + category_id + ", '" + address + "', '" + price.Replace(" ", "") + "', current_timestamp, " + user_id + ", '" + description + "')");
                                Console.WriteLine("Объявление успешно добавлено!");
                                var announcement1 = WorkWithDB.GetData("select * from users");
                                WriteText("Announcement", announcement1.Item1, announcement1.Item2);
                            }
                        }

                        break;
                    default:
                        Console.WriteLine("К сожалению, это невозможно, таблица с заданным именем не найдена.");
                        break;
                }
            }
            else Environment.Exit(0);
        }

        private static void WriteText(string nameTable, List<string> headers, List<List<string>> data)
        {
            Console.WriteLine($"{nameTable}:\n");
            string header = "";
            for (int i = 0; i < headers.Count; i++)
            {
                header += headers[i] + " | ";
            }
            header.TrimEnd('|');
            Console.WriteLine(header);
            string row;
            for (int i = 0; i < data.Count; i++)
            {
                row = "";
                for (int j = 0; j < data[i].Count; j++)
                {
                    row += data[i][j] + " | ";
                }
                Console.WriteLine(row);
            }
            Console.WriteLine("\n");
        }
    }
}

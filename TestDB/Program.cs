using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDB
{
    class Program
    {
        const string separator = " | ";
        static void Main(string[] args)
        {

            WorkWithDB.CreateUsersTable();
            WorkWithDB.CreateCategoriesTable();
            WorkWithDB.CreateAnnouncementTable();

            WorkWithDB.InsertData();

            var (users_headers, users_values) = WorkWithDB.GetData("select * from users");
            WriteText("Users", users_headers, users_values);

            var (categories_headers, categories_values) = WorkWithDB.GetData("select * from categories");
            WriteText("Categories", categories_headers, categories_values);

            var (announcement_headers, announcement_values) = WorkWithDB.GetData("select * from announcement");
            WriteText("Announcement", announcement_headers, announcement_values);

            if (Console.ReadLine().ToLower() == "insert")
            {
                Console.WriteLine("В какую таблицу писать?");
                string table = Console.ReadLine().ToLower();
                switch (table)
                {
                    case "users":
                        var users_var = new Dictionary<string, string>();
                        Console.WriteLine("Введите логин пользователя");
                        string username = Console.ReadLine();
                        users_var.Add("@username", username);
                        Console.WriteLine("Введите имя пользователя");
                        string first_name = Console.ReadLine();
                        users_var.Add("@first_name", first_name);
                        Console.WriteLine("Введите фамилию пользователя");
                        string last_name = Console.ReadLine();
                        users_var.Add("@last_name", last_name);
                        Console.WriteLine("Введите электронный адрес пользователя");
                        string email = Console.ReadLine();
                        users_var.Add("@email", email);
                        Console.WriteLine("Введите телефон пользователя");
                        string phone = Console.ReadLine();
                        users_var.Add("@phone", phone);

                        WorkWithDB.InsertData("INSERT INTO USERS(username, first_name, last_name, email, phone) VALUES(@username, @first_name, @last_name, @email, @phone)", users_var);
                        Console.WriteLine("Пользователь успешно добавлен!");
                        var (users_head, users_value) = WorkWithDB.GetData("select * from users");
                        WriteText("Users", users_head, users_value);

                        break;
                    case "categories":
                        Console.WriteLine("Введите название категории");
                        string name_cat = Console.ReadLine();
                        Console.WriteLine("Введенная категория является верхнеуровневой? (y/n)");
                        string sub_cat = Console.ReadLine().ToLower();
                        bool err = false;
                        if (sub_cat == "y")
                        {
                            WorkWithDB.InsertData("INSERT INTO CATEGORIES(NAME) VALUES(@name_cat)", new Dictionary<string, string> { { "@name_cat", name_cat } });
                        }
                        else
                        {
                            Console.WriteLine("Введите название категории, к которой относится добавляемая категория");
                            string sub_cat_name = Console.ReadLine().ToLower();
                            var (data_cat_header, data_cat) = WorkWithDB.GetData("select id from CATEGORIES where name = '" + sub_cat_name + "')");
                            try
                            {
                                string id = data_cat[0][0];
                                WorkWithDB.InsertData("INSERT INTO CATEGORIES(NAME, Main_category) VALUES(@name_cat, @id)", new Dictionary<string, string> { { "@name_cat", name_cat }, { "@id", id } });
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
                            var (cat_head, cat_value) = WorkWithDB.GetData("select * from CATEGORIES");
                            WriteText("Categories", cat_head, cat_value);
                        }
                        break;
                    case "announcement":
                        var announcement_var = new Dictionary<string, string>();
                        bool errAnn = false;
                        Console.WriteLine("Введите заголовок объявления");
                        string caption = Console.ReadLine();
                        announcement_var.Add("@caption", caption);
                        Console.WriteLine("Введите название категории пользователя");
                        string category_id = Console.ReadLine().ToLower();
                        var (cat_res_head, cat_res) = WorkWithDB.GetData("select id from CATEGORIES where name like '" + category_id + "')");
                        try
                        {
                            category_id = cat_res[0][0];
                            announcement_var.Add("@category_id", category_id);
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
                            var (user_res_head, user_res) = WorkWithDB.GetData("select id from users where name like '" + user_id + "')");
                            try
                            {
                                user_id = user_res[0][0];
                                announcement_var.Add("@user_id", user_id);
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
                                announcement_var.Add("@address", address);
                                Console.WriteLine("Введите цену товара");
                                string price = Console.ReadLine();
                                announcement_var.Add("@price", price);
                                Console.WriteLine("Введите описание объявления");
                                string description = Console.ReadLine();
                                announcement_var.Add("@description", description);

                                WorkWithDB.InsertData("INSERT INTO USERS(username, first_name, last_name, email, phone) VALUES(@caption, @category_id, @address, @price, @current_timestamp, @user_id, @description)", announcement_var);
                                Console.WriteLine("Объявление успешно добавлено!");
                                var (announcement_head, announcement_value) = WorkWithDB.GetData("select * from announcement");
                                WriteText("Announcement", announcement_head, announcement_value);
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
                header += headers[i] + separator;
            }
            header.TrimEnd('|');
            Console.WriteLine(header);
            string row;
            for (int i = 0; i < data.Count; i++)
            {
                row = "";
                for (int j = 0; j < data[i].Count; j++)
                {
                    row += data[i][j] + separator;
                }
                Console.WriteLine(row);
            }
            Console.WriteLine("\n");
        }
    }
}

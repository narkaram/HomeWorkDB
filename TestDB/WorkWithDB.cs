using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDB
{
    class WorkWithDB
    {
        public const string connectionString = "Host=****;Username=postgres;Password=****;Database=testvally";

        public static void CreateUsersTable()
        {
            using (var connection = new NpgsqlConnection(connectionString)) {
                connection.Open();

                if (!tableIsExest(connection, "users"))
                {
                    var sql = @"
                    CREATE SEQUENCE users_id_seq;

                    CREATE TABLE users
                    (
                        id              BIGINT                      NOT NULL    DEFAULT NEXTVAL('users_id_seq'),
                        username        CHARACTER VARYING(255)      NOT NULL,
                        first_name      CHARACTER VARYING(255)      NOT NULL,
                        last_name       CHARACTER VARYING(255),
                        email           CHARACTER VARYING(255)      NOT NULL,
                        phone           CHARACTER VARYING(255),
  
                        CONSTRAINT users_key PRIMARY KEY (id),
                        CONSTRAINT users_username_unique UNIQUE (username),
                        CONSTRAINT users_email_unique UNIQUE (email)
                    );

                    CREATE UNIQUE INDEX users_email_unq_idx ON users(lower(email));
                    ";

                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void CreateAnnouncementTable()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                if (!tableIsExest(connection, "announcement"))
                {
                    var sql = @"
                    CREATE SEQUENCE announcement_id_seq;

                    select exists (select * from information_schema.tables where table_name = 'table_name' and table_schema = 'public')

                    CREATE TABLE announcement
                    (
                        id              BIGINT                      NOT NULL    DEFAULT NEXTVAL('announcement_id_seq'),
                        caption         CHARACTER VARYING(255)      NOT NULL,
                        category_id     BIGINT                      NOT NULL,
                        address         CHARACTER VARYING(255)      NOT NULL,
                        price           MONEY,      
                        create_date     TIMESTAMP WITH TIME ZONE    NOT NULL,
                        user_id         BIGINT                      NOT NULL,
                        description     CHARACTER VARYING(8000)      NOT NULL,
  
                        CONSTRAINT announcement_key PRIMARY KEY (id),
                        CONSTRAINT announcement_fk_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
                        CONSTRAINT announcement_fk_category_id FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE CASCADE
                    );";

                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void CreateCategoriesTable()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                if (!tableIsExest(connection, "categories"))
                {
                    var sql = @"
                    CREATE SEQUENCE categories_id_seq;

                    CREATE TABLE categories
                    (
                        id              BIGINT                      NOT NULL    DEFAULT NEXTVAL('categories_id_seq'),
                        name            CHARACTER VARYING(255)      NOT NULL,
                        main_category   BIGINT,
  
                        CONSTRAINT categories_key PRIMARY KEY (id),
                        CONSTRAINT categories_fk_category_id FOREIGN KEY (main_category) REFERENCES categories(id) ON DELETE CASCADE
                    );";

                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void InsertData()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var sql1 = @"
                    TRUNCATE public.users CASCADE;
                    ALTER SEQUENCE users_id_seq RESTART WITH 1;

                    INSERT INTO USERS (username, first_name, last_name, email, phone)
                    VALUES ('Ren@r', 'Ренар', null, 'renar1986@mail.ru', '89938848848');

                    INSERT INTO USERS (username, first_name, last_name, email, phone)
                    VALUES ('Kefir', 'Антон', null, 'my_email111@mail.ru', '8922284888');

                    INSERT INTO USERS (username, first_name, last_name, email, phone)
                    VALUES ('RoboKop', 'Альберт', 'Никантин', 'nikantin_a@ya.ru', '8991231232');

                    INSERT INTO USERS (username, first_name, last_name, email, phone)
                    VALUES ('Konstantin', 'Костя', 'Максимов', 'kk_maximov@gmail.com', null);

                    INSERT INTO USERS (username, first_name, last_name, email, phone)
                    VALUES ('VarVar', 'Варвара', null, 'helentyeva@mail.ru', null);
                ";

                using (var cmd = new NpgsqlCommand(sql1, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                var sql2 = @"
                    TRUNCATE public.CATEGORIES CASCADE;
                    ALTER SEQUENCE categories_id_seq RESTART WITH 1;
    
                    INSERT INTO CATEGORIES (ID, NAME, MAIN_CATEGORY)
                    VALUES (1, 'Транспорт', null);

                    INSERT INTO CATEGORIES (ID, NAME, MAIN_CATEGORY)
                    VALUES (2, 'Автомобили', 1);

                    INSERT INTO CATEGORIES (ID, NAME, MAIN_CATEGORY)
                    VALUES (3, 'Мотоциклы', 1);

                    INSERT INTO CATEGORIES (ID, NAME, MAIN_CATEGORY)
                    VALUES (4, 'Недвижимость', null);

                    INSERT INTO CATEGORIES (ID, NAME, MAIN_CATEGORY)
                    VALUES (5, 'Квартиры', 4);
                ";

                using (var cmd = new NpgsqlCommand(sql2, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                var sql3 = @"
                    TRUNCATE public.ANNOUNCEMENT CASCADE;
                    INSERT INTO ANNOUNCEMENT (caption, category_id, address, price, create_date, user_id, description)
                    VALUES ('Hyundai Solaris, 2013', 2, 'г. Уфа', 550000, current_timestamp, 1, 'Автомобиль в хорошем состоянии');

                    INSERT INTO ANNOUNCEMENT (caption, category_id, address, price, create_date, user_id, description)
                    VALUES ('Квадроцикл Tiger Sport 250 cc', 3, 'г. Уфа', 199900, current_timestamp, 2, 'Ознакомиться со всеми предложениями можно на нашем официальном сайте или в нашем магазине.');

                    INSERT INTO ANNOUNCEMENT (caption, category_id, address, price, create_date, user_id, description)
                    VALUES ('Снегоход sharmax SN-550 maх PRO', 1, 'г. Уфа', 339900, current_timestamp, 3, 'СНЕГОХОД SHARMAX SN-550 MAХ PRO -  построен на базе инновационной модульной рамы, каждый элемент которой индивидуально вырезается на лазерном станке.');

                    INSERT INTO ANNOUNCEMENT (caption, category_id, address, price, create_date, user_id, description)
                    VALUES ('1-к. квартира, 28,8 м², 7/28 эт.', 5, 'г. Уфа, Малая Тихорецкая ул.', 2880000, current_timestamp, 4, 'Продам шикарную квартира в ЖК Июнь!');

                    INSERT INTO ANNOUNCEMENT (caption, category_id, address, price, create_date, user_id, description)
                    VALUES ('2-к. квартира, 76,8 м², 6/16 эт.', 5, 'г. Уфа, ул. Софьи Перовской, 54', 7300000, current_timestamp, 5, 'Продам просторную 2-ком. квартиру в элитном районе Южный.');
                ";
                using (var cmd = new NpgsqlCommand(sql3, connection))
                {
                    cmd.ExecuteNonQuery();
                }

            }
        }

        public static (List<string>, List<List<string>>) GetData(string query)
        {
            List<string> headers = new List<string>();
            List<List<string>> datas = new List<List<string>>();
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var sql = query;
                NpgsqlCommand command = connection.CreateCommand();
                command.CommandText = sql;
                NpgsqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    int countColumn = reader.FieldCount;
                    for (int i = 0; i < countColumn; i++)
                        headers.Add(reader.GetName(i));
                    while (reader.Read())
                    {
                        List<string> tmp = new List<string>();
                        for (int i = 0; i < countColumn; i++)
                        {
                            string tmpValue = reader.GetValue(i).ToString();
                            tmp.Add(tmpValue);
                        }
                        datas.Add(tmp);
                    }
                }
                reader.Close();
                command.Dispose();
            }
            return (headers, datas);
        }

        private static bool tableIsExest(NpgsqlConnection connection, string nameTable)
        {
            var sqlCheck = "select exists(select * from information_schema.tables where table_name = '"+ nameTable + "' and table_schema = 'public')";
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = sqlCheck;
            NpgsqlDataReader reader = command.ExecuteReader();
            bool existTable = false;
            if (reader.HasRows)
            {
                if(reader.Read())
                    existTable = bool.Parse(reader.GetValue(0).ToString());
            }
            reader.Close();
            command.Dispose();
            return existTable;
        }
    }
}

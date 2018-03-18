using System;
using System.Collections.Generic;
using FSNCheck.Data;
using MySql.Data.MySqlClient;

namespace TELEGA
{
    class MySQLControl
    {
        private string m_user_id;
        private string m_password;
        private string m_database;
        private string m_host;

        public MySQLControl()
        {
            m_user_id = "Master";
            m_password = "Telegram";
            m_database = "CheckTelegram";
            m_host = "mydbtelegramcheck.cswjqwlmeojz.us-west-2.rds.amazonaws.com";
        }
        /// <summary>
        /// Добавление нового пользователя в БД
        /// </summary>
        /// <param name="id_user"></param>
        /// <param name="first_name"></param>
        /// <param name="last_name"></param>
        /// <param name="user_name"></param>
        public void AddUsers(int id_user, string first_name, string last_name, string user_name, string fns_login, string fns_password)
        {
            try
            {
                MySQL_Insert(@"INSERT INTO CheckTelegram.users (ID_users, First_name, Last_name, User_name, FNS_login, FNS_password) 
                             VALUES ('" + id_user + "', '" + first_name + "','" + last_name + "', '" + user_name + "', '" + fns_login + "', '" + fns_password + "');");
            }
            catch 
            {
                UpdateUsers(fns_login, fns_password, id_user);
            };
        }
        public void UpdateUsers(string fns_login, string fns_password, int id_user)
        {
            try
            {
                MySQL_Insert(@"UPDATE CheckTelegram.users SET FNS_login='" + fns_login + "', FNS_password='" + fns_password + "' WHERE ID_users='" + id_user + "';");
            }catch { };
        }

        public void AccountFNS(int id_user)
        {
            try 
            {
                MySQL_SelectAccountFNS(@"SELECT FNS_login, FNS_password FROM CheckTelegram.users where ID_users = '" + id_user + "'");
            }
            catch
            {

            };
        }

        /// <summary>
        /// Добавление нового магазина в БД
        /// </summary>
        /// <param name="store_name"></param>
        /// <param name="id_user"></param>
        public void AddStore(string store_name, string address, string inn)
        {
            
            //string store_name_new = store_name.Replace("'", " ").Trim();
           // string inn_new = inn.Replace("\"", " ").Trim();
            try
            {
                MySQL_Insert("INSERT INTO CheckTelegram.store (INN, Store_name, Address) VALUES ('" + Convert.ToInt64(inn) + "','" + store_name + "', '" + address + "');");
            } catch { }
        }
        /// <summary>
        /// Добавление нового чека в БД
        /// </summary>
        /// <param name="id_check"></param>
        /// <param name="store_name"></param>
        /// <param name="address"></param>
        /// <param name="date_time"></param>
        public void AddCheck(int id_users, Check check)
        {
           // string store_name_new = check.Document.Receipt.User.Replace("'", " ").Trim();
            try
            {
                MySQL_Insert(@"INSERT INTO CheckTelegram.check (ID_check, ID_Users, Store_Name, Address, DateTime, Operator, CheckNumber, SumTotal, SumTotalNds, RN_KKT, NDS18, INN_Store, FD, FP, FN) 
                VALUES ('" + check.Document.Receipt.RequestNumber + "', '" + id_users + "' ,'"
                          + check.Document.Receipt.User + "', '" + check.Document.Receipt.RetailPlaceAddress + "', '" 
                          + check.Document.Receipt.DateTime + "', '"+ check.Document.Receipt.Operator +"', '"+check.Document.Receipt.RequestNumber +"', '"+ check.Document.Receipt.TotalSum/100 +"', '"
                          + check.Document.Receipt.Nds10/100 +"', '"+ check.Document.Receipt.KktRegId +"', '"
                          + check.Document.Receipt.Nds18/100 + "', '"+ Convert.ToInt64(check.Document.Receipt.UserInn)+"' , '" + check.Document.Receipt.FiscalDocumentNumber + "', '" 
                          + check.Document.Receipt.FiscalSign + "', '" + check.Document.Receipt.FiscalDriveNumber + "');");
                
               AddProduct(check);
            } catch { }
        }
        /// <summary>
        /// Добавление продуктов в БД
        /// </summary>
        /// <param name="check"></param>
        public void AddProduct(Check check)
        {
           // int count = MaxProducts();
            foreach (var item in check.Document.Receipt.Items)
            {
                double sum = Convert.ToDouble(item.Sum) / 100;
                double price = Convert.ToDouble(item.Price) / 100;
                double nds10 = Convert.ToDouble(item.Nds10) / 100;
                double nds18 = Convert.ToDouble(item.Nds18) / 100;
               // int random = new Random(DateTime.Now.Millisecond).Next(1000);
                
                try
                {
                    MySQL_Insert(@"INSERT INTO CheckTelegram.products (ID_check, Product_name, Product_sum, Product_quantity, Product_NDS, Product_NDS18, Product_Price) 
                    VALUES ('" + check.Document.Receipt.RequestNumber + "', '" + item.Name + "', '" + sum + "', '" + item.Quantity + "', '" + nds10 + "', '" + nds18 + "', '" + price + "');");

                } catch { }
               
            }
        }


        public List<string> ID_Users()
        {
            try
            {
                return MySQL_Select_Users(@"SELECT ID_users FROM CheckTelegram.users;");
            }
            catch
            {
                return null;
            }
        }

        public int MaxProducts()
        {
            try
            {
                return MySQL_Max(@"SELECT Max(pr.ID) FROM CheckTelegram.products As pr");
            }
            catch 
            {
                return 0;
            }
        }
        public int MySQL_Max(string command_text)
        {
            MySqlConnection my_connection = new MySqlConnection("Database=" + m_database + ";Data Source=" + m_host + ";User Id=" + m_user_id + ";Password=" + m_password + ";CharSet=utf8;");
            MySqlCommand myCommand = new MySqlCommand(command_text, my_connection);
            my_connection.Open(); //Устанавливаем соединение с базой данных.
            MySqlDataReader MyDataReader = myCommand.ExecuteReader();
            int max = 0;
            while (MyDataReader.Read())
            {
                max = MyDataReader.GetInt32(0);
            }
            MyDataReader.Close();
            my_connection.Close();
            return max;
        }
        public void MySQL_Insert(string command_text)
        {
            MySqlConnection my_connection = new MySqlConnection("Database=" + m_database + ";Data Source=" + m_host + ";User Id=" + m_user_id + ";Password=" + m_password + ";CharSet=utf8;");
            MySqlCommand myCommand = new MySqlCommand(command_text, my_connection);
            my_connection.Open(); //Устанавливаем соединение с базой данных.
            MySqlDataReader MyDataReader = myCommand.ExecuteReader();
            while (MyDataReader.Read())
            {
               
            }
            MyDataReader.Close();
            my_connection.Close();
        }
        public void MySQL_SelectExampel(string command_text)
        {

            MySqlConnection my_connection = new MySqlConnection("Database=" + m_database + ";Data Source=" + m_host + ";User Id=" + m_user_id + ";Password=" + m_password + ";CharSet=utf8;");
            MySqlCommand myCommand = new MySqlCommand(command_text, my_connection);
            //my_connection.CharacterSet = "utf8";

            my_connection.Open(); //Устанавливаем соединение с базой данных.
            MySqlDataReader MyDataReader = myCommand.ExecuteReader();
            string id_user = null, last_name = null, first_name = null;
            string check_number = null, product = null, sum = null;
            while (MyDataReader.Read())
            {
                id_user = MyDataReader.GetString(0);
                last_name = MyDataReader.GetString(1);
                first_name = MyDataReader.GetString(2);
                 product = MyDataReader.GetString(3);
                 check_number = MyDataReader.GetString(4);
                sum = MyDataReader.GetString(5);
                Console.WriteLine("{0} {1} {2} {3} {4,7} {5,6}", id_user, last_name, first_name, check_number, product, sum);

            }
           
            MyDataReader.Close();
            my_connection.Close();
        }
        public void MySQL_SelectAccountFNS(string command_text)
        {

            MySqlConnection my_connection = new MySqlConnection("Database=" + m_database + ";Data Source=" + m_host + ";User Id=" + m_user_id + ";Password=" + m_password + ";CharSet=utf8;");
            MySqlCommand myCommand = new MySqlCommand(command_text, my_connection);
            my_connection.Open(); //Устанавливаем соединение с базой данных.
            MySqlDataReader MyDataReader = myCommand.ExecuteReader();
            while (MyDataReader.Read())
            {
                m_fns_login = MyDataReader.GetString(0);
                m_fns_password = MyDataReader.GetString(1);
            }

            MyDataReader.Close();
            my_connection.Close();
        }


        public List<string> MySQL_Select_Users(string command_text)
        {
            List<string> users = new List<string>();
            MySqlConnection my_connection = new MySqlConnection("Database=" + m_database + ";Data Source=" + m_host + ";User Id=" + m_user_id + ";Password=" + m_password + ";CharSet=utf8;");
            MySqlCommand myCommand = new MySqlCommand(command_text, my_connection);
            my_connection.Open(); //Устанавливаем соединение с базой данных.
            MySqlDataReader MyDataReader = myCommand.ExecuteReader();
            while (MyDataReader.Read())
            {
                users.Add(MyDataReader.GetString(0));
            }
            MyDataReader.Close();
            my_connection.Close();
            return users;
        }


        /// <summary>
        /// Получение данных о чеке
        /// </summary>
        /// <param name="command_text"></param>
        /// <returns> Запись данных в список </returns>
        public List<CheckProduct> ResultCheck(string command_text)
        {
            MySqlConnection my_connection = new MySqlConnection("Database=" + m_database + ";Data Source=" + m_host + ";User Id=" + m_user_id + ";Password=" + m_password + ";CharSet=utf8;");
            MySqlCommand myCommand = new MySqlCommand(command_text, my_connection);
            my_connection.Open(); //Устанавливаем соединение с базой данных.
            MySqlDataReader MyDataReader = myCommand.ExecuteReader();
            List<CheckProduct> check_product = new List<CheckProduct>();
            while (MyDataReader.Read())
            {
                check_product.Add(new CheckProduct(MyDataReader.GetString(0), MyDataReader.GetString(2), 
                                                   MyDataReader.GetString(1),MyDataReader.GetString(3)));
            }
            MyDataReader.Close();
            my_connection.Close();
            return check_product;
        }


        public List<ProductStandart> GetProductsStandart(string command_text)
        {
            MySqlConnection my_connection = new MySqlConnection("Database=" + "mydb" + ";Data Source=" + m_host + ";User Id=" + m_user_id + ";Password=" + m_password + ";CharSet=utf8;");
            MySqlCommand myCommand = new MySqlCommand(command_text, my_connection);
            my_connection.Open(); //Устанавливаем соединение с базой данных.
            MySqlDataReader MyDataReader = myCommand.ExecuteReader();
            List<ProductStandart> product_standart = new List<ProductStandart>();
            while (MyDataReader.Read())
            {
                product_standart.Add(new ProductStandart(MyDataReader.GetString(0), MyDataReader.GetString(2),
                                                      MyDataReader.GetString(1)));
            }
            MyDataReader.Close();
            my_connection.Close();
            return product_standart;
        }

        public string FNS_Login
        {
            get { return m_fns_login; }
        }
        public string FNS_Password
        {
            get { return m_fns_password; }
        }

        private string m_fns_login;
        private string m_fns_password;
    }
}

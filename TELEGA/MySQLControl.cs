﻿using System;
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
            m_user_id = "b4a5125e0c43c3";
            m_password = "25a56a14";
            m_database = "ibmsl_1873546bc5817409ce81";
            m_host = "eu-cdbr-sl-lhr-01.cleardb.net";
        }
        /// <summary>
        /// Добавление нового пользователя в БД
        /// </summary>
        /// <param name="id_user"></param>
        /// <param name="first_name"></param>
        /// <param name="last_name"></param>
        /// <param name="user_name"></param>
        public void AddUsers(int id_user, string first_name, string last_name, string user_name)
        {
            try
            {
                MySQL_Insert(@"INSERT INTO ibmsl_1873546bc5817409ce81.users (ID_users, First_name, Last_name, User_name) 
                             VALUES ('" + id_user + "', '" + first_name + "','" + last_name + "', '" + user_name + "');");
            }catch { };
        }
        /// <summary>
        /// Добавление нового магазина в БД
        /// </summary>
        /// <param name="store_name"></param>
        /// <param name="id_user"></param>
        public void AddStore(string store_name, int id_user)
        {
            try
            {
                MySQL_Insert("INSERT INTO ibmsl_1873546bc5817409ce81.store (Store_name, ID_users) VALUES ('" + store_name + "', '" + id_user + "');");
            } catch { }
        }
        /// <summary>
        /// Добавление нового чека в БД
        /// </summary>
        /// <param name="id_check"></param>
        /// <param name="store_name"></param>
        /// <param name="address"></param>
        /// <param name="date_time"></param>
        public void AddCheck(int id_check, string store_name, string address, string date_time, Check check)
        {
            try
            {
                MySQL_Insert(@"INSERT INTO ibmsl_1873546bc5817409ce81.check (ID_check, Store_name, Address, DateTime) 
                VALUES ('" + id_check + "', '" + store_name + "', '" + address + "', '" + date_time + "');");
                
                AddProduct(check);
            } catch { }
        }
        /// <summary>
        /// Добавление продуктов в БД
        /// </summary>
        /// <param name="check"></param>
        public void AddProduct(Check check)
        {
            int count = MaxProducts();
            foreach (var item in check.Document.Receipt.Items)
            {
                double sum = Convert.ToDouble(item.Sum) / 100;
               // int random = new Random(DateTime.Now.Millisecond).Next(1000);
                
                try
                {
                    MySQL_Insert(@"INSERT INTO ibmsl_1873546bc5817409ce81.products (ID, ID_check, Product_name, Product_sum, Product_quantity) 
                    VALUES ('" + count + "', '" + check.Document.Receipt.ShiftNumber + "', '" + item.Name + "', '" + sum + "', '" + item.Quantity + "');");
                    count++;
                } catch { }
               
            }
        }

        public int MaxProducts()
        {
            try
            {
               return MySQL_Max(@"SELECT Max(pr.ID) FROM ibmsl_1873546bc5817409ce81.products As pr");
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
    }
}

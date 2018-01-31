using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace TELEGA
{
    class MySQLControl
    {
       // private string m_connect = "Database=ibmx_2f92d9c8849688d;Data Source=eu-cdbr-sl-lhr-01.cleardb.net;User Id=b92be25f9296cd;Password=c92ecc39";
        private string m_user_id = "b92be25f9296cd";
        private string m_password = "c92ecc39";
        private string m_database = "ibmx_2f92d9c8849688d";
        private string m_host = "eu-cdbr-sl-lhr-01.cleardb.net";

        public void MySQL_Query(string command_text)
        {
            
            MySqlConnection my_connection = new MySqlConnection("Database=" + m_database + ";Data Source=" + m_host + ";User Id=" + m_user_id + ";Password=" + m_password + ";CharSet=utf8;");
            MySqlCommand myCommand = new MySqlCommand(command_text, my_connection);
            //my_connection.CharacterSet = "utf8";
            
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
        public void MySQL_DeleteExampel(string command_text)
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

        public List<String>  MySQLSelect(string command_text)
        {
            MySqlConnection my_connection = new MySqlConnection("Database=" + m_database + ";Data Source=" + m_host + ";User Id=" + m_user_id + ";Password=" + m_password + ";CharSet=utf8;");
            MySqlCommand myCommand = new MySqlCommand(command_text, my_connection);
            my_connection.Open(); //Устанавливаем соединение с базой данных.
            MySqlDataReader MyDataReader = myCommand.ExecuteReader();
            List<string> tabel = new List<string>();
            int index = 0;
            while (MyDataReader.Read())
            {
                for (int k = 0; k < MyDataReader.GetString(index).LongCount(); k++)
                {
                    tabel.Add(MyDataReader.GetString(k));
                }
                index++;
            }
            //TODO: записать правильно таблицу (имя и данные)
            MyDataReader.Close();
            my_connection.Close();
            return tabel;
        }
    }
}

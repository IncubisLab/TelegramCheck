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
            MySqlConnection my_connection = new MySqlConnection("Database=" + m_database + ";Data Source=" + m_host + ";User Id=" + m_user_id + ";Password=" + m_password);
            MySqlCommand myCommand = new MySqlCommand(command_text, my_connection);
            my_connection.Open(); //Устанавливаем соединение с базой данных.
            MySqlDataReader MyDataReader = myCommand.ExecuteReader();
            while (MyDataReader.Read())
            {
               
            }
            MyDataReader.Close();
            my_connection.Close();
        }
    }
}

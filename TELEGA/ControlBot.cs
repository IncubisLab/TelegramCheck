using System;
using Telegram.Bot;

namespace TELEGA
{
    class ControlBot
    {
        public void Run_Qurey_Console(MySQLControl my_sql_control, TelegramBotClient bot)
        {
            Console.WriteLine("Введите команду/");
            string text = Console.ReadLine();
            if (text == "Select")
            {
                Console.WriteLine("Введите номер чека");
                string num = Console.ReadLine();
                try
                {
                    my_sql_control.MySQL_SelectExampel("Select * From users, products Where products.id_check = '" + num + "'");
                }
                catch { }
            }
            if (text == "Delete")
            {
                Console.WriteLine("Введите номер чека");
                string num = Console.ReadLine();
                try
                {
                    my_sql_control.MySQL_SelectExampel("DELETE FROM ibmx_2f92d9c8849688d.products WHERE id_check='" + num + "'");
                    my_sql_control.MySQL_SelectExampel("DELETE FROM ibmx_2f92d9c8849688d.check WHERE chek_number='" + num + "'");

                }
                catch { }
            }
            if (text == "Product")
            {
                Console.WriteLine("Введите имя продукта ");
                string product = Console.ReadLine();
                Data_Analysis data_analysis = new Data_Analysis(my_sql_control);
                foreach (var check in data_analysis.Parser_Check(product))
                {
                    Console.WriteLine("{0}   {1} руб.  кол х {2} {3}", check.Product_Name, check.Product_Sum, check.Product_Quantity, check.Store_Name);
                }
            }
            if (text == "Telegraph")
            {
                TelegraphAPI telegraphAPI = new TelegraphAPI();
                telegraphAPI.GetPageList();
            }    

            else if (text == "Exit") { bot.StopReceiving(); Environment.Exit(0); }
            Run_Qurey_Console(my_sql_control, bot);
        }
    }
}

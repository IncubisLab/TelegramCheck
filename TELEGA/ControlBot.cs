using System;
using Telegram.Bot;
using System.Threading;

namespace TELEGA
{
    class ControlBot
    {
        /// <summary>
        /// Управление ботом из консоли
        /// </summary>
        /// <param name="my_sql_control"></param>
        /// <param name="bot"></param>
        public async void Run_Qurey_Console(MySQLControl my_sql_control, TelegramBotClient bot)
        {
            Console.WriteLine("Введите команду/");
            switch (Console.ReadLine())
            {
                case "Select":
                    {
                        Console.WriteLine("Введите номер чека");
                        string num = Console.ReadLine();
                        try
                        {
                            my_sql_control.MySQL_SelectExampel("Select * From users, products Where products.id_check = '" + num + "'");
                        }
                        catch { } break;
                    }
                case "Delete":
                    {
                        Console.WriteLine("Введите номер чека");
                        string num = Console.ReadLine();
                        try
                        {
                            my_sql_control.MySQL_SelectExampel("DELETE FROM ibmx_2f92d9c8849688d.products WHERE id_check='" + num + "'");
                            my_sql_control.MySQL_SelectExampel("DELETE FROM ibmx_2f92d9c8849688d.check WHERE chek_number='" + num + "'");

                        }
                        catch { } break;
                    }
                case "Product":
                    {
                        Console.WriteLine("Введите имя продукта ");
                        string product = Console.ReadLine();
                        Data_Analysis data_analysis = new Data_Analysis(my_sql_control);
                        foreach (var check in data_analysis.Parser_Check(product))
                        {
                            Console.WriteLine("{0}   {1} руб.  кол х {2} {3}", check.Product_Name, check.Product_Sum, 
                                                                               check.Product_Quantity, check.Store_Name);
                        } break;
                    }
                case "Telegraph":
                    {
                        TelegraphAPI telegraphAPI = new TelegraphAPI();
                        //telegraphAPI.GetPageList();
                       Console.WriteLine(telegraphAPI.GetPageList1());
                        break;
                    }
                case "Max":
                    {
                        Console.WriteLine("Продуктов в БД всего {0}", my_sql_control.MaxProducts());
                        break;
                    }
                case "Stop":
                    {
                      
                        await bot.SendTextMessageAsync(397600634, "Бот остановлен по техническим причинам!");
                        Console.WriteLine("Бот отсановлен!");
                        Thread.Sleep(100);
                        break;
                    }
                case "Exit":
                    {
                        bot.StopReceiving(); Environment.Exit(0);
                        break;
                    }
            } 
            Run_Qurey_Console(my_sql_control, bot);
        }
    }
}

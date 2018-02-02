using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using FSNCheck;
using FSNCheck.Data;
using Newtonsoft.Json;
using TELEGA;


namespace Telegram.Bot.Examples.Echo
{
    public static class Program
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("513572219:AAFnhp76wp-AMslfGNF7RVZcqmm3UU32kvs");
        private static MySQLControl my_sql_control = new MySQLControl();
        private static ControlBot controlBot = new ControlBot();
        public static void Main(string[] args)
        {
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;

            var me = Bot.GetMeAsync().Result;
            Console.Title = me.Username;

            Bot.StartReceiving();
            Console.WriteLine("Start listening for {0}", me.Username);
            Console.ReadKey();
            controlBot.Run_Qurey_Console(my_sql_control, Bot);
           
        }
  
        private static string GET(string Url, string Data)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(Url + "?" + Data);
            System.Net.WebResponse resp = req.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string Out = sr.ReadToEnd();
            sr.Close();
            return Out;
        }
        private static async void ParserQR_Code(string input, Message message)
        {
            string fn = RegularExpressions(input, "fn=(\\d+)");
            string fd = RegularExpressions(input, "i=(\\d+)");
            string fp = RegularExpressions(input, "fp=(\\d+)");

            Checking checking = new Checking("+79817889931", "405381");

            CheckInfo checkInfo = new CheckInfo
            {
                FN = fn,
                FD = fd,
                FS = fp
            };

            var check = checking.GetCheck(checkInfo);
           

            StringBuilder str = new StringBuilder("");
            try
            {
                foreach (var item in check.Document.Receipt.Items)
                {
                    double sum = Convert.ToDouble(item.Sum) / 100;
                    str.Append(string.Format("\n{0} x {2} - {1} руб", item.Name, sum, item.Quantity));
                }
                await Bot.SendTextMessageAsync(message.Chat.Id, str.ToString());
            }
            catch 
            { 
                Console.WriteLine("Ошибка объекта");
            }
            Console.WriteLine("Чек номер {0}", check.Document.Receipt.ShiftNumber);
            my_sql_control.AddStore(check.Document.Receipt.User, (int)message.Chat.Id);
            my_sql_control.AddCheck(check.Document.Receipt.ShiftNumber, check.Document.Receipt.User, "***");
            my_sql_control.AddProduct(check);
//            try
//            {
//                my_sql_control.MySQL_Insert("INSERT INTO ibmsl_1873546bc5817409ce81.store (Store_name, ID_users) VALUES ('" + check.Document.Receipt.User + "', '" + message.Chat.Id + "');");
//            }
//            catch { }
//            try
//            {
//                my_sql_control.MySQL_Insert(@"INSERT INTO ibmsl_1873546bc5817409ce81.check (ID_check, Store_name, Address) 
//                VALUES ('" + check.Document.Receipt.ShiftNumber + "', '" + check.Document.Receipt.User + "', '???');");
//            }
//            catch { }
           // InsertCheck(check.Document.Receipt.ShiftNumber, message.Chat.Id);
           // InsertProduct(check);
//            foreach (var item in check.Document.Receipt.Items)
//            {
//                double sum = Convert.ToDouble(item.Sum) / 100;
//                //Random rnd = new Random();
//                //rnd.Next(1000);
//                //int seed = Convert.ToInt32(DateTime.Now.Millisecond.ToString());
//                int cnr = new Random(DateTime.Now.Millisecond).Next(1000);
//                try
//                {
//                    my_sql_control.MySQL_Insert(@"INSERT INTO ibmsl_1873546bc5817409ce81.products (ID, ID_check, Product_name, Product_sum, Product_quantity) 
//                    VALUES ('" + cnr +"', '" + check.Document.Receipt.ShiftNumber + "', '" + item.Name + "', '" + sum + "', '" + item.Quantity + "');");
//                }
//                catch 
//                {
                    
//                }
//            }
        }
        //private static void InsertCheck(int chek_number, ValueType user_id)
        //{
        //    try
        //    {
        //       // my_sql_control.MySQL_Query("INSERT INTO ibmx_2f92d9c8849688d.check (chek_number, user_id) VALUES ('" + chek_number + "', '" + user_id + "');");
                
        //    }
        //    catch { }
        //}
        //private static void InsertProduct(Check check)
        //{
        //    foreach (var item in check.Document.Receipt.Items)
        //    {
        //        double sum = Convert.ToDouble(item.Sum) / 100;
        //        try
        //        {
        //            my_sql_control.MySQL_Insert("INSERT INTO ibmx_2f92d9c8849688d.products (Product_name, id_check, Sum, Quantity) VALUES ('" + item.Name + "', '" + check.Document.Receipt.ShiftNumber + "', '" + sum + "', '" + item.Quantity + "');");
        //        }
        //        catch { }
        //    }
        //}
        private static async void BotOnPhotoMassage(Message message)
        {
            if (message.Type == MessageType.PhotoMessage)
            {
                var test = await Bot.GetFileAsync(message.Photo[message.Photo.Count() - 1].FileId);
                var image = Bitmap.FromStream(test.FileStream);
                string file_name = test.FilePath;
                image.Save(test.FilePath);

                string get =
                    String.Format(
                        "https://api.qrserver.com/v1/read-qr-code/?fileurl=https://api.telegram.org/file/bot513572219:AAFnhp76wp-AMslfGNF7RVZcqmm3UU32kvs/{0}",
                        file_name);
                string data = GET(get, "");
                Console.WriteLine("Пользователь: {0} загрузил фото чека",message.Chat.Username);
//                try
//                {
//                    my_sql_control.MySQL_Insert(@"INSERT INTO ibmsl_1873546bc5817409ce81.users (ID_users, First_name, Last_name, User_name) 
//                                               VALUES ('"+ message.Chat.Id +"', '"+ message.Chat.FirstName +@"', 
//                                                         '"+ message.Chat.LastName +"', '"+ message.Chat.Username +"');");
//                }
//                catch { };
                my_sql_control.AddUsers((int)message.Chat.Id, message.Chat.FirstName, message.Chat.LastName, message.Chat.Username);
                await Bot.SendTextMessageAsync(message.Chat.Id, data);
                ScanData scanData = JsonConvert.DeserializeObject<ScanData>(data.Replace('[', ' ').Replace(']', ' '));
                if (scanData.Symbol.Error == null)
                {
                    ParserQR_Code(scanData.Symbol.Data, message);
                }
               
            }
        }
        private static void BotOnTextMessage(Message message)
        {
           
          

            if (message.Type == MessageType.TextMessage && (message.Text != null))
            {
                Console.WriteLine("Пользователь: {0} загрузил данные чека", message.Chat.Username);
//                try
//                {
//                    my_sql_control.MySQL_Insert(@"INSERT INTO ibmsl_1873546bc5817409ce81.users (ID_users, First_name, Last_name, User_name) 
//                                               VALUES ('" + message.Chat.Id + "', '" + message.Chat.FirstName + @"', 
//                                                       '" + message.Chat.LastName + "', '" + message.Chat.Username + "');");
//                }
//                catch { };
                my_sql_control.AddUsers((int)message.Chat.Id, message.Chat.FirstName, message.Chat.LastName, message.Chat.Username);
                ParserQR_Code(message.Text, message);
            }
            //switch (message.Text.Split(' ').First())
            //{
            //    case "/info":
            //        {
            //            await Bot.SendTextMessageAsync(message.Chat.Id, "Who or Where are you?");
            //            break;
            //        }

            //    case "/QR_Code":
            //        {
            //            //message.Text = null;
            //            await Bot.SendTextMessageAsync(message.Chat.Id, "Введите текс с QR-кодом!");   

                        //Data_Analysis data_analysis = new Data_Analysis(my_sql_control);
                        //data_analysis.Search_Product_Check(message.Text);
//                        break;
//                    }
//                default:
//                    const string usage = @"Usage:
///info   - Информация о продукте
///QR_Code - Инфо по QR коду";

//                    await Bot.SendTextMessageAsync(
//                        message.Chat.Id,
//                        usage,
//                        replyMarkup: new ReplyKeyboardRemove());
//                    break;
      
//            }
        }
        private static void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            BotOnPhotoMassage(message);
            BotOnTextMessage(message);
        }
        private static string RegularExpressions(string input, string pattern)
        {
            Regex regex = new Regex(pattern);
            Match match = regex.Match(input);
            return match.Groups[1].Value;
        }
    }
}
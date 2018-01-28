using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;
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

        public static void Main(string[] args)
        {
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;

            var me = Bot.GetMeAsync().Result;
            Console.Title = me.Username;

            Bot.StartReceiving();
            Console.WriteLine(string.Format("Start listening for {0}",me.Username));
            Console.ReadKey();
            Bot.StopReceiving();
        }

        private static string GET(string Url, string Data)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(Url + "?" + Data);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.Stream stream = resp.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(stream);
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
            { Console.WriteLine("Ошибка объекта");
              ParserQR_Code( input,  message);
            }
            try
            {
                my_sql_control.MySQL_Query("INSERT INTO ibmx_2f92d9c8849688d.check (chek_number, user_id) VALUES ('" + check.Document.Receipt.ShiftNumber + "', '" + message.Chat.Id + "');");
            }
            catch { }
            
                foreach (var item in check.Document.Receipt.Items)
                {
                    double sum = Convert.ToDouble(item.Sum) / 100;
                    try
                    {
                        my_sql_control.MySQL_Query("INSERT INTO ibmx_2f92d9c8849688d.products (Product_name, id_check, Sum, Quantity) VALUES ('" + item.Name + "', '" + check.Document.Receipt.ShiftNumber + "', '" + sum + "', '" + item.Quantity + "');");
                    }
                    catch { }
                }
           
           
        }
        private static async void BotOnPhotoMassage(Message message)
        {
            if (message.Type == MessageType.PhotoMessage)
            {
                var test = await Bot.GetFileAsync(message.Photo[message.Photo.Count() - 1].FileId);
                var image = Bitmap.FromStream(test.FileStream);
                int num = message.Photo.Count() - 1;
                string file_name = test.FilePath;
                image.Save(test.FilePath);

                string get =
                    String.Format(
                        "https://api.qrserver.com/v1/read-qr-code/?fileurl=https://api.telegram.org/file/bot513572219:AAFnhp76wp-AMslfGNF7RVZcqmm3UU32kvs/{0}",
                        file_name);
                string data = GET(get, "");
                Console.WriteLine("Имя пользователя: {0}",message.Chat.FirstName);
                try
                {
                    my_sql_control.MySQL_Query("INSERT INTO ibmx_2f92d9c8849688d.users (idUsers, LastName, FirstName) VALUES ('" + message.Chat.Id + "', '" + message.Chat.LastName + "', '" + message.Chat.FirstName + "');");
                }
                catch { };
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
            if (message.Type == MessageType.TextMessage)
            {
                ParserQR_Code(message.Text, message);
            }
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
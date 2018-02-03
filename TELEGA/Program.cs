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
using Newtonsoft.Json;
using TELEGA;
using Telegram.Bot.Types.ReplyMarkups;


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
            Checking checking = new Checking("+79817889931", "405381");
            CheckInfo checkInfo = new CheckInfo
            {
                FN = RegularExpressions(input, "fn=(\\d+)"),
                FD = RegularExpressions(input, "i=(\\d+)"),
                FS = RegularExpressions(input, "fp=(\\d+)")
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
            my_sql_control.AddCheck(check.Document.Receipt.ShiftNumber, check.Document.Receipt.User, 
                                    check.Document.Receipt.RetailPlaceAddress, check.Document.Receipt.DateTime);
            my_sql_control.AddProduct(check);
        }
        private static async void BotOnPhotoMassage(Message message)
        {
            if (message.Type == MessageType.PhotoMessage)
            {
                var test = await Bot.GetFileAsync(message.Photo[message.Photo.Count() - 1].FileId);
                var image = Bitmap.FromStream(test.FileStream);
                string file_name = test.FilePath;
                image.Save(test.FilePath);

                string get = String.Format("https://api.qrserver.com/v1/read-qr-code/?fileurl=https://api.telegram.org/file/bot513572219:AAFnhp76wp-AMslfGNF7RVZcqmm3UU32kvs/{0}",file_name);
                string data = GET(get, "");
                Console.WriteLine("Пользователь: {0} загрузил фото чека",message.Chat.Username);
                my_sql_control.AddUsers((int)message.Chat.Id, message.Chat.FirstName, message.Chat.LastName, message.Chat.Username);
                await Bot.SendTextMessageAsync(message.Chat.Id, data);
                ScanData scanData = JsonConvert.DeserializeObject<ScanData>(data.Replace('[', ' ').Replace(']', ' '));
                if (scanData.Symbol.Error == null)
                {
                    ParserQR_Code(scanData.Symbol.Data, message);
                }
            }
        }
        private static async void BotOnTextMessage(Message message)
        {
            if (message == null || message.Type != MessageType.TextMessage) return;
            switch (message.Text.Split(' ').First())
            {
                case "/info":
                    {
                        if (message.Type == MessageType.TextMessage && (message.Text != null))
                        {
                            Console.WriteLine("Пользователь: {0} загрузил данные чека", message.Chat.Username);
                            my_sql_control.AddUsers((int)message.Chat.Id, message.Chat.FirstName, message.Chat.LastName, message.Chat.Username);
                            ParserQR_Code(message.Text, message);
                        }
                        break;
                    }

                case "/Product":
                    {
                        string product = message.Text.Remove(0, message.Text.IndexOf(' ') + 1);
                        Data_Analysis data_analysis = new Data_Analysis(my_sql_control);
                       
                        foreach (var check in data_analysis.Parser_Check(product))
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, string.Format("{0}   {1} руб.   {2}", check.Product_Name, 
                                check.Product_Sum, check.Store_Name));
                        }
                        break;
                    }
                case "/Telegraph":
                    {
                        string product = message.Text.Remove(0, message.Text.IndexOf(' ') + 1);
                        TelegraphAPI control_telegraph = new TelegraphAPI();
                         Data_Analysis data_analysis = new Data_Analysis(my_sql_control);
                         control_telegraph.AddListNodeElementNew(data_analysis.Parser_Check(product));
                         control_telegraph.EditPage(product);
                         await Bot.SendTextMessageAsync(message.Chat.Id, "http://telegra.ph//Sample-Page-02-03-16");
                         //http://telegra.ph//Sample-Page1-02-03-2
                        break;
                    }
                default:
                    const string usage = @"Usage:
/info   - Информация о продукте
/Product - Инфо по продукту
/Telegraph - Вывести информацию в телеграф";

                    await Bot.SendTextMessageAsync(message.Chat.Id,usage, replyMarkup: new ReplyKeyboardRemove());
                    break;
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
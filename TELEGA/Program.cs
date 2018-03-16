using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using FSNCheck;
using Newtonsoft.Json;
using TELEGA;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Threading;

namespace Telegram.Bot.Examples.Echo
{
    public static class Program
    {
        //private static readonly TelegramBotClient Bot = new TelegramBotClient("513572219:AAFnhp76wp-AMslfGNF7RVZcqmm3UU32kvs");
        private static readonly TelegramBotClient Bot = new TelegramBotClient("546895443:AAGpKxhnQCkQCpDRqOSF2_3A72X_hmo8OtI");
        private static MySQLControl my_sql_control = new MySQLControl();
        private static ControlBot controlBot = new ControlBot();
        public static void Main(string[] args)
        {
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnCallbackQuery += BotOnCallbackQuery;
            var me = Bot.GetMeAsync().Result;
            Console.Title = "Telegram Бот "+ me.Username +" запущен!";
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
            my_sql_control.AccountFNS((int)message.Chat.Id);

           // Checking checking = new Checking(my_sql_control.FNS_Login, my_sql_control.FNS_Password);
            Checking checking = new Checking("+79817889931", "405381");
            CheckInfo checkInfo = new CheckInfo
            {
                FN = RegularExpressions(input, "fn=(\\d+)"),
                FD = RegularExpressions(input, "i=(\\d+)"),
                FS = RegularExpressions(input, "fp=(\\d+)")
            };

            var check = checking.GetCheck(checkInfo);
            if (check == null)
            {
                Thread.Sleep(100);
               var check1 = checking.GetCheck(checkInfo);
                if (check1 == null)
                {
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Ответ сервера ФНС не получен!\nВозможно чек невалидный!");
                    return;
                }
                
            }
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
            Thread.Sleep(10);
            my_sql_control.AddStore(check.Document.Receipt.User, (int)message.Chat.Id, check.Document.Receipt.UserInn);
            Thread.Sleep(10);
            my_sql_control.AddCheck(check.Document.Receipt.ShiftNumber,(int)message.Chat.Id, check.Document.Receipt.User, 
                                    check.Document.Receipt.RetailPlaceAddress, check.Document.Receipt.DateTime, check);
           // Thread.Sleep(100); // пауза для закрытия БД
            Data_Analysis data_analysis = new Data_Analysis(my_sql_control);
            TelegraphAPI control_telegraph = new TelegraphAPI();
            List<CheckProduct> products = data_analysis.Search_Product(check);
            if (products == null) 
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, "Достигнуто Max подключений!\n Повторите попытку позже!");
                return; 
            }
            if (products.Count > 0)
            {
                control_telegraph.AddListNodeElementNew(products);
                control_telegraph.EditPage("по чеку");
                await Bot.SendTextMessageAsync(message.Chat.Id, "http://telegra.ph//Sample-Page-02-03-16");
            }
            else
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, "Данного продукта нет в БД");
            }
        }
        private static async void BotOnPhotoMassage(Message message)
        {
            if (message.Type == MessageType.PhotoMessage)
            {
                var test = await Bot.GetFileAsync(message.Photo[message.Photo.Count() - 1].FileId);
                var image = Bitmap.FromStream(test.FileStream);
                string file_name = test.FilePath;
                image.Save(test.FilePath);

              //  string get = String.Format("https://api.qrserver.com/v1/read-qr-code/?fileurl=https://api.telegram.org/file/bot513572219:AAFnhp76wp-AMslfGNF7RVZcqmm3UU32kvs/{0}",file_name);
                string get = String.Format("https://api.qrserver.com/v1/read-qr-code/?fileurl=https://api.telegram.org/file/bot546895443:AAGpKxhnQCkQCpDRqOSF2_3A72X_hmo8OtI/{0}", file_name);
                string data = GET(get, "");
                Console.WriteLine("Пользователь: {0} загрузил фото чека",message.Chat.Username);
              
                await Bot.SendTextMessageAsync(message.Chat.Id, "Данные по QR коду!");
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
                          //  my_sql_control.AddUsers((int)message.Chat.Id, message.Chat.FirstName, message.Chat.LastName, message.Chat.Username);
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
                         List<CheckProduct> products = data_analysis.Parser_Check(product);
                         if (products.Count > 0)
                         {
                             control_telegraph.AddListNodeElementNew(products);
                             control_telegraph.EditPage(product);
                             await Bot.SendTextMessageAsync(message.Chat.Id, "http://telegra.ph//Sample-Page-02-03-16");
                         }
                         else
                         {
                             await Bot.SendTextMessageAsync(message.Chat.Id, "Данного продукта нет в БД");
                         }
                         //http://telegra.ph//Sample-Page1-02-03-2
                        break;
                    }
                case "/Stop":
                    {
                        //foreach (var user in my_sql_control.ID_Users())
                        //{
                        //    await Bot.SendTextMessageAsync(user, "Бот остановлен по техническим причинам!");
                        //}
                        await Bot.SendTextMessageAsync(message.Chat.Id, "Пройди процес регестрации под своим аккаунтом ФНС!"+ Environment.NewLine + "/start");
                        //Бот обновлен, для дальнейшего использования бота необходима авторизоваться под своим аккаунтов ФНС! Используете /start
                        Console.WriteLine("Бот отсановлен!");
                        break;
                    }
                case "/start":
                    {
                        var keyboard = new InlineKeyboardMarkup(new[] {
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData("Регистрация"),
                                InlineKeyboardButton.WithCallbackData("Авторизация"),
                            }
                         });
                        await Bot.SendTextMessageAsync(message.Chat.Id, "Для работы бота необходимо авторизоваться или зарегестрироваться в системе ФНС!", ParseMode.Default, false, false, 0, keyboard);
                        break;
                    }
                case "/authorization":
                    {
                        ParserAuthorization(message);
                        break;
                    }
                case "/reg":
                    {
                        Checking chek = new Checking();
                        chek.RegistrationFNS("+79006226297", "ande75357@gmail.com", "Andrey");
                        break;
                    }
                default:
                    const string usage = @"Использование:
/info   - Информация о продукте
/Product - Инфо по продукту
/Telegraph - Вывести информацию в телеграф
В случае ошибки писать https://t.me/joinchat/F7LnehCPgzY5b8sfHwN6KA";


                    await Bot.SendTextMessageAsync(message.Chat.Id,usage, replyMarkup: new ReplyKeyboardRemove());
                    break;
            }
        }
        private static async void ParserAuthorization(Message message)
        {
            string login = "+" + RegularExpressions(message.Text, "login:(\\d+)");
            //await Bot.SendTextMessageAsync(message.Chat.Id, "Ваш логин:" + login);
            string password = RegularExpressions(message.Text, "password:(\\d+)");
          //  await Bot.SendTextMessageAsync(message.Chat.Id, "Ваш пароль:" + password);
            my_sql_control.AddUsers((int)message.Chat.Id, message.Chat.FirstName, message.Chat.LastName, message.Chat.Username, login, password);
            await Bot.SendTextMessageAsync(message.Chat.Id, "Данные записаны!");
         //   my_sql_control.UpdateUsers(login, password, (int)message.Chat.Id);
        }
        private static async void BotOnCallbackQuery(object sender, CallbackQueryEventArgs ev)
        {
            var message_ev = ev.CallbackQuery.Message;
            if (ev.CallbackQuery.Data == "Регистрация")
            {
                await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Зарегестрируйтесь в системе ФНС", false);
                var keyboard = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup
                {
                    Keyboard = new[] {
                                        new[] // row 1
                                               {
                                                   new KeyboardButton("Телефон") { RequestContact = true }
                                               },
                                            },
                    ResizeKeyboard = true
                };
                await Bot.SendTextMessageAsync(message_ev.Chat.Id, "Введите номер телефона", ParseMode.Default, false, false, 0, keyboard);
            }
            if (ev.CallbackQuery.Data == "Авторизация")
            {
                await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Авторизуйтесь в системе ФНС", false);
                await Bot.SendTextMessageAsync(message_ev.Chat.Id, "Введите логин и пароль для авторизации как напримрие!");
                await Bot.SendTextMessageAsync(message_ev.Chat.Id, string.Format("/authorization {0}login:andrey {0}password:xxxxx", Environment.NewLine));
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
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
       // private static readonly TelegramBotClient Bot = new TelegramBotClient("546895443:AAGpKxhnQCkQCpDRqOSF2_3A72X_hmo8OtI");
    //    private static readonly TelegramBotClient Bot = new TelegramBotClient("514039168:AAHZEBIQS9d50PyxSrv53892N_X_tB_ShJA"); //TelegramCheck
    //  private static readonly TelegramBotClient Bot = new TelegramBotClient("594291926:AAGG_UdMmhRIsec8mdT8e3TNRpHN7_fNbfc");
      //private static readonly TelegramBotClient Bot = new TelegramBotClient("594291926:AAEQkWxK_szjmwKaDeL7nGCYZ8zDqr1zD38");
      private static readonly TelegramBotClient Bot = new TelegramBotClient("616639924:AAFXRTmOSY_1tFyEiauA2KBHhl-S8yr7xUo");
      //  private static readonly TelegramBotClient Bot = new TelegramBotClient("575255435:AAEO7i4fqTIszlU4Cnyg3U8VWpqboC_Zx0s");
      //  private static readonly TelegramBotClient Bot = new TelegramBotClient("601147016:AAHsgXnB3GnC7u-BGhtBu7X-gEZs9ca67Y0");
        
        
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
            TelegraphAPI control_telegraph = new TelegraphAPI();
            my_sql_control.AccountFNS((int)message.Chat.Id);

            Checking checking = new Checking(my_sql_control.FNS_Login, my_sql_control.FNS_Password);

            CheckInfo checkInfo = new CheckInfo
            {
                FN = RegularExpressions(input, "fn=(\\d+)"),
                FD = RegularExpressions(input, "i=(\\d+)"),
                FS = RegularExpressions(input, "fp=(\\d+)")
            };

            var check = checking.GetCheck(checkInfo);
            if (check == null)
            {
                Thread.Sleep(500);
               var check1 = checking.GetCheck(checkInfo);
               check = check1;
                if (check1 == null)
                {
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Ответ сервера ФНС не получен!\nВозможно чек невалидный!");
                    return;
                }
                
            }
            StringBuilder str = new StringBuilder("");
            try
            {
                control_telegraph.PrintCheck(check);
                control_telegraph.CreatePage1("Чек №" + check.Document.Receipt.RequestNumber);
                Thread.Sleep(700);
                await Bot.SendTextMessageAsync(message.Chat.Id, "Ваш чек!");
                 
                await Bot.SendTextMessageAsync(message.Chat.Id, control_telegraph.GetPageList1());
                 await Bot.SendTextMessageAsync(message.Chat.Id, "Ожидайте результата сравнения цен!");
            }
            catch
            {
                Console.WriteLine("Ошибка объекта");
            }

            Console.WriteLine("Чек номер {0}", check.Document.Receipt.RequestNumber);
            Thread.Sleep(10);
            my_sql_control.AddStore(check.Document.Receipt.User, check.Document.Receipt.RetailPlaceAddress, check.Document.Receipt.UserInn);
            Thread.Sleep(10);
            my_sql_control.AddCheck((int)message.Chat.Id, check);
            Data_Analysis data_analysis = new Data_Analysis(my_sql_control);
            List<CheckProduct> products = data_analysis.Search_Product(check);
            if (products == null) 
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, "Достигнуто Max подключений!\n Повторите попытку позже!");
                return; 
            }
            if (products.Count > 0)
            {
                control_telegraph.AddListNodeElementNew2(data_analysis.LReportCheck);
               // control_telegraph.EditPage("по чеку №"+ check.Document.Receipt.RequestNumber + " Магазин: " + check.Document.Receipt.User);
                control_telegraph.CreatePage1("Сравнение цен о продуктах по чеку №" + check.Document.Receipt.RequestNumber + " Магазин: " + check.Document.Receipt.User);
                //await Bot.SendTextMessageAsync(message.Chat.Id, "http://telegra.ph//Sample-Page-02-03-16");
                Thread.Sleep(700);
                await Bot.SendTextMessageAsync(message.Chat.Id, control_telegraph.GetPageList1());
            }
            else
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, "Данного продукта нет в БД");
            }

            await Bot.SendTextMessageAsync(message.Chat.Id, "Продукт не распознался?! \nДобавь ключевое слово продукта!\n/keyproduct \"название продукта\"");
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
              //  string get = String.Format("https://api.qrserver.com/v1/read-qr-code/?fileurl=https://api.telegram.org/file/bot546895443:AAGpKxhnQCkQCpDRqOSF2_3A72X_hmo8OtI/{0}", file_name);
            //  string get = String.Format("https://api.qrserver.com/v1/read-qr-code/?fileurl=https://api.telegram.org/file/bot514039168:AAHZEBIQS9d50PyxSrv53892N_X_tB_ShJA/{0}", file_name);
            //    string get = String.Format("https://api.qrserver.com/v1/read-qr-code/?fileurl=https://api.telegram.org/file/bot594291926:AAHJC5OvjWsB11poZkbRVthZ8W_uBMnGRYI/{0}", file_name);
               // string get = String.Format("https://api.qrserver.com/v1/read-qr-code/?fileurl=https://api.telegram.org/file/bot594291926:AAEQkWxK_szjmwKaDeL7nGCYZ8zDqr1zD38/{0}", file_name);
                string get = String.Format("https://api.qrserver.com/v1/read-qr-code/?fileurl=https://api.telegram.org/file/bot616639924:AAFXRTmOSY_1tFyEiauA2KBHhl-S8yr7xUo/{0}", file_name);
                //  string get = String.Format("https://api.qrserver.com/v1/read-qr-code/?fileurl=https://api.telegram.org/file/bot575255435:AAEO7i4fqTIszlU4Cnyg3U8VWpqboC_Zx0s/{0}", file_name); // CheckBot
               // string get = String.Format("https://api.qrserver.com/v1/read-qr-code/?fileurl=https://api.telegram.org/file/bot601147016:AAHsgXnB3GnC7u-BGhtBu7X-gEZs9ca67Y0/{0}", file_name);
                
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
            //await Bot.SendTextMessageAsync("397600634", "Данного продукта нет в БД");
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
                case "/keyproduct":
                    {
                        AddKeyProduct(message);
                        break;
                    }
                case "/reg":
                    {
                        Checking chek = new Checking();
                        chek.RegistrationFNS("+79006226297", "ande75357@gmail.com", "Andrey");
                        break;
                    }
                default:
//                    const string usage = @"Использование:
///info   - Информация о продукте
///Product - Инфо по продукту
///Telegraph - Вывести информацию в телеграф
//В случае ошибки писать https://t.me/joinchat/F7LnehCPgzY5b8sfHwN6KA";
                    var keyboard1 = new InlineKeyboardMarkup(new[] {
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData("Отправить текст QR -кода!"),
                                InlineKeyboardButton.WithCallbackData("Получить инфо о продукте!"),
                            }
                         });
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Отправте фото с QR -кодом и получите сравнение цен о продуктах или выбирите ниже!", ParseMode.Default, false, false, 0, keyboard1);
                  //  await Bot.SendTextMessageAsync(message.Chat.Id,usage, replyMarkup: new ReplyKeyboardRemove());
                    break;
            }
        }
        private static async void AddKeyProduct(Message message)
        {
            string product = message.Text.Remove(0, message.Text.IndexOf(' ') + 1);
            await Bot.SendTextMessageAsync(message.Chat.Id, "Продукт: "+ product+ " записан!");
            my_sql_control.AddKeyProduct(product);
        }
        private static async void ParserAuthorization(Message message)
        {
            string login = "+" + RegularExpressions(message.Text, "login:(\\d+)");
            string password = RegularExpressions(message.Text, "password:(\\d+)");
            my_sql_control.AddUsers((int)message.Chat.Id, message.Chat.FirstName, message.Chat.LastName, message.Chat.Username, login, password);
            await Bot.SendTextMessageAsync(message.Chat.Id, "Данные записаны!");
        }
        private static async void BotOnCallbackQuery(object sender, CallbackQueryEventArgs ev)
        {
            var message_ev = ev.CallbackQuery.Message;
            if (ev.CallbackQuery.Data == "Регистрация")
            {
                await Bot.SendTextMessageAsync(message_ev.Chat.Id, "К сожалению пока нет технической возможности используйте приложение ФНС для регистраци!!!"); 
            }
            if (ev.CallbackQuery.Data == "Авторизация")
            {
                await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Авторизуйтесь в системе ФНС", false);
                await Bot.SendTextMessageAsync(message_ev.Chat.Id, "Введите логин и пароль для авторизации как напримрие!");
                await Bot.SendTextMessageAsync(message_ev.Chat.Id, string.Format("/authorization {0}login:79817889931 {0}password:xxxxx", Environment.NewLine));
            }
            if (ev.CallbackQuery.Data == "Отправить текст QR -кода!")
            {
                await Bot.SendTextMessageAsync(message_ev.Chat.Id, "/info \"QR-код\"");
            }
            if (ev.CallbackQuery.Data == "Получить инфо о продукте!")
            {
                await Bot.SendTextMessageAsync(message_ev.Chat.Id, "/Telegraph \"название продукта!\"");
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
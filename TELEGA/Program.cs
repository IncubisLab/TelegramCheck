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
using System.Threading.Tasks;
using FSNCheck;
using FSNCheck.Data;
using Newtonsoft.Json;
using TELEGA;

namespace Telegram.Bot.Examples.Echo
{
    public static class Program
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("513572219:AAFnhp76wp-AMslfGNF7RVZcqmm3UU32kvs");

        public static void Main(string[] args)
        {
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.OnInlineQuery += BotOnInlineQueryReceived;
            Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            Bot.OnReceiveError += BotOnReceiveError;

            var me = Bot.GetMeAsync().Result;

            Console.Title = me.Username;

            Bot.StartReceiving();
          //  Console.WriteLine($"Start listening for @{me.Username}");
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
                    // str.AppendLine($"{item.Name} - {item.Sum}");
                    double sum = Convert.ToDouble(item.Sum) / 100;
                    str.Append(string.Format("\n{0} x {2} - {1} руб", item.Name, sum, item.Quantity));
                }

                await Bot.SendTextMessageAsync(message.Chat.Id, str.ToString());
            } catch (System.NullReferenceException e) { throw; }
        }
        private static async void BotOnPhotoMassage(Message message)
        {
            if (message.Type == MessageType.PhotoMessage)
            {
                var test = await Bot.GetFileAsync(message.Photo[message.Photo.Count() - 1].FileId);
                var image = Bitmap.FromStream(test.FileStream);
                int num = message.Photo.Count() - 1;
                image.Save(test.FilePath);

                string get =
                    String.Format(
                        "https://api.qrserver.com/v1/read-qr-code/?fileurl=https://api.telegram.org/file/bot513572219:AAFnhp76wp-AMslfGNF7RVZcqmm3UU32kvs/{0}",
                        message.Photo[num].FilePath);
                // message.Photo.Count()-1 => the biggest resolution
                string data = GET(get, "");

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
           

            //if (message == null || message.Type != MessageType.TextMessage) return;


            //IReplyMarkup keyboard = new ReplyKeyboardRemove();

//            switch (message.Text.Split(' ').First())
//            {
//                // send inline keyboard
//                case "/inline":
//                    await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

//                    await Task.Delay(500); // simulate longer running task

//                    keyboard = new InlineKeyboardMarkup(new[]
//                    {
//                        new [] // first row
//                        {
//                            InlineKeyboardButton.WithCallbackData("1.1"),
//                            InlineKeyboardButton.WithCallbackData("1.2"),
//                        },
//                        new [] // second row
//                        {
//                            InlineKeyboardButton.WithCallbackData("2.1"),
//                            InlineKeyboardButton.WithCallbackData("2.2"),
//                        }
//                    });

//                    await Bot.SendTextMessageAsync(
//                        message.Chat.Id,
//                        "Choose",
//                        replyMarkup: keyboard);
//                    break;

//                // send custom keyboard
//                case "/keyboard":
//                    keyboard = new ReplyKeyboardMarkup(new[]
//                    {
//                        new [] // first row
//                        {
//                            new KeyboardButton("1.1"),
//                            new KeyboardButton("1.2"),
//                        },
//                        new [] // last row
//                        {
//                            new KeyboardButton("2.1"),
//                            new KeyboardButton("2.2"),
//                        }
//                    });

//                    await Bot.SendTextMessageAsync(
//                        message.Chat.Id,
//                        "Choose",
//                        replyMarkup: keyboard);
//                    break;

//                // send a photo
//                case "/photo":
//                    await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

//                    const string file = @"Files/tux.png";

//                    var fileName = file.Split(Path.DirectorySeparatorChar).Last();

//                    using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
//                    {
//                        var fts = new FileToSend(fileName, fileStream);

//                        await Bot.SendPhotoAsync(
//                            message.Chat.Id,
//                            fts,
//                            "Nice Picture");
//                    }
//                    break;

//                // request location or contact
//                case "/request":
//                    keyboard = new ReplyKeyboardMarkup(new[]
//                    {
//                        new KeyboardButton("Location")
//                        {
//                            RequestLocation = true
//                        },
//                        new KeyboardButton("Contact")
//                        {
//                            RequestContact = true
//                        },
//                    });

//                    await Bot.SendTextMessageAsync(
//                        message.Chat.Id,
//                        "Who or Where are you?",
//                        replyMarkup: keyboard);
//                    break;

//                default:
//                    const string usage = @"Usage:
///inline   - send inline keyboard
///keyboard - send custom keyboard
///photo    - send a photo
///request  - request location or contact
//";

//                    await Bot.SendTextMessageAsync(
//                        message.Chat.Id,
//                        usage,
//                        replyMarkup: new ReplyKeyboardRemove());
//                    break;
//            }
        }
        private static string RegularExpressions(string input, string pattern)
        {
            Regex regex = new Regex(pattern);
            Match match = regex.Match(input);
            return match.Groups[1].Value;
        }

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            await Bot.AnswerCallbackQueryAsync(
                callbackQueryEventArgs.CallbackQuery.Id,
               // $"Received {callbackQueryEventArgs.CallbackQuery.Data}");
                string.Format("Received {0}",callbackQueryEventArgs.CallbackQuery.Data));
        }

        private static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            //Console.WriteLine($"Received inline query from: {inlineQueryEventArgs.InlineQuery.From.Id}");
            Console.WriteLine("Received inline query from:{0}",inlineQueryEventArgs.InlineQuery.From.Id);

            InlineQueryResult[] results = {
                new InlineQueryResultLocation
                {
                    Id = "1",
                    Latitude = 40.7058316f, // displayed result
                    Longitude = -74.2581888f,
                    Title = "New York",
                    InputMessageContent = new InputLocationMessageContent // message if result is selected
                    {
                        Latitude = 40.7058316f,
                        Longitude = -74.2581888f,
                    }
                },

                new InlineQueryResultLocation
                {
                    Id = "2",
                    Longitude = 52.507629f, // displayed result
                    Latitude = 13.1449577f,
                    Title = "Berlin",
                    InputMessageContent = new InputLocationMessageContent // message if result is selected
                    {
                        Longitude = 52.507629f,
                        Latitude = 13.1449577f
                    }
                }
            };

            await Bot.AnswerInlineQueryAsync(
                inlineQueryEventArgs.InlineQuery.Id,
                results,
                isPersonal: true,
                cacheTime: 0);
        }

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            //Console.WriteLine($"Received inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
            Console.WriteLine("Received inline result: {0}", chosenInlineResultEventArgs.ChosenInlineResult.ResultId);
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message);
        }
    }
}
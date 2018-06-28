using System;
using System.IO;
using System.Net;
using System.Text;
using FSNCheck.Data;
using Newtonsoft.Json;

namespace FSNCheck
{
    public class Checking
    {
        private string _login;
        private string _password;

        public Checking(string login, string password)
        {
            _login = login;
            _password = password;
        }
        public Checking() { }

        public void RegistrationFNS(string phone, string email, string name)
        {
            string outStr = null;
            string url = "http://proverkacheka.nalog.ru:9999/v1/mobile/users/signup"+ "?phone:"+phone + "?email:" + email + "?name:" + name;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Host = "proverkacheka.nalog.ru:9999";
           // req.KeepAlive = true;
         //   req.Accept = "Encoding: gzip";
            req.UserAgent = "okhttp/3.0.1";
            req.Headers.Add("Device-Id: FB8B0D6048904DBC8895EAC25F583E96");
            req.Headers.Add("Device-OS: Adnroid 4.4.4");
            req.Headers.Add("Version: 2");
            req.Headers.Add("ClientVersion: 1.4.4.1");
            req.GetResponse();
        }

        private string GetAuthToken()
        {
            //string authString = $"{_login}:{_password}";
            string authString = string.Format("{0}:{1}", _login, _password);

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(authString);
            return Convert.ToBase64String(plainTextBytes);
        }

        private Check ConvertJsonToCheck(string data)
        {
            return JsonConvert.DeserializeObject<Check>(data);
        }

        /// <summary>
        /// Возвращает данные покупки по чеку
        /// </summary>
        /// <param name="fn">Fiscal storage (Номер фискального накопителя - ФН)</param>
        /// <param name="fd">Fiscal document number (Номер фискального документа - ФД)</param>
        /// <param name="fs">Fiscal sign (Подпись фискального документа - ФП)</param>
        /// <returns>Возвращает данные чека типа <see cref="Check"/></returns>
        public Check GetCheck(string fn, string fd, string fs)
        {
            CheckInfo checkInfo = new CheckInfo
            {
                FN = fn,
                FD = fd,
                FS = fs
            };

            return GetCheck(checkInfo);
        }

        /// <summary>
        /// Возвращает данные покупки по чеку
        /// </summary>
        /// <param name="checkInfo">Данные о чеке</param>
        /// <returns>Возвращает данные чека типа <see cref="Check"/></returns>
        public Check GetCheck(CheckInfo checkInfo)
        {
            // string url = $"https://proverkacheka.nalog.ru:9999/v1/inns/*/kkts/*/fss/{checkInfo.FN}/tickets/{checkInfo.FD}?fiscalSign={checkInfo.FS}&sendToEmail=no";
            string url = "https://proverkacheka.nalog.ru:9999/v1/inns/*/kkts/*/fss/" + checkInfo.FN + "/tickets/" + checkInfo.FD + "?fiscalSign=" + checkInfo.FS + "&sendToEmail=no";
            string baseAuth = GetAuthToken();

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Host = "proverkacheka.nalog.ru:9999";
            req.KeepAlive = true;
            req.Accept = "Encoding: gzip";
            req.UserAgent = "okhttp/3.0.1";
            req.Headers.Add("Device-Id: FB8B0D6048904DBC8895EAC25F583E96");
            req.Headers.Add("Device-OS: Adnroid 4.4.4");
            req.Headers.Add("Version: 2");
            req.Headers.Add("ClientVersion: 1.4.1.3");
            // req.Headers.Add($"Authorization: Basic {baseAuth}");
            req.Headers.Add(string.Format("Authorization: Basic {0}", baseAuth));
           
            
            HttpWebResponse resp = null;
            string outStr = null;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
                using (StreamReader stream = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                {
                    outStr = stream.ReadToEnd();
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Ответ сервера ФНС не получен!");
            }

            if (outStr == null) return null;
            return ConvertJsonToCheck(outStr);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegraph;
using Telegraph.Net;
using Telegraph.Net.Models;

namespace TELEGA
{
    class TelegraphAPI
    {
        private TelegraphClient m_client;
        private ITokenClient m_tokenClient;
        private List<NodeElement> m_node_element = new List<NodeElement>();
        /// <summary>
        /// Иницилизация аккаунта телеграф по токену
        /// </summary>
        public TelegraphAPI()
        {
            m_client = new TelegraphClient();
            m_tokenClient = m_client.GetTokenClient("40b9813e6e0303023473d29a70eee7fd37342fbecabe0b7734bd0229d98a");
        }
        /// <summary>
        /// Удаление аккаунта
        /// </summary>
        public async void RevokeAccount()
        {
            // Revoke an access token
            Account account = await m_tokenClient.RevokeAccessTokenAsync();
        }
        /// <summary>
        /// Получение информации об аккаунте в телеграфе
        /// </summary>
        public async void GetAccountInfo()
        {
            Account account = await m_tokenClient.GetAccountInformationAsync(
              AccountFields.ShortName | AccountFields.AuthorUrl | AccountFields.AuthorUrl
            );
        }
        /// <summary>
        /// Изменение информации об аккаунте
        /// </summary>
        public async void EditAccountInfo()
        {
            Account updatedAccount = await m_tokenClient.EditAccountInformationAsync(
              "new-short-name",
              "new-author-name",
              "new-author-url"
            );
        }
        /// <summary>
        /// Создание страницы
        /// </summary>
        public async void CreatePage()
        {
            Page newPage = await m_tokenClient.CreatePageAsync(
              "Sample Page",
              content: new List<NodeElement> {
                    new NodeElement("p", null, "Hello, world!")
               }.ToArray(),
              returnContent: true
            );
        }
        public void AddListNodeElementNew(List<CheckProduct> products)
        {
            List<NodeElement> elem = new List<NodeElement>();
            
            foreach (var product in products)
            {
                elem.Add(new NodeElement("b", null, "Продукт: "));
                elem.Add(new NodeElement("li", null, product.Product_Name, new NodeElement("b", null, " Сумма: "),
                         new NodeElement("text", null, product.Product_Sum + "руб."), new NodeElement("b", null, " Магазин: "),
                         new NodeElement("text", null, product.Store_Name)));
            }
            m_node_element.Add(new NodeElement("ol", null, elem.ToArray()));
        }
        public void AddListNodeElementNew2(ListReportCheck reportCheck)
        {
            List<NodeElement> elem = new List<NodeElement>();
            List<NodeElement> elem1 = new List<NodeElement>();
            foreach (var report in  reportCheck.ReportCh)
            {
               // elem.Add(new NodeElement("b",null, report.m_product_check_name));
               elem.Add(new NodeElement("ul",null, new NodeElement("li", null, new NodeElement("b", null, report.m_product_name.ToUpper()))));new NodeElement("li", null, new NodeElement("b", null, report.m_product_name.ToUpper()));
                
                foreach (var product in report.m_check_product)
                {
                   
                    elem1.Add(new NodeElement("li", null, new NodeElement("b", null, "Продукт: "+product.Product_Name), new NodeElement("b", null, " Сумма: "),
                        new NodeElement("text", null, product.Product_Sum + "руб."), new NodeElement("b", null, " Магазин: "),
                        new NodeElement("text", null, product.Store_Name)));
                }
               // elem.Add(new NodeElement("li", null, new NodeElement("b", null, report.m_product_name.ToUpper())));
               // elem.Add(new NodeElement("li", null, new NodeElement("b", null, report.m_product_name.ToUpper()), new NodeElement("ol", null,"s")));
                elem.Add(new NodeElement("ol", null, elem1.ToArray()));
                elem1.Clear();
            }           




            //foreach (var product in products)
            //{
            //    elem.Add(new NodeElement("b", null, "Продукт: "));
            //    elem.Add(new NodeElement("li", null, product.Product_Name, new NodeElement("b", null, " Сумма: "),
            //        new NodeElement("text", null, product.Product_Sum + "руб."), new NodeElement("b", null, " Магазин: "),
            //        new NodeElement("text", null, product.Store_Name)));
            //}
            //foreach (var el in elem1)
            //{
            //    m_node_element.Add(new NodeElement("ul", null, el));
            //    foreach (var em in elem)
            //    {
                  
            //        m_node_element.Add(new NodeElement("ol", null, em));
            //    }
            //}
           // m_node_element.Add(new NodeElement("ol", null, elem.ToArray()));
                //, new NodeElement("ol", null, elem.ToArray()));
            //m_node_element.Add(new NodeElement("ul", null, elem.ToArray()));
            m_node_element = elem;
        }
        /// <summary>
        /// Изменение существующей страницы
        /// </summary>
        /// <param name="product"></param>
        public async void EditPage(string product)
        {
            await m_tokenClient.EditPageAsync(
            // "Sample-Page1-02-03-2",
            "Sample-Page-02-03-16",
             "Сравнение цен о продукте " + product,
             content: m_node_element.ToArray(), returnContent: true
             );
        }
        /// <summary>
        /// Информация о страницах на аккаунте Телеграф
        /// </summary>
        public void GetPageList()
        {
            var response = m_tokenClient.GetPageListAsync(0, 40).Result;
            foreach (var page in response.Pages)
            {
                Console.WriteLine(page.Url);
            }
        }
        /// <summary>
        /// Установить лимит на количество страниц в аккаунте
        /// </summary>
        public async void PageLimit()
        {
            // Get first 50 pages created by the account with the context access-token
            PageList pageList = await m_tokenClient.GetPageListAsync(offset: 0, limit: 50);
        }
    }
}

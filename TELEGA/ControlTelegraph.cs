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
    class ControlTelegraph
    {
        private TelegraphClient m_client;
        private ITokenClient m_tokenClient;
        private List<NodeElement> m_node_element = new List<NodeElement>();

        public ControlTelegraph()
        {
            m_client = new TelegraphClient();
            m_tokenClient = m_client.GetTokenClient("40b9813e6e0303023473d29a70eee7fd37342fbecabe0b7734bd0229d98a");
        }

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

        public void AddListNodeElement(List<CheckProduct> products)
        {
            foreach(var product in products)
            {
                m_node_element.Add(
                    new NodeElement("ol", null,
                       new NodeElement("b", null, "Продукт: "),
                       new NodeElement("li", null, product.Product_Name, new NodeElement("b", null, " Сумма: "),
                       new NodeElement("text", null, product.Product_Sum + "руб."),
                       new NodeElement("b", null, " Магазин: "),
                       new NodeElement("text", null, product.Store_Name))
                       ));
            }
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
        public async void EditPage()
        {
            Page editedPage = await m_tokenClient.EditPageAsync(
             "Sample-Page1-02-03",
             "Информация о чекe",
             content: m_node_element.ToArray(), returnContent: true
             );
        }

    }
}

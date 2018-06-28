using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using FSNCheck.Data;

namespace TELEGA
{
    class Data_Analysis
    {
        private MySQLControl my_sql_control;
        private ListReportCheck m_list_report_check = new ListReportCheck();
        public Data_Analysis(MySQLControl my_sql_control)
        {
            this.my_sql_control = my_sql_control;
        }
        /// <summary>
        /// SQL запрос по названию продукта 
        /// </summary>
        /// <param name="name_product"></param>
        /// <returns>Получение списка продуктов </returns>
        public List<CheckProduct> Parser_Check(string name_product)
        {
            try
            {
                return my_sql_control.ResultCheck(@"Select DISTINCT pr.Product_name, pr.Product_quantity, pr.Product_sum, lt.Store_name
                From CheckTelegram.products as pr, CheckTelegram.`check` as ch, CheckTelegram.store as st, mydb.List_Store as lt
                Where ch.ID_check = pr.ID_check and st.INN = ch.INN_Store and st.Link_store = lt.ID
                and pr.Product_name LIKE '%" + name_product + "%'" +
                @"union
                Select DISTINCT pr.Product_name, pr.Product_quantity, pr.Product_sum, ch.Store_name
                From CheckTelegram.products as pr, CheckTelegram.check as ch
                Where ch.ID_check = pr.ID_check and pr.Product_name LIKE '%" + name_product + "%'");
            }
            catch (MySqlException e)
            {
                Console.WriteLine("Достигнуто Max подключений!");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<ProductStandart> StandartProduct()
        {
            try
            {
                return my_sql_control.GetProductsStandart("SELECT pr.ProductName, pr.BrandName, pr.Refinement FROM mydb.List_Products As pr;");
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        /// <summary>
        /// Поиск ключевых слов продуктов
        /// </summary>
        /// <param name="product_name"> Название продукта в исходном виде </param>
        public List<CheckProduct> Search_Key_Product1(string product_name, ValueType price)
        {
            List<CheckProduct> products = new List<CheckProduct>();
           // var keywords = StandartProduct();
            List<string> keywords = new List<string>();
            List<string> keywords1 = new List<string>();
            foreach (var key in StandartProduct())
            {
                keywords.Add(key.Product_name);
                keywords1.Add(key.Product_name);
            }
                var matches = Regex.Matches(product_name.ToLower(), @"\w+");
                var result = matches.Cast<Match>()
                    .Select(m => m.Value)
                    .Where(keywords.Contains)
                    .GroupBy(n => n)
                    .Select(g => new { Product = g.First(), Count = g.Count() });
                foreach (var e in result)
                {
                    if (e.Count >= 1)
                    {
                        List<CheckProduct> pr = Parser_Check(e.Product);
                        m_list_report_check.AddReportCheck(e.Product, product_name, Convert.ToString(price), pr);
                        if (pr == null) { Console.WriteLine("Достигнуто Max подключений!"); return null; }
                        products.AddRange(pr);
                    }
                }
            
            return products;
        }

        public List<CheckProduct> Search_Product(Check check)
        {
            List<CheckProduct> products = new List<CheckProduct>();
            foreach (var item in check.Document.Receipt.Items)
            {
                List<CheckProduct> pr = Search_Key_Product1(item.Name, item.Price);
                if (pr == null) { Console.WriteLine("Достигнуто Max подключений!"); return null; }
                products.AddRange(pr);
            }
            return products;
        }

        public ListReportCheck LReportCheck
        {
            get { return m_list_report_check; }
        }


        // TODO: Добавить метод для парсинга чека (номер чека, продукт)
        // TODO: Запомнить список продуктов 
    }
}

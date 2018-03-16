using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.IO;
using System.Text.RegularExpressions;
using FSNCheck.Data;
using System.Threading;
namespace TELEGA
{
    class Data_Analysis
    {
        private MySQLControl my_sql_control;
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
                return my_sql_control.ResultCheck(@"Select DISTINCT pr.Product_name, pr.Product_quantity, pr.Product_sum, st.Store_name
                                         From CheckTelegram.products as pr, CheckTelegram.users as us, 
                                         CheckTelegram.store as st, CheckTelegram.`check` as ch
                                         Where us.ID_users = ch.ID_Users and ch.ID_check = pr.ID_check
                                         and pr.Product_name LIKE '%" + name_product + "%'");
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

        public List<CheckProduct> Prser_Check_Product(string name_product, int number_check)
        {
           return my_sql_control.ResultCheck(@"SELECT distinct pr.ID_check, pr.Product_name, pr.Product_sum, pr.Product_quantity 
                                         FROM CheckTelegram.products As pr
                                         where pr.Product_name like '%" + name_product + "%' and pr.ID_check = " + number_check);
        }
        /// <summary>
        /// Поиск ключевых слов продуктов
        /// </summary>
        /// <param name="product_name"> Название продукта в исходном виде </param>
        public List<CheckProduct> Search_Key_Product1(string product_name)
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
                    // Console.WriteLine("{0} : {1}", e.Word, e.Count);
                    if (e.Count >= 1)
                    {
                       // List<CheckProduct> pr = Parser_Check(e.Product);
                        List<CheckProduct> pr = Parser_Check(e.Product);
                               if (pr == null) { Console.WriteLine("Достигнуто Max подключений!"); return null; }
                                products.AddRange(pr);
                        //var matches1 = Regex.Matches(product_name.ToLower(), @"\w+");
                        //var result1 = matches.Cast<Match>()
                        //    .Select(m => m.Value)
                        //    .Where(keywords1.Contains)
                        //    .GroupBy(n => n)
                        //    .Select(g => new { Product = g.First(), Count = g.Count() });
                        //foreach (var e1 in result1)
                        //{
                        //    if (e1.Count >= 1)
                        //    {
                        //        
                        //    }
                        //}
                        
                        // Thread.Sleep(100);
                    }
                }
            
            return products;
        }
        /// <summary>
        /// Поиск ключевых слов продуктов
        /// </summary>
        /// <param name="product_name"> Название продукта в исходном виде </param>
        public List<CheckProduct> Search_Key_Product(string product_name)
        {
            List<CheckProduct> products = new List<CheckProduct>();
            var keywords = File.ReadAllText("key_products.txt", Encoding.UTF8).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var matches = Regex.Matches(product_name.ToLower(), @"\w+");
            var result = matches.Cast<Match>()
                .Select(m => m.Value)
                .Where(keywords.Contains)
                .GroupBy(n => n)
                .Select(g => new { Product = g.First(), Count = g.Count() });
            foreach (var e in result)
            {
               // Console.WriteLine("{0} : {1}", e.Word, e.Count);
                if (e.Count >= 1 )
                {
                   List<CheckProduct> pr = Parser_Check(e.Product);
                   if (pr == null) { Console.WriteLine("Достигнуто Max подключений!"); return null; }
                   products.AddRange(pr);
                 // Thread.Sleep(100);
                }
            }
            return products;
        }

        public List<CheckProduct> Search_Product(Check check)
        {
            List<CheckProduct> products = new List<CheckProduct>();
            foreach (var item in check.Document.Receipt.Items)
            {
                List<CheckProduct> pr = Search_Key_Product1(item.Name);
                if (pr == null) { Console.WriteLine("Достигнуто Max подключений!"); return null; }
                products.AddRange(pr);
                //Thread.Sleep(100);
            }
            return products;
        }

        // TODO: Добавить метод для парсинга чека (номер чека, продукт)
        // TODO: Запомнить список продуктов 
    }
}

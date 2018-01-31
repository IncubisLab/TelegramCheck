using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace TELEGA
{
    class Data_Analysis
    {
        private MySQLControl my_sql_control;
        public Data_Analysis(MySQLControl my_sql_control)
        {
            this.my_sql_control = my_sql_control;
        }
        public void Search_Product_Check(int number_check, string name_product)
        {
            //TODO: Написать SQL запрос для определения продукта с наименьшей ценой в магазинах
            //TODO: Вывести имя магазина
            //TODO: для поиска использовать Like или что подобное)
            my_sql_control.MySQLSelect(@"Select * From users, store, check, products 
                                         Where users.ID_users = store.ID_users and store.Store_name = check.Store_name and
                                         products.Product_name ="+name_product + "products.ID_check =" + number_check);
           
        }
    }
}

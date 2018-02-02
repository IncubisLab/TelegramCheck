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
        public void Search_Product_Check(string name_product)
        {
           // int number_check;
            //TODO: Написать SQL запрос для определения продукта с наименьшей ценой в магазинах
            //TODO: Вывести имя магазина
            //TODO: для поиска использовать Like или что подобное)
//            my_sql_control.MySQLSelect(@"Select * From users, store, check, products 
//                                         Where users.ID_users = store.ID_users and store.Store_name = check.Store_name and
//                                         products.Product_name ="+name_product + "products.ID_check =" + number_check);
            my_sql_control.MySQLSelect(@"Select DISTINCT pr.Product_name, pr.Product_quantity, pr.Product_sum, st.Store_name
                                         From ibmsl_1873546bc5817409ce81.products as pr, ibmsl_1873546bc5817409ce81.users as us, 
                                         ibmsl_1873546bc5817409ce81.store as st, ibmsl_1873546bc5817409ce81.`check` as ch
                                         Where us.ID_users = st.ID_users and st.Store_name = ch.Store_name and ch.ID_check = pr.ID_check
                                         and pr.Product_name LIKE '%" + name_product +"%'");
           
        }
    }
}

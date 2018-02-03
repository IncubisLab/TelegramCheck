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
        /// <summary>
        /// SQL запрос по названию продукта 
        /// </summary>
        /// <param name="name_product"></param>
        /// <returns>Получение списка продуктов </returns>
        public List<CheckProduct> Parser_Check(string name_product)
        {
           return my_sql_control.ResultCheck(@"Select DISTINCT pr.Product_name, pr.Product_quantity, pr.Product_sum, st.Store_name
                                         From ibmsl_1873546bc5817409ce81.products as pr, ibmsl_1873546bc5817409ce81.users as us, 
                                         ibmsl_1873546bc5817409ce81.store as st, ibmsl_1873546bc5817409ce81.`check` as ch
                                         Where us.ID_users = st.ID_users and st.Store_name = ch.Store_name and ch.ID_check = pr.ID_check
                                         and pr.Product_name LIKE '%" + name_product + "%'");
        }
    }
}

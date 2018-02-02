using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TELEGA
{
    struct CheckProduct
    {
        public string Product_Name;
        public string Product_Sum;
        public string Product_Quantity;
        public string Store_Name;

        public CheckProduct (string product_Name, string product_Sum, string product_Quantity, string store_Name)
        {
            Product_Name = product_Name;
            Product_Sum = product_Sum;
            Product_Quantity = product_Quantity;
            Store_Name = store_Name;
        }
    }
}

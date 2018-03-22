using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TELEGA
{
    class ReportCheck
    {
        public string m_product_name;
        public string m_product_check_name;
        public string m_price;
        public List<CheckProduct> m_check_product = new List<CheckProduct>();

        public ReportCheck(string product_name, string product_check_name, string price, List<CheckProduct> check_product)
        {
            m_product_name = product_name;
            m_product_check_name = product_check_name;
            m_check_product = check_product;
            m_price = price;
        }
    }
}

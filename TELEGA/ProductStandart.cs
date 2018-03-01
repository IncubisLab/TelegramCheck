using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TELEGA
{
    class ProductStandart
    {
        public string Product_name;
        public string Product_brand;
        public string Refinement;

        public ProductStandart(string product_name, string product_brand, string refinement)
        {
            Product_name = product_name;
            Product_brand = product_brand;
            Refinement = refinement;
        }
    }
}

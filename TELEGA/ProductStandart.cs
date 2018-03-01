using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TELEGA
{
    class ProductStandart
    {
        private string Product_name;
        private string Product_brand;
        private string Refinement;

        private ProductStandart(string product_name, string product_brand, string refinement)
        {
            Product_name = product_name;
            Product_brand = Product_brand;
            Refinement = refinement;
        }
    }
}

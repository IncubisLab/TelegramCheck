using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TELEGA
{
    class ListReportCheck
    {
        private  List<ReportCheck> report_chek = new List<ReportCheck>();

        public void AddReportCheck(string product_name, string product_check_name, List<CheckProduct> m_check_product)
        {
            report_chek.Add(new ReportCheck(product_name, product_check_name, m_check_product));
        }

        public List<ReportCheck> ReportCh
        {
            get { return report_chek; }
        }
    }
}

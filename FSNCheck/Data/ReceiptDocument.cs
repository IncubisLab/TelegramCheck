namespace FSNCheck.Data
{
    public class ReceiptDocument
    {
        public int ShiftNumber { get; set; }
        public int CashTotalSum { get; set; }
        public CartItem[] Items { get; set; }
        public string FiscalDriveNumber { get; set; }
        public string User { get; set; }
        public string FiscalSign { get; set; }
        public int TaxationType { get; set; }
        public int ReceiptCode { get; set; }
        public int OperationType { get; set; }
        public int TotalSum { get; set; }
        public string KktRegId { get; set; }
        public string RawData { get; set; }
        public string UserInn { get; set; }
        public string DateTime { get; set; }
        public int FiscalDocumentNumber { get; set; }
        public int Nds10 { get; set; }
        public string Operator { get; set; }
        public int Nds18 { get; set; }
        public int RequestNumber { get; set; }
    }
}

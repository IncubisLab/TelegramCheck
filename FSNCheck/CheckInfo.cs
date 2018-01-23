namespace FSNCheck
{
    public class CheckInfo
    {
        /// <summary>
        /// Fiscal storage (Номер фискального накопителя - ФН)
        /// </summary>
        public string FN { get; set; }

        /// <summary>
        /// Fiscal document number (Номер фискального документа - ФД)
        /// </summary>
        public string FD { get; set; }

        /// <summary>
        /// Fiscal sign (Подпись фискального документа - ФП)
        /// </summary>
        public string FS { get; set; }
    }
}

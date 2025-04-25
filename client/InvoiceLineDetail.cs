public class InvoiceLineDetail
{
    public int InvoiceLineNumber { get; set; }

    public string InvoiceLineType { get; set; }

    public string LineCode { get; set; }

    public decimal InvoiceLineAmount { get; set; }

    public string DefaultDistributionAccount { get; set; }

    public string Description { get; set; }

    public string TaxClassificationCode { get; set; }

    public string DistributionSupplier { get; set; }

    public string Info1 { get; set; }

    public string Info2 { get; set; }

    public string Info3 { get; set; }

    public string ToJSONString()
    {
        string amountFormat = "0.00";

        return string.Format("$!$\r\n   \"invoiceLineNumber\": {0},\r\n   \"invoiceLineType\": \"{1}\",\r\n   \"lineCode\": \"{2}\",\r\n   \"invoiceLineAmount\": {3},\r\n   \"defaultDistributionAccount\": \"{4}\",\r\n   \"description\": \"{5}\",\r\n   \"taxClassificationCode\": \"{6}\",\r\n   \"distributionSupplier\": \"{7}\",\r\n   \"info1\": \"{8}\",\r\n   \"info2\": \"{9}\",\r\n   \"info3\": \"{10}\"\r\n   $&$",
            InvoiceLineNumber,
            InvoiceLineType,
            LineCode,
            InvoiceLineAmount.ToString(amountFormat),
            DefaultDistributionAccount,
            Description,
            TaxClassificationCode,
            DistributionSupplier,
            Info1,
            Info2,
            Info3
            );
    }
}

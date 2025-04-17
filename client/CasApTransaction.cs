public class CasApTransaction
{
    public bool IsBlockSupplier { get; set; }

    public string InvoiceType { get; set; }

    public string SupplierNumber { get; set; }

    public int SupplierSiteNumber { get; set; }

    public DateTime InvoiceDate { get; set; }

    public string InvoiceNumber { get; set; }

    public decimal InvoiceAmount { get; set; }

    public string PayGroup { get; set; }

    public DateTime DateInvoiceReceived { get; set; }

    public DateTime? DateGoodsReceived { get; set; }

    public string RemittanceCode { get; set; }

    public bool SpecialHandling { get; set; }

    public string NameLine1 { get; set; }

    public string NameLine2 { get; set; }

    public string AddressLine1 { get; set; }

    public string AddressLine2 { get; set; }

    public string AddressLine3 { get; set; }

    public string City { get; set; }

    public string Country { get; set; }

    public string Province { get; set; }

    public string PostalCode { get; set; }

    public string QualifiedReceiver { get; set; }

    public string Terms { get; set; }

    public string PayAloneFlag { get; set; }

    public string PaymentAdviceComments { get; set; }

    public string RemittanceMessage1 { get; set; }

    public string RemittanceMessage2 { get; set; }

    public string RemittanceMessage3 { get; set; }

    public DateTime? GLDate { get; set; }

    public string InvoiceBatchName { get; set; }

    public string CurrencyCode { get; set; }

    public string? AccountNumber { get; set; }

    private string _transitNumber = string.Empty;
    public string TransitNumber
    {
        get
        {
            if (!string.IsNullOrEmpty(_transitNumber))
            {
                if (_transitNumber.Length < 5)
                {
                    List<string> concat = new List<string>();
                    for (int i = 0; i < (5 - _transitNumber.Length); i++)
                    {
                        concat.Add("0");
                    }
                    concat.Add(_transitNumber);
                    return string.Join("", concat);
                }
            }

            return _transitNumber;
        }

        set
        {
            _transitNumber = value;
        }
    }

    private string _institutionNumber = string.Empty;
    public string InstitutionNumber
    {
        get
        {
            if (!string.IsNullOrEmpty(_institutionNumber))
            {
                if (_institutionNumber.Length < 4)
                {
                    List<string> concat = new List<string>();
                    for (int i = 0; i < (4 - _institutionNumber.Length); i++)
                    {
                        concat.Add("0");
                    }
                    concat.Add(_institutionNumber);
                    return string.Join("", concat);
                }
            }

            return _institutionNumber;
        }

        set
        {
            _institutionNumber = value;
        }
    }

    public string? EFTAdvice { get; set; }

    public string? EmailAddress { get; set; }

    public List<CasApTransactionInvoiceLineDetail> InvoiceLineDetails { get; set; }

    public string ToJSONString()
    {
        string amountFormat = "0.00";

        List<string> lines = new List<string>();
        foreach (var invoiceLineItem in InvoiceLineDetails)
        {
            lines.Add(invoiceLineItem.ToJSONString());
        }

        if (!string.IsNullOrEmpty(InstitutionNumber) && !string.IsNullOrEmpty(TransitNumber) && !string.IsNullOrEmpty(AccountNumber))
        {
            return string.Format("$!$\r\n \"invoiceType\": \"{0}\",\r\n \"supplierNumber\": \"{1}\",\r\n \"supplierSiteNumber\": \"{2}\",\r\n \"invoiceDate\": \"{3}\",\r\n \"invoiceNumber\": \"{4}\",\r\n \"invoiceAmount\": {5},\r\n \"payGroup\": \"{6}\",\r\n \"dateInvoiceReceived\": \"{7}\",\r\n \"dateGoodsReceived\": \"{8}\",\r\n \"remittanceCode\": \"{9}\",\r\n \"specialHandling\": \"{10}\",\r\n \"nameLine1\": \"{11}\",\r\n \"nameLine2\": \"{12}\",\r\n \"addressLine1\": \"{13}\",\r\n \"addressLine2\": \"{14}\",\r\n \"addressLine3\": \"{15}\",\r\n \"city\": \"{16}\",\r\n \"country\": \"{17}\",\r\n \"province\": \"{18}\",\r\n \"postalCode\": \"{19}\",\r\n \"qualifiedReceiver\": \"{20}\",\r\n \"terms\": \"{21}\",\r\n \"payAloneFlag\": \"{22}\",\r\n \"paymentAdviceComments\": \"{23}\",\r\n \"remittanceMessage1\": \"{24}\",\r\n \"remittanceMessage2\": \"{25}\",\r\n \"remittanceMessage3\": \"{26}\",\r\n \"glDate\": \"{27}\",\r\n \"invoiceBatchName\": \"{28}\",\r\n \"currencyCode\": \"{29}\",\r\n \"bankNumber\": \"{30}\",\r\n \"branchNumber\": \"{31}\",\r\n \"accountNumber\": \"{32}\",\r\n \"eftAdviceFlag\": \"{33}\",\r\n \"eftEmailAddress\": \"{34}\",\r\n \"invoiceLineDetails\": [{35}]\r\n$&$",
                InvoiceType,
                SupplierNumber,
                SupplierSiteNumber.ToString("000"),
                ConvertDate(InvoiceDate),
                InvoiceNumber,
                InvoiceAmount.ToString(amountFormat),
                PayGroup,
                ConvertDate(DateInvoiceReceived),
                ConvertDate(DateGoodsReceived),
                RemittanceCode,
                (SpecialHandling ? "D" : "N"),
                NameLine1,
                NameLine2,
                AddressLine1,
                AddressLine2,
                AddressLine3,
                City,
                Country,
                Province,
                string.IsNullOrEmpty(PostalCode) ? "" : PostalCode.Replace(" ", ""),
                QualifiedReceiver,
                Terms,
                PayAloneFlag,
                PaymentAdviceComments,
                RemittanceMessage1,
                RemittanceMessage2,
                RemittanceMessage3,
                ConvertDate(GLDate),
                InvoiceBatchName,
                CurrencyCode,
                InstitutionNumber,
                TransitNumber,
                AccountNumber,
                EFTAdvice,
                EmailAddress,
                string.Join(",", lines)
                ).Replace("$!$", "{").Replace("$&$", "}");
        }
        else
        {
            return string.Format("$!$\r\n \"invoiceType\": \"{0}\",\r\n \"supplierNumber\": \"{1}\",\r\n \"supplierSiteNumber\": \"{2}\",\r\n \"invoiceDate\": \"{3}\",\r\n \"invoiceNumber\": \"{4}\",\r\n \"invoiceAmount\": {5},\r\n \"payGroup\": \"{6}\",\r\n \"dateInvoiceReceived\": \"{7}\",\r\n \"dateGoodsReceived\": \"{8}\",\r\n \"remittanceCode\": \"{9}\",\r\n \"specialHandling\": \"{10}\",\r\n \"nameLine1\": \"{11}\",\r\n \"nameLine2\": \"{12}\",\r\n \"addressLine1\": \"{13}\",\r\n \"addressLine2\": \"{14}\",\r\n \"addressLine3\": \"{15}\",\r\n \"city\": \"{16}\",\r\n \"country\": \"{17}\",\r\n \"province\": \"{18}\",\r\n \"postalCode\": \"{19}\",\r\n \"qualifiedReceiver\": \"{20}\",\r\n \"terms\": \"{21}\",\r\n \"payAloneFlag\": \"{22}\",\r\n \"paymentAdviceComments\": \"{23}\",\r\n \"remittanceMessage1\": \"{24}\",\r\n \"remittanceMessage2\": \"{25}\",\r\n \"remittanceMessage3\": \"{26}\",\r\n \"glDate\": \"{27}\",\r\n \"invoiceBatchName\": \"{28}\",\r\n \"currencyCode\": \"{29}\",\r\n \"invoiceLineDetails\": [{30}]\r\n$&$",
                InvoiceType,
                SupplierNumber,
                SupplierSiteNumber.ToString("000"),
                ConvertDate(InvoiceDate),
                InvoiceNumber,
                InvoiceAmount.ToString(amountFormat),
                PayGroup,
                ConvertDate(DateInvoiceReceived),
                ConvertDate(DateGoodsReceived),
                RemittanceCode,
                (SpecialHandling ? "D" : "N"),
                NameLine1,
                NameLine2,
                AddressLine1,
                AddressLine2,
                AddressLine3,
                City,
                Country,
                Province,
                string.IsNullOrEmpty(PostalCode) ? "" : PostalCode.Replace(" ", ""),
                QualifiedReceiver,
                Terms,
                PayAloneFlag,
                PaymentAdviceComments,
                RemittanceMessage1,
                RemittanceMessage2,
                RemittanceMessage3,
                ConvertDate(GLDate),
                InvoiceBatchName,
                CurrencyCode,
                string.Join(",", lines)
                ).Replace("$!$", "{").Replace("$&$", "}");
        }
    }

    private string ConvertDate(DateTime? date)
    {
        if (date.HasValue)
        {
            return date.Value.ToLocalTime().ToString("dd-MMM-yyyy").Replace(".", "");
        }
        return "";
    }
}
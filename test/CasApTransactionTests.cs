public class CasApTransactionTests(ICasHttpClient casHttpClient, AppSettings appSettings)
{
    // TODO this is correctly configured to use coast utilities and should use this solution cas-interface-service.api instead
    [Fact]
    public async Task Send_Invoices_Cas_Transaction_Succeed()
    {
        var invoices = new CasApTransaction();
        invoices.IsBlockSupplier = true;
        invoices.InvoiceType = "Standard";
        invoices.SupplierNumber = "2000428";
        invoices.SupplierSiteNumber = 1;
        invoices.InvoiceDate = DateTime.Now;
        invoices.InvoiceNumber = "INV-2025-026102";
        invoices.InvoiceAmount = 284.00m;
        invoices.PayGroup = "GEN CHQ";
        invoices.DateInvoiceReceived = DateTime.Now;
        invoices.RemittanceCode = "01";
        invoices.SpecialHandling = false;
        //https://cscp-vs.dev.jag.gov.bc.ca/api/data/v9.0/contacts?$filter=fullname%20eq%20%27Ida%20Albert%27
        invoices.NameLine1 = "Ida Albert";
        invoices.AddressLine1 = "2671 Champions Lounge";
        invoices.AddressLine2 = "30";
        invoices.AddressLine3 = "Galaxy Studios";
        invoices.City = "Chilliwack";
        invoices.Country = "CA";
        invoices.Province = "BC";
        invoices.PostalCode = "V4R9M0";
        invoices.QualifiedReceiver = "systemuser";
        invoices.Terms = "Immediate";
        invoices.PayAloneFlag = "Y";
        invoices.PaymentAdviceComments = "";
        invoices.RemittanceMessage1 = "21-03304-VIC-Albert, Ida";
        invoices.RemittanceMessage2 = "Income Support-Lost Earning Capacity-Minimum Wage";
        invoices.RemittanceMessage3 = "Crime Victim Assistance Program";
        invoices.GLDate = DateTime.Now;
        invoices.InvoiceBatchName = "SNBATCH";
        invoices.CurrencyCode = "CAD";
        invoices.InvoiceLineDetails = new List<CasApTransactionInvoiceLineDetail>
        {
            new CasApTransactionInvoiceLineDetail
            {
                InvoiceLineNumber = 1,
                InvoiceLineType = "Item",
                LineCode = "DR",
                InvoiceLineAmount = 284.00m,
                // vsd_programtyp columns vsd_clientcode.vsd_responsibilitycentre, vsd_serviceline, vsd_stob, vsd_projectcode
                //DefaultDistributionAccount = "010.15106.12120.7902.1501300.000000.0000",
                DefaultDistributionAccount = "010.15004.10250.5298.1500000.000000.0000",
            }
        };

        casHttpClient.Initialize(appSettings.Client);
        await casHttpClient.ApTransaction(invoices);

        // returns 
        // 04/07/2025 19:28:40 Output Message: {"invoice_number":"INV-2025-026102","CAS-Returned-Messages":"[004] Supplier Site is blank, inactive or invalid for the supplier entered.;[018] Name Line 1 must be blank for this supplier type.;[043] Default Distribution Account is blank or invalid, Error detail = Value 15106 for the flexfield segment BCGOV_RSP does not exist in the value set BCGOV_RSP.."}
        // 04/15/2025 19:23:48 Output Message: {"invoice_number":"INV-2025-026102","CAS-Returned-Messages":"[018] Name Line 1 must be blank for this supplier type.;[043] Default Distribution Account is blank or invalid, Error detail = Value 15106 for the flexfield segment BCGOV_RSP does not exist in the value set BCGOV_RSP.."}

        // successful JSON from cas-interface-service logs
        //04/10/2025 08:30:26 JSON: {"operatingUnit":null,"invoiceType":"Standard","poNumber":null,"supplierName":null,"supplierNumber":"2000428","supplierSiteNumber":"001","invoiceDate":"09-Apr-2025","invoiceNumber":"INV-2025-087279","invoiceAmount":1000.00,"payGroup":"GEN CHQ","dateInvoiceReceived":"09-Apr-2025","dateGoodsReceived":"","remittanceCode":"01","specialHandling":"N","bankNumber":null,"branchNumber":null,"accountNumber":null,"eftAdviceFlag":null,"eftEmailAddress":null,"nameLine1":"Amanda Khalid","nameLine2":"","addressLine1":"16 High Street","addressLine2":"","addressLine3":"","city":"Coquitlam","country":"CA","province":"BC","postalCode":"V8V6V5","qualifiedReceiver":"CVAP Service Account","terms":"Immediate","payAloneFlag":"Y","paymentAdviceComments":"","remittanceMessage1":"21-22290-VIC-Khalid, Amanda","remittanceMessage2":"Wage Loss-Wage Loss","remittanceMessage3":"Crime Victim Assistance Program","termsDate":null,"glDate":"10-Apr-2025","invoiceBatchName":"SNBATCH","currencyCode":"CAD","invoiceLineDe...
        //04/10/2025 08:30:48 JSON: {"operatingUnit":null,"invoiceType":"Standard","poNumber":null,"supplierName":null,"supplierNumber":"2000428","supplierSiteNumber":"001","invoiceDate":"09-Apr-2025","invoiceNumber":"INV-2025-087279","invoiceAmount":1000.00,"payGroup":"GEN CHQ","dateInvoiceReceived":"09-Apr-2025","dateGoodsReceived":"","remittanceCode":"01","specialHandling":"N","bankNumber":null,"branchNumber":null,"accountNumber":null,"eftAdviceFlag":null,"eftEmailAddress":null,"nameLine1":"Amanda Khalid","nameLine2":"","addressLine1":"16 High Street","addressLine2":"","addressLine3":"","city":"Coquitlam","country":"CA","province":"BC","postalCode":"V8V6V5","qualifiedReceiver":"CVAP Service Account","terms":"Immediate","payAloneFlag":"Y","paymentAdviceComments":"","remittanceMessage1":"21-22290-VIC-Khalid, Amanda","remittanceMessage2":"Wage Loss-Wage Loss","remittanceMessage3":"Crime Victim Assistance Program","termsDate":null,"glDate":"10-Apr-2025","invoiceBatchName":"SNBATCH","currencyCode":"CAD","invoiceLineDe...

    }
}

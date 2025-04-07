using Microsoft.Extensions.Configuration;
using Utilities;

public class CasApTransactionInvoiceTests(ICasHttpClient casHttpClient, IConfiguration configuration)
{
    // TODO this is correctly configured to use coast utilities and should use this solution cas-interface-service.api instead
    [Fact]
    public async Task Send_Invoices_Cas_Transaction_Succeed()
    {
        var invoices = new CasApTransactionInvoices();
        invoices.IsBlockSupplier = true;
        invoices.InvoiceType = "Standard";
        invoices.SupplierNumber = "2002740";
        invoices.SupplierSiteNumber = 111;
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
                DefaultDistributionAccount = "010.15106.12120.7902.1501300.000000.0000",
            }
        };
        //https://wsgw.test.jag.gov.bc.ca/victim/api/cas
        var clientId = configuration["ClientId"];
        var clientKey = configuration["ClientKey"];
        casHttpClient.Initialize(clientId, clientKey, "https://wsgw.test.jag.gov.bc.ca/victim/api/cas");
        await casHttpClient.ApTransaction(invoices);

        // returns 
        // 04/07/2025 19:28:40 Output Message: {"invoice_number":"INV-2025-026102","CAS-Returned-Messages":"[004] Supplier Site is blank, inactive or invalid for the supplier entered.;[018] Name Line 1 must be blank for this supplier type.;[043] Default Distribution Account is blank or invalid, Error detail = Value 15106 for the flexfield segment BCGOV_RSP does not exist in the value set BCGOV_RSP.."}
    }
}

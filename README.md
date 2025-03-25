# Program pro vytvoreni/generovani XML souboru.
Program uvnitr obsahuje komentare pro veskerou informaci.

### Existuje menu ze 3 variant:
```
Console.WriteLine("Vyberte akci:");
Console.WriteLine("1 - Vytvořit fakturu");
Console.WriteLine("2 - Otevřít existující fakturu");
Console.WriteLine("3 - Ukončit");
```

### Pri vytvareni, uzivatel bude vkladat data:
```
Console.WriteLine($"ID: {id}");
Console.WriteLine($"Datum: {date}");
Console.WriteLine($"Dodavatel: {supplierName}, IČO: {supplierICO}");
Console.WriteLine($"Zákazník: {customerName}, IČO: {customerICO}");
Console.WriteLine($"Položka: {itemDescription}, Množství: {quantity}, Cena: {price}, Celkem: {total}");
```

Uzivatel muze ulozit podle vlastni cesty nebo lokalne v slozce kde se nachazi program.



### Pro druhou variantu uzivatel muze nacist soubor:
```
 Console.WriteLine("Vyberte možnost:");
 Console.WriteLine("1 - Otevřít fakturu z lokální složky");
 Console.WriteLine("2 - Otevřít fakturu z vlastní cesty");
```


### Otevreni souboru typu ISDOC v jine aplikaci

Lze vyuzit aplikaci ISDOC reader napriklad od [STORMWARE](https://www.stormware.cz/demoverze/).

Priklad jak vypada soubor:

![image](https://github.com/user-attachments/assets/a50632a4-72d3-4025-9e46-3966ae72abf5)



### Zpusob jak se uklada soubor:
```
 DateTime invoiceDate = DateTime.ParseExact(date, "dd-MM-yyyy", null);
 decimal tax = total * 0.21m;

 XNamespace ns = "http://isdoc.cz/namespace/2013";
 XDocument doc = new XDocument(
     new XElement(ns + "Invoice",
         new XAttribute("version", "6.0.2"),
         new XElement(ns + "DocumentType", "1"),
         new XElement(ns + "SubDocumentType", "INV"),
         new XElement(ns + "SubDocumentTypeOrigin", "ISDOC"),
         new XElement(ns + "ID", id),
         new XElement(ns + "UUID", Guid.NewGuid().ToString()),
         new XElement(ns + "IssueDate", invoiceDate.ToString("yyyy-MM-dd")),
         new XElement(ns + "TaxPointDate", invoiceDate.ToString("yyyy-MM-dd")),
         new XElement(ns + "LocalCurrencyCode", "CZK"),
         new XElement(ns + "ForeignCurrencyCode", "CZK"),
         new XElement(ns + "CurrRate", "1"),
         new XElement(ns + "RefCurrRate", "1"),

         // Dodavatel (bez PostalAddress)
         new XElement(ns + "AccountingSupplierParty",
             new XElement(ns + "Party",
                 new XElement(ns + "PartyIdentification",
                     new XElement(ns + "UserID", "SUP001"),
                     new XElement(ns + "CatalogFirmIdentification", "0"),
                     new XElement(ns + "ID", supplierICO)
                 ),
                 new XElement(ns + "PartyName",
                     new XElement(ns + "Name", supplierName)
                 ),
                 new XElement(ns + "PartyTaxScheme",
                     new XElement(ns + "CompanyID", $"CZ{supplierICO}"),
                     new XElement(ns + "TaxScheme", "VAT")
                 )
             )
         ),

         // Zákazník (bez PostalAddress)
         new XElement(ns + "AccountingCustomerParty",
             new XElement(ns + "Party",
                 new XElement(ns + "PartyIdentification",
                     new XElement(ns + "UserID", "CUST001"),
                     new XElement(ns + "CatalogFirmIdentification", "0"),
                     new XElement(ns + "ID", customerICO)
                 ),
                 new XElement(ns + "PartyName",
                     new XElement(ns + "Name", customerName)
                 ),
                 new XElement(ns + "PartyTaxScheme",
                     new XElement(ns + "CompanyID", $"CZ{customerICO}"),
                     new XElement(ns + "TaxScheme", "VAT")
                 )
             )
         ),

         // Položky faktury
         new XElement(ns + "InvoiceLines",
             new XElement(ns + "InvoiceLine",
                 new XElement(ns + "ID", "1"),
                 new XElement(ns + "InvoicedQuantity",
                     new XAttribute("unitCode", "Ks"), quantity),
                 new XElement(ns + "LineExtensionAmountCurr", total),
                 new XElement(ns + "LineExtensionAmount", total),
                 new XElement(ns + "LineExtensionAmountTaxInclusive", total + tax),
                 new XElement(ns + "LineExtensionTaxAmount", tax),
                 new XElement(ns + "UnitPrice", price),
                 new XElement(ns + "UnitPriceTaxInclusive", price * 1.21m),
                 new XElement(ns + "ClassifiedTaxCategory",
                     new XElement(ns + "Percent", "21"),
                     new XElement(ns + "VATCalculationMethod", "0")
                 ),
                 new XElement(ns + "Item",
                     new XElement(ns + "Description", itemDescription)
                 )
             )
         ),

         // Souhrn DPH
         new XElement(ns + "TaxTotal",
             new XElement(ns + "TaxSubTotal",
                 new XElement(ns + "TaxableAmount", total),
                 new XElement(ns + "TaxInclusiveAmount", total + tax),
                 new XElement(ns + "TaxAmount", tax),
                 new XElement(ns + "TaxCategory",
                     new XElement(ns + "Percent", "21")
                 )
             ),
             new XElement(ns + "TaxAmount", tax)
         ),

         // Finální částky
         new XElement(ns + "LegalMonetaryTotal",
             new XElement(ns + "LineExtensionAmount", total),
             new XElement(ns + "TaxExclusiveAmount", total),
             new XElement(ns + "TaxInclusiveAmount", total + tax),
             new XElement(ns + "AlreadyClaimedTaxExclusiveAmount", "0"),
             new XElement(ns + "AlreadyClaimedTaxInclusiveAmount", "0"),
             new XElement(ns + "DifferenceTaxExclusiveAmount", total),
             new XElement(ns + "DifferenceTaxInclusiveAmount", total + tax),
             new XElement(ns + "PayableAmount", total + tax)
         )
     )
 );

 // Uložení s UTF-8 bez BOM
 using (StreamWriter sw = new StreamWriter(filePath, false, new UTF8Encoding(false)))
 {
     doc.Save(sw);
 }
```

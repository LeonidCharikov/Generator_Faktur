using System;
using System.IO;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace Generator_Faktur
{
    class Program
    {
        static void Main()
        {
            while (true)
            {
                // MENU/ Pocatek programu.
                Console.WriteLine("==============================================");
                Console.WriteLine("MENU GENERATORU FAKTUR POMOCI XML");
                Console.WriteLine("==============================================");
                Console.WriteLine("Vyberte akci:");
                Console.WriteLine("1 - Vytvořit fakturu");
                Console.WriteLine("2 - Otevřít existující fakturu");
                Console.WriteLine("3 - Ukončit");
                Console.WriteLine("==============================================");
                Console.Write("Zadejte volbu: ");
                // Nacteni cisla od uzivatele/funkce.
                string choice = Console.ReadLine();

                // IF else vyber metod.
                if (choice == "1")
                {
                    VytvorFakturu();
                }
                else if (choice == "2")
                {
                    OtevriFakturu();
                }
                else if (choice == "3")
                {
                    break;
                }
                else
                {
                    // Jestli uzivatel zada cokoliv jineho krome 1,2,3 tak vypise chybu
                    Console.WriteLine("==============================================");
                    Console.WriteLine("!Neplatná volba!, zkuste to znovu.");
                }
            }
        }

        // Metoda pro vytvoreni Faktury
        static void VytvorFakturu()
        {
            while (true)
            {
                // Primani hodnot od uzivatele, vetsina hodnot se kontrolujou aby nebyly chybne.
                Console.WriteLine("==============================================");
                string id = GetInput("Zadejte ID faktury: ");
                string date = GetValidatedDate("Zadejte datum faktury (dd-MM-yyyy): ");
                string supplierName = GetValidatedName("Zadejte jméno dodavatele: ");
                string supplierICO = GetValidatedICO("Zadejte IČO dodavatele: ");
                string customerName = GetValidatedName("Zadejte jméno zákazníka: ");
                string customerICO = GetValidatedICO("Zadejte IČO zákazníka: ");
                string itemDescription = GetInput("Zadejte popis položky: ");
                decimal quantity = GetValidatedDecimal("Zadejte množství položky: ");
                decimal price = GetValidatedDecimal("Zadejte cenu položky za kus: ");
                decimal total = quantity * price;

                // Vypisuje hodnoty po vlozeni pro jejich kontrolu.
                Console.WriteLine("==============================================");
                Console.WriteLine("\nZadané údaje faktury:");
                Console.WriteLine($"ID: {id}");
                Console.WriteLine($"Datum: {date}");
                Console.WriteLine($"Dodavatel: {supplierName}, IČO: {supplierICO}");
                Console.WriteLine($"Zákazník: {customerName}, IČO: {customerICO}");
                Console.WriteLine($"Položka: {itemDescription}, Množství: {quantity}, Cena: {price}, Celkem: {total}");
                Console.WriteLine("==============================================");

                // Po kontrole uzivatel musi opdovedet jestli data jsou spravna nebo ne.
                Console.Write("Jsou tyto údaje správné? (ano/ne): ");
                string confirmation = Console.ReadLine().Trim().ToLower();

                // Jestli vybere ano, tak bude pokracovat v cyklu pro ukladani souboru.
                if (confirmation == "ano")
                {
                    while (true)
                    {
                        Console.WriteLine("==============================================");
                        Console.Write("Zadejte cestu k složce pro uložení faktury (např. C:\\Faktury): ");
                        string folderPath = Console.ReadLine();

                        // Kontrola, zda cesta končí znakem \ 
                        if (!folderPath.EndsWith("\\"))
                        {
                            folderPath += "\\"; // Přidáme znak \ na konec, aby se jednalo o cestu k složce
                        }

                        // Kontrola, zda složka existuje
                        if (Directory.Exists(folderPath))
                        {
                            // Vytvoření cesty k souboru
                            string filePath = Path.Combine(folderPath, $"{id}.isdoc");

                            // Pokus o uložení souboru
                            if (CreateInvoice(filePath, id, date, supplierName, supplierICO, customerName, customerICO, itemDescription, quantity, price, total))
                            {
                                break; // Úspěšně uloženo, ukončíme smyčku
                            }
                            else
                            {
                                // Pokud se uložení nepovede, nabídneme možnosti
                                Console.WriteLine("==============================================");
                                Console.WriteLine("Nepodařilo se uložit soubor do zadané složky.");
                                Console.Write("Chcete zadat novou cestu nebo uložit lokálně? (new/lok): ");
                                string saveChoice = Console.ReadLine().Trim().ToLower();

                                if (saveChoice == "lok")
                                {
                                    // Uložení do lokální složky "Faktury"
                                    string localPath = Path.Combine(Directory.GetCurrentDirectory(), "Faktury");

                                    // Vytvoření složky, pokud neexistuje
                                    if (!Directory.Exists(localPath))
                                    {
                                        Directory.CreateDirectory(localPath);
                                    }

                                    // Vytvoření cesty k souboru
                                    string localFilePath = Path.Combine(localPath, $"{id}.isdoc");

                                    // Uložení souboru
                                    if (CreateInvoice(localFilePath, id, date, supplierName, supplierICO, customerName, customerICO, itemDescription, quantity, price, total))
                                    {
                                        break; // Úspěšně uloženo, ukončíme smyčku
                                    }
                                }
                                // Pokud uživatel zvolí "novou", smyčka se opakuje a uživatel zadá novou cestu
                            }
                        }
                        else
                        {
                            // Pokud složka neexistuje, dáme uživateli na výběr
                            Console.WriteLine("==============================================");
                            Console.WriteLine("Zadaná složka neexistuje.");
                            Console.Write("Chcete zadat novou cestu nebo uložit lokálně? (novou/lok): ");
                            string saveChoice = Console.ReadLine().Trim().ToLower();

                            if (saveChoice == "lok")
                            {
                                // Uložení do lokální složky "Faktury"
                                string localPath = Path.Combine(Directory.GetCurrentDirectory(), "Faktury");

                                // Vytvoření složky, pokud neexistuje
                                if (!Directory.Exists(localPath))
                                {
                                    Directory.CreateDirectory(localPath);
                                }

                                // Vytvoření cesty k souboru
                                string localFilePath = Path.Combine(localPath, $"{id}.isdoc");

                                // Uložení souboru
                                if (CreateInvoice(localFilePath, id, date, supplierName, supplierICO, customerName, customerICO, itemDescription, quantity, price, total))
                                {
                                    break; // Úspěšně uloženo, ukončíme smyčku
                                }
                            }
                            // Pokud uživatel zvolí "novou", smyčka se opakuje a uživatel zadá novou cestu
                        }
                    }

                    // Po úspěšném uložení faktury ukončíme vnější smyčku a vrátíme se do hlavního menu
                    break;
                }
                else
                {
                    Console.WriteLine("Zadejte údaje znovu.\n");
                }
            }
        }

        // Metoda pro otevreni souboru.
        static void OtevriFakturu()
        {
            // Menu pro otevreni slozky.
            Console.WriteLine("==============================================");
            Console.WriteLine("Vyberte možnost:");
            Console.WriteLine("1 - Otevřít fakturu z lokální složky");
            Console.WriteLine("2 - Otevřít fakturu z vlastní cesty");
            Console.WriteLine("==============================================");
            Console.Write("Zadejte volbu: ");
            // Prijem hodnoty
            string openChoice = Console.ReadLine();

            // Tenhle kousek kodu byl vytvoren pomoci AI, aby zmensit/zlehcit kod protoze varianta 1 a 2 ne skoro vubec nelisi. Proto jsem poprosil AI to spojit.
            // AI pouzil podmineny vyrazi(ternary operator), ktery prirazuje odpoved uzivatele na zacatku = openChoice.
            // Jestli uzivatel zada 1 tak se folderPath se nastavi na cestu Faktury kterou program vytvoril sam.
            // Jinak poprosi uzivatel vlozit vlastni cestu do slozky.
            string folderPath = openChoice == "1"
                ? Path.Combine(Directory.GetCurrentDirectory(), "Faktury")
                : GetInput("Zadejte cestu ke složce s fakturami: ");

            // Kontrola zda slozka existuje
            if (Directory.Exists(folderPath))
            {
                var files = Directory.GetFiles(folderPath, "*.isdoc");
                if (files.Length > 0)
                {
                    Console.WriteLine("==============================================");
                    Console.WriteLine(Directory.GetCurrentDirectory());
                    Console.WriteLine("Dostupné faktury:");
                    // Program projde vsechny nalezene soubory a vypise jejich nazvy.
                    // Kazdy soubor je ocislovan od 1 do files.Lenght.
                    for (int i = 0; i < files.Length; i++)
                    {
                        //Vypise pouze nazev souboru bez cele cesty diky Path.GetFileName. 
                        // Napr '1 - 01.isdoc'
                        Console.WriteLine($"{i + 1} - {Path.GetFileName(files[i])}");
                    }
                    Console.Write("Vyberte fakturu: ");
                    // Nacte vstup od uzivatele.
                    // int.TryParse pokusi prevest vstup na cele cislo INT. Jestli to bude string (abc) vrati false.
                    // Kontrola zda zadane cislo se nachazi v platnem rozsahu. Coz namena ze cislo musi byt vetsi nez 0, protoze seznam souboru zacina od 1. Cislo nesmi byt vetsi nez pocet souboru.
                    if (int.TryParse(Console.ReadLine(), out int fileChoice) && fileChoice > 0 && fileChoice <= files.Length)
                    {
                        // Nacteni vybraneho souboru.
                        // Jelikoz indexovani zacina od 0 tak se musi odecist o 1.
                        ReadInvoice(files[fileChoice - 1]);
                    }
                    else
                    {
                        Console.WriteLine("==============================================");
                        Console.WriteLine("Neplatná volba.");
                    }
                }
                else
                {
                    Console.WriteLine("==============================================");
                    Console.WriteLine("V zadané složce nejsou žádné faktury.");
                }
            }
            else
            {
                Console.WriteLine("==============================================");
                Console.WriteLine("Zadaná složka neexistuje.");
            }
        }

        // Kontroluje aby uzivatel nezadal prazdnou hodnotu.
        static string GetInput(string prompt)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine().Trim();
            } while (string.IsNullOrEmpty(input));
            return input;
        }

        // Kontrola datumu pomoci Regexu kde den a mesic obsahuji prave 2 cisla a rok 4.
        static string GetValidatedDate(string prompt)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine().Trim();
            } while (!Regex.IsMatch(input, "^\\d{2}-\\d{2}-\\d{4}$"));
            return input;
        }

        // Kontrola zda Jmena dodavatele a zakaznika zacinaji velkym pismenem.
        static string GetValidatedName(string prompt)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine().Trim();
            } while (!Regex.IsMatch(input, "^[A-Z][a-zA-Z ]*$"));
            return input;
        }

        // Kontrola zda obe ICO byly zadane spravne, a to tim ze ICO musi obsahovat prave 8 cisel.
        static string GetValidatedICO(string prompt)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine().Trim();
            } while (!Regex.IsMatch(input, "^\\d{8}$"));
            return input;
        }

        // Kontrola zda uzivatel zadal spravne desetinne cislo, jinak se bude opakovat.
        static decimal GetValidatedDecimal(string prompt)
        {
            decimal value;
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine().Trim();
            } while (!decimal.TryParse(input, out value));
            return value;
        }

        // Vytvoreni souboru ve formatu XML.
        static bool CreateInvoice(string filePath, string id, string date, string supplierName, string supplierICO, string customerName, string customerICO, string itemDescription, decimal quantity, decimal price, decimal total)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    Console.WriteLine("==============================================");
                    Console.WriteLine($"Faktura s ID '{id}' již existuje.");
                    Console.Write("Chcete změnit ID faktury? (ano/ne): ");
                    string changeIdChoice = Console.ReadLine().Trim().ToLower();
                    if (changeIdChoice == "ano")
                    {
                        Console.WriteLine("==============================================");
                        id = GetInput("Zadejte nové ID faktury: ");
                        filePath = Path.Combine(Path.GetDirectoryName(filePath), $"{id}.isdoc");
                    }
                    else
                    {
                        Console.WriteLine("==============================================");
                        Console.WriteLine("!CHYBA!: Nelze uložit fakturu se stejným ID.");
                        return false;
                    }
                }

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

                Console.WriteLine($"Faktura byla úspěšně uložena do: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("==============================================");
                Console.WriteLine($"Došlo k !CHYBĚ!: {ex.Message}");
                return false;
            }
        }

        // Nacteni souboru.
        static void ReadInvoice(string filePath)
        {
            //Kontrola zda existuje soubor po te ceste kterou jsme vybrali.
            if (File.Exists(filePath))
            {
                XDocument doc = XDocument.Load(filePath);
                Console.WriteLine("==============================================");
                Console.WriteLine("Obsah faktury:");
                Console.WriteLine(doc);
            }
            else
            {
                Console.WriteLine("==============================================");
                Console.WriteLine("Faktura nebyla nalezena.");
            }
        }
    }
}
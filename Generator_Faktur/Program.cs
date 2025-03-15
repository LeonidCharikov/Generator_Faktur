using System;
using System.IO;
using System.Linq;
// Knihovna pro praci s XML souborem.
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Generator_Faktur
{
    class Program
    {
        static void Main()
        {
            while (true)
            {
                // Menu ktere se zobrazi po zapnuti programu v konzoli kde nabidne moznosti co lze udelat.
                Console.WriteLine("Vyberte akci:");
                Console.WriteLine("1 - Vytvořit fakturu");
                Console.WriteLine("2 - Otevřít existující fakturu");
                Console.WriteLine("3 - Ukončit");
                Console.Write("Zadejte volbu: ");
                // Kontrola co uzivatel napise (1,2,3).
                string choice = Console.ReadLine();

                // Jestli uzivatel vybere variantu 1, tak se zapne script kde uzivatel bude vytvaret fakturu.
                if (choice == "1")
                {
                    // Uzivatel muze zadat cestu do slozky kam se bude ukladat soubor.
                    Console.Write("Zadejte cestu pro uložení faktury (např. C:\\Faktury\\invoice.isdoc): ");
                    // Prijma cestu
                    string filePath = Console.ReadLine();

                    // Pote uzivatel zacne vkladat hodnoty podle toho co znamenaji.
                    while (true)
                    {
                        // ID faktury. 
                        string id = GetInput("Zadejte ID faktury: ");
                        // Datum, je treba aby byla ve formatu dd-mm-yyyy.
                        string date = GetValidatedDate("Zadejte datum faktury (dd-MM-yyyy): ");
                        // Jmeno dodavatele. Musi zacinat s vlekym pismenem.
                        string supplierName = GetValidatedName("Zadejte jméno dodavatele: ");
                        // ICO dodavatele musi obsahovat prave 8 cisel.
                        string supplierICO = GetValidatedICO("Zadejte IČO dodavatele: ");
                        // Jmeno zakaznika take musi zacinat s velkym cislem.
                        string customerName = GetValidatedName("Zadejte jméno zákazníka: ");
                        // ICO zakaznika take musi obsahovat prave 8 cisel.
                        string customerICO = GetValidatedICO("Zadejte IČO zákazníka: ");
                        // Popis slozky napr. Iphone.
                        string itemDescription = GetInput("Zadejte popis položky: ");
                        // Mnozstvi produktu. Musi to byt prave napsano cislem
                        decimal quantity = GetValidatedDecimal("Zadejte množství položky: ");
                        // Cena polozky taky jenom cislem.
                        decimal price = GetValidatedDecimal("Zadejte cenu položky za kus: ");
                        // vypocita celkovou cenu (Pocet * cena).
                        decimal total = quantity * price;

                        //Zde program po vlozeni vsech dat vypise data ktere uzivatel zada pro kontrolu.
                        Console.WriteLine("\nZadané údaje faktury:");
                        Console.WriteLine($"ID: {id}");
                        Console.WriteLine($"Datum: {date}");
                        Console.WriteLine($"Dodavatel: {supplierName}, IČO: {supplierICO}");
                        Console.WriteLine($"Zákazník: {customerName}, IČO: {customerICO}");
                        Console.WriteLine($"Položka: {itemDescription}, Množství: {quantity}, Cena: {price}, Celkem: {total}");

                        // Program se pta zda jsou data spravna a uzivatel musi odpovedet bud ANO/NE.
                        Console.Write("Jsou tyto údaje správné? (ano/ne): ");
                        // Prijma odpoved Uzivatele. Trim smaze mezery a ToLower prevede znaky na mala pismena.
                        string confirmation = Console.ReadLine().Trim().ToLower();

                        // Jestli uzivatel odpovi ANO tak se zapne tenhle skript.
                        if (confirmation == "ano")
                        {
                            // Kontrola, jestli lze vytvorit a ulozit soubor v slozce/ceste kterou zada uzivatel.
                            if (!CreateInvoice(filePath, id, date, supplierName, supplierICO, customerName, customerICO, itemDescription, quantity, price, total))
                            {
                                // Jestli nelze vytvorit/ulozit soubor tak se zepta jestli nechce nahodou aby program ulozil soubor sam.
                                Console.Write("Přejete si uložit fakturu lokálně? (ano/ne): ");
                                // Opdoved se upravi. Pomoci Trim a ToLower.
                                string localSaveChoice = Console.ReadLine().Trim().ToLower();
                                // Jestli uzivatel odpovi ano tak se zapne tenhle kousek kodu.
                                if (localSaveChoice == "ano")
                                {
                                    // Vytvori cestu k slozce Faktury.
                                    //Path.Combine() je metoda ktera spojuje casti cesty do jeden platne cesty k souboru nebo slozce.
                                    string localPath = Path.Combine(Directory.GetCurrentDirectory(), "Faktury");
                                    // Vytvori soubor na zaklade promenne ID a bude to soubor typu .ISDOC
                                    string localFilePath = Path.Combine(localPath, $"{id}.isdoc");
                                    // Vlozi do faktury data.
                                    CreateInvoice(localFilePath, id, date, supplierName, supplierICO, customerName, customerICO, itemDescription, quantity, price, total);
                                }
                            }
                            break;
                        }
                        else
                        {
                            // Jestli uzivatel vybere variantu NE tak musi napsat vsechny data znova.
                            Console.WriteLine("Zadejte údaje znovu.\n");
                        }
                    }
                }
                // Jestli uzivatel vybere z menu variantu 2.
                else if (choice == "2")
                {
                    // Nabidne moznost vlozit vlastni cestu k souboru nebo otevrit lokalni/vlastne vytvorenou aplikaci.
                    Console.WriteLine("Vyberte možnost:");
                    Console.WriteLine("1 - Otevřít fakturu z lokální složky");
                    Console.WriteLine("2 - Otevřít fakturu z vlastní cesty");
                    Console.Write("Zadejte volbu: ");
                    // Prijma hodnotu.
                    string openChoice = Console.ReadLine();

                    // Tenhle kousek kodu byl vytvoren pomoci AI, aby zmensit/zlehcit kod protoze varianta 1 a 2 ne skoro vubec nelisi. Proto jsem poprosil AI to spojit.
                    // AI pouzil podmineny vyrazi(ternary operator), ktery prirazuje odpoved uzivatele na zacatku = openChoice.
                    // Jestli uzivatel zada 1 tak se folderPath se nastavi na cestu Faktury kterou program vytvoril sam.
                    // Jinak poprosi uzivatel vlozit vlastni cestu do slozky,
                    string folderPath = openChoice == "1"
                        ? Path.Combine(Directory.GetCurrentDirectory(), "Faktury")
                        : GetInput("Zadejte cestu ke složce s fakturami: ");

                    // Zde se provadi kontrola zda existuje slozka.
                    if (Directory.Exists(folderPath))
                    {
                        //Ziskani souboru typu .isdoc
                        var files = Directory.GetFiles(folderPath, "*.isdoc");
                        //Kontrola zda byly nalezeny nejake soubory.
                        if (files.Length > 0)
                        {
                            //Vypise dostupne faktury.
                            Console.WriteLine("Dostupné faktury:");
                            // Program projde vsechny nalezene soubory a vypise jejich nazvy.
                            // Kazdy soubor je ocislovan od 1 do files.Lenght.
                            for (int i = 0; i < files.Length; i++)
                            {
                                //Vypise pouze nazev souboru bez cele cesty diky Path.GetFileName. 
                                // Napr '1 - 01.isdoc'
                                Console.WriteLine($"{i + 1} - {Path.GetFileName(files[i])}");
                            }
                            // Zepta se uzivatele jakou slozku otevrit.
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
                            // Jestli uzivatel zada spatnou hodnotu/ID slozky tak vypise chybu.
                            else
                            {
                                Console.WriteLine("Neplatná volba.");
                            }
                        }
                        // Jestli slozka bude existovat ale nebudou tam zadne soubory tak vypise chybu.
                        else
                        {
                            Console.WriteLine("V zadané složce nejsou žádné faktury.");
                        }
                    }
                    // Jestli zadna cesta/slozka neexisutje tak vypise chybu.
                    else
                    {
                        Console.WriteLine("Zadaná složka neexistuje.");
                    }
                }
                // Jestli uzivatel zada z menu hodnotu 3 tak se program skonci.
                else if (choice == "3")
                {
                    break;
                }
                // Jestli uzivatel zada cokoliv krome 1,2,3 tak vypise chybu.
                else
                {
                    Console.WriteLine("Neplatná volba, zkuste to znovu.");
                }
            }
        }

        // Kontroluje aby uzivatel nezadal prazdnou hodnotu.
        static string GetInput(string prompt)
        {
            // Dotaz se bude opakovat dokud uzivatel nezada platny vstup.
            string input;
            do
            {
                // Zobrazi vyzvu uzivatele/dotaz.
                Console.Write(prompt);
                // Trim umoznuje smazat mezery.
                input = Console.ReadLine().Trim();
                // Podminka zda vstup neni prazdny.
            } while (string.IsNullOrEmpty(input));
            // Jakmile uzivatel zada neprazdny retezec, metoda jej vrati.
            return input;
        }

        // Kontrola datumu pomoci Regexu kde den a mesic obsahuji prave 2 cisla a rok 4.
        static string GetValidatedDate(string prompt)
        {
            string input;
            do
            {
                // Zobrazi vyzvu uzivatele/dotaz.
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
                // Zobrazi vyzvu uzivatele/dotaz.
                Console.Write(prompt);
                input = Console.ReadLine().Trim();
                //Regex ktery kontroluje ze prvni pismeno je prave velke a dalsi uz je jedno.
            } while (!Regex.IsMatch(input, "^[A-Z][a-zA-Z ]*$"));
            return input;
        }

        // Kontrola zda obe ICO byly zadane spravne, a to tim ze ICO musi obsahovat prave 8 cisel.
        static string GetValidatedICO(string prompt)
        {
            string input;
            do
            {
                // Zobrazi vyzvu uzivatele/dotaz.
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
                // Zobrazi vyzvu uzivatele/dotaz.
                Console.Write(prompt);
                input = Console.ReadLine().Trim();
                // Pokusi prevest string na decimalni cislo.
            } while (!decimal.TryParse(input, out value));
            return value;
        }

        // Vytvoreni souboru ve formatu XML a ulozit ji.
        static bool CreateInvoice(string filePath, string id, string date, string supplierName, string supplierICO, string customerName, string customerICO, string itemDescription, decimal quantity, decimal price, decimal total)
        {
            try
            {
                //Ziskani cesty k adresari ze zadane cesty k souboru.
                string directoryPath = Path.GetDirectoryName(filePath);
                // Kontrola, zda adresar existuje. Pokud ne, vytvori.
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Kontrola zda nahodou uz existuje soubor s stejnym nazvem.
                if (File.Exists(filePath))
                {
                    // Napise ze faktura s takovym ID uz existuje.
                    Console.WriteLine($"Faktura s ID '{id}' již existuje.");
                    // Zepta jestli nechce prejmenovat soubor.
                    Console.Write("Chcete změnit ID faktury? (ano/ne): ");
                    // Zmensi text a smaze mezery u odpovedi.
                    string changeIdChoice = Console.ReadLine().Trim().ToLower();
                    // Jestli odpovi ANO, tak poprosi zadat novy nazev/ID.
                    if (changeIdChoice == "ano")
                    {
                        id = GetInput("Zadejte nové ID faktury: ");
                        filePath = Path.Combine(directoryPath, $"{id}.isdoc");
                    }
                    else
                    {
                        //Vypise chybu ze nelze ulozit.
                        Console.WriteLine("Chyba: Nelze uložit fakturu se stejným ID.");
                        return false;
                    }
                }

                // Vytvori XML soubor dokument s daty.
                XDocument doc = new XDocument(
                    new XElement("Invoice",
                        new XElement("ID", id),
                        new XElement("Date", date),
                        new XElement("Supplier",
                            new XElement("Name", supplierName),
                            new XElement("ICO", supplierICO)
                        ),
                        new XElement("Customer",
                            new XElement("Name", customerName),
                            new XElement("ICO", customerICO)
                        ),
                        new XElement("Items",
                            new XElement("Item",
                                new XElement("Description", itemDescription),
                                new XElement("Quantity", quantity),
                                new XElement("Price", price),
                                new XElement("Total", total)
                            )
                        )
                    )
                );

                // Ulozeni souboru.
                doc.Save(filePath);
                // Po uspesnem ulozeni souboru vypise celou cestu kde se soubor nachazi.
                Console.WriteLine($"Faktura byla úspěšně uložena do: {filePath}");
                return true;
            }
             // Jestli system/pocitac zabrani vytvareni souboru ve vlastni ceste tak vypise chybu.
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Chyba: Nemáte oprávnění k zápisu do zadané cesty.");
                return false;
            }
            catch (Exception ex)
            {
                // Osetreni obecne vyjimky.
                Console.WriteLine($"Došlo k chybě: {ex.Message}");
                return false;
            }
        }

        // Nacitani faktury z konzole.
        static void ReadInvoice(string filePath)
        {
            //Kontrola zda existuje cesta/soubor.
            if (File.Exists(filePath))
            {
                // Nacteni XML souboru
                XDocument doc = XDocument.Load(filePath);
                Console.WriteLine("Obsah faktury:");
                // Vypis veskerou fakturu dle XDocument
                Console.WriteLine(doc);
            }
            else
            {
                // Jestli neni vypise chybu.
                Console.WriteLine("Faktura nebyla nalezena.");
            }
        }
    }
}
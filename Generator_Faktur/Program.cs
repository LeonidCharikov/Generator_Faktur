using System;
using System.IO;
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

                        // Kontrola, zda cesta konci znakem \ (cesta k slozce).
                        if (!folderPath.EndsWith("\\"))
                        {
                            folderPath += "\\"; // Pridame znak \ na konec, aby se jednalo o cestu k slozce.
                        }

                        // Kontrola, zda slozka existuje.
                        if (Directory.Exists(folderPath))
                        {
                            Console.WriteLine("==============================================");
                            Console.WriteLine("Složka existuje. Můžete pokračovat.");
                            break; 
                        }
                        else
                        {
                            // Pokud slozka neexistuje da vyber.
                            Console.WriteLine("==============================================");
                            Console.WriteLine("Zadaná složka neexistuje.");
                            Console.Write("Chcete zadat novou cestu nebo uložit lokálně? (new/lok): ");
                            string saveChoice = Console.ReadLine().Trim().ToLower();

                            if (saveChoice == "lok")
                            {
                                // Ulozeni do lokalni slozky "Faktury".
                                string localPath = Path.Combine(Directory.GetCurrentDirectory(), "Faktury");

                                // Vytvoreni slozky, pokud neexistuje.
                                if (!Directory.Exists(localPath))
                                {
                                    Directory.CreateDirectory(localPath);
                                }

                                // Vytvoreni cesty k souboru.
                                string filePath = Path.Combine(localPath, $"{id}.isdoc");

                                // Ulozeni souboru.
                                if (CreateInvoice(filePath, id, date, supplierName, supplierICO, customerName, customerICO, itemDescription, quantity, price, total))
                                {
                                    break; 
                                }
                            }
                            // Pokud uzivatel vybere new, cyklus se bude opakovat.
                        }
                    }   
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
                // Kontrola zda existuje soubor se stejnym nazvem.
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

                // Vytvoreni XML dokumentu.
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
                Console.WriteLine("==============================================");
                // Vypis cele cesty, kam se soubor ulozil.
                Console.WriteLine($"Faktura byla úspěšně uložena do: {filePath}");
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("==============================================");
                Console.WriteLine("!CHYBA!: Nemáte oprávnění k zápisu do zadané cesty.");
                return false;
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
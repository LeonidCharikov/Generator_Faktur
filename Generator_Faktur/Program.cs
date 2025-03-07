using System;
using System.IO;
using System.Linq;
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
                Console.WriteLine("Vyberte akci:");
                Console.WriteLine("1 - Vytvořit fakturu");
                Console.WriteLine("2 - Otevřít existující fakturu");
                Console.WriteLine("3 - Ukončit");
                Console.Write("Zadejte volbu: ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Write("Zadejte cestu pro uložení faktury (např. C:\\Faktury\\invoice.isdoc): ");
                    string filePath = Console.ReadLine();

                    while (true)
                    {
                        string id = GetInput("Zadejte ID faktury: ");
                        string date = GetValidatedDate("Zadejte datum faktury (dd-MM-yyyy): ");
                        string supplierName = GetValidatedName("Zadejte jméno dodavatele: ");
                        string supplierICO = GetValidatedICO("Zadejte IČO dodavatele: ");
                        string customerName = GetValidatedName("Zadejte jméno zákazníka: ");
                        string customerICO = GetValidatedICO("Zadejte IČO zákazníka: ");
                        string itemDescription = GetInput("Zadejte popis položky: ");
                        decimal quantity = GetValidatedDecimal("Zadejte množství položky: ");
                        decimal price = GetValidatedDecimal("Zadejte cenu položky: ");
                        decimal total = quantity * price;

                        Console.WriteLine("\nZadané údaje faktury:");
                        Console.WriteLine($"ID: {id}");
                        Console.WriteLine($"Datum: {date}");
                        Console.WriteLine($"Dodavatel: {supplierName}, IČO: {supplierICO}");
                        Console.WriteLine($"Zákazník: {customerName}, IČO: {customerICO}");
                        Console.WriteLine($"Položka: {itemDescription}, Množství: {quantity}, Cena: {price}, Celkem: {total}");

                        Console.Write("Jsou tyto údaje správné? (ano/ne): ");
                        string confirmation = Console.ReadLine().Trim().ToLower();

                        if (confirmation == "ano")
                        {
                            if (!CreateInvoice(filePath, id, date, supplierName, supplierICO, customerName, customerICO, itemDescription, quantity, price, total))
                            {
                                Console.Write("Přejete si uložit fakturu lokálně? (ano/ne): ");
                                string localSaveChoice = Console.ReadLine().Trim().ToLower();
                                if (localSaveChoice == "ano")
                                {
                                    string localPath = Path.Combine(Directory.GetCurrentDirectory(), "Faktury");
                                    string localFilePath = Path.Combine(localPath, $"{id}.isdoc");
                                    CreateInvoice(localFilePath, id, date, supplierName, supplierICO, customerName, customerICO, itemDescription, quantity, price, total);
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
                else if (choice == "2")
                {
                    Console.WriteLine("Vyberte možnost:");
                    Console.WriteLine("1 - Otevřít fakturu z lokální složky");
                    Console.WriteLine("2 - Otevřít fakturu z vlastní cesty");
                    Console.Write("Zadejte volbu: ");
                    string openChoice = Console.ReadLine();

                    string folderPath = openChoice == "1"
                        ? Path.Combine(Directory.GetCurrentDirectory(), "Faktury")
                        : GetInput("Zadejte cestu ke složce s fakturami: ");

                    if (Directory.Exists(folderPath))
                    {
                        var files = Directory.GetFiles(folderPath, "*.isdoc");
                        if (files.Length > 0)
                        {
                            Console.WriteLine("Dostupné faktury:");
                            for (int i = 0; i < files.Length; i++)
                            {
                                Console.WriteLine($"{i + 1} - {Path.GetFileName(files[i])}");
                            }
                            Console.Write("Vyberte fakturu: ");
                            if (int.TryParse(Console.ReadLine(), out int fileChoice) && fileChoice > 0 && fileChoice <= files.Length)
                            {
                                ReadInvoice(files[fileChoice - 1]);
                            }
                            else
                            {
                                Console.WriteLine("Neplatná volba.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("V zadané složce nejsou žádné faktury.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Zadaná složka neexistuje.");
                    }
                }
                else if (choice == "3")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Neplatná volba, zkuste to znovu.");
                }
            }
        }

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

        static bool CreateInvoice(string filePath, string id, string date, string supplierName, string supplierICO, string customerName, string customerICO, string itemDescription, decimal quantity, decimal price, decimal total)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                if (File.Exists(filePath))
                {
                    Console.WriteLine($"Faktura s ID '{id}' již existuje.");
                    Console.Write("Chcete změnit ID faktury? (ano/ne): ");
                    string changeIdChoice = Console.ReadLine().Trim().ToLower();
                    if (changeIdChoice == "ano")
                    {
                        id = GetInput("Zadejte nové ID faktury: ");
                        filePath = Path.Combine(directoryPath, $"{id}.isdoc");
                    }
                    else
                    {
                        Console.WriteLine("Chyba: Nelze uložit fakturu se stejným ID.");
                        return false;
                    }
                }

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

                doc.Save(filePath);
                Console.WriteLine($"Faktura byla úspěšně uložena do: {filePath}");
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Chyba: Nemáte oprávnění k zápisu do zadané cesty.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Došlo k chybě: {ex.Message}");
                return false;
            }
        }

        static void ReadInvoice(string filePath)
        {
            if (File.Exists(filePath))
            {
                XDocument doc = XDocument.Load(filePath);
                Console.WriteLine("Obsah faktury:");
                Console.WriteLine(doc);
            }
            else
            {
                Console.WriteLine("Faktura nebyla nalezena.");
            }
        }
    }
}
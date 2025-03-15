# Program pro vytvoreni/generovani XML souboru.

### Existuje menu ze 3 variant:
```
Console.WriteLine("Vyberte akci:");
Console.WriteLine("1 - Vytvořit fakturu");
Console.WriteLine("2 - Otevřít existující fakturu");
Console.WriteLine("3 - Ukončit");
```

### Pri vytvareni, uzivatel bude vkladat data:\
```
Console.WriteLine($"ID: {id}");
Console.WriteLine($"Datum: {date}");
Console.WriteLine($"Dodavatel: {supplierName}, IČO: {supplierICO}");
Console.WriteLine($"Zákazník: {customerName}, IČO: {customerICO}");
Console.WriteLine($"Položka: {itemDescription}, Množství: {quantity}, Cena: {price}, Celkem: {total}");
```

Uzivatel muze ulozit podle vlastni cesty nebo lokalne v slozce kde se nachazi program.



### Pro druhou variantu uzivatel muze nacist soubor:\
```
 Console.WriteLine("Vyberte možnost:");
 Console.WriteLine("1 - Otevřít fakturu z lokální složky");
 Console.WriteLine("2 - Otevřít fakturu z vlastní cesty");
```


### Pro treti variantu se program ukonci.







### Zpusob jak se uklada soubor:
```
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
```

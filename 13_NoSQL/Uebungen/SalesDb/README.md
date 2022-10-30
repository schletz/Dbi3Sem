# Übungen zu MongoDB Abfragen: Die Sales DB

## Model

![](klassendiagramm_1654.svg)
<small>[PlantUML Source](https://www.plantuml.com/plantuml/uml/hPBTQXin48Nlzoa4lMb30haB5BEaD3KOYXD8Na2M9kjfzRCTpGRQqdUl56b_h7ZlugjbvepEcSzp6rWI3zOqHzGWmFRyMrZWezUXCV3CWhqWx8uiD18eHk-QHXaPWlWw1FOMQ2yjYCsBD0C8V_WHHQtusuZuktX62knI_xDuP4BNvRgHPyjW9Pgvzczq-g8rGGWh9e49WAUnPN5FLVaFzh6oSmmTK2ufZ2lNmv9XaKECGUv90-qOx4gKdhmULChoaBzo-TSW7KURsXePx0EXIbvteD1AavQ6B2vFYCtcuxVB3zljpStrzITnvzLWewATJ577yVlN83iTrQWlHGSEI3BejYrefQkMFCDVnOdEKus3G7aRFJNeEct1vTNFUZUBxl5CNRv8DF9WO4lUfeUYvHDQE6q--MKh8fd1NrnTjIEQA6f5OJvYcyxoVRWLIijRpuq36XC9bTECal9La1onx-Ss9C5t3BmfXJOjDJwQTrfQYDVDadSbAkEvVnfXLYw5IVScJ3F1YbpVYvGgzhkb-BIrNuEforzT7qJDiSRinxEO-4w7qfQpMSUxJ3KfAyOrkw9Biiaf2-SepIquFLZp7m00)</small>

## Beschreibung der Dokumente

### Dokument Customer (Collection *customers*)

- **name:** Name des Kunden.
- **billingAddress:** Rechnungsadresse. Ein Objekt vom Typ *Address*.
  - **street:** Straße.
  - **streetNr:** Hausnummer.
  - **zip:** Postleitzahl.
  - **city:** Stadt.
  - **state:** Bundesland (W, N, B für Wien, Niederösterreich, Burgenland)
- **shippingAddresses:** Lieferadresse. Ist ein Array von Adressen. Es ist immer
  mindestens eine Adresse vorhanden.

### Dokument Product (Collection *products*)

- **ean:** Europäische Artikelnummer. Eine Zahl, die als String gespeichert wird. Ist eindeutig
  in der Collection *products*.
- **name:** Name des Produktes.
- **category:** Produktkategorie.
- **recommendedPrice:** Empfohlener Verkaufspreis.
- **stock**: Aktueller Lagerstand.
- **minStock**: Lagerstand, ab dem nachbestellt werden soll (minimal erlaubter Lagerstand).
- **availableFrom:** Gibt an, ab wann ein Produkt verfügbar ist. Ist das Feld nicht vorhanden, gilt
  keine Einschränkung. Ist ein DateTime Wert (2022-10-30T08:00:00Z bedeutet, dass das Produkt ab
  dem 30.10.2022 um 8h UTC verfügbar ist und daher gelistet werden kann.
- **availableTo:** Gibt an, bis wann Produkt verfügbar ist. Ist das Feld nicht vorhanden, gilt
  keine Einschränkung. Ist ein DateTime Wert (2022-11-30T016:00:00Z bedeutet, dass das Produkt bis
  30.11.2022 um 16h UTC verfügbar ist und daher gelistet werden kann.

### Dokument Order (Collection *orders*)

- **customerId:** Verweist auf die (object) Id des Kunden, der die Bestellung aufgegeben hat.
- **customerName:** Der Name des Kunden, der die Bestellung aufgegeben hat.
- **dateTime:** Datums- und Zeitwert, wann die Bestellung abgeschickt wurde.
- **shippingAddress:** Verwendete Lieferadresse.
- **orderItems:** Liste von OrderItem Einträgen.
  - **product:** Eingebettetes Objekt vom Typ *Product*. Ist das bestellte Produkt.
  - **quantity:** Die Anzahl, wie viel des Produktes bestellt wurden.
  - **itemPrice:** Stückpreis des Produktes, der tatsächlich verrechnet wird. Das kann vom
    empfohlenen Verkaufspreis abweichen, daher wird der Preis auch hier gespeichert.

## Generieren der Datenbank

Die Datenbank wird mit einem .NET Programm erzeugt. Starte das Programm im Ordner
*13_NoSQL/Uebungen/SalesDb/SalesDbGenerator*, indem du die Datei *SalesDbGenerator.csproj* in
Visual Studio (oder einer anderen IDE) öffnest und das Programm ausführst. Alternativ kann
in diesem Ordner auch der Befehl *dotnet run* ausgeführt werden.

## Filterabfragen

Du kannst die folgende Aufgabe auf 2 Arten lösen:

- Eingeben der *find()* Funktion in der Shell von Studio 3T.
- Generieren der Filter in .NET mit *AsQueryable()* und LINQ oder mit der *Find()* Methode.

Unter dem Beispiel ist die korrekte Liste der zurückgegebenen *_id* Werte angegeben. Es werden
nur die unteren 4 Bytes der Id angezeigt.

Falls du die Aufgabe in **.NET** lösen möchtest, kopiere das Programm im Ordner
*13_NoSQL/Uebungen/SalesDb/SalesDbGenerator* in ein anderes Verzeichnis. Du kannst die Aufgaben
mit *AsQueryable()* oder der *Find()* Funktion der Collection lösen. Beachte, dass die Lösung mit
*AsQueryable()* nicht immer möglich ist. Dann muss auf die *Find()* Funktion
zurückgegriffen werden.

**(1)** Gib alle Produkte (Collection *products*) aus, die die Kategorie "Electronics"
(Feld *category*) besitzen.

```
00000003, 00000009
```

**(2)** Gib alle Produkte (Collection *products*) aus, die unter 400 Euro kosten
(Feld *recommendedPrice*).

```
00000006
```

**(3)** Gib alle Produkte (Collection *products*) aus, ab dem 1.1.2022 0h UTC nicht mehr verfügbar
sind (Feld *availableTo*).

```
00000003, 00000008
```

**(4)** Gib alle Produkte (Collection *products*) aus, die keinen Wert für *availableTo* haben.

```
00000001, 00000002, 00000004, 00000005, 00000006, 00000007, 00000009, 0000000a
```

**(5)** Gib alle Produkte (Collection *products*) aus, wo der Lagerstand (Feld *stock*) unter
dem minimalen Lagerstand (Feld *minStock*) liegt. Hinweis: Verwende *$where*.

```
00000002, 00000004, 00000006
```

**(6)** Gib alle Kunden (Collection *customers*) aus, die eine Lieferadresse
(Feld *shippingAddresses*) aus dem Burgenland (*state* ist B) in diesem Array haben.

```
00000001, 00000002, 00000003, 00000005, 00000008, 0000000a
```

**(7)** Gib alle Kunden (Collection *customers*) aus, die mehr als 2 Lieferadressen
(Feld *shippingAddresses*) in diesem Array haben.

```
00000003, 00000004, 00000005, 00000008, 0000000a
```

**(8)** Gib alle Orders (Collection *orders*) aus, wo das Produkt mit der EAN *566572* (Feld *ean*)
im Array *orderItems* vorkommt.


```
00000008, 00000011, 0000001e, 00000062, 00000063, 00000064
```

**(9)** Gib alle Orders (Collection *orders*) aus, wo ein Produkt im Array *orderItems*
vorkommt, dessen *itemPrice* größer als 990 Euro ist.


```
00000004, 0000001f, 00000026, 0000003c, 00000054
```

**(10)** Gib alle Orders (Collection *orders*) aus, wo *kein* Produkt der Kategorie *Sportswear*
(Feld *product.category*) im Array *orderItems* vorkommt.


```
00000002, 00000005, 00000010, 00000012, 00000013, 00000014, 0000001b, 0000001d, 0000001e, 00000026, 
00000027, 0000002a, 0000002b, 0000002c, 0000002e, 00000032, 00000033, 00000037, 0000003a, 00000045, 
00000046, 00000047, 00000049, 0000004a, 0000004d, 00000051, 00000052, 00000058, 0000005a, 0000005b, 
0000005f, 00000064
```

**(11)** Gib alle Orders (Collection *orders*) aus, wo *nur* Produkte der Kategorie *Sportswear* 
(Feld *product.category*) im Array *orderItems* vorkommen.

```
00000009, 0000000c, 0000000f, 00000017, 00000018, 0000001c, 00000023, 00000025, 00000028, 00000029, 
00000038, 00000039, 0000003b, 00000061
```
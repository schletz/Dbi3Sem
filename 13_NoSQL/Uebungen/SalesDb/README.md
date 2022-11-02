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
  keine Einschränkung. Ist ein DateTime Wert. *2022-10-30T08:00:00Z* bedeutet, dass das Produkt ab
  dem 30.10.2022 um 8h UTC verfügbar ist und daher gelistet werden kann.
- **availableTo:** Gibt an, bis wann Produkt verfügbar ist. Ist das Feld nicht vorhanden, gilt
  keine Einschränkung. Ist ein DateTime Wert. *2022-11-30T016:00:00Z* bedeutet, dass das Produkt bis
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

Synchronisiere zuerst das Repository auf deinen Rechner oder rufe - wenn das Repo schon vorhanden
ist - die Datei *resetGit.cmd* auf, damit du die neusten Dateien auf den Rechner bekommst.

### Mit dem .NET Generatorprogramm

Starte das Programm im Ordner
*13_NoSQL/Uebungen/SalesDb/SalesDbGenerator*, indem du die Datei *SalesDbGenerator.csproj* in
Visual Studio (oder einer anderen IDE) öffnest und das Programm ausführst. Alternativ kann
in diesem Ordner auch der Befehl *dotnet run* ausgeführt werden.

### Mit dem Java Generatorprogramm

Starte das Programm im Ordner
*13_NoSQL/Uebungen/SalesDb/salesdb-java*, indem du die Datei *pom.xml* in
IntelliJ (oder einer anderen IDE) öffnest und das Programm ausführst.

### Mit der Shell des Docker Containers

Öffne in Docker Desktop eine Shell des Containers *mongodb* und füge die folgenden Befehle ein.
Bestätige mit Enter. Mit *exit* kannst du die Shell verlassen, wenn alle Befehle erfolgreich
ausgeführt wurden.

```bash
apt-get update && apt-get install wget < /dev/null
cd /home
for collection in customers orders products
do
    wget https://raw.githubusercontent.com/schletz/Dbi3Sem/master/13_NoSQL/Uebungen/SalesDb/Dump/$collection.json &&
    mongoimport --authenticationDatabase=admin --uri="mongodb://root:1234@localhost:27017/salesDb" --file=$collection.json --drop &&
    rm $collection.json
done

```

## Bearbeitung der Aufgaben

Du kannst die Aufgaben auf 3 Arten lösen:

- Eingeben der *find()* bzw. *aggregate()* Funktion in der Shell von Studio 3T.
- Generieren der Filter in .NET mit *AsQueryable()* und LINQ oder mit der *Find()* bzw. *Aggregate()* Methode.
- Generieren der Filter in Java mit der *find()* bzw. *aggregate()* Methode.

Falls du die Aufgabe in **.NET** lösen möchtest, kopiere das Programm im Ordner
*13_NoSQL/Uebungen/SalesDb/SalesDbGenerator* in ein anderes Verzeichnis. Du kannst die Aufgaben
mit *AsQueryable()* oder der *Find()* bzw. *Aggregate()* Methode der Collection lösen.
Beachte, dass die Lösung mit *AsQueryable()* nicht immer möglich ist. Dann muss auf die *Find()*
bzw. *Aggregate()* Funktion zurückgegriffen werden.

Falls du die Aufgabe in **Java** lösen möchtest, kopiere das Programm im Ordner
*13_NoSQL/Uebungen/SalesDb/salesdb-java* in ein anderes Verzeichnis. Öffne danach die Datei
*pom.xml* mit IntelliJ oder einer anderen IDE.

## 1 Filterabfragen

Unter jedem Beispiel ist die korrekte Liste der zurückgegebenen *_id* Werte angegeben. Es werden
nur die letzten 4 Bytes der Id angezeigt.

**(1.1)** Gib alle Produkte (Collection *products*) aus, die die Kategorie "Electronics"
(Feld *category*) besitzen.

```
00000003, 00000009
```

**(1.2)** Gib alle Produkte (Collection *products*) aus, die unter 400 Euro kosten
(Feld *recommendedPrice*).

```
00000006
```

**(1.3)** Gib alle Produkte (Collection *products*) aus, ab dem 1.1.2022 0h UTC nicht mehr verfügbar
sind (Feld *availableTo*).

```
00000003, 00000008
```

**(1.4)** Gib alle Produkte (Collection *products*) aus, die keinen Wert für *availableTo* haben.

```
00000001, 00000002, 00000004, 00000005, 00000006, 00000007, 00000009, 0000000a
```

**(1.5)** Gib alle Produkte (Collection *products*) aus, wo der Lagerstand (Feld *stock*) unter
dem minimalen Lagerstand (Feld *minStock*) liegt. Hinweis: Verwende *$where*.

```
00000002, 00000004, 00000006
```

**(1.6)** Gib alle Kunden (Collection *customers*) aus, die eine Lieferadresse
(Feld *shippingAddresses*) aus dem Burgenland (*state* ist B) in diesem Array haben.

```
00000001, 00000002, 00000003, 00000005, 00000008, 0000000a
```

**(1.7)** Gib alle Kunden (Collection *customers*) aus, die mehr als 2 Lieferadressen
(Feld *shippingAddresses*) in diesem Array haben.

```
00000003, 00000004, 00000005, 00000008, 0000000a
```

**(1.8)** Gib alle Orders (Collection *orders*) aus, wo das Produkt mit der EAN *566572* (Feld *ean*)
im Array *orderItems* vorkommt.


```
00000008, 00000011, 0000001e, 00000062, 00000063, 00000064
```

**(1.9)** Gib alle Orders (Collection *orders*) aus, wo ein Produkt im Array *orderItems*
vorkommt, dessen *itemPrice* größer als 990 Euro ist.


```
00000004, 0000001f, 00000026, 0000003c, 00000054
```

**(1.10)** Gib alle Orders (Collection *orders*) aus, wo *kein* Produkt der Kategorie *Sportswear*
(Feld *product.category*) im Array *orderItems* vorkommt.


```
00000002, 00000005, 00000010, 00000012, 00000013, 00000014, 0000001b, 0000001d, 0000001e, 00000026, 
00000027, 0000002a, 0000002b, 0000002c, 0000002e, 00000032, 00000033, 00000037, 0000003a, 00000045, 
00000046, 00000047, 00000049, 0000004a, 0000004d, 00000051, 00000052, 00000058, 0000005a, 0000005b, 
0000005f, 00000064
```

**(1.11)** Gib alle Orders (Collection *orders*) aus, wo *nur* Produkte der Kategorie *Sportswear* 
(Feld *product.category*) im Array *orderItems* vorkommen.

```
00000009, 0000000c, 0000000f, 00000017, 00000018, 0000001c, 00000023, 00000025, 00000028, 00000029, 
00000038, 00000039, 0000003b, 00000061
```

## 2 Aggregation

Unter jedem Beispiel ist die korrekte Ausgabe. Bei ObjectIds werden nur die letzten 4 Bytes der Id
angezeigt. Die Felder deines Ergebnisses müssen durch die Projektion mit der Lösung ident sein.
Dezimalzahlen werden in der Lösung aus Platzgründen auf 2 Stellen gerundet, in deiner Abfrage soll 
nicht gerundet werden. Die Sortierung ist - außer sie ist angegeben - nicht wichtig.

### Collection *products*

**(2.1)** Gib die Anzahl der Produkte pro Kategorie (Feld *category*) aus.

```javascript
[{"category":"Books","productCount":5},{"category":"Sportswear","productCount":3},{"category":"Electronics","productCount":2}]
```

**(2.2)** Gib die Anzahl der Produkte pro Kategorie (Feld *category*) aus und sortiere das Ergebnis
nach der Kategorie.

```javascript
[{"category":"Books","productCount":5},{"category":"Electronics","productCount":2},{"category":"Sportswear","productCount":3}]
```

**(2.3)** Gib die Anzahl der Produkte pro Kategorie (Feld *category*) aus und sortiere absteigend nach der Anzahl.

```javascript
[{"category":"Books","productCount":5},{"category":"Sportswear","productCount":3},{"category":"Electronics","productCount":2}]
```

### Collection *customers*

**(2.4)** Ermittle die Anzahl der Adressen, die die Kunden im Feld *shippingAddresses* haben.
Verwende *$size*, um die Größe des Arrays zu bestimmen
(siehe <sup>https://www.mongodb.com/docs/manual/reference/operator/aggregation/size/</sup>).

```javascript
[{"id":"00000001","firstname":"Lorena","lastname":"Baganz","addressCount":2},
{"id":"00000002","firstname":"Annabell","lastname":"Roth","addressCount":2},
{"id":"00000003","firstname":"Liana","lastname":"Forst","addressCount":3},
{"id":"00000004","firstname":"Amir","lastname":"Pogorzelski","addressCount":3},
{"id":"00000005","firstname":"Kiana","lastname":"Buschbaum","addressCount":3},
{"id":"00000006","firstname":"Elias","lastname":"Thiomas","addressCount":2},
{"id":"00000007","firstname":"Mick","lastname":"Sprenger","addressCount":2},
{"id":"00000008","firstname":"Felix","lastname":"Kasten","addressCount":3},
{"id":"00000009","firstname":"Christina","lastname":"Kreb","addressCount":1},
{"id":"0000000a","firstname":"Nelly","lastname":"Reusse","addressCount":3}]
```

**(2.5)** Gib pro Kunde eine Liste der Bundesländer (*shippingAddresses.state*) an, die der Kunde
in *shippingAddresses* hat. Die Liste darf das Bundesland nicht mehrfach enthalten. Hinweis:
Verwende *{"$setUnion": [ "$shippingAddresses.state"]}* um eine eindeutige Werteliste zu bekommen.

```javascript
[{"id":"00000001","firstname":"Lorena","lastname":"Baganz","states":["B","N"]},
{"id":"00000002","firstname":"Annabell","lastname":"Roth","states":["B","W"]},
{"id":"00000003","firstname":"Liana","lastname":"Forst","states":["B","N"]},
{"id":"00000004","firstname":"Amir","lastname":"Pogorzelski","states":["N","W"]},
{"id":"00000005","firstname":"Kiana","lastname":"Buschbaum","states":["B","N","W"]},
{"id":"00000006","firstname":"Elias","lastname":"Thiomas","states":["N"]},
{"id":"00000007","firstname":"Mick","lastname":"Sprenger","states":["N","W"]},
{"id":"00000008","firstname":"Felix","lastname":"Kasten","states":["B","N","W"]},
{"id":"00000009","firstname":"Christina","lastname":"Kreb","states":["N"]},
{"id":"0000000a","firstname":"Nelly","lastname":"Reusse","states":["B","N"]}]
```

**(2.6)** Ermittle pro Bundesland die Anzahl der Einträge, die es in *shippingAddresses.state*
hat. Hinweis: Starte mit *$unwind* und gruppiere nachher.

```javascript
[{"state":"B","count":6},{"state":"N","count":12},{"state":"W","count":6}]
```

**(2.7)** Ermittle pro Bundesland die Anzahl der Kunden, wo dieses Bundesland in
*shippingAddresses.state* vorkommt. Jeder Kunde wird nur 1x gezählt, auch wenn er 2 Adressen aus
diesem Bundesland in *shippingAddresses* hat. Hinweis: Gruppiere 2x.

```javascript
[{"state":"B","count":6},{"state":"N","count":9},{"state":"W","count":5}]
```

### Collection *orders*

**(2.8)** Ermittle den Umsatz (Summe von *orderItems.itemPrice* aller orders) pro Kunde.

```javascript
[{"customerId":"00000006","customerFirstname":"Elias","customerLastname":"Thiomas","revenue":15577.8},
{"customerId":"00000008","customerFirstname":"Felix","customerLastname":"Kasten","revenue":10391.9},
{"customerId":"00000007","customerFirstname":"Mick","customerLastname":"Sprenger","revenue":17910.0},
{"customerId":"00000002","customerFirstname":"Annabell","customerLastname":"Roth","revenue":16333.0},
{"customerId":"0000000a","customerFirstname":"Nelly","customerLastname":"Reusse","revenue":24215.8},
{"customerId":"00000003","customerFirstname":"Liana","customerLastname":"Forst","revenue":12504.1},
{"customerId":"00000005","customerFirstname":"Kiana","customerLastname":"Buschbaum","revenue":21656.9},
{"customerId":"00000001","customerFirstname":"Lorena","customerLastname":"Baganz","revenue":13558.2},
{"customerId":"00000004","customerFirstname":"Amir","customerLastname":"Pogorzelski","revenue":17896.4},
{"customerId":"00000009","customerFirstname":"Christina","customerLastname":"Kreb","revenue":12482.0}]
```

**(2.9)** Ermittle den durchschnittlichen Verkaufspreis (Mittelwert von *orderItems.itemPrice* aller
orders) jedes Produktes.

```javascript
[{"ean":"952025","productName":"Intelligent Fresh Soap","avgPrice":531.54},{"ean":"627358","productName":"Tasty Concrete Ball","avgPrice":267.42},
{"ean":"372414","productName":"Gorgeous Granite Table","avgPrice":444.53},{"ean":"817795","productName":"Refined Metal Chips","avgPrice":730.65},
{"ean":"317174","productName":"Licensed Cotton Chips","avgPrice":574.67},{"ean":"656074","productName":"Rustic Granite Bike","avgPrice":977.20},
{"ean":"538974","productName":"Sleek Metal Shirt","avgPrice":970.36},{"ean":"198430","productName":"Unbranded Metal Ball","avgPrice":963.49},{
  "ean":"308463","productName":"Refined Concrete Table","avgPrice":797.5},{"ean":"566572","productName":"Refined Steel Cheese","avgPrice":762.17}]
```

**(2.10)** Ermittle den Umsatz (Summe von *orderItems.itemPrice* aller orders) pro Jahr und Monat, sortiert nach Jahr und Monat.

```javascript
[{"year":2021,"month":1,"revenue":9048.1},{"year":2021,"month":2,"revenue":8471.4},
{"year":2021,"month":3,"revenue":9224.0},{"year":2021,"month":4,"revenue":3911.0},
{"year":2021,"month":5,"revenue":12638.6},{"year":2021,"month":6,"revenue":6819.6},
{"year":2021,"month":7,"revenue":4411.1},{"year":2021,"month":8,"revenue":6339.8},
{"year":2021,"month":9,"revenue":3153.8},{"year":2021,"month":10,"revenue":3234.0},
{"year":2021,"month":11,"revenue":3783.6},{"year":2021,"month":12,"revenue":13784.1},
{"year":2022,"month":1,"revenue":11086.7},{"year":2022,"month":2,"revenue":8713.4},
{"year":2022,"month":3,"revenue":5042.7},{"year":2022,"month":4,"revenue":3102.0},
{"year":2022,"month":5,"revenue":9482.9},{"year":2022,"month":6,"revenue":3961.0},
{"year":2022,"month":7,"revenue":2015.4},{"year":2022,"month":8,"revenue":11130.8},
{"year":2022,"month":9,"revenue":8904.9},{"year":2022,"month":10,"revenue":4227.1},
{"year":2022,"month":11,"revenue":7432.5},{"year":2022,"month":12,"revenue":2607.6}]
```

**(2.11)** Bei welchen OrderItems ist *itemPrice* < 92% von *recommendedPrice*
(*itemPrice* < 0.92 x *recommendedPrice*)?

```javascript
[{"orderId":"00000004","ean":"952025","productName":"Intelligent Fresh Soap","recommendedPrice":536.7,"itemPrice":492.2},
{"orderId":"0000000a","ean":"198430","productName":"Unbranded Metal Ball","recommendedPrice":991.2,"itemPrice":893.9},
{"orderId":"00000010","ean":"308463","productName":"Refined Concrete Table","recommendedPrice":816.2,"itemPrice":745.8},
{"orderId":"00000025","ean":"538974","productName":"Sleek Metal Shirt","recommendedPrice":981.5,"itemPrice":893.3},
{"orderId":"00000037","ean":"656074","productName":"Rustic Granite Bike","recommendedPrice":988.3,"itemPrice":895.0},
{"orderId":"00000044","ean":"308463","productName":"Refined Concrete Table","recommendedPrice":816.2,"itemPrice":740.4},
{"orderId":"00000053","ean":"817795","productName":"Refined Metal Chips","recommendedPrice":742.2,"itemPrice":672.6},
{"orderId":"0000005b","ean":"627358","productName":"Tasty Concrete Ball","recommendedPrice":272.1,"itemPrice":250.3},
{"orderId":"0000005d","ean":"817795","productName":"Refined Metal Chips","recommendedPrice":742.2,"itemPrice":670.4}]
```

**(2.12)** Ermittle den gesamten Umsatz pro Bundesland in *billingAddress.state*. Hinweis: es ist
ein *$lookup* nötig. Gehe zuerst in die *customers* Collection, und führe einen Join über das Feld
*customers.id = orders.customerId* aus. Summiere dann alle Werte von *orderItems.itemPrice* auf, wo der
Kunde den entsprechenden State in *billingAddress.state* hat.

```javascript
[{"state":"B","revenue":47719.2},{"state":"W","revenue":44621.3},{"state":"N","revenue":70185.6}]
```

# PARTITION BY

## Aggregatsfunktionen und PARTITION BY

Zuerst wollen wie die beste Zeit pro Klasse und Bewerb ermitteln. Ein einfaches *GROUP BY*
und die Funktion *MIN()* liefern bereits das korrekte Ergebnis:

```sql
--	Korrekte Ausgabe (12 Zeile(n))		
--	S_KLASSE	E_BEWERB 	BESTEZEIT
--	1AFIT   	100m Lauf	9,98      
--	1AFIT   	400m Lauf	59,4      
--	1AFIT   	5km Lauf 	1248,89   
--	1AHIF   	100m Lauf	11,38     
--	1AHIF   	400m Lauf	54,91     
--	1AHIF   	5km Lauf 	1281,81   
--	1BFIT   	100m Lauf	12,8      
--	1BFIT   	400m Lauf	63,12     
--	1BFIT   	5km Lauf 	1329,52   
--	1BHIF   	100m Lauf	11,65     
--	1BHIF   	400m Lauf	56,02     
--	1BHIF   	5km Lauf 	1307      
SELECT S_Klasse, E_Bewerb, ROUND(MIN(E_Zeit), 2) AS BesteZeit
FROM vSchuelerergebnis
GROUP BY  S_Klasse, E_Bewerb
ORDER BY  S_Klasse, E_Bewerb;
```

Für einen Infoscreen sollen nun die einzelnen Ergebnisse der Teilnehmer angezeigt werden. Als zusätzliche
Information soll die oben ermittelte Bestzeit pro Klasse und Bewerb auch angegeben werden. Mit einer
einfachen Gruppierung scheitern wir allerdings:
```sql
--  ORA-00979: Kein GROUP BY-Ausdruck
--  00979. 00000 -  "not a GROUP BY expression"
--  *Cause:    
--  *Action:
--  Fehler in Zeile: 1 Spalte: 28
SELECT E_Bewerb, S_Klasse, S_ID, S_Zuname, E_Zeit, ROUND(MIN(E_Zeit), 2) AS BesteZeit
FROM vSchuelerergebnis
GROUP BY  S_Klasse, E_Bewerb
ORDER BY  S_Klasse, E_Bewerb;
```

Mit Unterabfragen oder Views können wir die Tabellen kombinieren:
```sql
--	E_BEWERB 	S_KLASSE 	E_ID   	S_ZUNAME  	E_ZEIT 	BESTEZEIT
--	100m Lauf	1AFIT    	1002   	Zuname1011	16,0556	9,9832   
--	100m Lauf	1AFIT    	1004   	Zuname1015	14,1242	9,9832   
--	100m Lauf	1AFIT    	1023   	Zuname1012	16,8605	9,9832   
--	100m Lauf	1AFIT    	1038   	Zuname1012	13,8395	9,9832   
--	100m Lauf	1AFIT    	1051   	Zuname1013	15,2058	9,9832    
--  ...
SELECT se.E_Bewerb, se.S_Klasse, se.E_ID, se.S_Zuname, se.E_Zeit, km.BesteZeit
FROM vSchuelerergebnis se INNER JOIN (
    SELECT S_Klasse, E_Bewerb, ROUND(MIN(E_Zeit), 2) AS BesteZeit
    FROM vSchuelerergebnis
    GROUP BY  S_Klasse, E_Bewerb
) km ON (se.S_Klasse = km.S_Klasse AND se.E_Bewerb = km.E_Bewerb)
ORDER BY E_Bewerb, S_Klasse, E_ID;
```

Das ist natürlich nicht nur für Studierende kompliziert, deswegen haben sich die Datenbankhersteller
Mechanismen überlegt, wie unterschiedliche Gruppierungsgebenen in einer Abfrage kombiniert werden können.
Dieser Mechanismus das *PARTITION BY*:
> It is used to break the data into small partitions and is been separated by a boundary or in simple 
> dividing the input into logical groups. The analytical functions are performed within this partitions.  
> So when the boundaries are crossed then the function get restarted to segregate the data. The "partition by" 
> clause is similar to the "GROUP BY" clause that is used in aggregate functions. They are also known as 
> query partition clause in Oracle. (Vgl. [www.experts-exchange.com](https://www.experts-exchange.com/articles/32791/Partition-By-Clause-in-Oracle.html))

**PARTITION BY erlaubt es uns, innerhalb einer Abfrage nochmals zu Gruppieren und Daten zu aggregieren.
Dabei geben wir "vom Feinen ins Grobe"**.

Folgende Abfrage liefert das gleiche Ergebnis, ist jedoch weitaus angenehmer zu schreiben:
```sql
SELECT 
    E_Bewerb, S_Klasse, E_ID, S_Zuname, E_Zeit,
    MIN(E_Zeit) OVER (PARTITION BY S_Klasse, E_Bewerb) AS BesteZeit
FROM vSchuelerergebnis 
ORDER BY E_Bewerb, S_Klasse, E_ID;
```

## PARTITION BY und GROUP BY

Die Aussage "vom Feinen ins Grobe" wird dann klar, wenn wir die beste Zeit des Schülers und die Beste
Zeit seiner Klasse gegenüberstellen wollen. Die feinere Aussage ist die beste Zeit des Schülers pro Bewerb.
Hier muss mit einer Gruppierung gearbeitet werden, da wir keine Einzelergebnisse haben wollen:

```sql
--	E_BEWERB 	S_KLASSE	S_ID	S_ZUNAME  	BESTZEIT
--	100m Lauf	1AFIT   	1011	Zuname1011	12,962  
--	100m Lauf	1AFIT   	1012	Zuname1012	12,6026 
--	100m Lauf	1AFIT   	1013	Zuname1013	12,9784 
--	100m Lauf	1AFIT   	1014	Zuname1014	13,9207 
--	100m Lauf	1AFIT   	1015	Zuname1015	9,9832  
--  ...
SELECT 
    E_Bewerb, S_Klasse, S_ID, S_Zuname, MIN(E_Zeit) AS BesteZeit
FROM vSchuelerergebnis 
GROUP BY E_Bewerb, S_Klasse, S_ID, S_Zuname
ORDER BY E_Bewerb, S_Klasse, S_ID;
```

Kombinieren wir nun diese Abfrage mit der besten Zeit der Klasse, wird ein Fehler geliefert:
```sql
--  00979. 00000 -  "not a GROUP BY expression"
SELECT 
    E_Bewerb, S_Klasse, S_ID, S_Zuname, MIN(E_Zeit) AS BesteZeit,
    MIN(E_Zeit) OVER (PARTITION BY S_Klasse, E_Bewerb) AS BesteZeitKlasse
FROM vSchuelerergebnis 
GROUP BY E_Bewerb, S_Klasse, S_ID, S_Zuname
ORDER BY E_Bewerb, S_Klasse, S_ID;
```

Was ist hier passiert? Wir gruppieren zuerst nach *E_Bewerb*, *S_Klasse*, *S_ID*, *S_Zuname*. Dadurch steht
die Spalte E_Zeit als Einzelwert nicht mehr zur Verfügung. Bei der Ermittlung der besten Zeit der Klasse sagen
wir allerdings der Datenbank, dass sie aus der Tabelle das Minimum von E_Zeit partitioniert nach Klasse
und Bewerb ermitteln soll. Dies führt zu einem Fehler.

Um die Aufgabe trotzdem lösen zu können, betrachten wir die Spalte *BesteZeit*, die mittels *MIN(E_Zeit)*
berechnet wurde. Sie steht uns zur Verfügung, da sie auch nach der Gruppierung erhalten bleibt. Diese
können wir zur Ermittlung der Bestzeit der Klasse heranziehen. Hier wird das Minimum der Minima der
Einzelergebnisse pro Bewerb, Klasse und Schüler als beste Zeit der Klasse ermittelt:

```sql
--	E_BEWERB 	S_KLASSE	S_ID	S_ZUNAME  	BESTEZEIT	BESTEZEITKLASSE
--	100m Lauf	1AFIT   	1011	Zuname1011	12,962   	9,9832         
--	100m Lauf	1AFIT   	1012	Zuname1012	12,6026  	9,9832         
--	100m Lauf	1AFIT   	1013	Zuname1013	12,9784  	9,9832         
--	100m Lauf	1AFIT   	1014	Zuname1014	13,9207  	9,9832         
--	100m Lauf	1AFIT   	1015	Zuname1015	9,9832   	9,9832         
--  ...
SELECT 
    E_Bewerb, S_Klasse, S_ID, S_Zuname, MIN(E_Zeit) AS BesteZeit,
    MIN(MIN(E_Zeit)) OVER (PARTITION BY S_Klasse, E_Bewerb) AS BesteZeitKlasse
FROM vSchuelerergebnis 
GROUP BY E_Bewerb, S_Klasse, S_ID, S_Zuname
ORDER BY E_Bewerb, S_Klasse, S_ID;
```

## Übungen
1. Geben Sie die Einzelergebnisse und die beste Zeit der Klasse sowie die beste Zeit der Abteilung aus.
   Verwenden sie hierfür zwei MIN Ausdrücke mit unterschiedlichem PARTITION BY Angaben. Es sind die
   Spalten *E_BEWERB, S_KLASSE, E_ID, S_ZUNAME, E_ZEIT, BESTEZEITKLASSE, BESTEZEITABTEILUNG* auszugeben.

1. Geben Sie pro Einzelergebnis die Differenz zur besten Zeit der Klasse aus. Sie können dafür den
   MIN() Ausdruck in einer Berechnung verwenden. Es sind die Spalten 
   *E_BEWERB, S_KLASSE, E_ID, S_ZUNAME, DIFFERENZ* auszugeben.

1. Ermitteln Sie pro Schüler die schlechteste Zeit. Geben Sie zusätzlich die schlechteste Zeit
   der Klasse und des jeweiligen Geschlechts in diesem Bewerb aus. Auszugeben sind die Spalten
   *E_BEWERB, S_KLASSE, S_ID, S_ZUNAME, ZEITKLASSE, ZEITKLASSEGESCHL*.
   Müssen Sie nach *S_Geschl* gruppieren, obwohl es nicht ausgegeben wird? Warum?

1. Sie möchten die beste Zeit des Schülers pro Bewerb mit der durchschnittlichen Zeit der Abteilung in diesem Bewerb ausgeben.
   Können Sie die durchschnittliche Zeit der Abteilung pro Bewerb als Mittelwert der Einzelergebnisse überhaupt ermitteln?
   Welche Gruppenfunktionen kombinieren Sie in Ihrer Abfrage zur Ermittlung des Mittelwertes der Abteilung?
   *E_BEWERB, S_ABTEILUNG, S_ID, S_ZUNAME, BESTEZEIT, AVGABTEILUNG*


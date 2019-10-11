# Ranking

## Wer hat gewonnen?
Wir wollen natürlich wissen, wer beim Sportfest gewonnen - also die niedrigste Zeit pro Bewerb - hat.
Im vorigen Jahr haben wir dies über eine Unterabfrage gelöst:

```sql
--	S_ID          	S_ZUNAME   	S_ABTEILUNG	S_KLASSE	S_GESCHL	E_ID	E_SCHUELER	E_BEWERB 	E_ZEIT
--	1015          	Zuname1015 	FIT        	1AFIT   	w       	1273	1015      	100m Lauf	9,9832
SELECT *
FROM vSchuelerergebnis se1
WHERE E_Zeit = (SELECT MIN(se2.E_Zeit) FROM vSchuelerergebnis se2)
```

Das ist natürlich sinnlos, da es ja einen Gewinner pro Bewerb gibt. Mit Unterabfragen und *UNION* können wir
das Ergebnis mit 3 getrennten Abfragen ermitteln.

```sql
--	S_ID          	S_ZUNAME  	S_ABTEILUNG	S_KLASSE	S_GESCHL	E_ID	E_SCHUELER	E_BEWERB 	E_ZEIT   
--	1003          	Zuname1003	HIF        	1AHIF   	m       	1142	1003      	400m Lauf	54,91    
--	1011          	Zuname1011	FIT        	1AFIT   	m       	1312	1011      	5km Lauf 	1248,8879
--	1015          	Zuname1015	FIT        	1AFIT   	w       	1273	1015      	100m Lauf	9,9832   
SELECT *
FROM vSchuelerergebnis se1
WHERE E_Zeit = (SELECT MIN(se2.E_Zeit) FROM vSchuelerergebnis se2 WHERE E_Bewerb = '100m Lauf')
UNION
SELECT *
FROM vSchuelerergebnis se1
WHERE E_Zeit = (SELECT MIN(se2.E_Zeit) FROM vSchuelerergebnis se2 WHERE E_Bewerb = '400m Lauf')
UNION
SELECT *
FROM vSchuelerergebnis se1
WHERE E_Zeit = (SELECT MIN(se2.E_Zeit) FROM vSchuelerergebnis se2 WHERE E_Bewerb = '5km Lauf')
```

Diese Abfrage ist natürlich schon mühsamer. Zum Glück stellt uns die Datenbank mit *RANK* eine analytische
Funktion bereit, die Ränge ermitteln kann. Um einen Rang zu ermitteln, brauchen wir
- Eine Spalte, dessen Werte sortiert werden sollen.
- Eine Sortierreihenfolge (aufsteigend oder absteigend).
- Eine Gruppierung (*PARTITION BY*), für die jeweils extra gereiht wird. 

Wir ermitteln nun den Rang des jeweiligen Ergebnisses
```sql
SELECT 
    S_ID, S_Zuname, E_Bewerb, E_Zeit, 
    RANK() OVER (ORDER BY E_Zeit) AS Rang
FROM vSchuelerergebnis
ORDER BY S_ID, S_Zuname, E_Bewerb, E_Zeit;
```

Das ist natürlich sinnlos, da die einzelnen Bewerbe ja unterschiedlich gereiht werden sollen. Abhilfe
bringt die sogenannte *PARTITION BY* Klausel. 

```sql
SELECT 
    S_ID, S_Zuname, E_Bewerb, E_Zeit, 
    RANK() OVER (PARTITION BY E_Bewerb ORDER BY E_Zeit) AS Rang
FROM vSchuelerergebnis
ORDER BY S_ID, S_Zuname, E_Bewerb, E_Zeit;
```

Wenn wir nun Platz 1 - 3 ermitteln wollen, müssen wir filtern. Da wir die analytische Funktion nicht
in der WHERE Klausel verwenden können, haben wir 2 Möglichkeiten
- Anlegen einer View *vRanking*.
- Den Rang als Unterabfrage in FROM direkt verwenden.

Der erste Ansatz sieht in SQL so aus:

```sql
CREATE VIEW vRanking AS
SELECT 
    S_ID, S_Zuname, E_Bewerb, E_Zeit, 
    RANK() OVER (PARTITION BY E_Bewerb ORDER BY E_Zeit) AS Rang
FROM vSchuelerergebnis;

SELECT * FROM vRanking WHERE Rang <= 3
ORDER BY E_Bewerb, Rang
```

Möchten wir keine View anlegen, wird die Abfrage für das Ranking einfach in FROM geschrieben:
```sql
SELECT *
FROM (
    SELECT 
        S_ID, S_Zuname, E_Bewerb, E_Zeit, 
        RANK() OVER (PARTITION BY E_Bewerb ORDER BY E_Zeit) AS Rang
    FROM vSchuelerergebnis) Raenge
WHERE Rang <= 3
ORDER BY E_Bewerb, Rang;
```

Natürlich ist *RANK* nicht die einzige Analytische Funktion. Auf [https://www.oracletutorial.com](https://www.oracletutorial.com/oracle-analytic-functions/) 
gibt es eine Liste aller verfügbaren Funktionen:

| Name          	| Description                                                                                                    	| 
| --------------	| ---------------------------------------------------------------------------------------------------------------	| 
| *CUME_DIST*   	| Calculate the cumulative distribution of a value in a set of values                                            	| 
| *DENSE_RANK*  	| Calculate the rank of a row in an ordered set of rows with no gaps in rank values.                             	| 
| *FIRST_VALUE* 	| Get the value of the first row in a specified window frame.                                                    	| 
| *LAG*         	| Provide access to a row at a given physical offset that comes before the current row without using a self-join.	| 
| *LAST_VALUE*  	| Get the value of the last row in a specified window frame.                                                     	| 
| *LEAD*        	| Provide access to a row at a given physical offset that follows the current row without using a self-join.     	| 
| *NTH_VALUE*   	| Get the Nth value in a set of values.                                                                          	| 
| *NTILE*       	| Divide an ordered set of rows into a number of buckets and assign an appropriate bucket number to each row.    	| 
| *PERCENT_RANK*	| Calculate the percent rank of a value in a set of values.                                                      	| 
| *RANK*        	| Calculate the rank of a value in a set of values                                                               	| 
| *ROW_NUMBER*  	| Assign a unique sequential integer starting from 1 to each row in a partition or in the whole result           	| 


## Übungen
1. Reihen Sie die Ergebnisse pro Abteilung und Bewerb, wobei die kleinste Zeit den besten Rang (1) haben
   soll.
1. Reihen Sie die besten Ergebnisse jedes Schülers pro Bewerb innerhalb seiner Klasse. Erstellen Sie dafür
   eine View *vSchuelerBeste* mit dem besten Ergebnis des Schülers pro Bewerb. Danach reihen Sie die Daten dieser View.
1. Lösen Sie die vorige Aufgabe ohne View, indem Sie statt der View eine Unterabfrage im FROM verwenden.
1. Geben Sie - basierend auf der View *vSchuelerBeste* - den Prozentrang (*PERCENT_RANK*) innerhalb der Abteilung
   in diesem Bewerb aus. Was sagt der Prozentrang aus?
1. Geben Sie pro Abteilung die besten 3 Ergebnisse der Frauen im Bewerb *100m Lauf* aus. Lösen Sie das Beispiel
   so, indem Sie zuerst nur den Rang innerhalb von Abteilung und Bewerb der Frauen im 100m Lauf ermitteln.
   Danach erstellen Sie eine Unterabfrage für die Filterung des Ranges. Nach welchen Kriterien müssen Sie
   partitionieren?



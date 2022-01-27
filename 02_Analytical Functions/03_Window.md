## Spezielle Window Functions

### LAG() und LEAD()

Oft möchte man Werte aus dem vorigen Datensatz (z. B. zur Differenzbildung) in die Abfrage
aufnehmen. Mit *LAG* (Verzögerung) wird der in der Sortierung davorstehende, mit *LEAD* der nachfolgende
Wert ausgegeben.

```sql
SELECT vSchuelerergebnis.*,
    LAG(E_Zeit, 1) OVER(ORDER BY E_ID) AS LetzteZeit
FROM vSchuelerergebnis
ORDER BY E_ID;
```

### FIRST_VALUE und LAST_VALUE

Geben den ersten bzw. letzten Wert einer sortierten Folge zurück.

```sql
SELECT vSchuelerergebnis.*,
    FIRST_VALUE(S_ID) OVER(PARTITION BY E_Bewerb ORDER BY E_ID) AS ErsterSchueler
FROM vSchuelerergebnis
ORDER BY E_ID;
```

## RANGE Angaben

Es gibt bei gleitenden Mittelwerten, kumulierten Summen, ... oft die Problemstellung, die
vergangenen Werte bis zum aktuellen Datensatz zu berücksichtigen. Mit *ROWS BETWEEN* kann elegant
eine Funktion auf die vorangegangenen Datensätze angewandt werden. Hier wird das gleitende
Mittel pro Bewerb bis zum aktuellen Ergebnis berechnet.

Natürlich ist ein ORDER BY erforderlich, da die Datenbank wissen muss, welcher Wert vorher und
welcher Wert nachher kommt.

```sql
SELECT vSchuelerergebnis.*,
    AVG(E_Zeit) OVER(PARTITION BY E_Bewerb ORDER BY E_ID ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS Average
FROM vSchuelerergebnis
ORDER BY E_Bewerb, E_ID;
```

Numerische Angaben sind natürlich auch möglich. Hier wird der Mittelwert aus 5 Werten berechnet:
`ROW-2  ROW-1  ROW  ROW+1  ROW+2`

Solche Mittelwerte sind beim Glätten von Kurven wichtig. Die bekannteste Anwendung in den Medien
ist wohl die 7 Tages Inzidenz.

```sql
SELECT vSchuelerergebnis.*,
    AVG(E_Zeit) OVER(PARTITION BY E_Bewerb ORDER BY E_ID ROWS BETWEEN 2 PRECEDING AND 2 FOLLOWING) AS Average
FROM vSchuelerergebnis
ORDER BY E_Bewerb, E_ID;
```

## Übung

Erstelle mit folgenden Anweisungen einen User *Covid* mit dem Passwort *oracle*:

```sql
DROP USER Covid CASCADE;
CREATE USER Covid IDENTIFIED BY oracle;
GRANT CONNECT, RESOURCE, CREATE VIEW TO Covid;
GRANT UNLIMITED TABLESPACE TO Covid;
```

Kopiere das SQL Skript in [covid.sql](covid.sql) in ein Abfragefenster und führe alle
Befehle aus. Achte darauf, dass du im korrekten Schema (Covid) bist.

Führe nun folgende Auswertungen durch:

**(1)** Erstelle eine View *vNeuinfektionen*. Diese View soll 4 Spalten haben: Datum, BundeslandId,
Name, Neuinfektionen. Die Neuinfektionen sind eine berechnete Größe aus dem Wert der Spalte
*BestaetigteFaelleBundeslaender* mit dem vorigen Wert **des entsprechenden Bundeslandes**.

Hinweis: Der vorige Wert ist immer 10 Werte vor dem aktuellen Wert, da pro Tag 9 Bundesländer +
Österreich gesamt vorkommen. Auf https://oracle-base.com/articles/misc/lag-lead-analytic-functions
gibt es Informationen über die Parameter von *LAG()*.

Die Daten sind lückenlos, d. h. der Wert des vorigen Tages ist immer 10 Zeilen über dem aktuellen
Wert.

**(2)** Interessant ist auch das 7 Tagesmittel von Österreich. Berechne daher den Mittelwert
der Neuinfektionen für 7 Tage (= 7 Zeilen). Filtere zuerst nach dem Bundesland mit der ID 10
(Österreich). Das Ergebnis soll in der View *vInzidenz* abrufbar sein. Diese View bietet die
Spalten Datum und Inzidenz an.

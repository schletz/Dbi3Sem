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


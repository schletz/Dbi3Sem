# XML

## Was sind semistrukturierte Formate?

https://www.dbai.tuwien.ac.at/education/ssd/current/folien/ssd-einfuehrung.pdf

## XML Grundlagen

https://www.dbai.tuwien.ac.at/education/ssd/current/slides/2-XML.pdf

## Beispiele

### Bibliothek

Betrachten Sie das folgende XML Dokument:

```xml
<?xml version="1.0" encoding="UTF-8" standalone="yes" ?>
<Bibliothek>
    <Buch id="1000">
        <Titel>C# Programming</Titel>
        <Kategorie>
            Pro
            <NameDE>Programmieren</NameDE>
            <NameEN>Programming</NameEN>
        </Kategorie>
        <Autor>Anders Hejlsberg</Autor>
    </Buch>
    <Buch id="1001">
        <Titel>Java in a Nutshell</Titel>
        <Kategorie>
            Pro
            <NameDE>Programmieren</NameDE>
            <NameEN>Programming</NameEN>
        </Kategorie>
        <Autor>Benjamin Evans, David Flanagan</Autor>
        <Auflage>1</Auflage>
    </Buch>
```

Bearbeiten Sie folgende Fragestellungen:

1. Ist das Dokument wohlgeformt?
2. Schätzen Sie ungefähr ab, wie viel % der Zeichen reine Information und wie viel % der Zeichen
   der "overhead" - also die Strukturinformation - verbraucht.
3. Sind Sie mit der Speicherung der Autoren zufrieden? Geben Sie eine bessere Lösung an.
4. Beachten Sie das Element Auflage. Ist es überall vorhanden? Gibt es Nachteile, wenn dem
   nicht so ist? Denken Sie an ein Programm, welches auf das Element zugreifen möchte.
5. Betrachten Sie die Kategorien. Finden Sie eine effizientere Möglichkeit, die Kategorien
   zu speichern?

### Prüfungsexport

Sie möchten für eine Datenbank (eine Prüfungsverwaltung) als Exportmöglichkeit eine XML Datei
anbieten. Die Datenbank speichert Schüler/innen, Lehrer/innen und Prüfungen, die abgehalten
werden.

Ein JOIN zwischen den Tabellen Schüler, Lehrer und Prüfung liefert folgendes Ergebnis:

| S_ID | S_Zuname  | S_Vorname | S_Gebdat   | Lehrer_ID | L_Zuname | L_Vorname | P_Fach | P_Note |
| ---- | --------- | --------- | ---------- | --------- | -------- | --------- | ------ | ------ |
| 1000 | Lambrecht | Paul      | 03.05.1998 | SZ        | Schletz  | Michael   | DBI    | 1      |
| 1000 | Lambrecht | Paul      | 03.05.1998 | SZ        | Schletz  | Michael   | POS    | 4      |
| 1000 | Lambrecht | Paul      | 03.05.1998 | NAI       | Naimer   | Eva       | D      | 2      |
| 1001 | Witzl     | Amelie    | 14.11.2001 | SZ        | Schletz  | Michael   | POS    | 2      |
| 1001 | Witzl     | Amelie    | 14.11.2001 | PS        | Preissl  | Johann    | DBI    | 4      |
| 1002 | Brugger   | Daniel    |            | PS        | Preissl  | Johann    | DBI    | 5      |

Definieren Sie eine XML Datei, die diese Informationen ausgibt. Überlegen Sie sich folgende
Punkte:

- Wie sieht der Elementbaum aus? Achten Sie darauf, dass Sie so wenig Information wie möglich
  redundant (mehrfach) speichern. Das Root Element soll *PruefungExport* heißen.
- Schreiben Sie in einem (vernünftigen) Editor die XML Datei mit korrektem Header und setzen Sie
  Ihren Elementbaum konkret um.
- Überlegen Sie sich, was mehr Sinn macht: Speicherung der Prüfungen innerhalb des Schülers oder des Lehrers.


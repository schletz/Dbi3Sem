# Reguläre Ausdrücke

Unterlagen auf http://griesmayer.com/?menu=Oracle&semester=Semester_5&topic=07_RegularExpression

## Übung
Gehen Sie auf die Seite https://regex101.com/ und kopieren Sie die Musterstrings unter den Übungsbeispielen
in den dortigen Editor. Finden Sie den regulären Ausdruck, der alle mit gültig gekennzeichneten Strings 
vollständig erkennt (also hervorhebt) und alle als falsch gekennzeichneten Strings ablehnt.

1. Alle Kolleg- und Abendschulklassen (also AIF, BIF, KIF). Da wir im Wintersemester nur ungerade Semester
haben dürfen, müssen diese mit 1, 3, 5, ... beginnen.
```
3CAIF  (gültig)
5CAIF  (gültig)
3AIF   (falsch)
2CAIF  (falsch)
3CAIFx (falsch)
```

2. Alle Räume in C-Gebäude im 2. oder 3. Stock:
```
C1.02 (falsch)
C20   (falsch)
C2.01 (gültig)
C2.11 (gültig)
C4.12 (falsch)
```

3. Datumswerte der Form YYYY-MM-DD
```
2019-01-12 (gültig)
2019-1-12  (falsch)
2019-21-12 (falsch)
19-21-12   (falsch)
19-21-41   (falsch)
```

Überlegen Sie, ob Sie den Wertebereich des Monats und des Tages sinnvoll auf gültige Werte beschränken können.

4. Passwörter, die mindestens 8 Stellen lang sind, mit einem Buchstaben beginnen und einer Ziffer enden. Dazwischen können Buchstaben oder Ziffern vorkommen.
```
Abfde4      (falsch)
AbfdSadfe5  (gültig)
1Abfdsadfe6 (falsch)
1Abfdsadfe  (falsch)
```

5. Namen der Form Vor- und Zuname, wobei der Name aus 2 Teilen besteht und jeder Teil mindestens 2 Zeichen lang sein muss. Jedes Zeichen darf nur A-Z, äöü oder ß sein.
```
Max Mustermann         (gültig)
M Mustermann           (falsch)
Max M                  (falsch)
MaxM                   (falsch)
Max Stefan Mustermann  (falsch)
Max Stefan Muste2rmann (falsch)
```

6. Erweiterung auf 2 oder 3 Namensteile mit den Regeln des vorigen Beispieles.
```
Max Mustermann               (gültig)
Max Mustermann2              (falsch)
Max Stefan Mustermann        (falsch)
Mustermann                   (falsch)
Max Stefan Thomas Mustermann (falsch)
Max Stefan Thomas Mustermann (falsch)
```

7. Der reguläre Ausdruck soll den Inhalt des href Attributes extrahieren, also http://www.orf.at/mysite?id=1&page=2 liefern.
<a href="http://www.orf.at/mysite?id=1&page=2">Linktext</a>


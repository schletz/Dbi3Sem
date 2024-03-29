# Studio 3T: Eine GUI für MongoDB

Damit wir - ähnlich wie in relationalen Datenbanken mit einem SQL Editor - Befehle bequemer
absetzen können, gibt es auch GUI Programme für MongoDB. Ein Tool, welches Intellisense, Autocomplete,
etc. beherrscht ist Studio 3T. Es gibt eine Free Version, die für unsere Zwecke ausreichend ist.

Lade von **https://studio3t.com/free/** die Installationsdateien für dein Betriebssystem und
starte das Setup. Wir wählen die Punkte *default encryption*, *Shell-centric* und
*do not import existing connection settings* aus.

![](studio3t_setup_2108.png)

Beim ersten Programmstart fragt das Programm - auch in der free Version - nach einer Registrierung.
Das wollen wir natürlich nicht unterstützen und geben eine beliebige Adresse, die mit
*@trash-mail.com* endet, ein. Auf [der Seite von Trash-Mail](https://www.trash-mail.com/posteingang/)
können ggf. Mails, die dorthin gesendet werden, abgerufen werden. Als Passwort geben wir auch ein
beliebiges Passwort mit Sonderzeichen ein. Es wird keine Bestätigungsmail verlangt, das Programm
lässt sich sofort nach der Registrierung starten. 

![](studio3t_register_2128.png)

Nach der Installation können wir uns mit der Datenbank verbinden:

![](studio3t_connect_2024.png)

Der Connection String zur Datenbank, die wir im Kapitel [Installation](02_Mongodb_Install.md)
angelegt haben, lautet 

> **mongodb://root:1234@localhost:27017**

Nachdem wir uns mit der Datenbank verbinden haben, wählen wir unsere erstellte *examsDb*. Nun
können wir z. B. das erste Kommando absetzen:

```javascript
db.getCollection("exams").find({"student.nr": 100001})
```

Die Ergebnisse werden als Tabellendarstellung angezeigt. Wir können auch die JSON View aktivieren.

![](studio3t_shell_2122.png)
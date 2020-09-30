# JSON

- Folien zu JSON: https://www.dbai.tuwien.ac.at/education/ssd/current/folien/ssd-json.pdf
- [YT: What is JSON? | JSON Tutorial For Beginners](https://www.youtube.com/watch?v=uw_rP5bV9r0)

## Übung 1

Betrachten Sie das folgende JSON Dokument mit Studierenden. Schreiben Sie Anweisungen, die folgende
Informationen im Browser ausgeben. Gehen Sie dabei wie im Video "JSON Tutorial For Beginners" vor,
indem Sie eine Datei *index.html* erstellen (Position 11:00). Binden Sie danach die Datei *school.js*
mit dem *script* Element ein und geben Sie die folgenden Informationen in der Konsole des Browsers aus.

Hinweis: Erstellen Sie eine Variable und weisen Sie das nachfolgende JSON Dokument als Inhalt zu.

- Geben Sie den Namen der ersten Klasse im Dokument aus.
- Geben Sie den Nachnamen des zweiten Studierenden der ersten Klasse aus.

```javascript
[
    {
        "schoolclass": "5BAIF",
        "students":
        [
            {
                "id": 1001,
                "firstname": "Firstname1",
                "lastname": "Lastname1",
                "dateOfBirth": "2001-02-21",
                "lastLogin": "2020-09-28T23:28:56.782Z"
            },
            {
                "id": 1002,
                "firstname": "Firstname2",
                "lastname": "Lastname2",
                "lastLogin": "2020-09-27T23:28:56.782Z"
            },
            {
                "id": 1003,
                "firstname": "Firstname3",
                "lastname": "Lastname3",
                "dateOfBirth": "2001-02-23",
                "lastLogin": "2020-09-28T14:28:56.782Z"
            }
        ]
    },
    {
        "schoolclass": "5CAIF",
        "students":
        [
            {
                "id": 1004,
                "firstname": "Firstname4",
                "lastname": "Lastname4",
                "dateOfBirth": "2001-02-11",
                "lastLogin": "2020-09-28T09:28:56.782Z"
            },
            {
                "id": 1005,
                "firstname": "Firstname5",
                "lastname": "Lastname5",
                "dateOfBirth": "2001-02-12",
            },
            {
                "id": 1006,
                "firstname": "Firstname6",
                "lastname": "Lastname6",
                "dateOfBirth": "2001-02-13",
                "lastLogin": "2020-09-24T14:28:56.782Z"
            }
        ]
    }
]
```

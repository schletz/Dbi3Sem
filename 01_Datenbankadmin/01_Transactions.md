# Transaktionen

Materialien auf http://griesmayer.com/?menu=Oracle&semester=Semester_3&topic=02_Transaction

In SQL Developer muss unter *Preferences* - *Advanced* der Punkt *Autocommit* für diese Übung deaktiviert 
werden!

## Anlegen von 2 Usern
Wir verbinden uns mit dem User System zur Datenbank und legen 2 Tabellen an.
```sql
CREATE USER User1 IDENTIFIED BY oracle;
GRANT CONNECT, RESOURCE, DBA TO User1;
CREATE USER User2 IDENTIFIED BY oracle;
GRANT CONNECT, RESOURCE, DBA TO User2;
```

## COMMIT mit mehreren Usern
### Operationen unter User1
Nun erstellen wir die Tabelle *GRIESMAYER_ACCOUNTS*
```sql
CREATE TABLE GRIESMAYER_ACCOUNTS
(
  ACCOUNT_ID  INTEGER      NOT NULL PRIMARY KEY,
  FIRST_NAME  VARCHAR(15)  NOT NULL,
  BIRTH_DATE  DATE         NOT NULL,
  AMOUNT      DECIMAL(8,2) NOT NULL
);
```

Damit User2 auf diese Tabelle zugreifen kann, gewähren wir das Recht für User2:
```sql
GRANT SELECT ON User1.GRIESMAYER_ACCOUNTS TO User2;
GRANT DELETE ON User1.GRIESMAYER_ACCOUNTS TO User2;
GRANT INSERT ON User1.GRIESMAYER_ACCOUNTS TO User2;
GRANT UPDATE ON User1.GRIESMAYER_ACCOUNTS TO User2;
```

Nun werden einige Datensätze eingefügt. Sie sind unter User1 sichtbar:
```sql
DELETE FROM   GRIESMAYER_ACCOUNTS;
INSERT INTO GRIESMAYER_ACCOUNTS VALUES (1, 'Thomas', TO_DATE('1973-07-14', 'yyyy-mm-dd'),  500.50);
INSERT INTO GRIESMAYER_ACCOUNTS VALUES (2, 'Andera', TO_DATE('1975-08-20', 'yyyy-mm-dd'),  100.00);
INSERT INTO GRIESMAYER_ACCOUNTS VALUES (3, 'Marion', TO_DATE('1981-12-12', 'yyyy-mm-dd'), -200.00);
INSERT INTO GRIESMAYER_ACCOUNTS VALUES (4, 'Verena', TO_DATE('1977-01-27', 'yyyy-mm-dd'),  900.00);
INSERT INTO GRIESMAYER_ACCOUNTS VALUES (5, 'Kurt',   TO_DATE('1975-02-28', 'yyyy-mm-dd'),  800.40);

SELECT * FROM   GRIESMAYER_ACCOUNTS;

SELECT * FROM  V$TRANSACTION;
```

### Operationen unter User2
Das SELECT zeigt keine Datensätze:
```sql
SELECT * FROM  User1.GRIESMAYER_ACCOUNTS;
```

### Operationen unter User1
Wir setzen ein *COMMIT* ab, damit die Transaktion geschrieben wird.
```sql
COMMIT;
```

### Operationen unter User2
Das SELECT zeigt jetzt die Änderung der Datensätze:
```sql
SELECT * FROM  User1.GRIESMAYER_ACCOUNTS;
```

### Operationen unter User1
Wir aktualisieren den Wert von *AMOUNT* bei User1 ohne COMMIT.
```sql
UPDATE GRIESMAYER_ACCOUNTS SET AMOUNT = 100 WHERE ACCOUNT_ID = 1;
```

### Operationen unter User2
Die Änderung ist noch nicht sichtbar. Wir starten ebenfalls eine Änderung:
```sql
SELECT * FROM  User1.GRIESMAYER_ACCOUNTS;
UPDATE GRIESMAYER_ACCOUNTS SET AMOUNT = 200 WHERE ACCOUNT_ID = 2;
```

Erst beim *COMMIT* des jeweiligen Users wird die Änderung sichtbar.

### Gleichzeitiges Ändern von Datensätzen
Wird der gleiche Datensatz von beiden Usern veränedrt, so wartet das 2. COMMIT bis das erste
COMMIT durchgeführt wurde.


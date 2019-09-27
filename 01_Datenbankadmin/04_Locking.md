# Locking

Unterlagen auf http://griesmayer.com/?menu=Oracle%20OLD&semester=Semester_3&topic=08_Locking

## Busy wait
Legen Sie die Tabelle *GRIESMAYER_ACCOUNTS* wie in 01_Transactions unter **User1** an und geben Sie User2 
alle Rechte:
```sql
CREATE TABLE GRIESMAYER_ACCOUNTS
(
  ACCOUNT_ID  INTEGER      NOT NULL PRIMARY KEY,
  FIRST_NAME  VARCHAR(15)  NOT NULL,
  BIRTH_DATE  DATE         NOT NULL,
  AMOUNT      DECIMAL(8,2) NOT NULL
);

GRANT SELECT ON User1.GRIESMAYER_ACCOUNTS TO User2;
GRANT DELETE ON User1.GRIESMAYER_ACCOUNTS TO User2;
GRANT INSERT ON User1.GRIESMAYER_ACCOUNTS TO User2;
GRANT UPDATE ON User1.GRIESMAYER_ACCOUNTS TO User2;

DELETE FROM   GRIESMAYER_ACCOUNTS;
COMMIT;
INSERT INTO GRIESMAYER_ACCOUNTS VALUES (1, 'Thomas', TO_DATE('1973-07-14', 'yyyy-mm-dd'),  500.50);
INSERT INTO GRIESMAYER_ACCOUNTS VALUES (2, 'Andera', TO_DATE('1975-08-20', 'yyyy-mm-dd'),  100.00);
INSERT INTO GRIESMAYER_ACCOUNTS VALUES (3, 'Marion', TO_DATE('1981-12-12', 'yyyy-mm-dd'), -200.00);
INSERT INTO GRIESMAYER_ACCOUNTS VALUES (4, 'Verena', TO_DATE('1977-01-27', 'yyyy-mm-dd'),  900.00);
INSERT INTO GRIESMAYER_ACCOUNTS VALUES (5, 'Kurt',   TO_DATE('1975-02-28', 'yyyy-mm-dd'),  800.40);
```

Nun aktualisieren wir einen Datensatz aus GRIESMAYER_ACCOUNTS unter **User1**:
```sql
UPDATE GRIESMAYER_ACCOUNTS
SET FIRST_NAME = 'Klaus'
WHERE ACCOUNT_ID = 1;
```

Wir aktualisieren den selben Datensatz unter **User2**:
```sql
UPDATE GRIESMAYER_ACCOUNTS
SET FIRST_NAME = 'Michael'
WHERE ACCOUNT_ID = 1;
```

Nun entsteht der *Busy wait*. Sie müssen nun unter User1 und User2 ein *COMMIT* absetzen, um die Daten
schreiben zu können. Um Locks zu beobachten, führen Sie folgendes SQL Statement aus:
```sql
SELECT SID,
       DECODE ( block,
                    0, 'Not Blocking',
                    1, 'Blocking',
                    2, 'Global'
               ) status,
        DECODE (lmode,
                    0, 'None',
                    1, 'Null',
                    2, 'Row-S (SS)',
                    3, 'Row-X (SX)',
                    4, 'Share',
                    5, 'S/Row-X (SSX)',
                    6, 'Exclusive', TO_CHAR(lmode)
                ) mode_held,
        DECODE (REQUEST,
                    0, 'None',
                    1, 'Null',
                    2, 'Row-S (SS)',
                    3, 'Row-X (SX)',
                    4, 'Share',
                    5, 'S/Row-X (SSX)',
                    6, 'Exclusive', TO_CHAR(lmode)
                ) mode_request
FROM   v$lock
WHERE  TYPE = 'TM';
```

## Deadlock

Unter **User1** führen Sie folgende Statements aus:
```sql
UPDATE GRIESMAYER_ACCOUNTS
SET FIRST_NAME = 'Klaus'
WHERE ACCOUNT_ID = 1;
UPDATE GRIESMAYER_ACCOUNTS
SET FIRST_NAME = 'Klaus'
WHERE ACCOUNT_ID = 3;
```

**User2** führt folgendes aus:
```sql
UPDATE GRIESMAYER_ACCOUNTS
SET FIRST_NAME = 'Klaus'
WHERE ACCOUNT_ID = 2;
UPDATE GRIESMAYER_ACCOUNTS
SET FIRST_NAME = 'Klaus'
WHERE ACCOUNT_ID = 1;
```

Wenn nun unter **User1** der Datensatz 2 ebenfalls aktualisiert wird, entsteht ein *Dead Lock*:
```sql
UPDATE GRIESMAYER_ACCOUNTS
SET FIRST_NAME = 'Klaus'
WHERE ACCOUNT_ID = 2;
```

Die Datenbank löst den Deadlock automatisch auf, indem das Statement abgebrochen wird.

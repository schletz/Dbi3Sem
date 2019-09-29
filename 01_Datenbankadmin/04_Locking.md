# Locking

Unterlagen auf http://griesmayer.com/?menu=Oracle%20OLD&semester=Semester_3&topic=08_Locking

## Busy wait
Legen Sie die Tabelle *GRIESMAYER_ACCOUNTS* wie unter *01_Transactions* mit dem **User1** an und geben Sie User2
alle Rechte auf diese Tabelle:
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
COMMIT;
```

Führen Sie nun die folgenden SQL Anweisungen in SQL Developer unter dem entsprechenden User aus:

| User1                                                                      	| User2                                                                        	| 
| ---------------------------------------------------------------------------	| -----------------------------------------------------------------------------	| 
| *UPDATE GRIESMAYER_ACCOUNTS SET FIRST_NAME = 'Klaus' WHERE ACCOUNT_ID = 1;*	|                                                                              	| 
|                                                                            	| *UPDATE User1.GRIESMAYER_ACCOUNTS SET FIRST_NAME = 'Michael' WHERE ACCOUNT_ID = 1;*	| 

Es entsteht ein *Busy wait*. Sie müssen nun unter *User1* ein *COMMIT* absetzen, um die Daten
schreiben zu können. Um den Lock zu beobachten, führen Sie vor dem *COMMIT* folgendes SQL Statement 
unter User1 aus:

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

Sie bekommen folgende Ausgabe:

| SID	| STATUS      	| MODE_HELD 	| MODE_REQUEST	|
| ---	| ------------	| ----------	| ------------	|
| 261	| Not Blocking	| Row-X (SX)	| None        	|
| 261	| Not Blocking	| Row-X (SX)	| None        	|


Nach dem *COMMIT* unter *User1* verschwindet zwar der Busy Wait, der Datenstz mit der Account ID 1 ist jedoch
durch User2 weiterhin gesperrt:

| SID	| STATUS      	| MODE_HELD 	| MODE_REQUEST |
| ---	| ------------	| ----------	| ------------ |
| 261	| Not Blocking	| Row-X (SX)	| None         |

Erst nach einem *COMMIT* unter *User2* verschwindet der Lock.

## Deadlock
Führen Sie in SQLDeveloper folgende Anweisungen unter den entsprechenden Usern aus:

| User1                                                                                                                                                  	| User2                                                                                                                                                  	| 
| -------------------------------------------------------------------------------------------------------------------------------------------------------	| -------------------------------------------------------------------------------------------------------------------------------------------------------	| 
| *UPDATE GRIESMAYER_ACCOUNTS SET FIRST_NAME = 'Klaus' WHERE ACCOUNT_ID = 1;*<br>*UPDATE GRIESMAYER_ACCOUNTS SET FIRST_NAME = 'Klaus' WHERE ACCOUNT_ID = 3;*	|                                                                                                                                                        	| 
|                                                                                                                                                        	| *UPDATE User1.GRIESMAYER_ACCOUNTS SET FIRST_NAME = 'Michael' WHERE ACCOUNT_ID = 2;*<br>*UPDATE User1.GRIESMAYER_ACCOUNTS SET FIRST_NAME = 'Michael' WHERE ACCOUNT_ID = 1;*	| 
| *UPDATE GRIESMAYER_ACCOUNTS SET FIRST_NAME = 'Klaus' WHERE ACCOUNT_ID = 2;*                                                                            	|                                                                                                                                                        	| 

Nachdem Sie die Anweisungen unter User2 ausgeführt haben, ist dieser im Zustand *Busy wait*. Führen Sie nun 
unter User1 das *UPDATE* Statement aus, bekommt einer der beiden User eine Fehlermeldung:
```
Fehler beim Start in Zeile: 2 in Befehl -
UPDATE User1.GRIESMAYER_ACCOUNTS SET FIRST_NAME = 'Michael' WHERE ACCOUNT_ID = 1
Fehlerbericht -
ORA-00060: Deadlock beim Warten auf Ressource festgestellt
```

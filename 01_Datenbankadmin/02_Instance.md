# Instance

Materialien auf http://griesmayer.com/?menu=Oracle&semester=Semester_5&topic=04_Instance

## Befehle in der Bash (in der VM)
Ausgeben der SID der Oracle Installation:
```
echo $ORACLE_SID
orcl12c
```

## Befehle in SQLPlus
Aus der Bash wird SQLPlus mit folgendem Befehl gestartet. Der User *sys* mit dem Kennwort *oracle* meldet
sich in der Rolle *sysdba* an:
```
sqlplus sys/oracle as sysdba
```

### Datenbank normal herunterfahren (shutdown normal):
Es wird gewartet, bis die Verbindungen beendet werden. Verbinden Sie sich
dafür in SQL Developer mit einem beliebigen User zur Datenbank. Die nachfolgenden Befehle erstellen eine
Tabelle und fügen Datensätze ein. Durch das fehlende Commit läuft die Transaktion noch.
```sql
CREATE TABLE DEMOTABLE (ID INTEGER PRIMARY KEY);
INSERT INTO DEMOTABLE VALUES (1);
INSERT INTO DEMOTABLE VALUES (2);
INSERT INTO DEMOTABLE VALUES (3);
INSERT INTO DEMOTABLE VALUES (4);

SELECT * FROM v$TRANSACTION
```

Nun versuchen Sie, die Datenbank normal mit *shutdown normal* herunterzufahren. Erst nach einem *COMMIT* und
einem Disconnect des Users wird die Zeile *Pluggable Database closed.* erscheinen.
```
SQL> shutdown normal;
Pluggable Database closed.
SQL> startup;     
```

### Shutdown Immediate
Fügen Sie nun erneut Werte in die Tabelle Demotable ein. Danach wird die Datenbank mit *shutdown immediate*
sofort heruntergefahren. Transaktionen werden abgebrochen und Verbindungen werden geschlossen.
```
SQL> shutdown immediate;  
Pluggable Database closed.
SQL> startup;
```

Nach dem erneuten Hochfahren erkennen Sie mittels SELECT, dass die Werte nicht mehr geschrieben wurden.
Die Transkation wurde rückgängig gemacht.

## Andere Befehle in der Bash
Ausgeben des Homeverzeichnisses von Oracle
```
echo $ORACLE_HOME
```

Suche der Konfigurationsdatei und Laden im Editor
```
find $ORACLE_HOME -name spfile*.ora
/u01/app/oracle/product/12.2/db_1/dbs/spfileorcl12c.ora
nano /u01/app/oracle/product/12.2/db_1/dbs/spfileorcl12c.ora
```


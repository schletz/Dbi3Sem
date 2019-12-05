# Der Index in einer Datenbank

![](https://d117h1jjiq768j.cloudfront.net/images/default-source/default-album/tutorialimages-album/odbc-album/ruseindxa.gif?sfvrsn=1)

<sup>Quelle: https://www.progress.com/tutorials/odbc/using-indexes</sup>

## Anlegen eines Index
```sql
CREATE INDEX idx ON Table(Cols)
```

## Anlegen der Tabellen und Generieren der Musterdaten
```sql
drop table sales_person;
drop table sales;

CREATE TABLE SALES_PERSON
(
	SALES_PERSON_ID NUMBER PRIMARY KEY,
	FIRST_NAME      VARCHAR(20),
	LAST_NAME       VARCHAR(20),
	GENDER          CHAR(1),
	BIRTH_DATE      DATE
);

INSERT INTO SALES_PERSON VALUES (1, 'Thomas',    'Griesmayer', 'M', to_date('1973-07-14', 'yyyy-mm-dd'));
INSERT INTO SALES_PERSON VALUES (2, 'Andrea',    'Griesmayer', 'F', to_date('1973-01-12', 'yyyy-mm-dd'));
INSERT INTO SALES_PERSON VALUES (3, 'Werner',    'Berger',     'M', to_date('1969-02-20', 'yyyy-mm-dd'));
INSERT INTO SALES_PERSON VALUES (4, 'Alexander', 'Lober',      'M', to_date('1975-02-19', 'yyyy-mm-dd'));
INSERT INTO SALES_PERSON VALUES (5, 'Thomas',    'Lober',      'M', to_date('1954-05-22', 'yyyy-mm-dd'));
INSERT INTO SALES_PERSON VALUES (6, 'Anderas',   'Huberter',   'M', to_date('1969-03-16', 'yyyy-mm-dd'));
commit;



CREATE TABLE SALES
(
	SALES_DATE      DATE,
	SALES_TIME      NUMBER,
	PRODUCT_TYPE    VARCHAR(20),
	SALES_PERSON_ID NUMBER,
	PRODUCTS        NUMBER,
	REVENUE         DECIMAL(8,2)
);


CREATE OR REPLACE PROCEDURE TESTDATA AS 
X INTEGER;
PRD INTEGER;
ANZ INTEGER;
BEGIN
  FOR X IN 1..200000 LOOP
    PRD := DBMS_RANDOM.VALUE(1, 6);
    IF (DBMS_RANDOM.VALUE(0,1) < 0.7) THEN
      ANZ := 1;
    ELSE
      ANZ := DBMS_RANDOM.VALUE(2, 10);
    END IF;

    INSERT INTO SALES
    SELECT TO_DATE(TRUNC(DBMS_RANDOM.VALUE(TO_CHAR(TO_DATE('01-01-2000','dd-mm-yyyy'),'J'),TO_CHAR(TO_DATE('21-12-2012','dd-mm-yyyy'),'J'))),'J') as "SALES_DATE",
           CAST(DBMS_RANDOM.VALUE(0, 23) as INTEGER)*100+CAST(DBMS_RANDOM.VALUE(0, 59) as INTEGER) as "SALES_TIME",
           CASE PRD
             WHEN 1 THEN 'Bike City'
             WHEN 2 THEN 'Bike Cross'
             WHEN 3 THEN 'Bike Kids'
             WHEN 4 THEN 'Bike Mountain'
             WHEN 5 THEN 'Car Regular'
             WHEN 6 THEN 'Car VAN'
          END as "PRODUCT_TYPE",
          CAST(DBMS_RANDOM.VALUE(1, 6) as INTEGER) as "SALES_PERSON_ID",
          ANZ as "PRODUCTS",
          CASE PRD
             WHEN 1 THEN (900 * ANZ)
             WHEN 2 THEN (1200 * ANZ)
             WHEN 3 THEN (250 * ANZ)
             WHEN 4 THEN (1550 * ANZ)
             WHEN 5 THEN (15300 * ANZ)
             WHEN 6 THEN (18950 * ANZ)
          END as "REVENUE"
    FROM  DUAL;
  END LOOP;
END TESTDATA;

-- danach im Worksheet folgenden Befehl ausführen
exec TESTDATA();
```

## Beispiele für den Execution Plan
Betrachten Sie bei jedem Select den Execution Plan.

```sql
SELECT  PRODUCT_TYPE,
        sum(REVENUE) as REVENUE
FROM    SALES
WHERE   SALES_PERSON_ID = 2
GROUP BY PRODUCT_TYPE
ORDER BY PRODUCT_TYPE;

CREATE INDEX idx1 ON SALES (SALES_PERSON_ID);
commit;
SELECT  PRODUCT_TYPE,
        sum(REVENUE) as REVENUE
FROM    SALES
WHERE   SALES_PERSON_ID = 2
GROUP BY PRODUCT_TYPE
ORDER BY PRODUCT_TYPE;

SELECT  PRODUCT_TYPE,
        sum(REVENUE) as REVENUE
FROM    SALES
WHERE   SALES_PERSON_ID = -9999
GROUP BY PRODUCT_TYPE
ORDER BY PRODUCT_TYPE;

CREATE INDEX idx2 ON SALES (PRODUCT_TYPE);
commit;
SELECT  PRODUCT_TYPE,
        sum(REVENUE) as REVENUE
FROM    SALES
WHERE   SALES_PERSON_ID = 2
GROUP BY PRODUCT_TYPE
ORDER BY PRODUCT_TYPE;

DROP INDEX idx1;
DROP INDEX idx2;
SELECT  PRODUCT_TYPE,
        sum(REVENUE) as REVENUE
FROM    SALES
WHERE   SALES_PERSON_ID = 2 and
        PRODUCT_TYPE LIKE 'Bike%'
GROUP BY PRODUCT_TYPE
ORDER BY PRODUCT_TYPE;

CREATE INDEX idx1 ON SALES (PRODUCT_TYPE);
CREATE INDEX idx2 ON SALES (SALES_PERSON_ID);
commit;

SELECT  PRODUCT_TYPE,
        sum(REVENUE) as REVENUE
FROM    SALES
WHERE   SALES_PERSON_ID = 2 and
        PRODUCT_TYPE LIKE 'Bike%'
GROUP BY PRODUCT_TYPE
ORDER BY PRODUCT_TYPE;

SELECT  PRODUCT_TYPE,
        sum(REVENUE) as REVENUE
FROM    SALES
WHERE   SALES_PERSON_ID = 2 and
        PRODUCT_TYPE = 'Bike Kids'
GROUP BY PRODUCT_TYPE
ORDER BY PRODUCT_TYPE;

SELECT  PRODUCT_TYPE,
        sum(REVENUE) as REVENUE
FROM    SALES
WHERE   SALES_PERSON_ID between 0 and 100 or
        PRODUCT_TYPE = 'Bike Kids'
GROUP BY PRODUCT_TYPE
ORDER BY PRODUCT_TYPE;

SELECT  PRODUCT_TYPE,
        sum(REVENUE) as REVENUE
FROM    SALES s,
        SALES_PERSON sp
GROUP BY PRODUCT_TYPE;

CREATE INDEX idx3 ON SALES_PERSON (SALES_PERSON_ID);
SELECT  PRODUCT_TYPE,
        sum(REVENUE) as REVENUE
FROM    SALES s
        INNER JOIN
        SALES_PERSON sp
        ON s.SALES_PERSON_ID = sp.SALES_PERSON_ID
GROUP BY PRODUCT_TYPE;

DROP INDEX idx1;
DROP INDEX idx2;
DROP INDEX idx3;
SELECT  PRODUCT_TYPE,
        sum(REVENUE) as REVENUE
FROM    SALES s,
        SALES_PERSON sp
WHERE   s.SALES_PERSON_ID = sp.SALES_PERSON_ID
GROUP BY PRODUCT_TYPE;


drop index idx_date_person;
drop index idx_salesperson; 
drop index  idx_salesdate ;
drop index idx_person_date;

SELECT  *
FROM    SALES
WHERE   SALES_PERSON_ID = 2;
-- Full Table Scan!

CREATE INDEX idx_salesperson ON SALES (SALES_PERSON_ID);
SELECT  *
FROM    SALES
WHERE   SALES_PERSON_ID = 2;
-- Full Table Scan, da Spalte zu wenig selektiv!

drop index idx_salesperson; 
SELECT  *
FROM    SALES
WHERE   sales_date = to_date('12-12-2011','dd-mm-yyyy');
-- Full Table Scan

CREATE INDEX idx_salesdate ON SALES (sales_date);
SELECT  *
FROM    SALES
WHERE   sales_date = to_date('12-12-2011','dd-mm-yyyy');
-- Index (RANGE) Scan, da Abfrage auf Datum sehr selektiv ist

drop index  idx_salesdate ;
CREATE INDEX idx_person_date ON SALES (SALES_PERSON_ID,sales_date);
SELECT  *
FROM    SALES
WHERE   sales_date = to_date('12-12-2011','dd-mm-yyyy');
-- bei 400000 Records in der Sales Tabelle (ansonsten Full Table Scan): 
-- index zugriff mittels skip scan (oder full index scan), da der index nicht 
-- nach der Spalte sortiert ist, nach welcher gefiltert wird

drop index idx_person_date;
create index idx_date_person ON SALES (sales_date,SALES_PERSON_ID);
SELECT  * 
FROM    SALES
WHERE   sales_date = to_date('12-12-2011','dd-mm-yyyy');
-- index scan zugriff, da der index nach der spalte der where bedingung sortiert ist

-- nur index scan, da daten von Tabelle nicht benötigt werden (kein Zugriff über RowID auf Tabelle)
SELECT  Count(*)
FROM    SALES
WHERE   sales_date = to_date('12-12-2011','dd-mm-yyyy');
drop index idx_date_person;
```

Weitere Informationen auf https://use-the-index-luke.com/sql/explain-plan/oracle/operations, Punkt "Index and Table Access".
Unterlagen auf http://griesmayer.com/content/Oracle%20OLD/Semester_3/09_Index/Folie_Index.pdf

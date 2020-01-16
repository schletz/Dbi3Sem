# Locking

Unterlagen auf http://www.griesmayer.com/?menu=Oracle&semester=Semester_7&topic=06_Locking

Autocommit muss deaktiviert sein. Es werden 2 User benötigt:

```sql
CREATE USER User1 IDENTIFIED BY oracle;
GRANT DBA,CONNECT TO User1;
CREATE USER User2 IDENTIFIED BY oracle;
GRANT DBA,CONNECT TO User2;
```

## Abfragen unter User1

```sql
/*************************************************/
/**                 User1 USER !!!          */
/*************************************************/

/***************/
/** BUSY WAIT **/
/***************/

/* 01 */
   SET AUTOCOMMIT OFF;

   DROP TABLE CUSTOMER;

   CREATE TABLE CUSTOMER
   (
      CUSTOMER_ID     INTEGER PRIMARY KEY,
      FIRST_NAME      VARCHAR(30),
      BALANCE         DECIMAL(12,2)
   );

   INSERT INTO CUSTOMER VALUES (1, 'Fritz',   800);
   INSERT INTO CUSTOMER VALUES (2, 'Susi',   1000);
   INSERT INTO CUSTOMER VALUES (3, 'Werner', -200);
   INSERT INTO CUSTOMER VALUES (4, 'Hans',      0);
   INSERT INTO CUSTOMER VALUES (5, 'Alex',    400);
   INSERT INTO CUSTOMER VALUES (6, 'Thomas',  100);

   COMMIT;
   
   GRANT SELECT,DELETE,INSERT,UPDATE ON CUSTOMER TO User2;
/* 01 */

/* 02*/
   SELECT *
   FROM   CUSTOMER
   WHERE  CUSTOMER_ID = 1;

   SELECT *
   FROM   CUSTOMER
   WHERE  CUSTOMER_ID = 2;

   UPDATE CUSTOMER
   SET    BALANCE = BALANCE - 100
   WHERE  CUSTOMER_ID = 1;

   UPDATE CUSTOMER
   SET    BALANCE = BALANCE + 100
   WHERE  CUSTOMER_ID = 2;
/* 02 */

/* 04 */
   COMMIT;
/* 04 */

/**************/
/** DEADLOCK **/
/**************/

/* 06 */
   SET AUTOCOMMIT OFF;

   DROP TABLE CUSTOMER;

   CREATE TABLE CUSTOMER
   (
      CUSTOMER_ID     INTEGER PRIMARY KEY,
      FIRST_NAME      VARCHAR(30),
      BALANCE         DECIMAL(12,2)
   );

   INSERT INTO CUSTOMER VALUES (1, 'Fritz',   800);
   INSERT INTO CUSTOMER VALUES (2, 'Susi',   1000);
   INSERT INTO CUSTOMER VALUES (3, 'Werner', -200);
   INSERT INTO CUSTOMER VALUES (4, 'Hans',      0);
   INSERT INTO CUSTOMER VALUES (5, 'Alex',    400);
   INSERT INTO CUSTOMER VALUES (6, 'User2',  100);

   COMMIT;
   
   GRANT SELECT,DELETE,INSERT,UPDATE ON CUSTOMER TO User2;
/* 06 */

/* 07 */
   UPDATE CUSTOMER
   SET    BALANCE = BALANCE - 100
   WHERE  CUSTOMER_ID = 1;
/* 07 */

/* 09 */
   UPDATE CUSTOMER
   SET    BALANCE = BALANCE + 100
   WHERE  CUSTOMER_ID = 6;
/* 09 */

/* 11 */
   ROLLBACK;
/* 11 */
```

## Abfragen unter User2

```sql
/*************************************************/
/**                 User2 USER !!!              */
/*************************************************/

/***************/
/** BUSY WAIT **/
/***************/

/* 03 */
   SET AUTOCOMMIT OFF;

   SELECT *
   FROM   User1.CUSTOMER
   WHERE  CUSTOMER_ID = 1;

   UPDATE User1.CUSTOMER
   SET    BALANCE = BALANCE + 100
   WHERE  CUSTOMER_ID = 1;
/* 03 */

/* 05 */
   COMMIT;
/* 05 */

/**************/
/** DEADLOCK **/
/**************/

/* 08 */
   SET AUTOCOMMIT OFF;

   UPDATE User1.CUSTOMER
   SET    BALANCE = BALANCE + 500
   WHERE  CUSTOMER_ID = 6;
/* 08 */

/* 10 */
   UPDATE User1.CUSTOMER
   SET    BALANCE = BALANCE - 500
   WHERE  CUSTOMER_ID = 1;
/* 10 */

/* 12 */
   COMMIT;
/* 12 */

```
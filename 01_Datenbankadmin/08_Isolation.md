# Isolation Levels

Unterlagen auf http://www.griesmayer.com/?menu=Oracle&semester=Semester_7&topic=07_Isolation

## Abfragen unter User1

```sql
/*************************************************/
/**                 USER1                        */
/*************************************************/

/***************/
/* READ COMMIT */
/***************/

/* 01*/
   DROP TABLE customer;

   CREATE TABLE customer
   (
      CUSTOMER_ID INTEGER PRIMARY KEY,
      FIRST_NAME  VARCHAR(30),
      BALANCE     DECIMAL(8,2)
   );

   INSERT INTO customer VALUES (1, 'Fritz',   800);
   INSERT INTO customer VALUES (2, 'Susi',   1000);
   INSERT INTO customer VALUES (3, 'Werner', -200);
   INSERT INTO customer VALUES (4, 'Hans',      0);
   INSERT INTO customer VALUES (5, 'Alex',    400);
   INSERT INTO customer VALUES (6, 'Thomas',  100);

   COMMIT;
   
   GRANT SELECT,DELETE,INSERT, UPDATE ON CUSTOMER TO User2;
/* 01*/

/* 02*/
   SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
/* 02*/

/* 04*/
   UPDATE customer
   SET    balance = 500
   WHERE  customer_id = 1;

   SELECT balance
   FROM   customer
   WHERE  customer_id = 1;

   INSERT INTO customer VALUES (7,'Max',300);

   SELECT *
   FROM   customer
   WHERE  customer_id = 7;

   DELETE FROM customer
   WHERE  customer_id = 4;

   SELECT *
   FROM   customer
   WHERE  customer_id = 4;
/* 04 */

/* 07 */
   COMMIT;
   SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
/* 07 */

/* 09 */
   SELECT *
   FROM   customer
   WHERE  customer_id in (1, 4, 7);
/* 09 */

/* 12 */
   SELECT *
   FROM   customer
   WHERE  customer_id in (1, 4, 7);
/* 12 */

/* 13 */
   ROLLBACK;
/* 13 */
   


/****************/
/* SERIALIZABLE */
/****************/

/* 20 */
   DELETE FROM customer;

   INSERT INTO customer VALUES (1, 'Fritz',   800);
   INSERT INTO customer VALUES (2, 'Susi',   1000);
   INSERT INTO customer VALUES (3, 'Werner', -200);
   INSERT INTO customer VALUES (4, 'Hans',      0);
   INSERT INTO customer VALUES (5, 'Alex',    400);
   INSERT INTO customer VALUES (6, 'Thomas',  100);

   COMMIT;
/* 20 */

/* 21 */
   SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
/* 21 */

/* 23 */
   UPDATE customer
   SET    balance = 500
   WHERE  customer_id = 1;

   SELECT balance
   FROM   customer
   WHERE  customer_id = 1;

   INSERT INTO customer VALUES (7,'Max',300);

   SELECT *
   FROM   customer
   WHERE  customer_id = 7;

   DELETE FROM customer
   WHERE  customer_id = 4;

   SELECT *
   FROM   customer
   WHERE  customer_id = 4;
/* 23 */

/* 25 */
   COMMIT;
   SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
/* 25 */

/* 29 */
   ROLLBACK;
/* 29 */



/*************/
/* READ ONLY */
/*************/

/* 30 */
   DELETE FROM customer;

   INSERT INTO customer VALUES (1, 'Fritz',   800);
   INSERT INTO customer VALUES (2, 'Susi',   1000);
   INSERT INTO customer VALUES (3, 'Werner', -200);
   INSERT INTO customer VALUES (4, 'Hans',      0);
   INSERT INTO customer VALUES (5, 'Alex',    400);
   INSERT INTO customer VALUES (6, 'Thomas',  100);

   COMMIT;
/* 30 */

/* 31 */
   SET TRANSACTION READ ONLY;
   
   SELECT *
   FROM   customer
   WHERE  customer_id = 1;
/* 31 */

/* 33 */
   SELECT *
   FROM   customer
   WHERE  customer_id = 1;

   UPDATE customer
   SET    balance = balance + 500
   WHERE  customer_id = 1;
/* 33 */

/* 34 */
   ROLLBACK;
/* 34 */


```

## Abfrage unter User 2

```sql
/*********************************************/
/**                 User 2                   */
/*********************************************/

/***************/
/* READ COMMIT */
/***************/

/* 03 */
   SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
/* 03 */

/* 05 */
   SELECT balance
   FROM   User1.customer
   WHERE  customer_id = 1;

   SELECT *
   FROM   User1.customer
   WHERE  customer_id = 7;

   SELECT *
   FROM   User1.customer
   WHERE  customer_id = 4;
/* 05 */

/* 06 */
   SELECT balance
   FROM   User1.customer
   WHERE  customer_id = 1;

   UPDATE User1.customer
   SET    balance = balance - 100
   WHERE  customer_id = 1;
   /*
   DIFFERENCE
   UPDATE User1.customer
   SET    balance = 700
   WHERE  customer_id = 1;
   */
/* 06 */

/* 08 */
   SELECT *
   FROM   User1.customer
   WHERE  customer_id in (1, 4, 7);
/* 08 */

/* 10 */
   COMMIT;
   SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
/* 10 */

/* 11 */
   SELECT *
   FROM   User1.customer
   WHERE  customer_id in (1, 4, 7);
/* 11 */

/* 13 */
   ROLLBACK;
/* 13 */



/****************/
/* SERIALIZABLE */
/****************/

/* 22 */
   SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
/* 22 */

/* 24 */
   SELECT balance
   FROM   User1.customer
   WHERE  customer_id = 1;

   SELECT *
   FROM   User1.customer
   WHERE  customer_id = 7;

   SELECT *
   FROM   User1.customer
   WHERE  customer_id = 4;
/* 24 */

/* 26 */
   SELECT balance
   FROM   User1.customer
   WHERE  customer_id = 1;

   SELECT *
   FROM   User1.customer
   WHERE  customer_id = 7;

   SELECT *
   FROM   User1.customer
   WHERE  customer_id = 4;
/* 26 */

/* 27 */
   SELECT balance
   FROM   User1.customer
   WHERE  customer_id = 1;

   UPDATE User1.customer
   SET    balance = balance - 100
   WHERE  customer_id = 1;

   /*ERROR!!!*/
/* 27 */

/* 28 */
   ROLLBACK;
/* 28 */

   

/*************/
/* READ ONLY */
/*************/

/* 32 */
   SELECT *
   FROM   User1.customer
   WHERE  customer_id = 1;

   UPDATE User1.customer
   SET    balance = balance - 100
   WHERE  customer_id = 1;

   SELECT *
   FROM   User1.customer
   WHERE  customer_id = 1;
   
   COMMIT;
/* 32 */
```

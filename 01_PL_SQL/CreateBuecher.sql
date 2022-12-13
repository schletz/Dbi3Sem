CREATE TABLE BUECHER(
   BuchID          NUMBER(3,0) NOT NULL PRIMARY KEY 
  ,Titel           VARCHAR2(50) NOT NULL
  ,Medium          VARCHAR2(20) NOT NULL
  ,ISBN            VARCHAR2(20)
  ,ASIN            VARCHAR2(10) NOT NULL
  ,Seiten          NUMBER(3,0)
  ,Preis           NUMBER(5,2)
  ,Veroeffentlicht DATE 
);
INSERT INTO BUECHER(BuchID,Titel,Medium,ISBN,ASIN,Seiten,Preis,Veroeffentlicht) VALUES (10,'QuickSteps in die C-Programmierung','Taschenbuch','978-1976736346','197673634X',154,9.95,'31.12.17');
INSERT INTO BUECHER(BuchID,Titel,Medium,ISBN,ASIN,Seiten,Preis,Veroeffentlicht) VALUES (11,'QuickSteps in die C-Programmierung','Kindle-Version',NULL,'B078T1SPBS',NULL,2.99,'04.01.18');
INSERT INTO BUECHER(BuchID,Titel,Medium,ISBN,ASIN,Seiten,Preis,Veroeffentlicht) VALUES (20,'IT-Projektmanagement - Essentials','Taschenbuch','978-1980447818','1980447810',150,9.95,'03.03.18');
INSERT INTO BUECHER(BuchID,Titel,Medium,ISBN,ASIN,Seiten,Preis,Veroeffentlicht) VALUES (21,'IT-Projektmanagement - Essentials','Kindle-Version',NULL,'B07BCFM63P',NULL,2.99,'10.03.18');
INSERT INTO BUECHER(BuchID,Titel,Medium,ISBN,ASIN,Seiten,Preis,Veroeffentlicht) VALUES (30,'Lazarus - Essentials','Taschenbuch','978-1983045080','198304508X',180,10.95,'02.06.18');
INSERT INTO BUECHER(BuchID,Titel,Medium,ISBN,ASIN,Seiten,Preis,Veroeffentlicht) VALUES (31,'Lazarus - Essentials','Kindle-Version',NULL,'B07DGVXG22',NULL,3.49,'02.06.18');
INSERT INTO BUECHER(BuchID,Titel,Medium,ISBN,ASIN,Seiten,Preis,Veroeffentlicht) VALUES (40,'Arduino - Essentials','Taschenbuch','978-1980955245','1980955247',116,8.95,'28.04.18');
INSERT INTO BUECHER(BuchID,Titel,Medium,ISBN,ASIN,Seiten,Preis,Veroeffentlicht) VALUES (41,'Arduino - Essentials','Kindle-Version',NULL,'B07CRYCT2C',NULL,4.49,'01.05.18');
INSERT INTO BUECHER(BuchID,Titel,Medium,ISBN,ASIN,Seiten,Preis,Veroeffentlicht) VALUES (50,'Linux - Essentials','Taschenbuch','978-1717972835','1717972837',142,9.95,'29.07.18');
INSERT INTO BUECHER(BuchID,Titel,Medium,ISBN,ASIN,Seiten,Preis,Veroeffentlicht) VALUES (51,'Linux - Essentials','Kindle-Version',NULL,'B07G5YCFC4',NULL,2.99,'04.08.18');
INSERT INTO BUECHER(BuchID,Titel,Medium,ISBN,ASIN,Seiten,Preis,Veroeffentlicht) VALUES (60,'Python 3 - Essentials','Taschenbuch','978-1724131997','1724131990',210,10.95,'29.09.18');
INSERT INTO BUECHER(BuchID,Titel,Medium,ISBN,ASIN,Seiten,Preis,Veroeffentlicht) VALUES (61,'Python 3 - Essentials','Kindle-Version',NULL,'B07HVWVHNX',NULL,2.99,'30.09.18');
INSERT INTO BUECHER(BuchID,Titel,Medium,ISBN,ASIN,Seiten,Preis,Veroeffentlicht) VALUES (70,'PL/SQL - Compact','Taschenbuch',NULL,'-',NULL,NULL,NULL);
INSERT INTO BUECHER(BuchID,Titel,Medium,ISBN,ASIN,Seiten,Preis,Veroeffentlicht) VALUES (71,'PL/SQL - Compact','Kindle-Version',NULL,'-',NULL,NULL,NULL);


COMMIT;

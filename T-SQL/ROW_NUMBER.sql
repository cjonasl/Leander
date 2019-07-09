CREATE TABLE AAA
(
  Id int identity(1,1) NOT NULL,
  A int NOT NULL,
  B int NOT NULL
)

INSERT INTO AAA(A, B)
VALUES(8, 2)

INSERT INTO AAA(A, B)
VALUES(6, 1)

INSERT INTO AAA(A, B)
VALUES(3, 2)

INSERT INTO AAA(A, B)
VALUES(1, 5)

INSERT INTO AAA(A, B)
VALUES(10, 4)

SELECT
  Id,
  A,
  B,
  ROW_NUMBER() OVER(ORDER BY Id) AS RowNrId,
  ROW_NUMBER() OVER(ORDER BY A) AS RowNrA,
  ROW_NUMBER() OVER(ORDER BY B) AS RowNrB,
  ROW_NUMBER() OVER(ORDER BY Id desc) AS RowNrIdDesc,
  ROW_NUMBER() OVER(ORDER BY A desc) AS RowNrADesc,
  ROW_NUMBER() OVER(ORDER BY B desc) AS RowNrBDesc
FROM
  AAA
ORDER BY Id
GO

DROP TABLE AAA

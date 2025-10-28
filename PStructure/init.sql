-- -----------------------------------------------------
-- SCHEMA: Orders (mit Sub-Positionen)
-- -----------------------------------------------------
USE Orders;

-- Drop alte Tabellen
DROP TABLE IF EXISTS OrderPosition;

CREATE TABLE OrderPosition (
                               OrderID INT NOT NULL,
                               OrderSubNumber VARCHAR(10) NOT NULL,
                               PositionNumber INT NOT NULL,
                               SubPositionNumber INT DEFAULT 0,
                               SubSubPositionNumber INT DEFAULT 0,
                               ProductName VARCHAR(255) NOT NULL,
                               Quantity INT NOT NULL,
                               UnitPrice DECIMAL(18,2) NOT NULL,
                               PRIMARY KEY(OrderID, OrderSubNumber, PositionNumber, SubPositionNumber, SubSubPositionNumber),
                               FOREIGN KEY (OrderID) REFERENCES OrderHeader(OrderID)
);

-- Pro Order zufällig 3-5 Positionen, jede Position hat 0-2 Sub-Positionen, Sub-Positionen haben 0-2 SubSub-Positionen
DELIMITER $$
CREATE PROCEDURE populate_order_positions_hierarchical()
BEGIN
    DECLARE order_counter INT DEFAULT 1;
    DECLARE num_positions INT;
    DECLARE num_subpositions INT;
    DECLARE num_subsubpositions INT;
    DECLARE pos INT;
    DECLARE subpos INT;
    DECLARE subsubpos INT;

    WHILE order_counter <= 50 DO
        SET pos = 1;
        SET num_positions = 3 + FLOOR(RAND()*3); -- 3-5 Positionen pro Auftrag

        WHILE pos <= num_positions DO
            -- Hauptposition
            INSERT INTO OrderPosition(OrderID, OrderSubNumber, PositionNumber, SubPositionNumber, SubSubPositionNumber, ProductName, Quantity, UnitPrice)
            VALUES(LAST_INSERT_ID(), 'A', pos, 0, 0,
                   (SELECT ProductName FROM Products.Product ORDER BY RAND() LIMIT 1),
                   FLOOR(1 + RAND()*10),
                   (SELECT StandardPrice FROM Products.Product ORDER BY RAND() LIMIT 1));

            -- Sub-Positionen
            SET subpos = 1;
            SET num_subpositions = FLOOR(RAND()*3); -- 0-2 Subpositionen
            WHILE subpos <= num_subpositions DO
                INSERT INTO OrderPosition(OrderID, OrderSubNumber, PositionNumber, SubPositionNumber, SubSubPositionNumber, ProductName, Quantity, UnitPrice)
                VALUES(LAST_INSERT_ID(), 'A', pos, subpos, 0,
                       (SELECT ProductName FROM Products.Product ORDER BY RAND() LIMIT 1),
                       FLOOR(1 + RAND()*5),
                       (SELECT StandardPrice FROM Products.Product ORDER BY RAND() LIMIT 1));

                -- Sub-Sub-Positionen
                SET subsubpos = 1;
                SET num_subsubpositions = FLOOR(RAND()*3); -- 0-2 SubSub-Positionen
                WHILE subsubpos <= num_subsubpositions DO
                    INSERT INTO OrderPosition(OrderID, OrderSubNumber, PositionNumber, SubPositionNumber, SubSubPositionNumber, ProductName, Quantity, UnitPrice)
                    VALUES(LAST_INSERT_ID(), 'A', pos, subpos, subsubpos,
                           (SELECT ProductName FROM Products.Product ORDER BY RAND() LIMIT 1),
                           FLOOR(1 + RAND()*3),
                           (SELECT StandardPrice FROM Products.Product ORDER BY RAND() LIMIT 1));
                    SET subsubpos = subsubpos + 1;
END WHILE;

                SET subpos = subpos + 1;
END WHILE;

            SET pos = pos + 1;
END WHILE;

        SET order_counter = order_counter + 1;
END WHILE;
END$$
DELIMITER ;

CALL populate_order_positions_hierarchical();
DROP PROCEDURE populate_order_positions_hierarchical;

SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF EXISTS (SELECT 1 FROM Review WHERE TRY_CONVERT(INT, Rating) IS NULL OR TRY_CONVERT(INT, Rating) NOT BETWEEN 1 AND 5)
    THROW 50001, 'Correct invalid ratings before changing the schema.', 1;
IF EXISTS (SELECT 1 FROM Booking WHERE Status NOT IN ('Pending', 'Confirmed', 'Cancelled'))
    THROW 50002, 'Correct invalid booking statuses first.', 1;
ALTER TABLE Review ALTER COLUMN Rating INT NOT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Review_Rating')
    ALTER TABLE Review WITH CHECK ADD CONSTRAINT CK_Review_Rating CHECK (Rating BETWEEN 1 AND 5);
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Booking_Status')
    ALTER TABLE Booking WITH CHECK ADD CONSTRAINT CK_Booking_Status CHECK (Status IN ('Pending', 'Confirmed', 'Cancelled'));
COMMIT;

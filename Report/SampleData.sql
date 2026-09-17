-- Run once in a test database after DatabaseSchema.sql.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
INSERT INTO Member (Name,Email,Phone,Address,Password,RegDate)
VALUES ('Sample Member','coursework.sample@example.test','0770000000',
        'Negombo','Sample123!',CAST(GETDATE() AS DATE));
DECLARE @MemberID INT = SCOPE_IDENTITY();
INSERT INTO FacilityType (TypeName) VALUES ('Sample Tennis');
DECLARE @TypeID INT = SCOPE_IDENTITY();
INSERT INTO Facility (Name,TypeID,Location,Capacity,HourlyRate)
VALUES ('Sample Tennis Court',@TypeID,'Negombo',4,1500.00);
DECLARE @FacilityID INT = SCOPE_IDENTITY();
INSERT INTO SportPreference (MemberID,TypeID) VALUES (@MemberID,@TypeID);
INSERT INTO Booking (MemberID,FacilityID,BookingDate,StartTime,EndTime,Status)
VALUES (@MemberID,@FacilityID,'2026-08-01','2026-08-01T09:00:00',
        '2026-08-01T10:00:00','Confirmed');
INSERT INTO Review (MemberID,FacilityID,Rating,Comments,ReviewDate)
VALUES (@MemberID,@FacilityID,4,'The court was clean.','2026-08-01T11:00:00');
INSERT INTO Inquiry (GuestName,Email,Message,DateSent)
VALUES ('Sample Guest','guest@example.test','Are weekend sessions available?',
        CAST(GETDATE() AS DATE));
COMMIT;
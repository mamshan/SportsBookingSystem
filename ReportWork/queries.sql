-- Query 1: Bookings with member and facility names
SELECT TOP (10) b.BookingID,m.Name AS Member,f.Name AS Facility,b.BookingDate,b.Status FROM Booking b JOIN Member m ON b.MemberID=m.MemberID JOIN Facility f ON b.FacilityID=f.FacilityID ORDER BY b.BookingID;
GO

-- Query 2: Facilities available for a selected time
SELECT f.FacilityID,f.Name,f.Location FROM Facility f WHERE NOT EXISTS (SELECT 1 FROM Booking b WHERE b.FacilityID=f.FacilityID AND b.BookingDate='2026-10-17' AND b.Status<>'Cancelled' AND (b.StartTime IS NULL OR b.EndTime IS NULL OR (b.StartTime<'2026-10-17T11:00:00' AND b.EndTime>'2026-10-17T10:00:00'))) ORDER BY f.FacilityID;
GO

-- Query 3: Average rating for each reviewed facility
SELECT f.Name,COUNT(*) AS Reviews,CAST(AVG(CAST(r.Rating AS DECIMAL(5,2))) AS DECIMAL(5,2)) AS AverageRating FROM Review r JOIN Facility f ON r.FacilityID=f.FacilityID GROUP BY f.FacilityID,f.Name ORDER BY AverageRating DESC;
GO

-- Query 4: Five least expensive facilities
SELECT TOP (5) Name,Location,HourlyRate FROM Facility WHERE HourlyRate IS NOT NULL ORDER BY HourlyRate,FacilityID;
GO

-- Query 5: Members who have made a booking
SELECT MemberID,Name FROM Member WHERE MemberID IN (SELECT MemberID FROM Booking) ORDER BY MemberID;
GO

-- Query 6: Facilities with no booking records
SELECT f.FacilityID,f.Name FROM Facility f LEFT JOIN Booking b ON f.FacilityID=b.FacilityID WHERE b.BookingID IS NULL ORDER BY f.FacilityID;
GO

-- Query 7: Preferred sports for each member
SELECT m.Name AS Member,t.TypeName AS PreferredSport FROM SportPreference s JOIN Member m ON m.MemberID=s.MemberID JOIN FacilityType t ON t.TypeID=s.TypeID ORDER BY m.MemberID,t.TypeName;
GO

-- Query 8: Latest guest inquiries
SELECT TOP (5) InquiryID,GuestName,Message,DateSent FROM Inquiry ORDER BY DateSent DESC,InquiryID DESC;
GO

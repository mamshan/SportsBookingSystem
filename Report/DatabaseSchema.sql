-- Recreates the verified schema in an empty database.

CREATE TABLE Member (
    MemberID INT IDENTITY(1,1) NOT NULL,
    Name VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    Phone VARCHAR(255) NULL,
    Address VARCHAR(255) NULL,
    Password VARCHAR(255) NOT NULL,
    RegDate DATE NULL,
    CONSTRAINT Member__UN UNIQUE (Email),
    CONSTRAINT Member_PK PRIMARY KEY (MemberID)
);
GO

CREATE TABLE FacilityType (
    TypeID INT IDENTITY(1,1) NOT NULL,
    TypeName VARCHAR(255) NOT NULL,
    CONSTRAINT FacilityType_PK PRIMARY KEY (TypeID)
);
GO

CREATE TABLE Facility (
    FacilityID INT IDENTITY(1,1) NOT NULL,
    Name VARCHAR(255) NOT NULL,
    TypeID INT NOT NULL,
    Location VARCHAR(255) NULL,
    Capacity INT NULL,
    HourlyRate DECIMAL(25,2) NULL,
    CONSTRAINT Facility_PK PRIMARY KEY (FacilityID),
    CONSTRAINT Facility_FacilityType_FK FOREIGN KEY (TypeID) REFERENCES FacilityType(TypeID)
);
GO

CREATE TABLE Booking (
    BookingID INT IDENTITY(1,1) NOT NULL,
    MemberID INT NOT NULL,
    FacilityID INT NOT NULL,
    BookingDate DATE NOT NULL,
    StartTime DATETIME NULL,
    EndTime DATETIME NULL,
    Status VARCHAR(255) NOT NULL,
    CONSTRAINT Booking_PK PRIMARY KEY (BookingID),
    CONSTRAINT Booking_Facility_FK FOREIGN KEY (FacilityID) REFERENCES Facility(FacilityID),
    CONSTRAINT Booking_Member_FK FOREIGN KEY (MemberID) REFERENCES Member(MemberID),
    CONSTRAINT CK_Booking_Status CHECK ([Status]='Cancelled' OR [Status]='Confirmed' OR [Status]='Pending')
);
GO

CREATE TABLE Review (
    ReviewID INT IDENTITY(1,1) NOT NULL,
    MemberID INT NOT NULL,
    FacilityID INT NOT NULL,
    Rating INT NOT NULL,
    Comments VARCHAR(255) NULL,
    ReviewDate DATETIME NULL,
    CONSTRAINT Review_PK PRIMARY KEY (ReviewID),
    CONSTRAINT Review_Facility_FK FOREIGN KEY (FacilityID) REFERENCES Facility(FacilityID),
    CONSTRAINT Review_Member_FK FOREIGN KEY (MemberID) REFERENCES Member(MemberID),
    CONSTRAINT CK_Review_Rating CHECK ([Rating]>=(1) AND [Rating]<=(5))
);
GO

CREATE TABLE SportPreference (
    SportPreferenceID INT IDENTITY(1,1) NOT NULL,
    MemberID INT NOT NULL,
    TypeID INT NOT NULL,
    CONSTRAINT SportPreference__UN UNIQUE (MemberID, TypeID),
    CONSTRAINT SportPreference_PK PRIMARY KEY (SportPreferenceID),
    CONSTRAINT SportPreference_FacilityType_FK FOREIGN KEY (TypeID) REFERENCES FacilityType(TypeID),
    CONSTRAINT SportPreference_Member_FK FOREIGN KEY (MemberID) REFERENCES Member(MemberID)
);
GO

CREATE TABLE Inquiry (
    InquiryID INT IDENTITY(1,1) NOT NULL,
    GuestName VARCHAR(255) NULL,
    Email VARCHAR(255) NULL,
    Message VARCHAR(255) NULL,
    DateSent DATE NULL,
    CONSTRAINT Inquiry_PK PRIMARY KEY (InquiryID)
);
GO
CREATE DATABASE SportHub;

USE SportHub;


USE SportHub;
GO

/* =========================================================
   SPORT HUB - SQL SERVER DATABASE
   ASP.NET MVC PROJECT
   ========================================================= */


/* =========================================================
   1. REMOVE OLD TABLES
   This allows us to run the script again if necessary.
   ========================================================= */

IF OBJECT_ID('dbo.MEMBER_SPORT_PREFERENCE', 'U') IS NOT NULL
    DROP TABLE dbo.MEMBER_SPORT_PREFERENCE;

IF OBJECT_ID('dbo.REVIEW', 'U') IS NOT NULL
    DROP TABLE dbo.REVIEW;

IF OBJECT_ID('dbo.RENTAL', 'U') IS NOT NULL
    DROP TABLE dbo.RENTAL;

IF OBJECT_ID('dbo.BOOKING', 'U') IS NOT NULL
    DROP TABLE dbo.BOOKING;

IF OBJECT_ID('dbo.INQUIRY', 'U') IS NOT NULL
    DROP TABLE dbo.INQUIRY;

IF OBJECT_ID('dbo.EQUIPMENT', 'U') IS NOT NULL
    DROP TABLE dbo.EQUIPMENT;

IF OBJECT_ID('dbo.FACILITY', 'U') IS NOT NULL
    DROP TABLE dbo.FACILITY;

IF OBJECT_ID('dbo.SPORT', 'U') IS NOT NULL
    DROP TABLE dbo.SPORT;

IF OBJECT_ID('dbo.MEMBER', 'U') IS NOT NULL
    DROP TABLE dbo.MEMBER;

GO


/* =========================================================
   2. MEMBER TABLE
   ========================================================= */

CREATE TABLE MEMBER
(
    MemberID       INT IDENTITY(1,1) NOT NULL,
    Name           NVARCHAR(200) NOT NULL,
    Email          NVARCHAR(100) NOT NULL,
    Phone          NVARCHAR(15) NULL,
    Address        NVARCHAR(200) NULL,
    RegisteredDate DATETIME2 NOT NULL
                   CONSTRAINT DF_MEMBER_RegisteredDate
                   DEFAULT GETDATE(),
    Password       NVARCHAR(255) NOT NULL,

    CONSTRAINT MEMBER_PK
        PRIMARY KEY (MemberID),

    CONSTRAINT MEMBER_EMAIL_UK
        UNIQUE (Email)
);

GO


/* =========================================================
   3. FACILITY TABLE
   ========================================================= */

CREATE TABLE FACILITY
(
    FacilityID INT IDENTITY(1,1) NOT NULL,
    Name       NVARCHAR(200) NOT NULL,
    Type       NVARCHAR(50) NOT NULL,
    Location   NVARCHAR(100) NULL,
    HourlyRate DECIMAL(6,2) NULL,
    Status     NVARCHAR(20) NOT NULL
               CONSTRAINT DF_FACILITY_Status
               DEFAULT 'Available',

    CONSTRAINT FACILITY_PK
        PRIMARY KEY (FacilityID),

    CONSTRAINT FACILITY_STATUS_CHK
        CHECK (Status IN
        ('Available', 'Maintenance', 'Closed'))
);

GO


/* =========================================================
   4. SPORT TABLE
   ========================================================= */

CREATE TABLE SPORT
(
    SportID   INT IDENTITY(1,1) NOT NULL,
    SportName NVARCHAR(50) NOT NULL,

    CONSTRAINT SPORT_PK
        PRIMARY KEY (SportID),

    CONSTRAINT SPORT_NAME_UK
        UNIQUE (SportName)
);

GO


/* =========================================================
   5. EQUIPMENT TABLE
   ========================================================= */

CREATE TABLE EQUIPMENT
(
    EquipmentID       INT IDENTITY(1,1) NOT NULL,
    Name              NVARCHAR(100) NOT NULL,
    Type              NVARCHAR(50) NULL,
    QuantityAvailable INT NOT NULL
                      CONSTRAINT DF_EQUIPMENT_Quantity
                      DEFAULT 0,

    CONSTRAINT EQUIPMENT_PK
        PRIMARY KEY (EquipmentID),

    CONSTRAINT EQUIPMENT_QTY_CHK
        CHECK (QuantityAvailable >= 0)
);

GO


/* =========================================================
   6. BOOKING TABLE
   ========================================================= */

CREATE TABLE BOOKING
(
    BookingID   INT IDENTITY(1,1) NOT NULL,
    MemberID    INT NOT NULL,
    FacilityID  INT NOT NULL,
    BookingDate DATE NOT NULL,
    TimeSlot    NVARCHAR(30) NOT NULL,
    Status      NVARCHAR(20) NOT NULL
                CONSTRAINT DF_BOOKING_Status
                DEFAULT 'Confirmed',
    CreatedAt   DATETIME2 NOT NULL
                CONSTRAINT DF_BOOKING_CreatedAt
                DEFAULT GETDATE(),

    CONSTRAINT BOOKING_PK
        PRIMARY KEY (BookingID),

    CONSTRAINT BOOKING_MEMBER_FK
        FOREIGN KEY (MemberID)
        REFERENCES MEMBER(MemberID),

    CONSTRAINT BOOKING_FACILITY_FK
        FOREIGN KEY (FacilityID)
        REFERENCES FACILITY(FacilityID),

    CONSTRAINT BOOKING_STATUS_CHK
        CHECK (Status IN
        ('Confirmed', 'Cancelled', 'Completed'))
);

GO


/* =========================================================
   7. REVIEW TABLE
   ========================================================= */

CREATE TABLE REVIEW
(
    ReviewID      INT IDENTITY(1,1) NOT NULL,
    BookingID     INT NOT NULL,
    Rating        INT NOT NULL,
    ReviewComment NVARCHAR(500) NULL,
    ReviewDate    DATETIME2 NOT NULL
                  CONSTRAINT DF_REVIEW_Date
                  DEFAULT GETDATE(),

    CONSTRAINT REVIEW_PK
        PRIMARY KEY (ReviewID),

    CONSTRAINT REVIEW_BOOKING_FK
        FOREIGN KEY (BookingID)
        REFERENCES BOOKING(BookingID),

    CONSTRAINT REVIEW_RATING_CHK
        CHECK (Rating BETWEEN 1 AND 5)
);

GO


/* =========================================================
   8. RENTAL TABLE
   ========================================================= */

CREATE TABLE RENTAL
(
    RentalID    INT IDENTITY(1,1) NOT NULL,
    MemberID    INT NOT NULL,
    EquipmentID INT NOT NULL,
    RentDate    DATE NOT NULL
                CONSTRAINT DF_RENTAL_RentDate
                DEFAULT CAST(GETDATE() AS DATE),
    ReturnDate  DATE NULL,
    Status      NVARCHAR(20) NOT NULL
                CONSTRAINT DF_RENTAL_Status
                DEFAULT 'Active',

    CONSTRAINT RENTAL_PK
        PRIMARY KEY (RentalID),

    CONSTRAINT RENTAL_MEMBER_FK
        FOREIGN KEY (MemberID)
        REFERENCES MEMBER(MemberID),

    CONSTRAINT RENTAL_EQUIPMENT_FK
        FOREIGN KEY (EquipmentID)
        REFERENCES EQUIPMENT(EquipmentID),

    CONSTRAINT RENTAL_STATUS_CHK
        CHECK (Status IN
        ('Active', 'Returned', 'Overdue'))
);

GO


/* =========================================================
   9. MEMBER_SPORT_PREFERENCE TABLE
   Junction table for MEMBER <-> SPORT
   ========================================================= */

CREATE TABLE MEMBER_SPORT_PREFERENCE
(
    MemberID INT NOT NULL,
    SportID  INT NOT NULL,

    CONSTRAINT MSP_PK
        PRIMARY KEY (MemberID, SportID),

    CONSTRAINT MSP_MEMBER_FK
        FOREIGN KEY (MemberID)
        REFERENCES MEMBER(MemberID),

    CONSTRAINT MSP_SPORT_FK
        FOREIGN KEY (SportID)
        REFERENCES SPORT(SportID)
);

GO


/* =========================================================
   10. INQUIRY TABLE
   ========================================================= */

CREATE TABLE INQUIRY
(
    InquiryID  INT IDENTITY(1,1) NOT NULL,
    GuestName  NVARCHAR(200) NOT NULL,
    GuestEmail NVARCHAR(100) NOT NULL,
    Message    NVARCHAR(500) NOT NULL,
    SentDate   DATETIME2 NOT NULL
               CONSTRAINT DF_INQUIRY_SentDate
               DEFAULT GETDATE(),

    CONSTRAINT INQUIRY_PK
        PRIMARY KEY (InquiryID)
);

GO


/* =========================================================
   11. INSERT SPORTS
   ========================================================= */

INSERT INTO SPORT
    (SportName)
VALUES
    ('Tennis'),
    ('Soccer'),
    ('Basketball'),
    ('Volleyball');

GO


/* =========================================================
   12. INSERT MEMBERS
   ========================================================= */

INSERT INTO MEMBER
    (Name, Email, Phone, Address, Password)
VALUES
    ('Kasun Perera',
     'kasun.perera@mail.com',
     '0771234567',
     '12 Galle Road, Colombo',
     'Password123!'),

    ('Nimali Silva',
     'nimali.silva@mail.com',
     '0772345678',
     '45 Kandy Road, Kandy',
     'Password123!'),

    ('Ashan Fernando',
     'ashan.f@mail.com',
     '0773456789',
     '8 Lake Drive, Nugegoda',
     'Password123!'),

    ('Dilani Jayasuriya',
     'dilani.j@mail.com',
     '0774567890',
     '22 Park Lane, Maharagama',
     'Password123!'),

    ('Ruwan Bandara',
     'ruwan.b@mail.com',
     '0775678901',
     '5 Hill Street, Kotte',
     'Password123!');

GO


/* =========================================================
   13. INSERT FACILITIES
   ========================================================= */

INSERT INTO FACILITY
    (Name, Type, Location, HourlyRate, Status)
VALUES
    ('Court A',
     'Tennis',
     'Maharagama Sports Complex',
     1500.00,
     'Available'),

    ('Court B',
     'Tennis',
     'Maharagama Sports Complex',
     1500.00,
     'Available'),

    ('Field 1',
     'Soccer',
     'Nugegoda Community Ground',
     3000.00,
     'Available'),

    ('Court 1',
     'Basketball',
     'Kotte Youth Centre',
     2000.00,
     'Maintenance'),

    ('Court 2',
     'Basketball',
     'Kotte Youth Centre',
     2000.00,
     'Available'),

    ('Court V1',
     'Volleyball',
     'Colombo Beach Park',
     1200.00,
     'Available');

GO


/* =========================================================
   14. INSERT EQUIPMENT
   ========================================================= */

INSERT INTO EQUIPMENT
    (Name, Type, QuantityAvailable)
VALUES
    ('Tennis Racket',
     'Racket',
     10),

    ('Soccer Ball',
     'Ball',
     15),

    ('Basketball',
     'Ball',
     12),

    ('Volleyball Net',
     'Net',
     4);

GO


/* =========================================================
   15. MEMBER SPORT PREFERENCES
   ========================================================= */

INSERT INTO MEMBER_SPORT_PREFERENCE
    (MemberID, SportID)
VALUES
    (1, 1), -- Kasun - Tennis
    (1, 2), -- Kasun - Soccer

    (2, 3), -- Nimali - Basketball

    (3, 1), -- Ashan - Tennis

    (4, 4), -- Dilani - Volleyball
    (4, 2), -- Dilani - Soccer

    (5, 3), -- Ruwan - Basketball
    (5, 1); -- Ruwan - Tennis

GO


/* =========================================================
   16. INSERT BOOKINGS
   ========================================================= */

INSERT INTO BOOKING
    (MemberID, FacilityID, BookingDate, TimeSlot, Status)
VALUES
    (1, 1, '2026-09-05', '14:00-15:00', 'Confirmed'),

    (2, 5, '2026-09-06', '10:00-11:00', 'Confirmed'),

    (3, 1, '2026-09-07', '16:00-17:00', 'Completed'),

    (1, 3, '2026-09-08', '09:00-10:00', 'Cancelled'),

    (4, 6, '2026-09-08', '17:00-18:00', 'Confirmed'),

    (5, 5, '2026-09-09', '18:00-19:00', 'Completed'),

    (2, 3, '2026-09-10', '11:00-12:00', 'Confirmed');

GO


/* =========================================================
   17. INSERT REVIEWS
   ========================================================= */

INSERT INTO REVIEW
    (BookingID, Rating, ReviewComment)
VALUES
    (3,
     5,
     'Court was in excellent condition, great experience.'),

    (3,
     4,
     'Lighting could be a bit brighter in the evening.'),

    (6,
     5,
     'Basketball court was well maintained, staff were friendly.');

GO


/* =========================================================
   18. INSERT RENTALS
   ========================================================= */

INSERT INTO RENTAL
    (MemberID, EquipmentID, RentDate, ReturnDate, Status)
VALUES
    (1,
     1,
     '2026-09-05',
     '2026-09-05',
     'Returned'),

    (2,
     2,
     '2026-09-06',
     NULL,
     'Active'),

    (4,
     4,
     '2026-09-08',
     NULL,
     'Active'),

    (5,
     3,
     '2026-09-09',
     '2026-09-09',
     'Returned');

GO


/* =========================================================
   19. INSERT GUEST INQUIRIES
   ========================================================= */

INSERT INTO INQUIRY
    (GuestName, GuestEmail, Message)
VALUES
    ('Saman Kumara',
     'saman.k@mail.com',
     'Do you have badminton courts available at any location?'),

    ('Chamari Rathnayake',
     'chamari.r@mail.com',
     'What are your opening hours on weekends?'),

    ('Isuru Madushanka',
     'isuru.m@mail.com',
     'Can guests rent equipment without becoming a member?');

GO


/* =========================================================
   20. VERIFY TABLES
   ========================================================= */

SELECT * FROM MEMBER;

SELECT * FROM SPORT;

SELECT * FROM MEMBER_SPORT_PREFERENCE;

SELECT * FROM FACILITY;

SELECT * FROM BOOKING;

SELECT * FROM REVIEW;

SELECT * FROM EQUIPMENT;

SELECT * FROM RENTAL;

SELECT * FROM INQUIRY;

GO


/* =========================================================
   21. CHECK RECORD COUNTS
   ========================================================= */

SELECT 'MEMBER' AS TableName,
       COUNT(*) AS RecordCount
FROM MEMBER

UNION ALL

SELECT 'SPORT',
       COUNT(*)
FROM SPORT

UNION ALL

SELECT 'MEMBER_SPORT_PREFERENCE',
       COUNT(*)
FROM MEMBER_SPORT_PREFERENCE

UNION ALL

SELECT 'FACILITY',
       COUNT(*)
FROM FACILITY

UNION ALL

SELECT 'BOOKING',
       COUNT(*)
FROM BOOKING

UNION ALL

SELECT 'REVIEW',
       COUNT(*)
FROM REVIEW

UNION ALL

SELECT 'EQUIPMENT',
       COUNT(*)
FROM EQUIPMENT

UNION ALL

SELECT 'RENTAL',
       COUNT(*)
FROM RENTAL

UNION ALL

SELECT 'INQUIRY',
       COUNT(*)
FROM INQUIRY;

GO



SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE';




SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME IN
(
    'MEMBER',
    'SPORT',
    'MEMBER_SPORT_PREFERENCE'
)
ORDER BY TABLE_NAME, ORDINAL_POSITION;




SELECT DB_NAME() AS CurrentDatabase;


SELECT *
FROM MEMBER
ORDER BY MemberID DESC;


GO

ALTER TABLE EQUIPMENT
ADD RentalFee DECIMAL(10,2) NOT NULL
    CONSTRAINT DF_EQUIPMENT_RentalFee DEFAULT 0;
GO

UPDATE EQUIPMENT
SET RentalFee =
    CASE
        WHEN Name = 'Tennis Racket' THEN 500
        WHEN Name = 'Soccer Ball' THEN 300
        WHEN Name = 'Basketball' THEN 400
        WHEN Name = 'Volleyball Net' THEN 750
        ELSE 0
    END;
GO

SELECT EquipmentID, Name, Type, QuantityAvailable, RentalFee
FROM EQUIPMENT
ORDER BY EquipmentID;
GO

USE SportHub;
GO

ALTER TABLE EQUIPMENT
ADD RentalFee DECIMAL(10,2) NOT NULL
    CONSTRAINT DF_EQUIPMENT_RentalFee DEFAULT 0;
GO

USE SportHub;
GO

SELECT EquipmentID,
       Name,
       Type,
       QuantityAvailable,
       RentalFee
FROM EQUIPMENT
ORDER BY EquipmentID;



SELECT COLUMN_NAME, DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'SPORT';

USE SportHub;
GO

SELECT COLUMN_NAME, DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'SPORT';
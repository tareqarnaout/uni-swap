CREATE TABLE Products
(
    ProductId int IDENTITY(1,1) PRIMARY KEY,
    Title nvarchar(100) NOT NULL,
    Description nvarchar(MAX) NULL,       -- Holds long descriptions
    Price decimal(18, 2) NOT NULL,        -- decimal is best for money (prevents rounding errors)
    
    -- File Upload Columns
    ImageUrl nvarchar(500) NULL,          -- Path to the .jpg/.png file
    ConditionReportUrl nvarchar(500) NULL,-- Path to the .pdf file (Proof of purchase)
    
    -- Relationships
    SellerId int NOT NULL,                -- Links to the Users table
    CreatedAt datetime DEFAULT GETDATE(), -- Automatically sets the upload time
    
    -- Foreign Key Constraint (Safety)
    -- This ensures a Product cannot exist without a valid User (Seller)
    CONSTRAINT FK_Products_Users FOREIGN KEY (SellerId) REFERENCES Student(UserId)
);
GO
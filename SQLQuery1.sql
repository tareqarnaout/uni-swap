CREATE TABLE Student
( 
    UserID int IDENTITY(1,1) PRIMARY KEY,   
    FirstName nvarchar(50) NOT NULL,        
    LastName nvarchar(50) NOT NULL,
    Email nvarchar(255) NOT NULL UNIQUE,     
    PasswordHash varchar(255) NOT NULL,
)
GO
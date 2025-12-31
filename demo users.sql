
-- 1. Add the Role column to your existing table
-- We set a DEFAULT value so existing users automatically become 'Student'
ALTER TABLE Student
ADD Role varchar(20) NOT NULL DEFAULT 'Student';
GO

-- 2. (Optional) If you haven't inserted data yet, clear the table to start fresh
-- TRUNCATE TABLE Student; 

-- 3. Insert Demo Users (including an Admin)
INSERT INTO Student (FirstName, LastName, Email, PasswordHash, Role)
VALUES 
('Admin', 'User', 'admin@uniswap.edu', 'admin123', 'Admin'),
('John', 'Doe', 'john@university.edu', 'secret123', 'Student'),
('Sarah', 'Smith', 'sarah@university.edu', 'secret456', 'Student');
GO

INSERT INTO Student (FirstName, LastName, Email, PasswordHash, Role)
VALUES ('Mike', 'Johnson', 'mike@university.edu', 'secret789', 'Student');
GO
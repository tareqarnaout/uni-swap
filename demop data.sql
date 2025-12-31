-- 1. Get the IDs of your students so we can link products to them
-- (We use the Emails to find the correct Student ID)
DECLARE @JohnId int = (SELECT TOP 1 UserID FROM Student WHERE Email = 'john@university.edu');
DECLARE @SarahId int = (SELECT TOP 1 UserID FROM Student WHERE Email = 'sarah@university.edu');
DECLARE @MikeId int = (SELECT TOP 1 UserID FROM Student WHERE Email = 'mike@university.edu');

-- Safety Check: If these users don't exist, we can't add products for them.
IF @JohnId IS NOT NULL AND @SarahId IS NOT NULL
BEGIN
    -- 2. Insert the Products
    INSERT INTO Products (Title, Description, Price, ImageUrl, ConditionReportUrl, SellerId)
    VALUES
    -- Item 1: Calculus Book (Sold by John)
    ('Calculus 2nd Edition', 'Used for one semester. No highlighting inside. Great condition.', 45.00, 
     'https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?auto=format&fit=crop&w=500&q=60', 
     NULL, @JohnId),

    -- Item 2: Coffee Maker (Sold by Sarah)
    ('Mini Coffee Maker', 'Single serve coffee maker. Perfect for dorm rooms. Includes reusable filter.', 20.00, 
     'https://images.unsplash.com/photo-1517080315817-48651c618b13?auto=format&fit=crop&w=500&q=60', 
     NULL, @SarahId),

    -- Item 3: Desk Lamp (Sold by Mike)
    ('Modern Desk Lamp', 'Adjustable neck, pink metal finish. Bulb included.', 15.00, 
     'https://images.unsplash.com/photo-1533241249764-70db6d735165?auto=format&fit=crop&w=500&q=60', 
     NULL, @MikeId),

    -- Item 4: Laptop (Sold by Sarah - Expensive item with PDF proof)
    ('MacBook Pro 2019', '13-inch, 256GB SSD. Minor scratches on the bottom case. Battery health is good.', 650.00, 
     'https://images.unsplash.com/photo-1517336714731-489689fd1ca4?auto=format&fit=crop&w=500&q=60', 
     '/files/condition_reports/macbook_report.pdf', @SarahId),

    -- Item 5: Backpack (Sold by John)
    ('Black Backpack', 'Waterproof laptop backpack. Fits 15-inch laptops. Like new.', 30.00, 
     'https://images.unsplash.com/photo-1553062407-98eeb64c6a62?auto=format&fit=crop&w=500&q=60', 
     NULL, @JohnId),

    -- Item 6: Notes (Sold by Mike)
    ('Engineering Notes Bundle', 'Handwritten notes for Intro to Engineering. Very detailed diagrams.', 0.00, 
     'https://images.unsplash.com/photo-1519389950473-47ba0277781c?auto=format&fit=crop&w=500&q=60', 
     NULL, @MikeId);

    PRINT 'Products successfully added!';
END
ELSE
BEGIN
    PRINT 'Error: Could not find Students. Please run the Student data script first.';
END
GO
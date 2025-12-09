USE OrderServiceDb;

-- Orders with various statuses: Pending, Processing, Shipped, Completed, Cancelled
INSERT INTO Orders (CustomerName, CustomerEmail, ProductId, ProductName, ProductPrice, Quantity, TotalAmount, Status, CreatedAt, UpdatedAt) VALUES
    -- Completed orders (older)
    ('Nguyễn Văn An', 'an.nguyen@email.com', 1, 'iPhone 15 Pro Max', 1399.99, 1, 1399.99, 'Completed', DATE_SUB(NOW(), INTERVAL 15 DAY), DATE_SUB(NOW(), INTERVAL 10 DAY)),
    ('Trần Thị Bình', 'binh.tran@email.com', 4, 'Galaxy S24 Ultra', 1299.99, 1, 1299.99, 'Completed', DATE_SUB(NOW(), INTERVAL 14 DAY), DATE_SUB(NOW(), INTERVAL 9 DAY)),
    ('Phạm Thu Dung', 'dung.pham@email.com', 6, 'Pixel 8 Pro', 999.99, 1, 999.99, 'Completed', DATE_SUB(NOW(), INTERVAL 12 DAY), DATE_SUB(NOW(), INTERVAL 7 DAY)),
    ('Võ Thị Phương', 'phuong.vo@email.com', 5, 'Galaxy S24', 799.99, 2, 1599.98, 'Completed', DATE_SUB(NOW(), INTERVAL 10 DAY), DATE_SUB(NOW(), INTERVAL 5 DAY)),
    ('Đặng Minh Giang', 'giang.dang@email.com', 7, 'Pixel 8', 699.99, 1, 699.99, 'Completed', DATE_SUB(NOW(), INTERVAL 8 DAY), DATE_SUB(NOW(), INTERVAL 4 DAY)),
    ('Bùi Văn Hải', 'hai.bui@email.com', 9, 'Xiaomi 14 Pro', 649.99, 2, 1299.98, 'Completed', DATE_SUB(NOW(), INTERVAL 7 DAY), DATE_SUB(NOW(), INTERVAL 3 DAY)),
    
    -- Shipped orders (in transit)
    ('Hoàng Văn Em', 'em.hoang@email.com', 8, 'OnePlus 12', 799.99, 1, 799.99, 'Shipped', DATE_SUB(NOW(), INTERVAL 3 DAY), DATE_SUB(NOW(), INTERVAL 1 DAY)),
    ('Lê Minh Cường', 'cuong.le@email.com', 3, 'iPhone 15', 899.99, 2, 1799.98, 'Shipped', DATE_SUB(NOW(), INTERVAL 2 DAY), DATE_SUB(NOW(), INTERVAL 12 HOUR)),
    ('Mai Thị Hương', 'huong.mai@email.com', 16, 'Apple AirPods Pro 2', 249.99, 2, 499.98, 'Shipped', DATE_SUB(NOW(), INTERVAL 2 DAY), DATE_SUB(NOW(), INTERVAL 18 HOUR)),
    
    -- Processing orders (being prepared)
    ('Ngô Văn Khoa', 'khoa.ngo@email.com', 19, 'iPad Pro 12.9 M2', 1099.99, 1, 1099.99, 'Processing', DATE_SUB(NOW(), INTERVAL 1 DAY), DATE_SUB(NOW(), INTERVAL 6 HOUR)),
    ('Đinh Thị Lan', 'lan.dinh@email.com', 11, 'Samsung Galaxy A54', 449.99, 1, 449.99, 'Processing', DATE_SUB(NOW(), INTERVAL 18 HOUR), DATE_SUB(NOW(), INTERVAL 4 HOUR)),
    ('Trương Văn Minh', 'minh.truong@email.com', 12, 'Xiaomi Redmi Note 13 Pro', 349.99, 3, 1049.97, 'Processing', DATE_SUB(NOW(), INTERVAL 12 HOUR), DATE_SUB(NOW(), INTERVAL 2 HOUR)),
    
    -- Pending orders (just placed)
    ('Huỳnh Văn Sơn', 'son.huynh@email.com', 2, 'iPhone 15 Pro', 1199.99, 1, 1199.99, 'Pending', DATE_SUB(NOW(), INTERVAL 4 HOUR), NULL),
    ('Tô Thị Tâm', 'tam.to@email.com', 10, 'Nothing Phone 2', 699.99, 1, 699.99, 'Pending', DATE_SUB(NOW(), INTERVAL 2 HOUR), NULL),
    ('Lý Văn Uy', 'uy.ly@email.com', 18, 'Anker 65W GaN Charger', 59.99, 2, 119.98, 'Pending', DATE_SUB(NOW(), INTERVAL 1 HOUR), NULL),
    ('Phan Thị Vân', 'van.phan@email.com', 20, 'Samsung Galaxy Tab S9 Ultra', 1199.99, 1, 1199.99, 'Pending', DATE_SUB(NOW(), INTERVAL 30 MINUTE), NULL),
    
    -- Cancelled orders
    ('Cao Văn Xuân', 'xuan.cao@email.com', 14, 'Samsung Galaxy A14', 199.99, 2, 399.98, 'Cancelled', DATE_SUB(NOW(), INTERVAL 5 DAY), DATE_SUB(NOW(), INTERVAL 4 DAY)),
    ('Dương Thị Yến', 'yen.duong@email.com', 15, 'Xiaomi Redmi 13C', 149.99, 1, 149.99, 'Cancelled', DATE_SUB(NOW(), INTERVAL 6 DAY), DATE_SUB(NOW(), INTERVAL 5 DAY));
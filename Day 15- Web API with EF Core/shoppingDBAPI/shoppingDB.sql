IF DB_ID('shoppingDB') IS NULL CREATE DATABASE shoppingDB;
GO
USE shoppingDB;
GO
DROP TABLE IF EXISTS products;
CREATE TABLE products (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    category VARCHAR(50),
    price DECIMAL(10,2)
);
INSERT INTO products (name, category, price) VALUES
('Laptop', 'Electronics', 899.99),
('Headphones', 'Electronics', 59.50),
('Coffee Mug', 'Kitchen', 12.00);
GO
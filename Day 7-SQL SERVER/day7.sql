
USE employeeManagementDB;
GO

-- CREATE TABLE depts
-- (
--     deptNo int primary key,
--     deptName varchar(20),
--     deptLocation varchar(20)
-- );
-- GO

-- INSERT INTO depts VALUES (10,'HR','Texas');
-- INSERT INTO depts VALUES (20,'Accounts','New York');
-- INSERT INTO depts VALUES (30,'Training','San Francisco');
-- INSERT INTO depts VALUES (40,'IT','Vegas');
-- GO

-- CREATE TABLE employees
-- (
--     empNo int primary key,
--     empName varchar(30),
--     empDesignation varchar(20),
--     empSalary int,
--     empIsPermenant bit,
--     empDept int,
--     constraint fk_empDept foreign key (empDept) references depts
-- );
-- GO

-- INSERT INTO employees VALUES (1,'Peter','Developer',3000,1,40);
-- INSERT INTO employees VALUES (2,'Mary','Trainer',4000,1,30);
-- INSERT INTO employees VALUES (3,'Raj','Accountant',5000,1,20);
-- INSERT INTO employees VALUES (4,'Monica','Sr.Accountant',8000,1,20);
-- INSERT INTO employees VALUES (5,'Penny','Sr.Hr',3100,0,10);
-- INSERT INTO employees VALUES (6,'Leonard','Developer',12000,1,40);
-- INSERT INTO employees VALUES (7,'Sheldon','Developer',13000,1,40);
-- INSERT INTO employees VALUES (8,'Mike','Hr Manager',4000,1,10);
-- INSERT INTO employees VALUES (9,'Tyson','Developer',8000,0,10);
-- INSERT INTO employees VALUES (10,'Murray','Sr.Trainer',9000,1,30);
-- INSERT INTO employees VALUES (11,'Peter','Developer',13000,0,40);
-- INSERT INTO employees VALUES (12,'Julie','HR Associate',5000,1,10);
-- GO

SELECT * FROM depts;
GO
SELECT * FROM employees;
GO
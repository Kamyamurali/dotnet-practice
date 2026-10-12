create table Accounts (accNo int, accName varchar(20), accType varchar(20), accBalance int, accIsActive bit, constraint pk_accNo primary key(accNo));
insert into Accounts values(101,'Nikhil','Savings',6000,1),(102,'Sheldon','Checking',6000,1),(103,'Penny','Loan',6000,1),(104,'Raj','Savings',6000,0);
select * from Accounts;
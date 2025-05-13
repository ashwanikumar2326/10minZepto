create table tbl_cart
(
cid int primary key identity(1,1),
pid int foreign key references tbl_product(pid),
uid varchar(100) foreign key references tbl_user(emailid),
quantity int,
total int, 
add_date datetime
)
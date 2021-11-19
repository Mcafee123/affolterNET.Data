create table T_DemoTable(
    Id uniqueidentifier not null
        constraint PK_DemoTable
            primary key clustered,
    Message nvarchar(1000)
)

insert into T_DemoTable (Id, Message) values (newid(), 'It is working!')

go

create view V_Demo
as select * from T_DemoTable
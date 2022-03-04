create table T_DemoTable(
    Id uniqueidentifier not null
        constraint PK_DemoTable
            primary key clustered,
    Message nvarchar(1000) not null,
    Status nvarchar(50) not null
        constraint DF_T_DemoTable_Status
            default 'auto default'
)

insert into T_DemoTable (Id, Message) values (newid(), 'It is working!')

go

create view V_Demo
as select * from T_DemoTable
create table DemoTable(
    Id uniqueidentifier not null
        constraint PK_DemoTable
            primary key clustered,
    Message nvarchar(1000)
)

insert into DemoTable (Id, Message) values (newid(), 'It is working!')
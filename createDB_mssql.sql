create table dbo.Logins
(
    Id_Login int identity
        constraint PK_Logins
            primary key,
    Username nvarchar(50)  not null,
    Password nvarchar(255) not null,
    Usertype tinyint       not null
)
go

create table dbo.Documents
(
    Id          int identity
        constraint PK_Documents
            primary key,
    LoginId     int           not null
        constraint FK_Documents_Logins_LoginId
            references dbo.Logins
            on delete cascade,
    FileName    nvarchar(max) not null,
    ContentType nvarchar(max) not null,
    FileSize    bigint        not null,
    StoragePath nvarchar(max) not null,
    UploadedAt  datetime2     not null
)
go

create index IX_Documents_LoginId
    on dbo.Documents (LoginId)
go

create table dbo.Events
(
    Id        int identity
        constraint PK_Events
            primary key,
    UserId    int           not null
        constraint FK_Events_Logins_UserId
            references dbo.Logins
            on delete cascade,
    Name      nvarchar(max) not null,
    StartDate datetime2     not null,
    EndDate   datetime2     not null
)
go

create index IX_Events_UserId
    on dbo.Events (UserId)
go

create table dbo.Searches
(
    Id     int identity
        constraint PK_Searches
            primary key,
    Word   nvarchar(max) not null,
    Date   datetime2     not null,
    UserId int           not null
        constraint FK_Searches_Logins_UserId
            references dbo.Logins
            on delete cascade
)
go

create index IX_Searches_UserId
    on dbo.Searches (UserId)
go
/*
create table dbo.__EFMigrationsHistory
(
    MigrationId    nvarchar(150) not null
        constraint PK___EFMigrationsHistory
            primary key,
    ProductVersion nvarchar(32)  not null
)
go
*/

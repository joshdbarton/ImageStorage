use [master]

if db_id('SqlImageStorage') is null
	create database [SqlImageStorage]
go

use [SqlImageStorage]
go

drop table if exists [Person];
drop table if exists [Image];

create table [Image] (
	[Id] integer primary key identity(1,1),
	[Image] varbinary(max) not null
);

create table [Person] (
	[Id] integer primary key identity(1,1),
	[Name] nvarchar(255) not null,
	[ImageId] integer,
	constraint [FK_Person_Image] foreign key ([ImageId]) references [Image]([Id])
);
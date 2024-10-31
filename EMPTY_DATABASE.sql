-- depoquick.dbo.Promotions definition

-- Drop table

-- DROP TABLE depoquick.dbo.Promotions;

CREATE TABLE depoquick.dbo.Promotions (
	PromotionId int IDENTITY(1,1) NOT NULL,
	Label nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	DiscountPercentage int NOT NULL,
	StartDate datetime2 NOT NULL,
	EndDate datetime2 NOT NULL,
	IsUsed bit NOT NULL,
	CONSTRAINT PK_Promotions PRIMARY KEY (PromotionId)
);


-- depoquick.dbo.Users definition

-- Drop table

-- DROP TABLE depoquick.dbo.Users;

CREATE TABLE depoquick.dbo.Users (
	UserId int IDENTITY(1,1) NOT NULL,
	Name nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Email nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Password nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	IsAdmin bit NOT NULL,
	CONSTRAINT PK_Users PRIMARY KEY (UserId)
);


-- depoquick.dbo.Warehouses definition

-- Drop table

-- DROP TABLE depoquick.dbo.Warehouses;

CREATE TABLE depoquick.dbo.Warehouses (
	WarehouseId int IDENTITY(1,1) NOT NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Zone] int NOT NULL,
	[Size] int NOT NULL,
	IsHeated bit NOT NULL,
	AvailableFrom datetime2 NOT NULL,
	AvailableTo datetime2 NOT NULL,
	CONSTRAINT PK_Warehouses PRIMARY KEY (WarehouseId)
);


-- depoquick.dbo.Reservations definition

-- Drop table

-- DROP TABLE depoquick.dbo.Reservations;

CREATE TABLE depoquick.dbo.Reservations (
	ReservationId int IDENTITY(1,1) NOT NULL,
	StartDate datetime2 NOT NULL,
	EndDate datetime2 NOT NULL,
	WarehouseId int NOT NULL,
	Price float NOT NULL,
	Status int NOT NULL,
	RejectionNote nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	PaymentStatus int NULL,
	ClientId int NOT NULL,
	CONSTRAINT PK_Reservations PRIMARY KEY (ReservationId),
	CONSTRAINT FK_Reservations_Users_ClientId FOREIGN KEY (ClientId) REFERENCES depoquick.dbo.Users(UserId) ON DELETE CASCADE,
	CONSTRAINT FK_Reservations_Warehouses_WarehouseId FOREIGN KEY (WarehouseId) REFERENCES depoquick.dbo.Warehouses(WarehouseId) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_Reservations_ClientId ON dbo.Reservations (  ClientId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Reservations_WarehouseId ON dbo.Reservations (  WarehouseId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;

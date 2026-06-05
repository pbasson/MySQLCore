CREATE TABLE IF NOT EXISTS MySQLCore.User (
  Id int NOT NULL AUTO_INCREMENT,
  UserName varchar(100) NOT NULL,
  FirstName varchar(100) NOT NULL,
  LastName varchar(100) NOT NULL,
  Email varchar(256) NOT NULL,
  DateOfBirth DATE DEFAULT NULL,
  IsActive boolean NOT NULL DEFAULT TRUE,

  CreatedBy varchar(100) DEFAULT NULL,
  UpdatedBy varchar(100) DEFAULT NULL,
  CreatedDatetime DATETIME,
  UpdatedDatetime DATETIME,

  PRIMARY KEY (Id),
  UNIQUE KEY UX_User_UserName (UserName),
  UNIQUE KEY UX_User_Email (Email)
);


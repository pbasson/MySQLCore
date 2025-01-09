CREATE TABLE IF NOT EXISTS MySQLCore.CRUDTransaction (
  Id int NOT NULL AUTO_INCREMENT,
  Name varchar(100) DEFAULT NULL,

  CreatedBy varchar(100) DEFAULT NULL,
  UpdatedBy varchar(100) DEFAULT NULL,
  CreatedDatetime DATETIME,
  UpdatedDatetime DATETIME,
  
  PRIMARY KEY (Id)
);


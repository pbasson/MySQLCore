CREATE TABLE IF NOT EXISTS MySQLCore.ImageTransaction (
  ImageTransactionID int NOT NULL AUTO_INCREMENT PRIMARY KEY,
  ImageType varchar(100) DEFAULT NULL,

  CreatedBy varchar(100) DEFAULT NULL,
  UpdatedBy varchar(100) DEFAULT NULL,
  CreatedDatetime DATETIME,
  UpdatedDatetime DATETIME
  
);


CREATE TABLE IF NOT EXISTS MySQLCore.ProcessedMessage (
  Id int NOT NULL AUTO_INCREMENT PRIMARY KEY,
  MessageId char(36) NOT NULL,
  MessageType varchar(100) NOT NULL,
  EntityName varchar(100) NOT NULL,
  EntityId int NOT NULL,
  Status int NOT NULL,
  ProcessedAt DATETIME NOT NULL
)
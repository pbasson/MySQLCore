CREATE TABLE IF NOT EXISTS MySQLCore.ImageGallery (
  ImageGalleryId int NOT NULL AUTO_INCREMENT PRIMARY KEY,
  GalleryName varchar(100) DEFAULT NULL,
  GalleryPath varchar(255) DEFAULT NULL,

  CreatedBy varchar(100) DEFAULT NULL,
  UpdatedBy varchar(100) DEFAULT NULL,
  CreatedDatetime DATETIME,
  UpdatedDatetime DATETIME
  
);


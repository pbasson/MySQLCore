CREATE TABLE IF NOT EXISTS MySQLCore.ImageFile (
  ImageFileId int NOT NULL AUTO_INCREMENT PRIMARY KEY,
  ImageGalleryId INT NOT NULL,
  ImageName varchar(100) DEFAULT NULL,
  ImagePath varchar(100) DEFAULT NULL,

  FOREIGN KEY (ImageGalleryId) REFERENCES ImageGallery(ImageGalleryId) ON DELETE CASCADE
);


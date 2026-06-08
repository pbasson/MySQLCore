CREATE TABLE IF NOT EXISTS MySQLCore.ImageFile (
  ImageFileId int NOT NULL AUTO_INCREMENT PRIMARY KEY,
  ImageGalleryId INT NOT NULL,
  ImageName varchar(100) DEFAULT NULL,
  ImagePosition INT NOT NULL DEFAULT 0,

  FOREIGN KEY (ImageGalleryId) REFERENCES ImageGallery(ImageGalleryId) ON DELETE CASCADE,
  UNIQUE KEY UX_ImageFile_ImageGalleryId_ImagePosition (ImageGalleryId, ImagePosition)
);

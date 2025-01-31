CREATE TABLE IF NOT EXISTS MySQLCore.ImageGallery (
  ImageGalleryId int NOT NULL AUTO_INCREMENT PRIMARY KEY,
  ImageTransactionID INT NOT NULL,
  ImagePath varchar(100) DEFAULT NULL,

  FOREIGN KEY (ImageTransactionID) REFERENCES ImageTransaction(ImageTransactionID) ON DELETE CASCADE
);


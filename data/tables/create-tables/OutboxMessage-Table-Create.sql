CREATE TABLE IF NOT EXISTS MySQLCore.OutboxMessage (
    Id BIGINT AUTO_INCREMENT PRIMARY KEY,

    MessageId CHAR(36) NOT NULL,
    EventType VARCHAR(150) NOT NULL,
    Payload JSON NOT NULL,

    EntityName VARCHAR(100) NULL,
    EntityId INT NULL,

    Status INT NOT NULL DEFAULT 1,
    RetryCount INT NOT NULL DEFAULT 0,
    ErrorMessage TEXT NULL,

    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PublishedAt DATETIME NULL,
    LastAttemptAt DATETIME NULL,

    UNIQUE KEY UX_OutboxMessage_MessageId (MessageId),
    INDEX IX_OutboxMessage_Status_CreatedAt (Status, CreatedAt),
    INDEX IX_OutboxMessage_Entity (EntityName, EntityId)
);
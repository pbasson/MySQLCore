# Messaging Integration Tests

Set `MYSQLCORE_TEST_CONNECTION` to a disposable MySQL 8 server, then run
`dotnet test test/MySQLCore.Messaging.IntegrationTest`.

The account must be able to create/drop databases and triggers. Each test creates
and removes its own database named `issue6_<random GUID>`. Without the environment
variable, these tests are reported as skipped. No RabbitMQ server is required;
redelivery is simulated by invoking the repository again after commit.

Production requires the unique `ProcessedMessage.MessageId` index from the table
creation script. Updating that script or the EF model does not migrate an existing
database. Check for duplicates before applying the index to an existing table.

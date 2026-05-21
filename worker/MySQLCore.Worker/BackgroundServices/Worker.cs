// namespace MySQLCore.Worker.BackgroundServices;

// public class Worker : BackgroundService
// {
//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         while (!stoppingToken.IsCancellationRequested)
//         {
//             Console.WriteLine("Running background task...");
//             await Task.Delay(5000, stoppingToken);
//         }
//     }
// }
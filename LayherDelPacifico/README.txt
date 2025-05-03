# LayherDelPacifico Worker Service

## Overview
The **LayherDelPacifico Worker Service** is a robust and scalable .NET 6 Worker Service designed to handle background tasks efficiently. This solution demonstrates advanced expertise in .NET development, leveraging modern practices and patterns to deliver a high-quality, maintainable, and performant application.

## Key Features
- **Background Task Execution**: Implements the `BackgroundService` class to run continuous and scheduled tasks in the background.
- **Folder Monitoring**: Utilizes the `IWatcherFolder` service to monitor and process files in specified directories dynamically.
- **FTP Integration**: Seamlessly integrates with FTP servers using configurable credentials for secure file handling.
- **Log Management**: Includes a `IPurgeLog` service to manage and purge log files, ensuring optimal storage usage.
- **Configuration-Driven**: Fully configurable through `appsettings.json`, supporting modular and environment-specific setups.
- **Error Handling and Logging**: Implements structured logging with `ILogger` to capture and manage application events and errors effectively.

## Technical Highlights
- **.NET 6**: Built on the latest LTS version of .NET, ensuring long-term support and modern features.
- **Dependency Injection**: Follows best practices by injecting services like `IWatcherFolder`, `IPurgeLog`, and `IConfiguration` for testability and modularity.
- **Asynchronous Programming**: Leverages `async/await` for non-blocking operations, ensuring high performance and responsiveness.
- **Clean Architecture**: Adheres to separation of concerns by organizing code into core interfaces, services, and worker layers.

## Developer's Expertise
This project reflects a deep understanding of:
- Building scalable and maintainable .NET applications.
- Implementing background services for enterprise-grade solutions.
- Designing extensible and modular architectures.
- Integrating third-party services like FTP and logging frameworks.
- Writing clean, readable, and testable code.

## How It Works
1. **Configuration**: The service reads configurations for paths, FTP credentials, and logging from `appsettings.json`.
2. **Folder Monitoring**: The `WatcherFolder` service monitors specified directories and processes files based on business logic.
3. **Log Purging**: The `PurgeLog` service ensures log files are managed efficiently by removing old files beyond a configurable limit.
4. **Continuous Execution**: The worker runs continuously, handling tasks in a loop while respecting cancellation tokens for graceful shutdowns.

## Why Choose This Solution?
This solution is a testament to the developer's commitment to delivering high-quality software. It combines modern .NET capabilities with practical design patterns to solve real-world problems effectively. Whether you're looking for a reliable background service or a foundation for more complex systems, this project showcases the expertise and attention to detail required for success.
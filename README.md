<H1 align="center" bold>  GitHub Monitoring Bot </H1>

<H2 align="center">Небольшой .NET-бот, который периодически проверяет pull request'ы в выбранных GitHub-репозиториях и отправляет уведомления в Telegram при появлении новых PR или изменении их статуса.</H2>

## Что умеет

- запускается как фоновый `HostedService`;
- читает настройки из `appsettings.json`;
- получает доступные текущему GitHub-аккаунту репозитории через `GitPat`;
- отслеживает только репозитории из `RepoList`;
- проверяет PR в статусах `Open`, `Merged`, `Closed`;
- сохраняет состояние PR в SQLite (`github-monitoring.db`);
- автоматически применяет EF Core migrations при старте;
- отправляет уведомления в Telegram-чат или в topic через `ThreadId`.

## Стек

- `.NET 10`
- `Microsoft.Extensions.Hosting`
- `Entity Framework Core + SQLite`
- `Octokit`
- `Telegram.Bot`
- `Mapster`
- `xUnit`

## Структура

- `Program.cs` - регистрация DI, конфигурации, GitHub/Telegram клиентов и запуск worker'а.
- `Services/` - GitHub-запросы, мониторинг PR и определение статуса.
- `Notifications/` - форматирование и отправка Telegram-сообщений.
- `Data/` - `DbContext`, миграции и репозиторий записей PR.
- `Models/` - настройки и доменные модели.
- `Mapping/` - маппинг GitHub PR в запись базы.
- `tests/` - unit-тесты форматтера, маппера и resolver'а статуса.

## Конфигурация

Настройки лежат в `appsettings.json`.

```json
{
  "BotToken": "telegram-bot-token",
  "GitPat": "github-personal-access-token",
  "ChatId": "telegram-chat-id",
  "ThreadId": 123,
  "Interval": 10000,
  "RepoList": [
    {
      "RepoName": "SuperHeroApi",
      "EventId": 825809399
    }
  ]
}
```

- `BotToken` - токен Telegram-бота.
- `GitPat` - GitHub Personal Access Token. Аккаунт токена должен видеть репозитории из `RepoList`.
- `ChatId` - ID чата или канала Telegram.
- `ThreadId` - ID topic'а в forum-чате Telegram; можно убрать или поставить `null`, если topic не нужен.
- `Interval` - пауза между проверками в миллисекундах.
- `RepoList[].RepoName` - имя репозитория для отслеживания.
- `RepoList[].EventId` - служебный ID события/репозитория из конфигурации; сейчас не участвует в фильтрации PR.

## Запуск

1. Установите .NET SDK с поддержкой `net10.0`.
2. Заполните `appsettings.json`.
3. Выполните команды из корня проекта:

```bash
dotnet restore
dotnet run
```

При первом запуске приложение создаст `github-monitoring.db` в рабочем каталоге и применит миграции. После этого worker будет проверять GitHub с интервалом из `Interval`.

## Тесты

```bash
dotnet test
```

## Формат уведомлений

Сообщение содержит статус PR, репозиторий, номер PR, заголовок, автора, даты создания/обновления, опционально описание, даты закрытия/мержа, ссылку на репозиторий и ссылку на pull request.

![Пример уведомления](primer.png)

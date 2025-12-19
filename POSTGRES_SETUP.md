# Инструкция по настройке PostgreSQL и EF Core

## Что было сделано:

1. ✅ Добавлены пакеты EF Core и Npgsql в проект `Calls.Infrastructure`
2. ✅ Создан `CallsDbContext` для работы с базой данных
3. ✅ Настроена конфигурация моделей (Room, RoomParticipant, ParticipantSettings, SmallUserInfo)
4. ✅ Реализован `EfRoomRepository` с использованием EF Core
5. ✅ Обновлен `Program.cs` для регистрации DbContext и репозитория
6. ✅ Создан `appsettings.json` с connection string

## Что нужно сделать вам:

### 1. Установить PostgreSQL (если еще не установлен)

Если PostgreSQL еще не установлен на вашем компьютере, установите его:
- Скачайте с официального сайта: https://www.postgresql.org/download/
- Или используйте Docker: `docker run --name postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres`

### 2. Создать базу данных

Подключитесь к PostgreSQL и создайте базу данных:

```sql
CREATE DATABASE CallsDb;
```

Или используйте psql:
```bash
psql -U postgres
CREATE DATABASE CallsDb;
```

### 3. Обновить connection string (если нужно)

В файле `Calls.Api/appsettings.json` обновите connection string под ваши настройки:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=CallsDb;Username=postgres;Password=ваш_пароль"
  }
}
```

Параметры:
- `Host` - адрес сервера PostgreSQL (обычно localhost)
- `Port` - порт (по умолчанию 5432)
- `Database` - имя базы данных (CallsDb)
- `Username` - имя пользователя (обычно postgres)
- `Password` - пароль

### 4. Установить инструменты EF Core (если еще не установлены)

```bash
dotnet tool install --global dotnet-ef
```

Если уже установлены, обновите до последней версии:
```bash
dotnet tool update --global dotnet-ef
```

### 5. Создать миграцию

Выполните команду из корневой папки проекта:

```bash
dotnet ef migrations add InitialCreate --project Calls.Infrastructure\Calls.Infrastructure.csproj --startup-project Calls.Api\Calls.Api.csproj --context CallsDbContext
```

Эта команда создаст файлы миграции в папке `Calls.Infrastructure/Migrations/`.

### 6. Применить миграцию к базе данных

Есть два способа:

**Способ 1: Автоматически при запуске приложения**
Миграции будут применены автоматически при старте приложения в режиме Development (это уже настроено в `Program.cs`).

**Способ 2: Вручную через команду**
```bash
dotnet ef database update --project Calls.Infrastructure\Calls.Infrastructure.csproj --startup-project Calls.Api\Calls.Api.csproj --context CallsDbContext
```

### 7. Проверить работу

Запустите приложение:
```bash
cd Calls.Api
dotnet run
```

Приложение автоматически применит миграции (если они еще не применены) и создаст таблицы в базе данных.

## Структура базы данных

После применения миграций будут созданы следующие таблицы:

- `Rooms` - основная таблица для комнат
  - `RoomId` (Guid, PK)
  - `Name` (string, max 500 символов)

- `RoomParticipants` - таблица участников комнат
  - `RoomId` (Guid, часть PK)
  - `UserId` (Guid, часть PK)
  - Данные из `ParticipantSettings` и `SmallUserInfo` (как отдельные столбцы или вложенные таблицы в зависимости от конфигурации EF Core)

## Важные замечания

1. **Connection String**: Убедитесь, что connection string в `appsettings.json` правильный и соответствует вашим настройкам PostgreSQL.

2. **Пароли**: Не коммитьте `appsettings.json` с реальными паролями в git. Используйте `appsettings.Development.json` для локальной разработки (он уже добавлен в .gitignore).

3. **Миграции**: При изменении моделей создавайте новые миграции командой:
   ```bash
   dotnet ef migrations add НазваниеМиграции --project Calls.Infrastructure\Calls.Infrastructure.csproj --startup-project Calls.Api\Calls.Api.csproj --context CallsDbContext
   ```

4. **Репозиторий**: Теперь используется `EfRoomRepository` вместо `InMemoryRoomRepository`. Все данные будут сохраняться в PostgreSQL.

## Если возникнут проблемы

1. **Ошибка подключения к БД**: Проверьте, что PostgreSQL запущен и connection string правильный.
2. **Ошибки миграции**: Убедитесь, что база данных существует и у пользователя есть права на создание таблиц.
3. **Ошибки компиляции**: Проверьте, что все пакеты NuGet восстановлены: `dotnet restore`


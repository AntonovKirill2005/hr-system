# rut-shop.net

Подробный учебный пример магазина на `ASP.NET Core Minimal API` с акцентом на:

- чистую структуру проекта;
- простую и читаемую бизнес-логику;
- `EF Core + PostgreSQL`;
- подробную Swagger-документацию;
- понятные комментарии в коде.

---

## 1) Что это за проект

`rut-shop.net` демонстрирует базовый backend интернет-магазина:

- каталог товаров;
- список клиентов и их бонусный баланс;
- создание покупки;
- начисление бонусов программы лояльности;
- выгрузку текстового чека.

Проект intentionally учебный: в нем сделаны простые и прозрачные решения, чтобы легче было изучать архитектуру и API-слой.

---

## 2) Технологии

- `.NET 10`
- `ASP.NET Core Minimal API`
- `Entity Framework Core`
- `Npgsql` (`PostgreSQL provider`)
- `Swagger / OpenAPI` (`Swashbuckle`)

---

## 3) Структура проекта

- `app`  
  Исходный код и `RutShop.csproj` (Minimal API, слои `api`, `database`, `dto`, `interfaces`, `model`, `services`); в этой папке лежит `.gitignore` для сборок и артефактов IDE.

- `RutShop.sln`  
  Файл решения Visual Studio / `dotnet` (содержит проект `app/RutShop.csproj`).

- `out`  
  Примеры тел ответов API (JSON и текст чека) для демонстрации без запуска сервиса; см. `out/README.txt`.

Внутри `app`:

- `api` — endpoint-модули (`products`, `customers`, `purchases`).
- `database` — `ShopDbContext`, при необходимости сидирование в Development.
- `dto` — DTO запросов и ответов, маппер.
- `interfaces` — контракты сервисов.
- `model` — доменные сущности.
- `services` — реализации бизнес-логики.
- `Program.cs` — composition root.

---

## 4) Домен и упрощения

В текущей версии покупка хранится в упрощенном формате:

- `Purchase.ProductIds` — массив `Guid` купленных товаров;
- один и тот же `Guid` может повторяться (если купили несколько единиц одного товара);
- итоговая сумма рассчитывается как сумма цен по этим `Guid`;
- бонусы начисляются по итоговой сумме.

Это сделано специально для простоты учебного примера.

---

## 5) Конфигурация PostgreSQL

Строки подключения (в каталоге `app`):

- `app/appsettings.json` → `ConnectionStrings:Postgres`
- `app/appsettings.Development.json` → `ConnectionStrings:Postgres`

По умолчанию:

- host: `localhost`
- port: `5432`
- username: `postgres`
- password: `postgres`
- db: `rut_shop_db` / `rut_shop_db_dev`

Перед запуском убедитесь, что PostgreSQL поднят и база из строки подключения создана (например `CREATE DATABASE rut_shop_db_dev;`).

В среде **Development** при старте вызывается `EnsureCreated` (создание таблиц, если БД пуста). Товары и клиентов нужно завести через API (`POST`), затем создавать покупки.

---

## 6) Как запустить

Из корня репозитория примера:

```bash
dotnet run --project app/RutShop.csproj
```

Или из каталога приложения:

```bash
cd app
dotnet run
```

Сборка всего решения:

```bash
dotnet build RutShop.sln
```

---

## 7) Swagger

После запуска откройте:

- `http://localhost:<port>/swagger`

В Swagger описаны:

- назначение каждого endpoint-а;
- структура входных данных;
- коды ответов (`200/201/400/404`);
- описание поведения бизнес-логики.

---

## 8) Основные endpoint-ы

**Товары:** `GET` список / по id, `POST` создать, `PUT` изменить, `DELETE` удалить (нельзя, если товар есть в покупках).

**Клиенты:** `GET` список / по id, `GET .../loyalty` — только баланс, `POST` / `PUT` / `DELETE` (удаление запрещено, если есть покупки).

**Покупки:** `GET` список / по id, `POST` создать, `PUT` заменить состав (с откатом старых списаний и бонусов), `DELETE` отменить (возврат на склад и бонусов), `GET .../receipt` — текстовый чек.

---

## 9) Пример создания покупки

`POST /api/purchases`

```json
{
  "customerId": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1",
  "productIds": [
    "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1",
    "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2",
    "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"
  ]
}
```

---

## 10) Быстрая проверка API

В проекте есть файл:

- `app/rut-shop.net.http`

Он содержит готовые запросы для ручного тестирования в IDE (подставьте адрес и `purchaseId` из ответа после создания покупки).

---

## 11) Почему такая архитектура

Эта структура выбрана как «чистый базовый шаблон»:

- endpoint-ы только принимают/отдают данные;
- бизнес-правила находятся в сервисах;
- доступ к БД изолирован через `DbContext`;
- модели/DTO не смешаны;
- проект легко расширять (например, добавить заказы, оплату, роли, миграции).

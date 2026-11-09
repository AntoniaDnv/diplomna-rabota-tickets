# QueuEase

Дипломен проект: „Система за управление на опашки с електронни талони“.

Уеб система за издаване на електронни талони, извикване, пренасочване и отчитане на обслужени посетители. Демонстрационният сценарий е регистратура на ДКЦ.

**Автор:** Антония Пенкова Динева, 12В клас
**Ръководител консултант:** Ивайло Миланов

## Документация

- [Техническа спецификация](docs/specification.md)

## Технологии

ASP.NET Core MVC (.NET 10), Entity Framework Core, MS SQL Server, ASP.NET Core Identity, SignalR, xUnit.

## Стартиране

```bash
dotnet build
dotnet test
dotnet run --project src/QueuEase.Web
```

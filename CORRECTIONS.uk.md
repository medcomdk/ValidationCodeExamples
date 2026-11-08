# CORRECTIONS.uk.md

**Репозиторій:** ValidationCodeExamples  
**URL:** https://github.com/medcomdk/ValidationCodeExamples  
**Аналізована гілка:** main (коміт 22822a7f7028fc89b51fad5a5b980e24c1b86548)  
**Цільовий форк:** https://github.com/admeduardovieira/ValidationCodeExamples  
**Дата:** 2026-11-08  
**Агент:** Kilo (REA 6.7.0), ідентичність Git admeduardovieira / admeduardovieira@users.noreply.github.com  

---

## Предложені виправлення та статус

| ID | Назва | Severity | Статус | Локація | Ризик | Пропонований PR |
|----|-------|----------|--------|---------|-------|-----------------|
| REA-015 | Замінити `Coding.First()` на `FirstOrDefault()` + перевірка null | P2 | Пропозиція (ще не реалізовано) | `FirelySDKDemo/Program.cs:84` | Низький | `fix/rea-015-coding-first-or-default` |

---

## Деталі по кожному виправленню

### REA-015 — `Coding.First()` → `FirstOrDefault()` + перевірка null

**Проблема:** `Program.cs:84` викликає `issue.Details?.Coding.First().CodeElement?.Value`. Якщо список `Coding` порожній, `.First()` кидає `InvalidOperationException`.

**Довід:** `Program.cs:84` — `Console.WriteLine($"IssueCode: {issue} - Issue details {issue.Details?.Text} - Error Code : {issue.Details?.Coding.First().CodeElement?.Value}");`

**Причина:** використання `.First()` на колекції, яка може бути порожньою.

**Пропонована виправка:** замінити `.First()` на `.FirstOrDefault()` і перевірити null перед доступом до `.CodeElement?.Value`.

**Файли:** `FirelySDKDemo/FirelySDKDemo/Program.cs`

**Тест валідації:** додати тест, що генерує проблему валідації без Coding і підтвердити, що виняток не виникає при виведенні.

**Ризик:** низький.

**Польза:** уникнути InvalidOperationException при виведен проблем.

**Пріоритет:** негайно.

---

## Не виправлені проблеми та залишкові ризики

- Виправлення вище має статус "Пропозиція" на цьому етапі аудиту (Фаза 1 завершена). Жодної зміни не застосовано до вихідного коду ще.
- Обмеження: dotnet не встановлено; не вдалося запустити `dotnet build`/`dotnet test` для динамічної валідації виправлень.
- Повторювана задокументована проблема (`conformsTo` не підтримується) виправляється支持никами; не є ціллю цього аудиту.

---

## Записи звітів

- База PT: `REVERSE_ENGINEERING_REPORT.md` та `CORRECTIONS.md` (це файл).
- Переклади: `REVERSE_ENGINEERING_REPORT.en.md`, `REVERSE_ENGINEERING_REPORT.uk.md`, `CORRECTIONS.en.md`, `CORRECTIONS.uk.md`.
- Всі в корені робочого клону / області аудиту, поруч з базовими звітами portugalsкою.

**Примітка перекладу (uk):** Технічний переклад з допомогою автоматики; рекомендується перегляд носієм мови перед використанням як нормативного довідника. Перегляду носієм не отримано.

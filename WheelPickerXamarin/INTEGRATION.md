# Инструкция по интеграции WheelPicker для Xamarin.iOS

## Структура проекта

Проект портирован на Xamarin.iOS и находится в папке `WheelPickerXamarin/`.

### Структура файлов:

```
WheelPickerXamarin/
├── WheelPickerXamarin.csproj          # Файл проекта
├── Classes/
│   ├── WheelPickerStyle.cs           # Перечисление стилей
│   ├── IWheelPickerDataSource.cs     # Интерфейс источника данных
│   ├── IWheelPickerDelegate.cs       # Интерфейс делегата
│   ├── IWheelPickerLayoutDelegate.cs # Интерфейс делегата layout
│   ├── WheelPickerCollectionViewLayout.cs # Кастомный layout
│   ├── WheelPickerCell.cs            # Ячейка коллекции
│   └── WheelPicker.cs                # Основной класс
├── Example/
│   └── ViewController.cs             # Пример использования
└── README.md                          # Документация
```

## Шаги по интеграции

### 1. Добавление проекта в решение

1. Откройте ваше решение Xamarin.iOS в Visual Studio или Visual Studio for Mac
2. Правой кнопкой на решение → Add → Add Existing Project
3. Выберите файл `WheelPickerXamarin/WheelPickerXamarin.csproj`

### 2. Добавление ссылки на проект

1. В вашем проекте приложения правой кнопкой на References
2. Выберите Add Reference
3. В разделе Projects выберите `WheelPickerXamarin`

### 3. Использование в коде

Добавьте using директиву:

```csharp
using WheelPickerXamarin;
```

Используйте WheelPicker как показано в примере `Example/ViewController.cs`.

## Основные отличия от Swift версии

1. **Интерфейсы вместо протоколов**: В C# используются интерфейсы `IWheelPickerDataSource` и `IWheelPickerDelegate` вместо Swift протоколов
2. **Опциональные методы**: Методы в интерфейсах могут возвращать `null` или иметь пустую реализацию для опциональных методов
3. **Синтаксис**: Используется синтаксис C# вместо Swift
4. **Делегаты**: Вместо `weak var delegate` используется обычное свойство `public IWheelPickerDelegate Delegate`

## Требования

- Xamarin.iOS
- iOS 13.0 или выше
- .NET Standard 2.0 или выше

## Известные ограничения

- Все методы интерфейсов должны быть реализованы, даже если они опциональные (можно оставить пустую реализацию или вернуть null)
- Некоторые Swift-специфичные возможности могут работать по-другому в C#

## Поддержка

При возникновении проблем проверьте:
1. Правильность ссылок на проект
2. Версию iOS SDK (должна быть 13.0 или выше)
3. Правильность реализации интерфейсов


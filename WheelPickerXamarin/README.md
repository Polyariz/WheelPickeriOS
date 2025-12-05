# WheelPicker для Xamarin.iOS

Портированная версия WheelPicker из Swift на C# для использования в проектах Xamarin.iOS.

## Описание

WheelPicker - это простой и настраиваемый компонент для выбора значений в горизонтальном или вертикальном направлении. Поддерживает отображение текста или изображений, имеет два стиля: 3D и Flat.

## Возможности

- ✅ Вертикальный или горизонтальный picker
- ✅ Данные в виде изображений или текста
- ✅ Настройка UILabel и UIImageView
- ✅ Стили Flat и 3D
- ✅ Кастомная маска
- ✅ Настройка fisheye фактора

## Установка

1. Добавьте проект `WheelPickerXamarin.csproj` в ваше решение Xamarin.iOS
2. Добавьте ссылку на проект в вашем приложении
3. Используйте `using WheelPickerXamarin;` в вашем коде

## Использование

### Базовый пример

```csharp
using WheelPickerXamarin;
using UIKit;
using CoreGraphics;

public class MyViewController : UIViewController, IWheelPickerDataSource, IWheelPickerDelegate
{
    private WheelPicker picker;
    private string[] items = { "Item 1", "Item 2", "Item 3" };

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        picker = new WheelPicker(new CGRect(50, 100, 200, 300))
        {
            DataSource = this,
            Delegate = this,
            InteritemSpacing = 25.0f,
            Style = WheelPickerStyle.StyleFlat,
            ScrollDirection = UICollectionViewScrollDirection.Vertical
        };
        
        View.AddSubview(picker);
    }

    public int NumberOfItems(WheelPicker wheelPicker)
    {
        return items.Length;
    }

    public string TitleFor(WheelPicker wheelPicker, int index)
    {
        return items[index];
    }

    public UIImage ImageFor(WheelPicker wheelPicker, int index)
    {
        return null;
    }

    public void WheelPickerDidSelectItemAt(WheelPicker wheelPicker, int index)
    {
        Console.WriteLine($"Selected: {items[index]}");
    }

    public CGSize? WheelPickerMarginForItem(WheelPicker wheelPicker, int index)
    {
        return null;
    }

    public void WheelPickerConfigureLabel(WheelPicker wheelPicker, UILabel label, int index)
    {
        // Опциональная настройка label
    }

    public void WheelPickerConfigureImageView(WheelPicker wheelPicker, UIImageView imageView, int index)
    {
        // Опциональная настройка imageView
    }
}
```

### Настройка свойств

```csharp
picker.InteritemSpacing = 25.0f;           // Расстояние между элементами
picker.FisheyeFactor = 0.001f;              // Фактор искажения
picker.Style = WheelPickerStyle.Style3D;    // Стиль: Style3D или StyleFlat
picker.IsMaskDisabled = false;              // Включить/выключить маску
picker.ScrollDirection = UICollectionViewScrollDirection.Vertical; // Направление прокрутки
picker.TextColor = UIColor.Blue.ColorWithAlpha(0.5f);
picker.HighlightedTextColor = UIColor.Blue;
picker.Font = UIFont.SystemFontOfSize(18.0f);
picker.HighlightedFont = UIFont.BoldSystemFontOfSize(20.0f);
```

### Программный выбор элемента

```csharp
picker.Select(2, animated: true);  // Выбрать элемент с индексом 2
picker.ScrollTo(2, animated: true); // Прокрутить к элементу с индексом 2
```

### Обновление данных

```csharp
picker.ReloadData();
```

## Требования

- Xamarin.iOS
- iOS 13.0 или выше
- .NET Standard 2.0 или выше

## Лицензия

MIT License - см. файл LICENSE

## Портирование

Этот проект является портированной версией оригинального Swift проекта [WheelPicker](https://github.com/TheMindStudios/WheelPicker).


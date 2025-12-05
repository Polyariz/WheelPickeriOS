//
//  IWheelPickerDataSource.cs
//  WheelPickerXamarin
//
//  Ported from Swift to C# for Xamarin.iOS
//

using UIKit;

namespace WheelPickerXamarin
{
    public interface IWheelPickerDataSource
    {
        int NumberOfItems(WheelPicker wheelPicker);
        
        string TitleFor(WheelPicker wheelPicker, int index); // Optional - can return null
        
        UIImage ImageFor(WheelPicker wheelPicker, int index); // Optional - can return null
    }
}


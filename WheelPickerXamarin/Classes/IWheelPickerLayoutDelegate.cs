//
//  IWheelPickerLayoutDelegate.cs
//  WheelPickerXamarin
//
//  Ported from Swift to C# for Xamarin.iOS
//

using UIKit;

namespace WheelPickerXamarin
{
    public interface IWheelPickerLayoutDelegate
    {
        WheelPickerStyle PickerViewStyle(WheelPickerCollectionViewLayout layout);
    }
}


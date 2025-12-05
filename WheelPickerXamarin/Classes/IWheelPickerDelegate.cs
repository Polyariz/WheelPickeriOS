//
//  IWheelPickerDelegate.cs
//  WheelPickerXamarin
//
//  Ported from Swift to C# for Xamarin.iOS
//

using CoreGraphics;
using UIKit;

namespace WheelPickerXamarin
{
    public interface IWheelPickerDelegate
    {
        void WheelPickerDidSelectItemAt(WheelPicker wheelPicker, int index); // Optional - can be empty
        
        CGSize? WheelPickerMarginForItem(WheelPicker wheelPicker, int index); // Optional - can return null
        
        void WheelPickerConfigureLabel(WheelPicker wheelPicker, UILabel label, int index); // Optional - can be empty
        
        void WheelPickerConfigureImageView(WheelPicker wheelPicker, UIImageView imageView, int index); // Optional - can be empty
    }
}


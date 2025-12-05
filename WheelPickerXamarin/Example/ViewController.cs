//
//  ViewController.cs
//  WheelPickerXamarin Example
//
//  Example usage of WheelPicker for Xamarin.iOS
//

using System;
using CoreGraphics;
using UIKit;
using WheelPickerXamarin;

namespace WheelPickerXamarinExample
{
    public class ViewController : UIViewController, IWheelPickerDataSource, IWheelPickerDelegate
    {
        private WheelPicker monthPicker;
        private WheelPicker yearPicker;

        private string[] months = { "November", "December", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October" };
        private string[] years = { "2011", "2012", "2013", "2014", "2015", "2016", "2017" };

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.Black;

            // Setup Month Picker
            monthPicker = new WheelPicker(new CGRect(50, 100, 150, 200))
            {
                DataSource = this,
                Delegate = this,
                InteritemSpacing = 25.0f,
                Style = WheelPickerStyle.StyleFlat,
                IsMaskDisabled = true,
                ScrollDirection = UICollectionViewScrollDirection.Vertical,
                TextColor = UIColor.White.ColorWithAlpha(0.5f),
                HighlightedTextColor = UIColor.White
            };
            View.AddSubview(monthPicker);

            // Setup Year Picker
            yearPicker = new WheelPicker(new CGRect(220, 100, 150, 200))
            {
                DataSource = this,
                Delegate = this,
                InteritemSpacing = 25.0f,
                Style = WheelPickerStyle.StyleFlat,
                IsMaskDisabled = true,
                ScrollDirection = UICollectionViewScrollDirection.Vertical,
                TextColor = UIColor.White.ColorWithAlpha(0.5f),
                HighlightedTextColor = UIColor.White
            };
            View.AddSubview(yearPicker);
        }

        // MARK: - IWheelPickerDataSource

        public int NumberOfItems(WheelPicker wheelPicker)
        {
            if (monthPicker == wheelPicker)
            {
                return months.Length;
            }
            else if (yearPicker == wheelPicker)
            {
                return years.Length;
            }
            return 0;
        }

        public string TitleFor(WheelPicker wheelPicker, int index)
        {
            if (monthPicker == wheelPicker)
            {
                return months[index];
            }
            else if (yearPicker == wheelPicker)
            {
                return years[index];
            }
            return null;
        }

        public UIImage ImageFor(WheelPicker wheelPicker, int index)
        {
            return null; // Not using images in this example
        }

        // MARK: - IWheelPickerDelegate

        public void WheelPickerDidSelectItemAt(WheelPicker wheelPicker, int index)
        {
            if (monthPicker == wheelPicker)
            {
                Console.WriteLine($"Selected month: {months[index]}");
            }
            else if (yearPicker == wheelPicker)
            {
                Console.WriteLine($"Selected year: {years[index]}");
            }
        }

        public CGSize? WheelPickerMarginForItem(WheelPicker wheelPicker, int index)
        {
            return new CGSize(0.0f, 0.0f);
        }

        public void WheelPickerConfigureLabel(WheelPicker wheelPicker, UILabel label, int index)
        {
            // Optional customization of label
            // label.BackgroundColor = UIColor.FromHSB(index / (float)months.Length, 1.0f, 1.0f);
        }

        public void WheelPickerConfigureImageView(WheelPicker wheelPicker, UIImageView imageView, int index)
        {
            // Optional customization of image view
        }
    }
}


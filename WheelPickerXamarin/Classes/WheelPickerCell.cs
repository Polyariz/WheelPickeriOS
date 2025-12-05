//
//  WheelPickerCell.cs
//  WheelPickerXamarin
//
//  Ported from Swift to C# for Xamarin.iOS
//

using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace WheelPickerXamarin
{
    public class WheelPickerCell : UICollectionViewCell
    {
        public static readonly NSString Identifier = new NSString("WheelPickerCell");

        public UILabel Label { get; private set; }
        public UIImageView ImageView { get; private set; }
        public UIFont Font { get; set; }
        public UIFont HighlightedFont { get; set; }

        [Export("initWithFrame:")]
        public WheelPickerCell(CGRect frame) : base(frame)
        {
            Layer.DoubleSided = false;
            
            Label = new UILabel(ContentView.Bounds)
            {
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.Gray,
                Lines = 1,
                LineBreakMode = UILineBreakMode.TailTruncation,
                HighlightedTextColor = UIColor.Black,
                AutoresizingMask = UIViewAutoresizing.FlexibleMargins
            };
            ContentView.AddSubview(Label);

            ImageView = new UIImageView(ContentView.Bounds)
            {
                BackgroundColor = UIColor.Clear,
                ContentMode = UIViewContentMode.Center,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight
            };
            ContentView.AddSubview(ImageView);
        }

        public override bool Selected
        {
            get => base.Selected;
            set
            {
                if (base.Selected != value && !base.Selected)
                {
                    var transition = CATransition.CreateAnimation();
                    transition.Duration = 0.15;
                    transition.Type = CAAnimation.TransitionFade;
                    Label.Layer.AddAnimation(transition, null);
                    Label.Font = value ? HighlightedFont : Font;
                }
                base.Selected = value;
            }
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();
            Label.Text = "";
            ImageView.Image = null;
        }
    }
}


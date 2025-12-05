//
//  WheelPickerCollectionViewLayout.cs
//  WheelPickerXamarin
//
//  Ported from Swift to C# for Xamarin.iOS
//

using System;
using System.Collections.Generic;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace WheelPickerXamarin
{
    public class WheelPickerCollectionViewLayout : UICollectionViewFlowLayout
    {
        private nfloat width = 0.0f;
        private nfloat height = 0.0f;
        private nfloat midX = 0.0f;
        private nfloat midY = 0.0f;
        private nfloat maxAngle = 0.0f;

        public IWheelPickerLayoutDelegate Delegate { get; set; }

        public override void PrepareLayout()
        {
            base.PrepareLayout();

            var visibleRect = new CGRect(
                CollectionView?.ContentOffset ?? CGPoint.Empty,
                CollectionView?.Bounds.Size ?? CGSize.Empty
            );
            
            midX = visibleRect.GetMidX();
            midY = visibleRect.GetMidY();
            width = visibleRect.Width / 2;
            height = visibleRect.Height / 2;
            maxAngle = (nfloat)(Math.PI / 2);
        }

        public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds)
        {
            return true;
        }

        public override UICollectionViewLayoutAttributes LayoutAttributesForItem(NSIndexPath indexPath)
        {
            var attributes = base.LayoutAttributesForItem(indexPath);
            
            if (attributes == null || Delegate == null)
                return attributes;

            var style = Delegate.PickerViewStyle(this);

            switch (style)
            {
                case WheelPickerStyle.StyleFlat:
                    return attributes;

                case WheelPickerStyle.Style3D:
                    if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
                    {
                        var distance = attributes.Frame.GetMidX() - midX;
                        var currentAngle = maxAngle * distance / width / (nfloat)(Math.PI / 2.0);
                        var transform = CATransform3D.Identity;
                        transform = CATransform3D.Translate(transform, -distance, 0.0f, -width);
                        transform = CATransform3D.Rotate(transform, currentAngle, 0, 1, 0);
                        transform = CATransform3D.Translate(transform, 0, 0, width);
                        
                        if (Math.Abs(currentAngle) < maxAngle)
                        {
                            attributes.Transform3D = transform;
                            attributes.Alpha = 1.0f;
                        }
                        else
                        {
                            attributes.Alpha = 0.0f;
                        }
                    }
                    else // Vertical
                    {
                        var distance = attributes.Frame.GetMidY() - midY;
                        var currentAngle = maxAngle * distance / height / (nfloat)(Math.PI / 2);
                        var transform = CATransform3D.Identity;
                        transform = CATransform3D.Translate(transform, 0, -distance, 0);
                        transform = CATransform3D.Rotate(transform, currentAngle, 1, 0, 0);
                        transform = CATransform3D.Translate(transform, 0, distance, 0);
                        
                        if (Math.Abs(currentAngle) < maxAngle / 2)
                        {
                            attributes.Transform3D = transform;
                            attributes.Alpha = 1.0f;
                        }
                        else
                        {
                            attributes.Alpha = 0.0f;
                        }
                    }
                    return attributes;
            }

            return attributes;
        }

        public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
        {
            if (Delegate == null)
                return base.LayoutAttributesForElementsInRect(rect);

            var style = Delegate.PickerViewStyle(this);

            switch (style)
            {
                case WheelPickerStyle.StyleFlat:
                    return base.LayoutAttributesForElementsInRect(rect);

                case WheelPickerStyle.Style3D:
                    var attributes = new List<UICollectionViewLayoutAttributes>();
                    
                    if (CollectionView != null && CollectionView.NumberOfSections() > 0)
                    {
                        var itemCount = CollectionView.NumberOfItemsInSection(0);
                        for (nuint index = 0; index < itemCount; index++)
                        {
                            var indexPath = NSIndexPath.FromItemSection(index, 0);
                            var attribut = LayoutAttributesForItem(indexPath);
                            if (attribut != null)
                            {
                                attributes.Add(attribut);
                            }
                            else
                            {
                                attributes.Add(new UICollectionViewLayoutAttributes());
                            }
                        }
                    }
                    return attributes.ToArray();
            }

            return base.LayoutAttributesForElementsInRect(rect);
        }
    }
}


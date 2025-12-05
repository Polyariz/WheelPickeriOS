//
//  WheelPicker.cs
//  WheelPickerXamarin
//
//  Ported from Swift to C# for Xamarin.iOS
//

using System;
using System.Linq;
using AudioToolbox;
using CoreGraphics;
using CoreAnimation;
using Foundation;
using UIKit;

namespace WheelPickerXamarin
{
    public class WheelPicker : UIView, IUICollectionViewDataSource, IUICollectionViewDelegate, IUICollectionViewDelegateFlowLayout, IWheelPickerLayoutDelegate
    {
        private UICollectionView collectionView;
        private int selectedItem = 0;
        private int prevIndex = 0;

        public IWheelPickerDelegate Delegate { get; set; }
        public IWheelPickerDataSource DataSource { get; set; }

        public UIFont Font { get; set; } = UIFont.SystemFontOfSize(20.0f);
        public UIFont HighlightedFont { get; set; } = UIFont.BoldSystemFontOfSize(20.0f);
        public UIColor TextColor { get; set; } = UIColor.DarkGray;
        public UIColor HighlightedTextColor { get; set; } = UIColor.Black;

        private bool isMaskDisabled = false;
        public bool IsMaskDisabled
        {
            get => isMaskDisabled;
            set
            {
                isMaskDisabled = value;
                if (!isMaskDisabled)
                {
                    var maskLayer = new CAGradientLayer
                    {
                        Frame = Bounds,
                        Colors = new CGColor[]
                        {
                            UIColor.Clear.CGColor,
                            UIColor.Black.CGColor,
                            UIColor.Black.CGColor,
                            UIColor.Clear.CGColor
                        },
                        Locations = new NSNumber[] { 0.0f, 0.33f, 0.66f, 1.0f }
                    };

                    if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
                    {
                        maskLayer.StartPoint = new CGPoint(0.0f, 0.0f);
                        maskLayer.EndPoint = new CGPoint(1.0f, 0.0f);
                    }
                    else
                    {
                        maskLayer.StartPoint = new CGPoint(0.0f, 0.0f);
                        maskLayer.EndPoint = new CGPoint(0.0f, 1.0f);
                    }

                    Layer.Mask = maskLayer;
                    Layer.MasksToBounds = true;
                }
                else
                {
                    Layer.Mask = null;
                }
            }
        }

        public nfloat InteritemSpacing { get; set; } = 10.0f;

        private nfloat fisheyeFactor = 0.0001f;
        public nfloat FisheyeFactor
        {
            get => fisheyeFactor;
            set
            {
                fisheyeFactor = value;
                var clampedValue = Math.Max(Math.Min(fisheyeFactor, 1.0f), 0.0f);
                var transform = CATransform3D.Identity;

                if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
                {
                    transform.M34 = -clampedValue;
                }
                else
                {
                    transform.M34 = clampedValue;
                }

                if (collectionView != null)
                {
                    collectionView.Layer.SublayerTransform = transform;
                }
            }
        }

        public WheelPickerStyle Style { get; set; } = WheelPickerStyle.Style3D;

        private UICollectionViewScrollDirection scrollDirection = UICollectionViewScrollDirection.Horizontal;
        public UICollectionViewScrollDirection ScrollDirection
        {
            get => scrollDirection;
            set
            {
                scrollDirection = value;
                var fisheye = FisheyeFactor;
                FisheyeFactor = fisheye;
                var mask = IsMaskDisabled;
                IsMaskDisabled = mask;
                ReloadData();
            }
        }

        public int SelectedItem
        {
            get => selectedItem;
            private set => selectedItem = value;
        }

        public WheelPicker(CGRect frame) : base(frame)
        {
            Initialize();
        }

        public WheelPicker(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        private void Initialize()
        {
            collectionView = new UICollectionView(Bounds, CollectionViewLayout())
            {
                Delegate = this,
                DataSource = this,
                BackgroundColor = BackgroundColor,
                DecelerationRate = UIScrollView.DecelerationRateFast,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
                ShowsVerticalScrollIndicator = false,
                ShowsHorizontalScrollIndicator = false
            };

            collectionView.RegisterClassForCell(typeof(WheelPickerCell), WheelPickerCell.Identifier);
            Layer.Mask = null;
            AddSubview(collectionView);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            collectionView.CollectionViewLayout = CollectionViewLayout();
            collectionView.Layer.Mask.Frame = collectionView.Bounds;
        }

        private WheelPickerCollectionViewLayout CollectionViewLayout()
        {
            var layout = new WheelPickerCollectionViewLayout
            {
                ScrollDirection = ScrollDirection,
                SectionInset = UIEdgeInsets.Zero,
                MinimumLineSpacing = 0.0f,
                Delegate = this
            };
            return layout;
        }

        // MARK: - Public methods

        public void ReloadData()
        {
            InvalidateIntrinsicContentSize();
            collectionView.CollectionViewLayout.InvalidateLayout();
            collectionView.ReloadData();

            if (DataSource?.NumberOfItems(this) == null)
            {
                return;
            }

            collectionView.SetNeedsDisplay();
            Selected(selectedItem, false, false);
        }

        public void Select(int item, bool animated)
        {
            Selected(item, animated, true);
        }

        public void ScrollTo(int item, bool animated)
        {
            switch (Style)
            {
                case WheelPickerStyle.StyleFlat:
                    var indexPath = NSIndexPath.FromItemSection((nuint)item, 0);
                    var position = ScrollDirection == UICollectionViewScrollDirection.Horizontal
                        ? UICollectionViewScrollPosition.CenteredHorizontally
                        : UICollectionViewScrollPosition.CenteredVertically;
                    collectionView.ScrollToItem(indexPath, position, animated);
                    break;

                case WheelPickerStyle.Style3D:
                    if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
                    {
                        collectionView.SetContentOffset(
                            new CGPoint(OffsetForItem(item), collectionView.ContentOffset.Y),
                            animated
                        );
                    }
                    else
                    {
                        collectionView.SetContentOffset(
                            new CGPoint(collectionView.ContentOffset.X, OffsetForItem(item)),
                            animated
                        );
                    }
                    break;
            }
        }

        // MARK: - Private methods

        private void Selected(int item, bool animated, bool notifySelection)
        {
            var scrollPosition = ScrollDirection == UICollectionViewScrollDirection.Vertical
                ? UICollectionViewScrollPosition.CenteredVertically
                : UICollectionViewScrollPosition.CenteredHorizontally;

            selectedItem = item;
            var indexPath = NSIndexPath.FromItemSection((nuint)item, 0);
            collectionView.SelectItem(indexPath, animated, scrollPosition);
            ScrollTo(item, animated);

            if (notifySelection)
            {
                Delegate?.WheelPickerDidSelectItemAt(this, item);
            }
        }

        private nfloat OffsetForItem(int item)
        {
            nfloat offset = 0.0f;

            for (int index = 0; index < item; index++)
            {
                var indexPath = NSIndexPath.FromItemSection((nuint)index, 0);
                var cellSize = CollectionViewSizeForItem(collectionView, collectionView.CollectionViewLayout, indexPath);

                if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
                {
                    offset += cellSize.Width;
                }
                else
                {
                    offset += cellSize.Height;
                }
            }

            var firstIndexPath = NSIndexPath.FromItemSection(0, 0);
            var firstSize = CollectionViewSizeForItem(collectionView, collectionView.CollectionViewLayout, firstIndexPath);
            var selectedIndexPath = NSIndexPath.FromItemSection((nuint)item, 0);
            var selectedSize = CollectionViewSizeForItem(collectionView, collectionView.CollectionViewLayout, selectedIndexPath);

            if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
            {
                offset -= (firstSize.Width - selectedSize.Width) / 2;
            }
            else
            {
                offset -= (firstSize.Height - selectedSize.Height) / 2;
            }

            return offset;
        }

        private CGSize CollectionViewSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            var size = ScrollDirection == UICollectionViewScrollDirection.Horizontal
                ? new CGSize(InteritemSpacing, collectionView.Bounds.Size.Height)
                : new CGSize(collectionView.Bounds.Size.Width, InteritemSpacing);

            var title = DataSource?.TitleFor(this, (int)indexPath.Item);
            if (!string.IsNullOrEmpty(title))
            {
                var titleSize = SizeForString(title);
                if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
                {
                    size.Width += titleSize.Width;
                }
                else
                {
                    size.Height += titleSize.Height;
                }

                var marginSize = Delegate?.WheelPickerMarginForItem(this, (int)indexPath.Item);
                if (marginSize.HasValue)
                {
                    if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
                    {
                        size.Width += marginSize.Value.Width * 2;
                    }
                    else
                    {
                        size.Height += marginSize.Value.Height * 2;
                    }
                }
            }
            else
            {
                var image = DataSource?.ImageFor(this, (int)indexPath.Item);
                if (image != null)
                {
                    if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
                    {
                        size.Width += image.Size.Width;
                    }
                    else
                    {
                        size.Height += image.Size.Height;
                    }
                }
            }

            return size;
        }

        private CGSize SizeForString(string str)
        {
            var size = str.GetSizeUsingAttributes(new UIStringAttributes { Font = Font });
            var highlightedSize = str.GetSizeUsingAttributes(new UIStringAttributes { Font = HighlightedFont });

            return new CGSize(
                (nfloat)Math.Ceiling(Math.Max(size.Width, highlightedSize.Width)),
                (nfloat)Math.Ceiling(Math.Max(size.Height, highlightedSize.Height))
            );
        }

        private void DidEndScrolling()
        {
            switch (Style)
            {
                case WheelPickerStyle.StyleFlat:
                    var center = new CGPoint(collectionView.Bounds.Width / 2, collectionView.Bounds.Height / 2);
                    var indexPath = collectionView.IndexPathForItemAtPoint(center);
                    if (indexPath != null)
                    {
                        Select((int)indexPath.Item, true);
                    }
                    break;

                case WheelPickerStyle.Style3D:
                    var numberOfItems = DataSource?.NumberOfItems(this);
                    if (numberOfItems == null)
                        return;

                    for (int index = 0; index < numberOfItems.Value; index++)
                    {
                        var itemIndexPath = NSIndexPath.FromItemSection((nuint)index, 0);
                        var cell = collectionView.CellForItem(itemIndexPath);
                        if (cell != null)
                        {
                            var half = ScrollDirection == UICollectionViewScrollDirection.Horizontal
                                ? cell.Bounds.Size.Width / 2
                                : cell.Bounds.Size.Height / 2;
                            var currentOffset = ScrollDirection == UICollectionViewScrollDirection.Horizontal
                                ? collectionView.ContentOffset.X
                                : collectionView.ContentOffset.Y;

                            if (OffsetForItem(index) + half > currentOffset)
                            {
                                Select(index, true);
                                break;
                            }
                        }
                    }
                    break;
            }
        }

        private void GenerateFeedback()
        {
            SystemSound.PlaySystemSound(1104);

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                var generator = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Medium);
                generator.ImpactOccurred();
            }
        }

        // MARK: - UICollectionViewDataSource

        public nint NumberOfSections(UICollectionView collectionView)
        {
            var count = DataSource?.NumberOfItems(this);
            return count > 0 ? 1 : 0;
        }

        public nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return DataSource?.NumberOfItems(this) ?? 0;
        }

        public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(WheelPickerCell.Identifier, indexPath) as WheelPickerCell;
            if (cell == null)
            {
                return new UICollectionViewCell();
            }

            var title = DataSource?.TitleFor(this, (int)indexPath.Item);
            if (!string.IsNullOrEmpty(title))
            {
                cell.Label.Hidden = false;
                cell.ImageView.Hidden = true;

                cell.Label.Text = title;
                cell.Label.TextColor = TextColor;
                cell.Label.HighlightedTextColor = HighlightedTextColor;
                cell.Label.Font = Font;
                cell.Font = Font;
                cell.HighlightedFont = HighlightedFont;
                cell.Label.Bounds = new CGRect(CGPoint.Empty, SizeForString(title));

                var margin = Delegate?.WheelPickerMarginForItem(this, (int)indexPath.Item);
                if (margin.HasValue)
                {
                    cell.Label.Frame = cell.Label.Frame.Inset(-margin.Value.Width, -margin.Value.Height);
                }

                Delegate?.WheelPickerConfigureLabel(this, cell.Label, (int)indexPath.Item);
            }
            else
            {
                var image = DataSource?.ImageFor(this, (int)indexPath.Item);
                if (image != null)
                {
                    cell.Label.Hidden = true;
                    cell.ImageView.Hidden = false;

                    cell.ImageView.Image = image;

                    var margin = Delegate?.WheelPickerMarginForItem(this, (int)indexPath.Item);
                    if (margin.HasValue)
                    {
                        cell.ImageView.Frame = cell.ImageView.Frame.Inset(-margin.Value.Width, -margin.Value.Height);
                    }

                    Delegate?.WheelPickerConfigureImageView(this, cell.ImageView, (int)indexPath.Item);
                }
            }

            cell.Label.Font = Font;
            cell.Selected = indexPath.Item == (nuint)selectedItem;
            return cell;
        }

        // MARK: - UICollectionViewDelegateFlowLayout

        [Export("collectionView:layout:sizeForItemAtIndexPath:")]
        public CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            var size = ScrollDirection == UICollectionViewScrollDirection.Horizontal
                ? new CGSize(InteritemSpacing, Bounds.Size.Height)
                : new CGSize(Bounds.Size.Width, InteritemSpacing);

            var title = DataSource?.TitleFor(this, (int)indexPath.Item);
            if (!string.IsNullOrEmpty(title))
            {
                var titleSize = SizeForString(title);
                if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
                {
                    size.Width += titleSize.Width;
                }
                else
                {
                    size.Height += titleSize.Height;
                }

                var marginSize = Delegate?.WheelPickerMarginForItem(this, (int)indexPath.Item);
                if (marginSize.HasValue)
                {
                    size.Width += marginSize.Value.Width * 2;
                    size.Height += marginSize.Value.Height * 2;
                }
            }
            else
            {
                var image = DataSource?.ImageFor(this, (int)indexPath.Item);
                if (image != null)
                {
                    if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
                    {
                        size.Width += image.Size.Width;
                    }
                    else
                    {
                        size.Height += image.Size.Height;
                    }
                }
            }

            return size;
        }

        [Export("collectionView:layout:insetForSectionAtIndex:")]
        public UIEdgeInsets GetInsetForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
        {
            var number = DataSource?.NumberOfItems(this);
            if (number > 0)
            {
                var firstIndexPath = NSIndexPath.FromItemSection(0, 0);
                var firstSize = CollectionViewSizeForItem(collectionView, collectionView.CollectionViewLayout, firstIndexPath);
                var lastIndexPath = NSIndexPath.FromItemSection((nuint)(number.Value - 1), 0);
                var lastSize = CollectionViewSizeForItem(collectionView, collectionView.CollectionViewLayout, lastIndexPath);

                if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
                {
                    return new UIEdgeInsets(
                        0.0f,
                        (collectionView.Bounds.Size.Width - firstSize.Width) / 2,
                        0.0f,
                        (collectionView.Bounds.Size.Width - lastSize.Width) / 2
                    );
                }
                else
                {
                    return new UIEdgeInsets(
                        (collectionView.Bounds.Size.Height - firstSize.Height) / 2,
                        0.0f,
                        (collectionView.Bounds.Size.Height - lastSize.Height) / 2,
                        0.0f
                    );
                }
            }

            return UIEdgeInsets.Zero;
        }

        [Export("collectionView:layout:minimumInteritemSpacingForSectionAtIndex:")]
        public nfloat GetMinimumInteritemSpacingForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
        {
            return 0.0f;
        }

        [Export("collectionView:layout:minimumLineSpacingForSectionAtIndex:")]
        public nfloat GetMinimumLineSpacingForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
        {
            return 0.0f;
        }

        // MARK: - UICollectionViewDelegate

        [Export("collectionView:didSelectItemAtIndexPath:")]
        public void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            Select((int)indexPath.Item, true);
        }

        // MARK: - UIScrollViewDelegate

        [Export("scrollViewDidEndDecelerating:")]
        public void DecelerationEnded(UIScrollView scrollView)
        {
            if (!scrollView.Tracking)
            {
                DidEndScrolling();
            }
        }

        [Export("scrollViewDidEndDragging:willDecelerate:")]
        public void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
        {
            if (!willDecelerate)
            {
                DidEndScrolling();
            }
        }

        [Export("scrollViewDidScroll:")]
        public void Scrolled(UIScrollView scrollView)
        {
            var cells = collectionView.VisibleCells;
            foreach (var cell in cells)
            {
                cell.Selected = false;
            }

            if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
            {
                var x = collectionView.ContentOffset.X + collectionView.Bounds.Width / 2;
                var point = new CGPoint(x, collectionView.ContentOffset.Y);
                var indexPath = collectionView.IndexPathForItemAtPoint(point);
                if (indexPath != null)
                {
                    var cell = collectionView.CellForItem(indexPath);
                    if (cell != null)
                    {
                        cell.Selected = true;
                        if ((int)indexPath.Row != prevIndex)
                        {
                            GenerateFeedback();
                            prevIndex = (int)indexPath.Row;
                        }
                    }
                }
            }
            else
            {
                var y = collectionView.ContentOffset.Y + collectionView.Bounds.Height / 2;
                var point = new CGPoint(collectionView.ContentOffset.X, y);
                var indexPath = collectionView.IndexPathForItemAtPoint(point);
                if (indexPath != null)
                {
                    var cell = collectionView.CellForItem(indexPath);
                    if (cell != null)
                    {
                        cell.Selected = true;
                        if ((int)indexPath.Row != prevIndex)
                        {
                            GenerateFeedback();
                            prevIndex = (int)indexPath.Row;
                        }
                    }
                }
            }
        }

        // MARK: - WheelPickerLayoutDelegate

        public WheelPickerStyle PickerViewStyle(WheelPickerCollectionViewLayout layout)
        {
            return Style;
        }
    }
}


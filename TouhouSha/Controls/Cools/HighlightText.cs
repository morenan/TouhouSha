using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace TouhouSha.Controls
{
    public class HighlightText : Control
    {
        public HighlightText()
        {
            CreateFormattedText();
        }

        #region Properties

        #region Text

        static public readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(HighlightText),
            new PropertyMetadata(String.Empty));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion

        #region HighlightForeground

        static public readonly DependencyProperty HighlightForegroundProperty = DependencyProperty.Register(
            "HighlightForeground", typeof(Brush), typeof(HighlightText),
            new PropertyMetadata(Brushes.White));

        public Brush HighlightForeground
        {
            get { return (Brush)GetValue(HighlightForegroundProperty); }
            set { SetValue(HighlightForegroundProperty, value); }
        }


        #endregion

        #region HighlightThickness

        static public readonly DependencyProperty HighlightThicknessProperty = DependencyProperty.Register(
            "HighlightThickness", typeof(double), typeof(HighlightText),
            new PropertyMetadata(1.0d));

        public double HighlightThickness
        {
            get { return (double)GetValue(HighlightThicknessProperty); }
            set { SetValue(HighlightThicknessProperty, value); }
        }


        #endregion

        #region Orientation

        static public readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(Orientation), typeof(HighlightText),
            new PropertyMetadata(Orientation.Horizontal));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        #endregion

        #region MaxTextWidth

        static public readonly DependencyProperty MaxTextWidthProperty = DependencyProperty.Register(
            "MaxTextWidth", typeof(double), typeof(HighlightText),
            new PropertyMetadata(10000.0d));

        public double MaxTextWidth
        {
            get { return (double)GetValue(MaxTextWidthProperty); }
            set { SetValue(MaxTextWidthProperty, value); }
        }

        #endregion 

        #endregion

        #region Numbers

        private FormattedText fmttext;
        private Geometry geo_textbody;
        private List<FormattedText> fmttext_v;
        private List<Geometry> geo_textbody_v;

        #endregion

        #region Methods

        protected void CreateFormattedText()
        {
            if (Orientation == Orientation.Horizontal)
            {
                this.fmttext = new FormattedText(
                    Text, 
                    System.Threading.Thread.CurrentThread.CurrentUICulture, 
                    FlowDirection, 
                    new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), 
                    FontSize, 
                    Foreground);
                fmttext.MaxTextWidth = MaxTextWidth;
                fmttext.MaxLineCount = 256;
                this.geo_textbody = fmttext.BuildGeometry(new Point(0, 0));
            }
            else
            {
                Point p0 = new Point(0, 0);
                this.fmttext_v = (Text ?? String.Empty)
                    .Cast<char>()
                    .Select(c => new FormattedText(
                        c.ToString(), 
                        System.Threading.Thread.CurrentThread.CurrentUICulture, 
                        FlowDirection,
                        new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), 
                        FontSize, 
                        Foreground))
                    .ToList();
                this.geo_textbody_v = fmttext_v
                    .Select(ft =>
                    {
                        Geometry geo = ft.BuildGeometry(p0);
                        p0.Y += geo.Bounds.Height;
                        p0.Y += 2;
                        return geo;
                    })
                    .ToList();
            }
            if (IsVisible)
            {
                InvalidateMeasure();
                InvalidateArrange();
                InvalidateVisual();
            }
        }

        protected override Size MeasureOverride(Size sz)
        {
            if (Orientation == Orientation.Horizontal && geo_textbody != null)
                return geo_textbody.Bounds.Size;
            if (Orientation == Orientation.Vertical && geo_textbody_v != null)
            {
                if (geo_textbody_v.Count() == 0) return new Size(4, 4);
                return new Size(
                    geo_textbody_v.Select(geo => geo.Bounds.Width).Max(),
                    geo_textbody_v.Select(geo => geo.Bounds.Height + 2).Sum());
            }
            return base.MeasureOverride(sz);
        }

        protected override Size ArrangeOverride(Size sz)
        {
            return base.ArrangeOverride(sz);
        }
        
        #endregion

        #region Event Handler

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == TextProperty
             || e.Property == FontSizeProperty
             || e.Property == FontFamilyProperty
             || e.Property == FontWeightProperty
             || e.Property == FontStretchProperty
             || e.Property == FontStyleProperty
             || e.Property == ForegroundProperty
             || e.Property == FlowDirectionProperty
             || e.Property == OrientationProperty
             || e.Property == MaxTextWidthProperty)
                CreateFormattedText();
            if (e.Property == HighlightForegroundProperty
             || e.Property == HighlightThicknessProperty)
                InvalidateVisual();
        }

        protected override void OnRender(DrawingContext ctx)
        {
            base.OnRender(ctx);
            if (Orientation == Orientation.Horizontal && geo_textbody != null)
                ctx.DrawGeometry(Foreground, new Pen(HighlightForeground, HighlightThickness), geo_textbody);
            if (Orientation == Orientation.Vertical && geo_textbody_v != null)
                geo_textbody_v.ForEach(geo => ctx.DrawGeometry(Foreground, new Pen(HighlightForeground, HighlightThickness), geo));
        }

        #endregion
    }
}

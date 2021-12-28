using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace AvaloniaFractalTrees
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            Content = new CanvasPanel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        class CanvasPanel : Panel
        {
            protected override void OnPointerMoved(PointerEventArgs e)
            {
                base.OnPointerMoved(e);

                mLastMousePoint = e.GetPosition(this);

                InvalidateVisual();
            }

            protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
            {
                base.OnPointerWheelChanged(e);

                if (e.Delta.Y > 0)
                {
                    mLeftBranchFactor -= 0.005;
                    mRightBranchFactor += 0.005;
                }
                else
                {
                    mLeftBranchFactor += 0.005;
                    mRightBranchFactor -= 0.005;
                }

                InvalidateVisual();
            }

            public override void Render(DrawingContext context)
            {
                base.Render(context);

                context.FillRectangle(Brushes.Black, new Rect(0, 0, Bounds.Width, Bounds.Height));

                using(context.PushPreTransform(Matrix.CreateTranslation(Bounds.Width / 2, Bounds.Height)))
                {
                    DrawBranch(
                        context,
                        mLastMousePoint.Y.Map(0, Bounds.Height, 100, 300),
                        mLastMousePoint.X.Map(0, Bounds.Width, Math.PI/32, Math.PI / 4));
                }
            }

            void DrawBranch(DrawingContext context, double len, double angle)
            {
                context.DrawLine(new Pen(Brushes.White, 1), new Point(0, 0), new Point(0, -len));

                if (len < 4)
                    return;

                using(context.PushPreTransform(Matrix.CreateTranslation(0, -len)))
                {
                    using (context.PushPreTransform(Matrix.CreateRotation(angle)))
                    {
                        DrawBranch(context, len * mLeftBranchFactor, angle);
                    }

                    using (context.PushPreTransform(Matrix.CreateRotation(-angle)))
                    {
                        DrawBranch(context, len * mRightBranchFactor, angle);
                    }
                }
            }

            Point mLastMousePoint;
            double mLeftBranchFactor = 0.67;
            double mRightBranchFactor = 0.67;
        }
    }

    static class MethodExtensions
    {
        public static double Map(this double value, double fromSource, double toSource, double fromTarget, double toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }
    }
}
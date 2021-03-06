﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using AForge.Wpf.LanguageLocalization;
using ContourAnalysisNS;
using Emgu.CV;
using Brushes = System.Windows.Media.Brushes;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace AForge.Wpf
{
    /// <summary>
    /// Interaction logic for SampleSelection.xaml
    /// </summary>
    public partial class SampleSelection
    {
        private List<Contour<System.Drawing.Point>> _contours;
        private Templates _samples;
        private double _strokeThickness;
        public SampleSelection(ImageSource imageSource, List<Contour<System.Drawing.Point>> contours,double strokeThickness)
        {
            InitializeComponent();
            SelectionImage.Source = imageSource;
            _contours = contours;
            DrawContours(Contours);
        }
        public List<Contour<System.Drawing.Point>> Contours => _contours;
        public Templates Samples => _samples;

        private void DrawContours(List<Contour<System.Drawing.Point>> contours)
        {
            if (contours ==null)
            {
                return;
            }
            if (contours.Count == 0) return;
            LineCanvas.Children.Clear();

            foreach (var contour in contours)
            {
                if (contour.Total < 2) continue;
                var contourArray = contour.ToArray();
                foreach (var point in contourArray)
                {
                    var positionBuffer = _strokeThickness * 0.08;
                    Line line = new Line
                    {
                        StrokeThickness = _strokeThickness,
                        Stroke = Brushes.Red,
                        X1 = point.X - positionBuffer,
                        X2 = point.X + positionBuffer,
                        Y1 = point.Y - positionBuffer,
                        Y2 = point.Y + positionBuffer
                    };
                    LineCanvas.Children.Add(line);
                    
                }
            }
        }

        private bool _mouseDown; // Set to 'true' when mouse is held down.
        private System.Windows.Point _mouseDownPos; // The point where the mouse button was clicked down.

        public SampleSelection(ImageSource image, Templates samples, List<Contour<System.Drawing.Point>> contours, double strokeThickness)
        {
            InitializeComponent();
            SelectionImage.Source = image;
            _samples = samples;
            _contours = contours;
            _strokeThickness = strokeThickness;
            DrawContours(contours);
            SampleWindow.Title = ResLocalization.SampleSelection + " | " + ResLocalization.SampleCount + " :" + _samples.Count;
        }

        #region MouseCapture

        void DrawSampleContours()
        {
            LineCanvas.Children.Clear();
            for (var index = 0; index < _samples.Count; index++)
            {
                var sample = _samples[index];
                var startPoint = sample.startPoint;
                sample.name = index.ToString();
                for (int i = 0; i < sample.contour.Count; i++)
                {
                    //sample.contour[i] *= 3;
                   
                    var point = new Point((float) (sample.contour[i].a + startPoint.X),
                        (float) (sample.contour[i].b + startPoint.Y));
                    var line = new Line
                    {
                        StrokeThickness = 3,
                        Stroke = Brushes.Red,
                        X1 = point.X - 0.1,
                        X2 = point.X + 0.1,
                        Y1 = point.Y - 0.1,
                        Y2 = point.Y + 0.1,
                        Name = "Line"
                    };
                    LineCanvas.Children.Add(line);
                }
            }
        }
        private void SelectionCanvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mouseDown = true;
            _mouseDownPos = e.GetPosition(SelectionCanvas);
            SelectionCanvas.CaptureMouse();

            // Initial placement of the drag selection box.         
            Canvas.SetLeft(SelectionRectangle, _mouseDownPos.X);
            Canvas.SetTop(SelectionRectangle, _mouseDownPos.Y);
            SelectionRectangle.Width = 0;
            SelectionRectangle.Height = 0;

            // Make the drag selection box visible.
            SelectionRectangle.Visibility = Visibility.Visible;
        }

        private void SelectionCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_mouseDown) return;
            // When the mouse is held down, reposition the drag selection box.

            var mousePos = e.GetPosition(SelectionCanvas);

            if (_mouseDownPos.X < mousePos.X)
            {
                Canvas.SetLeft(SelectionRectangle, _mouseDownPos.X);
                SelectionRectangle.Width = mousePos.X - _mouseDownPos.X;
            }
            else
            {
                Canvas.SetLeft(SelectionRectangle, mousePos.X);
                SelectionRectangle.Width = _mouseDownPos.X - mousePos.X;
            }

            if (_mouseDownPos.Y < mousePos.Y)
            {
                Canvas.SetTop(SelectionRectangle, _mouseDownPos.Y);
                SelectionRectangle.Height = mousePos.Y - _mouseDownPos.Y;
            }
            else
            {
                Canvas.SetTop(SelectionRectangle, mousePos.Y);
                SelectionRectangle.Height = _mouseDownPos.Y - mousePos.Y;
            }
        }

        private void SelectionCanvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _mouseDown = false;
            SelectionCanvas.ReleaseMouseCapture();



            var position = SelectionRectangle.TranslatePoint(new System.Windows.Point(0, 0), SelectionCanvas);
            var top = position.Y;
            var left = position.X;
            var bottom = top + SelectionRectangle.ActualHeight;
            var right = left + SelectionRectangle.ActualWidth;
            RemoveContoursSelected(top, left, bottom, right);
            
            DrawContours(_contours);

            SelectionRectangle.Visibility = Visibility.Collapsed;
        }

    #endregion
    private void RemoveContoursSelected(double top, double left, double bottom, double right)
    {
        if (_contours == null) return;
            var removingContours = new List<Contour<System.Drawing.Point>>(_contours.Count);
            var removeSamples = new List<Template>();
            for (var q = 0; q < _contours.Count; q++)
            {
                var contourArray = _contours[q].ToArray();
                foreach (var point in contourArray)
                {
                    if (point.X > left && point.X < right && point.Y < bottom && point.Y > top)
                    {
                        removeSamples.Add(_samples[q]);
                        removingContours.Add(_contours[q]);
                        break;
                    }
                }
            }
            
            for (var index = 0; index < removingContours.Count; index++)
            {
                _samples.Remove(removeSamples[index]);
                _contours.Remove(removingContours[index]);
            }
            SampleWindow.Title = ResLocalization.SampleSelection+" | "+ResLocalization.SampleCount+" :" + _samples.Count;
        }
    }
}

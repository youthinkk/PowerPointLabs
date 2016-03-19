﻿using System;
using System.Windows;
using System.Windows.Input;
using PowerPointLabs.ActionFramework.Common.Extension;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace PowerPointLabs.ResizeLab
{
    /// <summary>
    /// Interaction logic for ResizePane.xaml
    /// </summary>
    public partial class ResizeLabPaneWPF : IResizeLabPane
    {
        private ResizeLabMain _resizeLab;
        public static bool IsAspectRatioLocked { get; set; }
        
        private const string UnlockAspectRatioToolTip = "Unlocks the aspect ratio of objects when performing resizing of objects";
        private const string LockAspectRatioToolTip = "Locks the aspect ratio of objects when performing resizing of objects";

        // Dialog windows
        private StretchSettingsDialog _stretchSettingsDialog;
        private SameDimensionSettingsDialog _sameDimensionSettingsDialog;

        public ResizeLabPaneWPF()
        {
            InitializeComponent();
            InitialiseLogicInstance();
            UnlockAspectRatio();
        }

        internal void InitialiseLogicInstance()
        {
            if (_resizeLab == null)
            {
                _resizeLab = new ResizeLabMain(this);
            }
        }

        internal void InitialiseAspectRatio()
        {
            UnlockAspectRatio();
        }

        #region Execute Stretch and Shrink

        private void StretchLeftBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedShape = GetSelectedShapes();
            Action<PowerPoint.ShapeRange> resizeAction = shapes => _resizeLab.StretchLeft(shapes);

            ModifySelectionAspectRatio();
            ExecuteResizeAction(selectedShape, resizeAction);
        }

        private void StretchRightBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedShapes = GetSelectedShapes();
            Action<PowerPoint.ShapeRange> resizeAction = shapes => _resizeLab.StretchRight(shapes);

            ModifySelectionAspectRatio();
            ExecuteResizeAction(selectedShapes, resizeAction);
        }

        private void StretchTopBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedShapes = GetSelectedShapes();
            Action<PowerPoint.ShapeRange> resizeAction = shapes => _resizeLab.StretchTop(shapes);

            ModifySelectionAspectRatio();
            ExecuteResizeAction(selectedShapes, resizeAction);
        }

        private void StretchBottomBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedShapes = GetSelectedShapes();
            Action<PowerPoint.ShapeRange> resizeAction = shapes => _resizeLab.StretchBottom(shapes);

            ModifySelectionAspectRatio();
            ExecuteResizeAction(selectedShapes, resizeAction);
        }

        private void StretchSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_stretchSettingsDialog == null || !_stretchSettingsDialog.IsOpen)
            {
                _stretchSettingsDialog = new StretchSettingsDialog(_resizeLab);
                _stretchSettingsDialog.Show();
            }
            else
            {
                _stretchSettingsDialog.Activate();
            }
            
        }

        #endregion

        #region Execute Same Dimension

        private void SameWidthBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedShapes = GetSelectedShapes();
            Action<PowerPoint.ShapeRange> resizeAction = shapes => _resizeLab.ResizeToSameWidth(shapes);

            ModifySelectionAspectRatio();
            ExecuteResizeAction(selectedShapes, resizeAction);
        }

        private void SameHeightBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedShapes = GetSelectedShapes();
            Action<PowerPoint.ShapeRange> resizeAction = shapes => _resizeLab.ResizeToSameHeight(shapes);

            ModifySelectionAspectRatio();
            ExecuteResizeAction(selectedShapes, resizeAction);
        }

        private void SameSizeBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedShapes = GetSelectedShapes();
            Action<PowerPoint.ShapeRange> resizeAction = shapes => _resizeLab.ResizeToSameHeightAndWidth(shapes);

            ModifySelectionAspectRatio();
            ExecuteResizeAction(selectedShapes, resizeAction);
        }

        private void SameDimensionSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_sameDimensionSettingsDialog == null || !_sameDimensionSettingsDialog.IsOpen)
            {
                _sameDimensionSettingsDialog = new SameDimensionSettingsDialog(_resizeLab);
                _sameDimensionSettingsDialog.Show();
            }
            else
            {
                _sameDimensionSettingsDialog.Activate();
            }
        }

        #endregion

        #region Execute Fit
        private void FitWidthBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedShapes = GetSelectedShapes();
            var slideWidth = this.GetCurrentPresentation().SlideWidth;
            var slideHeight = this.GetCurrentPresentation().SlideHeight;
            Action<PowerPoint.ShapeRange, float, float, bool> resizeAction =
                (shapes, referenceWidth, referenceHeight, isAspectRatio) =>
                {
                    _resizeLab.FitToWidth(shapes, referenceWidth, referenceHeight, isAspectRatio);
                };

            ModifySelectionAspectRatio();
            ExecuteResizeAction(selectedShapes, slideWidth, slideHeight, resizeAction);
        }

        private void FitHeightBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedShapes = GetSelectedShapes();
            var slideWidth = this.GetCurrentPresentation().SlideWidth;
            var slideHeight = this.GetCurrentPresentation().SlideHeight;
            Action<PowerPoint.ShapeRange, float, float, bool> resizeAction =
                (shapes, referenceWidth, referenceHeight, isAspectRatio) =>
                {
                    _resizeLab.FitToHeight(shapes, referenceWidth, referenceHeight, isAspectRatio);
                };

            ModifySelectionAspectRatio();
            ExecuteResizeAction(selectedShapes, slideWidth, slideHeight, resizeAction);
        }

        private void FillBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedShapes = GetSelectedShapes();
            var slideWidth = this.GetCurrentPresentation().SlideWidth;
            var slideHeight = this.GetCurrentPresentation().SlideHeight;
            Action<PowerPoint.ShapeRange, float, float, bool> resizeAction =
                (shapes, referenceWidth, referenceHeight, isAspectRatio) =>
                {
                    _resizeLab.FitToFill(shapes, referenceWidth, referenceHeight, isAspectRatio);
                };

            ModifySelectionAspectRatio();
            ExecuteResizeAction(selectedShapes, slideWidth, slideHeight, resizeAction);
        }

        #endregion

        #region Execute Slight Adjust
        private void IncreaseHeightBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedShape = GetSelectedShapes();
            Action<PowerPoint.ShapeRange> resizeAction = shapes => _resizeLab.IncreaseHeight(shapes);

            ModifySelectionAspectRatio();
            ExecuteResizeAction(selectedShape, resizeAction);
        }

        private void DecreaseHeightBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedShape = GetSelectedShapes();
            Action<PowerPoint.ShapeRange> resizeAction = shapes => _resizeLab.DecreaseHeight(shapes);

            ModifySelectionAspectRatio();
            ExecuteResizeAction(selectedShape, resizeAction);
        }

        private void IncreaseWidthBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedShape = GetSelectedShapes();
            Action<PowerPoint.ShapeRange> resizeAction = shapes => _resizeLab.IncreaseWidth(shapes);

            ModifySelectionAspectRatio();
            ExecuteResizeAction(selectedShape, resizeAction);
        }

        private void DecreaseWidthBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedShape = GetSelectedShapes();
            Action<PowerPoint.ShapeRange> resizeAction = shapes => _resizeLab.DecreaseWidth(shapes);

            ModifySelectionAspectRatio();
            ExecuteResizeAction(selectedShape, resizeAction);
        }

        #endregion

        #region Execute Aspect Ratio

        private void LockAspectRatio_UnChecked(object sender, RoutedEventArgs e)
        {
            UnlockAspectRatio();
        }

        private void LockAspectRatio_Checked(object sender, RoutedEventArgs e)
        {
            LockAspectRatio();
        }

        private void RestoreAspectRatioBtn_Click(object sender, RoutedEventArgs e)
        {
            PowerPoint.ShapeRange selectedShapes = GetSelectedShapes();
            var slideHeight = this.GetCurrentPresentation().SlideHeight;
            var slideWidth = this.GetCurrentPresentation().SlideWidth;

            if (selectedShapes != null)
            {
                _resizeLab.RestoreAspectRatio(selectedShapes, slideHeight, slideWidth);
            }
        }

        private void UnlockAspectRatio()
        {
            IsAspectRatioLocked = false;
            LockAspectRatioCheckBox.ToolTip = LockAspectRatioToolTip;

            ModifySelectionAspectRatio();
        }

        private void LockAspectRatio()
        {
            IsAspectRatioLocked = true;
            LockAspectRatioCheckBox.ToolTip = UnlockAspectRatioToolTip;

            ModifySelectionAspectRatio();
        }

        private void ModifySelectionAspectRatio()
        {
            if (_resizeLab.IsSelecionValid(GetSelection(), false))
            {
                _resizeLab.ChangeShapesAspectRatio(GetSelectedShapes(), IsAspectRatioLocked);
            }
        }

        #endregion

        #region Preview Stretch and Shrink

        private void StretchLeftBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedShapes = GetSelectedShapes(false);
            Action<PowerPoint.ShapeRange> previewAction = shapes => _resizeLab.StretchLeft(shapes);

            ModifySelectionAspectRatio();
            Preview(selectedShapes, previewAction, 2);
        }

        private void StretchRightBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedShapes = GetSelectedShapes(false);
            Action<PowerPoint.ShapeRange> previewAction = shapes => _resizeLab.StretchRight(shapes);

            ModifySelectionAspectRatio();
            Preview(selectedShapes, previewAction, 2);
        }

        private void StretchTopBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedShapes = GetSelectedShapes(false);
            Action<PowerPoint.ShapeRange> previewAction = shapes => _resizeLab.StretchTop(shapes);

            ModifySelectionAspectRatio();
            Preview(selectedShapes, previewAction, 2);
        }

        private void StretchBottomBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedShapes = GetSelectedShapes(false);
            Action<PowerPoint.ShapeRange> previewAction = shapes => _resizeLab.StretchBottom(shapes);

            ModifySelectionAspectRatio();
            Preview(selectedShapes, previewAction, 2);
        }

        #endregion

        #region Preview Same Dimension

        private void SameWidthBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedShapes = GetSelectedShapes(false);
            Action<PowerPoint.ShapeRange> previewAction = shapes => _resizeLab.ResizeToSameWidth(shapes);

            ModifySelectionAspectRatio();
            Preview(selectedShapes, previewAction, 2);
        }
        
        private void SameHeightBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedShapes = GetSelectedShapes(false);
            Action<PowerPoint.ShapeRange> previewAction = shapes => _resizeLab.ResizeToSameHeight(shapes);

            ModifySelectionAspectRatio();
            Preview(selectedShapes, previewAction, 2);
        }

        private void SameSizeBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedShapes = GetSelectedShapes(false);
            Action<PowerPoint.ShapeRange> previewAction = shapes => _resizeLab.ResizeToSameHeightAndWidth(shapes);

            Preview(selectedShapes, previewAction, 2);
        }

        #endregion

        #region Preview Fit

        private void FitWidthBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedShapes = GetSelectedShapes(false);
            var slideWidth = this.GetCurrentPresentation().SlideWidth;
            var slideHeight = this.GetCurrentPresentation().SlideHeight;
            Action<PowerPoint.ShapeRange, float, float, bool> previewAction =
                (shapes, referenceWidth, referenceHeight, isAspectRatio) =>
                {
                    _resizeLab.FitToWidth(shapes, referenceWidth, referenceHeight, isAspectRatio);
                };

            ModifySelectionAspectRatio();
            Preview(selectedShapes, slideWidth, slideHeight, previewAction);
        }

        private void FitHeightBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedShapes = GetSelectedShapes(false);
            var slideWidth = this.GetCurrentPresentation().SlideWidth;
            var slideHeight = this.GetCurrentPresentation().SlideHeight;
            Action<PowerPoint.ShapeRange, float, float, bool> previewAction =
                (shapes, referenceWidth, referenceHeight, isAspectRatio) =>
                {
                    _resizeLab.FitToHeight(shapes, referenceWidth, referenceHeight, isAspectRatio);
                };

            ModifySelectionAspectRatio();
            Preview(selectedShapes, slideWidth, slideHeight, previewAction);
        }

        private void FillBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedShapes = GetSelectedShapes(false);
            var slideWidth = this.GetCurrentPresentation().SlideWidth;
            var slideHeight = this.GetCurrentPresentation().SlideHeight;
            Action<PowerPoint.ShapeRange, float, float, bool> previewAction =
                (shapes, referenceWidth, referenceHeight, isAspectRatio) =>
                {
                    _resizeLab.FitToFill(shapes, referenceWidth, referenceHeight, isAspectRatio);
                };

            ModifySelectionAspectRatio();
            Preview(selectedShapes, slideWidth, slideHeight, previewAction);
        }

        #endregion

        #region Preview Slight Adjust
        private void IncreaseHeightBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedShapes = GetSelectedShapes(false);
            Action<PowerPoint.ShapeRange> previewAction = shapes => _resizeLab.IncreaseHeight(shapes);

            ModifySelectionAspectRatio();
            Preview(selectedShapes, previewAction, 1);
        }

        private void DecreaseHeightBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedShapes = GetSelectedShapes(false);
            Action<PowerPoint.ShapeRange> previewAction = shapes => _resizeLab.DecreaseHeight(shapes);

            ModifySelectionAspectRatio();
            Preview(selectedShapes, previewAction, 1);
        }

        private void IncreaseWidthBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedShapes = GetSelectedShapes(false);
            Action<PowerPoint.ShapeRange> previewAction = shapes => _resizeLab.IncreaseWidth(shapes);

            ModifySelectionAspectRatio();
            Preview(selectedShapes, previewAction, 1);
        }

        private void DecreaseWidthBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedShapes = GetSelectedShapes(false);
            Action<PowerPoint.ShapeRange> previewAction = shapes => _resizeLab.DecreaseWidth(shapes);

            ModifySelectionAspectRatio();
            Preview(selectedShapes, previewAction, 1);
        }

        #endregion

        #region Helper Functions

        private PowerPoint.ShapeRange GetSelectedShapes(bool handleError = true)
        {
            var selection = GetSelection();

            return _resizeLab.IsSelecionValid(selection, handleError) ? GetSelection().ShapeRange : null;
        }

        private PowerPoint.Selection GetSelection()
        {
            return this.GetCurrentSelection();
        }

        private void Btn_MouseLeave(object sender, MouseEventArgs e)
        {
            Reset();
        }

        #endregion
    }
}

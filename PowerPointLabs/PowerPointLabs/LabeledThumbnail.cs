﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PowerPointLabs.Views;

namespace PowerPointLabs
{
    public partial class LabeledThumbnail : UserControl
    {
        private bool _nameFinishHandled;
        private bool _isHighlighted;
        private bool _isGoodName;
        
        private string _nameLabel;

        public enum Status
        {
            Idle,
            Editing
        }

        # region Properties
        public string NameLable
        {
            get
            {
                return _nameLabel;
            }
            set
            {
                if (Verify(value))
                {
                    _nameLabel = value;
                    labelTextBox.Text = value;
                    _isGoodName = true;
                }
                else
                {
                    MessageBox.Show(TextCollection.LabeledThumbnailInvalidCharacterError);
                    labelTextBox.SelectAll();
                    _isGoodName = false;
                }
            }
        }

        public string ImagePath { get; set; }

        public Status State { get; private set; }
        # endregion

        # region Constructors
        public LabeledThumbnail()
        {
            Initialize();
        }

        public LabeledThumbnail(string imagePath, string nameLable)
        {
            Initialize(imagePath, nameLable);
        }
        # endregion

        # region API
        public void DeHighlight()
        {
            motherPanel.BackColor = Color.FromKnownColor(KnownColor.Window);
            thumbnailPanel.BackColor = Color.FromKnownColor(KnownColor.Transparent);
            labelTextBox.BackColor = Color.FromKnownColor(KnownColor.Window);
            labelTextBox.ForeColor = Color.Black;

            // dehighlight will hard-disable the text box editing
            labelTextBox.Enabled = false;
            State = Status.Idle;
        }

        public void Highlight()
        {
            motherPanel.BackColor = Color.FromKnownColor(KnownColor.Highlight);
            thumbnailPanel.BackColor = Color.FromKnownColor(KnownColor.Highlight);

            if (!labelTextBox.Enabled)
            {
                labelTextBox.BackColor = Color.FromKnownColor(KnownColor.Highlight);
                labelTextBox.ForeColor = Color.White;
            }
        }

        public void StartNameEdit()
        {
            // add the text box
            if (!motherPanel.Controls.Contains(labelTextBox))
            {
                motherPanel.Controls.Add(labelTextBox);
            }

            _nameFinishHandled = false;
            State = Status.Editing;
            
            Highlight();

            labelTextBox.Enabled = true;
            labelTextBox.Focus();
            labelTextBox.SelectAll();
        }

        public void FinishNameEdit()
        {
            // since messagebox will trigger LostFocus event, this method
            // has chance to be triggered mulitple times. To avoid this,
            // a flag will be set on the first time the function is called,
            // and skip the function by checking if the flag has been set.
            if (_nameFinishHandled) return;

            _nameFinishHandled = true;
            NameLable = labelTextBox.Text;

            var oldName = Path.GetFileNameWithoutExtension(ImagePath);

            if (_isGoodName &&
                !IsDuplicateName(oldName))
            {
                State = Status.Idle;

                labelTextBox.Enabled = false;
                NameEditFinish(this, oldName);
            }
            else
            {
                StartNameEdit();
            }
        }

        public void ToggleHighlight()
        {
            if (_isHighlighted)
            {
                DeHighlight();
            }
            else
            {
                Highlight();
            }

            _isHighlighted = !_isHighlighted;
        }
        # endregion

        # region Helper Functions
        // for names, we do not allow names involve '\' or '.'
        // Regex = [\\\.]
        private const string InvalidCharsRegex = "[\\\\\\.]";

        private double CalculateScalingRatio(Size oldSize, Size newSize)
        {
            double scalingRatio;

            if (oldSize.Width >= oldSize.Height)
            {
                scalingRatio = (double)newSize.Width / oldSize.Width;
            }
            else
            {
                scalingRatio = (double)newSize.Height / oldSize.Height;
            }

            return scalingRatio;
        }

        private Bitmap CreateThumbnailImage(Image oriImage, int width, int height)
        {
            var scalingRatio = CalculateScalingRatio(oriImage.Size, new Size(width, height));

            // calculate width and height after scaling
            var scaledWidth = (int)Math.Round(oriImage.Size.Width * scalingRatio);
            var scaledHeight = (int)Math.Round(oriImage.Size.Height * scalingRatio);

            // calculate left top corner position of the image in the thumbnail
            var scaledLeft = (width - scaledWidth) / 2;
            var scaledTop = (height - scaledHeight) / 2;

            // define drawing area
            var drawingRect = new Rectangle(scaledLeft, scaledTop, scaledWidth, scaledHeight);
            var thumbnail = new Bitmap(width, height);

            // here we set the thumbnail as the highest quality
            using (var thumbnailGraphics = Graphics.FromImage(thumbnail))
            {
                thumbnailGraphics.CompositingQuality = CompositingQuality.HighQuality;
                thumbnailGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                thumbnailGraphics.SmoothingMode = SmoothingMode.HighQuality;
                thumbnailGraphics.DrawImage(oriImage, drawingRect);
            }

            return thumbnail;
        }

        private void Initialize()
        {
            InitializeComponent();

            motherPanel.MouseDown += (sender, e) => Click(this, e);
            motherPanel.DoubleClick += (sender, e) => DoubleClick(this, e);

            thumbnailPanel.MouseDown += (sender, e) => Click(this, e);
            thumbnailPanel.DoubleClick += ThumbnailPanelDoubleClick;

            labelTextBox.DoubleClick += (sender, e) => labelTextBox.SelectAll();
            labelTextBox.EnabledChanged += EnableChangedHandler;
            labelTextBox.KeyPress += EnterKeyWhileEditing;
            labelTextBox.LostFocus += NameLableLostFocus;

            var customPaintTextBox = new CustomPaintTextBox(labelTextBox);
        }

        private void Initialize(string imagePath, string nameLable)
        {
            Initialize();

            NameLable = nameLable;
            ImagePath = imagePath;

            thumbnailPanel.BackgroundImage = CreateThumbnailImage(new Bitmap(ImagePath), 50, 50);
            
            // critical line, we need to free the reference to the image immediately after we've
            // finished thumbnail generation, else we could not modify (rename/ delete) the
            // image.
            GC.Collect();

            State = Status.Idle;
            labelTextBox.Enabled = false;
        }

        private bool IsDuplicateName(string oldName)
        {
            // if the name hasn't changed, we don't need to check for duplicate name
            // since the default name/ old name is confirmed unique.
            if (oldName == NameLable) return false;

            var newPath = ImagePath.Replace(oldName, NameLable);

            // if the new name has been used, the new name is not allowed
            if (File.Exists(newPath))
            {
                MessageBox.Show(TextCollection.LabeledThumbnailFileNameExistError);
                return true;
            }

            return false;
        }

        private bool Verify(string name)
        {
            var invalidChars = new Regex(InvalidCharsRegex);
            
            return !(string.IsNullOrWhiteSpace(name) || invalidChars.IsMatch(name));
        }
        # endregion

        # region Event Handlers
        public delegate void ClickEventDelegate(object sender, EventArgs e);
        public delegate void DoubleClickEventDelegate(object sender, EventArgs e);
        public delegate void NameEditFinishEventDelegate(object sender, string oldName);

        public new event ClickEventDelegate Click;
        public new event DoubleClickEventDelegate DoubleClick;
        public event NameEditFinishEventDelegate NameEditFinish;

        private void EnableChangedHandler(object sender, EventArgs e)
        {
            if (labelTextBox.Enabled == false)
            {
                labelTextBox.BackColor = Color.FromKnownColor(KnownColor.Window);
                labelTextBox.Text = string.Empty;
            }
            else
            {
                labelTextBox.BackColor = Color.FromKnownColor(KnownColor.Window);
                labelTextBox.ForeColor = Color.Black;
                labelTextBox.Text = NameLable;
            }
        }

        private void EnterKeyWhileEditing(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                FinishNameEdit();
                e.Handled = true;
            }
        }

        private void NameLableLostFocus(object sender, EventArgs args)
        {
            FinishNameEdit();
        }

        private void ThumbnailPanelDoubleClick(object sender, EventArgs e)
        {
            if (State == Status.Editing)
            {
                labelTextBox.SelectAll();
                return;
            }

            DoubleClick(this, e);
        }
        # endregion
    }
}
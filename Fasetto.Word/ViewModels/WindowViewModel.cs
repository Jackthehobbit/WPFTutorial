using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace Fasetto.Word.ViewModels
{
    /// <summary>
    /// View Model for the custom flat window
    /// </summary>
    public class WindowViewModel : BaseViewModel
    {

        #region Private Members
        /// <summary>
        /// The window this view model controls
        /// </summary>
        private Window mWindow;
        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        private int mOuterMarginSize = 10;
        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        private int mWindowRadius = 10;
        #endregion

        #region Public Properties
        /// <summary>
        /// The smallest width the window can go
        /// </summary>
        public double WindowMinWidth { get; set; } = 400;
        /// <summary>
        /// The smallest height the window can go
        /// </summary>
        public double WindowMinHeight { get; set; } = 400;

        /// <summary>
        /// The height of the caption bar section of the window
        /// </summary>
        public int TitleHeight { get; set; } = 42;

        /// <summary>
        /// The height of the caption bar section of the window
        /// </summary>
        public GridLength TitleHeightGridLength{ get { return new GridLength(TitleHeight + ResizeBorder); } }

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public int OuterMarginSize {
            get
            {
                return mWindow.WindowState == WindowState.Maximized ? 0 : mOuterMarginSize;
            }
            set
            {
                mOuterMarginSize = value;
            }
        }

        /// <summary>
        /// The Margin around the window to allow for a drop shadow
        /// </summary>
        public Thickness OuterMarginThickness { get { return new Thickness(OuterMarginSize); } }

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        public int WindowRadius
        {
            get
            {
                return mWindow.WindowState == WindowState.Maximized ? 0 : mWindowRadius;
            }
            set
            {
                mWindowRadius = value;
            }
        }

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        public CornerRadius WindowCornerRadius{ get { return new CornerRadius(WindowRadius); } }
        
        /// <summary>
        /// The size of the resize border around the window
        /// </summary>
        public int ResizeBorder { get; set; } = 6;

        /// <summary>
        /// The size of the resize border around the window
        /// </summary>
        public Thickness ResizeBorderThickness { get { return new Thickness(ResizeBorder+OuterMarginSize); } }

        /// <summary>
        /// The size of the resize border around the window
        /// </summary>
        public Thickness InnerContentPadding { get { return new Thickness(ResizeBorder); } }
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public WindowViewModel(Window window)
        {
            mWindow = window;

            //Listen out for the window resizing
            window.StateChanged += (sender, e) =>
            {
                //Fire of events for all properties that are effected by a resize
                OnPropertyChanged(nameof(ResizeBorderThickness));
                OnPropertyChanged(nameof(WindowRadius));
                OnPropertyChanged(nameof(WindowCornerRadius));
                OnPropertyChanged(nameof(OuterMarginSize));
                OnPropertyChanged(nameof(OuterMarginThickness));
            };

            //Initialse the commands
            MinimizeCommand = new RelayCommand(() => mWindow.WindowState = WindowState.Minimized);
            MaximizeCommand = new RelayCommand(() => mWindow.WindowState ^= WindowState.Maximized);
            CloseCommand = new RelayCommand(() => mWindow.Close());
            MenuCommand = new RelayCommand(() => SystemCommands.ShowSystemMenu(mWindow,GetMousePosition()));

            //Fix Window resize issue
            var resizer = new WindowResizer(mWindow);
        }

        #endregion
        
        #region Commands
        /// <summary>
        /// Command to Minimize a window
        /// </summary>
        public ICommand MinimizeCommand { get; set; }

        /// <summary>
        /// Command to Maximize a window
        /// </summary>
        public ICommand MaximizeCommand { get; set; }

        /// <summary>
        /// Command to Close a window
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// Command to run the system menu
        /// </summary>
        public ICommand MenuCommand { get; set; }

        #endregion

        #region Private Helpers
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        /// <summary>
        /// Gets the current mouse position on screen
        /// </summary>
        /// <returns></returns>
        private Point GetMousePosition()
        {
            var position = Mouse.GetPosition(mWindow);

            return new Point(position.X + mWindow.Left, position.Y + mWindow.Top);
        }
        #endregion
    }
}

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GameModel;
using GuiHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WpfGameOfLife.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        private IGameOfLifeModel _gameModel;

        public int X { get; set; }
        public int Y { get; set; }
        public int Gen => _gameModel.Generation;
        public RelayCommand InitCmd { get; set; }
        public ICommand StartStopCmd { get; set; }
        public RelayCommand StepCmd { get; set; }
        public BitmapSource BoardImage { get; set; }

        private DispatcherTimer _dpTimer;
        private readonly Timer _timer;

        public GameViewModel()
        {
            _gameModel = SimpleIoc.Default.GetInstanceWithoutCaching<IGameOfLifeModel>();
            _gameModel.Init(400, 400);
            UpdateBoard();

            StepCmd = new RelayCommand(() => {
                _gameModel.SingleStep();
                UpdateBoard();
            });

            _dpTimer = new DispatcherTimer();
            _dpTimer.Tick += _dpTimer_Tick;

            _timer = new Timer(_timer_callback);

            StartStopCmd = new RelayCommand(() =>
            {
                if (_dpTimer.IsEnabled)
                {
                    _dpTimer.Stop();
                    _timer.Change(0, 0);
                }
                else
                {
                    _dpTimer.Interval = TimeSpan.FromMilliseconds(40);
                    _dpTimer.Start();
                    _timer.Change(0, 10);
                }
                
            });
        }

        private void _timer_callback(object state)
        {
            _gameModel.SingleStep();
        }

        private void _dpTimer_Tick(object sender, EventArgs e)
        {
            UpdateBoard();
            RaisePropertyChanged(nameof(Gen));
        }

        private void UpdateBoard()
        {
            var bitmap = MatrixHelper.ConvertToBitmap(_gameModel.Board, Color.Black, Color.White);

            BoardImage = ToBitmapSource(bitmap);
            RaisePropertyChanged(nameof(BoardImage));
        }

        public static BitmapSource ToBitmapSource(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }
    }
}

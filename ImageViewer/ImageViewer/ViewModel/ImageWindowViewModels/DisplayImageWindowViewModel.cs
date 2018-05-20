using ImageViewer.Methods;
using ImageViewer.Model.Event;
using ImageViewer.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ImageViewer.Model;
using ImageViewer.View.ImagesWindow;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Prism.Events;
using System.Diagnostics;
using System.IO;

namespace ImageViewer.ViewModel.ImageWindowViewModels
{
    public class DisplayImageWindowViewModel : BaseViewModel, IDisposable
    {

        #region currentViewModels
        List<ImagePresenterViewModel> _presentersList = new List<ImagePresenterViewModel>();
        private Dictionary<Type, SubscriptionToken> _subscriptionTokens = new Dictionary<Type, SubscriptionToken>();
        private bool disposedValue = false;
        private ImagePresenterViewModel _currentViewModel1;
        private ImagePresenterViewModel _currentViewModel2;
        private ImagePresenterViewModel _currentViewModel3;
        private ImagePresenterViewModel _currentViewModel4;
        private ImagePresenterViewModel _currentViewModel5;
        private ImagePresenterViewModel _currentViewModel6;
        private ImagePresenterViewModel _currentViewModel7;
        private ImagePresenterViewModel _currentViewModel8;
        private ImagePresenterViewModel _currentViewModel9;

        public ImagePresenterViewModel CurrentViewModel1
        {
            get { return _currentViewModel1; }
            set
            {
                if (value == null && _currentViewModel1 != null)
                    _currentViewModel1.Dispose();
                _currentViewModel1 = value;
                _presentersList[0] = value;
                NotifyPropertyChanged();
            }
        }
        public ImagePresenterViewModel CurrentViewModel2
        {
            get { return _currentViewModel2; }
            set
            {
                if (value == null && _currentViewModel2 != null)
                    _currentViewModel2.Dispose();
                _currentViewModel2 = value;
                _presentersList[1] = value;
                NotifyPropertyChanged();
            }
        }
        public ImagePresenterViewModel CurrentViewModel3
        {
            get { return _currentViewModel3; }
            set
            {
                if (value == null && _currentViewModel3 != null)
                    _currentViewModel3.Dispose();
                _currentViewModel3 = value;
                _presentersList[2] = value;
                NotifyPropertyChanged();
            }
        }
        public ImagePresenterViewModel CurrentViewModel4
        {
            get { return _currentViewModel4; }
            set
            {
                if (value == null && _currentViewModel4 != null)
                    _currentViewModel4.Dispose();
                _currentViewModel4 = value;
                _presentersList[3] = value;
                NotifyPropertyChanged();
            }
        }
        public ImagePresenterViewModel CurrentViewModel5
        {
            get { return _currentViewModel5; }
            set
            {
                if (value == null && _currentViewModel5 != null)
                    _currentViewModel5.Dispose();
                _currentViewModel5 = value;
                _presentersList[4] = value;
                NotifyPropertyChanged();
            }
        }
        public ImagePresenterViewModel CurrentViewModel6
        {
            get { return _currentViewModel6; }
            set
            {
                if (value == null && _currentViewModel6 != null)
                    _currentViewModel6.Dispose();
                _currentViewModel6 = value;
                _presentersList[5] = value;
                NotifyPropertyChanged();
            }
        }
        public ImagePresenterViewModel CurrentViewModel7
        {
            get { return _currentViewModel7; }
            set
            {
                if (value == null && _currentViewModel7 != null)
                    _currentViewModel7.Dispose();
                _currentViewModel7 = value;
                _presentersList[6] = value;
                NotifyPropertyChanged();
            }
        }
        public ImagePresenterViewModel CurrentViewModel8
        {
            get { return _currentViewModel8; }
            set
            {
                if (value == null && _currentViewModel8 != null)
                    _currentViewModel8.Dispose();
                _currentViewModel8 = value;
                _presentersList[7] = value;
                NotifyPropertyChanged();
            }
        }
        public ImagePresenterViewModel CurrentViewModel9
        {
            get { return _currentViewModel9; }
            set
            {
                if (value == null && _currentViewModel9 != null)
                    _currentViewModel9.Dispose();
                _currentViewModel9 = value;
                _presentersList[8] = value;
                NotifyPropertyChanged();
            }
        }

        #endregion
        private static int _imageCounter = 0;
        private int _maxWindows = 0;
        private List<PixelInformationView> _pivList = new List<PixelInformationView>();
        public RelayCommand ClosePIVsCommand { get; set; }
        public RelayCommand ShowToolbarCommand { get; set; }
        public RelayCommand DesynchronizationCommand { get; set; }
        private GridStatusEvent.GridStatus _gridStatus;
        public static Tools Tool = Tools.None;
        public GridStatusEvent.GridStatus GridStatus
        {
            get { return _gridStatus; }
            set
            {
                _gridStatus = value;
                NotifyPropertyChanged();
                if (_gridStatus == GridStatusEvent.GridStatus.OneToOne)
                {
                    CurrentViewModel2 = null;
                    CurrentViewModel3 = null;
                    CurrentViewModel4 = null;
                    ClearImagePresenter();
                    if (_imageCounter > 1)
                        _imageCounter = 1;

                }
                if (_gridStatus == GridStatusEvent.GridStatus.OneToTwo)
                {
                    CurrentViewModel3 = null;
                    CurrentViewModel4 = null;
                    ClearImagePresenter();
                    if (_imageCounter > 2)
                        _imageCounter = 2;

                }
                if (_gridStatus == GridStatusEvent.GridStatus.TwoToTwo)
                {
                    ClearImagePresenter();
                    if (_imageCounter > 4)
                        _imageCounter = 4;
                }
            }
        }
        public static int ImageCounter
        {
            get
            {
                return _imageCounter;
            }
        }
        private Visibility toolBarVisibility;
        public Visibility ToolbarVisibility
        {
            get
            {
                return toolBarVisibility;
            }
            set
            {
                toolBarVisibility = value;
                NotifyPropertyChanged();
            }
        }
        private ObservableCollection<Image> _imageList;

        public DisplayImageWindowViewModel()
        {
            _presentersList.Add(CurrentViewModel1);
            _presentersList.Add(CurrentViewModel2);
            _presentersList.Add(CurrentViewModel3);
            _presentersList.Add(CurrentViewModel4);
            _presentersList.Add(CurrentViewModel5);
            _presentersList.Add(CurrentViewModel6);
            _presentersList.Add(CurrentViewModel7);
            _presentersList.Add(CurrentViewModel8);
            _presentersList.Add(CurrentViewModel9);

            GridStatus = GridStatusEvent.GridStatus.OneToOne;
            ShowToolbarCommand = new RelayCommand(ShowToolbar);
            ClosePIVsCommand = new RelayCommand(ClosePIVs);
            DesynchronizationCommand = new RelayCommand(Desynchronize);
            SubscriptionToken subscriptionToken;
            subscriptionToken = _aggregator.GetEvent<HideToolbarEvent>().Subscribe(HideToolbar);
            _subscriptionTokens.Add(typeof(HideToolbarEvent), subscriptionToken);
            subscriptionToken = _aggregator.GetEvent<DisposeEvent>().Subscribe(Dispose);
            _subscriptionTokens.Add(typeof(DisposeEvent), subscriptionToken);
            subscriptionToken = _aggregator.GetEvent<SendPixelInformationViewEvent>().Subscribe(item => { _pivList.Add(item); });
            _subscriptionTokens.Add(typeof(SendPixelInformationViewEvent), subscriptionToken);
            subscriptionToken = _aggregator.GetEvent<DisplayImage>().Subscribe(item =>
            {
                foreach (var presenter in _presentersList)
                {
                    if (presenter == null)
                        continue;
                    if (presenter.ImageList == item)
                    {
                        MessageBox.Show($"This list is already loaded to the window with ID: {presenter.PresenterID}", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }
                _imageList = item;
                _imageCounter++;
                CreateMultiView(_imageList);
            });
            _subscriptionTokens.Add(typeof(DisplayImage), subscriptionToken);

            subscriptionToken = _aggregator.GetEvent<GridStatusEvent>().Subscribe((item) =>
            {
                GridStatus = item;
            });
            _subscriptionTokens.Add(typeof(GridStatusEvent), subscriptionToken);
        }

        private void Desynchronize(Object arg)
        {
            int id = Int16.Parse((string)arg);
            _aggregator.GetEvent<SynchronizationEvent>().Publish(id);
        }
        private void CreateMultiView(ObservableCollection<Image> _imageList)
        {
            switch (GridStatus)
            {
                case GridStatusEvent.GridStatus.OneToOne:
                    _maxWindows = 1;
                    GetImagePresentersFor1x1();
                    break;
                case GridStatusEvent.GridStatus.OneToTwo:
                    _maxWindows = 2;
                    GetImagePresentersFor1x2();
                    break;
                case GridStatusEvent.GridStatus.TwoToTwo:
                    _maxWindows = 4;
                    GetImagePresentersFor2x2();
                    break;
                case GridStatusEvent.GridStatus.ThreeToThree:
                    _maxWindows = 9;
                    GetImagePresentersFor3x3();
                    break;
            }
        }

        private void GetImagePresentersFor3x3()
        {
            switch (_imageCounter)
            {
                case 1:
                    CurrentViewModel1 = new ImagePresenterViewModel(_imageList, 1, Tool, _maxWindows);
                    break;
                case 2:
                    CurrentViewModel2 = new ImagePresenterViewModel(_imageList, 2, Tool, _maxWindows);
                    break;
                case 3:
                    CurrentViewModel3 = new ImagePresenterViewModel(_imageList, 3, Tool, _maxWindows);
                    break;
                case 4:
                    CurrentViewModel4 = new ImagePresenterViewModel(_imageList, 4, Tool, _maxWindows);
                    break;
                case 5:
                    CurrentViewModel5 = new ImagePresenterViewModel(_imageList, 5, Tool, _maxWindows);
                    break;
                case 6:
                    CurrentViewModel6 = new ImagePresenterViewModel(_imageList, 6, Tool, _maxWindows);
                    break;
                case 7:
                    CurrentViewModel7 = new ImagePresenterViewModel(_imageList, 7, Tool, _maxWindows);
                    break;
                case 8:
                    CurrentViewModel8 = new ImagePresenterViewModel(_imageList, 8, Tool, _maxWindows);
                    break;
                case 9:
                    CurrentViewModel9 = new ImagePresenterViewModel(_imageList, 9, Tool, _maxWindows);
                    break;
                default:
                    _imageCounter = 1;
                    GetImagePresentersFor3x3();
                    break;
            }
        }

        private void GetImagePresentersFor2x2()
        {
            switch (_imageCounter)
            {
                case 1:
                    CurrentViewModel1 = new ImagePresenterViewModel(_imageList, 1, Tool, _maxWindows);
                    break;
                case 2:
                    CurrentViewModel2 = new ImagePresenterViewModel(_imageList, 2, Tool, _maxWindows);
                    break;
                case 3:
                    CurrentViewModel3 = new ImagePresenterViewModel(_imageList, 3, Tool, _maxWindows);
                    break;
                case 4:
                    CurrentViewModel4 = new ImagePresenterViewModel(_imageList, 4, Tool, _maxWindows);
                    break;
                default:
                    _imageCounter = 1;
                    GetImagePresentersFor2x2();
                    break;
            }
        }

        private void GetImagePresentersFor1x2()
        {
            switch (_imageCounter)
            {
                case 1:
                    CurrentViewModel1 = new ImagePresenterViewModel(_imageList, 1, Tool, _maxWindows);
                    break;
                case 2:
                    CurrentViewModel2 = new ImagePresenterViewModel(_imageList, 2, Tool, _maxWindows);
                    break;
                default:
                    _imageCounter = 1;
                    GetImagePresentersFor1x2();
                    break;
            }
        }

        private void GetImagePresentersFor1x1()
        {
            CurrentViewModel1 = new ImagePresenterViewModel(_imageList, 1, Tool, _maxWindows);
            _imageCounter = 1;
        }

        private void ClearImagePresenterList(int startingIndex)
        {

        }
        public void ClosePIVs(object obj)
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                foreach (var item in _pivList)
                {
                    item.Close();
                }
            }));
        }

        private void HideToolbar()
        {
            ToolbarVisibility = Visibility.Collapsed;
        }
        private void ShowToolbar(object obj)
        {
            ToolbarVisibility = Visibility.Visible;
        }

        private void ClearImagePresenter()
        {
            CurrentViewModel5 = null;
            CurrentViewModel6 = null;
            CurrentViewModel7 = null;
            CurrentViewModel8 = null;
            CurrentViewModel9 = null;
        }

        #region IDisposable Support
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    handle.Dispose();
                }
                disposedValue = true;
            }
        }

        ~DisplayImageWindowViewModel()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            _currentViewModel1 = null;
            _currentViewModel2 = null;
            _currentViewModel3 = null;
            _currentViewModel4 = null;
            _currentViewModel5 = null;
            _currentViewModel6 = null;
            _currentViewModel7 = null;
            _currentViewModel8 = null;
            _currentViewModel9 = null;

            _aggregator.GetEvent<HideToolbarEvent>().Unsubscribe(_subscriptionTokens[typeof(HideToolbarEvent)]);
            _subscriptionTokens.Remove(typeof(HideToolbarEvent));
            _aggregator.GetEvent<DisposeEvent>().Unsubscribe(_subscriptionTokens[typeof(DisposeEvent)]);
            _subscriptionTokens.Remove(typeof(DisposeEvent));
            _aggregator.GetEvent<SendPixelInformationViewEvent>().Unsubscribe(_subscriptionTokens[typeof(SendPixelInformationViewEvent)]);
            _subscriptionTokens.Remove(typeof(SendPixelInformationViewEvent));
            _aggregator.GetEvent<DisplayImage>().Unsubscribe(_subscriptionTokens[typeof(DisplayImage)]);
            _subscriptionTokens.Remove(typeof(DisplayImage));
            _aggregator.GetEvent<GridStatusEvent>().Unsubscribe(_subscriptionTokens[typeof(GridStatusEvent)]);
            _subscriptionTokens.Remove(typeof(GridStatusEvent));

            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
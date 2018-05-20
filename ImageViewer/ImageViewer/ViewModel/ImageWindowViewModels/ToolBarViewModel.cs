using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageViewer.Methods;
using ImageViewer.Model.Event;
using ImageViewer.Model;
using ImageViewer.View;
using System.Windows.Media;

namespace ImageViewer.ViewModel.ImageWindowViewModels
{
    public class ToolBarViewModel : BaseViewModel
    {
        private Tools _tool = Tools.None;
        public RelayCommand HideToolBarCommand { get; set; }
        public RelayCommand CreateMagnifyingGlassToolCommand { get; set; }
        public RelayCommand CreateRegionToolCommand { get; set; }
        public RelayCommand PanImageToolCommand { get; set; }
        public RelayCommand CreatePixelPickerToolCommand { get; set; }
        public RelayCommand GridOneCommand { get; set; }
        public RelayCommand GridOneToTwoCommand { get; set; }
        public RelayCommand GridTwoToTwoCommand { get; set; }
        public RelayCommand GridThreeToTreeCommand { get; set; }
        public RelayCommand SerializeOutputFromPresenters { get; set; }
        public RelayCommand CreateRotateImageToolCommand { get; set; }

        public Tools Tool
        {
            get
            {
                return _tool;
            }
            set
            {
                if (_tool == value)
                    _tool = Tools.None;
                else
                    _tool = value;

                switch (_tool)
                {
                    case Tools.RegionSelection:
                        _aggregator.GetEvent<SendToolEvent>().Publish(new CreateRegion());
                        break;
                    case Tools.PixelInformations:
                        _aggregator.GetEvent<SendToolEvent>().Publish(new PixelPicker());
                        break;
                    case Tools.ImagePan:
                        _aggregator.GetEvent<SendToolEvent>().Publish(new PanImage());
                        break;
                    case Tools.Rotate:
                        _aggregator.GetEvent<SendToolEvent>().Publish(new Rotate());
                        break;
                    default:
                        _aggregator.GetEvent<SendToolEvent>().Publish(null);
                        break;
                }
                DisplayImageWindowViewModel.Tool = Tool;
                NotifyPropertyChanged();
            }
        }
        private GridStatusEvent.GridStatus _gridStatus;
        public GridStatusEvent.GridStatus GridStatus
        {
            get { return _gridStatus; }
            set
            {
                _gridStatus = value;
                NotifyPropertyChanged();
            }
        }
        public ToolBarViewModel()
        {
            HideToolBarCommand = new RelayCommand(HideToolBarExecute);
            PanImageToolCommand = new RelayCommand(PanImageTool);
            CreateRegionToolCommand = new RelayCommand(CreateRegionTool);
            CreatePixelPickerToolCommand = new RelayCommand(CreatePixelPickerTool);
            GridOneCommand = new RelayCommand(GridOneExecute);
            GridOneToTwoCommand = new RelayCommand(GridOneToTwoExecute);
            GridTwoToTwoCommand = new RelayCommand(GridTwoToTwoExecute);
            GridThreeToTreeCommand = new RelayCommand(GridThreeToTreeExecute);
            SerializeOutputFromPresenters = new RelayCommand(SerializeOutput);
            GridStatus = GridStatusEvent.GridStatus.OneToOne;
            CreateRotateImageToolCommand = new RelayCommand(CreateRotateImageTool);

        }

        private void SerializeOutput(Object obj)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.Cancel || result == System.Windows.Forms.DialogResult.None)
                    return;
                string path = dialog.SelectedPath;
                _aggregator.GetEvent<SerializeOutputEvent>().Publish(path);
            }
        }

        private void GridOneExecute(object obj)
        {
            GridStatus = GridStatusEvent.GridStatus.OneToOne;
            _aggregator.GetEvent<GridStatusEvent>().Publish(GridStatusEvent.GridStatus.OneToOne);
        }

        private void GridOneToTwoExecute(object obj)
        {
            GridStatus = GridStatusEvent.GridStatus.OneToTwo;
            _aggregator.GetEvent<GridStatusEvent>().Publish(GridStatusEvent.GridStatus.OneToTwo);
        }

        private void GridTwoToTwoExecute(object obj)
        {
            GridStatus = GridStatusEvent.GridStatus.TwoToTwo;
            _aggregator.GetEvent<GridStatusEvent>().Publish(GridStatusEvent.GridStatus.TwoToTwo);
        }

        private void GridThreeToTreeExecute(object obj)
        {
            GridStatus = GridStatusEvent.GridStatus.ThreeToThree;
            _aggregator.GetEvent<GridStatusEvent>().Publish(GridStatusEvent.GridStatus.ThreeToThree);
        }

        private void HideToolBarExecute(object obj)
        {
            _aggregator.GetEvent<HideToolbarEvent>().Publish();
        }

        private void PanImageTool(object obj)
        {
            Tool = Tools.ImagePan;
        }

        private void CreateRotateImageTool(object obj)
        {
            Tool = Tools.Rotate;
        }


        private void CreateRegionTool(object obj)
        {
            Tool = Tools.RegionSelection;
        }

        private void CreatePixelPickerTool(object obj)
        {
            Tool = Tools.PixelInformations;
        }
    }
}

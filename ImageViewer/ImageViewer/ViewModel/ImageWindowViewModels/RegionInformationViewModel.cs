using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageViewer.Model.Event;

namespace ImageViewer.ViewModel.ImageWindowViewModels
{
    class RegionInformationViewModel : BaseViewModel
    {
        string[] _minValues = new string[9];
        string[]_maxValues = new string[9];
        string[] _averageValues = new string[9];
        string[] _regionSize = new string[9];
        string[] _variances = new string[9];
        string[]_deviations = new string[9];
        int _id = 0;
        
        public int PresenterID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                NotifyPropertyChanged("Deviations");
                NotifyPropertyChanged("Variances");
                NotifyPropertyChanged("RegionSize");
                NotifyPropertyChanged("MinValues");
                NotifyPropertyChanged("MaxValues");
                NotifyPropertyChanged("AverageValues");
            }
        }
        public string Deviations
        {
            get
            {
                return _deviations[_id];
            }
            set
            {
                _deviations[_id] = value;
                NotifyPropertyChanged();
            }
        }
        public string Variances
        {
            get
            {
                return _variances[_id];
            }
            set
            {
                _variances[_id] = value;
                NotifyPropertyChanged();
            }
        }

        public string RegionSize
        {
            get
            {
                return _regionSize[_id];
            }
            set
            {
                _regionSize[_id] = value;
                NotifyPropertyChanged();
            }
        }

        public string MinValues
        {
            get
            {
                return _minValues[_id];
            }
            set
            {
                _minValues[_id] = value;
                NotifyPropertyChanged();
            }
        }
        public string MaxValues
        {
            get
            {
                return _maxValues[_id];
            }
            set
            {
                _maxValues[_id] = value;
                NotifyPropertyChanged();
            }
        }
        public string AverageValues
        {
            get
            {
                return _averageValues[_id];
            }
            set
            {
                _averageValues[_id] = value;
                NotifyPropertyChanged();
            }
        }
        public RegionInformationViewModel()
        {
            _aggregator.GetEvent<SendRegionInformationEvent>().Subscribe(SetPixelInformation);
            _aggregator.GetEvent<SendPresenterIDEvent>().Subscribe((id) => { PresenterID = id-1; });
        }

        private void SetPixelInformation(Dictionary<string,Object> parameters)
        {
            try
            {
                _id = (int)parameters["PresenterID"];
                _id--;
                double[] averages = (double[])parameters["Averages"];
                double[] variances = (double[])parameters["Variances"];
                double[] deviations = (double[])parameters["Deviations"];
                byte[] mins = (byte[])parameters["Mins"];
                byte[] maxs = (byte[])parameters["Maxs"];
                int regionWidth = (int)parameters["Width"];
                int regionHeight = (int)parameters["Height"];
                AverageValues = $"R:  {Math.Round(averages[0], 2)}\nG:  {Math.Round(averages[1], 2)}\nB:  {Math.Round(averages[2], 2)}\nA:  {Math.Round(averages[3], 2)}";
                MinValues = $"R:  {mins[0]}\nG:  {mins[1]}\nB:  {mins[2]}\nA:  {mins[3]}";
                MaxValues = $"R:  {maxs[0]}\nG:  {maxs[1]}\nB:  {maxs[2]}\nA:  {maxs[3]}";
                RegionSize = $"Region size:\t{regionWidth} x {regionHeight}";
                Variances = $"R:  {Math.Round(variances[0], 2)}\nG:  {Math.Round(variances[1], 2)}\nB:  {Math.Round(variances[2], 2)}\nA:  {Math.Round(variances[3], 2)}";
                Deviations = $"R:  {Math.Round(deviations[0], 2)}\nG:  {Math.Round(deviations[1], 2)}\nB:  {Math.Round(deviations[2], 2)}\nA:  {Math.Round(deviations[3], 2)}";
            }
            catch (KeyNotFoundException)
            {

                return;
            }
        }
    }
}

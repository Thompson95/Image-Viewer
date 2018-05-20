using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ImageViewer.Model
{
    public interface ITool
    {
        void AffectImage(Dictionary<String, Object> args);
        Tools GetToolEnum();
    }
}

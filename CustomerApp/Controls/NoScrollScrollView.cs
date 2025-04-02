using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApp.Controls
{
    public class NoScrollScrollView : ScrollView
    {
        public NoScrollScrollView(): base()
        {
            var recognizer = new PanGestureRecognizer();
            recognizer.PanUpdated += OnPanUpdated;
        }
        private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Running)
            {
                
            }
        }
    }
}

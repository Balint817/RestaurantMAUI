using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApp.ViewModel
{
    public class RegisterPageVM
    {
        public Command ToggleFlyoutCommand => AppShell.ToggleFlyoutCommand;
        public RegisterPageVM()
        {
            
        }
    }
}

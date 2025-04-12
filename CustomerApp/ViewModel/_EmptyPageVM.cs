using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerApp.Services;

namespace CustomerApp.ViewModel
{
    public partial class EmptyPageVM: BindableObject
    {
        public LanguageService LanguageService => LanguageService.Instance;
        public Command ToggleFlyoutCommand => AppShell.ToggleFlyoutCommand;
        public EmptyPageVM()
        {

        }
    }
}

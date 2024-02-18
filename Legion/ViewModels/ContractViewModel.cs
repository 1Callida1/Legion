using Legion.Models;
using Legion.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.ViewModels
{
    public class ContractViewModel : ViewModelBase
    {
        private ApplicationDbContext _context;

        public ContractViewModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public override IScreen HostScreen => throw new NotImplementedException();
    }
}

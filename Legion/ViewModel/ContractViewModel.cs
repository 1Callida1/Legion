using Legion.Models;
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
    internal class ContractViewModel : ReactiveObject
    {
        private ApplicationDbContext _context;

        public ContractViewModel(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}

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
    public class InvestorsViewModel : ReactiveObject
    {
        private ApplicationDbContext _context;

        public InvestorsViewModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public ObservableCollection<Investor> Investors => new ObservableCollection<Investor>(new List<Investor>() {
            new Investor() { LastName = "Abobov", MiddleName = "Abobovich"},
            new Investor() { LastName = "Abobov", MiddleName = "Abobovich"},
            new Investor() { LastName = "Abobov", MiddleName = "Abobovich"},
            new Investor() { LastName = "Abobov", MiddleName = "Abobovich"},
            new Investor() { LastName = "Abobov", MiddleName = "Abobovich"},
        });
    }
}

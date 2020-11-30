using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boardgames.Wpf.Client.ViewModels
{
    public interface IAsyncLoad
    {
        Task LoadDataAsync();
    }
}

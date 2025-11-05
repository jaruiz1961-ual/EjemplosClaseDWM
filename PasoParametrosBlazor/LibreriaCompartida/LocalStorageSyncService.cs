using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaCompartida
{
    // MyApp.Shared/LocalStorageSyncService.cs
    public class LocalStorageSyncService
    {
        public event Action? OnValorChanged;

        public void NotificarCambio() => OnValorChanged?.Invoke();
    }

}

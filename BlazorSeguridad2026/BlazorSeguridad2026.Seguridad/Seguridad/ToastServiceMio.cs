using BootstrapBlazor.Components;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlazorSeguridad2026.Base.Seguridad
{
  
public class ToastServiceMio
    {
        public event Action<string, string>? OnShow;

        public void ShowSuccess(string message) =>
            OnShow?.Invoke("success", message);

        public void ShowError(string message) =>
            OnShow?.Invoke("danger", message);
    }


}

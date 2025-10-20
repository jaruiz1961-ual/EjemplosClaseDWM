// La función debe estar en el objeto 'window' o ser accesible globalmente
window.mostrarAlerta = (mensaje) => {
    alert(mensaje);
};

window.sumar = (a, b) => {
    return a + b;
};

window.limitaCaractares = (campo, maximoCaracteres) => {
    
    var cadenaOriginal = document.getElementById(campo);
    let valorCadenaModificada = cadenaOriginal.value.substring(0, cadenaOriginal.value.length - 1);
    if (cadenaOriginal.value.length >= maximoCaracteres) {
        cadenaOriginal.value = valorCadenaModificada;
        return false;
    }
    else {
        return true;
    }
}

window.InteropSetters =  {

    dotNetReference: null,

        // Función llamada por C# (en OnInitialized) para guardar la referencia
        setDotNetReference: function(dotNetRef) {
            this.dotNetReference = dotNetRef;
            console.log("[JS] Referencia de .NET establecida.");
        },

    limitaCaractares:function (campo, maximoCaracteres) {

        var cadenaOriginal = document.getElementById(campo);
        let valorCadenaModificada = cadenaOriginal.value.substring(0, cadenaOriginal.value.length - 1);
        if (cadenaOriginal.value.length >= maximoCaracteres) {

            if (this.dotNetReference) {

                // CORRECTO para Blazor Server: Usar invokeMethodAsync
                // Esto devuelve una Promesa y es la forma correcta de llamar a C# asíncronamente
                this.dotNetReference.invokeMethodAsync('EstablecerVariable', true)
                    .then(() => {
                        console.log("[JS] Llamada a C# asíncrona enviada con éxito.");
                    })
                    .catch(error => {
                        console.error("[JS] Error al invocar el método C#:", error);
                    });

            } else {
                console.error("[JS] La referencia de .NET no está disponible.");
            }
            cadenaOriginal.value = valorCadenaModificada;
            
            return false;
        }
        else {
            return true;
        }
    }
}

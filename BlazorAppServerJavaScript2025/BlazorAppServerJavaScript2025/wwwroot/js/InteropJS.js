// La función debe estar en el objeto 'window' o ser accesible globalmente
window.mostrarAlerta = (mensaje) => {
    alert(mensaje);
};

window.suma = (a, b) => {
    return a + b;
};

window.suma2 = (n1, n2) => {
    const a1 = document.getElementById(n1);
    const a2 = document.getElementById(n2);
    const numero1 = parseInt(a1?.value ?? '0', 10);
    const numero2 = parseInt(a2?.value ?? '0', 10);
    return suma(numero1, numero2);
};

window.limitaCaractares = (campo, maximoCaracteres) => {
    const el = document.getElementById(campo);

    const raw = el.value ?? '';
    const maxNum = Number(maximoCaracteres);


    // Normalizar: eliminar retornos de carro (\r) si vienen como CRLF desde textarea
    const valor = raw.replace(/\r/g, '');

    const longitud = valor.length;

    if (longitud >= maxNum)
    {
        el.value = valor.substring(0, valor.length - 1);
        return true;
    }
    else
    {
        return false;
    }
}

window.quedanCaractares = (campo, maximoCaracteres) => {
    const el = document.getElementById(campo);

    const raw = el.value ?? '';
    const maxNum = Number(maximoCaracteres);

    // Normalizar: eliminar retornos de carro (\r) si vienen como CRLF desde textarea
    const valor = raw.replace(/\r/g, '');

    const longitud = valor.length;
    if (longitud == maxNum) {
        el.style.backgroundColor = "red";
    }
    else {
        el.style.backgroundColor = "white";
    }
    if (longitud > maxNum) {
        el.value = valor.substring(0, valor.length - 1);
        return 0;
    }
    else return maxNum - longitud;
}

window.duplicaTexto = (entrada, salida) => {
    const valor = document.getElementById(entrada)?.value ?? '';
    const elemento = document.getElementById(salida);
    if (!elemento) return;
    elemento.innerHTML = ' ' + valor;
}

window.ampliaTexto = (entrada, salida, altura) => {
    const valor = document.getElementById(entrada)?.value ?? '';
    const valorAltura = document.getElementById(altura)?.value ?? '';
    const elemento = document.getElementById(salida);
    if (!elemento) return;
    elemento.style.fontSize = valorAltura + "px";
    elemento.style.color = "red";
    elemento.innerHTML = ' ' + valor;
}

window.InteropSetters =  {

    dotNetReference: null,

    // Función llamada por C# (en OnInitialized) para guardar la referencia
    setDotNetReference: function(dotNetRef) {
        this.dotNetReference = dotNetRef;
        console.log("[JS] Referencia de .NET establecida.");
    },

    limitaCaractares: function (campo, maximoCaracteres) {
        const el = document.getElementById(campo);
        if (!el) return true;

        const raw = el.value ?? '';
        const maxNum = Number(maximoCaracteres);
        const valor = raw.replace(/\r/g, '');

        if (isNaN(maxNum)) {
            console.warn('[InteropSetters.limitaCaractares] maximoCaracteres no es numérico:', maximoCaracteres);
        }

        if (valor.length >= maxNum) {
            if (this.dotNetReference) {
                this.dotNetReference.invokeMethodAsync('EstablecerVariable', true)
                    .catch(error => {
                        console.error("[JS] Error al invocar el método C#:", error);
                    });
            } else {
                console.error("[JS] La referencia de .NET no está disponible.");
            }

            el.value = valor.substring(0, valor.length - 1);
            return false;
        } else {
            return true;
        }
    }
}



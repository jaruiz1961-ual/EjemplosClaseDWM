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

window.validaPassword = (campo) => {
    // Escribe una función en JavaScript llamada validarContrasena que tome una contraseña como entrada y determine si es válida según ciertos criterios. La contraseña se considera válida si cumple con las siguientes condiciones:

    // Tiene al menos 8 caracteres de longitud.
    // Contiene al menos una letra minúscula y una letra mayúscula.
    // Contiene al menos un número.
    // Contiene al menos un carácter especial de los siguientes: !@#$%^&*()_+[]{}|;:',.<>?.
    // La función debe devolver true si la contraseña es válida y false en caso contrario.
    const entrada = document.getElementById(campo);
    const cadena = entrada?.value ?? '';
    const verificarLongitud = (cadena) => {
        return cadena.length >= 8;
    }

    const verificarNumero = (cadena) => {
        const numeros = [1, 2, 3, 4, 5, 6, 7, 8, 9, 0];
        return numeros.some((item) => cadena.includes(item))
    }

    const verificaCaracterEspecial = (cadena) => {
        const caracteres = ['!', '@', '#', '$', '%', ' ', '^', '&', '*', '(', ')', '_', '+', '[', ']', '{', '}', '|', ';', ':', "'", ',', '.', '<', '>', '?'];
        return caracteres.some((item) => cadena.includes(item))
    }

    const veriticarLower = (cadena) => { // 97 122
        return [...cadena].map((item) => item.codePointAt(0))
            .some((item) => item >= 97 && item <= 122)
    }

    const veriticarUpper = (cadena) => {
        return [...cadena].map((item) => item.codePointAt(0))
            .some((item) => item >= 65 && item <= 90)
    }


    const validarContrasena = (cadena) => {
            return verificarLongitud(cadena)
            && verificarNumero(cadena)
            && verificaCaracterEspecial(cadena)
            && veriticarLower(cadena)
            && veriticarUpper(cadena)
    }
    var resultado = validarContrasena(cadena);
   
    if (resultado) {   
   
            entrada.style.backgroundColor = "white";
        }
        else {
            entrada.style.backgroundColor = "red";
        }

    return resultado;

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



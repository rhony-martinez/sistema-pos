// Valida inputs numéricos con mensaje pequeño debajo (estilo productos)
function aplicarValidacionSoloNumeros(input, {
    maxDigits = 10,
    allowDecimal = false,  // en tu caso: false (solo enteros)
    mensaje = `Solo se permiten números (máx ${maxDigits} dígitos).`
} = {}) {
    if (!input) return;
    if (input.dataset.validNumericaActiva === "true") return;
    input.dataset.validNumericaActiva = "true";

    // Crear / reutilizar <small class="error-msg">
    let errorSpan =
        input.parentElement?.querySelector(`small.error-msg[data-for="${input.id}"]`) ||
        document.querySelector(`small.error-msg[data-for="${input.id}"]`);

    if (!errorSpan) {
        errorSpan = document.createElement("small");
        errorSpan.classList.add("error-msg");
        errorSpan.dataset.for = input.id;
        errorSpan.style.color = "red";
        errorSpan.style.display = "none";
        errorSpan.style.fontSize = "0.8rem";
        errorSpan.style.marginTop = "2px";
        input.insertAdjacentElement("afterend", errorSpan);
    }

    const showError = (txt) => {
        errorSpan.textContent = txt;
        errorSpan.style.display = "block";
        input.classList.add("input-error");
    };

    const hideError = () => {
        errorSpan.textContent = "";
        errorSpan.style.display = "none";
        input.classList.remove("input-error");
    };

    const sanitize = () => {
        let v = String(input.value ?? "");

        // quitar espacios al inicio
        if (/^\s/.test(v)) v = v.trimStart();

        // permitir decimal o no
        if (allowDecimal) {
            v = v.replace(/[^0-9.]/g, "");
            const parts = v.split(".");
            if (parts.length > 2) v = parts[0] + "." + parts.slice(1).join("");
        } else {
            v = v.replace(/\D/g, "");
        }

        // max dígitos (sin contar el punto si existiera)
        const onlyDigits = v.replace(/\D/g, "");
        if (onlyDigits.length > maxDigits) {
            // recorta manteniendo solo dígitos
            const recortado = onlyDigits.slice(0, maxDigits);
            v = allowDecimal ? recortado : recortado;
            showError(`Máximo ${maxDigits} números permitidos.`);
        } else {
            // si se cambió por caracteres inválidos, muestra el mensaje
            // (pero si quedó vacío no muestres error)
            hideError();
        }

        input.value = v;

        // si intentó meter letras o símbolos en el evento actual
        // (cuando el usuario pega, etc.) => mensaje
        const original = String(input.value ?? "");
        const invalidTyped = /[^\d.]/.test(original) && allowDecimal;
        if (!allowDecimal && /[^\d]/.test(original)) {
            if (input.value) showError(mensaje);
        } else if (invalidTyped) {
            if (input.value) showError(mensaje);
        }

        if (!input.value) hideError();
    };

    // bloquea teclas no válidas (sin romper backspace/flechas)
    input.addEventListener("keydown", (e) => {
        const allowedNav = ["Backspace", "Delete", "Tab", "Enter", "ArrowLeft", "ArrowRight", "Home", "End"];
        if (allowedNav.includes(e.key)) return;

        if (/^\d$/.test(e.key)) return;
        if (allowDecimal && e.key === "." && !input.value.includes(".")) return;

        e.preventDefault();
        showError(mensaje);
    });

    input.addEventListener("input", sanitize);
    input.addEventListener("blur", () => { if (!input.value) hideError(); });

    sanitize();
}

document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("form-sede");
    const modal = document.getElementById("modal");
    const modalText = document.getElementById("modal-text");
    const modalBtn = document.getElementById("modal-btn");
    const cancelBtn = document.getElementById("cancelBtn");

    console.log("✅ Script crear-sedes.js cargado y ejecutado");

    // === Mostrar modal elegante ===
    function showModal(message, callback) {
        modalText.textContent = message;
        modal.style.display = "flex";
        modalBtn.onclick = () => {
            modal.style.display = "none";
            if (callback) callback();
        };
    }

    // === Botón cancelar ===
    cancelBtn.addEventListener("click", () => {
        window.location.href = "gestion-sedes.html";
    });

    // === Validadores básicos ===
    function esEmailValido(email) {
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
    }

    function esTelefonoValido(tel) {
        // Permite + solo al inicio, dígitos, espacios y guiones
        // y exige al menos 7 dígitos reales
        return /^\+?\d[\d\s-]{6,}$/.test(tel);
    }

    function marcarError(input, mensaje) {
        input.classList.add("input-error");
        const feedback = input.parentElement.querySelector(".feedback-inline");
        if (feedback) feedback.textContent = mensaje;
    }

    function limpiarErrores() {
        form.querySelectorAll(".input-error").forEach(el => el.classList.remove("input-error"));
        form.querySelectorAll(".feedback-inline").forEach(f => f.textContent = "");
    }

    // === Envío del formulario ===
    form.addEventListener("submit", async (e) => {
        e.preventDefault();
        limpiarErrores();

        // Extraer datos de los inputs
        const data = {};
        form.querySelectorAll("[data-field]").forEach(input => {
            const key = input.getAttribute("data-field");
            data[key] = input.value.trim();
        });

        // === Validaciones ===
        let valido = true;

        // 1️⃣ Campos obligatorios
        for (const [campo, valor] of Object.entries(data)) {
            const input = form.querySelector(`[data-field="${campo}"]`);
            if (!valor) {
                marcarError(input, "Campo obligatorio");
                valido = false;
            }
        }

        // 2️⃣ Longitud mínima y máxima
        for (const [campo, valor] of Object.entries(data)) {
            const input = form.querySelector(`[data-field="${campo}"]`);
            if (valor && (valor.length < 2 || valor.length > 50)) {
                marcarError(input, "Debe tener entre 2 y 50 caracteres");
                valido = false;
            }
        }

        // 3️⃣ Validación de teléfono
        const telInput = form.querySelector('[data-field="telefono"]');
        if (telInput && data.telefono && !esTelefonoValido(data.telefono)) {
            marcarError(telInput, "Número de teléfono inválido");
            valido = false;
        }

        // 4️⃣ Validación de correo electrónico
        const correoInput = form.querySelector('[data-field="correo"]');
        if (correoInput && data.correo && !esEmailValido(data.correo)) {
            marcarError(correoInput, "Correo electrónico inválido");
            valido = false;
        }

        // Si hay errores → mostrar advertencia
        if (!valido) {
            showModal("⚠️ Revisa los campos resaltados antes de continuar.", () => {
                const primerError = form.querySelector(".input-error");
                if (primerError) primerError.focus();
            });
            return;
        }

        // === Normalización de datos ===
        const payload = {
            nombre: data.nombre.trim().replace(/\s+/g, " "),
            direccion: data.direccion.trim().replace(/\s+/g, " "),
            ciudad: data.ciudad.trim().replace(/\s+/g, " "),
            departamento: data.departamento.trim().replace(/\s+/g, " "),
            ubicacion: data.ubicacion.trim().replace(/\s+/g, " "),
            telefono: data.telefono.trim(),
            correo: data.correo.trim().toLowerCase(),
            estado: "ACTIVA"
        };

        // === Enviar al backend ===
        try {
            const response = await fetch(`${API_URL}/Sedes`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Accept": "application/json"
                },
                body: JSON.stringify(payload)
            });

            // Manejo de errores HTTP
            if (!response.ok) {
                let errorMsg = "";
                try {
                    const errorData = await response.json();
                    errorMsg = errorData.message || JSON.stringify(errorData);
                } catch {
                    errorMsg = await response.text();
                }

                if (response.status === 409) {
                    showModal("⚠️ Ya existe una sede con ese nombre.");
                } else if (response.status === 400) {
                    showModal(`⚠️ Datos inválidos: ${errorMsg}`);
                } else {
                    showModal(`❌ Error del servidor: ${errorMsg}`);
                }
                return;
            }

            // === Éxito ===
            showModal(`✅ La sede "${payload.nombre}" fue registrada correctamente.`, () => {
                form.reset();
                form.querySelector('[data-field="estado"]').value = "ACTIVA";
            });

        } catch (error) {
            console.error("Error de conexión:", error);
            showModal("❌ No se pudo conectar con el servidor. Verifica que el backend esté en ejecución.");
        }
    });
});

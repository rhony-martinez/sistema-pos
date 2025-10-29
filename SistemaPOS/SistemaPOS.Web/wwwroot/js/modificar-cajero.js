document.addEventListener("DOMContentLoaded", async () => {
    // === Referencias al DOM ===
    const form = document.getElementById("form-modificar-cajero");
    const cancelBtn = document.getElementById("cancelBtn");
    const modal = document.getElementById("modal");
    const modalText = document.getElementById("modal-text");
    const modalBtn = document.getElementById("modal-btn");
    const idSpan = document.getElementById("idUsuario");
    const estadoToggle = document.getElementById("estado");

    // === Obtener ID del usuario desde la URL ===
    const params = new URLSearchParams(window.location.search);
    const userId = params.get("id");

    if (!userId) {
        showModal("⚠️ No se especificó un usuario para modificar.");
        form.style.display = "none";
        return;
    }

    // Mostrar ID en pantalla
    idSpan.textContent = `${userId}`;

    // === Consultar datos del usuario desde la API ===
    try {
        const response = await fetch(`${API_URL}/Usuario/${userId}`);
        if (!response.ok) throw new Error("Error al obtener los datos del usuario.");
        const user = await response.json();

        // Rellenar formulario
        document.getElementById("primerNombre").value = user.usuPrimerNombre || "";
        document.getElementById("segundoNombre").value = user.usuSegundoNombre || "";
        document.getElementById("primerApellido").value = user.usuPrimerApellido || "";
        document.getElementById("segundoApellido").value = user.usuSegundoApellido || "";
        document.getElementById("correo").value = user.usuCorreo || "";
        document.getElementById("telefono").value = user.usuTelefono || "";

        // Estado: si es ACTIVO → slider a la derecha
        const isActive = user.usuEstado?.toUpperCase() === "ACTIVO";
        estadoToggle.checked = isActive;
        updateEstadoColores(isActive);

        // Escuchar cambios del toggle
        estadoToggle.addEventListener("change", () => {
            updateEstadoColores(estadoToggle.checked);
        });

    } catch (error) {
        console.error("❌ Error al cargar usuario:", error);
        showModal("No se pudo cargar la información del usuario.");
    }

    // === Manejar envío del formulario (PUT) ===
    form.addEventListener("submit", async (e) => {
        e.preventDefault();
        let valido = true;
        let mensaje = "";

        // Reset de errores visuales previos
        form.querySelectorAll("input").forEach(input => input.classList.remove("input-error"));

        const updatedUser = {
            usuPrimerNombre: document.getElementById("primerNombre").value.trim(),
            usuSegundoNombre: document.getElementById("segundoNombre").value.trim() || null,
            usuPrimerApellido: document.getElementById("primerApellido").value.trim(),
            usuSegundoApellido: document.getElementById("segundoApellido").value.trim() || null,
            usuCorreo: document.getElementById("correo").value.trim(),
            usuTelefono: document.getElementById("telefono").value.trim(),
            usuEstado: estadoToggle.checked ? "ACTIVO" : "INACTIVO"
        };

        // Expresiones regulares
        const regexSoloLetras = /^[A-Za-zÁÉÍÓÚáéíóúñÑ\s]+$/;
        const regexCorreo = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        const regexTelefono = /^[\d+\-\s()]{7,20}$/;

        // Campos obligatorios vacíos
        if (primerNombre.value.trim() === "") {
            marcarError(primerNombre);
            valido = false;
            mensaje = "El primer nombre es obligatorio.";
        }

        if (primerApellido.value.trim() === "") {
            marcarError(primerApellido);
            valido = false;
            mensaje = "El primer apellido es obligatorio.";
        }

        if (correo.value.trim() === "") {
            marcarError(correo);
            valido = false;
            mensaje = "El correo electrónico es obligatorio.";
        }

        if (telefono.value.trim() === "") {
            marcarError(telefono);
            valido = false;
            mensaje = "El teléfono es obligatorio.";
        }

        // Validación de solo letras y longitud (20 máx)
        [primerNombre, segundoNombre, primerApellido, segundoApellido].forEach(campo => {
            const valor = campo.value.trim();
            if (valor !== "" && (!regexSoloLetras.test(valor) || valor.length > 20)) {
                marcarError(campo);
                valido = false;
                mensaje = "Los nombres y apellidos deben contener solo letras y máximo 20 caracteres.";
            }
        });

        // Validar formato de correo
        if (correo.value.trim() !== "" && !regexCorreo.test(correo.value.trim())) {
            marcarError(correo);
            valido = false;
            mensaje = "El formato del correo electrónico no es válido.";
        }

        // Validar teléfono (solo dígitos, +, -, espacios y paréntesis)
        if (telefono.value.trim() !== "" && !regexTelefono.test(telefono.value.trim())) {
            marcarError(telefono);
            valido = false;
            mensaje = "El teléfono debe contener solo números y caracteres válidos (+, -, espacios, paréntesis).";
        }

        // Si hay errores, mostrar mensaje y detener envío
        if (!valido) {
            showInlineMessage(mensaje);
            return;
        }

        // Si pasa todas las validaciones, continuar con la lógica de actualización
        guardarCambios();

        try {
            const response = await fetch(`${API_URL}/Usuario/${userId}`, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(updatedUser)
            });

            if (!response.ok) {
                const errText = await response.text();
                throw new Error(errText || "Error al actualizar usuario.");
            }

            showModal("✅ Los cambios se guardaron correctamente.", true);
        } catch (error) {
            console.error("❌ Error al guardar cambios:", error);
            showModal("Ocurrió un error al guardar los cambios.");
        }
    });

    // === Botón Cancelar ===
    cancelBtn.addEventListener("click", () => {
        window.location.href = "gestion-usuarios-adm-local.html";
    });

    // === Funciones auxiliares ===
    function showModal(message, redirect = false) {
        modalText.textContent = message;
        modal.style.display = "flex";
        modalBtn.onclick = () => {
            modal.style.display = "none";
            if (redirect) {
                window.location.href = "gestion-usuarios-adm-local.html";
            }
        };
    }

    function updateEstadoColores(isActive) {
        const estadoActivo = document.getElementById("estadoActivo");
        const estadoInactivo = document.getElementById("estadoInactivo");
        // Evita errores si no existen (por ejemplo, si el DOM no cargó bien)
        if (!estadoActivo || !estadoInactivo) {
            console.warn("⚠️ Elementos de estado no encontrados aún.");
            return;
        }

        if (isActive) {
            estadoActivo.style.color = "#007bff";   // Azul cuando activo
            estadoInactivo.style.color = "#333";    // Gris cuando no aplica
        } else {
            estadoActivo.style.color = "#333";      // Gris cuando no aplica
            estadoInactivo.style.color = "#e63946"; // Rojo cuando inactivo
        }
    }

    // === Función para marcar error visualmente ===
    function marcarError(input) {
        input.classList.add("input-error");
    }

    // === Función para mostrar mensaje inline en la interfaz ===
    function showInlineMessage(msg) {
        const existing = document.querySelector(".inline-error");
        if (existing) existing.remove();

        const p = document.createElement("p");
        p.textContent = msg;
        p.classList.add("inline-error");
        form.insertBefore(p, form.firstChild);

        setTimeout(() => p.remove(), 4000);
    }

    // === Modal genérica (para errores más críticos) ===
    function showModalVal(msg) {
        modalText.textContent = msg;
        modal.style.display = "flex";
        modalBtn.onclick = () => modal.style.display = "none";
    }

    // === Simulación de envío (aquí va tu lógica PUT actual) ===
    async function guardarCambios() {
        // Aquí va tu lógica de actualización existente (fetch PUT)
        showModalVal("✅ Cambios guardados correctamente.");
    }
});

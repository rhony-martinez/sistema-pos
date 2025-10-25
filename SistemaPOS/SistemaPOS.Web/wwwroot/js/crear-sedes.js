document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("form-sede");
    const modal = document.getElementById("modal");
    const modalText = document.getElementById("modal-text");
    const modalBtn = document.getElementById("modal-btn");
    const cancelBtn = document.getElementById("cancelBtn");

    console.log("✅ Script crear-sedes.js cargado y ejecutado");

    // --- Mostrar modal bonito ---
    function showModal(message, callback) {
        modalText.textContent = message;
        modal.style.display = "flex";
        modalBtn.onclick = () => {
            modal.style.display = "none";
            if (callback) callback();
        };
    }

    // --- Botón cancelar ---
    cancelBtn.addEventListener("click", () => {
        window.location.href = "gestion-sedes.html";
    });

    // --- Enviar formulario ---
    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        // Limpiar errores previos
        document.querySelectorAll(".input-error").forEach(el => el.classList.remove("input-error"));

        // Recolectar datos
        const data = {};
        form.querySelectorAll("[data-field]").forEach(input => {
            const key = input.getAttribute("data-field");
            data[key] = input.value.trim();
        });

        // Validar campos obligatorios
        let missing = [];
        const nombre = form.querySelector('[data-field="nombre"]');
        if (!nombre.value.trim()) {
            nombre.classList.add("input-error");
            missing.push(nombre);
        }

        if (missing.length > 0) {
            showModal("⚠️ Debes completar los campos obligatorios antes de continuar.", () => missing[0].focus());
            return;
        }

        const payload = {
            nombre: data.nombre,
            direccion: data.direccion,
            ciudad: data.ciudad,
            departamento: data.departamento,
            ubicacion: data.ubicacion || "",
            telefono: data.telefono,
            correo: data.correo,
            estado: "ACTIVA"
        };

        try {
            const response = await fetch("http://localhost:5289/api/Sedes", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Accept": "application/json"
                },
                body: JSON.stringify(payload)
            });

            if (!response.ok) {
                let errorMsg = "";
                try {
                    const errorData = await response.json();
                    errorMsg = errorData.message || JSON.stringify(errorData);
                } catch {
                    errorMsg = await response.text();
                }

                if (response.status === 409) {
                    showModal(`⚠️ Ya existe una sede con ese nombre.`);
                } else if (response.status === 400) {
                    showModal(`⚠️ Datos inválidos: ${errorMsg}`);
                } else {
                    showModal(`❌ Error del servidor: ${errorMsg}`);
                }
                return;
            }

            // Éxito
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

document.addEventListener("DOMContentLoaded", async () => {
    aplicarValidacionesGlobales(); //  Activa las validaciones universales
    if (!checkAuth()) return;

    const sedeInput = document.getElementById("sedeId");
    const usuIdInput = document.getElementById("usuId");
    const usernameInput = document.getElementById("username");
    const form = document.getElementById("form-crear-cajero");

    // Modal reutilizable
    const modal = document.getElementById("modal");
    const modalText = document.getElementById("modal-text");
    const modalBtn = document.getElementById("modal-btn");

    function showModal(message, callback) {
        if (!modal || !modalText || !modalBtn) {
            // fallback si no existe modal
            alert(message);
            if (callback) callback();
            return;
        }
        modalText.textContent = message;
        modal.style.display = "flex";
        modalBtn.onclick = () => {
            modal.style.display = "none";
            if (callback) callback();
        };
    }

              // Solo permitir números en campos ID
    ["usuId", "sedeId", "telefono"].forEach(id => {
        document.getElementById(id).addEventListener("keypress", (e) => {
            if (!/[0-9]/.test(e.key)) {
                e.preventDefault();
                showModal("Solo se permiten números.");
            }
        });
    });



    // Cargar sede automáticamente desde el perfil del usuario logueado
    let profile = null;
    try {
        profile = await getUserProfile();
    } catch (err) {
        console.error("No se pudo obtener perfil:", err);
    }

    if (profile && profile.sedeId) {
        sedeInput.value = profile.sedeId;
    } else {
        showModal("⚠️ No se pudo obtener la sede del usuario logueado.");
        return;
    }

    // Envío del formulario
    form?.addEventListener("submit", async (e) => {
        e.preventDefault();

        // Limpiar errores previos
        document.querySelectorAll(".input-error").forEach(el => el.classList.remove("input-error"));

        const usuario = {
            usuId: parseInt(usuIdInput.value),
            primerNombre: document.getElementById("primerNombre").value.trim(),
            segundoNombre: document.getElementById("segundoNombre").value.trim(),
            primerApellido: document.getElementById("primerApellido").value.trim(),
            segundoApellido: document.getElementById("segundoApellido").value.trim(),
            correo: document.getElementById("correo").value.trim(),
            telefono: document.getElementById("telefono").value.trim(),
            username: usernameInput.value.trim(),
            password: document.getElementById("password").value.trim(),
            rol: "CAJERO",
            sedeId: parseInt(sedeInput.value)
        };
       

        // Validación de campos obligatorios
        const requiredFields = ["usuId", "primerNombre", "primerApellido", "correo", "username", "password"];
        let missing = [];
        requiredFields.forEach(id => {
            const field = document.getElementById(id);
            if (!field || !field.value.trim()) {
                field?.classList.add("input-error");
                if (field) missing.push(field);
            }
        });

        if (missing.length > 0) {
            showModal("Tienes campos vacíos. Complétalos antes de continuar.", () => missing[0].focus());
            return;
        }

        // Llamada al backend
        try {
            const res = await fetch(`${API_URL}/users`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${sessionStorage.getItem("token")}`
                },
                body: JSON.stringify(usuario)
            });

            if (!res.ok) {
                // Intentar leer mensaje del backend (si viene JSON)
                let err = {};
                try { err = await res.json(); } catch (_) { err = { error: await res.text().catch(() => "") }; }

                const msg = (err.error || "").toString().toLowerCase();

                // Detectar duplicados con coincidencias seguras
                //  - username: buscar "usuario" o "nombre de usuario" + "existe"
                //  - id: buscar palabra exacta "id" + "existe" o "ya existe"
                if ((/usuario/.test(msg) || /nombre de usuario/.test(msg)) && /existe/.test(msg)) {
                    document.getElementById("username").classList.add("input-error");
                    showModal("Ya existe un usuario con ese nombre de usuario.");
                } else if (/\bid\b/i.test(msg) && /existe/.test(msg)) {
                    document.getElementById("usuId").classList.add("input-error");
                    showModal("Ya existe un cajero con esa ID.");
                } else if (msg.includes("sede") && (msg.includes("no existe") || msg.includes("no encontrada"))) {
                    document.getElementById("sedeId").classList.add("input-error");
                    showModal("La sede ingresada no existe. Verifique el ID de la sede.");
                } else {
                    showModal("Error al crear cajero: " + (err.error || err.message || "Error desconocido"));
                }
                return;
            }

            // éxito
            showModal("✅ Cajero registrado correctamente.", () => {
                form.reset();
                // Redirige a la vista de usuarios del admin local
                window.location.href = "gestion-usuarios-adm-local.html";
            });
        } catch (networkError) {
            console.error("Error en la conexión:", networkError);
            showModal("❌ Error de conexión con el servidor.");
        }
    });

    // Botón cancelar (si existe)
    const cancelBtn = document.getElementById("cancelBtn");
    if (cancelBtn) {
        cancelBtn.addEventListener("click", () => {
            window.location.href = "gestion-usuarios-adm-local.html";
        });
    }
});

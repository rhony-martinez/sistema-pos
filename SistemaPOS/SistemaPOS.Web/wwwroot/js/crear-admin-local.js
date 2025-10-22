document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("form-crear-admin");
    const modal = document.getElementById("modal");
    const modalText = document.getElementById("modal-text");
    const modalBtn = document.getElementById("modal-btn");

    function showModal(message, callback) {
        modalText.textContent = message;
        modal.style.display = "flex";
        modalBtn.onclick = () => {
            modal.style.display = "none";
            if (callback) callback();
        };
    }

    // Solo permitir números en campos ID
    ["usuId", "sedeId"].forEach(id => {
        document.getElementById(id).addEventListener("keypress", (e) => {
            if (!/[0-9]/.test(e.key)) {
                e.preventDefault();
                showModal("Solo se permiten números en los campos de ID.");
            }
        });
    });

    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        // Limpiar errores previos
        document.querySelectorAll(".input-error").forEach(el => el.classList.remove("input-error"));

        // Validar campos obligatorios
        const required = ["usuId", "primerNombre", "primerApellido", "correo", "username", "password", "sedeId"];
        let vacios = [];
        required.forEach(id => {
            const field = document.getElementById(id);
            if (!field.value.trim()) {
                field.classList.add("input-error");
                vacios.push(field);
            }
        });

        if (vacios.length > 0) {
            showModal("Tienes campos vacíos. Complétalos antes de continuar.", () => vacios[0].focus());
            return;
        }

        const payload = {
            usuId: parseInt(document.getElementById("usuId").value),
            primerNombre: document.getElementById("primerNombre").value.trim(),
            segundoNombre: document.getElementById("segundoNombre").value.trim(),
            primerApellido: document.getElementById("primerApellido").value.trim(),
            segundoApellido: document.getElementById("segundoApellido").value.trim(),
            correo: document.getElementById("correo").value.trim(),
            telefono: document.getElementById("telefono").value.trim(),
            username: document.getElementById("username").value.trim(),
            password: document.getElementById("password").value.trim(),
            rol: "ADMIN_LOCAL",
            sedeId: parseInt(document.getElementById("sedeId").value)
        };

        try {
            const res = await fetch("http://localhost:5289/api/users", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${sessionStorage.getItem("token")}`
                },
                body: JSON.stringify(payload)
            });

            if (!res.ok) {
                const err = await res.json();
                const msg = err.error?.toLowerCase() || "";

                if (msg.includes("id") && msg.includes("existe")) {
                    document.getElementById("usuId").classList.add("input-error");
                    showModal("Ya existe un usuario con ese ID.");
                } else if (msg.includes("usuario") && msg.includes("existe")) {
                    document.getElementById("username").classList.add("input-error");
                    showModal("Ya existe un usuario con ese nombre.");
                } else if (msg.includes("entity changes")) {
                    const sedeVal = parseInt(document.getElementById("sedeId").value);
                    if (sedeVal > 1000) {
                        document.getElementById("sedeId").classList.add("input-error");
                        showModal("La sede ingresada no existe. Verifique el ID de la sede.");
                    } else {
                        document.getElementById("sedeId").classList.add("input-error");
                        showModal("Esta sede ya tiene un administrador asignado.");
                    }
                } else {
                    showModal("Error al crear administrador: " + (err.error || "Error desconocido"));
                }
                return;
            }

            showModal("✅ Administrador local creado correctamente.", () => {
                form.reset();
                window.location.href = "gestion-usuarios-adm-gral.html";
            });

        } catch (error) {
            console.error("Error de conexión:", error);
            showModal("❌ Error de conexión con el servidor.");
        }
    });

    // Botón cancelar
    document.getElementById("cancelBtn").addEventListener("click", () => {
        window.location.href = "gestion-usuarios-adm-gral.html";
    });
});

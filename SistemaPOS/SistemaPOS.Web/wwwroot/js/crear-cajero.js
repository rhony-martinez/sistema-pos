document.addEventListener("DOMContentLoaded", async () => {
    if (!checkAuth()) return;

    const sedeInput = document.getElementById("sedeId");
    const usuIdInput = document.getElementById("usuId");
    const usernameInput = document.getElementById("username");
    const form = document.getElementById("form-crear-cajero");

    // ✅ Mostrar modal reutilizable
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

    // ✅ Solo permitir números en ID
    usuIdInput.addEventListener("keypress", (e) => {
        if (!/[0-9]/.test(e.key)) {
            e.preventDefault();
            showModal("Solo se permiten números en el campo ID.");
        }
    });

    // ✅ Cargar sede automáticamente
    const profile = await getUserProfile();
    if (profile && profile.sedeId) {
        sedeInput.value = profile.sedeId;
    } else {
        showModal("⚠️ No se pudo obtener la sede del usuario logueado.");
        return;
    }

    // ✅ Enviar formulario
    form.addEventListener("submit", async (e) => {
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

        // ✅ Validación de campos vacíos
        const requiredFields = ["usuId", "primerNombre", "primerApellido", "correo", "username", "password"];
        let missing = [];
        requiredFields.forEach(id => {
            const field = document.getElementById(id);
            if (!field.value.trim()) {
                field.classList.add("input-error");
                missing.push(field);
            }
        });

        if (missing.length > 0) {
            showModal("Tienes campos vacíos. Complétalos antes de continuar.", () => {
                missing[0].focus();
            });
            return;
        }

        // ✅ Enviar datos al backend
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

                if (msg.includes("id")) {
                    document.getElementById("usuId").classList.add("input-error");
                    showModal("Ya existe un cajero con esa ID.");
                }
                else if (msg.includes("nombre de usuario")) {
                    document.getElementById("username").classList.add("input-error");
                    showModal("Ya existe un usuario con ese nombre.");
                }
                else {
                    showModal("Error al crear cajero: " + (err.error || "Error desconocido"));
                }
                return;
            }







            alert("✅ Usuario creado correctamente");
            form.reset();
            window.location.href = "gestion-usuarios-adm-gral.html";
        } catch (error) {
            console.error(error);
            alert("❌ No se pudo crear el usuario: " + error.message);
        }
 catch (error) {
            console.error("Error en la conexión:", error);
            showModal("❌ Error de conexión con el servidor.");
        }
    });

    // ✅ Botón cancelar
    const cancelBtn = document.getElementById("cancelBtn");
    if (cancelBtn) {
        cancelBtn.addEventListener("click", () => {
            window.location.href = "gestion-usuarios-adm-local.html";
        });
    }
});

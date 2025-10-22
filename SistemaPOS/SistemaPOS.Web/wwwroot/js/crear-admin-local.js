document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("form-crear-admin");
    const cancelarBtn = document.getElementById("cancelBtn");

    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        const token = sessionStorage.getItem("token");
        if (!token) {
            alert("No hay sesión activa. Inicia sesión nuevamente.");
            window.location.href = "index.html";
            return;
        }

        const usuario = {
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
            const response = await fetch("http://localhost:5289/api/users", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify(usuario)
            });

            if (!response.ok) {
                const error = await response.json();
                throw new Error(error.error || "Error al crear el usuario");
            }

            alert("✅ Administrador local creado correctamente.");
            window.location.href = "gestion-usuarios-adm-gral.html";
        } catch (err) {
            console.error("Error:", err);
            alert(`❌ No se pudo crear el usuario: ${err.message}`);
        }
    });

    cancelarBtn.addEventListener("click", () => {
        window.location.href = "gestion-usuarios-adm-gral.html";
    });
});

document.addEventListener("DOMContentLoaded", async () => {
    if (!checkAuth()) return;

    // Obtener sedeId del admin local
    const profile = await getUserProfile();
    if (profile && profile.sedeId) {
        document.getElementById("sedeId").value = profile.sedeId;
    } else {
        alert("No se pudo obtener la sede del usuario logueado.");
        return;
    }

    const form = document.getElementById("form-crear-cajero");

    form.addEventListener("submit", async (e) => {
        e.preventDefault();

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
            rol: "CAJERO",
            sedeId: parseInt(document.getElementById("sedeId").value)
        };

        // Validación básica
        if (!usuario.primerNombre || !usuario.primerApellido || !usuario.username || !usuario.password || !usuario.correo) {
            alert("Por favor, completa todos los campos obligatorios.");
            return;
        }

        try {
            const token = sessionStorage.getItem("token");
            const response = await fetch("http://localhost:5289/api/Users", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify(usuario)
            });

            if (response.ok) {
                alert("✅ Cajero creado correctamente.");
                window.location.href = "gestion-usuarios-adm-local.html";
            } else {
                const error = await response.json();
                alert("❌ No se pudo crear el cajero: " + (error.error || "Error desconocido"));
            }
        } catch (err) {
            console.error("Error en la conexión:", err);
            alert("❌ Error de conexión con el servidor.");
        }
    });

    // Botón cancelar
    document.getElementById("cancelar").addEventListener("click", () => {
        window.location.href = "gestion-usuarios-adm-local.html";
    });
});

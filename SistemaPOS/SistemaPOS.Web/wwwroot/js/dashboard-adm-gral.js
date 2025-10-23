document.addEventListener("DOMContentLoaded", async () => {
        // Verificar sesión
        if (typeof checkAuth === "function" && !checkAuth()) return;

        // Obtener el perfil del usuario logueado
        let profile = null;
        try {
            if (typeof getUserProfile === "function") {
                profile = await getUserProfile();
            } else {
                const token = sessionStorage.getItem("token");
                const res = await fetch("http://localhost:5289/api/Users/me", {
                    headers: { Authorization: `Bearer ${token}` }
                });
                if (res.ok) profile = await res.json();
            }
        } catch (err) {
            console.error("❌ Error obteniendo perfil:", err);
        }

        console.log("✅ Perfil recibido:", profile);

        // Actualizar nombre del usuario en el encabezado
        const nameTag = document.getElementById("user-name");
        if (nameTag && profile) {
            const nombre = profile.usuPrimerNombre ?? "";
            const apellido = profile.usuPrimerApellido ?? "";
            const username = profile.usuUsername ?? "";

            if (nombre || apellido) {
                nameTag.textContent = `${nombre} ${apellido}`.trim();
            } else {
                nameTag.textContent = username || "Usuario";
            }
        }
        // Botón de cerrar sesión
        document.getElementById("logoutBtn").addEventListener("click", () => logout());
    });

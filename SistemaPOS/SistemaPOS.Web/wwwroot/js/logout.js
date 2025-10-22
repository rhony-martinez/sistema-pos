document.addEventListener("DOMContentLoaded", () => {
    const logoutBtn = document.getElementById("logoutBtn");

    if (logoutBtn) {
        logoutBtn.addEventListener("click", async () => {
            const confirmLogout = confirm("¿Deseas cerrar sesión?");
            if (!confirmLogout) return;

            const token = sessionStorage.getItem("token");

            if (!token) {
                // si no hay token, simplemente redirige
                sessionStorage.clear();
                window.location.href = "index.html";
                return;
            }

            try {
                const response = await fetch("http://localhost:5289/api/auth/logout", {
                    method: "POST",
                    headers: {
                        "Authorization": `Bearer ${token}`
                    }
                });

                if (response.status === 204) {
                    console.log("Sesión cerrada correctamente en backend.");
                } else {
                    console.warn("No se pudo cerrar sesión en backend, pero se limpiará localmente.");
                }
            } catch (error) {
                console.error("Error al cerrar sesión:", error);
            } finally {
                // Limpieza local
                sessionStorage.removeItem("token");
                sessionStorage.removeItem("expiresAt");

                // Redirige al login
                window.location.href = "index.html";
            }
        });
    }
});

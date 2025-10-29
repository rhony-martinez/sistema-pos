document.addEventListener("DOMContentLoaded", () => {
    const logoutBtn = document.getElementById("logoutBtn");
    const modal = document.getElementById("logoutModal");
    const confirmBtn = document.getElementById("confirmLogout");
    const cancelBtn = document.getElementById("cancelLogout");

    if (!logoutBtn) return;

    const doLogout = async () => {
        const token = sessionStorage.getItem("token");

        if (!token) {
            sessionStorage.clear();
            window.location.href = "index.html";
            return;
        }

        try {
            const response = await fetch(`${API_URL}/auth/logout`, {
                method: "POST",
                headers: { "Authorization": `Bearer ${token}` }
            });

            if (response.status === 204) {
                console.log("Sesión cerrada correctamente en backend.");
            } else {
                console.warn("No se pudo cerrar sesión en backend, pero se limpiará localmente.");
            }
        } catch (error) {
            console.error("Error al cerrar sesión:", error);
        } finally {
            sessionStorage.removeItem("token");
            sessionStorage.removeItem("expiresAt");
            window.location.href = "index.html";
        }
    };

    logoutBtn.addEventListener("click", () => {
        if (modal) {
            modal.style.display = "flex";
        } else {
            if (confirm("¿Deseas cerrar sesión?")) {
                doLogout();
            }
        }
    });

    if (modal && confirmBtn && cancelBtn) {
        cancelBtn.addEventListener("click", () => {
            modal.style.display = "none";
        });

        confirmBtn.addEventListener("click", () => {
            modal.style.display = "none";
            doLogout();
        });
    }
});

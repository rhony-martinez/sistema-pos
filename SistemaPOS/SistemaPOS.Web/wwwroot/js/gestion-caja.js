document.getElementById("logoutBtn").addEventListener("click", async () => {
    const token = sessionStorage.getItem("token");
    if (!token) {
        sessionStorage.clear();
        window.location.href = "index.html";
        return;
    }

    try {
        const res = await fetch(`${API_URL}/auth/logout`, {
            method: "POST",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            }
        });

        // Limpiar sesión y redirigir sin importar el estado
        sessionStorage.clear();
        window.location.href = "index.html";
    } catch (err) {
        console.error("Error al cerrar sesión:", err);
        alert("Error al cerrar sesión. Intenta nuevamente.");
    }
});
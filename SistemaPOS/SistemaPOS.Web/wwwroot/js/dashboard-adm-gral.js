document.addEventListener("DOMContentLoaded", async () => {
    // Verificar sesión
    if (typeof checkAuth === "function" && !checkAuth()) return;

    const token = sessionStorage.getItem("token");
    const usersCardValue = document.querySelector(".card-purple .card-value");
    const sedesCardValue = document.querySelector(".card-orange .card-value");

    // Obtener perfil del usuario logueado
    let profile = null;
    try {
        if (typeof getUserProfile === "function") {
            profile = await getUserProfile();
        } else {
            const res = await fetch(`${API_URL}/Users/me`, {
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
            nameTag.textContent = username || "Administrador General";
        }
    }

    // Obtener cantidad de usuarios activos (Cajeros + Admin Local)
    try {
        const resUsers = await fetch(`${API_URL}/users/activos/count`, {
            headers: { Authorization: `Bearer ${token}` }
        });

        if (resUsers.ok) {
            const data = await resUsers.json();
            usersCardValue.textContent = data.usuariosActivos ?? "0";
        } else {
            usersCardValue.textContent = "—";
            console.warn("⚠️ No se pudo obtener la cantidad de usuarios activos.");
        }
    } catch (error) {
        console.error("❌ Error al obtener usuarios activos:", error);
        if (usersCardValue) usersCardValue.textContent = "—";
    }

    // Obtener cantidad de sedes activas
    try {
        const resSedes = await fetch(`${API_URL}/sede/activas/count`, {
            headers: { Authorization: `Bearer ${token}` }
        });

        if (resSedes.ok) {
            const data = await resSedes.json();
            sedesCardValue.textContent = data.sedesActivas ?? "0";
        } else {
            sedesCardValue.textContent = "—";
            console.warn("⚠️ No se pudo obtener la cantidad de sedes activas.");
        }
    } catch (error) {
        console.error("❌ Error al obtener sedes activas:", error);
        if (sedesCardValue) sedesCardValue.textContent = "—";
    }

    // Botón de cerrar sesión
    const logoutBtn = document.getElementById("logoutBtn");
    if (logoutBtn) logoutBtn.addEventListener("click", () => logout());

    setInterval(actualizarDashboard, 30000);
});

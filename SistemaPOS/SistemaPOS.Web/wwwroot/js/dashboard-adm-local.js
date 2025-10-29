document.addEventListener("DOMContentLoaded", async () => {
    // Verificar autenticación
    if (typeof checkAuth === "function" && !checkAuth()) return;

    // Obtener perfil del usuario autenticado
    let profile = null;
    try {
        if (typeof getUserProfile === "function") {
            profile = await getUserProfile();
        } else {
            const token = sessionStorage.getItem("token");
            const res = await fetch(`${API_URL}/Users/me`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            if (res.ok) profile = await res.json();
        }
    } catch (err) {
        console.error("❌ Error obteniendo perfil:", err);
    }

    console.log("✅ Perfil recibido:", profile);

    // ✅ Actualizar nombre del usuario en el encabezado
    const nameTag = document.getElementById("user-name");
    if (nameTag && profile) {
        const nombre = profile.usuPrimerNombre || profile.primerNombre || "";
        const apellido = profile.usuPrimerApellido || profile.primerApellido || "";
        const username = profile.usuUsername || "";

        if (nombre || apellido) {
            nameTag.textContent = `${nombre} ${apellido}`.trim();
        } else {
            nameTag.textContent = username || "Administrador Local";
        }
    }

    // ✅ Manejo del botón de cerrar sesión
    const logoutBtn = document.getElementById("logoutBtn");
    if (logoutBtn) {
        logoutBtn.addEventListener("click", () => logout());
    }

    // ---------------------------------------------------------------------
    // 🔹 Lógica dinámica para actualizar las tarjetas del dashboard
    // ---------------------------------------------------------------------

    if (!profile || !profile.sedeId) {
        console.warn("⚠️ No se encontró SedeId en el perfil del usuario.");
        return;
    }

    const sedeId = profile.sedeId;
    const token = sessionStorage.getItem("token");
    const headers = { Authorization: `Bearer ${token}` };

    async function actualizarDashboard() {
        try {
            // 1. Obtener cantidad de cajeros en esta sede
            const resCajeros = await fetch(`${API_URL}/users/cajeros/activos/${sedeId}`, { headers });
            const cajerosData = await resCajeros.json();
            const cantCajeros = cajerosData.cantidad ?? 0;

            const cardCajeros = document.querySelector(".card-purple .card-value");
            if (cardCajeros) cardCajeros.textContent = cantCajeros;

            // 2. Verificar si hay caja abierta
            const resCaja = await fetch(`${API_URL}/caja/abierta/${sedeId}`, { headers });
            const cajaData = await resCaja.json();
            const abierta = cajaData.abierta === true;

            const cardCaja = document.querySelector(".card-orange .card-value");
            const cardCajaDetails = document.querySelector(".card-orange .card-details");
            if (cardCaja) cardCaja.textContent = abierta ? "Sí" : "No";
            if (cardCajaDetails)
                cardCajaDetails.textContent = abierta
                    ? "Caja actualmente abierta"
                    : "No hay caja abierta";

        } catch (error) {
            console.error("❌ Error al actualizar dashboard:", error);
        }
    }

    // Ejecutar una vez al cargar
    actualizarDashboard();

    // (Opcional) Refrescar cada 30 segundos
    setInterval(actualizarDashboard, 30000);
});

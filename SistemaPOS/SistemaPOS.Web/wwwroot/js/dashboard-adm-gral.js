document.addEventListener("DOMContentLoaded", async () => {
  // Verificar sesión
  if (typeof checkAuth === "function" && !checkAuth()) return;

  const token = sessionStorage.getItem("token");

  const usersCardValue = document.querySelector(".card-purple .card-value");
  const sedesCardValue = document.querySelector(".card-orange .card-value");

  // 👇 Estas dos tarjetas asumo que existen en tu HTML para admin general:
  // .card-blue  -> "Ventas Hoy"
  // .card-green -> "Ingresos del Mes"
  const ventasHoyCardValue = document.querySelector(".card-blue .card-value");
  const ingresosMesCardValue = document.querySelector(".card-green .card-value");

  const headers = { Authorization: `Bearer ${token}` };

  const money = (n) => {
    const num = Number(n || 0);
    return num.toLocaleString("es-CO", { style: "currency", currency: "COP" });
  };

  const isSameDayLocal = (a, b) =>
    a.getFullYear() === b.getFullYear() &&
    a.getMonth() === b.getMonth() &&
    a.getDate() === b.getDate();

  const startOfMonthLocal = (d) => new Date(d.getFullYear(), d.getMonth(), 1, 0, 0, 0, 0);

  async function actualizarTarjetasVentas() {
    try {
      const resVentas = await fetch(`${API_URL}/Venta`, { headers });
      if (!resVentas.ok) {
        console.warn("⚠️ No se pudo obtener ventas.");
        if (ventasHoyCardValue) ventasHoyCardValue.textContent = "—";
        if (ingresosMesCardValue) ingresosMesCardValue.textContent = "—";
        return;
      }

      const ventas = await resVentas.json();
      const hoy = new Date();
      const inicioMes = startOfMonthLocal(hoy);

      let totalHoy = 0;
      let totalMes = 0;

      for (const v of (ventas || [])) {
        const f = new Date(v.fechaVenta);
        if (isNaN(f.getTime())) continue;

        const total = Number(v.venTotal || 0);

        if (isSameDayLocal(f, hoy)) totalHoy += total;
        if (f >= inicioMes && f <= hoy) totalMes += total;
      }

      if (ventasHoyCardValue) ventasHoyCardValue.textContent = money(totalHoy);
      if (ingresosMesCardValue) ingresosMesCardValue.textContent = money(totalMes);

    } catch (err) {
      console.error("❌ Error calculando ventas:", err);
      if (ventasHoyCardValue) ventasHoyCardValue.textContent = "—";
      if (ingresosMesCardValue) ingresosMesCardValue.textContent = "—";
    }
  }

  async function actualizarDashboard() {
    // 1) Perfil
    let profile = null;
    try {
      if (typeof getUserProfile === "function") {
        profile = await getUserProfile();
      } else {
        const res = await fetch(`${API_URL}/Users/me`, { headers });
        if (res.ok) profile = await res.json();
      }
    } catch (err) {
      console.error("❌ Error obteniendo perfil:", err);
    }

    console.log("✅ Perfil recibido:", profile);

    // Nombre en header
    const nameTag = document.getElementById("user-name");
    if (nameTag && profile) {
      const nombre = profile.usuPrimerNombre ?? "";
      const apellido = profile.usuPrimerApellido ?? "";
      const username = profile.usuUsername ?? "";

      nameTag.textContent = (nombre || apellido)
        ? `${nombre} ${apellido}`.trim()
        : (username || "Administrador General");
    }

    // 2) Usuarios activos
    try {
      const resUsers = await fetch(`${API_URL}/users/activos/count`, { headers });
      if (resUsers.ok) {
        const data = await resUsers.json();
        if (usersCardValue) usersCardValue.textContent = data.usuariosActivos ?? "0";
      } else {
        if (usersCardValue) usersCardValue.textContent = "—";
        console.warn("⚠️ No se pudo obtener la cantidad de usuarios activos.");
      }
    } catch (error) {
      console.error("❌ Error al obtener usuarios activos:", error);
      if (usersCardValue) usersCardValue.textContent = "—";
    }

    // 3) Sedes activas
    try {
      const resSedes = await fetch(`${API_URL}/sede/activas/count`, { headers });
      if (resSedes.ok) {
        const data = await resSedes.json();
        if (sedesCardValue) sedesCardValue.textContent = data.sedesActivas ?? "0";
      } else {
        if (sedesCardValue) sedesCardValue.textContent = "—";
        console.warn("⚠️ No se pudo obtener la cantidad de sedes activas.");
      }
    } catch (error) {
      console.error("❌ Error al obtener sedes activas:", error);
      if (sedesCardValue) sedesCardValue.textContent = "—";
    }

    // 4) Ventas hoy / ingresos del mes (todas las sedes)
    await actualizarTarjetasVentas();
  }

  // Botón de cerrar sesión
  const logoutBtn = document.getElementById("logoutBtn");
  if (logoutBtn) logoutBtn.addEventListener("click", () => logout());

  // Ejecutar una vez al cargar
  await actualizarDashboard();

  // Refrescar cada 30 segundos
  setInterval(actualizarDashboard, 30000);
});

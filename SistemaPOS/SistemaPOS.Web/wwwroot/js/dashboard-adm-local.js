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

  // sedeId puede venir como "sedeId" o "SedeId" dependiendo de tu backend
  const sedeId = profile?.sedeId ?? profile?.SedeId ?? null;

  if (!profile || !sedeId) {
    console.warn("⚠️ No se encontró SedeId en el perfil del usuario.");
    return;
  }

  const token = sessionStorage.getItem("token");
  const headers = { Authorization: `Bearer ${token}` };

  // ===== Helpers fechas =====
  function inicioDia(d = new Date()) {
    return new Date(d.getFullYear(), d.getMonth(), d.getDate(), 0, 0, 0, 0);
  }
  function finDia(d = new Date()) {
    return new Date(d.getFullYear(), d.getMonth(), d.getDate(), 23, 59, 59, 999);
  }
  function inicioMesActual() {
    const now = new Date();
    return new Date(now.getFullYear(), now.getMonth(), 1, 0, 0, 0, 0);
  }
  function finMesActual() {
    const now = new Date();
    // último milisegundo del último día del mes
    return new Date(now.getFullYear(), now.getMonth() + 1, 0, 23, 59, 59, 999);
  }
  function money(n) {
    return `$${Number(n || 0).toLocaleString("es-CO")}`;
  }

  // ===== Actualiza DOM de cards =====
  function setCardValue(selector, value) {
    const el = document.querySelector(selector);
    if (el) el.textContent = value;
  }

  async function actualizarDashboard() {
    try {
      // 1. Obtener cantidad de cajeros en esta sede
      const resCajeros = await fetch(`${API_URL}/users/cajeros/activos/${sedeId}`, { headers });
      const cajerosData = await resCajeros.json().catch(() => ({}));
      const cantCajeros = cajerosData.cantidad ?? 0;

      const cardCajeros = document.querySelector(".card-purple .card-value");
      if (cardCajeros) cardCajeros.textContent = cantCajeros;

      // 2. Verificar si hay caja abierta
      const resCaja = await fetch(`${API_URL}/caja/abierta/${sedeId}`, { headers });
      const cajaData = await resCaja.json().catch(() => ({}));
      const abierta = cajaData.abierta === true;

      const cardCaja = document.querySelector(".card-orange .card-value");
      const cardCajaDetails = document.querySelector(".card-orange .card-details");
      if (cardCaja) cardCaja.textContent = abierta ? "Sí" : "No";
      if (cardCajaDetails) {
        cardCajaDetails.textContent = abierta ? "Caja actualmente abierta" : "No hay caja abierta";
      }

      // 3. ✅ Ventas Hoy + 4. ✅ Ingresos del Mes (mes actual)
      // Tu backend (con Authorize + filtrado por sede en GET /Venta) debería devolver solo las ventas de la sede del ADMIN_LOCAL.
      // Igual dejo un filtro extra por si alguien es ADMIN_GENERAL o si el endpoint aún no filtra.
      const resVentas = await fetch(`${API_URL}/Venta`, { headers });
      if (!resVentas.ok) throw new Error(`Error consultando ventas (${resVentas.status})`);
      let ventas = await resVentas.json();

      ventas = Array.isArray(ventas) ? ventas : [];

      // Si por alguna razón vienen de todas las sedes, intentamos filtrar por sede:
      // (Solo funciona si el backend incluye Caja con SedeId. Si Caja viene null, el filtrado real debe ser backend.)
      const ventasFiltradasSede = ventas.filter(v => {
        const sedeVenta = v?.caja?.sedeId ?? v?.caja?.SedeId ?? null;
        // si no viene caja, no podemos saber: asumimos que backend ya filtró
        return sedeVenta == null ? true : String(sedeVenta) === String(sedeId);
      });

      const now = new Date();
      const hoyIni = inicioDia(now);
      const hoyFin = finDia(now);

      const mesIni = inicioMesActual();
      const mesFin = finMesActual();

      const ventasHoy = ventasFiltradasSede.filter(v => {
        const f = new Date(v.fechaVenta);
        return !isNaN(f.getTime()) && f >= hoyIni && f <= hoyFin;
      });

      const ventasMes = ventasFiltradasSede.filter(v => {
        const f = new Date(v.fechaVenta);
        return !isNaN(f.getTime()) && f >= mesIni && f <= mesFin;
      });

      const totalHoy = ventasHoy.reduce((acc, v) => acc + Number(v.venTotal || 0), 0);
      const totalMes = ventasMes.reduce((acc, v) => acc + Number(v.venTotal || 0), 0);

      // Cards: Ventas Hoy (azul) e Ingresos del Mes (verde)
      // Tu HTML usa .card-value como número grande.
      setCardValue(".card-blue .card-value", money(totalHoy));
      setCardValue(".card-green .card-value", money(totalMes));

      // (Opcional) si quieres que los detalles muestren algo real:
      // setCardValue(".card-blue .card-details", `${ventasHoy.length} ventas hoy`);
      // setCardValue(".card-green .card-details", `${ventasMes.length} ventas este mes`);

    } catch (error) {
      console.error("❌ Error al actualizar dashboard:", error);
    }
  }

  // Ejecutar una vez al cargar
  await actualizarDashboard();

  // Refrescar cada 30 segundos
  setInterval(actualizarDashboard, 30000);
});

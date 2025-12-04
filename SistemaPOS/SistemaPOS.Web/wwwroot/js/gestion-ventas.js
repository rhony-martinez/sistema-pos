document.addEventListener("DOMContentLoaded", () => {
  const token = sessionStorage.getItem("token");
  if (!token) {
    alert("Debes iniciar sesión.");
    window.location.href = "index.html";
    return;
  }

  // Inputs de rango (desde/hasta)
  const desdeEl = document.getElementById("fecha-desde");
  const hastaEl = document.getElementById("fecha-hasta");

  // Defaults: últimos 30 días (igual que tu informe)
  const hoy = new Date();
  const hace30 = new Date();
  hace30.setDate(hoy.getDate() - 30);

  if (desdeEl && !desdeEl.value) desdeEl.value = isoDate(hace30);
  if (hastaEl && !hastaEl.value) hastaEl.value = isoDate(hoy);

  // Carga inicial
  cargarVentas();

  // Re-render cuando cambian las fechas
  if (desdeEl) desdeEl.addEventListener("change", cargarVentas);
  if (hastaEl) hastaEl.addEventListener("change", cargarVentas);

  // Botón informe PDF
  const btn = document.getElementById("btn-informe");
  if (btn) btn.addEventListener("click", generarInformeVentasPdf);
});

function money(n) {
  return `$${Number(n || 0).toLocaleString("es-CO")}`;
}

function formatFecha(fechaIso) {
  const d = new Date(fechaIso);
  if (isNaN(d.getTime())) return "—";
  const fecha = d.toLocaleDateString("es-CO");
  const hora = d.toLocaleTimeString("es-CO", { hour: "2-digit", minute: "2-digit" });
  return `${fecha} ${hora}`;
}

function isoDate(d) {
  const yyyy = d.getFullYear();
  const mm = String(d.getMonth() + 1).padStart(2, "0");
  const dd = String(d.getDate()).padStart(2, "0");
  return `${yyyy}-${mm}-${dd}`;
}

function parseISOToDateStart(iso) {
  // yyyy-mm-dd => 00:00:00.000
  if (!iso) return null;
  const [y, m, d] = iso.split("-").map(Number);
  return new Date(y, m - 1, d, 0, 0, 0, 0);
}

function parseISOToDateEnd(iso) {
  // yyyy-mm-dd => 23:59:59.999
  if (!iso) return null;
  const [y, m, d] = iso.split("-").map(Number);
  return new Date(y, m - 1, d, 23, 59, 59, 999);
}

async function cargarVentas() {
  const tbody = document.getElementById("ventas-tbody");
  const token = sessionStorage.getItem("token");

  // rango
  const desdeStr = document.getElementById("fecha-desde")?.value;
  const hastaStr = document.getElementById("fecha-hasta")?.value;

  if (!tbody) return;

  // Validación básica del rango
  const desde = parseISOToDateStart(desdeStr);
  const hasta = parseISOToDateEnd(hastaStr);

  if (!desde || !hasta) {
    tbody.innerHTML = `<tr><td colspan="6">Selecciona un rango de fechas válido.</td></tr>`;
    return;
  }

  if (desde > hasta) {
    tbody.innerHTML = `<tr><td colspan="6">La fecha "desde" no puede ser mayor que "hasta".</td></tr>`;
    return;
  }

  tbody.innerHTML = `<tr><td colspan="6">Cargando...</td></tr>`;

  try {
    const res = await fetch(`${API_URL}/Venta`, {
      headers: {
        "Authorization": `Bearer ${token}`,
        "Content-Type": "application/json"
      }
    });

    if (!res.ok) {
      tbody.innerHTML = `<tr><td colspan="6">Error cargando ventas (${res.status})</td></tr>`;
      return;
    }

    const ventas = await res.json();

    // ✅ filtro por rango (incluye todo el día "hasta")
    const filtradas = (ventas || []).filter(v => {
      const f = new Date(v.fechaVenta);
      if (isNaN(f.getTime())) return false;
      return f >= desde && f <= hasta;
    });

    if (!filtradas.length) {
      tbody.innerHTML = `<tr><td colspan="6">No hay ventas para mostrar en ese rango.</td></tr>`;
      return;
    }

    // opcional: ordenar por fecha desc (más recientes arriba)
    filtradas.sort((a, b) => new Date(b.fechaVenta) - new Date(a.fechaVenta));

    tbody.innerHTML = "";
    for (const v of filtradas) {
      const tr = document.createElement("tr");

      tr.innerHTML = `
        <td>#V-${v.venId}</td>
        <td>${formatFecha(v.fechaVenta)}</td>
        <td>${money(v.venTotal)}</td>
        <td>${v.venMetodoPago || "—"}</td>
        <td>${v.cajeroNombre || "—"}</td>
        <td>
          <button class="btn btn-action" data-action="factura" data-id="${v.venId}">
            <i class="fas fa-file-invoice"></i>
          </button>
          <button class="btn btn-action btn-danger" data-action="eliminar" data-id="${v.venId}">
            <i class="fas fa-trash-alt"></i>
          </button>
        </td>
      `;

      tbody.appendChild(tr);
    }

    // acciones
    tbody.onclick = async (e) => {
      const btn = e.target.closest("button");
      if (!btn) return;

      const action = btn.dataset.action;
      const id = btn.dataset.id;

      if (action === "factura") {
        window.location.href = `factura.html?venId=${id}`;
      }

      if (action === "eliminar") {
        alert("Aún no está implementado eliminar (falta endpoint DELETE en backend).");
      }
    };
  } catch (err) {
    console.error(err);
    tbody.innerHTML = `<tr><td colspan="6">Error inesperado al cargar ventas.</td></tr>`;
  }
}

async function generarInformeVentasPdf() {
  const token = sessionStorage.getItem("token");
  const desde = document.getElementById("fecha-desde")?.value;
  const hasta = document.getElementById("fecha-hasta")?.value;

  if (!token) {
    alert("Debes iniciar sesión.");
    return;
  }

  const qs = new URLSearchParams();
  if (desde) qs.set("desde", desde);
  if (hasta) qs.set("hasta", hasta);

  const res = await fetch(`${API_URL}/Venta/reporte/pdf?${qs.toString()}`, {
    headers: { Authorization: `Bearer ${token}` }
  });

  if (!res.ok) {
    const err = await res.json().catch(() => null);
    alert(err?.mensaje || "Error generando reporte");
    return;
  }

  const blob = await res.blob();
  const url = URL.createObjectURL(blob);

  const a = document.createElement("a");
  a.href = url;
  a.download = `reporte_ventas_${desde || "ultimos30"}_${hasta || ""}.pdf`;
  document.body.appendChild(a);
  a.click();
  a.remove();
  URL.revokeObjectURL(url);
}

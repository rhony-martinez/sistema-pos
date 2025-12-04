document.addEventListener("DOMContentLoaded", () => {
  const token = sessionStorage.getItem("token");
  if (!token) {
    alert("Debes iniciar sesión.");
    window.location.href = "index.html";
    return;
  }

  const inputFecha = document.getElementById("filtro-fecha");
  cargarVentas(); // carga inicial

  if (inputFecha) {
    inputFecha.addEventListener("change", () => cargarVentas());
  }
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

async function cargarVentas() {
  const tbody = document.getElementById("ventas-tbody");
  const inputFecha = document.getElementById("filtro-fecha");
  const token = sessionStorage.getItem("token");

  if (!tbody) return;

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

    // filtro opcional por día (yyyy-mm-dd)
    let filtradas = ventas;
    const fechaSeleccionada = inputFecha?.value; // "2025-04-22"
    if (fechaSeleccionada) {
      filtradas = ventas.filter(v => {
        const f = new Date(v.fechaVenta);
        if (isNaN(f.getTime())) return false;
        const yyyy = f.getFullYear();
        const mm = String(f.getMonth() + 1).padStart(2, "0");
        const dd = String(f.getDate()).padStart(2, "0");
        return `${yyyy}-${mm}-${dd}` === fechaSeleccionada;
      });
    }

    if (!filtradas.length) {
      tbody.innerHTML = `<tr><td colspan="6">No hay ventas para mostrar.</td></tr>`;
      return;
    }

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
        // Reutiliza tu modal de factura si quieres o redirige
        // Ejemplo: redirigir a una vista "factura.html?id=123"
        window.location.href = `factura.html?venId=${id}`;
      }

      if (action === "eliminar") {
        // OJO: solo si tienes DELETE /Venta/{id}
        alert("Aún no está implementado eliminar (falta endpoint DELETE en backend).");
      }
    };
  } catch (err) {
    console.error(err);
    tbody.innerHTML = `<tr><td colspan="6">Error inesperado al cargar ventas.</td></tr>`;
  }
}

function isoDate(d) {
    const yyyy = d.getFullYear();
    const mm = String(d.getMonth() + 1).padStart(2, "0");
    const dd = String(d.getDate()).padStart(2, "0");
    return `${yyyy}-${mm}-${dd}`;
}

document.addEventListener("DOMContentLoaded", () => {
    const desde = document.getElementById("fecha-desde");
    const hasta = document.getElementById("fecha-hasta");
    const btn = document.getElementById("btn-informe");

    const hoy = new Date();
    const hace30 = new Date();
    hace30.setDate(hoy.getDate() - 30);

    if (desde && !desde.value) desde.value = isoDate(hace30);
    if (hasta && !hasta.value) hasta.value = isoDate(hoy);

    if (btn) btn.addEventListener("click", generarInformeVentasPdf);
});

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

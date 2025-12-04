document.addEventListener("DOMContentLoaded", () => {
  // --- BOTÓN CERRAR SESIÓN ---
  const logoutBtn = document.getElementById("logoutBtn");

  if (logoutBtn) {
    logoutBtn.addEventListener("click", async () => {
      const token = sessionStorage.getItem("token");

      if (!token) {
        sessionStorage.clear();
        window.location.href = "index.html";
        return;
      }

      try {
        await fetch(`${API_URL}/auth/logout`, {
          method: "POST",
          headers: {
            "Authorization": `Bearer ${token}`,
            "Content-Type": "application/json"
          }
        });

        sessionStorage.clear();
        window.location.href = "index.html";
      } catch (err) {
        console.error("Error al cerrar sesión:", err);
        alert("Error al cerrar sesión. Intenta nuevamente.");
      }
    });
  }

  // --- BOTÓN CREAR VENTA ---
  const btnCrearVenta = document.getElementById("btn-crear-venta");

  if (btnCrearVenta) {
    btnCrearVenta.addEventListener("click", () => {
      const token = sessionStorage.getItem("token");

      if (!token) {
        alert("Debe iniciar sesión para crear una venta.");
        window.location.href = "index.html";
        return;
      }

      window.location.href = "crear_venta.html";
    });
  }

  // ===============================
  // NUEVO: Cargar estado de caja (calculado)
  // ===============================
  cargarEstadoCaja();

  // ===============================
  // NUEVO: Botón cerrar caja
  // ===============================
  const btnCerrarCaja = document.getElementById("btn-cerrar-caja");
  if (btnCerrarCaja) {
    btnCerrarCaja.addEventListener("click", cerrarCaja);
  }
});

// ===============================
// Helpers
// ===============================
function money(n) {
  return `$${Number(n || 0).toLocaleString("es-CO")}`;
}

function setText(id, value) {
  const el = document.getElementById(id);
  if (el) el.textContent = value;
}

function pintarCajaEnCero() {
  setText("saldo-inicial", money(0));
  setText("ventas-netas", money(0));
  setText("ingresos-adicionales", money(0));
  setText("egresos", money(0));
  setText("saldo-final", money(0));
}

// ===============================
// NUEVO: consumir endpoint NUEVO de estado
// GET /api/Caja/abierta/estado/{sedeId}
// ===============================
async function cargarEstadoCaja() {
  try {
    const token = sessionStorage.getItem("token");
    const sedeId = sessionStorage.getItem("sedeId");

    if (!token || !sedeId) {
      pintarCajaEnCero();
      return;
    }

    const res = await fetch(`${API_URL}/Caja/abierta/estado/${sedeId}`, {
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
      }
    });

    // Si falla o no existe endpoint aún, no rompemos la pantalla
    if (!res.ok) {
      console.warn("No se pudo cargar estado de caja:", res.status);
      pintarCajaEnCero();
      return;
    }

    const data = await res.json();

    // Si no hay caja abierta -> null
    if (!data) {
      pintarCajaEnCero();
      return;
    }

    setText("saldo-inicial", money(data.montoInicial));
    setText("ventas-netas", money(data.ventasNetas));
    setText("ingresos-adicionales", money(data.ingresosAdicionales));
    setText("egresos", money(data.egresos));
    setText("saldo-final", money(data.saldoFinalEstimado));
  } catch (err) {
    console.error("Error cargando estado de caja:", err);
    pintarCajaEnCero();
  }
}

// ===============================
// NUEVO: cerrar caja usando endpoint NUEVO
// POST /api/Caja/cerrar/{sedeId}
// ===============================
async function cerrarCaja() {
  try {
    const token = sessionStorage.getItem("token");
    const sedeId = sessionStorage.getItem("sedeId");

    if (!token || !sedeId) {
      alert("No hay sesión activa.");
      return;
    }

    const res = await fetch(`${API_URL}/Caja/cerrar/${sedeId}/reporte/pdf`, {
      method: "POST",
      headers: {
        "Authorization": `Bearer ${token}`
        // NO Content-Type, no estás enviando body
      }
    });

    if (!res.ok) {
      // si tu backend devuelve JSON de error, intentamos leerlo
      const err = await res.json().catch(() => null);
      alert(err?.mensaje || "Error al cerrar caja");
      return;
    }

    // ✅ Descargar PDF
    const blob = await res.blob();
    const url = URL.createObjectURL(blob);

    const a = document.createElement("a");
    a.href = url;

    // Si el backend envía Content-Disposition con filename, mejor,
    // pero aquí ponemos uno fijo:
    a.download = `cierre_caja_sede_${sedeId}.pdf`;

    document.body.appendChild(a);
    a.click();
    a.remove();
    URL.revokeObjectURL(url);

    // Ya no hay caja abierta => queda en 0 (o lo que pinte tu endpoint)
    await cargarEstadoCaja();

  } catch (err) {
    console.error("Error al cerrar caja:", err);
    alert("Error al cerrar caja. Intenta nuevamente.");
  }
}


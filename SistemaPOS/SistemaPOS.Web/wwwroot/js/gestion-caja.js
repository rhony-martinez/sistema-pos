document.addEventListener("DOMContentLoaded", () => {
  // --- MODAL PERSONALIZADO ---
  function showModal(message, showConfirm = false) {
    modalMessage.textContent = message;

    modalButtons.innerHTML = showConfirm
      ? `
          <button id="modalCancel" class="btn btn-secondary">Cancelar</button>
          <button id="modalConfirm" class="btn btn-primary">Aceptar</button>
        `
      : `<button id="modalOk" class="btn btn-primary">OK</button>`;

    customModal.style.display = "flex";

    return new Promise(resolve => {
      if (showConfirm) {
        document.getElementById("modalCancel").onclick = () => {
          customModal.style.display = "none";
          resolve(false);
        };
        document.getElementById("modalConfirm").onclick = () => {
          customModal.style.display = "none";
          resolve(true);
        };
      } else {
        document.getElementById("modalOk").onclick = () => {
          customModal.style.display = "none";
          resolve(true);
        };
      }
    });
  }

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
        await showModal("Error al cerrar sesión. Intenta nuevamente.");
      }
    });
  }

  // --- BOTÓN CREAR VENTA ---
  const btnCrearVenta = document.getElementById("btn-crear-venta");

  if (btnCrearVenta) {
    btnCrearVenta.addEventListener("click", async () => {
      const token = sessionStorage.getItem("token");

      if (!token) {
        await showModal("Debe iniciar sesión para crear una venta.");
        window.location.href = "index.html";
        return;
      }

      window.location.href = "crear_venta.html";
    });
  }

  cargarEstadoCaja();

  // --- BOTÓN CERRAR CAJA ---
  const btnCerrarCaja = document.getElementById("btn-cerrar-caja");
  if (btnCerrarCaja) {
    btnCerrarCaja.addEventListener("click", cerrarCaja);
  }
});

// Helpers
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

// Cargar estado de caja
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

    if (!res.ok) {
      pintarCajaEnCero();
      return;
    }

    const data = await res.json();

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
    pintarCajaEnCero();
  }
}

// Cerrar caja
async function cerrarCaja() {
  try {
    const token = sessionStorage.getItem("token");
    const sedeId = sessionStorage.getItem("sedeId");

    if (!token || !sedeId) {
      await showModal("No hay sesión activa.");
      return;
    }

    const res = await fetch(`${API_URL}/Caja/cerrar/${sedeId}`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
      }
    });

    const json = await res.json().catch(() => null);

    if (!res.ok) {
      await showModal(json?.mensaje || "Error al cerrar caja");
      return;
    }

    await showModal(`Caja cerrada. Monto final: ${money(json?.montoFinal)}`);

    await cargarEstadoCaja();
  } catch (err) {
    await showModal("Error al cerrar caja. Intenta nuevamente.");
  }
}

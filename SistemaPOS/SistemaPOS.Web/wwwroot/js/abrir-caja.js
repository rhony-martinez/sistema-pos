// ===============================
// MODAL PERSONALIZADO PARA MENSAJES
// ===============================
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

/* ========= ✅ Validación estilo productos (reutilizable) ========= */
function aplicarValidacionSoloNumeros(input, { maxDigits = 10, mensaje = "Solo se permiten números." } = {}) {
  if (!input) return;
  if (input.dataset.validacionActiva === "true") return;
  input.dataset.validacionActiva = "true";

  let errorSpan = document.createElement("small");
  errorSpan.classList.add("error-msg");
  errorSpan.style.color = "red";
  errorSpan.style.display = "none";
  errorSpan.style.fontSize = "0.8rem";
  errorSpan.style.marginTop = "2px";
  input.insertAdjacentElement("afterend", errorSpan);

  function setError(msg) {
    errorSpan.textContent = msg;
    errorSpan.style.display = "block";
    input.classList.add("input-error");
  }
  function clearError() {
    errorSpan.textContent = "";
    errorSpan.style.display = "none";
    input.classList.remove("input-error");
  }

  input.addEventListener("input", (e) => {
    let value = String(e.target.value ?? "");
    value = value.replace(/[^0-9]/g, "");

    if (value.length > maxDigits) {
      value = value.slice(0, maxDigits);
      setError(`Máximo ${maxDigits} números permitidos.`);
    } else {
      if (String(e.target.value) !== value && value.length > 0) setError(mensaje);
      else clearError();
    }

    e.target.value = value;
    if (!value) clearError();
  });
}

// =============================================
// MODAL ABRIR CAJA
// =============================================
document.addEventListener("DOMContentLoaded", () => {
  const modalAbrirCaja = document.getElementById("modalAbrirCaja");
  const inputSede = document.getElementById("abrirCajaSede");
  const inputMonto = document.getElementById("abrirCajaMonto");
  const errorMonto = document.getElementById("errorMonto");

  const btnAbrirCaja = document.getElementById("btn-abrir-caja");
  const btnCancelarAbrirCaja = document.getElementById("btnCancelarAbrirCaja");
  const formAbrirCaja = document.getElementById("formAbrirCaja");

  // ✅ aplicar validación al monto inicial
  aplicarValidacionSoloNumeros(inputMonto, {
    maxDigits: 10,
    mensaje: "Solo se permiten números (máx 10)."
  });

  btnAbrirCaja?.addEventListener("click", () => {
    errorMonto?.classList.add("oculto");
    inputMonto.value = "";

    const sedeId = sessionStorage.getItem("sedeId");
    if (!sedeId) {
      showModal("No se encontró el ID de la sede en la sesión.");
      return;
    }

    inputSede.value = `Sede ID: ${sedeId}`;
    modalAbrirCaja.classList.remove("oculto");
  });

  btnCancelarAbrirCaja?.addEventListener("click", () => {
    modalAbrirCaja.classList.add("oculto");
  });

  formAbrirCaja?.addEventListener("submit", async (e) => {
    e.preventDefault();

    const montoStr = (inputMonto.value || "").trim();
    const monto = Number(montoStr);

    if (!montoStr || !Number.isFinite(monto) || monto <= 0) {
      errorMonto?.classList.remove("oculto");
      inputMonto.classList.add("input-error");
      return;
    } else {
      errorMonto?.classList.add("oculto");
      inputMonto.classList.remove("input-error");
    }

    const sedeId = sessionStorage.getItem("sedeId");
    if (!sedeId) {
      await showModal("No se encontró sedeId en la sesión.");
      return;
    }

    const payload = { sedeId: parseInt(sedeId), montoInicial: monto };

    try {
      const token = sessionStorage.getItem("token");
      const res = await fetch(`${API_URL}/Caja/abrir`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify(payload)
      });

      if (!res.ok) {
        const errorText = await res.text();
        showModal("⚠ Error: " + errorText);
        return;
      }

      await showModal("Caja abierta exitosamente.");
      modalAbrirCaja.classList.add("oculto");
      window.location.reload();
    } catch (err) {
      console.error(err);
      await showModal("Error de comunicación con el servidor.");
    }
  });
});

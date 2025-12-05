document.addEventListener("DOMContentLoaded", () => {
  const btnArquearCaja = document.getElementById("btn-arquear-caja");
  if (btnArquearCaja) btnArquearCaja.addEventListener("click", abrirModalMontosReales);

  // ✅ aplicar validación tipo producto a los 3 inputs
  aplicarValidacionSoloNumeros(document.getElementById("realEfectivo"), { maxDigits: 10, mensaje: "Solo números (máx 10)." });
  aplicarValidacionSoloNumeros(document.getElementById("realTarjeta"), { maxDigits: 10, mensaje: "Solo números (máx 10)." });
  aplicarValidacionSoloNumeros(document.getElementById("realTransferencia"), { maxDigits: 10, mensaje: "Solo números (máx 10)." });
});

/* ========= ✅ Validación estilo productos ========= */
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

// ======================================
//  MODAL 1 → INGRESO DE MONTOS REALES
// ======================================
function abrirModalMontosReales() {
  const modal = document.getElementById("modalArqueoReales");
  modal.classList.remove("oculto");
}

document.addEventListener("click", (e) => {
  if (e.target.id === "btnCancelarArqueoReales") cerrarModal("modalArqueoReales");
  if (e.target.id === "btnConfirmarArqueoReales") procesarArqueo();
  if (e.target.id === "btnCerrarResultadoArqueo") cerrarModal("modalResultadoArqueo");
});

function getMontosReales() {
  return {
    efectivo: Number(document.getElementById("realEfectivo").value) || 0,
    tarjeta: Number(document.getElementById("realTarjeta").value) || 0,
    transferencia: Number(document.getElementById("realTransferencia").value) || 0
  };
}

async function procesarArqueo() {
  const token = sessionStorage.getItem("token");
  const sedeId = sessionStorage.getItem("sedeId");

  if (!token || !sedeId) {
    mostrarModalMensaje("Debe iniciar sesión.");
    return;
  }

  const reales = getMontosReales();

  const res = await fetch(`${API_URL}/Caja/abierta/metodos/${sedeId}`, {
    headers: { "Authorization": `Bearer ${token}`, "Content-Type": "application/json" }
  });

  if (!res.ok) {
    mostrarModalMensaje("Error al consultar los montos de la caja.");
    return;
  }

  const teoricos = await res.json();

  const faltantes = {
    efectivo: reales.efectivo - teoricos.efectivo,
    tarjeta: reales.tarjeta - teoricos.tarjeta,
    transferencia: reales.transferencia - teoricos.transferencia
  };

  llenarModalResultado(reales, teoricos, faltantes);
  cerrarModal("modalArqueoReales");
  abrirModal("modalResultadoArqueo");
}

function llenarModalResultado(reales, teoricos, faltantes) {
  const money = (n) => `$${Number(n).toLocaleString("es-CO")}`;

  document.getElementById("resRealEfectivo").textContent = money(reales.efectivo);
  document.getElementById("resRealTarjeta").textContent = money(reales.tarjeta);
  document.getElementById("resRealTransferencia").textContent = money(reales.transferencia);

  document.getElementById("resTeoricoEfectivo").textContent = money(teoricos.efectivo);
  document.getElementById("resTeoricoTarjeta").textContent = money(teoricos.tarjeta);
  document.getElementById("resTeoricoTransferencia").textContent = money(teoricos.transferencia);

  document.getElementById("resFaltaEfectivo").textContent = money(faltantes.efectivo);
  document.getElementById("resFaltaTarjeta").textContent = money(faltantes.tarjeta);
  document.getElementById("resFaltaTransferencia").textContent = money(faltantes.transferencia);
}

function abrirModal(id) { document.getElementById(id).classList.remove("oculto"); }
function cerrarModal(id) { document.getElementById(id).classList.add("oculto"); }

function mostrarModalMensaje(mensaje) {
  const modal = document.getElementById("modalMensaje");
  const text = document.getElementById("modalMensajeTexto");
  text.textContent = mensaje;
  modal.classList.remove("oculto");
}

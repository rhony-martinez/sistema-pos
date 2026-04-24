﻿import { aplicarValidacionesGlobales } from "./validaciones_ventas.js";
//const API_URL = "http://localhost:5289/api";
//const headers = { "Content-Type": "application/json" };

const tbody = document.querySelector("#tb-items tbody");
const btnAgregar = document.getElementById("btn-agregar");
const btnConfirmar = document.getElementById("btn-confirmar");
const btnCancelar = document.getElementById("btn-cancelar");

const modal = document.getElementById("modal");
const modalText = document.getElementById("modal-text");
const modalBtn = document.getElementById("modal-btn");

const modalCantidad = document.getElementById("modal-cantidad");
const inputCantidad = document.getElementById("input-cantidad");
const btnCantOK = document.getElementById("btn-cant-ok");
const btnCantCancelar = document.getElementById("btn-cant-cancelar");

// ===============================
// CARGAR VALIDACIONES
// ===============================
document.addEventListener("DOMContentLoaded", () => {
  aplicarValidacionesGlobales();
});

// ===============================
// OBTENER CAJA ABIERTA
// ===============================
async function obtenerCajaAbierta() {
  const token = sessionStorage.getItem("token");
  const sedeId = sessionStorage.getItem("sedeId");

  const res = await fetch(`${API_URL}/Caja/abierta/detalle/${sedeId}`, {
    headers: {
      "Content-Type": "application/json",
      "Authorization": `Bearer ${token}`
    }
  });

  // Si la respuesta es error (404, 500, etc.)
  if (!res.ok) {
    console.warn("Caja no encontrada o cerrada:", res.status);
    return null;
  }

  // 🔥 Aqui está la diferencia clave:
  const raw = await res.text();   // <- obtenemos texto primero

  // Si viene vacío → caja cerrada → retornar null
  if (!raw) return null;

  try {
    return JSON.parse(raw);       // devolver JSON válido
  } catch (e) {
    console.error("Error parseando JSON de caja abierta:", e, raw);
    return null;
  }
}


// ===============================
// MODAL MENSAJES
// ===============================
function showModal(text, callback) {
  modalText.innerHTML = text;
  modal.style.display = "flex";
  modalBtn.onclick = () => {
    modal.style.display = "none";
    if (callback) callback();
  };
}

// ===============================
// MODAL CANTIDAD
// ===============================
function pedirCantidad(producto, onConfirm) {
  modalCantidad.style.display = "flex";
  inputCantidad.value = 1;

  btnCantOK.onclick = () => {
    const val = parseFloat(inputCantidad.value);
    if (isNaN(val) || val <= 0) return;
    modalCantidad.style.display = "none";
    onConfirm(val);
  };

  btnCantCancelar.onclick = () => {
    modalCantidad.style.display = "none";
  };
}

// ===============================
// RECALCULAR TOTALES
// ===============================
function recalc() {
  let subtotal = 0;
  tbody.querySelectorAll("tr").forEach(tr => {
    const cantidad = parseFloat(tr.querySelector(".cant").value || "0");
    const precio = parseFloat(tr.querySelector(".precio").value || "0");
    const sub = cantidad * precio;
    subtotal += sub;
    tr.querySelector(".sub").textContent = `$${sub.toLocaleString()}`;
  });

  const ivaTotal = subtotal * 0.19;
  const total = subtotal;

  document.getElementById("r-subtotal").textContent = `$${subtotal.toLocaleString()}`;
  document.getElementById("r-iva").textContent = `$${ivaTotal.toLocaleString()}`;
  document.getElementById("r-total").textContent = `$${total.toLocaleString()}`;

  actualizarCambio();
}

// ===============================
// CALCULAR CAMBIO EFECTIVO
// ===============================
function parseCurrencyToNumber(str) {
  if (!str) return 0;
  const cleaned = String(str).replace(/[^0-9-]/g, "");
  return cleaned === "" ? 0 : parseFloat(cleaned);
}

function actualizarCambio() {
  const metodoPago = document.querySelector('input[name="mpago"]:checked').value;
  const totalTexto = document.getElementById("r-total").textContent || "";
  const total = parseCurrencyToNumber(totalTexto);
  const montoRecibidoTexto = document.getElementById("monto_recibido").value || "";
  const montoRecibido = parseCurrencyToNumber(montoRecibidoTexto);

  const campoCambio = document.getElementById("monto_cambio");
  if (!campoCambio) return;

  if (metodoPago === "Efectivo") {
    const cambio = montoRecibido - total;
    campoCambio.value = cambio > 0
      ? `$${Math.round(cambio).toLocaleString("es-CO")}`
      : "$0";
  } else {
    campoCambio.value = "$0";
  }
}

// ===============================
// MOSTRAR / OCULTAR PANEL EFECTIVO
// ===============================
document.querySelectorAll('input[name="mpago"]').forEach(radio => {
  radio.addEventListener("change", () => {
    const metodo = document.querySelector('input[name="mpago"]:checked').value;
    const panelEfectivo = document.getElementById("panel-efectivo");
    panelEfectivo.style.display = metodo === "Efectivo" ? "block" : "none";
  });
});

document.getElementById("monto_recibido").addEventListener("input", actualizarCambio);

// ===============================
// BUSCAR PRODUCTO
// ===============================
async function buscarProductoPorId(id) {
  const res = await fetch(`${API_URL}/Producto/${id}`);
  if (!res.ok) return null;
  return await res.json();
}

// ===============================
// AGREGAR PRODUCTO
// ===============================
btnAgregar.addEventListener("click", async () => {
  const q = document.getElementById("busqueda").value.trim();
  if (!q) return showModal("Ingrese el ID del producto para agregarlo");

  const producto = await buscarProductoPorId(q);
  if (!producto) return showModal("No se encontró el producto con ese ID");

  pedirCantidad(producto, (cantidad) => {
    const tr = document.createElement("tr");
    tr.dataset.proId = producto.proId;
    tr.innerHTML = `
      <td>${producto.proNombre}</td>
      <td><input type="number" class="cant" value="${cantidad}" min="1" step="1" style="width:60px;text-align:center"></td>
      <td><input type="number" class="precio" value="${producto.proPrecioVenta}" readonly style="width:80px;text-align:right;background:#f5f5f5;border:1px solid #ccc"></td>
      <td>19%</td>
      <td class="sub">$${(producto.proPrecioVenta * cantidad).toLocaleString()}</td>
      <td><button class="rm">❌</button></td>
    `;
    tbody.appendChild(tr);
    recalc();

    const inputCant = tr.querySelector(".cant");
    inputCant.addEventListener("input", () => {
      inputCant.value = inputCant.value.replace(/[^0-9]/g, "");
      if (inputCant.value === "" || parseInt(inputCant.value) <= 0) {
        inputCant.value = 1;
      }
      recalc();
    });
  });
});

// ===============================
// ELIMINAR PRODUCTO / CAMBIOS
// ===============================
tbody.addEventListener("input", recalc);
tbody.addEventListener("click", (e) => {
  if (e.target.classList.contains("rm")) {
    e.target.closest("tr").remove();
    recalc();
  }
});

// ===============================
// CONFIRMAR VENTA
// ===============================
btnConfirmar.addEventListener("click", async () => {
  const filas = tbody.querySelectorAll("tr");
  if (filas.length === 0) return showModal("Debe agregar al menos un producto");

  const metodoPago = document.querySelector('input[name="mpago"]:checked').value;
  const observaciones = document.getElementById("observaciones").value;
  const cedulaInput = document.getElementById("cliente_documento");
  const cedula = cedulaInput?.value.trim();

  if (!cedula) {
    cedulaInput?.classList.add("input-error");
    return showModal("Debe ingresar la cédula del cliente antes de registrar la venta.");
  }

  const detalles = Array.from(filas).map(tr => ({
    proId: parseInt(tr.dataset.proId),
    detCantidad: parseFloat(tr.querySelector(".cant").value),
    detPrecioUnitario: parseFloat(tr.querySelector(".precio").value)
  }));

  const venTotal = detalles.reduce((sum, d) => sum + (d.detCantidad * d.detPrecioUnitario), 0);

  if (metodoPago === "Efectivo") {
    const montoRecibidoTexto = document.getElementById("monto_recibido").value.trim();
    const montoRecibido = parseFloat(montoRecibidoTexto.replace(/[^\d.-]/g, "")) || 0;

    if (montoRecibido === 0 || isNaN(montoRecibido)) {
      document.getElementById("monto_recibido").classList.add("input-error");
      return showModal("Debe ingresar el monto recibido antes de confirmar la venta.");
    }

    if (montoRecibido < venTotal) {
      document.getElementById("monto_recibido").classList.add("input-error");
      return showModal("El monto recibido no cubre el total de la venta.");
    }
  }

  const caja = await obtenerCajaAbierta();

  if (!caja) {
      return showModal("❌ Primero debes abrir caja antes de registrar una venta.");
  }


  if (!caja) {
    return showModal("❌ Primero debes abrir caja antes de registrar una venta.");
  }

  const venta = {
    venMetodoPago: metodoPago,
    cajaId: caja.cajaId, 
    venTotal,
    detalles,
    venObservaciones: observaciones,
    clienteDocumento: cedula
  };

  try {
    const res = await fetch(`${API_URL}/Venta`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${sessionStorage.getItem("token")}`
      },
      body: JSON.stringify(venta)
    });

    const json = await res.json();
    if (!res.ok) throw new Error(json?.message || "Error al registrar la venta");

    modalText.innerHTML = "✅ ¡Venta registrada exitosamente!";
    modalBtn.style.display = "none";

    const btnContainer = document.createElement("div");
    btnContainer.style.marginTop = "10px";
    btnContainer.style.display = "flex";
    btnContainer.style.gap = "10px";
    btnContainer.style.justifyContent = "center";

    const btnAceptar = document.createElement("button");
    btnAceptar.textContent = "Aceptar";
    btnAceptar.className = "btn-azul";
    btnAceptar.onclick = () => {
      modal.style.display = "none";
      window.location.href = "gestion-caja.html";
    };

    const btnFactura = document.createElement("button");
    btnFactura.textContent = "Mostrar factura";
    btnFactura.className = "btn-blanco";
    btnFactura.onclick = async () => {
      modal.style.display = "none";
      await mostrarFactura();
    };

    btnContainer.appendChild(btnAceptar);
    btnContainer.appendChild(btnFactura);
    modalText.appendChild(btnContainer);
    modal.style.display = "flex";

  } catch (err) {
    showModal(`❌ Error: ${err.message}`);
  }
});

// ===============================
// CANCELAR
// ===============================
btnCancelar.addEventListener("click", () => {
  window.location.href = "gestion-caja.html";
});

// ===============================
// MOSTRAR FACTURA
// ===============================
async function mostrarFactura() {
  const token = sessionStorage.getItem("token");
  const res = await fetch(`${API_URL}/Venta`, {
    method: "GET",
    headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
    }
  });
  if (!res.ok) return showModal("Error al obtener las ventas");

  const ventas = await res.json();
  if (!ventas.length) return showModal("No hay ventas registradas");

  const ultima = ventas[ventas.length - 1];
  const detalles = ultima.detalles || [];

  const fecha = new Date(ultima.fechaVenta);
  document.getElementById("factura-hora").textContent = fecha.toLocaleTimeString("es-CO", { hour: '2-digit', minute: '2-digit' });
  document.getElementById("factura-fecha").textContent = fecha.toLocaleDateString("es-CO");

  const tbodyFactura = document.getElementById("factura-detalles");
  tbodyFactura.innerHTML = "";
  detalles.forEach(d => {
    const tr = document.createElement("tr");
    tr.innerHTML = `
      <td>${d.detCantidad}</td>
      <td>${d.producto?.proNombre || '—'}</td>
      <td>$${d.detSubtotal.toLocaleString()}</td>
    `;
    tbodyFactura.appendChild(tr);
  });

  document.getElementById("factura-total").textContent = `$ ${ultima.venTotal.toLocaleString()}`;

  const modalFactura = document.getElementById("modal-factura");
  modalFactura.style.display = "flex";

  document.getElementById("btn-factura-cerrar").onclick = () => {
    modalFactura.style.display = "none";
    window.location.href = "gestion-caja.html";
  };
}
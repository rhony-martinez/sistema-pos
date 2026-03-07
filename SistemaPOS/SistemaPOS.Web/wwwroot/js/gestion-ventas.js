document.addEventListener("DOMContentLoaded", () => {
  const token = sessionStorage.getItem("token");
  if (!token) {
    showModal("Debes iniciar sesión.");
    window.location.href = "index.html";
    return;
  }

  const desdeEl = document.getElementById("fecha-desde");
  const hastaEl = document.getElementById("fecha-hasta");

  // ✅ límites requeridos
  const MIN_DESDE = "2025-05-31";
  const hoy = new Date();
  const HOY_ISO = isoDate(hoy);

  if (desdeEl) {
    desdeEl.min = MIN_DESDE;
    desdeEl.max = HOY_ISO;
  }
  if (hastaEl) {
    hastaEl.min = MIN_DESDE;
    hastaEl.max = HOY_ISO;
  }

  // Defaults últimos 30, respetando min/max
  const hace30 = new Date();
  hace30.setDate(hoy.getDate() - 30);

  if (desdeEl && !desdeEl.value) desdeEl.value = isoDate(hace30);
  if (hastaEl && !hastaEl.value) hastaEl.value = HOY_ISO;

  // forzar a rango válido al cargar
  normalizarRangoFechas(desdeEl, hastaEl, MIN_DESDE, HOY_ISO);

  cargarVentas();

  if (desdeEl) desdeEl.addEventListener("change", () => {
    normalizarRangoFechas(desdeEl, hastaEl, MIN_DESDE, HOY_ISO);
    cargarVentas();
  });

  if (hastaEl) hastaEl.addEventListener("change", () => {
    normalizarRangoFechas(desdeEl, hastaEl, MIN_DESDE, HOY_ISO);
    cargarVentas();
  });

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
  if (!iso) return null;
  const [y, m, d] = iso.split("-").map(Number);
  return new Date(y, m - 1, d, 0, 0, 0, 0);
}

function parseISOToDateEnd(iso) {
  if (!iso) return null;
  const [y, m, d] = iso.split("-").map(Number);
  return new Date(y, m - 1, d, 23, 59, 59, 999);
}

/* ========= ✅ Error pequeño debajo (estilo productos) ========= */
function attachErrorSpan(input) {
  if (!input) return null;
  if (input.dataset.errorSpan === "true") {
    // ya existe
    return input.nextElementSibling?.classList?.contains("error-msg") ? input.nextElementSibling : null;
  }
  input.dataset.errorSpan = "true";

  const s = document.createElement("small");
  s.classList.add("error-msg");
  s.style.color = "red";
  s.style.display = "none";
  s.style.fontSize = "0.8rem";
  s.style.marginTop = "2px";
  input.insertAdjacentElement("afterend", s);
  return s;
}

function setInputError(input, msg) {
  const s = attachErrorSpan(input);
  if (!input || !s) return;
  s.textContent = msg;
  s.style.display = "block";
  input.classList.add("input-error");
}

function clearInputError(input) {
  const s = attachErrorSpan(input);
  if (!input || !s) return;
  s.textContent = "";
  s.style.display = "none";
  input.classList.remove("input-error");
}

function normalizarRangoFechas(desdeEl, hastaEl, minIso, maxIso) {
  if (!desdeEl || !hastaEl) return;

  // limpiar errores
  clearInputError(desdeEl);
  clearInputError(hastaEl);

  // clamp a min/max
  if (desdeEl.value && desdeEl.value < minIso) {
    desdeEl.value = minIso;
    setInputError(desdeEl, `La fecha mínima es ${minIso}.`);
  }
  if (hastaEl.value && hastaEl.value < minIso) {
    hastaEl.value = minIso;
    setInputError(hastaEl, `La fecha mínima es ${minIso}.`);
  }

  if (desdeEl.value && desdeEl.value > maxIso) {
    desdeEl.value = maxIso;
    setInputError(desdeEl, `No puedes seleccionar una fecha mayor a hoy (${maxIso}).`);
  }
  if (hastaEl.value && hastaEl.value > maxIso) {
    hastaEl.value = maxIso;
    setInputError(hastaEl, `No puedes seleccionar una fecha mayor a hoy (${maxIso}).`);
  }

  // desde <= hasta
  if (desdeEl.value && hastaEl.value && desdeEl.value > hastaEl.value) {
    setInputError(desdeEl, `La fecha "desde" no puede ser mayor que "hasta".`);
    setInputError(hastaEl, `La fecha "hasta" no puede ser menor que "desde".`);
  }
}

function rangoFechasEsValido() {
  const desdeEl = document.getElementById("fecha-desde");
  const hastaEl = document.getElementById("fecha-hasta");
  if (!desdeEl || !hastaEl) return true;

  // si hay un error visible, no proceses
  const s1 = desdeEl.nextElementSibling;
  const s2 = hastaEl.nextElementSibling;
  const err1 = s1 && s1.classList?.contains("error-msg") && s1.style.display !== "none";
  const err2 = s2 && s2.classList?.contains("error-msg") && s2.style.display !== "none";
  return !(err1 || err2);
}

async function cargarVentas() {
  const tbody = document.getElementById("ventas-tbody");
  const token = sessionStorage.getItem("token");

  if (!tbody) return;

  // ✅ si el rango tiene error, no cargues
  if (!rangoFechasEsValido()) {
    tbody.innerHTML = `<tr><td colspan="6">Corrige el rango de fechas para filtrar.</td></tr>`;
    return;
  }

  const desdeStr = document.getElementById("fecha-desde")?.value;
  const hastaStr = document.getElementById("fecha-hasta")?.value;

  const desde = parseISOToDateStart(desdeStr);
  const hasta = parseISOToDateEnd(hastaStr);

  if (!desde || !hasta) {
    tbody.innerHTML = `<tr><td colspan="6">Selecciona un rango de fechas válido.</td></tr>`;
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

    const filtradas = (ventas || []).filter(v => {
      const f = new Date(v.fechaVenta);
      if (isNaN(f.getTime())) return false;
      return f >= desde && f <= hasta;
    });

    if (!filtradas.length) {
      tbody.innerHTML = `<tr><td colspan="5">No hay ventas para mostrar.</td></tr>`;
      return;
    }

    filtradas.sort((a, b) => new Date(b.fechaVenta) - new Date(a.fechaVenta));

    tbody.innerHTML = "";
    for (const v of filtradas) {
      const tr = document.createElement("tr");
      tr.innerHTML = `
        <td>#V-${v.venId}</td>
        <td>${formatFecha(v.fechaVenta)}</td>
        <td>${money(v.venTotal)}</td>
        <td>${v.venMetodoPago || "—"}</td>
        <td>
          <button class="btn btn-action btn-danger" data-action="eliminar" data-id="${v.venId}">
            <i class="fas fa-trash-alt"></i>
          </button>
        </td>
      `;
      tbody.appendChild(tr);
    }

    tbody.onclick = async (e) => {
      const btn = e.target.closest("button");
      if (!btn) return;

      const action = btn.dataset.action;
      const id = btn.dataset.id;

      if (action === "eliminar") {
        const ok = await showConfirm(
          `¿Seguro que deseas eliminar la venta #${id}? Esta acción NO se puede deshacer.`,
          "Sí, eliminar",
          "Cancelar"
        );
        if (!ok) return;

        try {
          const res = await fetch(`${API_URL}/Venta/${id}`, {
            method: "DELETE",
            headers: {
              "Authorization": `Bearer ${token}`,
              "Content-Type": "application/json"
            }
          });

          const json = await res.json().catch(() => null);

          if (!res.ok) {
            showModal(json?.mensaje || "No se pudo eliminar la venta.");
            return;
          }

          showModal("Venta eliminada correctamente.");
          await cargarVentas();
        } catch (err) {
          console.error(err);
          showModal("Error de conexión eliminando la venta.");
        }
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
    showModal("Debes iniciar sesión.");
    return;
  }

  // ✅ si rango inválido, no generes
  if (!rangoFechasEsValido()) {
    showModal("Corrige el rango de fechas antes de generar el informe.");
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
    showModal(err?.mensaje || "Error generando reporte");
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

/* ===== Modales (los tuyos) ===== */
function showModal(text = "Mensaje") {
  const modal = document.getElementById("ui-modal");
  const p = document.getElementById("ui-text");
  const ok = document.getElementById("ui-ok");

  if (!modal || !p || !ok) { alert(text); return; }
  p.textContent = text;

  const hide = () => modal.classList.add("hidden");
  ok.onclick = (e) => { e.stopPropagation(); hide(); };
  modal.onclick = (e) => { if (e.target === modal) hide(); };

  modal.classList.remove("hidden");
}

function showConfirm(text = "¿Seguro?", yesText = "Sí", noText = "Cancelar") {
  return new Promise((resolve) => {
    const modal = document.getElementById("ui-confirm");
    const p = document.getElementById("ui-ctext");
    const yes = document.getElementById("ui-yes");
    const cancel = document.getElementById("ui-cancel");

    if (!modal || !p || !yes || !cancel) { resolve(confirm(text)); return; }

    p.textContent = text;
    yes.textContent = yesText;
    cancel.textContent = noText;

    const done = (v) => {
      modal.classList.add("hidden");
      resolve(v);
    };

    yes.onclick = (e) => { e.stopPropagation(); done(true); };
    cancel.onclick = (e) => { e.stopPropagation(); done(false); };
    modal.onclick = (e) => { if (e.target === modal) done(false); };

    modal.classList.remove("hidden");
  });
}

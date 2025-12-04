let productosGlobal = [];

document.addEventListener("DOMContentLoaded", () => {
  cargarProductos();

  const buscador = document.getElementById("buscarProducto");
  if (buscador) {
    buscador.addEventListener("input", filtrarProductos);
  }

  // ===== Modal registrar producto =====
  const btnCargar = document.getElementById("btnCargarProducto");
  const modal = document.getElementById("modal-registrar-producto");
  const cancelBtn = document.getElementById("cancelarModal");
  const form = document.getElementById("form-registrar-producto");

  if (btnCargar) btnCargar.addEventListener("click", abrirModal);
  if (cancelBtn) cancelBtn.addEventListener("click", cerrarModal);

  window.addEventListener("click", (e) => {
    if (e.target === modal) cerrarModal();
  });

  cargarCategorias();
  cargarProductos();

  if (form) {
    form.addEventListener("submit", async (e) => {
      e.preventDefault();
      await registrarProducto();
    });
  }

  // ===== Delegación: botón editar precio =====
  const tbody = document.getElementById("tabla-productos");
  if (tbody) {
    tbody.addEventListener("click", (e) => {
      const btn = e.target.closest(".btn-editar");
      if (!btn) return;

      const id = parseInt(btn.dataset.id);
      const producto = productosGlobal.find((p) => p.proId === id);
      if (!producto) return;

      abrirModalEditarPrecio(producto);
    });
  }

  // ===== Modal editar precio =====
  const btnCancelarEdit = document.getElementById("btn-cancelar-editar-precio");
  if (btnCancelarEdit) btnCancelarEdit.addEventListener("click", cerrarModalEditarPrecio);

  const formEdit = document.getElementById("form-editar-precio");
  if (formEdit) formEdit.addEventListener("submit", guardarNuevoPrecio);

  // cerrar modal editar al click afuera
  const modalEdit = document.getElementById("modal-editar-precio");
  window.addEventListener("click", (e) => {
    if (e.target === modalEdit) cerrarModalEditarPrecio();
  });
});

async function cargarCategorias() {
  try {
    const response = await fetch(`${API_URL}/CategoriaProducto`);
    if (!response.ok) throw new Error("Error al obtener las categorías.");
    const categorias = await response.json();

    const select = document.getElementById("catNombre");
    select.innerHTML = `<option value="">Seleccione una categoría...</option>`;
    categorias.forEach((cat) => {
      const opt = document.createElement("option");
      opt.value = cat.catNombre;
      opt.textContent = cat.catNombre;
      select.appendChild(opt);
    });
  } catch (error) {
    console.error(error);
    alert("No fue posible cargar las categorías.");
  }
}

async function registrarProducto() {
  document.querySelectorAll(".input-error").forEach((el) => el.classList.remove("input-error"));

  const campos = ["proNombre", "proDescripcion", "proPrecioVenta", "proUnidad", "catNombre"];
  const vacios = [];

  campos.forEach((id) => {
    const input = document.getElementById(id);
    if (!input.value.trim()) {
      input.classList.add("input-error");
      vacios.push(input);
    }
  });

  if (vacios.length > 0) {
    showModal("Tienes campos obligatorios vacíos. Complétalos antes de continuar.", () => vacios[0].focus());
    return;
  }

  const data = {
    proNombre: document.getElementById("proNombre").value.trim(),
    proDescripcion: document.getElementById("proDescripcion").value.trim(),
    proPrecioVenta: parseFloat(document.getElementById("proPrecioVenta").value),
    proUnidad: document.getElementById("proUnidad").value.trim(),
    catNombre: document.getElementById("catNombre").value.trim(),
  };

  try {
    const response = await fetch(`${API_URL}/Producto`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.detalle || "Error al registrar el producto.");
    }

    showModal("✅ Producto registrado exitosamente.");
    cerrarModal();
    document.getElementById("form-registrar-producto").reset();
    await cargarProductos();
  } catch (error) {
    console.error("Error:", error);
    alert(`❌ ${error.message}`);
  }
}

function abrirModal() {
  const modal = document.getElementById("modal-registrar-producto");
  modal.style.display = "flex";

  setTimeout(() => {
    if (typeof aplicarValidacionesProducto === "function") {
      aplicarValidacionesProducto();
      console.log("Validaciones aplicadas");
    }
  }, 50);
}

function cerrarModal() {
  document.getElementById("modal-registrar-producto").style.display = "none";
}

async function cargarProductos() {
  try {
    const res = await fetch(`${API_URL}/Producto`);
    if (!res.ok) throw new Error("No se pudieron cargar los productos.");

    const productos = await res.json();
    productosGlobal = productos;
    renderizarTabla(productos);
  } catch (err) {
    console.error("❌ Error cargando productos:", err);
  }
}

function renderizarTabla(productos) {
  const tbody = document.getElementById("tabla-productos");
  tbody.innerHTML = "";

  if (!productos || productos.length === 0) {
    tbody.innerHTML = `<tr><td colspan="6" style="text-align:center;">No hay productos registrados.</td></tr>`;
    return;
  }

  productos.forEach((p) => {
    const row = document.createElement("tr");
    row.innerHTML = `
      <td>${p.proId}</td>
      <td>${p.proNombre}</td>
      <td>${p.proDescripcion || "-"}</td>
      <td>${Number(p.proPrecioVenta || 0).toLocaleString("es-CO", { style: "currency", currency: "COP" })}</td>
      <td>${p.catNombre}</td>
      <td class="table-actions">
        <button class="btn btn-action btn-editar" data-id="${p.proId}">
          <i class="fas fa-edit"></i>
        </button>
        <button class="btn btn-action" onclick="eliminarProducto(${p.proId})">
          <i class="fas fa-trash-alt"></i>
        </button>
      </td>
    `;
    tbody.appendChild(row);
  });
}

function filtrarProductos(e) {
  const termino = e.target.value.toLowerCase().trim();

  const filtrados = productosGlobal.filter((p) => p.proNombre.toLowerCase().includes(termino));
  renderizarTabla(filtrados);
}

/* === FUNCIONALIDAD DE ELIMINAR PRODUCTO === */

function eliminarProducto(id) {
  abrirModalConfirmacion(() => ejecutarEliminacion(id));
}

function abrirModalConfirmacion(onConfirm) {
  const modal = document.getElementById("modal-confirmacion");
  modal.classList.remove("oculto");

  document.getElementById("btn-cancelar-eliminacion").onclick = () => {
    modal.classList.add("oculto");
    mostrarModalCancelado();
  };

  document.getElementById("btn-aceptar-eliminacion").onclick = () => {
    modal.classList.add("oculto");
    onConfirm();
  };
}

async function ejecutarEliminacion(id) {
  try {
    const response = await fetch(`${API_URL}/producto/${id}/inactivar`, {
      method: "PUT",
    });

    if (!response.ok) {
      mostrarModalFalloConexion();
      return;
    }

    mostrarModalEliminado();
  } catch (error) {
    mostrarModalFalloConexion();
  }
}

function mostrarModalCancelado() {
  const modal = document.getElementById("modal-cancelado");
  modal.classList.remove("oculto");

  document.getElementById("btn-cerrar-cancelado").onclick = () => {
    modal.classList.add("oculto");
  };
}

function mostrarModalEliminado() {
  const modal = document.getElementById("modal-eliminado");
  modal.classList.remove("oculto");

  document.getElementById("btn-cerrar-eliminado").onclick = () => {
    modal.classList.add("oculto");
    cargarProductos();
  };
}

function mostrarModalFalloConexion() {
  const modal = document.getElementById("modal-error-conexion");
  modal.classList.remove("oculto");

  document.getElementById("btn-reintentar-eliminacion").onclick = () => {
    modal.classList.add("oculto");
  };
}

// ===============================
// EDITAR PRECIO (MODAL)
// ===============================
function abrirModalEditarPrecio(producto) {
  const modal = document.getElementById("modal-editar-precio");
  if (!modal) {
    console.warn("No existe el modal-editar-precio en el HTML.");
    return;
  }

  document.getElementById("editProId").value = producto.proId;
  document.getElementById("editProNombre").value = producto.proNombre || "";
  document.getElementById("editProPrecio").value = producto.proPrecioVenta || 0;

  document.getElementById("errorEditPrecio")?.classList.add("oculto");
  document.getElementById("editProPrecio")?.classList.remove("input-error");

  modal.classList.remove("oculto");
  modal.style.display = "flex";
}

function cerrarModalEditarPrecio() {
  const modal = document.getElementById("modal-editar-precio");
  if (!modal) return;
  modal.classList.add("oculto");
  modal.style.display = "none";
}

async function guardarNuevoPrecio(e) {
  e.preventDefault();

  const id = parseInt(document.getElementById("editProId").value);
  const precioInput = document.getElementById("editProPrecio");
  const error = document.getElementById("errorEditPrecio");

  const nuevoPrecio = parseFloat(precioInput.value);

  if (!nuevoPrecio || isNaN(nuevoPrecio) || nuevoPrecio <= 0) {
    error?.classList.remove("oculto");
    precioInput.classList.add("input-error");
    return;
  }

  precioInput.classList.remove("input-error");
  error?.classList.add("oculto");

  try {
    const res = await fetch(`${API_URL}/Producto/${id}/precio`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        // Si tu API está protegida, descomenta:
        // "Authorization": `Bearer ${sessionStorage.getItem("token")}`
      },
      body: JSON.stringify({ proPrecioVenta: nuevoPrecio }),
    });

    if (!res.ok) {
      const err = await res.json().catch(() => null);
      showModal(err?.mensaje || "No se pudo actualizar el precio.");
      return;
    }

    showModal("✅ Precio actualizado con éxito.");
    cerrarModalEditarPrecio();
    await cargarProductos();
  } catch (err) {
    console.error(err);
    showModal("❌ Error de conexión actualizando el precio.");
  }
}

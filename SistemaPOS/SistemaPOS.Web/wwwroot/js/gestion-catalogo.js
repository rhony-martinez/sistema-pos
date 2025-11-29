let productosGlobal = [];

document.addEventListener("DOMContentLoaded", () => {
    cargarProductos();

    const buscador = document.getElementById("buscarProducto");
    if (buscador) {
        buscador.addEventListener("input", filtrarProductos);
    }

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
    document.querySelectorAll(".input-error").forEach(el => el.classList.remove("input-error"));

    const campos = ["proNombre", "proDescripcion", "proPrecioVenta", "proUnidad", "catNombre"];
    const vacios = [];
    // Validar campos vacíos
    campos.forEach(id => {
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
        catNombre: document.getElementById("catNombre").value.trim()
        
    };

    console.log("📦 Datos a enviar:", data);

    

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
    } catch (error) {
        console.error("Error:", error);
        alert(`❌ ${error.message}`);
    }
}


function abrirModal() {
    const modal = document.getElementById("modal-registrar-producto");
    modal.style.display = "flex";

    // Esperar un ciclo de renderizado para que los inputs existan en el DOM
    setTimeout(() => {
        if (typeof aplicarValidacionesProducto === "function") {
            aplicarValidacionesProducto();
            console.log("✅ Validaciones de producto aplicadas");
        } else {
            console.error("⚠️ No se encontró la función aplicarValidacionesProducto()");
        }
    }, 50);
}

function cerrarModal() {
    document.getElementById("modal-registrar-producto").style.display = "none";
}

document.addEventListener("DOMContentLoaded", () => {
    
});

async function cargarProductos() {
    try {
        const res = await fetch(`${API_URL}/Producto`);
        if (!res.ok) throw new Error("No se pudieron cargar los productos.");

        const productos = await res.json();
        renderizarTabla(productos);
    } catch (err) {
        console.error("❌ Error cargando productos:", err);
    }
}

function renderizarTabla(productos) {
    const tbody = document.querySelector("table tbody");
    tbody.innerHTML = ""; // limpiar

    if (!productos || productos.length === 0) {
        tbody.innerHTML = `<tr><td colspan="6" style="text-align:center;">No hay productos registrados.</td></tr>`;
        return;
    }

    productos.forEach(p => {
        const row = document.createElement("tr");
        row.innerHTML = `
            <td>${p.proId}</td>
            <td>${p.proNombre}</td>
            <td>${p.proDescripcion || "-"}</td>
            <td>${p.proPrecioVenta.toLocaleString("es-CO", { style: "currency", currency: "COP" })}</td>
            <td>${p.catNombre}</td>
            <td class="table-actions">
                <button class="btn btn-action"><i class="fas fa-edit"></i></button>
                <button class="btn btn-action btn-danger"><i class="fas fa-trash-alt"></i></button>
            </td>
        `;
        tbody.appendChild(row);
    });
}

async function cargarProductos() {
    try {
        const res = await fetch(`${API_URL}/Producto`);
        if (!res.ok) throw new Error("No se pudieron cargar los productos.");

        const productos = await res.json();
        productosGlobal = productos; // guardar lista completa
        renderizarTabla(productos);
    } catch (err) {
        console.error("❌ Error cargando productos:", err);
    }
}

function filtrarProductos(e) {
    const termino = e.target.value.toLowerCase().trim();

    const filtrados = productosGlobal.filter(p =>
        p.proNombre.toLowerCase().includes(termino)
    );

    renderizarTabla(filtrados);
}

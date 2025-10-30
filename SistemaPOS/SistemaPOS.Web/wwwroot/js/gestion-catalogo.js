document.addEventListener("DOMContentLoaded", () => {
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
    const perfil = JSON.parse(localStorage.getItem("userProfile"));
    const sedeId = perfil?.sedeId;

    const data = {
        proNombre: document.getElementById("proNombre").value.trim(),
        proDescripcion: document.getElementById("proDescripcion").value.trim(),
        proPrecioVenta: parseFloat(document.getElementById("proPrecioVenta").value),
        proUnidad: document.getElementById("proUnidad").value.trim(),
        catNombre: document.getElementById("catNombre").selectedOptions[0].text,
        sedeId: sedeId,
    };

    console.log("📦 Datos a enviar:", data);

    if (
        !data.proNombre ||
        isNaN(data.proPrecioVenta) ||
        !data.catNombre ||
        !data.sedeId
    ) {
        alert("Por favor, complete todos los campos obligatorios.");
        return;
    }

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

        alert("✅ Producto registrado exitosamente.");
        cerrarModal();
        document.getElementById("form-registrar-producto").reset();
    } catch (error) {
        console.error("Error:", error);
        alert(`❌ ${error.message}`);
    }
}


function abrirModal() {
    document.getElementById("modal-registrar-producto").style.display = "flex";
}

function cerrarModal() {
    document.getElementById("modal-registrar-producto").style.display = "none";
}

async function cargarProductos() {
    try {
        const sedeId = localStorage.getItem("sedeId") || 1; // ⚠️ temporal
        const response = await fetch(`${API_URL}/Producto/sede/${sedeId}`);
        if (!response.ok) throw new Error("Error al obtener los productos.");

        const productos = await response.json();
        const tbody = document.getElementById("tabla-productos");
        tbody.innerHTML = "";

        productos.forEach((p) => {
            const fila = document.createElement("tr");
            fila.innerHTML = `
                <td>${p.proId}</td>
                <td>${p.proNombre}</td>
                <td>${p.proDescripcion || "-"}</td>
                <td>${p.proPrecioVenta.toLocaleString("es-CO", { style: "currency", currency: "COP" })}</td>
                <td>${p.categoria}</td>
                <td class="table-actions">
                    <button class="btn btn-action" title="Editar"><i class="fas fa-edit"></i></button>
                    <button class="btn btn-action btn-danger" title="Eliminar"><i class="fas fa-trash-alt"></i></button>
                </td>
            `;
            tbody.appendChild(fila);
        });

        if (productos.length === 0) {
            tbody.innerHTML = `<tr><td colspan="6" style="text-align:center; color:#666;">No hay productos en esta sede.</td></tr>`;
        }
    } catch (error) {
        console.error("Error cargando productos:", error);
        alert("No fue posible cargar los productos de la sede.");
    }
}


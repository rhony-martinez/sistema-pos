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
    const data = {
        proNombre: document.getElementById("proNombre").value.trim(),
        proDescripcion: document.getElementById("proDescripcion").value.trim(),
        proPrecioVenta: parseFloat(document.getElementById("proPrecioVenta").value),
        proUnidad: document.getElementById("proUnidad").value.trim(),
        catNombre: document.getElementById("catNombre").value.trim()
    };


    if (!data.proNombre || isNaN(data.proPrecioVenta) || !data.catNombre) {
        alert("Por favor, complete los campos obligatorios.");
        return;
    }

    try {
        const response = await fetch(`${API_URL}/Producto`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
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

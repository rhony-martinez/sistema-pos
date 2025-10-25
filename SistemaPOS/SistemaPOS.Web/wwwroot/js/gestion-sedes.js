document.addEventListener("DOMContentLoaded", async () => {
    const tablaBody = document.querySelector("tbody");

    // URL del backend
    const API_URL = "http://localhost:5289/api/Sede";

    try {
        // Petición al backend
        const response = await fetch(API_URL);

        if (!response.ok) {
            throw new Error(`Error HTTP: ${response.status}`);
        }

        // Convertir la respuesta en JSON
        const sedes = await response.json();

        // Filtrar solo las sedes activas
        const sedesActivas = sedes.filter(s => s.sedeEstado === "ACTIVA");

        // ✅ Ordenar por ID (de menor a mayor)
        sedesActivas.sort((a, b) => a.sedeId - b.sedeId);

        // Limpiar tabla
        tablaBody.innerHTML = "";

        // Si no hay sedes activas
        if (sedesActivas.length === 0) {
            tablaBody.innerHTML = `
                <tr><td colspan="6" style="text-align:center;">No hay sedes activas registradas.</td></tr>
            `;
            return;
        }

        // Llenar la tabla
        sedesActivas.forEach(sede => {
            const fila = document.createElement("tr");
            fila.innerHTML = `
                <td>${sede.sedeId}</td>
                <td>${sede.sedeNombre}</td>
                <td>${sede.sedeDireccion}</td>
                <td>${sede.sedeUbicacion}</td>
                <td>${sede.sedeTelefono}</td>
                <td>
                    <button class="btn btn-action" title="Editar"><i class="fas fa-edit"></i></button>
                    <button class="btn btn-action btn-danger" title="Eliminar"><i class="fas fa-trash-alt"></i></button>
                </td>
            `;
            tablaBody.appendChild(fila);
        });

    } catch (error) {
        console.error("Error al obtener las sedes:", error);
        tablaBody.innerHTML = `
            <tr><td colspan="6" style="text-align:center;color:red;">Error al cargar las sedes.</td></tr>
        `;
    }

    // Botones de navegación
    const btnConsultar = document.querySelector(".btn-filter");
    const btnCrear = document.querySelector(".btn-primary");

    if (btnConsultar) {
        btnConsultar.addEventListener("click", () => {
            window.location.href = "consultar_sede.html";
        });
    }

    if (btnCrear) {
        btnCrear.addEventListener("click", () => {
            window.location.href = "Create.html";
        });
    }
});

document.addEventListener("DOMContentLoaded", async () => {
    const tablaBody = document.querySelector("tbody");
    const botonCrear = document.querySelector(".btn-primary");
    const apiUrl = "http://localhost:5289/api/Usuario/admins-locales";

    // Navegación al crear Admin Local
    botonCrear.addEventListener("click", () => {
        window.location.href = "crear-admin-local.html";
    });

    // Cargar usuarios al iniciar
    try {
        const response = await fetch(apiUrl);
        if (!response.ok) throw new Error("Error al obtener los administradores locales.");
        const usuarios = await response.json();

        // Limpiar tabla antes de llenarla
        tablaBody.innerHTML = "";

        if (usuarios.length === 0) {
            tablaBody.innerHTML = `
                <tr><td colspan="7" style="text-align:center;">No hay administradores locales registrados.</td></tr>
            `;
            return;
        }

        // Llenar tabla dinámicamente
        usuarios.forEach(u => {
            const estadoColor = u.usuEstado === "ACTIVO" ? "text-blue" : "text-red";
            const estadoTexto = u.usuEstado === "ACTIVO" ? "Activo" : "Inactivo";

            const fila = `
                <tr>
                    <td>${u.usuUsername || "-"}</td>
                    <td>${u.usuPrimerNombre || "-"}</td>
                    <td>${u.usuPrimerApellido || "-"}</td>
                    <td>${u.usuCorreo || "-"}</td>
                    <td><i class="fas fa-circle ${estadoColor}"></i> ${estadoTexto}</td>
                    <td>${u.sedeId ?? "-"}</td>
                    <td class="table-actions">
                        <button class="btn btn-action" title="Editar"><i class="fas fa-edit"></i></button>
                        <button class="btn btn-action btn-danger" title="Eliminar"><i class="fas fa-trash-alt"></i></button>
                    </td>
                </tr>
            `;
            tablaBody.insertAdjacentHTML("beforeend", fila);
        });

    } catch (error) {
        console.error("❌ Error cargando usuarios:", error);
        tablaBody.innerHTML = `
            <tr><td colspan="7" style="text-align:center;color:#e63946;">Error al cargar usuarios.</td></tr>
        `;
    }
});
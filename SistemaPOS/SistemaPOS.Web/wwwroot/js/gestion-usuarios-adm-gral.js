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
        // 🔹 Renderizar tabla de admins locales
        function renderUsuarios(usuarios) {
            const tbody = document.querySelector("tbody");
            tbody.innerHTML = "";
        
            usuarios.forEach(usuario => {
                const fila = document.createElement("tr");
            
                // Estado (color e ícono)
                const estadoColor = usuario.usuEstado === "ACTIVO" ? "text-blue" : "text-red";
                const estadoTexto = usuario.usuEstado === "ACTIVO" ? "Activo" : "Inactivo";
            
                fila.innerHTML = `
                    <td>${usuario.usuUsername || "-"}</td>
                    <td>${usuario.usuPrimerNombre || "-"}</td>
                    <td>${usuario.usuPrimerApellido || "-"}</td>
                    <td>${usuario.usuCorreo || "-"}</td>
                    <td><i class="fas fa-circle ${estadoColor}"></i> ${estadoTexto}</td>
                    <td>${usuario.sedeId ?? "-"}</td>
                    <td class="table-actions">
                        <button class="btn btn-action btn-edit" data-id="${usuario.usuId}">
                            <i class="fas fa-edit"></i>
                        </button>
                    </td>
                `;
            
                tbody.appendChild(fila);
            });
        
            // ✅ Añadir evento a los botones de edición
            document.querySelectorAll(".btn-edit").forEach(btn => {
                btn.addEventListener("click", (e) => {
                    const userId = e.currentTarget.getAttribute("data-id");
                    // Redirigir al formulario de modificación con el ID
                    window.location.href = `modificar-adm-local.html?id=${userId}`;
                });
            });
        }

        renderUsuarios(usuarios);


    } catch (error) {
        console.error("❌ Error cargando usuarios:", error);
        tablaBody.innerHTML = `
            <tr><td colspan="7" style="text-align:center;color:#e63946;">Error al cargar usuarios.</td></tr>
        `;
    }
});
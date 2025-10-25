document.addEventListener("DOMContentLoaded", async () => {
    const tablaBody = document.querySelector("tbody");
    const botonCrear = document.querySelector(".btn-primary");
    const searchInput = document.querySelector(".search-bar input"); // 🔍 Input de búsqueda
    const apiUrl = "http://localhost:5289/api/Usuario/admins-locales";

    // Navegación al crear Admin Local
    botonCrear.addEventListener("click", () => {
        window.location.href = "crear-admin-local.html";
    });

    let usuarios = []; // Guardaremos aquí todos los administradores

    // === 1️⃣ Cargar usuarios al iniciar ===
    try {
        const response = await fetch(apiUrl);
        if (!response.ok) throw new Error("Error al obtener los administradores locales.");
        usuarios = await response.json();

        renderUsuarios(usuarios);
    } catch (error) {
        console.error("❌ Error cargando usuarios:", error);
        tablaBody.innerHTML = `
            <tr><td colspan="7" style="text-align:center;color:#e63946;">Error al cargar usuarios.</td></tr>
        `;
    }

    // === 2️⃣ Búsqueda dinámica ===
    searchInput.addEventListener("input", (e) => {
        const filtro = e.target.value.toLowerCase().trim();

        if (filtro === "") {
            renderUsuarios(usuarios);
            return;
        }

        const filtrados = usuarios.filter(u =>
            u.usuPrimerNombre?.toLowerCase().includes(filtro) ||
            u.usuPrimerApellido?.toLowerCase().includes(filtro) ||
            u.usuId?.toString().includes(filtro)
        );

        renderUsuarios(filtrados);
    });

    // === 3️⃣ Función para renderizar tabla ===
    function renderUsuarios(lista) {
        tablaBody.innerHTML = "";

        if (lista.length === 0) {
            tablaBody.innerHTML = `<tr><td colspan="7" style="text-align:center;">No se encontraron resultados.</td></tr>`;
            return;
        }

        lista.forEach(usuario => {
            const estadoColor = usuario.usuEstado === "ACTIVO" ? "text-blue" : "text-red";
            const estadoTexto = usuario.usuEstado === "ACTIVO" ? "Activo" : "Inactivo";

            const fila = `
                <tr>
                    <td>${usuario.usuUsername || "-"}</td>
                    <td>${usuario.usuPrimerNombre || "-"}</td>
                    <td>${usuario.usuPrimerApellido || "-"}</td>
                    <td>${usuario.usuCorreo || "-"}</td>
                    <td><i class="fas fa-circle ${estadoColor}"></i> ${estadoTexto}</td>
                    <td>${usuario.sedeId ?? "-"}</td>
                    <td class="table-actions">
                        <button class="btn btn-action btn-edit" title="Editar" data-id="${usuario.usuId}">
                            <i class="fas fa-edit"></i>
                        </button>
                    </td>
                </tr>
            `;

            tablaBody.insertAdjacentHTML("beforeend", fila);
        });

        // ✅ Evento para botón Editar
        document.querySelectorAll(".btn-edit").forEach(btn => {
            btn.addEventListener("click", (e) => {
                const userId = e.currentTarget.getAttribute("data-id");
                window.location.href = `modificar-adm-local.html?id=${userId}`;
            });
        });
    }
});

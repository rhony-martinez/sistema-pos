document.addEventListener("DOMContentLoaded", async () => {
    const tablaBody = document.querySelector("tbody");
    const botonCrear = document.querySelector(".btn-primary");
    const searchInput = document.querySelector(".search-bar input"); // 🔍 Input de búsqueda
    // const apiUrl = "http://localhost:5289/api/Usuario/admins-locales";
    const customModal = document.getElementById("customModal");
    const modalMessage = document.getElementById("modalMessage");
    const modalButtons = document.getElementById("modalButtons");

    // Navegación al crear Admin Local
    botonCrear.addEventListener("click", () => {
        window.location.href = "crear-admin-local.html";
    });

    let usuarios = []; // Guardaremos aquí todos los administradores

    // === 1️⃣ Cargar usuarios al iniciar ===
    try {
        const response = await fetch(`${API_URL}/Users/admins-locales`);
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
                    <button class="btn btn-action btn-del" 
        title="Desactivar" 
        data-id="${usuario.usuId}"
        ${usuario.usuEstado === "INACTIVO" ? "disabled" : ""}>
        <i class="fas ${usuario.usuEstado === "INACTIVO" ? "fa-ban" : "fa-trash"}"></i>

</button>

                </td>
            </tr>
        `;

        tablaBody.insertAdjacentHTML("beforeend", fila);
    });

    // Botón editar
    document.querySelectorAll(".btn-edit").forEach(btn => {
        btn.addEventListener("click", (e) => {
            const userId = e.currentTarget.getAttribute("data-id");
            window.location.href = `modificar-adm-local.html?id=${userId}`;
        });
    });

// ✅ Evento para botón Desactivar
        document.querySelectorAll(".btn-del").forEach(btn => {
            btn.addEventListener("click", async (e) => {
                const userId = e.currentTarget.getAttribute("data-id");
            
                // 🔵 Reemplazo del confirm()
                const confirmar = await showModal("¿Seguro que deseas desactivar este usuario?", true);
                if (!confirmar) return;
            
                try {
                    const token = sessionStorage.getItem("token");
                
                    const response = await fetch(`${API_URL}/Users/${userId}/desactivar`, {
                        method: "PATCH",
                        headers: {
                            "Authorization": `Bearer ${token}`,
                            "Content-Type": "application/json"
                        }
                    });
                
                    if (!response.ok) throw new Error("No se pudo desactivar el usuario");
                
                    // 🔵 Reemplazo del alert()
                    await showModal("Usuario desactivado correctamente.");
                
                    // Actualizar estado en memoria
                    usuarios = usuarios.map(u =>
                        u.usuId == userId ? { ...u, usuEstado: "INACTIVO" } : u
                    );
                
                    renderUsuarios(usuarios);
                
                } catch (error) {
                    console.error("❌ Error:", error);
                
                    // 🔵 Reemplazo del alert() de error
                    await showModal("Error al desactivar usuario.");
                }
            });
        });
}

// Mostrar modal con opciones
    function showModal(message, showConfirm = false) {
        modalMessage.textContent = message;
    
        modalButtons.innerHTML = showConfirm
            ? `
                <button id="modalCancel" class="btn btn-secondary">Cancelar</button>
                <button id="modalConfirm" class="btn btn-danger">Desactivar</button>`
            : `
                <button id="modalOk" class="btn btn-primary">OK</button>`;
    
        customModal.style.display = "flex";
    
        return new Promise(resolve => {
            if (showConfirm) {
                document.getElementById("modalCancel").onclick = () => {
                    customModal.style.display = "none";
                    resolve(false);
                };
                document.getElementById("modalConfirm").onclick = () => {
                    customModal.style.display = "none";
                    resolve(true);
                };
            } else {
                document.getElementById("modalOk").onclick = () => {
                    customModal.style.display = "none";
                    resolve(true);
                };
            }
        });
    }

});

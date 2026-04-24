document.addEventListener("DOMContentLoaded", async () => {
    if (!checkAuth()) return;

    const profile = await getUserProfile();
    const nameTag = document.querySelector(".user-profile span");
    if (nameTag && profile?.usuPrimerNombre && profile?.usuPrimerApellido) {
        nameTag.textContent = `${profile.usuPrimerNombre} ${profile.usuPrimerApellido}`;
    }

    // Botón para crear cajero
    document.querySelector(".btn-primary").addEventListener("click", () => {
        window.location.href = "crear-cajero.html";
    });

    console.log("Perfil del usuario autenticado:", profile);

    if (!profile?.sedeId) {
        console.error("❌ El perfil no contiene sedeId. Verifica el backend o el token.");
        document.querySelector("tbody").innerHTML = `
            <tr><td colspan="7" style="text-align:center;color:#e63946;">
                No se encontró la sede del usuario autenticado.
            </td></tr>`;
        return;
    }

    const apiUrl = `${API_URL}/Users/cajeros/${profile.sedeId}`;
    const tablaBody = document.querySelector("tbody");
    const searchInput = document.querySelector(".search-bar input"); // 🔍 Input de búsqueda
    let cajeros = []; // 🔹 Guardaremos los cajeros cargados aquí

    try {
        const response = await fetch(apiUrl);
        if (!response.ok) throw new Error("Error al obtener los cajeros de la sede.");
        cajeros = await response.json();

        renderTabla(cajeros);

    } catch (error) {
        console.error("❌ Error cargando cajeros:", error);
        tablaBody.innerHTML = `
            <tr><td colspan="7" style="text-align:center;color:#e63946;">Error al cargar cajeros.</td></tr>
        `;
    }

    // === 🔍 FILTRO DE BÚSQUEDA ===
    searchInput.addEventListener("input", (e) => {
        const filtro = e.target.value.toLowerCase().trim();
        if (filtro === "") {
            renderTabla(cajeros); // Mostrar todos si no hay texto
            return;
        }

        const filtrados = cajeros.filter(c => 
            c.usuPrimerNombre?.toLowerCase().includes(filtro) ||
            c.usuPrimerApellido?.toLowerCase().includes(filtro) ||
            c.usuId?.toString().includes(filtro)
        );

        renderTabla(filtrados);
    });

    // === 🧩 Función para renderizar tabla ===
    function renderTabla(lista) {
        tablaBody.innerHTML = "";

        if (lista.length === 0) {
            tablaBody.innerHTML = `<tr><td colspan="7" style="text-align:center;">No se encontraron resultados.</td></tr>`;
            return;
        }

        lista.forEach(c => {
            const estadoColor = c.usuEstado === "ACTIVO" ? "text-blue" : "text-red";
            const estadoTexto = c.usuEstado === "ACTIVO" ? "Activo" : "Inactivo";

            const fila = `
                <tr>
                    <td>${c.usuUsername || "-"}</td>
                    <td>${c.usuPrimerNombre || "-"}</td>
                    <td>${c.usuPrimerApellido || "-"}</td>
                    <td>${c.usuCorreo || "-"}</td>
                    <td><i class="fas fa-circle ${estadoColor}"></i> ${estadoTexto}</td>
                    <td>${c.sedeId ?? "-"}</td>
                    <td class="table-actions">
                        <button class="btn btn-action btn-edit" title="Editar" data-id="${c.usuId}">
                            <i class="fas fa-edit"></i>
                        </button>
            <button class="btn btn-action btn-del"
                    title="Desactivar"
                    data-id="${c.usuId}"
                    ${c.usuEstado === "INACTIVO" ? "disabled" : ""}>
                <i class="fas ${c.usuEstado === "INACTIVO" ? "fa-ban" : "fa-trash"}"></i>
            </button>
                    </td>
                </tr>`;
            tablaBody.insertAdjacentHTML("beforeend", fila);
        });

        // ✅ Evento para editar cajero
        document.querySelectorAll(".btn-edit").forEach(btn => {
            btn.addEventListener("click", (e) => {
                const userId = e.currentTarget.getAttribute("data-id");
                window.location.href = `modificar-cajero.html?id=${userId}`;
            });
        });

   // ✅ Evento para desactivar cajero con MODAL
        document.querySelectorAll(".btn-del").forEach(btn => {
            btn.addEventListener("click", async (e) => {
                const userId = e.currentTarget.getAttribute("data-id");

                // 🔵 Modal de confirmación (reemplazo de confirm())
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

                    // 🔵 Modal de éxito (reemplazo de alert())
                    await showModal("Usuario desactivado correctamente.");

                    // 🔄 Actualizar el array en memoria
                    cajeros = cajeros.map(c =>
                        c.usuId == userId ? { ...c, usuEstado: "INACTIVO" } : c
                    );

                    // 🔁 Volver a dibujar tabla
                    renderTabla(cajeros);

                } catch (error) {
                    console.error("❌ Error:", error);

                    // 🔵 Modal de error
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
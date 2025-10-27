document.addEventListener("DOMContentLoaded", async () => {
    const tablaBody = document.querySelector("tbody");
    const API_URL = "http://localhost:5289/api/Sede";

    // --- Función genérica para mostrar modal ---
    function showMessage(message, onAccept = null, showCancel = false) {
        const modal = document.getElementById("modal");
        const modalText = document.getElementById("modal-text");
        const modalBtn = document.getElementById("modal-btn");
        const modalCancel = document.getElementById("modal-cancel");

        modalText.textContent = message;
        modal.style.display = "flex";

        // Mostrar u ocultar botón cancelar
        modalCancel.style.display = showCancel ? "inline-block" : "none";

        // Botón aceptar
        modalBtn.onclick = () => {
            modal.style.display = "none";
            if (onAccept) onAccept();
        };

        // Botón cancelar
        modalCancel.onclick = () => {
            modal.style.display = "none";
        };
    }

    // --- Función de confirmación ---
    function confirmDelete(sedeId, onConfirm) {
        showMessage(
            `⚠️ ¿Seguro que quieres eliminar la sede #${sedeId}?`,
            onConfirm,
            true // mostramos el botón cancelar
        );
    }

    // --- Función de éxito ---
    function deleteSuccess() {
        showMessage("✅ Sede eliminada correctamente");
    }

    // --- Función de error ---
    function deleteError(msg) {
        showMessage(`❌ Hubo un error al eliminar la sede: ${msg}`);
    }

    try {
        const response = await fetch(API_URL);
        if (!response.ok) throw new Error(`Error HTTP: ${response.status}`);

        const sedes = await response.json();
        const sedesActivas = sedes.filter(s => s.sedeEstado === "ACTIVA");
        sedesActivas.sort((a, b) => a.sedeId - b.sedeId);

        tablaBody.innerHTML = "";

        if (sedesActivas.length === 0) {
            tablaBody.innerHTML = `
                <tr><td colspan="6" style="text-align:center;">No hay sedes activas registradas.</td></tr>
            `;
            return;
        }

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
                    <button class="btn btn-action btn-danger btn-eliminar" data-id="${sede.sedeId}" title="Eliminar">
                        <i class="fas fa-trash-alt"></i>
                    </button>
                </td>
            `;
            tablaBody.appendChild(fila);
        });

        // --- Evento eliminar con modal ---
        document.querySelectorAll(".btn-eliminar").forEach(btn => {
            btn.addEventListener("click", () => {
                const sedeId = btn.dataset.id;

                confirmDelete(sedeId, async () => {
                    try {
                        const resp = await fetch(`${API_URL}/${sedeId}/inactivar`, {
                            method: 'POST',
                            headers: {
                                'Accept': '*/*',
                                'Content-Type': 'application/json'
                            },
                            body: ''
                        });

                        if (!resp.ok) throw new Error(`Error HTTP: ${resp.status}`);
                        const data = await resp.json();

                        deleteSuccess();

                        // Eliminar fila de la tabla
                        btn.closest("tr").remove();

                    } catch (error) {
                        console.error("Error al eliminar la sede:", error);
                        deleteError(error.message || "desconocido");
                    }
                });
            });
        });

    } catch (error) {
        console.error("Error al obtener las sedes:", error);
        tablaBody.innerHTML = `
            <tr><td colspan="6" style="text-align:center;color:red;">Error al cargar las sedes.</td></tr>
        `;
    }

    // --- Botones de navegación ---
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

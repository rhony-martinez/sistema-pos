document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("formBuscar");

    // 🔹 Crear el modal de error dinámicamente si no existe
    if (!document.getElementById("modalError")) {
        const modalHTML = `
            <div id="modalError" class="modal">
                <div class="modal-content">
                    <h3 id="modalErrorTitulo">Error</h3>
                    <p id="modalErrorMensaje">Mensaje de error</p>
                    <button id="btnCerrarModal" class="btn-modal">Entendido</button>
                </div>
            </div>
        `;
        document.body.insertAdjacentHTML("beforeend", modalHTML);

        const modal = document.getElementById("modalError");
        const btnCerrar = document.getElementById("btnCerrarModal");
        btnCerrar.addEventListener("click", () => modal.style.display = "none");
        modal.addEventListener("click", e => {
            if (e.target === modal) modal.style.display = "none";
        });
    }

    const modal = document.getElementById("modalError");
    const modalTitulo = document.getElementById("modalErrorTitulo");
    const modalMensaje = document.getElementById("modalErrorMensaje");

    function mostrarModal(titulo, mensaje) {
        modalTitulo.textContent = titulo;
        modalMensaje.textContent = mensaje;
        modal.style.display = "flex";
    }

    // 🔹 Evento principal del formulario
    form.addEventListener("submit", async (event) => {
        event.preventDefault();

        const id = document.getElementById("idSede").value.trim();
        const nombre = document.getElementById("nombreSede").value.trim();

        if (!id && !nombre) {
            mostrarModal("Campos vacíos", "Por favor ingresa un ID o un nombre de sede para continuar.");
            return;
        }

        const params = new URLSearchParams();
        if (id) params.append("id", id);
        if (nombre) params.append("nombre", nombre);

        const url = `http://localhost:5289/api/Sede/buscar?${params.toString()}`;
        console.log("🌐 Solicitando:", url);

        try {
            const response = await fetch(url);
            if (!response.ok) {
                if (response.status === 404) {
                    mostrarModal("No encontrada", "No se encontró ninguna sede con los criterios proporcionados.");
                } else {
                    mostrarModal("Error", `Error ${response.status}: ${response.statusText}`);
                }
                return;
            }

            const sede = await response.json();

            localStorage.setItem("sedeEncontrada", JSON.stringify(sede));
            window.location.href = `consultar_sede_resultado.html?id=${sede.SedeId}`;

        } catch (error) {
            console.error("❌ Error en la solicitud:", error);
            mostrarModal("Error del servidor", "Ocurrió un error al procesar la solicitud. Verifica tu conexión o intenta nuevamente.");
        }
    });
});

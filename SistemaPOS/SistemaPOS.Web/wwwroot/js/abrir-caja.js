// ===============================
// MODAL PERSONALIZADO PARA MENSAJES
 // ===============================
function showModal(message, showConfirm = false) {
    modalMessage.textContent = message;  
    modalButtons.innerHTML = showConfirm
    ? `
        <button id="modalCancel" class="btn btn-secondary">Cancelar</button>
        <button id="modalConfirm" class="btn btn-primary">Aceptar</button>
        `
    : `<button id="modalOk" class="btn btn-primary">OK</button>`;

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

// =============================================
// MODAL ABRIR CAJA
// =============================================
const modalAbrirCaja = document.getElementById("modalAbrirCaja");
const inputSede = document.getElementById("abrirCajaSede");
const inputMonto = document.getElementById("abrirCajaMonto");
const errorMonto = document.getElementById("errorMonto");

const btnAbrirCaja = document.getElementById("btn-abrir-caja"); 
const btnCancelarAbrirCaja = document.getElementById("btnCancelarAbrirCaja");

const formAbrirCaja = document.getElementById("formAbrirCaja");


// ==================================================
// 1️⃣ — ABRIR MODAL
// ==================================================
btnAbrirCaja.addEventListener("click", () => {
    errorMonto.classList.add("oculto");
    inputMonto.value = "";

    const sedeId = sessionStorage.getItem("sedeId");

    if (!sedeId) {
        showModal("No se encontró el ID de la sede en la sesión.");
        return;
    }

    // Mostrar solo la sede (si quieres nombre, debes almacenarlo en login)
    inputSede.value = `Sede ID: ${sedeId}`;

    modalAbrirCaja.classList.remove("oculto");
});


// ==================================================
// 2️⃣ — CERRAR MODAL
// ==================================================
btnCancelarAbrirCaja.addEventListener("click", () => {
    modalAbrirCaja.classList.add("oculto");
});


// ==================================================
// 3️⃣ — VALIDAR Y ENVIAR FORM
// ==================================================
formAbrirCaja.addEventListener("submit", async (e) => {
    e.preventDefault();

    const monto = parseFloat(inputMonto.value);

    if (isNaN(monto) || monto <= 0) {
        errorMonto.classList.remove("oculto");
        return;
    } else {
        errorMonto.classList.add("oculto");
    }

    const sedeId = sessionStorage.getItem("sedeId");

    if (!sedeId) {
        await showModal("No se encontró sedeId en la sesión.");
        return;
    }

    const payload = {
        sedeId: parseInt(sedeId),
        montoInicial: monto
    };

    try {
        const token = sessionStorage.getItem("token");

        const res = await fetch(`${API_URL}/Caja/abrir`, {
            method: "POST",
            headers: { 
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify(payload)
        });

        if (!res.ok) {
            const errorText = await res.text();
            showModal("⚠ Error: " + errorText);
            return;
        }

        const data = await res.json();

        await showModal("Caja abierta exitosamente.");
        modalAbrirCaja.classList.add("oculto");
        window.location.reload();

    } catch (err) {
        console.error(err);
        await showModal("Error de comunicación con el servidor.");
    }
});
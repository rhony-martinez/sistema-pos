document.addEventListener("DOMContentLoaded", () => {
    const btnArquearCaja = document.getElementById("btn-arquear-caja"); 

    if (btnArquearCaja) {
        btnArquearCaja.addEventListener("click", abrirModalMontosReales);
    }
});

// ======================================
//  MODAL 1 → INGRESO DE MONTOS REALES
// ======================================
function abrirModalMontosReales() {
    const modal = document.getElementById("modalArqueoReales");
    modal.classList.remove("oculto");
}

document.addEventListener("click", (e) => {
    if (e.target.id === "btnCancelarArqueoReales") {
        cerrarModal("modalArqueoReales");
    }

    if (e.target.id === "btnConfirmarArqueoReales") {
        procesarArqueo();
    }

    if (e.target.id === "btnCerrarResultadoArqueo") {
        cerrarModal("modalResultadoArqueo");
    }
});

// ======================================
//  OBTENER VALORES REALES DEL FORMULARIO
// ======================================
function getMontosReales() {
    return {
        efectivo: parseFloat(document.getElementById("realEfectivo").value) || 0,
        tarjeta: parseFloat(document.getElementById("realTarjeta").value) || 0,
        transferencia: parseFloat(document.getElementById("realTransferencia").value) || 0
    };
}

// ======================================
//  PROCESAR ARQUEO COMPLETO
// ======================================
async function procesarArqueo() {
    console.log("Procesando arqueo...");
    const token = sessionStorage.getItem("token");
    const sedeId = sessionStorage.getItem("sedeId");

    if (!token || !sedeId) {
        mostrarModalMensaje("Debe iniciar sesión.");
        return;
    }

    const reales = getMontosReales();

    // Llamar endpoint nuevo
    const res = await fetch(`${API_URL}/Caja/abierta/metodos/${sedeId}`, {
        headers: {
            "Authorization": `Bearer ${token}`,
            "Content-Type": "application/json"
        }
    });

    if (!res.ok) {
        mostrarModalMensaje("Error al consultar los montos de la caja.");
        return;
    }

    const teoricos = await res.json();

    // Calcular faltantes
    const faltantes = {
        efectivo: reales.efectivo - teoricos.efectivo,
        tarjeta: reales.tarjeta - teoricos.tarjeta,
        transferencia: reales.transferencia - teoricos.transferencia
    };

    // Llenar modal de resultado
    llenarModalResultado(reales, teoricos, faltantes);

    // Cerrar modal 1
    cerrarModal("modalArqueoReales");
    console.log("Abriendo modalResultadoArqueo...");
    // Abrir modal 2
    abrirModal("modalResultadoArqueo");
}

// ======================================
//  LLENAR LA MODAL DE RESULTADO
// ======================================
function llenarModalResultado(reales, teoricos, faltantes) {
    const money = (n) => `$${Number(n).toLocaleString("es-CO")}`;

    // Reales
    document.getElementById("resRealEfectivo").textContent = money(reales.efectivo);
    document.getElementById("resRealTarjeta").textContent = money(reales.tarjeta);
    document.getElementById("resRealTransferencia").textContent = money(reales.transferencia);

    // Teóricos
    document.getElementById("resTeoricoEfectivo").textContent = money(teoricos.efectivo);
    document.getElementById("resTeoricoTarjeta").textContent = money(teoricos.tarjeta);
    document.getElementById("resTeoricoTransferencia").textContent = money(teoricos.transferencia);

    // Faltantes
    document.getElementById("resFaltaEfectivo").textContent = money(faltantes.efectivo);
    document.getElementById("resFaltaTarjeta").textContent = money(faltantes.tarjeta);
    document.getElementById("resFaltaTransferencia").textContent = money(faltantes.transferencia);
}

// ======================================
//  UTILIDADES DE MODALES
// ======================================
function abrirModal(id) {
    document.getElementById(id).classList.remove("oculto");
}

function cerrarModal(id) {
    document.getElementById(id).classList.add("oculto");
}

// ======================================
//  MODAL DE MENSAJES (MISMO ESTILO DE TU APP)
// ======================================
function mostrarModalMensaje(mensaje) {
    const modal = document.getElementById("modalMensaje");
    const text = document.getElementById("modalMensajeTexto");

    text.textContent = mensaje;
    modal.classList.remove("oculto");
}

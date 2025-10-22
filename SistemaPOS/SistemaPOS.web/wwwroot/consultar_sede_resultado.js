document.addEventListener("DOMContentLoaded", async () => {
    // Obtener el parámetro "id" de la URL (por ejemplo, ?id=1)
    const urlParams = new URLSearchParams(window.location.search);
    const id = urlParams.get("id");

    // Referencias a los elementos HTML
    const resultadoDiv = document.getElementById("resultado");
    const volverBtn = document.getElementById("volverBtn");

    // Si no hay ID en la URL
    if (!id) {
        resultadoDiv.innerHTML = `<p style="color:red;">❌ No se proporcionó un ID válido.</p>`;
        return;
    }

    try {
        // Llamar al endpoint del backend
        const response = await fetch(`https://localhost:7209/api/Sede/buscar?id=${id}`);

        if (!response.ok) {
            throw new Error("Error al consultar la sede.");
        }

        const data = await response.json();

        // Si la respuesta está vacía o no existe la sede
        if (!data || Object.keys(data).length === 0) {
            resultadoDiv.innerHTML = `
                <p style="color:red;">❌ No se encontró ninguna sede con el ID ${id}.</p>
            `;
            return;
        }

        // Mostrar datos obtenidos del backend
        resultadoDiv.innerHTML = `
            <p>✅ <strong>Sede encontrada:</strong></p>
            <p><strong>ID:</strong> ${data.sedE_ID ?? "N/A"}</p>
            <p><strong>Nombre:</strong> ${data.sedE_NOMBRE ?? "N/A"}</p>
            <p><strong>Ciudad:</strong> ${data.sedE_CIUDAD ?? "N/A"}</p>
            <p><strong>Departamento:</strong> ${data.sedE_DEPARTAMENTO ?? "N/A"}</p>
            <p><strong>Ubicación:</strong> ${data.sedE_UBICACION ?? "N/A"}</p>
            <p><strong>Correo:</strong> ${data.sedE_CORREO ?? "N/A"}</p>
            <p><strong>Teléfono:</strong> ${data.sedE_TELEFONO ?? "N/A"}</p>
            <p><strong>Estado:</strong> ${data.sedE_ESTADO ?? "N/A"}</p>
        `;
    } catch (error) {
        console.error(error);
        resultadoDiv.innerHTML = `
            <p style="color:red;">⚠️ Ocurrió un error al consultar la sede.</p>
        `;
    }

    // Acción del botón "Volver"
    volverBtn.addEventListener("click", () => {
        window.location.href = "consultar_sede.html";
    });
});

document.addEventListener("DOMContentLoaded", () => {
    console.log("consultar_sede.js cargado correctamente");

    const form = document.getElementById("form-consulta");
    const resultadoDiv = document.getElementById("resultado");

    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        const id = parseInt(document.getElementById("codigoSede").value.trim()) || null;
        const nombre = document.getElementById("nombreSede").value.trim();

        console.log("📤 Datos capturados del formulario:", { id, nombre });

        resultadoDiv.textContent = "Buscando sede...";
        resultadoDiv.style.color = "red";

        try {
            const url = new URL("https://localhost:/api/Sede/buscar");
            if (id) url.searchParams.append("id", id);
            if (nombre) url.searchParams.append("nombre", nombre);

            console.log("🌐 Enviando petición a:", url.toString());

            const response = await fetch(url);

            console.log("📥 Respuesta recibida:", response);

            if (!response.ok) {
                resultadoDiv.textContent = `❌ Error: ${response.status} ${response.statusText}`;
                return;
            }

            const data = await response.json();
            console.log("📦 Datos del servidor:", data);

            resultadoDiv.innerHTML = `
                ✅ <strong>Sede encontrada:</strong><br>
                ID: ${data.sedE_ID}<br>
                Nombre: ${data.sedE_NOMBRE}<br>
                Ciudad: ${data.sedE_CIUDAD || "N/A"}<br>
                Departamento: ${data.sedE_DEPARTAMENTO || "N/A"}<br>
                Estado: ${data.sedE_ESTADO || "N/A"}
            `;
            resultadoDiv.style.color = "green";
        } catch (error) {
            console.error("❌ Error en fetch:", error);
            resultadoDiv.textContent = "Ocurrió un error al consultar la sede.";
        }
    });
});

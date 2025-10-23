document.addEventListener("DOMContentLoaded", () => {
        const resultadoBody = document.getElementById("resultado");
        const sedeData = localStorage.getItem("sedeEncontrada");
    
        if (!sedeData) {
            resultadoBody.innerHTML = `
                <tr><td colspan="8" class="no-data">❌ No hay datos para mostrar.</td></tr>
            `;
            return;
        }
    
        const sede = JSON.parse(sedeData);
        console.log("📦 Sede cargada desde localStorage:", sede);
    
        resultadoBody.innerHTML = `
            <tr>
                <td>${sede.sedeId ?? sede.SedeId ?? "N/A"}</td>
                <td>${sede.sedeNombre ?? sede.SedeNombre ?? "N/A"}</td>
                <td>${sede.sedeCiudad ?? sede.SedeCiudad ?? "N/A"}</td>
                <td>${sede.sedeDepartamento ?? sede.SedeDepartamento ?? "N/A"}</td>
                <td>${sede.sedeDireccion ?? sede.SedeUbicacion ?? "N/A"}</td>
                <td>${sede.sedeCorreo ?? sede.SedeCorreo ?? "N/A"}</td>
                <td>${sede.sedeTelefono ?? sede.SedeTelefono ?? "N/A"}</td>
                <td>${sede.sedeEstado ?? sede.SedeEstado ?? "N/A"}</td>
            </tr>
        `;
    });
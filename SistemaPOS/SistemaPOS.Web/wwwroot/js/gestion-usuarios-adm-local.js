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
    console.log("Perfil del usuario autenticado:", profile); // 👈 Verificar qué trae exactamente

    if (!profile?.sedeId) {
        console.error("❌ El perfil no contiene sedeId. Verifica el backend o el token.");
        document.querySelector("tbody").innerHTML = `
            <tr><td colspan="7" style="text-align:center;color:#e63946;">
                No se encontró la sede del usuario autenticado.
            </td></tr>`;
        return;
    }

    const apiUrl = `http://localhost:5289/api/Usuario/cajeros/${profile.sedeId}`;
    const tablaBody = document.querySelector("tbody");

    try {
        const response = await fetch(apiUrl);
        if (!response.ok) throw new Error("Error al obtener los cajeros de la sede.");

        const cajeros = await response.json();
        tablaBody.innerHTML = "";

        if (cajeros.length === 0) {
            tablaBody.innerHTML = `<tr><td colspan="7" style="text-align:center;">No hay cajeros registrados en esta sede.</td></tr>`;
            return;
        }

        cajeros.forEach(c => {
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
                        <button class="btn btn-action" title="Editar"><i class="fas fa-edit"></i></button>
                        <button class="btn btn-action btn-danger" title="Eliminar"><i class="fas fa-trash-alt"></i></button>
                    </td>
                </tr>`;
            tablaBody.insertAdjacentHTML("beforeend", fila);
        });

    } catch (error) {
        console.error("❌ Error cargando cajeros:", error);
        tablaBody.innerHTML = `
            <tr><td colspan="7" style="text-align:center;color:#e63946;">Error al cargar cajeros.</td></tr>
        `;
    }
});
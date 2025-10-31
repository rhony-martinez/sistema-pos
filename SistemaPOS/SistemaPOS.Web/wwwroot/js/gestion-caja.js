document.addEventListener("DOMContentLoaded", () => {
  // --- BOTÓN CERRAR SESIÓN ---
  const logoutBtn = document.getElementById("logoutBtn");
  //const API_URL = window.API_URL || "http://localhost:5289/api"; // por si viene de config.js

  if (logoutBtn) {
    logoutBtn.addEventListener("click", async () => {
      const token = sessionStorage.getItem("token");

      if (!token) {
        sessionStorage.clear();
        window.location.href = "index.html";
        return;
      }

      try {
        const res = await fetch(`${API_URL}/auth/logout`, {
          method: "POST",
          headers: {
            "Authorization": `Bearer ${token}`,
            "Content-Type": "application/json"
          }
        });

        // Limpia la sesión y redirige sin importar el estado
        sessionStorage.clear();
        window.location.href = "index.html";
      } catch (err) {
        console.error("Error al cerrar sesión:", err);
        alert("Error al cerrar sesión. Intenta nuevamente.");
      }
    });
  }

  // --- BOTÓN CREAR VENTA ---
  const btnCrearVenta = document.getElementById("btn-crear-venta");

  if (btnCrearVenta) {
    btnCrearVenta.addEventListener("click", () => {
      // Verifica si hay sesión
      const token = sessionStorage.getItem("token");

      if (!token) {
        alert("Debe iniciar sesión para crear una venta.");
        window.location.href = "index.html";
        return;
      }

      // Redirige a la página de crear venta
      window.location.href = "crear_venta.html";
    });
  }
});

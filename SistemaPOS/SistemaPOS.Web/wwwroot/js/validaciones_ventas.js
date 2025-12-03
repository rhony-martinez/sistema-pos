// ==================== VALIDACIONES GLOBALES ====================

// Palabras SQL prohibidas
const RESERVED_WORDS = ["SELECT", "INSERT", "UPDATE", "DELETE", "FROM", "WHERE", "GROUP BY", "HAVING", "ORDER BY", "DROP", "CREATE", "ALTER", "TRUNCATE", "EXEC", "UNION", "ALL", "AND", "OR", "NOT", "NULL", "JOIN", "INNER", "LEFT", "RIGHT", "ON", "AS", "INTO", "VALUES", "SET"];
const MAX_LENGTH = 15;
const MAX_EMAIL_LENGTH = 40;

// ==================== MODAL ESTILO SISTEMA POS ====================
export function showModal(message, callback) {
  const modal = document.getElementById("modal");
  const modalText = document.getElementById("modal-text");
  const modalBtn = document.getElementById("modal-btn");

  if (!modal || !modalText || !modalBtn) {
    alert(message);
    if (callback) callback();
    return;
  }

  modalText.textContent = message;
  modal.style.display = "flex";

  modalBtn.onclick = () => {
    modal.style.display = "none";
    if (callback) callback();
  };
}

// ==================== VALIDACIONES UNIVERSALES ====================
export function aplicarValidacionesGlobales() {
  document.querySelectorAll("input, textarea").forEach(field => {
    field.addEventListener("input", () => {
      let value = field.value;

      // Reset de error visual
      field.classList.remove("input-error");

      // No permitir espacio al inicio
      if (/^\s/.test(value)) {
        field.classList.add("input-error");
        field.value = value.trimStart();
        showModal("No se permiten espacios en blanco al inicio.");
        return;
      }

      // No más de 2 espacios seguidos
      if (/\s{2,}/.test(value)) {
        field.classList.add("input-error");
        field.value = value.replace(/\s{2,}/g, " ");
        showModal("No se permiten más de un espacio en blanco seguido.");
        return;
      }

      // Longitud máxima por campo
      const limit =
        field.id === "observaciones"
          ? 70
          : field.id === "monto_recibido"
          ? 8
          : field.id === "cliente_correo"
          ? MAX_EMAIL_LENGTH
          : MAX_LENGTH;

      if (value.length > limit) {
        field.classList.add("input-error");
        field.value = value.slice(0, limit);
        showModal(`Máximo ${limit} caracteres permitidos.`);
        return;
      }

      // Solo números en campos numéricos
      if (
        ["cliente_documento", "monto_recibido", "busqueda", "input-cantidad", "cliente_telefono"].includes(field.id)
      ) {
        if (/[^0-9]/.test(value)) {
          field.classList.add("input-error");
          field.value = value.replace(/[^0-9]/g, "");
          showModal("Solo se permiten números en este campo.");
          return;
        }
      }

      // Bloquear caracteres especiales (excepto espacio y letras/números y algunos símbolos válidos en email)
      if (/[^a-zA-Z0-9\s@._-]/.test(value)) {
        field.classList.add("input-error");
        field.value = value.replace(/[^a-zA-Z0-9\s@._-]/g, "");
        showModal("No se permiten caracteres especiales.");
        return;
      }

      // Bloquear palabras reservadas SQL
      for (const word of RESERVED_WORDS) {
        const regex = new RegExp(`\\b${word}\\b`, "i");
        if (regex.test(value)) {
          field.classList.add("input-error");
          field.value = value.replace(regex, "");
          showModal(`No se permite usar la palabra reservada: ${word}`);
          return;
        }
      }

      // Si pasó todas las validaciones, quitar clase de error
      field.classList.remove("input-error");
    });
  });
}

--------------------------------------------------------
-- SISTEMA POS - DATOS INICIALES
--------------------------------------------------------

-- SEDE
INSERT INTO SEDE (
    SEDE_NOMBRE, SEDE_DIRECCION, SEDE_CIUDAD, SEDE_DEPARTAMENTO,
    SEDE_UBICACION, SEDE_TELEFONO, SEDE_CORREO
) VALUES (
    'Sede Central', 'Av. Principal #123', 'Bogotá', 'Cundinamarca',
    'Centro Comercial El Sol, Local 12', '3201234567', 'central@pos.com'
);

-- CATEGORÍAS DE PRODUCTO
INSERT INTO CATEGORIA_PRODUCTO (CAT_NOMBRE) VALUES ('Herramientas de Construcción');
INSERT INTO CATEGORIA_PRODUCTO (CAT_NOMBRE) VALUES ('Juguetes');
INSERT INTO CATEGORIA_PRODUCTO (CAT_NOMBRE) VALUES ('Papelería');

-- PRODUCTOS (asociados a categorías por CAT_ID)
--️ CAT_ID = 1, 2, 3 según el orden de inserción anterior
INSERT INTO PRODUCTO (PRO_NOMBRE, PRO_DESCRIPCION, PRO_PRECIO_VENTA, PRO_UNIDAD, CAT_ID)
VALUES ('Martillo de acero', 'Martillo de carpintero de 16 oz con mango de goma', 25000, 'unidad', 1);

INSERT INTO PRODUCTO (PRO_NOMBRE, PRO_DESCRIPCION, PRO_PRECIO_VENTA, PRO_UNIDAD, CAT_ID)
VALUES ('Carro de juguete', 'Carro metálico a escala 1:32 con tracción manual', 18000, 'unidad', 2);

INSERT INTO PRODUCTO (PRO_NOMBRE, PRO_DESCRIPCION, PRO_PRECIO_VENTA, PRO_UNIDAD, CAT_ID)
VALUES ('Cuaderno universitario', 'Cuaderno de 100 hojas cuadriculadas tamaño carta', 8000, 'unidad', 3);

-- CATALOGO (la sede central ofrece los 3 productos)
INSERT INTO CATALOGO (SEDE_ID, PRO_ID) VALUES (1, 1);
INSERT INTO CATALOGO (SEDE_ID, PRO_ID) VALUES (1, 2);
INSERT INTO CATALOGO (SEDE_ID, PRO_ID) VALUES (1, 3);

-- USUARIOS
-- ADMIN_GENERAL: puede crear sedes y usuarios de tipo ADMIN_LOCAL
INSERT INTO USUARIO (
    USU_PRIMER_NOMBRE, USU_PRIMER_APELLIDO, USU_CORREO,
    USU_USERNAME, USU_CLAVE_HASH, USU_ROL
) VALUES (
    'Laura', 'Gómez', 'laura.gomez@pos.com', 'admin_general', 'clave123', 'ADMIN_GENERAL'
);

-- ADMIN_LOCAL: asociado a la sede central
INSERT INTO USUARIO (
    USU_PRIMER_NOMBRE, USU_PRIMER_APELLIDO, USU_CORREO,
    USU_USERNAME, USU_CLAVE_HASH, USU_ROL, SEDE_ID
) VALUES (
    'Carlos', 'Martínez', 'carlos.martinez@pos.com', 'admin_local', 'clave123', 'ADMIN_LOCAL', 1
);

-- CAJERO: también en la sede central
INSERT INTO USUARIO (
    USU_PRIMER_NOMBRE, USU_PRIMER_APELLIDO, USU_CORREO,
    USU_USERNAME, USU_CLAVE_HASH, USU_ROL, SEDE_ID
) VALUES (
    'Ana', 'Rojas', 'ana.rojas@pos.com', 'cajero_1', 'clave123', 'CAJERO', 1
);

-- CAJA (asociada a la sede central)
INSERT INTO CAJA (CAJA_FECHA_APERTURA, CAJA_MONTO_INICIAL, SEDE_ID)
VALUES (SYSDATE, 200000, 1);

--------------------------------------------------------
-- VERIFICACIÓN RÁPIDA
--------------------------------------------------------
SELECT 'SEDE' AS TABLA, COUNT(*) AS TOTAL FROM SEDE
UNION ALL SELECT 'CATEGORIA_PRODUCTO', COUNT(*) FROM CATEGORIA_PRODUCTO
UNION ALL SELECT 'PRODUCTO', COUNT(*) FROM PRODUCTO
UNION ALL SELECT 'CATALOGO', COUNT(*) FROM CATALOGO
UNION ALL SELECT 'USUARIO', COUNT(*) FROM USUARIO
UNION ALL SELECT 'CAJA', COUNT(*) FROM CAJA;
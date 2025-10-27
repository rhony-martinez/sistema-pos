CREATE OR REPLACE FUNCTION fn_inactivar_sede(p_sede_id IN NUMBER)
RETURN VARCHAR2
IS
BEGIN
  UPDATE Sede
     SET sede_estado = 'INACTIVA'
   WHERE sede_id = p_sede_id
     AND NVL(sede_estado, 'ACTIVA') <> 'INACTIVA';

  IF SQL%ROWCOUNT > 0 THEN
    RETURN 'Sede inactivada correctamente.';
  ELSE
    RETURN 'La sede ya estaba inactiva o no existe.';
  END IF;
EXCEPTION
  WHEN OTHERS THEN
    RETURN 'Error: ' || SQLERRM;
END;